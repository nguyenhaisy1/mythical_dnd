"""Build the original Ben Vac 3D visual experiment. Run with Blender --background --python build_scene.py.
No external assets, network, production gameplay or archaeological accuracy claims.
"""
import bpy, math, random, json, base64, hashlib, sys
from pathlib import Path
from mathutils import Vector

ROOT = Path(__file__).resolve().parent
OUT = ROOT / 'assets'
OUT.mkdir(exist_ok=True)
rng = random.Random(13042026)
bpy.ops.object.select_all(action='SELECT')
bpy.ops.object.delete(use_global=False)
for block in bpy.data.materials: bpy.data.materials.remove(block)

# Colours are local material choices, not a global orange/teal grade.
PALETTE = {
 'earth':('#9b8f70', .96), 'earth_side':('#706959',1), 'path':('#b3a387',1),
 'water':('#748d8c',.46), 'water_ripple':('#a6b6ac',.75), 'water_dark':('#637f7d',.75),
 'wood':('#756852',.93), 'wood_light':('#9a8869',.94), 'wood_dark':('#494c42',.97), 'wood_fresh':('#b19b75',.96),
 'thatch':('#99906b',.98), 'thatch_light':('#b2a17b',.99), 'thatch_dark':('#827b59',.99),
 'bamboo':('#a6986c',.95), 'rope':('#b7a67f',1), 'net':('#716e56',1),
 'leaf':('#65785b',.96), 'leaf_light':('#81906a',.96), 'leaf_dark':('#4b624c',.96),
 'stone':('#868a7b',1), 'clay':('#926c58',.94), 'linen':('#d0c5a9',1),
 'cloth_red':('#955d4f',1), 'cloth_blue':('#596f72',1), 'skin':('#a67a58',.95), 'hair':('#333b35',1),
}
def rgba(hex):
 h=hex.lstrip('#'); return tuple(((int(h[i:i+2],16)/255+.055)/1.055)**2.4 if int(h[i:i+2],16)/255>.04045 else int(h[i:i+2],16)/255/12.92 for i in (0,2,4))+(1,)
materials={}
for name,(col,rough) in PALETTE.items():
 mat=bpy.data.materials.new(name); mat.diffuse_color=rgba(col); mat.use_nodes=True
 bs=mat.node_tree.nodes.get('Principled BSDF'); bs.inputs['Base Color'].default_value=rgba(col); bs.inputs['Roughness'].default_value=rough
 if name not in ['water_ripple','water_dark']:
  noise=mat.node_tree.nodes.new('ShaderNodeTexNoise');noise.inputs['Scale'].default_value=24 if name!='water' else 10;noise.inputs['Detail'].default_value=2
  bump=mat.node_tree.nodes.new('ShaderNodeBump');bump.inputs['Strength'].default_value=.17 if name!='water' else .12;bump.inputs['Distance'].default_value=.045 if name!='water' else .055
  mat.node_tree.links.new(noise.outputs['Fac'],bump.inputs['Height']);mat.node_tree.links.new(bump.outputs['Normal'],bs.inputs['Normal'])
 materials[name]=mat

# Batching keeps GLB draw calls bounded, while retaining real geometry in every view.
batches={k:[[],[]] for k in PALETTE}
def mesh(mat,verts,faces):
 v,f=batches[mat];i=len(v);v.extend(tuple(p) for p in verts);f.extend(tuple(i+x for x in face) for face in faces)
def quad(mat,a,b,c,d): mesh(mat,[a,b,c,d],[(0,1,2,3)])
def box(mat,center,size,rot=0):
 cx,cy,cz=center;sx,sy,sz=[s*.5 for s in size];co=math.cos(rot);si=math.sin(rot)
 points=[]
 for z in [-sz,sz]:
  for y in [-sy,sy]:
   for x in [-sx,sx]:points.append((cx+x*co-y*si,cy+x*si+y*co,cz+z))
 mesh(mat,points,[(0,2,3,1),(4,5,7,6),(0,1,5,4),(2,6,7,3),(0,4,6,2),(1,3,7,5)])
