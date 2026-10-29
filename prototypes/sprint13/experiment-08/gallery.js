'use strict';
(() => {
  const illustrativeHud = ['Nhà bên suối', 'Một cuộc gặp sau cơn mưa.', 'Quan sát sân nhà', 'Lắng nghe', 'Đi qua cầu'];
  const choices = {
    a: { title: 'A — Nét vẽ và mảng bóng', src: 'assets/a-cel.png', alt: 'Phương án A: ảnh ý tưởng sân nhà bên suối, thể hiện bằng nét hoạt hình và mảng bóng gọn.', description: 'Hướng hoạt hình 2D thử đường viền rõ và mảng sáng tối gọn để diễn tả người, sân nhà và lối đi.', hud: illustrativeHud },
    b: { title: 'B — Thế giới dệt bằng sợi', src: 'assets/b-woven.png', alt: 'Phương án B: ảnh ý tưởng cùng tình huống sân nhà bên suối, thể hiện như một thế giới dệt bằng sợi và vải.', description: 'Hướng dệt thử dùng sợi, nhịp đan và mảng vải để tạo nên cảnh vật, thay vì chỉ phủ một texture lên tranh.', hud: illustrativeHud },
    c: { title: 'C — Hình khối cách điệu', src: 'assets/c-faceted.png', alt: 'Phương án C: ảnh ý tưởng sân nhà bên suối có diện mạo 3D cách điệu và các hình khối rõ; không phải model thật.', description: 'Hướng hình khối thử một diện mạo 3D cách điệu qua bề mặt và ánh sáng. Đây vẫn là ảnh raster, không có model hoặc camera tự do.', hud: illustrativeHud }
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
    ['hud-title', 'hud-situation', 'hud-choice-1', 'hud-choice-2', 'hud-choice-3'].forEach((id, index) => { document.getElementById(id).textContent = choice.hud[index]; });
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
