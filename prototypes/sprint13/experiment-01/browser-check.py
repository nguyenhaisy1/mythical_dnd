"""Local UX checks; requires Python Playwright and installed Google Chrome. Not gameplay acceptance."""
from playwright.sync_api import sync_playwright
from pathlib import Path
import json
root=Path(__file__).resolve().parents[3]
out=root/'tests/results/sprint13-experiment01'
out.mkdir(parents=True,exist_ok=True)
with sync_playwright() as p:
 browser=p.chromium.launch(executable_path='/usr/bin/google-chrome',headless=True,args=['--no-sandbox'])
 page=browser.new_page(viewport={'width':1440,'height':900},device_scale_factor=1)
 errors=[];page.on('pageerror',lambda e:errors.append(str(e)))
 page.goto((root/'prototypes/sprint13/experiment-01/index.html').as_uri())
 page.screenshot(path=str(out/'menu.png'),full_page=True)
 page.click('#new-campaign');page.wait_for_selector('#reveal:not([hidden])')
 page.screenshot(path=str(out/'reveal.png'),full_page=True)
 assert page.locator('.skill-sources li').count()>0
 name=page.locator('#character-name').inner_text();page.click('#begin')
 page.screenshot(path=str(out/'world.png'),full_page=True)
 page.click('#preview-action');page.screenshot(path=str(out/'preview.png'))
 page.keyboard.press('Escape');assert page.evaluate('document.activeElement.id')=='preview-action'
 page.keyboard.press('j');assert page.locator('#claims .claim').count()==2
 page.screenshot(path=str(out/'journal.png'));page.keyboard.press('Escape')
 page.select_option('#text-scale','150');page.set_viewport_size({'width':1280,'height':720})
 page.screenshot(path=str(out/'world-150.png'),full_page=True)
 assert page.evaluate('document.documentElement.scrollWidth<=innerWidth')
 assert page.locator('#world-name').inner_text()==name
 assert not errors
 (out/'browser.json').write_text(json.dumps({'errors':errors,'scope':'browser UX not gameplay or human playtest','viewport':[1440,900],'scaled_viewport':[1280,720],'checks':['menu/reveal/world','preview Esc focus restore','journal two actor memories','text150 no horizontaloverflow','same actor persisted in UI']},indent=2))
 browser.close()