def rod(mat,a,b,r=.05,sides=7,r2=None):
 a=Vector(a);b=Vector(b);axis=(b-a).normalized();up=Vector((0,0,1)) if abs(axis.z)<.9 else Vector((1,0,0));u=axis.cross(up).normalized();v=axis.cross(u).normalized();verts=[]
 for p,rad in [(a,r),(b,r if r2 is None else r2)]:
  for i in range(sides):verts.append(p+rad*(u*math.cos(i*2*math.pi/sides)+v*math.sin(i*2*math.pi/sides)))
 faces=[tuple(reversed(range(sides))),tuple(range(sides,2*sides))]
 for i in range(sides):j=(i+1)%sides;faces.append((i,j,j+sides,i+sides))
 mesh(mat,verts,faces)
def chain(mat,points,r=.03,sides=6):
 for a,b in zip(points,points[1:]):rod(mat,a,b,r,sides)
def ellipsoid(mat,c,scale,segments=10,rings=6,jitter=0):
 verts=[]
 for j in range(rings+1):
  phi=math.pi*j/rings
  for i in range(segments):
   t=i*math.tau/segments;fac=1+rng.uniform(-jitter,jitter)
   verts.append((c[0]+scale[0]*math.sin(phi)*math.cos(t)*fac,c[1]+scale[1]*math.sin(phi)*math.sin(t)*fac,c[2]+scale[2]*math.cos(phi)*fac))
 faces=[]
 for j in range(rings):
  for i in range(segments):k=j*segments+i;n=j*segments+(i+1)%segments;faces.append((k,n,n+segments,k+segments))
 mesh(mat,verts,faces)
def vessel(c,r=.35,h=.55,mat='clay',open_top=True):
 n=14;profile=[(r*.6,0),(r,.15*h),(r*.95,.7*h),(r*.62,h),(r*.5,h+.04)]
 verts=[]
 for rad,z in profile:
  for i in range(n):a=i*math.tau/n;verts.append((c[0]+rad*math.cos(a),c[1]+rad*math.sin(a),c[2]+z))
 faces=[]
 for j in range(len(profile)-1):
  for i in range(n):k=j*n+i;l=j*n+(i+1)%n;faces.append((k,l,l+n,k+n))
 mesh(mat,verts,faces)
 if open_top:
  ellipsoid('wood_dark',(c[0],c[1],c[2]+h-.08),(r*.47,r*.47,.035),14,2)
def basket(c,r=.36,h=.55):
 vessel(c,r,h,'bamboo')
 for j in range(1,7):
  rad=r*(.75+.18*math.sin(j/6*math.pi));z=c[2]+h*j/7
  pts=[(c[0]+rad*math.cos(i*math.tau/24),c[1]+rad*math.sin(i*math.tau/24),z) for i in range(25)]
  chain('rope',pts,.014,4)
 for i in range(16):
  a=i*math.tau/16;rod('thatch_dark',(c[0]+r*.65*math.cos(a),c[1]+r*.65*math.sin(a),c[2]+.04),(c[0]+r*.63*math.cos(a),c[1]+r*.63*math.sin(a),c[2]+h),.012,4)

def rope_coil(c,r=.45):
 pts=[]
 for i in range(180):
  t=i*.15;rr=.1+(r-.1)*i/179;pts.append((c[0]+rr*math.cos(t),c[1]+rr*math.sin(t),c[2]+.025))
 chain('rope',pts,.022,5)

# Sculpted cutaway riverbank, a real mesh rather than a painted backdrop.
ys=[-8,-6,-4,-2,0,2,4,6,8];edge=[-.3,.15,-.4,.25,-.1,.5,-.1,.2,-.4]
verts=[(-10,-8,-.55),(-10,8,-.55)]+[(edge[i],y,-.55) for i,y in enumerate(ys)]+[(-10,-8,.65),(-10,8,.65)]+[(edge[i],y,.52+.1*math.sin(i)) for i,y in enumerate(ys)]
# Explicit terrain top strips connect shore to inland with slight slope.
for i in range(len(ys)-1):
 y1,y2=ys[i:i+2];x1,x2=edge[i:i+2]
 quad('earth',(-10,y1,.7),(-10,y2,.7),(x2,y2,.48),(x1,y1,.48))
 quad('earth_side',(x1,y1,-.55),(x2,y2,-.55),(x2,y2,.48),(x1,y1,.48))
