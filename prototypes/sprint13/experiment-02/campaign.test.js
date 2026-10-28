'use strict';
const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const vm = require('node:vm');
const { createCampaign, VERSION } = require('./campaign.js');

test('same seed reproduces the same full actor and Reveal has no reroll', () => {
  const session = createCampaign('nước-01', 'campaign-1');
  const first = session.reveal();
  assert.strictEqual(session.reveal(), first);
  assert.deepEqual(createCampaign('nước-01', 'campaign-1').reveal(), first);
  const otherId = createCampaign('nước-01', 'campaign-2').reveal();
  assert.deepEqual({ ...otherId, id: first.id }, first);
  assert.equal(session.version, VERSION);
  assert.equal(session.reroll, undefined);
});
test('500 generated adults have ordered histories and capacities derived from those histories', () => {
  const origins = new Set(); const lives = new Set();
  const gains = { 'river-child': ['rivercraft', 12], 'river-rope': ['ropework', 18], 'river-ferry': ['rivercraft', 16], 'wood-child': ['woodwork', 12], 'wood-patch': ['woodwork', 18], 'wood-carry': ['hauling', 16], 'grain-child': ['provisioning', 12], 'grain-store': ['provisioning', 18], 'grain-help': ['cooperation', 16] };
  for (let i = 0; i < 500; i++) {
    const actor = createCampaign('fixture-' + i, 'c' + i).reveal();
    origins.add(actor.household); lives.add(actor.history.map(x => x.id).join(','));
    assert.ok(actor.alive && actor.age >= 18 && actor.viability.passed);
    assert.ok(actor.history[0].age < actor.history[1].age && actor.history[1].age <= actor.age);
    const expected = {};
    for (const event of actor.history) { const [id, value] = gains[event.id]; expected[id] = (expected[id] || 0) + value; }
    assert.deepEqual(Object.fromEntries(actor.skills.map(s => [s.id, s.value])), expected);
    for (const skill of actor.skills) assert.deepEqual(skill.historyIds, actor.history.filter(h => gains[h.id][0] === skill.id).map(h => h.id));
    for (const attribute of actor.attributes) {
      assert.ok(attribute.value >= 3 && attribute.value <= 10);
      assert.ok(attribute.historyIds.every(id => actor.history.some(h => h.id === id)));
      if (attribute.historyIds.length === 0) assert.equal(attribute.value, 3);
    }
    for (const fact of actor.knownFacts) assert.ok(actor.history.some(h => h.id === fact.sourceHistoryId));
  }
  assert.equal(origins.size, 3); assert.equal(lives.size, 6);
});
test('nested DTO immutable and separate sessions share no mutable references', () => {
  const a = createCampaign('a', '1').reveal(); const b = createCampaign('a', '1').reveal();
  assert.notStrictEqual(a, b); assert.notStrictEqual(a.history, b.history);
  assert.throws(() => { a.history[0].text = 'changed'; }, TypeError);
  assert.throws(() => a.skills[0].historyIds.push('invented'), TypeError);
  assert.throws(() => { a.knownFacts[0].text = 'secret'; }, TypeError);
  assert.deepEqual(a, b);
});
test('rejects invalid identifiers and seeds instead of silently falling back', () => {
  for (const value of [undefined, null, 3, '', ' ', 'x'.repeat(257)]) {
    assert.throws(() => createCampaign(value, 'c'), TypeError);
    assert.throws(() => createCampaign('s', value), TypeError);
  }
});
test('browser export matches CommonJS and needs no clock, storage or random global', () => {
  const context = vm.createContext({});
  vm.runInContext('Math.random = () => { throw new Error("forbidden"); }; Date = undefined;', context);
  vm.runInContext(fs.readFileSync(__dirname + '/campaign.js', 'utf8'), context);
  const actor = context.MythicalCampaign.createCampaign('nước-01', 'campaign-1').reveal();
  assert.deepEqual(JSON.parse(JSON.stringify(actor)), createCampaign('nước-01', 'campaign-1').reveal());
  assert.deepEqual(Object.keys(actor).sort(), ['id','name','age','alive','community','household','origin','history','attributes','skills','knownFacts','obligation','viability','status'].sort());
});
