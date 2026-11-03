"""Check PNG provenance/reference chains and gallery behavior, not game/art acceptance."""
from datetime import datetime, timezone
from pathlib import Path
from urllib.parse import unquote, urlparse
import hashlib
import json
import platform

from PIL import Image
from playwright.sync_api import sync_playwright

ROOT = Path(__file__).resolve().parents[3]
EXPERIMENT = Path(__file__).resolve().parent
OUT = ROOT / 'tests/results/sprint13-experiment13'
ORIGINALS = Path.home() / '.codex/generated_images/01a134a1-dcc6-7512-baf2-02751e56401a'
KEYS = ['block-world', 'block-dialogue', 'frieze-world', 'frieze-dialogue', 'near-world', 'near-dialogue']
SOURCES = ['index.html', 'style.css', 'gallery.js', 'prompts.json', 'provenance.json']


def source_hashes():
    return {name: hashlib.sha256((EXPERIMENT / name).read_bytes()).hexdigest() for name in SOURCES}


def check_provenance():
    provenance = json.loads((EXPERIMENT / 'provenance.json').read_text())
    prompts = json.loads((EXPERIMENT / 'prompts.json').read_text())
    assert provenance['tool'] == 'built-in image_gen'
    assert provenance['real_3d_model_created'] is False
    assert provenance['runtime_or_geometry_evidence'] is False
    assert provenance['external_reference_images'] == []
    assert len(provenance['images']) == 6 and len(provenance['retained_variants']) == 1
    assert set(x['variant'] for x in provenance['images']) == set(KEYS)
    assert [x['variant'] for x in provenance['retained_variants']] == ['frieze-world-v1']
    assert len(prompts['prompts']) == 6 and len(prompts['iterations']) == 1
    assert set(x['key'] for x in prompts['prompts']) == set(KEYS)
    assert [x['key'] for x in prompts['iterations']] == ['frieze-world-v2']
    prompt_index = {(collection, item['key']): item for collection in ['prompts', 'iterations']
                    for item in prompts[collection]}
    assert len(prompt_index) == 7 and len({x['prompt'] for x in prompt_index.values()}) == 7
    items = provenance['images'] + provenance['retained_variants']
    by_asset = {x['asset']: x for x in items}
    assert len(by_asset) == 7
    graph = {}
    assets = []
    used_prompts = set()
    for item in items:
        key = item['variant']
        assert item['asset'] == f'assets/{key}.png'
        file = EXPERIMENT / item['asset']
        assert file.resolve().parent == (EXPERIMENT / 'assets').resolve()
        content = file.read_bytes()
        sha = hashlib.sha256(content).hexdigest()
        assert sha == item['sha256'], key
        with Image.open(file) as image:
            image.verify()
        with Image.open(file) as image:
            size = (1671, 941) if key == 'frieze-world' else (1672, 941)
            assert image.size == (item['width'], item['height']) == size, key
            assert image.mode == item['mode'] == 'RGB', key
        assert len(content) == item['bytes'], key
        original_name = item['source_generated_filename']
        assert Path(original_name).name == original_name
        original = ORIGINALS / original_name
        assert original.is_file(), 'Original unavailable: ' + key
        assert original.read_bytes() == content, 'Original differs: ' + key
        prompt_id = (item['prompt_collection'], item['prompt_key'])
        assert prompt_id not in used_prompts, 'Prompt mapped twice'
        used_prompts.add(prompt_id)
        prompt = prompt_index[prompt_id]
        assert item['reference_assets'] == prompt['reference_assets'], key
        assert len(set(item['reference_assets'])) == len(item['reference_assets'])
        assert set(item['reference_sha256']) == set(item['reference_assets']), key
        for ref in item['reference_assets']:
            assert ref in by_asset, 'Unknown reference: ' + ref
            ref_sha = hashlib.sha256((EXPERIMENT / ref).read_bytes()).hexdigest()
            assert item['reference_sha256'][ref] == ref_sha == by_asset[ref]['sha256'], ref
        graph[item['asset']] = item['reference_assets']
        assets.append({'key': key, 'sha256': sha, 'bytes': len(content),
                       'size': [item['width'], item['height']], 'mode': item['mode'],
                       'original_bytes_equal': True, 'prompt_id': list(prompt_id),
                       'reference_assets': item['reference_assets'],
                       'reference_hashes_match': True,
                       'displayed': key in KEYS})
    assert used_prompts == set(prompt_index)
    assert len({x['sha256'] for x in assets}) == 7
    visited, active = set(), set()

    def visit(asset):
        assert asset not in active, 'Reference cycle: ' + asset
        if asset in visited:
            return
        active.add(asset)
        for reference in graph[asset]:
            visit(reference)
        active.remove(asset)
        visited.add(asset)

    for asset in graph:
        visit(asset)
    assert len(visited) == 7
    return assets, {x['variant']: (x['width'], x['height']) for x in items}, graph


