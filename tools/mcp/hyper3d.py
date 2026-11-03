#!/usr/bin/env python3
"""Call the configured Hyper3D MCP through Codex; credentials stay with Codex.

No model inference, token extraction or automatic tool-call retries. Generation
may consume the authorized Hyper3D workspace's credits. Store raw responses in
ignored/private evidence, as result responses may contain temporary download URLs.
"""
import argparse
import asyncio
import json
import os
from pathlib import Path
import signal
import sys
import time

ROOT = Path(__file__).resolve().parents[2]
CODEX_PACKAGE = '@openai/codex@0.165.0'

class Client:
    def __init__(self, process):
        self.process = process
        self.sequence = 0

    async def notify(self, method):
        self.process.stdin.write((json.dumps({'jsonrpc':'2.0','method':method})+'\n').encode())
        await self.process.stdin.drain()

    async def rpc(self, method, params, timeout=90):
        self.sequence += 1
        request_id = self.sequence
        self.process.stdin.write((json.dumps({'jsonrpc':'2.0','id':request_id,'method':method,'params':params})+'\n').encode())
        await self.process.stdin.drain()
        deadline = time.monotonic()+timeout
        while True:
            remaining = deadline-time.monotonic()
            if remaining <= 0:
                raise TimeoutError('Request timed out; do not automatically repeat generation. Check Hyper3D Mine.')
            try:
                line = await asyncio.wait_for(self.process.stdout.readline(),remaining)
            except asyncio.TimeoutError as exc:
                raise TimeoutError('Request timed out; do not automatically repeat generation. Check Hyper3D Mine.') from exc
            if not line:
                raise RuntimeError('Codex MCP transport closed')
            message = json.loads(line)
            if message.get('id') == request_id and 'method' not in message:
                if 'error' in message:
                    raise RuntimeError('Codex RPC failed: '+str(message['error'].get('message','unknown')))
                return message['result']
            if 'method' in message and 'id' in message:
                raise RuntimeError('MCP requires interactive client handling: '+message['method'])


def private_write(path, value):
    path = path.resolve()
    path.parent.mkdir(parents=True,exist_ok=True)
    fd = os.open(path,os.O_WRONLY|os.O_CREAT|os.O_TRUNC,0o600)
    try:
        with os.fdopen(fd,'w') as stream:
            json.dump(value,stream,ensure_ascii=False,indent=2)
            stream.write('\n')
        os.chmod(path,0o600)
    except BaseException:
        raise


def payload(result):
    if isinstance(result.get('structuredContent'),dict):
        return result['structuredContent']
    for item in result.get('content',[]):
        if item.get('type')=='text':
            try:
                value=json.loads(item['text'])
                if isinstance(value,dict):return value
            except (json.JSONDecodeError,KeyError):pass
    return {}


def safe_summary(tool, result, output):
    data=payload(result)
    summary={'server':'hyper3d','tool':tool,'isError':bool(result.get('isError')),'output':str(output),'payload_keys':list(data)}
    for key in ['generation_id','asset_id','status']:
        if key in data:summary[key]=data[key]
    completed=data.get('status')=='completed' or (tool=='rodin_get_result' and bool(data.get('files')))
    if completed and data.get('display_url'):summary['display_url']=data['display_url']
    return summary


async def run(args):
    command=['npm','exec','--yes','--package',CODEX_PACKAGE,'--','codex','-c','service_tier="fast"','app-server']
    environment=dict(os.environ)
    # Select the real application configuration, rather than Snap's separate home.
    environment['CODEX_HOME']=str(Path.home()/'.codex')
    process=await asyncio.create_subprocess_exec(*command,cwd=ROOT,env=environment,
        stdin=asyncio.subprocess.PIPE,stdout=asyncio.subprocess.PIPE,stderr=asyncio.subprocess.DEVNULL,
        start_new_session=True,limit=8*1024*1024)
    client=Client(process)
    try:
        await client.rpc('initialize',{'clientInfo':{'name':'mythical-hyper3d-client','version':'0.1'},'capabilities':{'experimentalApi':True}})
        await client.notify('initialized')
        status=await client.rpc('mcpServerStatus/list',{})
        servers=[s for s in status['data'] if s['name']=='hyper3d']
        if len(servers)!=1 or servers[0]['authStatus']!='oAuth':
            raise RuntimeError('Configured hyper3d OAuth connection is not ready')
        server=servers[0]
        if args.command=='catalog':
            private_write(args.output,server)
            print(json.dumps({'server':'hyper3d','auth':server['authStatus'],'tools':list(server['tools']),'output':str(args.output)}))
            return 0
        if args.tool not in server['tools'] and args.tool not in [t.get('name') for t in server['tools'].values()]:
            raise RuntimeError('Tool not advertised by the connected Hyper3D server')
        arguments=json.loads(args.arguments.read_text())
        if not isinstance(arguments,dict):raise ValueError('Tool arguments must be a JSON object')
        thread=await client.rpc('thread/start',{'cwd':str(ROOT),'ephemeral':True,'approvalPolicy':'never','sandbox':'danger-full-access'})
        result=await client.rpc('mcpServer/tool/call',{'server':'hyper3d','threadId':thread['thread']['id'],'tool':args.tool,'arguments':arguments},args.timeout)
        private_write(args.output,result)
        print(json.dumps(safe_summary(args.tool,result,args.output),ensure_ascii=False))
        return 1 if result.get('isError') else 0
    finally:
        try:os.killpg(process.pid,signal.SIGTERM)
        except ProcessLookupError:pass
        try:await asyncio.wait_for(process.wait(),10)
        except asyncio.TimeoutError:
            try:os.killpg(process.pid,signal.SIGKILL)
            except ProcessLookupError:pass
            await process.wait()


def main():
    parser=argparse.ArgumentParser(description=__doc__)
    parser.add_argument('command',choices=['catalog','call'])
    parser.add_argument('--tool')
    parser.add_argument('--arguments',type=Path)
    parser.add_argument('--output',type=Path,required=True)
    parser.add_argument('--timeout',type=int,default=180)
    args=parser.parse_args()
    if args.command=='call' and (not args.tool or not args.arguments):parser.error('call requires --tool and --arguments')
    try:return asyncio.run(run(args))
    except (TimeoutError,RuntimeError,ValueError,OSError) as error:
        print(str(error),file=sys.stderr)
        return 2

if __name__=='__main__':raise SystemExit(main())
