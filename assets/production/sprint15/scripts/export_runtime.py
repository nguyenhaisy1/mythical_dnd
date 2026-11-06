"""Deterministic extraction of already-generated, officially keyed sprites; no creative drawing."""
import hashlib,json
from pathlib import Path
from PIL import Image
ROOT=Path(__file__).resolve().parents[4]
SOURCE=ROOT/'assets/production/sprint15'
DEST=ROOT/'client/UnityProject/Assets/Game/Resources/Sprint15/Art'
PAD=4

def union(paths):
 boxes=[Image.open(p).convert('RGBA').getbbox() for p in paths]
 return (min(b[0] for b in boxes)-PAD,min(b[1] for b in boxes)-PAD,max(b[2] for b in boxes)+PAD,max(b[3] for b in boxes)+PAD)

def export(names,paths,frame_group):
 crop=union(frame_group)
 for name,p in zip(names,paths):
  out=DEST/(name+'.png')
  if out.exists(): raise FileExistsError('Refusing to overwrite runtime asset: '+str(out))
  Image.open(p).convert('RGBA').crop(crop).save(out)

boats=SOURCE/'processed/boats'
export(['boat-damaged','boat-repaired'],[boats/'boat-variants-1.png',boats/'boat-variants-2.png'],[boats/'boat-variants-1.png',boats/'boat-variants-2.png'])
post=SOURCE/'processed/post/single-1.png'
export(['foreground'],[post],[post])
actors=SOURCE/'processed/actor'
export(['actor-idle-0','actor-idle-1'],[actors/'idle-1.png',actors/'idle-3.png'],[actors/f'idle-{i}.png' for i in range(1,5)])
print('Exported five independent RGBA sprites with common per-family crop and four-pixel margin.')
