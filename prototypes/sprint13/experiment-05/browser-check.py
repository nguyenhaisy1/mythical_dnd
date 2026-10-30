"""Actual local-browser 3D presentation checks; requires Playwright and Chrome.

This verifies geometry/cameras and UI behavior, not Unity gameplay, visual taste,
performance targets, historical accuracy, or project-owner approval.
"""
from contextlib import contextmanager
from functools import partial
from http.server import SimpleHTTPRequestHandler, ThreadingHTTPServer
from io import BytesIO
from pathlib import Path
from threading import Thread
import hashlib
import json
import math
import struct
from PIL import Image, ImageChops, ImageStat
from playwright.sync_api import sync_playwright

ROOT = Path(__file__).resolve().parents[3]
EXPERIMENT = ROOT / 'prototypes/sprint13/experiment-05'
OUT = ROOT / 'tests/results/sprint13-experiment05'
RENDERS = {'overview': 'render-overview.png', 'eye': 'render-eye.png', 'repair': 'render-repair.png'}


class QuietHandler(SimpleHTTPRequestHandler):
    def log_message(self, *_args):
        pass


@contextmanager
def local_server():
    handler = partial(QuietHandler, directory=str(ROOT / 'prototypes/sprint13'))
    server = ThreadingHTTPServer(('127.0.0.1', 0), handler)
    thread = Thread(target=server.serve_forever, daemon=True)
    thread.start()
    try:
        yield 'http://127.0.0.1:' + str(server.server_port)
    finally:
        server.shutdown()
        server.server_close()
        thread.join(timeout=5)


def snapshot(page):
    return page.evaluate('''() => ({ready: experimentDebug.ready, meshCount: experimentDebug.meshCount,
        triangleCount: experimentDebug.triangleCount, materialCount: experimentDebug.materialCount, textureCount: experimentDebug.textureCount, cameraFov: experimentDebug.cameraFov, cameraPosition: [...experimentDebug.cameraPosition],
        cameraTarget: [...experimentDebug.cameraTarget], currentPreset: experimentDebug.currentPreset,
        wireframe: experimentDebug.wireframe})''')


def distance(a, b):
    return math.sqrt(sum((x-y)**2 for x, y in zip(a, b)))


def canvas_pixels(page):
    data = page.locator('#scene-canvas').screenshot()
    image = Image.open(BytesIO(data)).convert('RGB')
    assert sum(ImageStat.Stat(image).stddev) > 6, '3D canvas is blank or nearly uniform'
    return data, image


def wait_camera_change(page, previous):
    page.wait_for_function('''previous => window.experimentDebug &&
        Math.hypot(...experimentDebug.cameraPosition.map((x,i) => x-previous[i])) > 0.01''', arg=previous)


def no_overflow(page):
    assert page.evaluate('document.documentElement.scrollWidth <= innerWidth'), 'Horizontal page overflow'


def full_view(page, key, width):
    trigger = page.locator(f'[data-render="{key}"]').first if key in RENDERS else page.locator('#reference-image')
    trigger.click()
    dialog = page.locator('#image-dialog')
    assert dialog.evaluate('(el) => el.open'), 'Image dialog did not open'
    image = page.locator('#dialog-image')
    image.evaluate('(el) => el.decode()')
    meta = image.evaluate('''el => ({width:el.naturalWidth,height:el.naturalHeight,
        fit:getComputedStyle(el).objectFit,source:el.getAttribute('src')})''')
    assert meta['width'] > 0 and meta['height'] > 0
    assert meta['fit'] != 'cover', 'Full image crops source'
    if key in RENDERS:
        assert meta['source'].endswith(RENDERS[key]), 'Wrong Blender render shown'
    page.screenshot(path=str(OUT / f'full-{key}-{width}.png'), full_page=True)
    page.keyboard.press('Escape')
    assert not dialog.evaluate('(el) => el.open')
    assert trigger.evaluate('(el) => el === document.activeElement'), 'Dialog did not restore focus'
    return meta


