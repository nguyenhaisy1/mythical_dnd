"""Local browser UX regression. Not runtime gameplay or human acceptance."""
from pathlib import Path
import json
from playwright.sync_api import sync_playwright

root = Path(__file__).resolve().parents[3]
experiment = root / 'prototypes/sprint13/experiment-02'
out = root / 'tests/results/sprint13-experiment02'
out.mkdir(parents=True, exist_ok=True)
checks = []
with sync_playwright() as p:
    browser = p.chromium.launch(executable_path='/usr/bin/google-chrome', headless=True, args=['--no-sandbox'])
    for width, height in [(1280, 720), (1920, 1080)]:
        page = browser.new_page(viewport={'width': width, 'height': height}, device_scale_factor=1)
        errors = []
        page.on('pageerror', lambda error: errors.append(str(error)))
        page.goto((experiment / 'index.html').as_uri())
        page.wait_for_load_state('load')
        compare = page.locator('a[href*="experiment-01/index.html"]')
        assert compare.count() > 0, 'Missing link to preserved Experiment01'
        assert (experiment / compare.first.get_attribute('href')).resolve().is_file()
        # Decode actual packaged raster independently of its CSS/HTML presentation.
        assert page.evaluate('''async () => {
            const image = new Image(); image.src = 'assets/ben-vac-painterly.png';
            try { await image.decode(); return image.naturalWidth >= 1024 && image.naturalHeight >= 512; }
            catch (_) { return false; }
        }'''), 'Scene raster missing or undecodable'
        assert page.evaluate('''() => Array.from(document.querySelectorAll('*')).some(element =>
            (element.tagName === 'IMG' && element.src.includes('ben-vac-painterly.png')) ||
            getComputedStyle(element).backgroundImage.includes('ben-vac-painterly.png'))'''), 'Raster is not used in page'
        page.screenshot(path=str(out / f'menu-{width}.png'), full_page=True)
        page.click('#menu-art')
        assert page.locator('#art-dialog').evaluate('(dialog) => dialog.open')
        page.screenshot(path=str(out / f'art-{width}.png'), full_page=True)
        page.keyboard.press('Escape')
        assert page.evaluate('document.activeElement.id') == 'menu-art'
        page.click('#new-campaign')
        page.wait_for_selector('#reveal:not([hidden])')
        assert page.locator('.skill-sources li').count() > 0
        name = page.locator('#character-name').inner_text()
        assert name.strip()
        assert page.locator('#character-biography').inner_text().strip()
        page.screenshot(path=str(out / f'reveal-{width}.png'), full_page=True)
        page.click('#begin')
        assert page.locator('#world-name').inner_text() == name
        page.screenshot(path=str(out / f'world-{width}.png'), full_page=True)
        page.click('#preview-action')
        assert page.locator('#action-dialog').evaluate('(dialog) => dialog.open')
        page.screenshot(path=str(out / f'preview-{width}.png'), full_page=True)
        page.keyboard.press('Escape')
        assert not page.locator('#action-dialog').evaluate('(dialog) => dialog.open')
        assert page.evaluate('document.activeElement.id') == 'preview-action'
        assert page.locator('#result-status').inner_text() == ''
        page.keyboard.press('j')
        assert page.locator('#journal-dialog').evaluate('(dialog) => dialog.open')
        assert page.locator('#claims .claim').count() == 2
        assert all(text.strip() for text in page.locator('#claims .claim small').all_inner_texts())
        page.screenshot(path=str(out / f'journal-{width}.png'), full_page=True)
        page.keyboard.press('Escape')
        assert page.evaluate('document.activeElement.id') == 'preview-action'
        page.select_option('#text-scale', '150')
        assert page.evaluate('document.documentElement.scrollWidth <= innerWidth'), 'Horizontal overflow at 150%'
        page.screenshot(path=str(out / f'world-150-{width}.png'), full_page=True)
        assert page.locator('#world-name').inner_text() == name
        page.click('#preview-action')
        assert page.locator('#action-dialog').evaluate('(dialog) => dialog.open')
        page.keyboard.press('Escape')
        assert not errors, errors
        checks.append({'viewport': [width, height], 'errors': errors, 'checks': [
            'scene PNG decodes and is referenced; full-art modal opens and restores focus', 'preserved Experiment01 link resolves',
            'menu/reveal/world; skills with history provenance', 'same actor across reveal/world',
            'modal cancel has no result; Escape restores focus', 'J opens two actor memories with sources',
            '150 percent text has no page horizontal overflow; action remains operable']})
        page.close()
    browser.close()
(out / 'browser.json').write_text(json.dumps({'scope': 'Local browser UX only; not gameplay, Unity, human playtest or user approval', 'results': checks}, ensure_ascii=False, indent=2) + '\n')
print(json.dumps({'status': 'passed', 'viewports': len(checks), 'evidence': str(out / 'browser.json')}))
