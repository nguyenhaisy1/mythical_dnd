/* Experimental UX model. Not the production generator or sha256-counter-v1. */
(function (root, factory) {
  const api = factory();
  if (typeof module === 'object' && module.exports) module.exports = api;
  else root.MythicalCampaign = api;
})(typeof globalThis !== 'undefined' ? globalThis : this, function () {
  'use strict';
  const VERSION = 's13-origin-experiment-v1';
  const labels = { the: 'Thể', xao: 'Xảo', giac: 'Giác', tri: 'Trí', tam: 'Tâm', khi: 'Khí' };
  // Every context/episode below is prototype-authored fiction, not a canonical NPC.
  const contexts = [
    { id: 'river', household: 'Nhà giữ dây neo', origin: 'Lớn lên trong một gia đình làm việc bên bến.', names: ['An', 'Nâu', 'Duyên'], obligation: 'Trở về phụ gia đình kiểm dây neo trước chiều.',
      childhood: { id: 'river-child', age: 10, text: 'Theo người nhà kiểm dây neo mỗi sáng; học cách nhìn vệt nước.', skill: 'rivercraft', label: 'Việc sông nước', gain: 12, attrs: { giac: 1 }, fact: 'Những vệt nước trên cọc bến giúp bạn nhận ra mực nước đã đổi.' },
      paths: [
        { id: 'river-rope', age: 18, text: 'Một mùa làm phụ việc buộc neo; đã tự thay những đoạn dây sờn.', skill: 'ropework', label: 'Buộc và sửa dây', gain: 18, attrs: { xao: 2 }, fact: 'Bạn biết cách kiểm đoạn dây sờn bằng mắt và tay.' },
        { id: 'river-ferry', age: 19, text: 'Theo một chuyến đò ngắn nhiều lần, học cách chuẩn bị và hỏi người dẫn đò.', skill: 'rivercraft', label: 'Việc sông nước', gain: 16, attrs: { the: 1, giac: 1 }, fact: 'Bạn từng đi chuyến đò gần bến; chưa biết tình trạng tuyến vào hôm nay.' }
      ] },
    { id: 'wood', household: 'Nhà bên mái sửa gỗ', origin: 'Lớn lên trong gia đình phụ việc gỗ tại Bến Vạc.', names: ['Bền', 'Thắm', 'Đượm'], obligation: 'Giúp nhà thu xếp dụng cụ sau buổi làm.',
      childhood: { id: 'wood-child', age: 11, text: 'Nhặt và phân loại gỗ thừa cùng người nhà, học tên những dụng cụ đơn giản.', skill: 'woodwork', label: 'Việc gỗ', gain: 12, attrs: { xao: 1 }, fact: 'Bạn nhận ra dụng cụ gỗ quen dùng trong gia đình.' },
      paths: [
        { id: 'wood-patch', age: 18, text: 'Phụ vá một tấm ván dưới sự chỉ dẫn; hiểu rằng nhìn vết nứt chưa đủ kết luận cả thuyền.', skill: 'woodwork', label: 'Việc gỗ', gain: 18, attrs: { xao: 1, tri: 1 }, fact: 'Bạn đã phụ vá ván, nhưng chưa kiểm tra con thuyền ở bến hôm nay.' },
        { id: 'wood-carry', age: 20, text: 'Chuyển gỗ cùng nhóm thợ và tập ghi nhớ từng phần được giao.', skill: 'hauling', label: 'Chuyển vật liệu', gain: 16, attrs: { the: 2 }, fact: 'Bạn biết cách phối hợp nâng vật dài cùng người khác.' }
      ] },
    { id: 'grain', household: 'Nhà cạnh sân phơi', origin: 'Lớn lên trong gia đình phơi và cất lương thực ở Bến Vạc.', names: ['Mầm', 'Lành', 'Sớm'], obligation: 'Về giúp nhà thu đồ đang phơi khi trời chuyển.',
      childhood: { id: 'grain-child', age: 10, text: 'Phụ trải và gom đồ phơi, học cách nhận ra phần bị ẩm.', skill: 'provisioning', label: 'Giữ lương thực', gain: 12, attrs: { giac: 1 }, fact: 'Bạn biết dấu hiệu ẩm ở đồ phơi quen thuộc.' },
      paths: [
        { id: 'grain-store', age: 19, text: 'Được giao kiểm những phần gia đình cất giữ; tập ghi nhớ thứ tự dùng trước.', skill: 'provisioning', label: 'Giữ lương thực', gain: 18, attrs: { tri: 2 }, fact: 'Bạn biết việc nhà mình đã giao, không biết kho của các nhà khác.' },
        { id: 'grain-help', age: 18, text: 'Cùng hàng xóm gom đồ khi mưa đến; học hỏi trước khi động vào phần của người khác.', skill: 'cooperation', label: 'Phối hợp việc chung', gain: 16, attrs: { tam: 1, khi: 1 }, fact: 'Bạn nhớ phải hỏi chủ nhà trước khi dùng đồ của họ.' }
      ] }
  ];
  function freeze(value) {
    if (value && typeof value === 'object') {
      Object.values(value).forEach(freeze);
      Object.freeze(value);
    }
    return value;
  }
  freeze(contexts);
  function requireToken(value, name) {
    if (typeof value !== 'string' || !value.trim() || value.length > 256)
      throw new TypeError(name + ' must be a nonempty string of at most 256 UTF-16 units');
  }
  // Purpose-scoped FNV-1a over UTF-16 code units, then one xorshift32 step.
  // Delimiters are unambiguous because input is a serialized JSON tuple.
  function draw(seed, purpose, count) {
    const input = JSON.stringify([VERSION, seed, purpose]);
    let word = 2166136261;
    for (let i = 0; i < input.length; i++) word = Math.imul(word ^ input.charCodeAt(i), 16777619) >>> 0;
    word ^= word << 13; word ^= word >>> 17; word ^= word << 5;
    return (word >>> 0) % count;
  }
  function createCampaign(seed, campaignId) {
    requireToken(seed, 'seed'); requireToken(campaignId, 'campaignId');
    const context = contexts[draw(seed, 'context', contexts.length)];
    const episode = context.paths[draw(seed, 'history:' + context.id, context.paths.length)];
    const episodes = [context.childhood, episode];
    const age = episode.age + draw(seed, 'elapsed-years', 7);
    const skillMap = {};
    const attributes = Object.entries(labels).map(([id, label]) => ({ id, label, value: 3, historyIds: [] }));
    for (const entry of episodes) {
      const skill = skillMap[entry.skill] || (skillMap[entry.skill] = { id: entry.skill, label: entry.label, value: 0, historyIds: [] });
      skill.value += entry.gain; skill.historyIds.push(entry.id);
      for (const [id, delta] of Object.entries(entry.attrs)) {
        const attribute = attributes.find(item => item.id === id);
        attribute.value += delta; attribute.historyIds.push(entry.id);
      }
    }
    const actor = freeze({
      id: campaignId + ':actor:1', name: context.names[draw(seed, 'name:' + context.id, context.names.length)], age, alive: true,
      community: 'Bến Vạc', household: context.household, origin: context.origin,
      history: episodes.map(entry => ({ id: entry.id, age: entry.age, text: entry.text })),
      attributes, skills: Object.values(skillMap),
      knownFacts: episodes.map(entry => ({ id: entry.id + ':memory', text: entry.fact, sourceHistoryId: entry.id, source: 'Trải nghiệm của bản thân', confidence: 'Ký ức cá nhân' })),
      obligation: context.obligation,
      viability: { passed: age >= 18 && episodes.every(entry => entry.age <= age), text: 'Người trưởng thành, còn sống; có việc quen thuộc và nơi nương tựa trong mô hình thử nghiệm.' },
      status: 'experimental-pending-approval'
    });
    if (!actor.viability.passed) throw new Error('Experimental origin is not viable');
    // The session captures one actor. Revealing never consumes another draw.
    return freeze({ campaignId, version: VERSION, reveal: () => actor });
  }
  return freeze({ VERSION, createCampaign });
});
