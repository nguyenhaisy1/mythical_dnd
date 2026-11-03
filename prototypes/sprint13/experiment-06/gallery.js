'use strict';
(() => {
  const choices = {
    a: { title: 'A — Tranh góc chéo', src: 'assets/a-oblique.png', alt: 'Phương án A: bến sửa thuyền nhìn chéo từ trên cao, thể hiện bằng tranh 2D.', description: 'Góc chéo cho thấy lối đi, chỗ làm việc và dòng sông trong cùng một khung hình. Hướng này thử chất liệu tranh và cách phân lớp bằng màu tự nhiên.' },
    b: { title: 'B — Gần với con người', src: 'assets/b-grounded.png', alt: 'Phương án B: ảnh mô phỏng cảm giác game 3D, gần tầm người ở một bến sửa thuyền.', description: 'Góc nhìn gần tầm người thử cảm giác nhập vai qua chất liệu, tỷ lệ nhân vật và chiều sâu. Đây là một ảnh tạo, không phải render từ model 3D thật.' },
    c: { title: 'C — Một lát cắt đời sống', src: 'assets/c-sideview.png', alt: 'Phương án C: tranh 2.5D nhìn ngang bến sửa thuyền, với nhiều lớp không gian.', description: 'Góc ngang thử tổ chức đời sống thành các lớp cảnh: lối đi, xưởng làm việc và mặt nước. Đây là hướng bố cục độc lập, không phải camera khác của ảnh A hoặc B.' }
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
