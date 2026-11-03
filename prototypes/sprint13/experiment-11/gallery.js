'use strict';
(() => {
  const choices = {
  "layered-25d": {
    "title": "2.5D nhiều lớp",
    "src": "assets/layered-25d.png",
    "alt": "Bảng concept 2.5D nhiều lớp: cảnh chơi nhìn ngang với nhân vật vẽ tay, chân dung, vật phẩm và UI minh họa.",
    "description": "Thử không gian nhìn ngang bằng các lớp cảnh chồng nhau và nhân vật 2D. Quan sát lớp trước–sau, lối đi và độ đọc của người, chân dung và UI."
  },
  "stylized-3d": {
    "title": "3D cách điệu",
    "src": "assets/stylized-3d.png",
    "alt": "Bảng concept diện mạo 3D cách điệu: khối cảnh và người, cùng chân dung, vật phẩm và UI; không phải model thật.",
    "description": "Thử thể tích, mặt phẳng vật liệu và góc nhìn ba phần tư trên một thế giới có quy mô đời thường. Đây là ảnh mô phỏng diện mạo, chưa có camera tự do hoặc model 3D thật."
  },
  "pixel-life": {
    "title": "Pixel đời làng",
    "src": "assets/pixel-life.png",
    "alt": "Bảng concept pixel art đời làng với cụm pixel ở cảnh, nhân vật trưởng thành, chân dung, vật phẩm và UI.",
    "description": "Thử nhịp cụm pixel ở cả cảnh, sprite nhân vật, chân dung, icon và khung UI. Ảnh concept chưa là tileset, spritesheet hoặc grid pixel production đã chuẩn hóa."
  },
  "anime-life": {
    "title": "Anime đời thường",
    "src": "assets/anime-life.png",
    "alt": "Bảng concept anime đời thường với nữ lao động trưởng thành, nét mặt, cử chỉ, cảnh chơi, vật phẩm và UI đồng bộ.",
    "description": "Thử nét vẽ tay, mảng bóng và biểu cảm của những người trưởng thành trong sinh hoạt nghề nghiệp. Quan sát nét mặt và vóc dáng có đủ riêng thay vì mặc định cast là anh hùng trẻ."
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
  let selected = 'layered-25d';
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
    const key = { '1': 'layered-25d', '2': 'stylized-3d', '3': 'pixel-life', '4': 'anime-life' }[event.key.toLowerCase()];
    if (key) { event.preventDefault(); select(key); }
  });
})();