def no_overflow(page):
    assert page.evaluate('document.documentElement.scrollWidth <= innerWidth'), 'Horizontal page overflow'


def decoded(page, key, sizes):
    image = page.locator('#hero-image')
    image.evaluate('(el) => el.decode()')
    data = image.evaluate('''el => ({src: el.getAttribute('src'), width: el.naturalWidth,
        height: el.naturalHeight, fit: getComputedStyle(el).objectFit})''')
    assert data['src'] == f'assets/{key}.png', data
    assert (data['width'], data['height']) == sizes[key], data
    assert data['fit'] == 'contain', 'Main artwork cropped'
    assert page.locator(f'.switches [data-choice="{key}"]').get_attribute('aria-pressed') == 'true'
    assert page.locator('.switches [aria-pressed="true"]').count() == 1
    assert page.locator('#image-error').is_hidden()
    return data


def modal(page, key):
    page.locator('#enlarge').click()
    assert page.locator('#image-dialog').evaluate('el => el.open')
    page.locator('#dialog-image').evaluate('el => el.decode()')
    assert page.locator('#dialog-image').get_attribute('src') == f'assets/{key}.png'
    assert page.locator('#dialog-image').evaluate('el => getComputedStyle(el).objectFit') == 'contain'
    assert page.locator('#original-image').get_attribute('href') == f'assets/{key}.png'
    page.keyboard.press('Escape')
    page.wait_for_function('!document.querySelector("#image-dialog").open && document.activeElement.id === "enlarge"')