quad('earth_side',(-10,-8,-.55),(-10,8,-.55),(-10,8,.7),(-10,-8,.7))
for y in [-8,8]:quad('earth_side',(-10,y,-.55),(.0,y,-.55),(.0,y,.5),(-10,y,.7))
box('water',(5,0,-.25),(10,16,.5))
# Grain/ripple strokes are geometry; sparse enough to leave a calm river plane.
for i in range(120):
 x=rng.uniform(.8,9.7);y=rng.uniform(-7.7,7.7);length=rng.uniform(.15,.8)
 quad('water_ripple' if i%4 else 'water_dark',(x,y,.014),(x+length,y+.025,.014),(x+length+.08,y+.055,.014),(x+.04,y+.035,.014))
# Path mottles and irregular bank stones, no texture overlay over the whole composition.
for i in range(100):
 x=rng.uniform(-9.8,-.6);y=rng.uniform(-7.7,7.7)
 if -8<x<-2 and 0<y<6:continue
 if i%3:ellipsoid('earth' if i%2 else 'path',(x,y,.71),(.08+rng.random()*.16,.06+rng.random()*.1,.008),5,2)
 else:ellipsoid('stone',(x,y,.73),(.12,.1,.07),7,4,.16)
for i,y in enumerate(ys):ellipsoid('stone',(edge[i]+.07,y,.42),(.38,.5,.3),7,4,.15)

# Main house, elevated deck, individual floorboards and bamboo wall panels.
hx,hy=-5.3,3.3;floor=2.0;w=5.5;d=4.0
for x in [hx-w/2,hx,hx+w/2]:
 for y in [hy-d/2-.9,hy-d/2,hy+d/2]:rod('wood_dark',(x,y,.58),(x,y,4.5),.115,9)
for y in [hy-d/2-1,hy-d/2,hy+d/2]:rod('wood',(hx-w/2-.15,y,floor-.18),(hx+w/2+.15,y,floor-.18),.12,8)
for j in range(23):
 x=hx-w/2+j*.25;box('wood_light' if j%4 else 'wood',(x,hy-.45,floor),(.235,d+.9,.1))
for x in [hx-w/2,hx+w/2]:
 for j in range(18):
  y=hy-d/2+j*.23;box('bamboo' if j%3 else 'wood_light',(x,y,3.14),(.08,.22,2.15))
for j in range(23):
 x=hx-w/2+j*.25
 box('bamboo' if j%3 else 'wood_light',(x,hy+d/2,3.14),(.23,.08,2.15))
 # Door opening at centre, open shutter to its left.
 if abs(x-hx)>.65:box('bamboo' if j%3 else 'wood_light',(x,hy-d/2,3.13),(.23,.08,2.13))
rod('wood_dark',(hx-w/2,hy-d/2,4.25),(hx+w/2,hy-d/2,4.25),.085)
for x in [hx-w/2,hx+w/2]:
 rod('wood',(x,hy-d/2-.95,2.9),(x,hy+d/2,2.9),.055)
 for y in [hy-d/2-.95,hy,hy+d/2]:rod('wood',(x,y,2.05),(x,y,2.95),.06)
# Front veranda rail except stair opening, hanging cloth and baskets.
for a,b in [(hx-w/2,hx-.7),(hx+.7,hx+w/2)]:
 rod('wood',(a,hy-d/2-.95,2.9),(b,hy-d/2-.95,2.9),.055)
 for x in [a,b]:rod('wood',(x,hy-d/2-.95,2.05),(x,hy-d/2-.95,2.95),.055)
box('cloth_red',(hx-1.6,hy-d/2-.075,3.45),(.65,.035,1.35))
for x,y in [(hx+1.7,hy-.9),(hx-1.8,hy-.8)]:basket((x,y,2.05),.3,.52)
# Six broad stair treads bridge the full deck height.
for j in range(6):
 y=hy-d/2-1.15-j*.33;z=1.78-j*.205
 box('wood_light',(hx,y,z),(1.3,.32,.12))
