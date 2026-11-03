import * as THREE from 'three';
import {GLTFLoader} from 'three/addons/loaders/GLTFLoader.js';
import {OrbitControls} from 'three/addons/controls/OrbitControls.js';

const $=id=>document.getElementById(id);
const viewport=$('viewport');
const status=$('status');
let renderer,scene,camera,controls,model;
let ready=false,preset='overview',framePending=false,applyingPreset=false;
const defaults={overview:{position:[16,16,21],target:[-1,1.2,0]},eye:{position:[10,6,16],target:[-2,2,0]},repair:{position:[3,4,9],target:[-1,1.4,3]}};
const cameras=window.BEN_VAC_SCENE?.cameras||defaults;
let materialList=[];
let opener=null;
const debug={};
Object.defineProperties(debug,{
  ready:{enumerable:true,get:()=>ready},
  meshCount:{enumerable:true,get:()=>{let count=0;model?.traverse(o=>{if(o.isMesh)count++;});return count;}},
  triangleCount:{enumerable:true,get:()=>{let count=0;model?.traverse(o=>{if(o.isMesh)count+=(o.geometry.index?.count||o.geometry.attributes.position?.count||0)/3;});return count;}},
  cameraPosition:{enumerable:true,get:()=>camera?.position.toArray()||[]},
  cameraTarget:{enumerable:true,get:()=>controls?.target.toArray()||[]},
  cameraFov:{enumerable:true,get:()=>camera?.fov||0},
  currentPreset:{enumerable:true,get:()=>preset},
  wireframe:{enumerable:true,get:()=>$('wireframe').checked}
});
Object.defineProperty(window,'experimentDebug',{value:Object.freeze(debug),writable:false,configurable:false});
function requestRender(){
  if(framePending||!renderer)return;
  framePending=true;
  requestAnimationFrame(()=>{framePending=false;renderer.render(scene,camera);});
}
function resize(){
  if(!renderer)return;
  const width=viewport.clientWidth,height=viewport.clientHeight;
  if(!width||!height)return;
  renderer.setSize(width,height,false);camera.aspect=width/height;camera.updateProjectionMatrix();requestRender();
}
function markPreset(name){
  preset=name;
  document.querySelectorAll('[data-camera]').forEach(button=>button.setAttribute('aria-pressed',String(button.dataset.camera===name)));
}
function setPreset(name){
  if(!ready||!cameras[name])return;
  const data=cameras[name];applyingPreset=true;
  camera.position.fromArray(data.position);camera.fov=Number(data.fov)||40;camera.updateProjectionMatrix();controls.target.fromArray(data.target);controls.update();applyingPreset=false;
  markPreset(name);requestRender();
}
function moveCamera(theta=0,phi=0,scale=1){
  if(!ready)return;
  const offset=camera.position.clone().sub(controls.target);
  const spherical=new THREE.Spherical().setFromVector3(offset);
  spherical.theta+=theta;spherical.phi=THREE.MathUtils.clamp(spherical.phi+phi,.08,controls.maxPolarAngle);
  spherical.radius=THREE.MathUtils.clamp(spherical.radius*scale,controls.minDistance,controls.maxDistance);
  camera.position.copy(controls.target).add(new THREE.Vector3().setFromSpherical(spherical));controls.update();markPreset('custom');requestRender();
}
function fail(){
  ready=false;status.dataset.state='error';status.textContent='Chế độ 3D chưa mở được. Bạn có thể xem ảnh render bên dưới.';
  $('viewer-error').hidden=false;
}
function init(){
  try{
    renderer=new THREE.WebGLRenderer({canvas:$('scene-canvas'),antialias:true,alpha:false,powerPreference:'default'});
    renderer.setPixelRatio(Math.min(window.devicePixelRatio||1,2));
    renderer.outputColorSpace=THREE.SRGBColorSpace;renderer.toneMapping=THREE.NeutralToneMapping;renderer.toneMappingExposure=1;
    renderer.shadowMap.enabled=true;renderer.shadowMap.type=THREE.PCFSoftShadowMap;
    scene=new THREE.Scene();scene.background=new THREE.Color('#d7dcd7');
    camera=new THREE.PerspectiveCamera(40,1,.1,250);
    camera.position.fromArray(defaults.overview.position);
    controls=new OrbitControls(camera,$('scene-canvas'));
    controls.enableDamping=false;controls.minDistance=2;controls.maxDistance=70;controls.maxPolarAngle=Math.PI/2+.18;controls.minPolarAngle=.08;controls.enablePan=true;
    controls.target.fromArray(defaults.overview.target);controls.update();
    controls.addEventListener('change',requestRender);
    controls.addEventListener('start',()=>{if(!applyingPreset)markPreset('custom');});
    scene.add(new THREE.HemisphereLight(0xf2f2ec,0x62695a,2.0));
    const key=new THREE.DirectionalLight(0xffffff,2.6);key.position.set(-10,18,9);key.target.position.set(0,0,0);key.castShadow=true;
    key.shadow.mapSize.set(2048,2048);Object.assign(key.shadow.camera,{left:-18,right:18,top:18,bottom:-18,near:.5,far:65});key.shadow.bias=-.00025;key.shadow.normalBias=.025;key.shadow.radius=3;
    scene.add(key,key.target);
    new ResizeObserver(resize).observe(viewport);window.addEventListener('resize',resize);resize();
    const encoded=window.BEN_VAC_SCENE?.glbBase64;
    if(typeof encoded!=='string'||!encoded.length){fail();return;}
    const bytes=Uint8Array.from(atob(encoded),c=>c.charCodeAt(0));
    new GLTFLoader().parse(bytes.buffer,'',gltf=>{
      model=gltf.scene;
      model.traverse(object=>{
        if(object.isLight)object.visible=false;
        if(object.isMesh){
          object.castShadow=true;object.receiveShadow=true;
          const materials=Array.isArray(object.material)?object.material:[object.material];
          materials.forEach(material=>{materialList.push(material);if('metalness'in material)material.metalness=0;if('roughness'in material)material.roughness=Math.max(.72,material.roughness);});
        }
      });
      materialList=[...new Set(materialList)];scene.add(model);ready=true;status.dataset.state='ready';status.textContent='Cảnh 3D đã sẵn sàng. Có thể xoay và phóng.';setPreset('overview');
    },()=>fail());
    $('scene-canvas').addEventListener('webglcontextlost',event=>{event.preventDefault();fail();});
  }catch(error){fail();}
}
document.querySelectorAll('[data-camera]').forEach(button=>button.addEventListener('click',()=>setPreset(button.dataset.camera)));
$('reset-camera').addEventListener('click',()=>setPreset('overview'));
$('wireframe').addEventListener('change',()=>{for(const material of materialList){material.wireframe=$('wireframe').checked;material.needsUpdate=true;}requestRender();});
$('text-scale').addEventListener('change',event=>{document.documentElement.style.fontSize=event.target.value+'%';resize();});
viewport.addEventListener('keydown',event=>{
  const presets={'1':'overview','2':'eye','3':'repair'};
  if(presets[event.key]){event.preventDefault();setPreset(presets[event.key]);return;}
  const movements={ArrowLeft:[-.12,0,1],ArrowRight:[.12,0,1],ArrowUp:[0,-.08,1],ArrowDown:[0,.08,1],'+':[0,0,.9],'=':[0,0,.9],'-':[0,0,1.1]};
  if(movements[event.key]){event.preventDefault();moveCamera(...movements[event.key]);}
});
const renderLabels={overview:'Nhìn chéo từ cao',eye:'Ngang tầm người',repair:'Gần khu sửa thuyền'};
function openImage(source,title,trigger){opener=trigger;$('image-title').textContent=title;$('dialog-image').src=source;$('dialog-image').alt=title;$('image-dialog').showModal();$('close-image').focus();}
document.querySelectorAll('[data-render]').forEach(button=>button.addEventListener('click',()=>openImage('assets/render-'+button.dataset.render+'.png',renderLabels[button.dataset.render],button)));
$('reference-image').addEventListener('click',event=>openImage('../experiment-03/assets/b-gouache.png','Ảnh tham chiếu — Hướng B, vòng 03',event.currentTarget));
function closeImage(){$('image-dialog').close();opener?.focus();}
$('close-image').addEventListener('click',closeImage);
$('image-dialog').addEventListener('cancel',event=>{event.preventDefault();closeImage();});
init();
