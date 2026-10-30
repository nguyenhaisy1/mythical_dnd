'use strict';
(() => {
  const choices = {
  "earth-monument": {
    "title": "Đất dựng thành",
    "src": "assets/earth-monument.png",
    "alt": "Ảnh đề xuất Đất dựng thành: khối 3D góc cạnh, địa hình thành đất, nhân vật và UI minh họa.",
    "description": "Thử khối đất, nhịp công trình và người có dáng dài trong một không gian xây dựng; UI đứng thành dải gọn.",
    "direction": "3D GÓC CẠNH / QUY MÔ LỚN"
  },
  "living-grid": {
    "title": "Ô đất sống",
    "src": "assets/living-grid.png",
    "alt": "Ảnh đề xuất Ô đất sống: không gian 2D nhìn từ trên, người và vật hình học cùng giao diện minh họa.",
    "description": "Thử mặt bằng hình học, đường nước và các vùng làm việc với người/vật như những hình nhận được ngay.",
    "direction": "2D NHÌN THẲNG TỪ TRÊN"
  },
  "round-world": {
    "title": "Làng vòng cung",
    "src": "assets/round-world.png",
    "alt": "Ảnh đề xuất Làng vòng cung: thế giới khối mềm, camera cong, cast và UI cùng ngôn ngữ hình.",
    "description": "Thử khối tròn, tỷ lệ người phóng đại và cảnh đồi được diễn đạt bằng một cách nhìn cong.",
    "direction": "KHỐI MỀM / CAMERA CONG"
  },
  "craft-first-person": {
    "title": "Đôi tay làm nghề",
    "src": "assets/craft-first-person.png",
    "alt": "Ảnh đề xuất Đôi tay làm nghề: góc nhìn thứ nhất với bàn tay, công cụ, người thợ và UI đồ họa.",
    "description": "Thử đặt bàn tay, công cụ và người thợ vào trung tâm khung nhìn; mảng in và nhịp chéo dẫn bố cục/UI.",
    "direction": "GÓC NHÌN THỨ NHẤT / ĐỒ HỌA MẠNH"
  },
  "glyph-world": {
    "title": "Ký hình",
    "src": "assets/glyph-world.png",
    "alt": "Ảnh đề xuất Ký hình: địa bàn, nhân vật, vật dụng và UI bằng ngôn ngữ ký hiệu 2D.",
    "description": "Thử người, đồ vật, địa bàn và portrait cùng được cô đọng thành một hệ ký hình dễ học.",
    "direction": "THẾ GIỚI BẰNG KÝ HIỆU"
  },
  "comic-life": {
    "title": "Mái nhà nhiều giọng",
    "src": "assets/comic-life.png",
    "alt": "Ảnh đề xuất Mái nhà nhiều giọng: trang game theo khung truyện với cast gia đình và UI minh họa.",
    "description": "Thử một màn hình chơi được tổ chức bằng khung truyện, gương mặt, cử chỉ và lựa chọn nằm trong nhịp trang.",
    "direction": "TRANG TRUYỆN TƯƠNG TÁC / ĐỜI GIA ĐÌNH"
  }
};
  const order = Object.keys(choices);
  const hero = document.querySelector('#hero-image');
  const title = document.querySelector('#scene-title');
  const direction = document.querySelector('#scene-direction');
  const description = document.querySelector('#scene-description');
  const status = document.querySelector('#selection-status');
  const error = document.querySelector('#image-error');
  const enlarge = document.querySelector('#enlarge');
  const dialog = document.querySelector('#image-dialog');
  const dialogImage = document.querySelector('#dialog-image');
  const dialogTitle = document.querySelector('#dialog-title');
  let selected = 'earth-monument';
  function select(key) {
    const choice = choices[key];
    if (!choice) return;
    selected = key;
    error.hidden = true;
    hero.src = choice.src;
    hero.alt = choice.alt;
    title.textContent = choice.title;
    direction.textContent = choice.direction;
    description.textContent = choice.description;
    enlarge.setAttribute('aria-label', `Mở ${choice.title} toàn khung`);
    document.querySelectorAll('.switches [data-choice]').forEach(button => button.setAttribute('aria-pressed', String(button.dataset.choice === key)));
    status.textContent = `Đang xem ${choice.title}. Cảnh và UI là ảnh minh họa, chưa phải game chạy.`;
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
    document.querySelector('#original-image').href = choice.src;
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
    const index = Number(event.key) - 1;
    if (/^[1-6]$/.test(event.key) && order[index]) { event.preventDefault(); select(order[index]); }
  });
})();
