(function () {
  'use strict';
  const $ = id => document.getElementById(id);
  const screens = ['menu', 'generating', 'reveal', 'world'];
  let current = 'menu';
  let actor = null;
  let generationTimer = null;
  let campaignNumber = 0;
  let opener = null;
  function show(name) {
    screens.forEach(id => { $(id).hidden = id !== name; });
    current = name;
    $('journal-button').hidden = name !== 'world';
    $('main').focus();
    window.scrollTo(0, 0);
  }
  function closeDialog(dialog) {
    dialog.close();
    if (opener && opener.isConnected && !opener.closest('[hidden]')) opener.focus();
  }
  function openDialog(id, source) {
    opener = source || document.activeElement;
    $(id).showModal();
  }
  function returnMenu(event) {
    if (event) event.preventDefault();
    clearTimeout(generationTimer);
    document.querySelectorAll('dialog[open]').forEach(closeDialog);
    show('menu');
  }
  function renderActor() {
    $('character-name').textContent = actor.name;
    $('character-origin').textContent = actor.age + ' tuổi · ' + actor.household + ' · ' + actor.community;
    $('character-biography').textContent = actor.origin + ' ' + actor.history.map(item => item.text).join(' ') + ' ' + actor.obligation;
    $('character-skills').replaceChildren();
    actor.skills.forEach(skill => {
      const item = document.createElement('li');
      const title = document.createElement('strong'); title.textContent = skill.label;
      const sources = document.createElement('ul'); sources.className = 'skill-sources';
      skill.historyIds.forEach(id => {
        const episode = actor.history.find(entry => entry.id === id);
        if (!episode) return;
        const source = document.createElement('li');
        source.textContent = 'Khi ' + episode.age + ' tuổi: ' + episode.text;
        sources.append(source);
      });
      item.append(title, sources); $('character-skills').append(item);
    });
    $('world-name').textContent = actor.name;
    $('world-household').textContent = actor.household;
    $('claims').replaceChildren();
    actor.knownFacts.forEach(fact => {
      const item = document.createElement('article'); item.className = 'claim';
      const text = document.createElement('p'); text.textContent = fact.text;
      const source = document.createElement('small');
      const history = actor.history.find(entry => entry.id === fact.sourceHistoryId);
      source.textContent = (fact.source || 'Trải nghiệm bản thân') + ' · ' + (history ? 'Khi ' + history.age + ' tuổi' : 'Thời điểm chưa rõ') + ' · ' + (fact.confidence || 'Ký ức cá nhân');
      item.append(text, source); $('claims').append(item);
    });
    $('result-status').textContent = '';
  }
  $('new-campaign').addEventListener('click', () => {
    show('generating');
    $('generation-status').textContent = 'Đang dựng một cuộc đời mẫu từ những trải nghiệm đã có.';
    // Entropy only selects a new UX experiment seed. The model is deterministic
    // for that seed; no production world, gameplay command or save is touched.
    generationTimer = window.setTimeout(() => {
      try {
        const entropy = new Uint32Array(4);
        window.crypto.getRandomValues(entropy);
        campaignNumber += 1;
        const session = window.MythicalCampaign.createCampaign(Array.from(entropy).join('-'), 'preview-' + campaignNumber);
        actor = session.reveal(); renderActor(); show('reveal');
      } catch (error) {
        $('generation-status').textContent = 'Không thể mở mẫu nhân vật. Hãy kiểm tra đủ các tệp của bản thử nghiệm rồi thử lại.';
      }
    }, 500);
  });
  $('cancel-generation').addEventListener('click', returnMenu);
  $('home').addEventListener('click', returnMenu);
  $('return-menu').addEventListener('click', returnMenu);
  $('begin').addEventListener('click', () => show('world'));
  ['menu-art','world-art'].forEach(id => $(id).addEventListener('click', event => openDialog('art-dialog', event.currentTarget)));
  ['journal-button','open-journal'].forEach(id => $(id).addEventListener('click', event => openDialog('journal-dialog', event.currentTarget)));
  ['preview-action','boat-marker'].forEach(id => $(id).addEventListener('click', event => openDialog('action-dialog', event.currentTarget)));
  document.querySelectorAll('[data-close]').forEach(button => button.addEventListener('click', () => closeDialog(button.closest('dialog'))));
  document.querySelectorAll('dialog').forEach(dialog => dialog.addEventListener('cancel', event => { event.preventDefault(); closeDialog(dialog); }));
  $('confirm-action').addEventListener('click', () => {
    closeDialog($('action-dialog'));
    $('result-status').textContent = 'Đã thử bước xác nhận. Trong game, kết quả sẽ xuất hiện ở đây sau khi luật được xử lý. Bản thử này chưa tiêu tốn thời gian, vật liệu hoặc ghi bản lưu.';
  });
  $('text-scale').addEventListener('change', event => {
    document.documentElement.style.fontSize = event.target.value + '%';
  });
  document.addEventListener('keydown', event => {
    if (event.key.toLowerCase() === 'j' && current === 'world' && !event.ctrlKey && !event.metaKey && !event.altKey && !document.querySelector('dialog[open]') && !['INPUT','TEXTAREA','SELECT'].includes(document.activeElement.tagName)) {
      event.preventDefault(); openDialog('journal-dialog');
    }
  });
})();
