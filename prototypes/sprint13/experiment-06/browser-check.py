"""Browser checks for the image-only concept gallery; not game/art acceptance."""
from pathlib import Path
from urllib.parse import unquote, urlparse
import hashlib
import json

from PIL import Image
from playwright.sync_api import sync_playwright

ROOT = Path(__file__).resolve().parents[3]
EXPERIMENT = Path(__file__).resolve().parent
OUT = ROOT / 'tests/results/sprint13-experiment06'
ASSETS = {'a': 'a-oblique.png', 'b': 'b-grounded.png', 'c': 'c-sideview.png'}


def no_overflow(page):
    assert page.evaluate('document.documentElement.scrollWidth <= innerWidth'), 'Horizontal page overflow'


def decoded(page, key):
    image = page.locator('#hero-image')
    image.evaluate('(el) => el.decode()')
    data = image.evaluate('''(el) => ({src: el.getAttribute('src'), width: el.naturalWidth,
        height: el.naturalHeight, fit: getComputedStyle(el).objectFit})''')
    assert data['src'] == 'assets/' + ASSETS[key], data
    assert data['width'] > 1000 and data['height'] > 600, data
    assert data['fit'] != 'cover', 'Main artwork cropped'
    assert page.locator(f'.switches [data-choice="{key}"]').get_attribute('aria-pressed') == 'true'
    return data


def main():
    OUT.mkdir(parents=True, exist_ok=True)
    provenance = json.loads((EXPERIMENT / 'provenance.json').read_text())
    hashes = []
    for item in provenance['images']:
        file = EXPERIMENT / item['asset']
        content = file.read_bytes()
        sha = hashlib.sha256(content).hexdigest()
        assert sha == item['sha256']
        image = Image.open(file)
        assert image.size == (item['width'], item['height']) and image.mode == item['mode']
        assert len(content) == item['bytes']
        original = Path.home() / '.codex/generated_images/01a134a1-dcc6-7512-baf2-02751e56401a' / item['source_generated_filename']
        if original.is_file():
            assert original.read_bytes() == content, 'Workspace image differs from built-in generated original'
        hashes.append(sha)
    assert len(hashes) == len(set(hashes)) == 3, 'Expected three distinct raster outputs'
    assert provenance['real_3d_model_created'] is False
    results = []
    with sync_playwright() as playwright:
        browser = playwright.chromium.launch(executable_path='/usr/bin/google-chrome', headless=True)
        for width, height in [(1280, 720), (1920, 1080), (390, 844)]:
            page = browser.new_page(viewport={'width': width, 'height': height}, device_scale_factor=1)
            errors, remote = [], []
            page.on('pageerror', lambda error: errors.append(str(error)))
            page.on('request', lambda request: remote.append(request.url) if request.url.startswith(('https://', 'http://')) else None)
            page.goto((EXPERIMENT / 'index.html').as_uri(), wait_until='load')
            no_overflow(page)
            variants = {}
            for key in ASSETS:
                page.locator(f'.switches [data-choice="{key}"]').click()
                variants[key] = decoded(page, key)
                page.locator('#scene').scroll_into_view_if_needed()
                page.screenshot(path=str(OUT / f'gallery-{key}-{width}.png'), full_page=True)
                page.locator('#enlarge').click()
                assert page.locator('#image-dialog').evaluate('(el) => el.open')
                page.locator('#dialog-image').evaluate('(el) => el.decode()')
                assert page.locator('#dialog-image').get_attribute('src') == 'assets/' + ASSETS[key]
                assert page.locator('#dialog-image').evaluate('(el) => getComputedStyle(el).objectFit') == 'contain'
                page.keyboard.press('Escape')
                assert not page.locator('#image-dialog').evaluate('(el) => el.open')
                assert page.locator('#enlarge').evaluate('(el) => el === document.activeElement')
            for shortcut, key in [('1', 'a'), ('b', 'b'), ('3', 'c'), ('a', 'a'), ('2', 'b'), ('c', 'c')]:
                page.keyboard.press(shortcut)
                decoded(page, key)
            page.locator('.card-image[data-choice="a"]').click()
            decoded(page, 'a')
            toggle = page.locator('#show-hud')
            assert toggle.is_checked() and page.locator('.mock-hud').is_visible()
            toggle.uncheck()
            assert not page.locator('.mock-hud').is_visible(), 'HUD cannot be hidden for art comparison'
            decoded(page, 'a')
            page.screenshot(path=str(OUT / f'hud-off-{width}.png'), full_page=True)
            toggle.focus()
            page.keyboard.press('Space')
            assert toggle.is_checked() and page.locator('.mock-hud').is_visible(), 'Keyboard HUD toggle failed'
            page.locator('#text-size').select_option('150')
            no_overflow(page)
            page.screenshot(path=str(OUT / f'text150-{width}.png'), full_page=True)
            assert 'Chưa phải game chạy' in page.locator('.notice').inner_text()
            assert 'GIAO DIỆN MINH HỌA' in page.locator('.mock-hud').inner_text()
            assert page.locator('canvas,model-viewer').count() == 0
            assert page.locator('.mock-hud button, .mock-hud a').count() == 0
            links = page.locator('a[href]').evaluate_all('(els) => els.map(el => el.href)')
            for link in links:
                parsed = urlparse(link)
                assert parsed.scheme == 'file', 'Unexpected external gallery link: ' + link
                assert Path(unquote(parsed.path)).is_file(), 'Broken local link: ' + link
            assert not errors, errors
            assert not remote, remote
            results.append({'viewport': [width, height], 'variants': variants,
                            'text150_no_horizontal_overflow': True, 'dialog_escape_focus': True,
                            'keyboard_and_card_switch': True, 'hud_visibility_and_keyboard_toggle': True,
                            'script_errors': errors, 'remote_requests': remote})
            page.close()
        version = browser.version
        browser.close()
    report = {'status': 'passed', 'browser': version, 'scope': 'Raster provenance and browser presentation only; no art, gameplay, Unity or human approval',
              'sha256': hashes, 'viewports': results}
    (OUT / 'browser.json').write_text(json.dumps(report, indent=2, ensure_ascii=False) + '\n')
    print(json.dumps({'status': 'passed', 'viewports': len(results), 'evidence': str(OUT / 'browser.json')}))


if __name__ == '__main__':
    main()
