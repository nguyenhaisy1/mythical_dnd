"""Import the completed Hyper3D GLB, place it, export and render cameras.

Run: blender -b --python prototypes/sprint13/experiment-05/prepare_model.py --
     --input tests/results/sprint13-experiment05/hyper3d/provider-original.glb
No mesh is generated here. Model geometry/materials originate in Hyper3D MCP.
"""
import argparse
import base64
import hashlib
import json
import math
from pathlib import Path
import sys
import bpy
from mathutils import Matrix,Vector

ROOT=Path(__file__).resolve().parent
OUT=ROOT/'assets'
parser=argparse.ArgumentParser()
parser.add_argument('--input',type=Path,required=True)
parser.add_argument('--no-render',action='store_true')
args=parser.parse_args(sys.argv[sys.argv.index('--')+1:] if '--' in sys.argv else [])
source=json.loads((OUT/'provider-source.json').read_text())
if hashlib.sha256(args.input.read_bytes()).hexdigest()!=source['original_sha256']:
 raise RuntimeError('Input differs from the downloaded Hyper3D evidence')
bpy.ops.object.select_all(action='SELECT');bpy.ops.object.delete(use_global=False)
bpy.ops.import_scene.gltf(filepath=str(args.input.resolve()))
imported=list(bpy.context.scene.objects)
meshes=[o for o in imported if o.type=='MESH']
if not meshes:raise RuntimeError('Provider GLB has no mesh')
points=[o.matrix_world@Vector(corner) for o in meshes for corner in o.bound_box]
lo=Vector(tuple(min(p[i] for p in points) for i in range(3)))
hi=Vector(tuple(max(p[i] for p in points) for i in range(3)))
size=hi-lo
if min(size)<=0:raise RuntimeError('Provider model is degenerate')
scale=8/max(size.x,size.y)
transform=Matrix.Translation(Vector((-(lo.x+hi.x)*scale/2,-(lo.y+hi.y)*scale/2,-lo.z*scale)))@Matrix.Scale(scale,4)
roots=[o for o in imported if o.parent not in imported]
for obj in roots:obj.matrix_world=transform@obj.matrix_world
bpy.context.view_layer.update()
height=size.z*scale
world=bpy.data.worlds.new('Neutral material inspection');world.use_nodes=True
world.node_tree.nodes['Background'].inputs[0].default_value=(.68,.68,.65,1)
world.node_tree.nodes['Background'].inputs[1].default_value=.7
scene=bpy.context.scene;scene.world=world
for name,position,energy,lamp_size in [('Large soft key',(1,-4,10),1300,8),('Neutral fill',(-7,5,7),650,7)]:
 data=bpy.data.lights.new(name,'AREA');data.energy=energy;data.shape='DISK';data.size=lamp_size
 obj=bpy.data.objects.new(name,data);bpy.context.collection.objects.link(obj);obj.location=position
 obj.rotation_euler=(Vector((0,0,height*.4))-obj.location).to_track_quat('-Z','Y').to_euler()
scene.render.engine='CYCLES';scene.cycles.device='CPU';scene.cycles.samples=24;scene.cycles.use_denoising=True
scene.render.resolution_x=1280;scene.render.resolution_y=900;scene.render.resolution_percentage=100
scene.render.image_settings.file_format='PNG';scene.render.threads_mode='FIXED';scene.render.threads=6
scene.view_settings.view_transform='AgX';scene.view_settings.look='AgX - Medium High Contrast';scene.view_settings.exposure=0;scene.view_settings.gamma=1
camera_data=bpy.data.cameras.new('Experiment05 camera');camera=bpy.data.objects.new('Experiment05 camera',camera_data);bpy.context.collection.objects.link(camera);scene.camera=camera
CAMERAS={
 'overview':{'position':(10,-13,9),'target':(0,0,height*.42),'lens':47},
 'eye':{'position':(8,-10,1.7),'target':(0,0,1.35),'lens':36},
 'repair':{'position':(5,-6,3.0),'target':(0,-.4,1.5),'lens':42},
}
def set_camera(key):
 c=CAMERAS[key];camera.location=c['position'];camera.rotation_euler=(Vector(c['target'])-camera.location).to_track_quat('-Z','Y').to_euler()
 camera_data.lens=c['lens'];camera_data.sensor_width=36;camera_data.sensor_fit='HORIZONTAL';camera_data.clip_start=.05;camera_data.clip_end=200
set_camera('overview')
bpy.ops.file.pack_all();bpy.context.preferences.filepaths.save_version=0
bpy.ops.wm.save_as_mainfile(filepath=str(OUT/'scene.blend'),compress=True)
bpy.ops.object.select_all(action='DESELECT')
for obj in imported:obj.select_set(True)
bpy.context.view_layer.objects.active=meshes[0]
bpy.ops.export_scene.gltf(filepath=str(OUT/'ben-vac.glb'),export_format='GLB',use_selection=True,export_lights=False,export_cameras=False,export_materials='EXPORT')
glb=(OUT/'ben-vac.glb').read_bytes()
def yup(v):return [v[0],v[2],-v[1]]
fovs={k:math.degrees(2*math.atan(36/(2*c['lens']*(1280/900)))) for k,c in CAMERAS.items()}
metadata={'name':'Xưởng sửa thuyền Hyper3D — thử nghiệm05','approval':'pending','provider':'Hyper3D MCP','providerDisplayUrl':source['display_url'],'glbBase64':base64.b64encode(glb).decode(),'cameras':{k:{'position':yup(v['position']),'target':yup(v['target']),'fov':fovs[k]} for k,v in CAMERAS.items()}}
(OUT/'scene-data.js').write_text('/* Hyper3D model imported and placed with prepare_model.py. */\nwindow.BEN_VAC_SCENE='+json.dumps(metadata,ensure_ascii=False,separators=(',',':'))+';\n')
materials={slot.material.name for obj in meshes for slot in obj.material_slots if slot.material}
triangles=sum(len(face.vertices)-2 for obj in meshes for face in obj.data.polygons)
manifest={'provider':'Hyper3D MCP','generation_id':source['generation_id'],'original_sha256':source['original_sha256'],'glb_sha256':hashlib.sha256(glb).hexdigest(),'mesh_count':len(meshes),'triangle_count':triangles,'material_count':len(materials),'blender':bpy.app.version_string,'placement_scale':scale,'original_bbox_blender':{'min':list(lo),'max':list(hi)},'cameraSourceZup':CAMERAS,'vertical_fov_degrees':fovs,'scope':'Imported provider geometry; only scale/placement/cameras/lights/export added locally. No procedural replacement mesh.','renders':[]}
(OUT/'scene-manifest.json').write_text(json.dumps(manifest,ensure_ascii=False,indent=2)+'\n')
print('PROVIDER_MODEL_EXPORT_READY',len(glb),triangles,flush=True)
if not args.no_render:
 for key in CAMERAS:
  set_camera(key);scene.render.filepath=str(OUT/f'render-{key}.png');bpy.ops.render.render(write_still=True)
  manifest['renders'].append({'camera':key,'file':f'render-{key}.png','width':1280,'height':900,'samples':24})
  (OUT/'scene-manifest.json').write_text(json.dumps(manifest,ensure_ascii=False,indent=2)+'\n')
  print('PROVIDER_RENDER_COMPLETE',key,flush=True)
print('PROVIDER_MODEL_PREPARED',flush=True)
