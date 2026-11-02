'use strict';
(() => {
  const choices = {
  "woven-world": {
    "title": "Thế giới dệt",
    "src": "assets/woven-world.png",
    "alt": "Ảnh đề xuất Thế giới dệt, thử ngôn ngữ thế giới, nhân vật và UI của hướng này.",
    "description": "Thử dựng cảnh bằng dải đan và nhịp sợi: người, đường nước, đồ dùng và giao diện cùng được tổ chức thành mảng, thay vì phủ vân vải lên cảnh quen.",
    "direction": "DẢI ĐAN / MẶT BẰNG CÔ ĐỌNG"
  },
  "line-volume": {
    "title": "Nét dây không gian",
    "src": "assets/line-volume.png",
    "alt": "Ảnh đề xuất Nét dây không gian, thử ngôn ngữ thế giới, nhân vật và UI của hướng này.",
    "description": "Thử gợi thể tích bằng những đường viền có độ sâu và một số mặt đặc có chủ đích; hình người, vật cầm tay và cửa sổ giao tiếp cùng dùng nhịp nét ấy.",
    "direction": "KHỐI RỖNG / ĐƯỜNG NÉT CÓ CHIỀU SÂU"
  },
  "folded-village": {
    "title": "Làng gấp",
    "src": "assets/folded-village.png",
    "alt": "Ảnh đề xuất Làng gấp, thử ngôn ngữ thế giới, nhân vật và UI của hướng này.",
    "description": "Thử sắp địa bàn thành các mặt gấp có độ sâu, nối sân, bến và hiên bằng nhịp bản lề; tỷ lệ người và giao diện cũng phải có hình khối riêng.",
    "direction": "NẾP GẤP / ĐỘ SÂU XẾP LỚP"
  },
  "rain-sculpt": {
    "title": "Mưa khắc hình",
    "src": "assets/rain-sculpt.png",
    "alt": "Ảnh đề xuất Mưa khắc hình, thử ngôn ngữ thế giới, nhân vật và UI của hướng này.",
    "description": "Thử dùng màn mưa, khoảng trống và khối phẳng sắc để chia lớp không gian; dáng người, cử chỉ và UI cần giữ rõ giữa những dải nhìn có nhịp.",
    "direction": "DẢI MƯA / KHỐI GÓC CẠNH"
  },
  "living-section": {
    "title": "Mặt cắt đời sống",
    "src": "assets/living-section.png",
    "alt": "Ảnh đề xuất Mặt cắt đời sống, thử ngôn ngữ thế giới, nhân vật và UI của hướng này.",
    "description": "Thử một góc nhìn ngang như mặt cắt sân nhà chung, để tầng sinh hoạt, đường lên và các vóc người có tuổi/nghề khác nhau trở thành cấu trúc màn hình.",
    "direction": "NHÌN NGANG / TẦNG SINH HOẠT"
  },
  "voxel-life": {
    "title": "Hạt khối",
    "src": "assets/voxel-life.png",
    "alt": "Ảnh đề xuất Hạt khối, thử ngôn ngữ thế giới, nhân vật và UI của hướng này.",
    "description": "Thử dựng người, cây, nhà, vật dụng và UI bằng những thể tích vuông thô nhất quán, để hình khối thực sự thay đổi thay vì thêm bộ lọc pixel lên ảnh.",
    "direction": "THỂ TÍCH HẠT VUÔNG / TỶ LỆ THÔ"
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
  let selected = 'woven-world';
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