for x in [hx-.5,hx+.5]:rod('wood',(x,hy-d/2-1.12,1.7),(x,hy-d/2-2.9,.58),.07)
# Layered gable thatch: reeds run down the slope; multiple short overlapping courses.
ridge=5.65;eave=4.14
for side in [-1,1]:
 quad('thatch_dark',(hx-3.2,hy,ridge),(hx+3.2,hy,ridge),(hx+3.2,hy+side*2.75,eave),(hx-3.2,hy+side*2.75,eave))
 for row in range(7):
  t0=row/7;t1=min(1,(row+1.85)/7)
  for j in range(67):
   x=hx-3.18+j*.097+rng.uniform(-.035,.035)
   y0=hy+side*t0*2.75;y1=hy+side*t1*2.75
   z0=ridge+(eave-ridge)*t0+.03;z1=ridge+(eave-ridge)*t1+rng.uniform(-.1,.05)
   rod(['thatch','thatch_light','thatch_dark'][rng.randrange(3)],(x,y0,z0),(x+rng.uniform(-.03,.03),y1,z1),rng.uniform(.027,.043),4)
rod('thatch',(hx-3.35,hy,ridge+.06),(hx+3.35,hy,ridge+.06),.13,9)

# Dock extends from bank to the river, broad joined planks and bracing.
for j in range(29):
 x=-.6+j*.195;box('wood_light' if j%4 else 'wood',(x,-.25,.85),(.18,2.55,.12))
for x in [-.55,1.7,4.9]:
 for y in [-1.4,.95]:
  rod('wood_dark',(x,y,-.35),(x,y,1.6),.13,9)
  # Several rope bindings around tops read as joinery from close camera.
  for k in range(3):
   pts=[(x+.145*math.cos(t*math.tau/12),y+.145*math.sin(t*math.tau/12),1.25+k*.05) for t in range(13)];chain('rope',pts,.017,4)
for y in [-1.1,.65]:rod('wood_dark',(-.65,y,.6),(5.15,y,.6),.13,8)
for a,b in [((1.7,-1.4,-.1),(4.9,-1.4,.7)),((1.7,.95,-.1),(4.9,.95,.7))]:rod('wood',a,b,.075)
# Low second side finger.
for j in range(12):box('wood',(3.4,1+j*.18,.8),(2.4,.16,.09))

# Coherent open boats with curved longitudinal hull planks, inner surface and ribs.
def boat(cx,cy,base,length=5.1,width=.86,turn=0):
 co,si=math.cos(turn),math.sin(turn)
 def tr(x,y,z):return(cx+x*co-y*si,cy+x*si+y*co,base+z)
 def point(t,a,inner=False):
  taper=max(.055,math.sin(math.pi*(t+1)/2))**.62;ww=width*taper-(.055 if inner else 0)
  yy=t*length/2;zz=.10+.28*abs(t)**3+(1-math.cos(a))*.77+(.065 if inner else 0)
  return tr(ww*math.sin(a),yy,zz)
 n=20;bands=10
 for side_inner in [False,True]:
  for j in range(bands):
   a0=-math.pi/2+j*math.pi/bands;a1=a0+math.pi/bands
   mat=['wood','wood_light','wood','wood_fresh'][j%4] if not side_inner else 'wood_light'
   for i in range(n):
    t0=-1+i*2/n;t1=-1+(i+1)*2/n
    quad(mat,point(t0,a0,side_inner),point(t1,a0,side_inner),point(t1,a1,side_inner),point(t0,a1,side_inner))
 for sign in [-1,1]:chain('wood_dark',[point(-1+i*2/n,sign*math.pi/2) for i in range(n+1)],.055,7)
 for i in range(1,9):
  t=-.88+i*.19;chain('wood_dark',[point(t,-math.pi/2+j*math.pi/10,True) for j in range(11)],.036,6)
 for t in [-.5,0,.5]:
  a=point(t,-.75,True);b=point(t,.75,True);rod('wood',a,b,.065,7)
 return tr
repair=boat(-1.9,-3.65,.72,5.2,.9,math.radians(-6))
for y in [-4.9,-2.5]:
 box('wood_dark',(-1.9,y,.82),(1.45,.16,.14))
 for x in [-2.55,-1.25]:rod('wood',(x,y,.7),(-1.9,y,1.05),.075)
moored=boat(6.0,2.0,-.03,4.6,.73,math.radians(7))
# Boat benches / reed roof for the moored boat.
for y in [.8,2.1,3.2]:box('wood_light',(6,y,.48),(1.02,.22,.09),.08)
for y in [1.05,2.75]:
 for x in [5.5,6.5]:rod('bamboo',(x,y,.6),(x,y,1.6),.035)
