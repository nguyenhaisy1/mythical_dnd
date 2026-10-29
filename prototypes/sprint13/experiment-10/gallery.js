'use strict';
(() => {
  const choices = {
    "men-cham": {
        "title": "Men chàm",
        "src": "assets/men-cham.png",
        "alt": "Bảng concept Men chàm: vật liệu gốm men xanh chàm và trắng ngà thử xuyên thế giới, nhân vật, chân dung, vật phẩm và giao diện.",
        "description": "Men chàm thử nối bề mặt gốm, nét xanh chàm và khoảng sáng ngà từ nhân vật đến icon, khung hội thoại và túi đồ. Quan sát xem nét mặt và chữ có còn dễ đọc trong cùng chất liệu."
    },
    "bong-roi": {
        "title": "Bóng rối",
        "src": "assets/bong-roi.png",
        "alt": "Bảng concept Bóng rối: hình cắt và ánh sáng xuyên lớp thử xuyên cảnh chơi, nhân vật, chân dung, vật phẩm và giao diện.",
        "description": "Bóng rối thử silhouette, lớp sáng xuyên và nét cắt trên cả con người lẫn bảng thông tin. Quan sát xem biểu cảm, vật phẩm và lựa chọn có phân biệt rõ khi dùng ít màu."
    }
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
  let selected = 'men-cham';
  function select(key) {
    if (!choices[key]) return;
    selected = key;
    const choice = choices[key];
    error.hidden = true;
    hero.src = choice.src;
    hero.alt = choice.alt;
    title.textContent = choice.title;
    description.textContent = choice.description;
    enlarge.setAttribute('aria-label', `Mở concept ${choice.title} toàn khung`);
    document.querySelectorAll('.switches [data-choice]').forEach(button => button.setAttribute('aria-pressed', String(button.dataset.choice === key)));
    status.textContent = `Đang xem concept ${choice.title}. UI trong ảnh là minh họa, chưa phải game chạy.`;
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
    const key = { '1': 'men-cham', '2': 'bong-roi', m: 'men-cham', r: 'bong-roi' }[event.key.toLowerCase()];
    if (key) { event.preventDefault(); select(key); }
  });
})();
