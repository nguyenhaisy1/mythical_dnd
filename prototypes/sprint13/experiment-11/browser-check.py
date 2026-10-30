"""Browser checks for the image-only concept gallery; not game/art acceptance."""
from pathlib import Path
from urllib.parse import unquote, urlparse
import hashlib
import json

from PIL import Image
from playwright.sync_api import sync_playwright

ROOT = Path(__file__).resolve().parents[3]
EXPERIMENT = Path(__file__).resolve().parent
OUT = ROOT / 'tests/results/sprint13-experiment11'
ASSETS = {'layered-25d': 'layered-25d.png', 'stylized-3d': 'stylized-3d.png', 'pixel-life': 'pixel-life.png', 'anime-life': 'anime-life.png'}


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
    assert len(hashes) == len(set(hashes)) == len(ASSETS), 'Expected four distinct raster outputs'
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
                assert page.locator('#original-image').get_attribute('href') == 'assets/' + ASSETS[key]
                assert Path(unquote(urlparse(page.locator('#original-image').get_attribute('href')).path)).suffix == '.png'
                assert page.locator(f'.card-image[data-choice="{key}"] img').evaluate('(el) => getComputedStyle(el).objectFit') == 'contain'
                page.keyboard.press('Escape')
                assert not page.locator('#image-dialog').evaluate('(el) => el.open')
                assert page.locator('#enlarge').evaluate('(el) => el === document.activeElement')
            for shortcut, key in [('1', 'layered-25d'), ('2', 'stylized-3d'), ('3', 'pixel-life'), ('4', 'anime-life')]:
                page.keyboard.press(shortcut)
                decoded(page, key)
            page.locator('.card-image[data-choice="layered-25d"]').click()
            decoded(page, 'layered-25d')
            assert page.locator('.mock-hud,#show-hud').count() == 0, 'HTML HUD obscures board UI'
            page.locator('#text-size').select_option('150')
            no_overflow(page)
            page.screenshot(path=str(OUT / f'text150-{width}.png'), full_page=True)
            assert 'UI trong ảnh là minh họa' in page.locator('.notice').inner_text()
            assert 'không làm chữ trong ảnh reflow' in page.locator('.keyboard-hint').inner_text()
            assert page.locator('canvas,model-viewer').count() == 0
            library = page.locator('.reference-library')
            assert 'mọi phiên bản được giữ lại' in library.inner_text()
            assert 'không tự thay thế hoặc đánh giá thấp những bản trước' in library.inner_text()
            history = library.locator('nav a')
            assert history.count() == 10
            for number in range(1, 11):
                assert history.nth(number - 1).get_attribute('href') == f'../experiment-{number:02}/index.html'
            assert not page.locator('html').get_attribute('data-art')
            assert 'image-rendering:pixelated' not in (EXPERIMENT / 'style.css').read_text()
            links = page.locator('a[href]').evaluate_all('(els) => els.map(el => el.href)')
            for link in links:
                parsed = urlparse(link)
                assert parsed.scheme == 'file', 'Unexpected external gallery link: ' + link
                assert Path(unquote(parsed.path)).is_file(), 'Broken local link: ' + link
            assert not errors, errors
            assert not remote, remote
            results.append({'viewport': [width, height], 'variants': variants,
                            'text150_no_horizontal_overflow': True, 'dialog_escape_focus': True,
                            'keyboard_and_card_switch': True, 'raster_board_without_html_hud': True, 'original_link_matches_selected_board': True, 'reference_library_01_to_10': True,
                            'script_errors': errors, 'remote_requests': remote})
            page.close()
        version = browser.version
        browser.close()
    report = {'status': 'passed', 'browser': version, 'scope': 'Raster provenance and gallery presentation only; HTML 150% does not scale/reflow raster game UI text; no art, game UI accessibility, gameplay, Unity or human approval',
              'sha256': hashes, 'viewports': results,
              'source_sha256': {name: hashlib.sha256((EXPERIMENT / name).read_bytes()).hexdigest() for name in ['index.html', 'style.css', 'gallery.js']}}
    (OUT / 'browser.json').write_text(json.dumps(report, indent=2, ensure_ascii=False) + '\n')
    print(json.dumps({'status': 'passed', 'viewports': len(results), 'evidence': str(OUT / 'browser.json')}))


if __name__ == '__main__':
    main()