for j in range(17):
 y=.9+j*.12
 pts=[(5.25+.12*i,y,1.5+.38*math.sin(math.pi*i/12)) for i in range(13)]
 chain('thatch',pts,.065,5)
rod('wood',(6.55,.0,.55),(7.9,-2.1,.11),.035,7)
box('wood_dark',(7.85,-2.05,.11),(.25,.6,.055),-.55)
chain('rope',[(5.0,.7,1.2),(5.5,.6,.65),(5.7,.1,.55)],.025)

# Work zone objects and purposeful signs of labour.
for j in range(7):box('wood_fresh',(-4.0,-3.4+j*.16,.79+j*.012),(.22,3.1,.11),.15)
for j in range(4):box('wood',(-4.25,-1.4,.85+j*.11),(1.9,.19,.1),-.1)
for x,y in [(-4.4,-5.3),(-.75,-5.6),(2.0,-.8),(-7.3,-1.8)]:basket((x,y,.74 if x<0 else .92),.31,.47)
for x,y in [(-3.5,-5.6),(-.6,-1.5),(4.2,-.2)]:rope_coil((x,y,.75 if x<0 else .94),.35)
for x,y in [(-6.9,.0),(-7.5,-.4),(-4.4,-5.8)]:vessel((x,y,.72),.29,.55)
# Low workbench and unmistakable adze, not generic medieval weapon.
box('wood_light',(-3.2,-5.25,1.12),(1.2,.55,.1))
for x in [-3.7,-2.7]:
 for y in [-5.45,-5.05]:rod('wood_dark',(x,y,.72),(x,y,1.1),.055)
rod('wood',(-3.6,-5.25,1.2),(-2.85,-5.22,1.2),.025)
box('stone',(-2.9,-5.25,1.23),(.14,.2,.06),.1)
# Net frame: draped sagging mesh rather than a solid rectangular plane.
for y in [-1.2,1.0]:rod('wood_dark',(-7.8,y,.7),(-7.8,y,2.6),.055)
rod('wood',(-7.8,-1.3,2.4),(-7.8,1.1,2.4),.045)
for i in range(15):
 y=-1.1+i*.15;chain('net',[(-7.8+.09*math.sin(j*.4),y+.08*math.sin(j*.5),2.35-j*.12) for j in range(15)],.009,3)
for j in range(14):
 z=2.35-j*.12;chain('net',[(-7.8+.08*math.sin(j*.4),-1.1+i*.15,z-.1*math.sin(i/14*math.pi)) for i in range(15)],.009,3)

# Small figures support scale and work, not finished character assets.
def person(x,y,z,shirt='linen',pose='stand',angle=0):
 co,si=math.cos(angle),math.sin(angle)
 def t(a,b,c):return(x+a*co-b*si,y+a*si+b*co,z+c)
 # Feet, trousers, slightly tapered torso and head.
 for sign in [-1,1]:
  knee=t(sign*.12,-.03,.43);hip=t(sign*.11,0,.78);foot=t(sign*.12,-.11,.07)
  rod('cloth_blue',foot,knee,.08,7);rod('cloth_blue',knee,hip,.095,7)
  ellipsoid('skin',foot,(.09,.15,.06),7,4)
 ellipsoid(shirt,t(0,0,1.05),(.25,.15,.35),9,6)
 rod('skin',t(0,0,1.27),t(0,0,1.38),.075)
 ellipsoid('skin',t(0,-.02,1.48),(.125,.12,.16),9,6)
 ellipsoid('hair',t(0,.015,1.57),(.13,.12,.1),9,5)
 ellipsoid('hair',t(0,.14,1.54),(.075,.075,.07),7,4)
 for sign in [-1,1]:
  shoulder=t(sign*.23,0,1.23)
  elbow=t(sign*.28,-.15,.98 if pose=='stand' else 1.05)
  hand=t(sign*.19,-.23,.82 if pose=='stand' else 1.18)
  rod(shirt,shoulder,elbow,.07);rod('skin',elbow,hand,.055);ellipsoid('skin',hand,(.055,.05,.06),6,4)
 if pose=='work':rod('wood_dark',t(-.19,-.23,1.18),t(.14,-.3,1.2),.022)