def world_positions(data, position_data):
    """Apply glTF scene transforms before testing volume; planes may form a 3D assembly."""
    identity = [[1 if row == col else 0 for col in range(4)] for row in range(4)]
    def multiply(a, b):
        return [[sum(a[r][k]*b[k][c] for k in range(4)) for c in range(4)] for r in range(4)]
    def local_matrix(node):
        if 'matrix' in node:
            values = node['matrix']
            assert len(values) == 16, 'Invalid node matrix'
            return [[values[col*4+row] for col in range(4)] for row in range(4)]
        x,y,z,w = node.get('rotation', [0,0,0,1])
        sx,sy,sz = node.get('scale', [1,1,1])
        tx,ty,tz = node.get('translation', [0,0,0])
        assert abs(x*x+y*y+z*z+w*w-1) < 1e-4, 'Node rotation is not a unit quaternion'
        return [[(1-2*(y*y+z*z))*sx,2*(x*y-z*w)*sy,2*(x*z+y*w)*sz,tx],
                [2*(x*y+z*w)*sx,(1-2*(x*x+z*z))*sy,2*(y*z-x*w)*sz,ty],
                [2*(x*z-y*w)*sx,2*(y*z+x*w)*sy,(1-2*(x*x+y*y))*sz,tz],
                [0,0,0,1]]
    nodes = data.get('nodes', [])
    scenes = data.get('scenes', [])
    if scenes:
        roots = scenes[data.get('scene', 0)].get('nodes', [])
    else:
        children = {c for node in nodes for c in node.get('children', [])}
        roots = [i for i in range(len(nodes)) if i not in children]
    points = []
    def visit(index, parent, ancestors):
        assert index not in ancestors and 0 <= index < len(nodes), 'Invalid/cyclic scene graph'
        node = nodes[index]
        matrix = multiply(parent, local_matrix(node))
        assert all(math.isfinite(v) for row in matrix for v in row), 'Nonfinite node transform'
        if 'mesh' in node:
            for primitive in data['meshes'][node['mesh']].get('primitives', []):
                for point in position_data[primitive['attributes']['POSITION']]:
                    vector = (*point, 1)
                    points.append(tuple(sum(matrix[row][col]*vector[col] for col in range(4)) for row in range(3)))
        for child in node.get('children', []):
            visit(child, matrix, ancestors | {index})
    for root in roots:
        visit(root, identity, set())
    return points


def glb_structure():
    """Inspect actual POSITION bytes, allowing a valid single-mesh Hyper3D asset."""
    raw = (EXPERIMENT / 'assets/ben-vac.glb').read_bytes()
    assert len(raw) >= 20, 'GLB is truncated'
    magic, version, size = struct.unpack_from('<4sII', raw)
    assert magic == b'glTF' and version == 2 and size == len(raw), 'Invalid GLB2 container'
    chunks, offset = {}, 12
    while offset < len(raw):
        assert offset + 8 <= len(raw), 'Truncated GLB chunk header'
        length, kind = struct.unpack_from('<II', raw, offset)
        offset += 8
        assert offset + length <= len(raw), 'Truncated GLB chunk payload'
        assert kind not in chunks, 'Duplicate GLB chunk kind'
        chunks[kind] = raw[offset:offset+length]
        offset += length
    assert 0x4E4F534A in chunks and 0x004E4942 in chunks, 'GLB lacks JSON/BIN scene chunks'
    data = json.loads(chunks[0x4E4F534A].decode().rstrip(' \x00'))
    binary = chunks[0x004E4942]
    meshes = data.get('meshes', [])
    primitives = [p for m in meshes for p in m.get('primitives', [])]
    assert primitives and data.get('nodes'), 'Missing model geometry/nodes'
    assert len(data.get('buffers', [])) == 1 and not data['buffers'][0].get('uri'), 'Expected self-contained GLB buffer'
    assert data['buffers'][0]['byteLength'] <= len(binary), 'GLB buffer exceeds BIN chunk'
    assert all(not image.get('uri') or image['uri'].startswith('data:') for image in data.get('images', [])), 'Model textures reference external files or URLs'
    accessors, views = data.get('accessors', []), data.get('bufferViews', [])
    component_formats = {5120:('b',1),5121:('B',1),5122:('h',2),5123:('H',2),5125:('I',4),5126:('f',4)}
    position_data, triangle_count = {}, 0
    for primitive in primitives:
        assert primitive.get('mode', 4) == 4, 'Expected triangulated model geometry'
        position_index = primitive.get('attributes', {}).get('POSITION')
        assert position_index is not None, 'Mesh has no POSITION accessor'
        accessor = accessors[position_index]
        count = accessor['count']
        index_count = accessors[primitive['indices']]['count'] if 'indices' in primitive else count
        assert index_count % 3 == 0, 'Triangle index count is not divisible by three'
        triangle_count += index_count // 3
        if position_index in position_data:
            continue
        position_data[position_index] = []
        assert accessor.get('type') == 'VEC3' and count > 0, 'Invalid POSITION shape'
        assert 'sparse' not in accessor and 'bufferView' in accessor, 'Decode compressed/sparse POSITION before structural checks'
        view = views[accessor['bufferView']]
        assert view.get('buffer', 0) == 0, 'POSITION references an external buffer'
        assert accessor['componentType'] in component_formats, 'Unsupported POSITION component type'
        fmt, component_size = component_formats[accessor['componentType']]
        element_size = component_size * 3
        stride = view.get('byteStride', element_size)
        assert stride >= element_size, 'Invalid POSITION stride'
        local_start = accessor.get('byteOffset', 0)
        assert local_start + (count-1)*stride + element_size <= view['byteLength'], 'POSITION exceeds buffer view'
        view_start = view.get('byteOffset', 0)
        assert view_start + view['byteLength'] <= len(binary), 'Buffer view exceeds BIN payload'
        for index in range(count):
            xyz = struct.unpack_from('<'+fmt*3, binary, view_start + local_start + index*stride)
            assert all(math.isfinite(v) for v in xyz), 'Nonfinite geometry coordinates'
            if accessor.get('normalized') and accessor['componentType'] != 5126:
                bits = component_size * 8
                if accessor['componentType'] in (5120, 5122):
                    xyz = tuple(max(-1, v/((1 << (bits-1))-1)) for v in xyz)
                else:
                    xyz = tuple(v/((1 << bits)-1) for v in xyz)
            position_data[position_index].append(xyz)
    points = world_positions(data, position_data)
    assert triangle_count > 100 and len(points) > 3, 'No substantive model geometry'
    lower = [min(p[axis] for p in points) for axis in range(3)]
    upper = [max(p[axis] for p in points) for axis in range(3)]
    extent = [hi-lo for lo, hi in zip(lower, upper)]
    diagonal = math.sqrt(sum(v*v for v in extent))
    assert diagonal > 0 and all(v > diagonal*1e-6 for v in extent), 'Model lacks genuine XYZ extent'
    def sub(a, b): return tuple(x-y for x,y in zip(a,b))
    def dot(a, b): return sum(x*y for x,y in zip(a,b))
    def cross(a, b): return (a[1]*b[2]-a[2]*b[1],a[2]*b[0]-a[0]*b[2],a[0]*b[1]-a[1]*b[0])
    origin = points[0]
    p1 = max(points, key=lambda p: dot(sub(p,origin), sub(p,origin)))
    line = sub(p1, origin)
    p2 = max(points, key=lambda p: dot(cross(line,sub(p,origin)), cross(line,sub(p,origin))))
    normal = cross(line, sub(p2,origin))
    normal_length = math.sqrt(dot(normal,normal))
    assert normal_length > diagonal*diagonal*1e-9, 'Model vertices are collinear'
    max_plane_distance = max(abs(dot(sub(p,origin),normal))/normal_length for p in points)
    assert max_plane_distance > diagonal*1e-6, 'Model vertices form a flat billboard'
    return {'mesh_definitions':len(meshes), 'primitive_count':len(primitives),
            'material_count':len(data.get('materials', [])), 'position_count':len(points),
            'triangle_count':triangle_count, 'bounds':{'min':lower,'max':upper,'extent':extent},
            'nonplanarity_distance':max_plane_distance, 'bytes':len(raw),
            'sha256':hashlib.sha256(raw).hexdigest()}


