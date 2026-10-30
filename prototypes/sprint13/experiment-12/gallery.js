'use strict';
(() => {
  const choices = {
  "clay-craft": {
    "title": "Đất biết kể",
    "src": "assets/clay-craft.png",
    "alt": "Bảng concept đất nặn với cảnh đời làng, nhân vật trưởng thành, chân dung, đồ vật và UI mang chất liệu thủ công.",
    "description": "Khối đất nặn, dấu tay và ánh sáng mềm thử biến đời làng thành một sân khấu vật chất gần gũi."
  },
  "clear-line": {
    "title": "Nét trong",
    "src": "assets/clear-line.png",
    "alt": "Bảng concept truyện tranh nét trong, cảnh sinh hoạt, mẫu nhân vật toàn thân, chân dung, vật phẩm và giao diện minh họa.",
    "description": "Nét viền sạch và mảng màu phẳng thử làm rõ người, đồ vật, lối đi và tình huống xã hội như một trang truyện có thể bước vào."
  },
  "mineral-fresco": {
    "title": "Vách thời gian",
    "src": "assets/mineral-fresco.png",
    "alt": "Bảng concept bích họa khoáng màu với thế giới, người lao động, chân dung, vật phẩm và các khung UI đồng bộ.",
    "description": "Mảng khoáng màu, hình thể giản lược và nhịp bố cục bích họa thử kể một cuộc đời như dấu tích còn lại trên vách."
  },
  "retro-prerender": {
    "title": "Ký ức CD-ROM",
    "src": "assets/retro-prerender.png",
    "alt": "Bảng concept RPG diện mạo dựng sẵn cổ điển, cảnh nhìn từ cao, cast trưởng thành, chân dung, vật phẩm và UI.",
    "description": "Phối cảnh cố định, vật liệu dựng sẵn và khung giao diện có trọng lượng thử một diện mạo RPG máy tính cuối thập niên 1990."
  },
  "charcoal-night": {
    "title": "Đêm than bạc",
    "src": "assets/charcoal-night.png",
    "alt": "Bảng concept đêm vẽ than và ánh bạc, cảnh đời thường, nhân vật, chân dung, vật phẩm cùng UI sáng tối.",
    "description": "Nét than và những vùng sáng bạc thử dẫn mắt bằng độ sáng, khoảng trống và cử chỉ thay cho một bảng màu rực."
  },
  "graphic-animation": {
    "title": "Gió mảng màu",
    "src": "assets/graphic-animation.png",
    "alt": "Bảng concept hoạt hình mảng màu không viền với cảnh chơi, tạo hình người trưởng thành, chân dung, icon và UI.",
    "description": "Hình khối cắt gọn, nhịp âm dương và các mảng màu chủ động thử một thế giới hoạt hình có thể nhận ra từ silhouette."
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
  let selected = 'clay-craft';
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
    const key = {"1": "clay-craft", "2": "clear-line", "3": "mineral-fresco", "4": "retro-prerender", "5": "charcoal-night", "6": "graphic-animation"}[event.key.toLowerCase()];
    if (key) { event.preventDefault(); select(key); }
  });
})();
