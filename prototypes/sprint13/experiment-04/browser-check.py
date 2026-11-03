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
EXPERIMENT = ROOT / 'prototypes/sprint13/experiment-04'
OUT = ROOT / 'tests/results/sprint13-experiment04'
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
        triangleCount: experimentDebug.triangleCount, cameraFov: experimentDebug.cameraFov, cameraPosition: [...experimentDebug.cameraPosition],
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


def glb_structure():
    raw = (EXPERIMENT / 'assets/ben-vac.glb').read_bytes()
    magic, version, size = struct.unpack_from('<4sII', raw)
    assert magic == b'glTF' and version == 2 and size == len(raw), 'Invalid GLB2 container'
    length, kind = struct.unpack_from('<II', raw, 12)
    assert kind == 0x4E4F534A, 'GLB lacks JSON scene chunk'
    data = json.loads(raw[20:20+length].decode().rstrip(' \x00'))
    meshes = data.get('meshes', [])
    primitives = [p for m in meshes for p in m.get('primitives', [])]
    assert len(primitives) > 3, 'Export is not a multi-part scene'
    assert data.get('buffers') and data.get('nodes'), 'Missing model geometry/nodes'
    return {'mesh_definitions':len(meshes), 'primitive_count':len(primitives),
            'material_count':len(data.get('materials', [])), 'bytes':len(raw),
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
            errors, warnings = [], []
            page.on('pageerror', lambda error: errors.append(str(error)))
            page.on('console', lambda msg: warnings.append(msg.text) if msg.type == 'warning' else None)
            page.goto((EXPERIMENT/'index.html').as_uri(), wait_until='load')
            page.wait_for_function('window.experimentDebug && experimentDebug.ready === true', timeout=90000)
            assert page.locator('#status').get_attribute('data-state') == 'ready'
            initial = snapshot(page)
            assert initial['meshCount'] > 3 and initial['triangleCount'] > 100, 'No substantive scene geometry'
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
            page.locator('#wireframe').check()
            assert snapshot(page)['wireframe'] is True
            page.screenshot(path=str(OUT/f'wireframe-{width}.png'), full_page=True)
            page.locator('#wireframe').uncheck()
            assert snapshot(page)['wireframe'] is False
            render_images = {key:full_view(page,key,width) for key in RENDERS}
            render_images['reference'] = full_view(page,'reference',width)
            page.select_option('#text-scale', '150')
            no_overflow(page)
            page.screenshot(path=str(OUT/f'text150-{width}.png'), full_page=True)
            assert not errors, 'JavaScript errors: '+repr(errors)
            results.append({'viewport':[width,height], 'initial_geometry':initial,
                            'presets':preset_data,'images':render_images,'page_errors':errors,
                            'console_warnings':warnings,
                            'checks':['GLB structure','mesh and triangle counts','distinct camera poses and rendered pixels',
                                      'drag orbit','wheel zoom','reset','keyboard orbit/zoom/preset','wireframe',
                                      'Blender images decode','modal Escape focus','150% no horizontal overflow']})
            page.close()
        with local_server() as base:
            page = browser.new_page()
            response = page.goto(base+'/experiment-04/', wait_until='load')
            assert response and response.status == 200
            link = page.locator('a[href*="experiment-03/index.html"]').first
            assert link.count(), 'Missing comparison link to experiment03'
            link.click()
            page.wait_for_url('**/experiment-03/index.html')
            assert page.locator('#direction-tabs').count(), 'HTTP comparison did not reach experiment03'
            page.close()
        browser.close()
    result = {'scope':'3D prototype browser checks; not gameplay, benchmark, human playtest or approval',
              'status':'passed','glb':structure,'viewports':results,'http_reference03':True}
    (OUT/'browser.json').write_text(json.dumps(result,indent=2,ensure_ascii=False)+'\n')
    print(json.dumps({'status':'passed','viewports':len(results),'mesh_definitions':structure['mesh_definitions'],
                      'material_count':structure['material_count'],'evidence':str(OUT/'browser.json')}))


if __name__ == '__main__':
    main()