person(-.65,-3.85,.73,'linen','work',math.pi/2)
person(3.1,-.45,.92,'cloth_red','stand',-.5)
person(-7.0,-.7,.72,'linen','work',-.5)

# Banana leaves use curved broad blades and ribs, not spherical toy trees.
def banana(x,y,z=0.7):
 rod('wood_light',(x,y,z),(x+.1,y,z+2.8),.13,9,r2=.075)
 for k in range(8):
  a=k*math.tau/8+.2;length=1.7+rng.random()*.6;start=Vector((x+.1,y,z+2.55));verts=[];pts=[]
  for j in range(8):
   t=j/7;centre=start+Vector((math.cos(a)*length*t,math.sin(a)*length*t,.65*math.sin(t*math.pi)-.25*t));width=.34*math.sin(math.pi*t)**.6
   perp=Vector((-math.sin(a),math.cos(a),0));verts.extend([centre-perp*width,centre+perp*width]);pts.append(centre+Vector((0,0,.008)))
  mesh(['leaf','leaf_light','leaf_dark'][k%3],verts,[(j*2,j*2+1,j*2+3,j*2+2) for j in range(7)]);chain('leaf_light',pts,.016,4)
for x,y in [(-9,-4),(-9,5.7),(-2,6.8)]:banana(x,y)
# Reeds and low bankside tufts, individually shaped blades.
for i in range(50):
 x=rng.uniform(-9.7,-.4);y=rng.uniform(-7.8,7.7)
 if -8<x<-2 and 0<y<6:continue
 for j in range(5):
  a=rng.random()*math.tau;h=rng.uniform(.2,.65);w=.03
  p=(x,y,.72);q=(x+math.cos(a)*.2,y+math.sin(a)*.2,.72+h)
  quad('leaf' if j%2 else 'leaf_dark',(p[0]-w,p[1],p[2]),(p[0]+w,p[1],p[2]),q,(q[0],q[1],q[2]))
# A small pile of stones at bank toe avoids an unnaturally straight shore.
for i in range(28):
 y=rng.uniform(-7.7,7.7);x=rng.uniform(-.2,.4);ellipsoid('stone',(x,y,.18),(.15,.2,.12),7,4,.2)

objects=[]
for mat,(verts,faces) in batches.items():
 if not verts:continue
 data=bpy.data.meshes.new('geometry_'+mat);data.from_pydata(verts,[],faces);data.materials.append(materials[mat]);data.update()
 obj=bpy.data.objects.new('BenVac_'+mat,data);bpy.context.collection.objects.link(obj);objects.append(obj)

# Neutral studio-world and soft illumination keep local material colours legible.
world=bpy.data.worlds.new('NeutralDaylight');bpy.context.scene.world=world;world.use_nodes=True
world.node_tree.nodes['Background'].inputs[0].default_value=(.67,.70,.71,1);world.node_tree.nodes['Background'].inputs[1].default_value=.65
light_data=bpy.data.lights.new('Soft overcast key','AREA');light_data.energy=1800;light_data.shape='DISK';light_data.size=12
light=bpy.data.objects.new('Soft overcast key',light_data);bpy.context.collection.objects.link(light);light.location=(-3,-4,13)
fill_data=bpy.data.lights.new('Large sky fill','AREA');fill_data.energy=650;fill_data.size=9
fill=bpy.data.objects.new('Large sky fill',fill_data);bpy.context.collection.objects.link(fill);fill.location=(7,3,9);fill.rotation_euler=(Vector((-2,0,1))-fill.location).to_track_quat('-Z','Y').to_euler()
scene=bpy.context.scene;scene.render.engine='CYCLES';scene.cycles.device='CPU';scene.cycles.samples=24;scene.cycles.use_denoising=True
scene.render.resolution_x=1280;scene.render.resolution_y=900;scene.render.resolution_percentage=100
scene.render.image_settings.file_format='PNG';scene.render.film_transparent=False
scene.view_settings.view_transform='AgX';scene.view_settings.look='AgX - Medium High Contrast';scene.view_settings.exposure=0;scene.view_settings.gamma=1
scene.render.threads_mode='FIXED';scene.render.threads=6
cam_data=bpy.data.cameras.new('Review camera');cam=bpy.data.objects.new('Review camera',cam_data);bpy.context.collection.objects.link(cam);scene.camera=cam
CAMERAS={
 'overview':{'position':(16,-21,16),'target':(-1,0,1.2),'lens':47},
 'eye':{'position':(-7,-6.5,2.2),'target':(.5,-.5,1.0),'lens':30},
 'repair':{'position':(4,-10,4.7),'target':(-2,-3.2,1.35),'lens':47},
}
def set_camera(key):
 c=CAMERAS[key];cam.location=c['position'];cam.rotation_euler=(Vector(c['target'])-cam.location).to_track_quat('-Z','Y').to_euler();cam_data.lens=c['lens'];cam_data.sensor_fit='HORIZONTAL';cam_data.sensor_width=36;cam_data.clip_start=.1;cam_data.clip_end=150
