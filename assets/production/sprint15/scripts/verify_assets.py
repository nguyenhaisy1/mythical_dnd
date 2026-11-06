"""Verify actual PNG geometry/alpha and source inventory without generating or editing art."""
import hashlib,json
from pathlib import Path
from PIL import Image,ImageChops
ROOT=Path(__file__).resolve().parents[4]
SOURCE=ROOT/'assets/production/sprint15'
manifest=json.loads((SOURCE/'manifest.json').read_text())
for source in manifest['sources']:
 p=ROOT/source['file'];assert hashlib.sha256(p.read_bytes()).hexdigest()==source['sha256'],p
 assert Image.open(p).size==tuple(source['size']),p
 assert (ROOT/source['prompt_file']).is_file(),source['prompt_file']
images={}
for asset in manifest['runtime_assets']:
 p=ROOT/asset['file'];assert hashlib.sha256(p.read_bytes()).hexdigest()==asset['sha256'],p
 im=Image.open(p).convert('RGBA');images[asset['id']]=im
 assert im.size==tuple(asset['size']) and im.getbbox()==tuple(asset['visible_bbox']),p
 if asset['id']=='base':assert im.size==(1920,1080);continue
 a=im.getchannel('A');b=im.getbbox();assert a.getextrema()[0]==0 and b[0]>=4 and b[1]>=4 and im.width-b[2]>=4 and im.height-b[3]>=4,p
 assert not any(r>220 and blue>220 and g<80 and alpha>128 for r,g,blue,alpha in (im.get_flattened_data() if hasattr(im,'get_flattened_data') else im.getdata())),'Retained solid-magenta key in '+str(p)
for names in [('actor-idle-0','actor-idle-1'),('boat-damaged','boat-repaired')]:
 a,b=(images[n] for n in names);assert a.size==b.size,names
 assert ImageChops.difference(a,b).convert('RGB').getbbox() is not None,'Identical variants '+str(names)
assert images['actor-idle-0'].getbbox()==images['actor-idle-1'].getbbox(),'Idle foot/root drift'
qc=json.loads((SOURCE/'asset-qc.json').read_text())['processor_summaries']
for group in qc.values():assert group['empty_count']==group['edge_touch_count']==group['paste_clamped_count']==0
assert qc['actor']['body_scale_cv']<=.08 and qc['actor']['anchor_y_std']<=.05
print('PASS art-source/QC:5 source images,6 runtime PNGs; alpha/padding/pivots/shared-crop/variant-difference checks. Not Unity/runtime acceptance.')
