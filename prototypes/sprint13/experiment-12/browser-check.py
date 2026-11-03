"""Check image provenance and gallery behavior, not game/art acceptance."""
from pathlib import Path
from urllib.parse import unquote, urlparse
import hashlib
import json
import platform
from datetime import datetime, timezone
from PIL import Image
from playwright.sync_api import sync_playwright

ROOT = Path(__file__).resolve().parents[3]
EXPERIMENT = Path(__file__).resolve().parent
OUT = ROOT / 'tests/results/sprint13-experiment12'
KEYS = ['clay-craft', 'clear-line', 'mineral-fresco', 'retro-prerender', 'charcoal-night', 'graphic-animation']
ORIGINALS = Path.home() / '.codex/generated_images/01a134a1-dcc6-7512-baf2-02751e56401a'


def no_overflow(page):
    assert page.evaluate('document.documentElement.scrollWidth <= innerWidth'), 'Horizontal page overflow'


def decoded(page, key):
    image = page.locator('#hero-image')
    image.evaluate('(el) => el.decode()')
    data = image.evaluate('''(el) => ({src: el.getAttribute('src'), width: el.naturalWidth,
        height: el.naturalHeight, fit: getComputedStyle(el).objectFit})''')
    assert data['src'] == f'assets/{key}.png', data
    assert (data['width'], data['height']) == (1672, 941), data
    assert data['fit'] == 'contain', 'Main artwork cropped'
    assert page.locator(f'.switches [data-choice="{key}"]').get_attribute('aria-pressed') == 'true'
    assert page.locator('.switches [aria-pressed="true"]').count() == 1
    return data


