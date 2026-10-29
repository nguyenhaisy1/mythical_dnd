'use strict';
(() => {
  const choices = {
    a: { title: 'A — Sơn mài sống', src: 'assets/a-lacquer.png', alt: 'Bản concept A: ngôn ngữ sơn mài thử cho thế giới, tạo hình nhân vật, chân dung, vật phẩm và giao diện.', description: 'Sơn mài thử nối mảng đen, sắc son, ánh kim và nét khắc giữa cảnh, trang phục, gương mặt và các khung giao diện.' },
    b: { title: 'B — Phù điêu đồng', src: 'assets/b-bronze.png', alt: 'Bản concept B: ngôn ngữ phù điêu đồng thử xuyên cảnh, nhân vật, chân dung, vật phẩm và giao diện.', description: 'Phù điêu đồng thử cấu trúc nổi/chìm và bề mặt kim loại trên cả thế giới, chân dung, biểu tượng và các bảng UI.' },
    c: { title: 'C — Mực sống', src: 'assets/c-ink.png', alt: 'Bản concept C: mực đen trắng với nét đỏ thử xuyên cảnh, tạo hình, chân dung, vật phẩm và giao diện.', description: 'Mực sống thử nét biểu cảm, mảng đen/trắng, điểm đỏ và khoảng trống để liên kết cảnh, con người và thông tin giao diện.' }
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
    const choice = choices[key];
    error.hidden = true;
    hero.src = choice.src;
    hero.alt = choice.alt;
    title.textContent = choice.title;
    description.textContent = choice.description;
    enlarge.setAttribute('aria-label', `Mở bản concept ${key.toUpperCase()} toàn khung`);
    document.querySelectorAll('.switches [data-choice]').forEach(button => button.setAttribute('aria-pressed', String(button.dataset.choice === key)));
    status.textContent = `Đang xem phương án ${choice.title}. UI trong ảnh là minh họa, chưa phải game chạy.`;
  }
  document.querySelectorAll('[data-choice]').forEach(button => button.addEventListener('click', () => select(button.dataset.choice)));
  hero.addEventListener('error', () => { error.hidden = false; });
  hero.addEventListener('load', () => { error.hidden = true; });
  if (hero.complete && !hero.naturalWidth) error.hidden = false;
  enlarge.addEventListener('click', () => {
    const choice = choices[selected];
    dialogImage.src = choice.src;
    document.querySelector('#original-image').href = choice.src;
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
  document.querySelector('#text-size').addEventListener('change', event => document.documentElement.style.setProperty('--text-scale', String(Number(event.target.value) / 100)));
  document.addEventListener('keydown', event => {
    if (dialog.open || event.altKey || event.ctrlKey || event.metaKey || event.target.closest('input,select,textarea,[contenteditable]')) return;
    const key = { '1': 'a', '2': 'b', '3': 'c', a: 'a', b: 'b', c: 'c' }[event.key.toLowerCase()];
    if (key) { event.preventDefault(); select(key); }
  });
})();
