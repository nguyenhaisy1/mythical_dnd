(function () {
  'use strict';
  const $ = id => document.getElementById(id);
  const directions = {
    a: { label: 'A / TRỜI ÂM', title: 'Để vật liệu tự nói bằng màu.', kicker: 'A / GẦN VỚI ĐỜI SỐNG', name: 'Hướng A — Trời âm', source: 'assets/a-overcast.png', alt: 'Hướng A: bến sông ở tầm mắt, ánh sáng trời âm và màu vật liệu tự nhiên', description: 'Cảnh tả thực, gần ảnh, nhìn từ ngang tầm người dưới ánh trời âm. Gỗ, bùn, vải và nước mang màu riêng; không phủ chung một lớp vàng lên cảnh.', tradeoff: 'Ít kịch tính ánh sáng hơn, cần tổ chức hình khối tốt để cảnh vẫn có điểm nhìn.', specs: ['Ngang tầm người', 'Tả thực, gần ảnh', 'Trời âm, trung tính'] },
    b: { label: 'B / NHÌN CHÉO', title: 'Đọc một nơi chốn từ trên cao.', kicker: 'B / KHÔNG GIAN CỦA GAME', name: 'Hướng B — Nhìn chéo', source: 'assets/b-gouache.png', alt: 'Hướng B: bến sông nhìn chéo từ cao, dựng bằng mảng bột màu gouache', description: 'Góc nhìn chéo mở ra bến, nhà và đường đi. Mảng gouache giúp tổ chức khoảng sáng tối, ưu tiên cấu trúc không gian hơn hiệu ứng điện ảnh.', tradeoff: 'Dễ quan sát tổng thể hơn, nhưng ít gần gũi với từng người; cần kiểm tiếp ở khoảng cách gameplay.', specs: ['Nhìn chéo từ cao', 'Gouache, mảng khoáng', 'Sáng tán xạ'] },
    c: { label: 'C / NÉT KHẮC', title: 'Giữ ít màu, để nét tạo chiều sâu.', kicker: 'C / NGÔN NGỮ ĐỒ HỌA', name: 'Hướng C — Nét khắc', source: 'assets/c-woodcut.png', alt: 'Hướng C: sinh hoạt ở bến sông bằng ngôn ngữ tranh khắc và bảng màu giới hạn', description: 'Nét khắc và các mảng màu giới hạn tạo hình cho gỗ, nước và sinh hoạt. Cảm giác thủ công đến từ cách tổ chức nét và khoảng trống.', tradeoff: 'Có thể tạo bản sắc mạnh, nhưng nhiều nét nhỏ dễ cạnh tranh với nhân vật và điểm tương tác.', specs: ['Bố cục đồ họa', 'Nét khắc, pigment ít màu', 'Tương phản theo mảng'] },
    reference: { label: '02 / MỐC THAM CHIẾU', title: 'Bản 02 — Ánh sáng buổi sớm.', name: 'Bản 02 — Mốc tham chiếu', source: '../experiment-02/assets/ben-vac-painterly.png', alt: 'Bản 02: bến sửa thuyền dưới ánh sáng vàng buổi sớm', description: 'Giữ lại bản đã được nhận xét đẹp hơn nhưng màu còn có cảm giác AI, để đối chiếu với ba hướng mới.', tradeoff: 'Đây là mốc tham chiếu theo phản hồi của bạn, chưa phải hướng đã chốt.', specs: ['Cảnh nhìn chéo', 'Tranh chi tiết', 'Nắng sớm ấm'] }
  };
  let selected = 'a';
  let comparison = false;
  let opener = null;
  const keys = ['a', 'b', 'c'];
  function attachImageState(img) {
    const error = img.closest('figure').querySelector('.image-error');
    if (!error) return;
    img.addEventListener('load', () => { error.hidden = true; });
    img.addEventListener('error', () => { error.hidden = false; });
  }
  function openArt(key, trigger) {
    const item = directions[key];
    if (!item) return;
    opener = trigger || document.activeElement;
    $('dialog-title').textContent = item.name;
    $('dialog-image').src = item.source;
    $('dialog-image').alt = item.alt;
    $('art-dialog').showModal();
    $('dialog-close').focus();
  }
  function bindArtButtons(root) {
    root.querySelectorAll('[data-open-art]').forEach(button => button.addEventListener('click', () => openArt(button.dataset.openArt, button)));
  }
  function selectDirection(key, focus) {
    selected = key;
    const item = directions[key];
    document.querySelectorAll('#direction-tabs [data-direction]').forEach(button => {
      const active = button.dataset.direction === key;
      button.setAttribute('aria-selected', String(active)); button.tabIndex = active ? 0 : -1;
      if (active && focus) button.focus();
    });
    $('direction-panel').setAttribute('aria-labelledby', 'tab-' + key);
    $('single-image').src = item.source; $('single-image').alt = item.alt;
    $('single-art').querySelector('.image-error').hidden = true;
    $('single-art').querySelector('.image-button').dataset.openArt = key;
    $('single-art').querySelector('.image-button').setAttribute('aria-label', 'Mở toàn ảnh hướng ' + key.toUpperCase());
    $('single-open').dataset.openArt = key;
    $('single-caption').textContent = item.label;
    $('single-kicker').textContent = item.kicker;
    $('single-title').textContent = item.title;
    $('single-description').textContent = item.description;
    $('single-tradeoff').replaceChildren();
    const strong = document.createElement('strong'); strong.textContent = 'Đánh đổi: ';
    $('single-tradeoff').append(strong, document.createTextNode(item.tradeoff));
    $('single-specs').querySelectorAll('dd').forEach((dd, index) => { dd.textContent = item.specs[index]; });
  }
  function renderComparison() {
    const grid = $('comparison-grid'); grid.replaceChildren();
    const include = $('include-reference').checked;
    grid.classList.toggle('with-reference', include);
    (include ? [...keys, 'reference'] : keys).forEach(key => {
      const item = directions[key];
      const card = document.createElement('article'); card.className = 'comparison-card';
      const figure = document.createElement('figure');
      const imageButton = document.createElement('button'); imageButton.className = 'image-button'; imageButton.dataset.openArt = key; imageButton.setAttribute('aria-label', 'Mở toàn ảnh ' + item.name);
      const image = document.createElement('img'); image.src = item.source; image.alt = item.alt; image.dataset.art = key;
      imageButton.append(image); figure.append(imageButton);
      const caption = document.createElement('figcaption');
      const label = document.createElement('span'); label.textContent = item.label;
      const open = document.createElement('button'); open.className = 'expand-button'; open.dataset.openArt = key; open.textContent = 'Xem ảnh ↗'; open.setAttribute('aria-label', 'Xem toàn ảnh ' + item.name);
      caption.append(label, open); figure.append(caption);
      const error = document.createElement('p'); error.className = 'image-error'; error.hidden = true; error.setAttribute('role', 'status'); error.textContent = 'Ảnh chưa tải được. Hãy kiểm tra tệp ảnh của bản thử.'; figure.append(error);
      const title = document.createElement('h3'); title.textContent = item.title;
      const spec = document.createElement('p'); spec.className = 'card-spec'; spec.textContent = item.specs.join(' · ');
      const description = document.createElement('p'); description.textContent = item.description;
      const tradeoff = document.createElement('p'); tradeoff.textContent = 'Đánh đổi: ' + item.tradeoff;
      card.append(figure, title, spec, description, tradeoff); grid.append(card); attachImageState(image);
    });
    bindArtButtons(grid);
  }
  function setView(compare) {
    comparison = compare;
    $('view-single').setAttribute('aria-pressed', String(!compare));
    $('view-compare').setAttribute('aria-pressed', String(compare));
    $('direction-panel').hidden = compare; $('comparison-panel').hidden = !compare;
    document.querySelector('.reference-control').hidden = !compare;
    if (compare) renderComparison();
  }
  document.querySelectorAll('#direction-tabs [data-direction]').forEach(button => {
    button.addEventListener('click', () => { selectDirection(button.dataset.direction, false); if (comparison) setView(false); });
    button.addEventListener('keydown', event => {
      let index = keys.indexOf(selected);
      if (event.key === 'ArrowRight') index = (index + 1) % keys.length;
      else if (event.key === 'ArrowLeft') index = (index + keys.length - 1) % keys.length;
      else if (event.key === 'Home') index = 0;
      else if (event.key === 'End') index = keys.length - 1;
      else return;
      event.preventDefault(); selectDirection(keys[index], true); if (comparison) setView(false);
    });
  });
  $('view-single').addEventListener('click', () => setView(false));
  $('view-compare').addEventListener('click', () => setView(true));
  $('include-reference').addEventListener('change', renderComparison);
  $('text-scale').addEventListener('change', event => { document.documentElement.style.fontSize = event.target.value + '%'; });
  $('dialog-close').addEventListener('click', () => $('art-dialog').close());
  $('art-dialog').addEventListener('close', () => { if (opener && opener.isConnected && !opener.closest('[hidden]')) opener.focus(); });
  attachImageState($('single-image'));
  bindArtButtons($('single-art'));
})();