def main():
    required = ['index.html', 'assets/scene-data.js', 'assets/ben-vac.glb'] + ['assets/'+v for v in RENDERS.values()]
    missing = [x for x in required if not (EXPERIMENT/x).is_file()]
    if missing:
        raise SystemExit('Not run: awaiting artifacts: '+', '.join(missing))
    OUT.mkdir(parents=True, exist_ok=True)
    results = []
    structure = glb_structure()
    with sync_playwright() as p:
        browser = p.chromium.launch(executable_path='/usr/bin/google-chrome', headless=True,
            args=['--no-sandbox', '--enable-unsafe-swiftshader', '--use-angle=swiftshader', '--disable-dev-shm-usage'])
        for width, height in [(1280, 720), (1920, 1080)]:
            page = browser.new_page(viewport={'width':width,'height':height}, device_scale_factor=1)
            errors, warnings, remote_requests = [], [], []
            page.on('pageerror', lambda error: errors.append(str(error)))
            page.on('request', lambda request: remote_requests.append(request.url) if request.url.startswith(('https://','http://')) else None)
            page.on('console', lambda msg: warnings.append(msg.text) if msg.type == 'warning' else None)
            page.goto((EXPERIMENT/'index.html').as_uri(), wait_until='load')
            page.wait_for_function('window.experimentDebug && experimentDebug.ready === true', timeout=90000)
            assert page.locator('#status').get_attribute('data-state') == 'ready'
            initial = snapshot(page)
            assert initial['meshCount'] > 0 and initial['triangleCount'] > 100, 'No substantive scene geometry'
            preset_data, images = {}, {}
            for key in RENDERS:
                page.locator(f'[data-camera="{key}"]').click()
                page.wait_for_function('key => experimentDebug.currentPreset === key', arg=key)
                state = snapshot(page)
                expected = page.evaluate('(key) => BEN_VAC_SCENE.cameras[key]', key)
                assert distance(state['cameraPosition'], expected['position']) < 1e-7, 'Preset position differs from scene'
                assert distance(state['cameraTarget'], expected['target']) < 1e-7, 'Preset target differs from scene'
                assert abs(state['cameraFov'] - expected['fov']) < 1e-7, 'Preset ignores scene projection FOV'
                png, pixels = canvas_pixels(page)
                preset_data[key] = {'camera':state, 'canvas_sha256':hashlib.sha256(png).hexdigest()}
                images[key] = pixels
                (OUT/f'canvas-{key}-{width}.png').write_bytes(png)
                page.screenshot(path=str(OUT/f'view-{key}-{width}.png'), full_page=True)
            assert all(distance(preset_data[a]['camera']['cameraPosition'], preset_data[b]['camera']['cameraPosition']) > .1
                       for a,b in [('overview','eye'),('overview','repair'),('eye','repair')]), 'Preset positions repeat'
            assert len({x['canvas_sha256'] for x in preset_data.values()}) == 3, 'Camera does not change rendered image'
            assert all(ImageChops.difference(images[a], images[b]).getbbox() for a,b in [('overview','eye'),('eye','repair')])
            viewport = page.locator('#viewport')
            before_drag = snapshot(page)['cameraPosition']
            box = viewport.bounding_box()
            page.mouse.move(box['x']+box['width']*.5, box['y']+box['height']*.45)
            page.mouse.down()
            page.mouse.move(box['x']+box['width']*.64, box['y']+box['height']*.52, steps=8)
            page.mouse.up()
            wait_camera_change(page, before_drag)
            before_zoom = snapshot(page)['cameraPosition']
            page.mouse.wheel(0, -300)
            wait_camera_change(page, before_zoom)
            page.locator('#reset-camera').click()
            viewport.focus()
            before_key = snapshot(page)['cameraPosition']
            page.keyboard.press('ArrowLeft')
            wait_camera_change(page, before_key)
            page.keyboard.press('1')
            page.wait_for_function("experimentDebug.currentPreset === 'overview'")
            before_key_zoom = snapshot(page)['cameraPosition']
            page.keyboard.press('+')
            wait_camera_change(page, before_key_zoom)
            before_wireframe, _ = canvas_pixels(page)
            page.locator('#wireframe').check()
            assert snapshot(page)['wireframe'] is True
            wireframe_pixels, _ = canvas_pixels(page)
            assert wireframe_pixels != before_wireframe, 'Wireframe toggle does not change rendered pixels'
            page.screenshot(path=str(OUT/f'wireframe-{width}.png'), full_page=True)
            page.locator('#wireframe').uncheck()
            assert snapshot(page)['wireframe'] is False
            render_images = {key:full_view(page,key,width) for key in RENDERS}
            render_images['reference'] = full_view(page,'reference',width)
            page.select_option('#text-scale', '150')
            no_overflow(page)
            page.screenshot(path=str(OUT/f'text150-{width}.png'), full_page=True)
            assert not errors, 'JavaScript errors: '+repr(errors)
            assert not remote_requests, 'Offline viewer requested HTTP resources: '+repr(remote_requests)
            results.append({'viewport':[width,height], 'initial_geometry':initial,
                            'presets':preset_data,'images':render_images,'page_errors':errors,
                            'console_warnings':warnings,'remote_requests':remote_requests,
                            'checks':['GLB structure and nonplanar XYZ geometry','mesh and triangle counts','distinct camera poses and rendered pixels',
                                      'drag orbit','wheel zoom','reset','keyboard orbit/zoom/preset','wireframe',
                                      'Blender images decode','modal Escape focus','150% no horizontal overflow']})
            page.close()
        with local_server() as base:
            page = browser.new_page()
            response = page.goto(base+'/experiment-05/', wait_until='load')
            assert response and response.status == 200
            link = page.locator('a[href*="experiment-04/index.html"]').first
            assert link.count(), 'Missing comparison link to experiment04'
            link.click()
            page.wait_for_url('**/experiment-04/index.html')
            assert page.locator('#scene-canvas').count(), 'HTTP comparison did not reach experiment04'
            page.close()
        browser.close()
    result = {'scope':'3D prototype browser checks; not gameplay, benchmark, human playtest or approval',
              'status':'passed','glb':structure,'viewports':results,'http_reference04':True}
    (OUT/'browser.json').write_text(json.dumps(result,indent=2,ensure_ascii=False)+'\n')
    print(json.dumps({'status':'passed','viewports':len(results),'mesh_definitions':structure['mesh_definitions'],
                      'material_count':structure['material_count'],'evidence':str(OUT/'browser.json')}))


if __name__ == '__main__':
    main()