def main():
    started = datetime.now(timezone.utc).isoformat()
    OUT.mkdir(parents=True, exist_ok=True)
    initial_hashes = source_hashes()
    assets, sizes, graph = check_provenance()
    results = []
    with sync_playwright() as playwright:
        browser = playwright.chromium.launch(executable_path='/usr/bin/google-chrome', headless=True)
        for width, height in [(1280, 720), (1920, 1080), (390, 844)]:
            page = browser.new_page(viewport={'width': width, 'height': height}, device_scale_factor=1)
            page.set_default_timeout(20000)
            errors, remote, failed = [], [], []
            page.on('pageerror', lambda error: errors.append(str(error)))
            page.on('request', lambda request: remote.append(request.url) if request.url.startswith(('https://', 'http://')) else None)
            page.on('requestfailed', lambda request: failed.append(request.url))
            page.goto((EXPERIMENT / 'index.html').as_uri(), wait_until='load')
            # Make off-screen lazy thumbnails load before full-page evidence capture.
            # This only changes the capture harness loading policy, not product source.
            page.locator('.card-image img').evaluate_all('''images => Promise.all(images.map(img => {
                img.loading = 'eager';
                return Promise.race([img.decode(), new Promise((_, reject) =>
                    setTimeout(() => reject(new Error('Thumbnail decode timeout: ' + img.src)), 15000))]);
            }))''')
            no_overflow(page)
            assert page.locator('.switches [data-choice]').evaluate_all('(els) => els.map(el => el.dataset.choice)') == KEYS
            assert page.locator('.cards .card').count() == 3
            assert page.locator('.card-image[data-choice]').count() == 6
            for direction in ['block', 'frieze', 'near']:
                assert page.locator(f'.card[data-direction="{direction}"] .card-image').count() == 2
            variants = {}
            for key in KEYS:
                page.locator(f'.switches [data-choice="{key}"]').click()
                variants[key] = decoded(page, key, sizes)
                thumb = page.locator(f'.card-image[data-choice="{key}"] img')
                thumb_data = thumb.evaluate('el => ({width: el.naturalWidth, height: el.naturalHeight, fit: getComputedStyle(el).objectFit})')
                assert (thumb_data['width'], thumb_data['height']) == sizes[key]
                assert thumb_data['fit'] == 'contain'
                page.locator('#scene').scroll_into_view_if_needed()
                no_overflow(page)
                page.screenshot(path=str(OUT / f'gallery-{key}-{width}.png'), full_page=True)
                modal(page, key)
            for shortcut, key in enumerate(KEYS, 1):
                page.keyboard.press(str(shortcut))
                decoded(page, key, sizes)
            for key in reversed(KEYS):
                page.locator(f'.card-image[data-choice="{key}"]').click()
                decoded(page, key, sizes)
            page.locator('#enlarge').click()
            page.locator('#close-dialog').click()
            page.wait_for_function('!document.querySelector("#image-dialog").open && document.activeElement.id === "enlarge"')
            assert page.locator('.mock-hud,#show-hud,canvas,model-viewer').count() == 0
            page.locator('#text-size').select_option('150')
            no_overflow(page)
            page.screenshot(path=str(OUT / f'text150-{width}.png'), full_page=True)
            assert page.locator('.reference-library nav a').count() == 12
            for number in range(1, 13):
                link = page.locator('.reference-library nav a').nth(number - 1)
                assert link.get_attribute('href') == f'../experiment-{number:02}/index.html'
            retained = page.locator('.retained a')
            assert retained.count() == 1
            assert retained.get_attribute('href') == 'assets/frieze-world-v1.png'
            assert retained.get_attribute('target') == '_blank'
            for link in page.locator('a[href]').evaluate_all('els => els.map(el => el.href)'):
                parsed = urlparse(link)
                assert parsed.scheme == 'file', 'Unexpected external link: ' + link
                assert Path(unquote(parsed.path)).is_file(), 'Broken local link: ' + link
            assert not errors, errors
            assert not remote, remote
            assert not failed, failed
            results.append({'viewport': [width, height], 'variants': variants,
                            'text100_and_150_no_horizontal_overflow': True,
                            'dialog_escape_close_focus': True, 'six_keyboard_and_six_card_switches': True,
                            'three_directions_two_frames_each': True, 'six_thumbnails_decoded_contain': True,
                            'original_link_matches_selected_frame': True, 'reference_library_01_to_12': True,
                            'retained_variant_link': True, 'script_errors': errors,
                            'remote_requests': remote, 'failed_requests': failed})
            print(f'Viewport {width}x{height}: passed', flush=True)
            page.close()
        version = browser.version
        browser.close()
    assert source_hashes() == initial_hashes, 'Source changed during review run'
    report = {'status': 'passed', 'exit_code': 0, 'scope': 'PNG copy/reference provenance and gallery only; not art/gameplay/runtime/Unity/approval',
              'task_ids': ['S13-EXPERIMENT-13', 'SPIN-030'], 'utc_started': started,
              'os': platform.system(), 'architecture': platform.machine(), 'runtime_version': platform.python_version(),
              'unity_version': None, 'backend': 'Chrome headless', 'browser': version,
              'displayed_frames': 6, 'retained_variants': 1, 'assets': assets,
              'reference_graph_acyclic': True, 'reference_graph': graph,
              'viewports': results, 'thumbnail_capture_policy': 'Set loading=eager then decode before full-page capture',
              'source_sha256': initial_hashes,
              'evidence_images': [p.name for p in sorted(OUT.glob('*.png'))]}
    (OUT / 'browser.json').write_text(json.dumps(report, ensure_ascii=False, indent=2) + '\n')
    print(json.dumps({'status': 'passed', 'browser': version, 'viewports': len(results),
                      'assets': len(assets), 'evidence': str(OUT / 'browser.json')}))


if __name__ == '__main__':
    main()
