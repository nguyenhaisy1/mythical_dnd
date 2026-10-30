'use strict';
(() => {
  const choices = {
    a: { title: 'A — Khắc gỗ · Mùa nước', src: 'assets/a-woodcut.png', alt: 'Phương án A: ý tưởng làng mùa nước và chuyển thóc lên kho cao, theo hướng tranh khắc gỗ màu phẳng.', description: 'Hướng khắc gỗ thử đường nét mạnh, mảng phẳng và số màu hạn chế để kể đời sống trong mùa nước.', hud: ['Làng mùa nước', 'Thử tình huống đưa thóc lên kho cao.', 'Xem đường khô', 'Hỏi hộ giữ kho', 'Giúp chuyển thóc'] },
    b: { title: 'B — Pixel art · Nghề đúc', src: 'assets/b-pixel.png', alt: 'Phương án B: ý tưởng hoạt động ở xưởng đúc đồng, theo hướng pixel art với cụm pixel rõ.', description: 'Hướng pixel art thử cụm điểm ảnh rõ, silhouette và ánh sáng được giản lược trong cảnh lao động ở xưởng đúc đồng.', hud: ['Xưởng đúc đồng', 'Thử một ngày theo nghề và quan sát mẻ đúc.', 'Xem khuôn đúc', 'Hỏi người thợ', 'Kiểm đồ nghề'] },
    c: { title: 'C — Giấy cắt · Chợ đêm', src: 'assets/c-papercut.png', alt: 'Phương án C: ý tưởng phiên chợ hoặc đêm hội ven sông, theo hướng giấy cắt nhiều lớp.', description: 'Hướng giấy cắt thử silhouette, mép vật liệu và lớp cảnh chồng nhau để gợi những cuộc gặp ở một phiên chợ ven sông.', hud: ['Phiên chợ ven sông', 'Thử dừng chân giữa những câu chuyện làng.', 'Lắng nghe', 'Hỏi người bán', 'Đi qua bến'] }
  };
  const hero = document.querySelector('#hero-image');
  const title = document.querySelector('#scene-title');
  const description = document.querySelector('#scene-description');
  const status = document.querySelector('#selection-status');
  const error = document.querySelector('#image-error');
  const enlarge = document.querySelector('#enlarge');
  const dialog = document.querySelector('#image-dialog');
  const dialogImage = document.querySelector('#dialog-image');
  const dialogTitle = document.querySelector('#dialog-title');
  let selected = 'a';
  function select(key) {
    if (!choices[key]) return;
    selected = key;
    document.documentElement.dataset.art = key;
    const choice = choices[key];
    error.hidden = true;
    hero.src = choice.src;
    hero.alt = choice.alt;
    title.textContent = choice.title;
    description.textContent = choice.description;
    ['hud-title', 'hud-situation', 'hud-choice-1', 'hud-choice-2', 'hud-choice-3'].forEach((id, index) => { document.getElementById(id).textContent = choice.hud[index]; });
    enlarge.setAttribute('aria-label', `Mở ảnh phương án ${key.toUpperCase()} toàn khung`);
    document.querySelectorAll('.switches [data-choice]').forEach(button => button.setAttribute('aria-pressed', String(button.dataset.choice === key)));
    status.textContent = `Đang xem phương án ${choice.title}. Ảnh ý tưởng độc lập, chưa phải game chạy.`;
  }
  document.querySelectorAll('[data-choice]').forEach(button => button.addEventListener('click', () => select(button.dataset.choice)));
  hero.addEventListener('error', () => { error.hidden = false; });
  hero.addEventListener('load', () => { error.hidden = true; });
  if (hero.complete && !hero.naturalWidth) error.hidden = false;
  enlarge.addEventListener('click', () => {
    const choice = choices[selected];
    dialogImage.src = choice.src;
    dialogImage.alt = choice.alt;
    dialogTitle.textContent = choice.title;
    dialog.showModal();
  });
  document.querySelector('#close-dialog').addEventListener('click', () => dialog.close());
  dialog.addEventListener('close', () => enlarge.focus());
  dialog.addEventListener('click', event => {
    const bounds = dialog.getBoundingClientRect();
    if (event.target === dialog && (event.clientX < bounds.left || event.clientX > bounds.right || event.clientY < bounds.top || event.clientY > bounds.bottom)) dialog.close();
  });
  document.querySelector('#show-hud').addEventListener('change', event => { document.querySelector('.mock-hud').hidden = !event.target.checked; });
  document.querySelector('#text-size').addEventListener('change', event => document.documentElement.style.setProperty('--text-scale', String(Number(event.target.value) / 100)));
  document.addEventListener('keydown', event => {
    if (dialog.open || event.altKey || event.ctrlKey || event.metaKey || event.target.closest('input,select,textarea,[contenteditable]')) return;
    const key = { '1': 'a', '2': 'b', '3': 'c', a: 'a', b: 'b', c: 'c' }[event.key.toLowerCase()];
    if (key) { event.preventDefault(); select(key); }
  });
})();
