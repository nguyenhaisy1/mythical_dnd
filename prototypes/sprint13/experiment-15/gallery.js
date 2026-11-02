'use strict';
(() => {
  const choices = {
  "vertical-earth": {
    "title": "Thành dựng đứng",
    "src": "assets/vertical-earth.png",
    "alt": "Ảnh đề xuất Thành dựng đứng, thử ngôn ngữ thế giới, nhân vật và UI của hướng này.",
    "description": "Thử đưa địa hình thành đất và các khoảng sinh hoạt lên một bố cục dọc, dùng tầng, lối nối và khối lớn để đọc cộng đồng.",
    "direction": "TẦNG KHÔNG GIAN / KHỐI THÀNH ĐẤT"
  },
  "tidal-mosaic": {
    "title": "Ghép đảo thủy triều",
    "src": "assets/tidal-mosaic.png",
    "alt": "Ảnh đề xuất Ghép đảo thủy triều, thử ngôn ngữ thế giới, nhân vật và UI của hướng này.",
    "description": "Thử chia địa bàn nước–đất thành các mảnh hình có nhịp, giữ con người và hoạt động như điểm nhập vai giữa bố cục nhìn từ trên.",
    "direction": "MẶT BẰNG GHÉP MẢNG / ĐƯỜNG NƯỚC"
  },
  "rolling-world": {
    "title": "Làng trên vành trống",
    "src": "assets/rolling-world.png",
    "alt": "Ảnh đề xuất Làng trên vành trống, thử ngôn ngữ thế giới, nhân vật và UI của hướng này.",
    "description": "Thử đẩy độ cong của cách nhìn rõ hơn, để hình người, mái nhà và đường đi cùng tham gia một nhịp không gian cuộn.",
    "direction": "Ý ĐỊNH CAMERA CONG / KHỐI MỀM"
  },
  "cubist-space": {
    "title": "Địa hình đa diện",
    "src": "assets/cubist-space.png",
    "alt": "Ảnh đề xuất Địa hình đa diện, thử ngôn ngữ thế giới, nhân vật và UI của hướng này.",
    "description": "Thử một nơi được đọc qua nhiều mặt và hướng nhìn trong cùng khung, thay phối cảnh đơn bằng nhịp mặt phẳng, người và đồ vật.",
    "direction": "NHIỀU ĐIỂM NHÌN / HÌNH CÔ ĐỌNG"
  },
  "shadow-world": {
    "title": "Làng của bóng",
    "src": "assets/shadow-world.png",
    "alt": "Ảnh đề xuất Làng của bóng, thử ngôn ngữ thế giới, nhân vật và UI của hướng này.",
    "description": "Thử để hình khối gần trắng và các mảng bóng giúp đọc vật, người, khoảng cách và vị trí tương tác.",
    "direction": "KHỐI SÁNG / KHÔNG GIAN QUA BÓNG"
  },
  "mirror-water": {
    "title": "Thủy kính",
    "src": "assets/mirror-water.png",
    "alt": "Ảnh đề xuất Thủy kính, thử ngôn ngữ thế giới, nhân vật và UI của hướng này.",
    "description": "Thử tổ chức một tình huống địa phương qua mặt nước và hình phản chiếu, nối nhân vật, vật dụng và UI bằng cùng ngôn ngữ.",
    "direction": "PHẢN CHIẾU / KHUNG NHÌN MẶT NƯỚC"
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
  let selected = 'vertical-earth';
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
