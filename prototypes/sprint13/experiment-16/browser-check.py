"""Check Experiment16 PNG provenance and gallery; not game or art acceptance."""
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
OUT = ROOT / 'tests/results/sprint13-experiment16'
ORIGINALS = Path.home() / '.codex/generated_images/01a134a1-dcc6-7512-baf2-02751e56401a'
KEYS = ['woven-world', 'line-volume', 'folded-village', 'rain-sculpt', 'living-section', 'voxel-life']
RETAINED = ['line-volume-v1', 'folded-village-v1']
SOURCES = ['index.html', 'style.css', 'gallery.js', 'prompts.json', 'provenance.json']


def source_hashes():
    return {name: hashlib.sha256((EXPERIMENT / name).read_bytes()).hexdigest() for name in SOURCES}


def check_provenance():
    provenance = json.loads((EXPERIMENT / 'provenance.json').read_text())
    prompts = json.loads((EXPERIMENT / 'prompts.json').read_text())
    assert provenance['tool'] == prompts['tool'] == 'built-in image_gen'
    assert provenance['real_3d_model_created'] is False
    assert provenance['runtime_or_geometry_evidence'] is False
    assert provenance['reference_images'] == prompts['reference_images'] == []
    assert provenance['conceptual_lineage_is_image_input'] is False
    assert len(provenance['images']) == 6 and len(provenance['retained_variants']) == 2
    assert [item['variant'] for item in provenance['images']] == KEYS
    assert [item['variant'] for item in provenance['retained_variants']] == RETAINED
    assert len(prompts['prompts']) == 6 and len(prompts['iterations']) == 2
    assert [item['key'] for item in prompts['prompts']] == KEYS
    assert [item['key'] for item in prompts['iterations']] == ['line-volume-v2', 'folded-village-v2']
    prompt_index = {(collection, item['key']): item
                    for collection in ['prompts', 'iterations'] for item in prompts[collection]}
    assert len(prompt_index) == len({item['prompt'] for item in prompt_index.values()}) == 8
    items = provenance['images'] + provenance['retained_variants']
    assert len({item['asset'] for item in items}) == 8
    used_prompts, assets, sizes = set(), [], {}
    for item in items:
        key = item['variant']
        assert item['asset'] == f'assets/{key}.png'
        file = EXPERIMENT / item['asset']
        assert file.resolve().parent == (EXPERIMENT / 'assets').resolve()
        content = file.read_bytes()
        sha = hashlib.sha256(content).hexdigest()
        assert sha == item['sha256'], key
        with Image.open(file) as bitmap:
            bitmap.verify()
        expected_size = (1672, 940 if key == 'line-volume' else 941)
        with Image.open(file) as bitmap:
            assert bitmap.size == (item['width'], item['height']) == expected_size, key
            assert bitmap.mode == item['mode'] == 'RGB', key
        sizes[key] = expected_size
        assert len(content) == item['bytes'], key
        assert item['displayed'] is (key in KEYS), key
        original_name = item['source_generated_filename']
        assert Path(original_name).name == original_name
        original = ORIGINALS / original_name
        assert original.is_file(), 'Original unavailable: ' + key
        assert original.read_bytes() == content, 'Original differs: ' + key
        prompt_id = (item['prompt_collection'], item['prompt_key'])
        assert prompt_id not in used_prompts, 'Prompt mapped twice'
        used_prompts.add(prompt_id)
        assert item['reference_assets'] == [], key
        # Prompt entries have no individual reference_assets field in this batch.
        # Check any explicit field if present; top-level references are empty.
        assert prompt_index[prompt_id].get('reference_assets', []) == [], key
        if key in ['line-volume', 'folded-village']:
            expected_prompt = ('iterations', key + '-v2')
        elif key in RETAINED:
            expected_prompt = ('prompts', key.removesuffix('-v1'))
        else:
            expected_prompt = ('prompts', key)
        assert prompt_id == expected_prompt, key
        assets.append({'key': key, 'sha256': sha, 'bytes': len(content), 'size': list(expected_size),
                       'mode': 'RGB', 'original_bytes_equal': True, 'prompt_id': list(prompt_id),
                       'reference_assets': [], 'displayed': key in KEYS})
    assert used_prompts == set(prompt_index)
    assert len({item['sha256'] for item in assets}) == 8
    assert sum(item['bytes'] for item in assets) == 16985863
    return assets, sizes


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


def modal(page, key, sizes):
    page.locator('#enlarge').click()
    assert page.locator('#image-dialog').evaluate('el => el.open')
    image = page.locator('#dialog-image')
    image.evaluate('el => el.decode()')
    assert image.get_attribute('src') == f'assets/{key}.png'
    assert image.evaluate('el => getComputedStyle(el).objectFit') == 'contain'
    assert image.evaluate('el => [el.naturalWidth, el.naturalHeight]') == list(sizes[key])
    assert page.locator('#original-image').get_attribute('href') == f'assets/{key}.png'
    page.keyboard.press('Escape')
    page.wait_for_function('!document.querySelector("#image-dialog").open && document.activeElement.id === "enlarge"')


