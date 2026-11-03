import fs from 'node:fs/promises';
import os from 'node:os';
import path from 'node:path';
import {fileURLToPath,pathToFileURL} from 'node:url';
import {spawnSync} from 'node:child_process';
import {createHash} from 'node:crypto';
const root=path.dirname(fileURLToPath(import.meta.url));
const temp=await fs.mkdtemp(path.join(os.tmpdir(),'mythical-e04-viewer-'));
try {
  for(const name of ['package.json','package-lock.json'])await fs.copyFile(path.join(root,name),path.join(temp,name));
  const installed=spawnSync('npm',['ci','--ignore-scripts','--no-audit','--no-fund'],{cwd:temp,stdio:'inherit'});
  if(installed.status!==0)throw new Error('Pinned viewer dependencies did not install');
  const esbuild=await import(pathToFileURL(path.join(temp,'node_modules/esbuild/lib/main.js')).href);
  await fs.mkdir(path.join(root,'vendor'),{recursive:true});
  await esbuild.build({entryPoints:[path.join(root,'viewer.js')],outfile:path.join(root,'vendor/viewer.bundle.js'),bundle:true,format:'iife',target:['chrome110','firefox115','safari16'],nodePaths:[path.join(temp,'node_modules')],minify:true,supported:{'template-literal':false},legalComments:'inline'});
  await fs.copyFile(path.join(temp,'node_modules/three/LICENSE'),path.join(root,'vendor/THREE-LICENSE.txt'));
  const bytes=await fs.readFile(path.join(root,'vendor/viewer.bundle.js'));
  await fs.writeFile(path.join(root,'vendor/manifest.json'),JSON.stringify({three:'0.186.1',esbuild:'0.28.2',file:'viewer.bundle.js',sha256:createHash('sha256').update(bytes).digest('hex'),scope:'bundled local viewer only; no CDN or production dependency'},null,2)+'\n');
  console.log('Built offline viewer; dependency cache isolated from repository.');
} finally {await fs.rm(temp,{recursive:true,force:true});}
