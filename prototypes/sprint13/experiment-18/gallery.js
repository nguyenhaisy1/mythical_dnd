'use strict';
(() => {
  const choices = {
  "open-yard": {
    "title": "Sân chéo mở",
    "src": "assets/open-yard.png",
    "alt": "Ảnh đề xuất Sân chéo mở: cảnh, nhân vật và UI minh họa của phương án này.",
    "description": "Thử sân sinh hoạt nhìn chéo với ít mái che, một khoảng trống lớn cho người, tay và công việc cùng đọc được.",
    "direction": "SÂN NGHỀ / GÓC CHÉO MỞ"
  },
  "three-bands": {
    "title": "Ba dải đường",
    "src": "assets/three-bands.png",
    "alt": "Ảnh đề xuất Ba dải đường: cảnh, nhân vật và UI minh họa của phương án này.",
    "description": "Thử ba dải hoạt động ngoài trời nhìn ngang, để sự che khuất giữa đường gần, sân giữa và lớp xa tổ chức cuộc gặp.",
    "direction": "BA LỚP NGANG / CHE KHUẤT CÔNG CỘNG"
  },
  "shoulder-craft": {
    "title": "Qua vai người thợ",
    "src": "assets/shoulder-craft.png",
    "alt": "Ảnh đề xuất Qua vai người thợ: cảnh, nhân vật và UI minh họa của phương án này.",
    "description": "Thử góc nhìn gần qua vai, để bàn tay, công cụ và người đối diện làm điểm nhập vai thay một toàn cảnh làng.",
    "direction": "GẦN VAI / TAY VÀ VẬT HỮU DỤNG"
  },
  "long-diagonal": {
    "title": "Đường chéo dài",
    "src": "assets/long-diagonal.png",
    "alt": "Ảnh đề xuất Đường chéo dài: cảnh, nhân vật và UI minh họa của phương án này.",
    "description": "Thử một lối hoặc cầu nhìn axon chạy chéo qua khung, với những thân người đủ lớn và khoảng nghỉ giữa các khối.",
    "direction": "LỐI CHÉO / NHỊP KHỐI VÀ NGƯỜI"
  },
  "column-forest": {
    "title": "Rừng là thân cột",
    "src": "assets/column-forest.png",
    "alt": "Ảnh đề xuất Rừng là thân cột: cảnh, nhân vật và UI minh họa của phương án này.",
    "description": "Thử đưa sinh hoạt vào giữa rễ và thân cây lớn, dùng khối gần để dựng chiều sâu quanh việc hái, buộc hoặc trao một bó vật.",
    "direction": "THÂN CÂY / KHUNG SINH HOẠT"
  },
  "plan-profile": {
    "title": "Sân phẳng, người nổi",
    "src": "assets/plan-profile.png",
    "alt": "Ảnh đề xuất Sân phẳng, người nổi: cảnh, nhân vật và UI minh họa của phương án này.",
    "description": "Thử mặt bằng phẳng đi cùng thân người và đồ vật nhìn mặt đứng theo một quy ước lai có chủ ý.",
    "direction": "MẶT BẰNG / PROFILE CÓ QUY ƯỚC"
  },
  "cel-adult": {
    "title": "Nét mặt ba sắc",
    "src": "assets/cel-adult.png",
    "alt": "Ảnh đề xuất Nét mặt ba sắc: cảnh, nhân vật và UI minh họa của phương án này.",
    "description": "Thử hình người gợi3D với ít mảng sáng tối nhưng giữ mắt, mày, mũi và dấu tuổi đủ rõ ở cự ly chơi.",
    "direction": "ÍT TÔNG / MẶT NGƯỜI TRƯỞNG THÀNH"
  },
  "soft-mass": {
    "title": "Khối liền mềm",
    "src": "assets/soft-mass.png",
    "alt": "Ảnh đề xuất Khối liền mềm: cảnh, nhân vật và UI minh họa của phương án này.",
    "description": "Thử thân thể và vật dụng bằng vài khối bo liền, giữ đầu theo tỷ lệ người lớn, bàn tay và tư thế lao động dễ đọc.",
    "direction": "KHỐI BO LIỀN / DÁNG NGƯỜI LỚN"
  },
  "ink-mass": {
    "title": "Mực đặc",
    "src": "assets/ink-mass.png",
    "alt": "Ảnh đề xuất Mực đặc: cảnh, nhân vật và UI minh họa của phương án này.",
    "description": "Thử dùng mảng mực đặc và khoảng trống ở tay, lưng, vật để đọc cử chỉ; nét nội bộ dành cho mặt và việc chính.",
    "direction": "SILHOUETTE ĐẶC / KHOẢNG RỖNG"
  },
  "ribbon-body": {
    "title": "Người bằng dải",
    "src": "assets/ribbon-body.png",
    "alt": "Ảnh đề xuất Người bằng dải: cảnh, nhân vật và UI minh họa của phương án này.",
    "description": "Thử để dải rộng cấu tạo cả thân, tay và chân, trong khi mặt đặc có nét tuổi và UI nối cùng nhịp dải.",
    "direction": "DẢI RỘNG / CƠ THỂ VÀ CHE KHUẤT"
  },
  "contour-step": {
    "title": "Đường đồng mức sống",
    "src": "assets/contour-step.png",
    "alt": "Ảnh đề xuất Đường đồng mức sống: cảnh, nhân vật và UI minh họa của phương án này.",
    "description": "Thử vài lớp cao độ và cạnh bậc cùng cỡ xuyên cơ thể, công trình, vật cầm và vùng UI.",
    "direction": "LỚP CAO ĐỘ / KHỐI UỐN LIỀN"
  },
  "square-gesture": {
    "title": "Pixel hình lớn",
    "src": "assets/square-gesture.png",
    "alt": "Ảnh đề xuất Pixel hình lớn: cảnh, nhân vật và UI minh họa của phương án này.",
    "description": "Thử pixel2D bằng cụm vuông lớn, người và vật có silhouette rõ, giảm chi tiết nhỏ nhưng vẫn giữ dấu mặt và tuổi.",
    "direction": "CỤM VUÔNG MÀU / CỬ CHỈ"
  },
  "one-bit": {
    "title": "Một-bit có tuổi",
    "src": "assets/one-bit.png",
    "alt": "Ảnh đề xuất Một-bit có tuổi: cảnh, nhân vật và UI minh họa của phương án này.",
    "description": "Thử một ngôn ngữ một-bit với silhouette, khoảng rỗng và một vài dấu mặt có chủ đích để giữ con người nhiều tuổi.",
    "direction": "ĐẶC VÀ RỖNG / ÍT DẤU MẶT"
  },
  "single-stage": {
    "title": "Sân khấu một nền",
    "src": "assets/single-stage.png",
    "alt": "Ảnh đề xuất Sân khấu một nền: cảnh, nhân vật và UI minh họa của phương án này.",
    "description": "Thử một nền sinh hoạt nhìn ngang ở cự ly gần, để nhiều dáng người và hành động cùng chung mặt tựa dễ hiểu.",
    "direction": "MỘT MẶT SÀN / NGƯỜI NHÌN NGANG"
  },
  "retro-facet": {
    "title": "Khối răng cưa",
    "src": "assets/retro-facet.png",
    "alt": "Ảnh đề xuất Khối răng cưa: cảnh, nhân vật và UI minh họa của phương án này.",
    "description": "Thử khối gợi3D thô với bước cạnh và mặt phẳng có chủ ý, dùng giới hạn render để thống nhất người, đồ và UI.",
    "direction": "BẬC CẠNH / THỂ TÍCH THÔ"
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
  let selected = 'open-yard';
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
    let index = -1;
    if (/^[0-9]$/.test(event.key)) index = event.key === '0' ? 9 : Number(event.key) - 1;
    if (event.key === 'ArrowRight') index = (order.indexOf(selected) + 1) % order.length;
    if (event.key === 'ArrowLeft') index = (order.indexOf(selected) - 1 + order.length) % order.length;
    if (event.key === 'Home') index = 0;
    if (event.key === 'End') index = order.length - 1;
    if (order[index]) { event.preventDefault(); select(order[index]); }
  });
})();
