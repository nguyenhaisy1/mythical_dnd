'use strict';
(() => {
  const choices = {
  "earth-corridor": {
    "title": "Hành lang thành đất",
    "src": "assets/earth-corridor.png",
    "alt": "Ảnh đề xuất Hành lang thành đất: cảnh, nhân vật và UI minh họa của phương án này.",
    "description": "Thử giữ quy mô thành đất bằng ít khối lớn, kéo người và việc đang làm về gần để cuộc gặp không bị kiến trúc lấn át.",
    "direction": "KHỐI THÀNH / TẦM NGƯỜI"
  },
  "river-negative": {
    "title": "Bờ liền nét",
    "src": "assets/river-negative.png",
    "alt": "Ảnh đề xuất Bờ liền nét: cảnh, nhân vật và UI minh họa của phương án này.",
    "description": "Thử mặt bằng phẳng bằng bờ liền và khoảng âm của sông, để người, thuyền và vật mang cùng được nhìn từ trên.",
    "direction": "MẶT BẰNG 90° / ĐẤT VÀ NƯỚC"
  },
  "gentle-arc": {
    "title": "Vành sân",
    "src": "assets/gentle-arc.png",
    "alt": "Ảnh đề xuất Vành sân: cảnh, nhân vật và UI minh họa của phương án này.",
    "description": "Thử trục mái, cột và đường theo cung nhìn thấy được, trong khi hai người gần giữ cử chỉ và điểm đặt chân rõ.",
    "direction": "CAMERA CUNG / CUỘC GẶP GẦN"
  },
  "living-relief": {
    "title": "Phù điêu đang sống",
    "src": "assets/living-relief.png",
    "alt": "Ảnh đề xuất Phù điêu đang sống: cảnh, nhân vật và UI minh họa của phương án này.",
    "description": "Thử người, vật và công trình như các khối nổi nông nhìn gần chính diện, với cạnh và khe tạo độ sâu; UI cùng quy tắc lồi–lõm.",
    "direction": "KHỐI NÔNG / CẠNH LỒI VÀ KHE ÂM"
  },
  "gesture-flat": {
    "title": "Người bằng cử chỉ",
    "src": "assets/gesture-flat.png",
    "alt": "Ảnh đề xuất Người bằng cử chỉ: cảnh, nhân vật và UI minh họa của phương án này.",
    "description": "Thử vai, tay và lưng phóng đại theo việc mang, vá hoặc dệt; thế giới và UI nối cùng nhịp hình thay một bộ avatar cùng khuôn.",
    "direction": "2D MẢNG / SILHOUETTE BẤT ĐỐI XỨNG"
  },
  "nested-frames": {
    "title": "Cửa trong cửa",
    "src": "assets/nested-frames.png",
    "alt": "Ảnh đề xuất Cửa trong cửa: cảnh, nhân vật và UI minh họa của phương án này.",
    "description": "Thử ba khung mái và cột giảm kích thước, che nhau rõ để đưa mắt tới một cuộc gặp ở gần; UI neo vào vùng âm bên rìa.",
    "direction": "TẦM NGƯỜI / BA KHUNG CHE KHUẤT"
  },
  "tangram-people": {
    "title": "Khối ghép đời thường",
    "src": "assets/tangram-people.png",
    "alt": "Ảnh đề xuất Khối ghép đời thường: cảnh, nhân vật và UI minh họa của phương án này.",
    "description": "Thử cả thân người, nhà, thuyền và icon bằng rất ít nêm hoặc lăng trụ lớn, giữ bàn tay và dáng làm việc giữa sự cô đọng.",
    "direction": "ÍT NÊM LỚN / THỂ TÍCH GHÉP"
  },
  "low-water": {
    "title": "Sát mặt nước",
    "src": "assets/low-water.png",
    "alt": "Ảnh đề xuất Sát mặt nước: cảnh, nhân vật và UI minh họa của phương án này.",
    "description": "Thử nhìn sát mặt nước qua mép thuyền vào việc trao, kéo hoặc chuyển vật; cơ thể và vật hữu dụng dẫn bố cục thay toàn cảnh làng.",
    "direction": "GÓC THẤP / THUYỀN VÀ THÂN NGƯỜI"
  },
  "close-pixel": {
    "title": "Pixel cự ly gần",
    "src": "assets/close-pixel.png",
    "alt": "Ảnh đề xuất Pixel cự ly gần: cảnh, nhân vật và UI minh họa của phương án này.",
    "description": "Thử đưa pixel tới gần nhân vật và cử chỉ, giảm mật độ cảnh để fullbody, portrait và biểu tượng dùng cùng nhịp raster có chủ ý.",
    "direction": "PIXEL 2D / NGƯỜI Ở CỰ LY GẦN"
  },
  "negative-space": {
    "title": "Âm bản sống",
    "src": "assets/negative-space.png",
    "alt": "Ảnh đề xuất Âm bản sống: cảnh, nhân vật và UI minh họa của phương án này.",
    "description": "Thử dùng khoảng trống và đường biên chung để dựng người, đồ và nơi chốn; mặt đặc của UI nối với nhịp hình–nền của cảnh.",
    "direction": "KHOẢNG TRỐNG / HÌNH VÀ NỀN"
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
  let selected = 'earth-corridor';
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
    const index = event.key === '0' ? 9 : Number(event.key) - 1;
    if (/^[0-9]$/.test(event.key) && order[index]) { event.preventDefault(); select(order[index]); }
  });
})();