set_camera('overview')
# Keep the authoring scene. Export only visible mesh geometry; lights are composed in the viewer.
bpy.context.preferences.filepaths.save_version=0
bpy.ops.wm.save_as_mainfile(filepath=str(OUT/'scene.blend'))
bpy.ops.object.select_all(action='DESELECT')
for obj in objects:obj.select_set(True)
bpy.context.view_layer.objects.active=objects[0]
bpy.ops.export_scene.gltf(filepath=str(OUT/'ben-vac.glb'),export_format='GLB',use_selection=True,export_lights=False,export_cameras=False,export_materials='EXPORT')
glb=(OUT/'ben-vac.glb').read_bytes()
def yup(v):return[v[0],v[2],-v[1]]
metadata={'name':'Bến Vạc — thử nghiệm 3D04','version':'s13-scene-04-v1','approval':'pending','coordinateSystem':'glTF Y-up; Blender source Z-up','glbBase64':base64.b64encode(glb).decode(),'cameras':{k:{'position':yup(v['position']),'target':yup(v['target']),'fov':math.degrees(2*math.atan(36/(2*v['lens']*(scene.render.resolution_x/scene.render.resolution_y)))),'lensMm':v['lens']} for k,v in CAMERAS.items()},'reference':'../experiment-03/assets/b-gouache.png','meshCount':len(objects)}
(OUT/'scene-data.js').write_text('/* Generated from build_scene.py; no network fetch required. */\nwindow.BEN_VAC_SCENE='+json.dumps(metadata,ensure_ascii=False,separators=(',',':'))+';\n')
manifest={'generator':'build_scene.py','blender':bpy.app.version_string,'seed':13042026,'approval':'pending','mesh_count':len(objects),'material_count':len(materials),'vertex_count':sum(len(o.data.vertices) for o in objects),'triangle_count':sum(len(p.vertices)-2 for o in objects for p in o.data.polygons),'glb_sha256':hashlib.sha256(glb).hexdigest(),'cameraSourceZup':CAMERAS,'scope':'Original procedural 3D art experiment; not Unity gameplay or archaeological reconstruction','renders':[]}
manifest['projection']={'sensor_fit':'HORIZONTAL','sensor_width_mm':36,'render_aspect':scene.render.resolution_x/scene.render.resolution_y,'vertical_fov_degrees':{k:v['fov'] for k,v in metadata['cameras'].items()},'eye_height_above_bank_m':1.7}
(OUT/'scene-manifest.json').write_text(json.dumps(manifest,ensure_ascii=False,indent=2)+'\n')
print('SCENE_EXPORT_READY',json.dumps({k:v for k,v in manifest.items() if k not in ['cameraSourceZup']},ensure_ascii=False),flush=True)
if '--no-render' not in sys.argv:
 for key in CAMERAS:
  set_camera(key);scene.render.filepath=str(OUT/f'render-{key}.png');bpy.ops.render.render(write_still=True)
  manifest['renders'].append({'camera':key,'file':f'render-{key}.png','width':scene.render.resolution_x,'height':scene.render.resolution_y,'samples':scene.cycles.samples})
  (OUT/'scene-manifest.json').write_text(json.dumps(manifest,ensure_ascii=False,indent=2)+'\n')
  print('RENDER_COMPLETE',key,flush=True)
print('SCENE_BUILD_COMPLETE',flush=True)
