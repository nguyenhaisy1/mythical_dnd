"""Actual local-browser art-gallery checks. Requires Playwright and Google Chrome.

Automation verifies presentation/interaction only, not aesthetic quality, gameplay,
historical accuracy, or owner approval. Screenshots/evidence go to ignored results.
"""
from contextlib import contextmanager
from functools import partial
from http.server import SimpleHTTPRequestHandler, ThreadingHTTPServer
from pathlib import Path
from threading import Thread
import hashlib
import json
from playwright.sync_api import sync_playwright

ROOT = Path(__file__).resolve().parents[3]
EXPERIMENT = ROOT / 'prototypes/sprint13/experiment-03'
OUT = ROOT / 'tests/results/sprint13-experiment03'
ASSETS = {'a': 'a-overcast.png', 'b': 'b-gouache.png', 'c': 'c-woodcut.png'}


class QuietHandler(SimpleHTTPRequestHandler):
    def log_message(self, *_args):
        pass


@contextmanager
def local_server():
    handler = partial(QuietHandler, directory=str(ROOT / 'prototypes/sprint13'))
    server = ThreadingHTTPServer(('127.0.0.1', 0), handler)
    worker = Thread(target=server.serve_forever, daemon=True)
    worker.start()
    try:
        yield 'http://127.0.0.1:' + str(server.server_port)
    finally:
        server.shutdown()
        server.server_close()
        worker.join(timeout=5)


def no_overflow(page):
    assert page.evaluate('document.documentElement.scrollWidth <= innerWidth'), 'Page overflows horizontally'


def full_view(page, direction, width):
    trigger = page.locator(f'[data-open-art="{direction}"]:visible').first
    trigger.click()
    dialog = page.locator('#art-dialog')
    assert dialog.evaluate('(el) => el.open'), 'Full view did not open'
    picture = page.locator('#dialog-image')
    picture.evaluate('(el) => el.decode()')
    data = picture.evaluate('''(el) => ({width: el.naturalWidth, height: el.naturalHeight,
        objectFit: getComputedStyle(el).objectFit, src: el.getAttribute('src')})''')
    assert data['width'] > 0 and data['height'] > 0
    assert data['objectFit'] != 'cover', 'Full view may crop the artwork'
    if direction in ASSETS:
        assert data['src'].endswith(ASSETS[direction]), 'Wrong full-view direction'
    page.screenshot(path=str(OUT / f'full-{direction}-{width}.png'), full_page=True)
    page.keyboard.press('Escape')
    assert not dialog.evaluate('(el) => el.open')
    assert trigger.evaluate('(el) => el === document.activeElement'), 'Escape did not restore focus'
    return data


def main():
    missing = [filename for filename in ASSETS.values() if not (EXPERIMENT / 'assets' / filename).is_file()]
    if missing:
        raise SystemExit('Not run: generated assets missing: ' + ', '.join(missing))
    OUT.mkdir(parents=True, exist_ok=True)
    results = []
    with sync_playwright() as playwright:
        browser = playwright.chromium.launch(executable_path='/usr/bin/google-chrome', headless=True,
                                             args=['--no-sandbox'])
        for width, height in [(1280, 720), (1920, 1080)]:
            page = browser.new_page(viewport={'width': width, 'height': height}, device_scale_factor=1)
            errors = []
            page.on('pageerror', lambda error: errors.append(str(error)))
            page.goto((EXPERIMENT / 'index.html').as_uri(), wait_until='load')
            for filename in ASSETS.values():
                assert page.evaluate('''async (filename) => {
                    const image = new Image(); image.src = 'assets/' + filename;
                    try {await image.decode(); return image.naturalWidth > 0 && image.naturalHeight > 0;}
                    catch (_) {return false;}
                }''', filename), 'Undecodable asset: ' + filename
            page.locator('#view-single').click()
            for direction, filename in ASSETS.items():
                tab = page.locator(f'#direction-tabs button[data-direction="{direction}"]')
                tab.click()
                assert tab.get_attribute('aria-selected') == 'true'
                image = page.locator('#single-image')
                image.evaluate('(el) => el.decode()')
                assert image.get_attribute('src').endswith(filename), 'Direction selection did not update artwork'
                assert image.evaluate('(el) => getComputedStyle(el).filter') == 'none', 'Gallery applies a color filter'
                no_overflow(page)
                page.screenshot(path=str(OUT / f'single-{direction}-{width}.png'), full_page=True)
                full_view(page, direction, width)
            # Tabs remain a keyboard-operable choice rather than a mouse-only image swap.
            first = page.locator('#direction-tabs button[data-direction="a"]')
            first.click()
            page.keyboard.press('ArrowRight')
            assert page.locator('#direction-tabs button[data-direction="b"]').get_attribute('aria-selected') == 'true'
            page.keyboard.press('End')
            assert page.locator('#direction-tabs button[data-direction="c"]').get_attribute('aria-selected') == 'true'
            page.keyboard.press('Home')
            assert first.get_attribute('aria-selected') == 'true'
            page.locator('#view-compare').click()
            assert page.locator('#view-compare').get_attribute('aria-pressed') == 'true'
            for direction in ASSETS:
                assert page.locator(f'#comparison-grid img[data-art="{direction}"]').is_visible()
            page.locator('#include-reference').check()
            reference = page.locator('#comparison-grid img[data-art="reference"]')
            reference.evaluate('(el) => el.decode()')
            assert reference.is_visible()
            assert 'experiment-02' in reference.get_attribute('src')
            page.screenshot(path=str(OUT / f'compare-{width}.png'), full_page=True)
            full_view(page, 'reference', width)
            page.select_option('#text-scale', '150')
            no_overflow(page)
            page.screenshot(path=str(OUT / f'compare-150-{width}.png'), full_page=True)
            page.locator('#view-single').click()
            no_overflow(page)
            full_view(page, 'a', width)
            page.screenshot(path=str(OUT / f'single-150-{width}.png'), full_page=True)
            assert not errors, errors
            results.append({'viewport': [width, height], 'page_errors': errors,
                            'checks': ['three image decodes and actual selection',
                                       'full view without cover crop; Escape restores focus',
                                       'arrow/Home/End direction selection',
                                       'side-by-side three directions plus preserved reference02',
                                       'no gallery color filter', 'text150 no horizontal overflow']})
            page.close()
        with local_server() as origin:
            page = browser.new_page(viewport={'width': 1280, 'height': 720})
            page.goto(origin + '/experiment-03/', wait_until='load')
            link = page.locator('a[href*="experiment-02/index.html"]').first
            assert link.count() == 1
            with page.expect_response(lambda r: '/experiment-02/index.html' in r.url) as response:
                link.click()
            assert response.value.status == 200
            assert '/experiment-02/index.html' in page.url
            assert page.locator('img[src*="ben-vac-painterly.png"]').first.evaluate('(el) => el.complete && el.naturalWidth > 0')
            results.append({'http_compare_link': 'Experiment02 and its artwork accessible under documented server root'})
            page.close()
        browser.close()
    evidence = {'scope': 'Browser art-gallery interaction only; not gameplay/human playtest/user approval',
                'results': results, 'asset_sha256': {key: hashlib.sha256((EXPERIMENT / 'assets' / filename).read_bytes()).hexdigest()
                                                     for key, filename in ASSETS.items()}}
    (OUT / 'browser.json').write_text(json.dumps(evidence, ensure_ascii=False, indent=2) + '\n')
    print(json.dumps({'status': 'passed', 'viewports': 2, 'http_compare': True, 'evidence': str(OUT / 'browser.json')}))


if __name__ == '__main__':
    main()
