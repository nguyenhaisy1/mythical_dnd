'use strict';
(() => {
  const choices = {
  "block-world": {
    "title": "Khối Lạc · Thế giới",
    "src": "assets/block-world.png",
    "alt": "Ảnh đề xuất khung chơi Khối Lạc nhìn chéo, thử hình khối và tỷ lệ cách điệu.",
    "description": "Thử camera nhìn chéo với khối lớn, bóng gọn và tỷ lệ người được thiết kế cho game. Quan sát lối đi, sàn cao, vật che nhau và người đang làm việc.",
    "direction": "3D CÁCH ĐIỆU / NHÌN CHÉO",
    "frame": "Thế giới"
  },
  "block-dialogue": {
    "title": "Khối Lạc · Đối thoại",
    "src": "assets/block-dialogue.png",
    "alt": "Ảnh đề xuất trạng thái đối thoại của hướng Khối Lạc, kiểm người nói, cận mặt và giao diện minh họa.",
    "description": "Thử trạng thái nói chuyện riêng: người nói, câu thoại, lựa chọn và cận mặt có giữ cùng quy tắc khối với khung thế giới không?",
    "direction": "3D CÁCH ĐIỆU / NHÌN CHÉO",
    "frame": "Đối thoại"
  },
  "frieze-world": {
    "title": "Dải đời sống · Thế giới",
    "src": "assets/frieze-world.png",
    "alt": "Ảnh đề xuất khung chơi Dải đời sống, thử không gian 2.5D theo các dải sinh hoạt.",
    "description": "Thử tổ chức sàn nhà, sân, bến và nước thành các dải/cao độ. Dáng người, động tác và khoảng trống giữ vai trò chính thay cho panorama nhiều chi tiết.",
    "direction": "2.5D / DẢI SINH HOẠT",
    "frame": "Thế giới"
  },
  "frieze-dialogue": {
    "title": "Dải đời sống · Đối thoại",
    "src": "assets/frieze-dialogue.png",
    "alt": "Ảnh đề xuất đối thoại Dải đời sống với người và giao diện theo ngôn ngữ hình cô đọng.",
    "description": "Thử cách một cuộc nói chuyện xuất hiện trong ngôn ngữ hình theo dải: speaker và câu nói rõ, UI giữ nhịp hình mà không lấn động tác.",
    "direction": "2.5D / DẢI SINH HOẠT",
    "frame": "Đối thoại"
  },
  "near-world": {
    "title": "Trong lòng làng · Thế giới",
    "src": "assets/near-world.png",
    "alt": "Ảnh đề xuất khung chơi Trong lòng làng với camera gần tầm người và tạo hình 3D cách điệu.",
    "description": "Thử hiện diện gần người bằng camera thấp hơn, mặt khối và tỷ lệ phóng đại. Khung này đối chiếu cự ly camera với hướng nhìn chéo, không chứng minh camera chạy thật.",
    "direction": "3D CÁCH ĐIỆU / GẦN TẦM NGƯỜI",
    "frame": "Thế giới"
  },
  "near-dialogue": {
    "title": "Trong lòng làng · Đối thoại",
    "src": "assets/near-dialogue.png",
    "alt": "Ảnh đề xuất đối thoại Trong lòng làng, thử biểu cảm và giao diện ở cự ly gần.",
    "description": "Thử cử chỉ, nét mặt và UI ở cự ly một cuộc gặp. Quan sát hình người/công cụ có còn đồng bộ với thế giới giản lược, thay vì quay về da và vải tả thực.",
    "direction": "3D CÁCH ĐIỆU / GẦN TẦM NGƯỜI",
    "frame": "Đối thoại"
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
  let selected = 'block-world';
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
