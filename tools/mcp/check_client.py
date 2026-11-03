"""Local transport/privacy controls; no live generation or credentials."""
import asyncio
import json
import unittest
from hyper3d import Client,safe_summary

class Input:
    def __init__(self):self.messages=[]
    def write(self,data):self.messages.append(json.loads(data))
    async def drain(self):pass
class Output:
    def __init__(self,lines=None):self.lines=list(lines or [])
    async def readline(self):
        if self.lines:return (json.dumps(self.lines.pop(0))+'\n').encode()
        await asyncio.sleep(60)
class Process:
    def __init__(self,lines=None):self.stdin=Input();self.stdout=Output(lines)

class Controls(unittest.IsolatedAsyncioTestCase):
    async def test_timeout_reports_ambiguity_without_repeating_request(self):
        process=Process();client=Client(process)
        with self.assertRaisesRegex(TimeoutError,'do not automatically repeat generation'):
            await client.rpc('mcpServer/tool/call',{'tool':'rodin_generate'},timeout=.01)
        self.assertEqual(len(process.stdin.messages),1)
    async def test_notifications_do_not_repeat_or_replace_the_response(self):
        process=Process([{'method':'progress','params':{}},{'id':1,'result':{'ok':True}}]);client=Client(process)
        self.assertEqual(await client.rpc('read-only',{}),{'ok':True})
        self.assertEqual(len(process.stdin.messages),1)
    async def test_interactive_request_is_not_silently_approved(self):
        process=Process([{'id':9,'method':'permission/request','params':{}}]);client=Client(process)
        with self.assertRaisesRegex(RuntimeError,'requires interactive'):
            await client.rpc('mcpServer/tool/call',{})
        self.assertEqual(len(process.stdin.messages),1)
    def test_only_completed_public_links_reach_summary(self):
        data={'generation_id':'example','status':'queued','display_url':'https://hyper3d.ai/workspace/rodin/example','files':[{'url':'https://example.invalid/?signature=private'}]}
        queued=safe_summary('rodin_generate',{'structuredContent':data},'evidence.json')
        self.assertNotIn('display_url',queued)
        data['status']='completed'
        completed=safe_summary('rodin_generate',{'structuredContent':data},'evidence.json')
        self.assertEqual(completed['display_url'],data['display_url'])
        self.assertNotIn('signature=private',json.dumps(completed))

if __name__=='__main__':unittest.main()