def main():
    started = datetime.now(timezone.utc).isoformat()
    OUT.mkdir(parents=True, exist_ok=True)
    initial_hashes = source_hashes()
    assets, sizes = check_provenance()
    results = []
    with sync_playwright() as playwright:
        browser = playwright.chromium.launch(executable_path='/usr/bin/google-chrome', headless=True)
        for width, height in [(1280, 720), (1920, 1080), (390, 844)]:
            page = browser.new_page(viewport={'width': width, 'height': height}, device_scale_factor=1)
            page.set_default_timeout(20000)
            errors, remote, failed = [], [], []
            page.on('pageerror', lambda error: errors.append(str(error)))
            page.on('request', lambda request: remote.append(request.url)
                    if request.url.startswith(('https://', 'http://')) else None)
            page.on('requestfailed', lambda request: failed.append(request.url))
            page.goto((EXPERIMENT / 'index.html').as_uri(), wait_until='load')
            # Capture policy only: ensure below-fold lazy thumbnails decode for screenshots.
            page.locator('.card-image img').evaluate_all('''images => Promise.all(images.map(img => {
                img.loading = 'eager';
                return Promise.race([img.decode(), new Promise((_, reject) =>
                    setTimeout(() => reject(new Error('Thumbnail decode timeout: ' + img.src)), 15000))]);
            }))''')
            no_overflow(page)
            assert page.locator('.switches [data-choice]').evaluate_all('(els) => els.map(el => el.dataset.choice)') == KEYS
            assert page.locator('.cards .card').count() == 6
            assert page.locator('.card-image[data-choice]').count() == 6
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
                modal(page, key, sizes)
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
            text_sizes = {}
            for text in ['100', '125', '150']:
                page.locator('#text-size').select_option(text)
                no_overflow(page)
                text_sizes[text] = page.evaluate('parseFloat(getComputedStyle(document.documentElement).fontSize)')
            assert text_sizes['100'] < text_sizes['125'] < text_sizes['150'], text_sizes
            page.screenshot(path=str(OUT / f'text150-{width}.png'), full_page=True)
            assert page.locator('.reference-library nav a').count() == 15
            for number in range(1, 16):
                link = page.locator('.reference-library nav a').nth(number - 1)
                assert link.get_attribute('href') == f'../experiment-{number:02}/index.html'
            retained = page.locator('.retained a')
            assert retained.count() == 2
            for index, key in enumerate(RETAINED):
                assert retained.nth(index).get_attribute('href') == f'assets/{key}.png'
                assert retained.nth(index).get_attribute('target') == '_blank'
                assert 'noopener' in retained.nth(index).get_attribute('rel').split()
            assert page.locator('.favorite-note').count() == 1
            for link in page.locator('a[href]').evaluate_all('els => els.map(el => el.href)'):
                parsed = urlparse(link)
                assert parsed.scheme == 'file', 'Unexpected external link: ' + link
                assert Path(unquote(parsed.path)).is_file(), 'Broken local link: ' + link
            assert not errors, errors
            assert not remote, remote
            assert not failed, failed
            results.append({'viewport': [width, height], 'variants': variants,
                            'text100_125_150_no_horizontal_overflow': True, 'root_font_px': text_sizes,
                            'dialog_escape_close_focus': True, 'six_keyboard_and_six_card_switches': True,
                            'six_direction_cards': True, 'six_thumbnails_decoded_contain': True,
                            'original_link_matches_selected_frame': True, 'reference_library_01_to_15': True,
                            'two_retained_variant_links': True, 'script_errors': errors,
                            'remote_requests': remote, 'failed_requests': failed})
            print(f'Viewport {width}x{height}: passed', flush=True)
            page.close()
        version = browser.version
        browser.close()
    assert source_hashes() == initial_hashes, 'Source changed during review run'
    report = {'status': 'passed', 'exit_code': 0,
              'scope': 'PNG copy/prompt provenance and gallery only; not art/gameplay/runtime/Unity/approval',
              'task_ids': ['S13-EXPERIMENT-16', 'SPIN-033'], 'utc_started': started,
              'os': platform.system(), 'architecture': platform.machine(), 'runtime_version': platform.python_version(),
              'unity_version': None, 'backend': 'Chrome headless', 'browser': version,
              'displayed_frames': 6, 'retained_variants': 2, 'assets': assets,
              'image_reference_count': 0, 'conceptual_lineage_is_image_input': False,
              'viewports': results, 'thumbnail_capture_policy': 'Set loading=eager then decode before full-page capture',
              'source_sha256': initial_hashes,
              'evidence_images': [p.name for p in sorted(OUT.glob('gallery-*.png'))]
                                 + [p.name for p in sorted(OUT.glob('text150-*.png'))]}
    (OUT / 'browser.json').write_text(json.dumps(report, ensure_ascii=False, indent=2) + '\n')
    print(json.dumps({'status': 'passed', 'browser': version, 'viewports': len(results),
                      'assets': len(assets), 'evidence': str(OUT / 'browser.json')}))


if __name__ == '__main__':
    main()