def main():
    started = datetime.now(timezone.utc).isoformat()
    OUT.mkdir(parents=True, exist_ok=True)
    provenance = json.loads((EXPERIMENT / 'provenance.json').read_text())
    prompts = json.loads((EXPERIMENT / 'prompts.json').read_text())
    assert [x['variant'] for x in provenance['images']] == KEYS
    assert [x['key'] for x in prompts['prompts']] == KEYS
    assert len({x['prompt'] for x in prompts['prompts']}) == 6
    assert provenance['tool'] == 'built-in image_gen'
    assert provenance['real_3d_model_created'] is False
    assert provenance['runtime_or_geometry_evidence'] is False
    assert provenance['reference_images'] == []
    assets = []
    for item in provenance['images']:
        file = EXPERIMENT / item['asset']
        assert item['asset'] == f"assets/{item['variant']}.png"
        content = file.read_bytes()
        sha = hashlib.sha256(content).hexdigest()
        assert sha == item['sha256']
        image = Image.open(file)
        image.verify()
        image = Image.open(file)
        assert image.size == (item['width'], item['height']) == (1672, 941)
        assert image.mode == item['mode'] == 'RGB'
        assert len(content) == item['bytes']
        original = ORIGINALS / item['source_generated_filename']
        assert original.is_file(), 'Original unavailable for this local provenance review'
        assert original.read_bytes() == content, 'Image differs from generated original'
        assets.append({'key': item['variant'], 'sha256': sha, 'bytes': len(content), 'original_bytes_equal': True})
    assert len({x['sha256'] for x in assets}) == 6
    results = []
    with sync_playwright() as playwright:
        browser = playwright.chromium.launch(executable_path='/usr/bin/google-chrome', headless=True)
        for width, height in [(1280, 720), (1920, 1080), (390, 844)]:
            page = browser.new_page(viewport={'width': width, 'height': height}, device_scale_factor=1)
            errors, remote, failed = [], [], []
            page.on('pageerror', lambda error: errors.append(str(error)))
            page.on('request', lambda request: remote.append(request.url) if request.url.startswith(('https://', 'http://')) else None)
            page.on('requestfailed', lambda request: failed.append(request.url))
            page.goto((EXPERIMENT / 'index.html').as_uri(), wait_until='load')
            # Off-viewport lazy images must load before decode/full-page capture.
            page.locator('.card-image img').evaluate_all(
                '(images) => Promise.all(images.map(img => {img.loading = \"eager\"; return img.decode();}))')
            no_overflow(page)
            assert page.locator('.switches [data-choice]').count() == 6
            assert page.locator('.card-image[data-choice]').count() == 6
            variants = {}
            for key in KEYS:
                page.locator(f'.switches [data-choice="{key}"]').click()
                variants[key] = decoded(page, key)
                thumbnail = page.locator(f'.card-image[data-choice="{key}"] img')
                thumbnail.evaluate('(el) => el.decode()')
                assert thumbnail.evaluate('(el) => el.naturalWidth === 1672 && el.naturalHeight === 941')
                assert thumbnail.evaluate('(el) => getComputedStyle(el).objectFit') == 'contain'
                page.locator('#scene').scroll_into_view_if_needed()
                no_overflow(page)
                page.screenshot(path=str(OUT / f'gallery-{key}-{width}.png'), full_page=True)
                page.locator('#enlarge').click()
                assert page.locator('#image-dialog').evaluate('(el) => el.open')
                page.locator('#dialog-image').evaluate('(el) => el.decode()')
                assert page.locator('#dialog-image').get_attribute('src') == f'assets/{key}.png'
                assert page.locator('#dialog-image').evaluate('(el) => getComputedStyle(el).objectFit') == 'contain'
                assert page.locator('#original-image').get_attribute('href') == f'assets/{key}.png'
                page.keyboard.press('Escape')
                assert not page.locator('#image-dialog').evaluate('(el) => el.open')
                assert page.locator('#enlarge').evaluate('(el) => el === document.activeElement')
            for shortcut, key in enumerate(KEYS, 1):
                page.keyboard.press(str(shortcut))
                decoded(page, key)
            for key in reversed(KEYS):
                page.locator(f'.card-image[data-choice="{key}"]').click()
                decoded(page, key)
            page.locator('#enlarge').click()
            page.locator('#close-dialog').click()
            assert not page.locator('#image-dialog').evaluate('(el) => el.open')
            assert page.locator('#enlarge').evaluate('(el) => el === document.activeElement')
            assert page.locator('.mock-hud,#show-hud,canvas,model-viewer').count() == 0
            page.locator('#text-size').select_option('150')
            no_overflow(page)
            page.screenshot(path=str(OUT / f'text150-{width}.png'), full_page=True)
            assert 'UI trong artwork là raster minh họa' in page.locator('.image-disclaimer').inner_text()
            assert 'không làm chữ trong ảnh reflow' in page.locator('.keyboard-hint').inner_text()
            library = page.locator('.reference-library')
            assert 'mọi phiên bản được giữ lại' in library.inner_text()
            assert 'không tự thay thế' in library.inner_text()
            history = library.locator('nav a')
            assert history.count() == 11
            for number in range(1, 12):
                assert history.nth(number - 1).get_attribute('href') == f'../experiment-{number:02}/index.html'
            assert not page.locator('html').get_attribute('data-art')
            assert 'image-rendering:pixelated' not in (EXPERIMENT / 'style.css').read_text()
            for link in page.locator('a[href]').evaluate_all('(els) => els.map(el => el.href)'):
                parsed = urlparse(link)
                assert parsed.scheme == 'file', 'Unexpected external link: ' + link
                assert Path(unquote(parsed.path)).is_file(), 'Broken local link: ' + link
            assert not errors, errors
            assert not remote, remote
            assert not failed, failed
            results.append({'viewport': [width, height], 'variants': variants,
                            'text150_no_horizontal_overflow': True, 'dialog_escape_close_focus': True,
                            'six_keyboard_and_six_card_switches': True, 'raster_board_without_html_hud': True,
                            'six_thumbnails_decoded_contain': True, 'original_link_matches_selected_board': True,
                            'reference_library_01_to_11': True, 'script_errors': errors,
                            'remote_requests': remote, 'failed_requests': failed})
            page.close()
        version = browser.version
        browser.close()
    report = {'status': 'passed', 'browser': version, 'exit_code': 0,
              'task_ids': ['S13-EXPERIMENT-12', 'SPIN-028'], 'utc_started': started,
              'os': platform.system(), 'architecture': platform.machine(),
              'runtime_version': platform.python_version(), 'unity_version': None, 'backend': 'Chrome headless',
              'evidence_images': [p.name for p in sorted(OUT.glob('*.png'))],
              'scope': 'PNG provenance and gallery presentation only; HTML150 does not reflow raster UI text; no art/gameplay/runtime/Unity/human approval',
              'assets': assets, 'viewports': results,
              'source_sha256': {name: hashlib.sha256((EXPERIMENT / name).read_bytes()).hexdigest()
                                for name in ['index.html', 'style.css', 'gallery.js', 'prompts.json', 'provenance.json']}}
    (OUT / 'browser.json').write_text(json.dumps(report, indent=2, ensure_ascii=False) + '\n')
    print(json.dumps({'status': 'passed', 'browser': version, 'viewports': len(results), 'assets': len(assets), 'evidence': str(OUT / 'browser.json')}))


if __name__ == '__main__':
    main()
