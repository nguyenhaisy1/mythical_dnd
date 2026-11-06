using System;
using System.Collections;
using Game.Application.Projection;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.Client.Sprint15
{
    /// <summary>Presenter/input only. All authoritative actions pass through the actor-scoped port.</summary>
    public sealed class RepairClientView : MonoBehaviour
    {
        private IRepairClientSession session;
        private RepairSceneView scene;
        private VisualElement root, panel, hud, modal, dialog;
        private ScrollView observations;
        private Label time, state, observation, outcome, previewText;
        private Button repair, cancel, confirm, back, advanceOne;
        private VisualElement focusBeforeModal;
        private RepairClientPreview preview;
        private string lastRequest;
        private float textScale = 1;
        private bool autoplayInputIsolated, probingBackgroundSubmit, scenePointerPressed;
        private int sceneGestureVersion;
        public bool AutoplayInputIsolated => autoplayInputIsolated;
        public int InterceptedInputEvents { get; private set; }
        public bool ModalOpen => modal != null && modal.style.display.value != DisplayStyle.None;
        public RepairClientResult LastResult { get; private set; }
        public RepairClientPreview CurrentPreview => preview;
        public int ScenePickCount { get; private set; }
        public int KeyboardEventCount { get; private set; }
        public float TextScale => textScale;
        public bool ModalBackgroundDisabled => ModalOpen && !panel.enabledInHierarchy;
        public bool ModalFocusContained => ModalOpen && root.panel != null &&
            root.panel.focusController.focusedElement is VisualElement focused && dialog.Contains(focused);
        public Rect PreviewDialogBounds => dialog.worldBound;
        public Rect PreviewCostBounds => previewText.worldBound;
        public Rect PreviewConfirmBounds => confirm.worldBound;
        public bool PreviewLayoutFits => ModalOpen && Fits(root.worldBound, dialog.worldBound, 8) &&
            Fits(dialog.worldBound, previewText.worldBound, 0) && Fits(dialog.worldBound, confirm.worldBound, 0);
        public bool LowerControlsVisible => observations != null && observations.scrollOffset.y > 0 &&
            observations.verticalScroller.highValue > 0 && Fits(root.worldBound, observations.contentViewport.worldBound, 0) &&
            Fits(observations.contentViewport.worldBound, outcome.worldBound, 0);
        public event Action<string> InputObserved;
        private static bool Fits(Rect parent, Rect child, float margin)
            => child.width > 0 && child.height > 0 && child.xMin >= parent.xMin + margin &&
                child.yMin >= parent.yMin + margin && child.xMax <= parent.xMax - margin && child.yMax <= parent.yMax - margin;

        public void Initialize(IRepairClientSession port, RepairSceneView sceneView)
        {
            session = port ?? throw new ArgumentNullException(nameof(port)); scene = sceneView;
            Build(); RegisterInputRouting(); Refresh();
        }
        // Callback automation owns its action stream; physical input must not join that stream.
        // This is enabled only by -s15Autoplay, never by normal play or the OS-input probe.
        public void IsolateAutoplayInput() { autoplayInputIsolated = true; }
        private void BlockEvent(EventBase evt)
        {
            InterceptedInputEvents++;
            root.panel?.focusController.IgnoreEvent(evt);
            evt.StopImmediatePropagation();
        }
        private void RegisterInputRouting()
        {
            root.RegisterCallback<KeyDownEvent>(evt => {
                if (autoplayInputIsolated || evt.keyCode == KeyCode.Space) BlockEvent(evt);
            }, TrickleDown.TrickleDown);
            root.RegisterCallback<KeyUpEvent>(evt => {
                if (autoplayInputIsolated || evt.keyCode == KeyCode.Space) BlockEvent(evt);
            }, TrickleDown.TrickleDown);
            root.RegisterCallback<NavigationSubmitEvent>(evt => {
                if ((autoplayInputIsolated && !probingBackgroundSubmit) ||
                    Input.GetKey(KeyCode.Space) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyUp(KeyCode.Space)) BlockEvent(evt);
            }, TrickleDown.TrickleDown);
            root.RegisterCallback<NavigationCancelEvent>(evt => {
                if (autoplayInputIsolated) BlockEvent(evt);
            }, TrickleDown.TrickleDown);
            root.RegisterCallback<NavigationMoveEvent>(evt => {
                if (autoplayInputIsolated) BlockEvent(evt);
            }, TrickleDown.TrickleDown);
            root.RegisterCallback<PointerDownEvent>(evt => {
                if (autoplayInputIsolated) BlockEvent(evt);
            }, TrickleDown.TrickleDown);
            root.RegisterCallback<PointerUpEvent>(evt => {
                if (autoplayInputIsolated) BlockEvent(evt);
            }, TrickleDown.TrickleDown);
            root.RegisterCallback<ClickEvent>(evt => {
                if (autoplayInputIsolated) BlockEvent(evt);
            }, TrickleDown.TrickleDown);
            root.RegisterCallback<WheelEvent>(evt => {
                if (autoplayInputIsolated) BlockEvent(evt);
            }, TrickleDown.TrickleDown);
        }
        public void Bind(IRepairClientSession port)
        { session = port ?? throw new ArgumentNullException(nameof(port)); ClosePreview(); LastResult = null; lastRequest = null; Refresh(); }
        private static Color Ink => new Color(.91f, .88f, .79f);
        private static Color Paper => new Color(.12f, .17f, .15f, .97f);
        private static void Padding(VisualElement e, float n)
        { e.style.paddingLeft = n; e.style.paddingRight = n; e.style.paddingTop = n; e.style.paddingBottom = n; }
        private static void Borders(VisualElement e, Color color)
        {
            e.style.borderLeftWidth = e.style.borderRightWidth = e.style.borderTopWidth = e.style.borderBottomWidth = 1;
            e.style.borderLeftColor = e.style.borderRightColor = e.style.borderTopColor = e.style.borderBottomColor = color;
        }
        private Label Label(string value, int size = 18)
        {
            var l = new Label(value); l.style.whiteSpace = WhiteSpace.Normal; l.style.color = Ink;
            l.style.fontSize = size; l.style.marginBottom = 12; l.style.flexShrink = 0; return l;
        }
        private Button Button(string value, Action action)
        {
            var b = new Button(action) { text = value }; b.focusable = true;
            b.style.height = 42; b.style.minHeight = 42; b.style.marginBottom = 8; b.style.fontSize = 17;
            b.style.backgroundColor = new Color(.25f, .32f, .25f); b.style.color = Ink;
            b.style.whiteSpace = WhiteSpace.Normal; b.style.flexShrink = 0;
            b.RegisterCallback<FocusInEvent>(_ => Borders(b, new Color(.95f, .74f, .32f)));
            b.RegisterCallback<FocusOutEvent>(_ => Borders(b, new Color(.4f, .47f, .37f)));
            Borders(b, new Color(.4f, .47f, .37f)); return b;
        }
        private void Build()
        {
            var settings = ScriptableObject.CreateInstance<PanelSettings>(); settings.scaleMode = PanelScaleMode.ConstantPixelSize;
            settings.themeStyleSheet = RuntimeTheme();
            settings.sortingOrder = 100;
            var document = gameObject.AddComponent<UIDocument>(); document.panelSettings = settings;
            root = document.rootVisualElement; root.name = "RepairClientRoot"; root.pickingMode = PickingMode.Ignore;
            root.style.position = Position.Absolute; root.style.left = root.style.right = root.style.top = root.style.bottom = 0;
            var font = Resources.Load<Font>("Sprint15/Fonts/NotoSans-Regular");
            if (font == null) throw new InvalidOperationException("Vietnamese font missing");
            root.style.unityFont = font; root.style.color = Ink;
            hud = new VisualElement(); hud.name = "Header"; hud.style.position = Position.Absolute;
            hud.style.left = 20; hud.style.right = 20; hud.style.top = 16; hud.style.backgroundColor = Paper; Padding(hud, 16);
            var title = Label("HỒNG BÀNG  /  BẾN VẠC", 25); title.style.marginBottom = 3; hud.Add(title);
            var caption = Label("Bản thử kỹ thuật — nhân vật cố định", 15); caption.style.marginBottom = 0; hud.Add(caption); root.Add(hud);
            observations = new ScrollView(ScrollViewMode.Vertical); panel = observations;
            observations.verticalScrollerVisibility = ScrollerVisibility.AlwaysVisible;
            observations.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
            observations.contentViewport.style.overflow = Overflow.Hidden;
            observations.contentViewport.style.minHeight = 0; observations.contentViewport.style.flexShrink = 1;
            observations.contentContainer.style.flexShrink = 0; observations.style.overflow = Overflow.Hidden;
            observations.RegisterCallback<FocusInEvent>(evt => {
                if (evt.target is VisualElement focused && observations.contentContainer.Contains(focused))
                    observations.schedule.Execute(() => observations.ScrollTo(focused));
            });
            panel.name = "ActorObservations"; panel.style.position = Position.Absolute;
            panel.style.right = 20; panel.style.top = 116; panel.style.bottom = 18; panel.style.width = 340;
            panel.style.backgroundColor = Paper; Padding(panel, 18); Borders(panel, new Color(.45f, .49f, .37f));
            panel.Add(Label("Bãi sửa thuyền", 23)); time = Label(""); panel.Add(time);
            state = Label(""); panel.Add(state); observation = Label("", 14); panel.Add(observation);
            repair = Button("Xem việc sửa thuyền  [R]", OpenPreview); panel.Add(repair);
            cancel = Button("Hủy công việc đang làm  [C]", () => CancelRepair()); panel.Add(cancel);
            advanceOne = Button("Chờ 1 phút  [Space]", () => Advance(1)); panel.Add(advanceOne);
            panel.Add(Button("Chờ 59 phút", () => Advance(59)));
            panel.Add(Button("Chờ 60 phút", () => Advance(60)));
            panel.Add(Button("Lưu mốc hiện tại  [S]", () => Save()));
            panel.Add(Button("Mở lại dữ liệu đã lưu  [L]", () => Reload()));
            panel.Add(Button("Gửi lại lệnh gần nhất", () => RetryLast()));
            var scales = new VisualElement(); scales.style.flexDirection = FlexDirection.Row;
            foreach (var value in new[] { 1f, 1.25f, 1.5f })
            { var v = value; var b = Button((v * 100).ToString("0") + "%", () => SetTextScale(v)); b.style.flexGrow = 1; scales.Add(b); }
            panel.Add(Label("Cỡ chữ", 14)); panel.Add(scales);
            outcome = Label("Mỗi lựa chọn được xử lý bằng luật game.", 15); panel.Add(outcome); root.Add(panel);
            var help = Label("Nhấp thuyền để xem việc sửa · Nhấp sân để di chuyển hình nhân vật · Tab: focus · Esc: đóng bảng", 14);
            help.style.position = Position.Absolute; help.style.left = 20; help.style.right = 390; help.style.bottom = 16;
            help.style.backgroundColor = Paper; Padding(help, 10); root.Add(help);
            modal = new VisualElement(); modal.name = "RepairPreviewModal"; modal.style.position = Position.Absolute;
            modal.style.left = modal.style.right = modal.style.top = modal.style.bottom = 0;
            modal.style.backgroundColor = new Color(.02f, .03f, .025f, .82f); modal.style.alignItems = Align.Center;
            modal.style.justifyContent = Justify.Center; modal.style.display = DisplayStyle.None;
            modal.RegisterCallback<PointerDownEvent>(e => e.StopPropagation());
            dialog = new VisualElement(); dialog.style.width = 510; dialog.style.maxWidth = Length.Percent(90);
            dialog.style.backgroundColor = Paper; Padding(dialog, 25); Borders(dialog, new Color(.77f, .66f, .4f));
            dialog.Add(Label("Sửa lại con thuyền", 26)); previewText = Label(""); dialog.Add(previewText);
            confirm = Button("Xác nhận bắt đầu  [Enter]", () => ConfirmPreview()); dialog.Add(confirm);
            back = Button("Quay lại  [Esc]", ClosePreview); dialog.Add(back); modal.Add(dialog); root.Add(modal);
        }
        public void ShowFatal(string message)
        {
            var settings = ScriptableObject.CreateInstance<PanelSettings>(); settings.themeStyleSheet = RuntimeTheme();
            var doc = gameObject.AddComponent<UIDocument>(); doc.panelSettings = settings;
            var box = doc.rootVisualElement; box.style.backgroundColor = Paper; Padding(box, 30); box.Add(Label(message, 22));
        }
        private static ThemeStyleSheet RuntimeTheme()
            => Resources.Load<ThemeStyleSheet>("Sprint15/DefaultRuntimeTheme")
                ?? throw new InvalidOperationException("Default runtime UI theme missing");
        public void ScrollToLowerControls() { if (!ModalOpen) observations.ScrollTo(outcome); }
        public void ScrollToTop() { if (!ModalOpen) observations.scrollOffset = Vector2.zero; }
        public void Refresh()
        {
            if (session == null) return;
            var v = session.Read(); scene.Render(v);
            time.text = v.ActorLabel + "\nPhút trong game: " + v.Tick;
            string Number(long? x) => x.HasValue ? x.Value.ToString() : "chưa rõ";
            string status = v.ActivityStatus == "running" ? "Đang sửa" : v.ActivityStatus == "completed" ? "Đã hoàn tất"
                : v.ActivityStatus == "cancelled" ? "Đã hủy" : v.ActivityStatus == "none" ? "Chưa bắt đầu" : "Chưa rõ";
            state.text = "Tình trạng thuyền: " + Number(v.BoatCondition) + "\nGỗ đang có: " + Number(v.TimberOnHand)
                + "\nĐang giữ chỗ: " + Number(v.TimberReserved) + "\nCó thể dùng: " + Number(v.TimberAvailable)
                + "\n\n" + status + (v.ActivityDueTick.HasValue ? " · hẹn phút " + v.ActivityDueTick.Value : "");
            observation.text = (v.Observation.Source == "direct-worksite-fixture" ? "Nguồn: quan sát trực tiếp tại bãi" : "Nguồn: chưa rõ")
                + "\nCập nhật ở phút " + v.Observation.ObservedTick;
            repair.SetEnabled(v.BoatVisible); cancel.SetEnabled(v.CanCancel);
        }
        public void OpenPreview()
        {
            if (ModalOpen) return;
            // Opening any modal consumes a pending world gesture, including keyboard opens.
            scenePointerPressed = false; sceneGestureVersion++;
            preview = session.PreviewRepair();
            string Value(long? x) => x.HasValue ? x.Value.ToString() : "chưa rõ";
            previewText.text = "Vật liệu: " + Value(preview.CostTimber) + " đơn vị gỗ\nThời gian: " + Value(preview.DurationTicks)
                + " phút trong game\nTình trạng tăng: " + Value(preview.ConditionGain) + " · tối đa " + Value(preview.ConditionCap)
                + "\n\nGỗ được giữ chỗ khi bắt đầu, tiêu thụ khi hoàn tất. Hủy chỉ bỏ giữ chỗ."
                + (preview.CanConfirm ? "" : "\n\nChưa thể xác nhận từ những thông tin hiện có.");
            focusBeforeModal = root.panel?.focusController.focusedElement as VisualElement;
            panel.SetEnabled(false);
            confirm.SetEnabled(preview.CanConfirm); modal.style.display = DisplayStyle.Flex;
            if (preview.CanConfirm) confirm.Focus(); else back.Focus();
        }
        public void ClosePreview()
        {
            if (modal == null || !ModalOpen) return;
            modal.style.display = DisplayStyle.None; panel.SetEnabled(true);
            if (focusBeforeModal != null && focusBeforeModal.enabledInHierarchy) focusBeforeModal.Focus(); else repair.Focus();
            focusBeforeModal = null;
        }
        // QA sends the same UI Toolkit submit event as keyboard navigation; this is not OS-input evidence.
        public bool ProbeModalBackgroundSubmit()
        {
            if (!ModalOpen) throw new InvalidOperationException("Preview must be open for modal probe");
            var previousResult = LastResult;
            probingBackgroundSubmit = true;
            try { using (var evt = NavigationSubmitEvent.GetPooled()) { evt.target = advanceOne; advanceOne.SendEvent(evt); } }
            finally { probingBackgroundSubmit = false; }
            return ModalOpen && !advanceOne.enabledInHierarchy && ReferenceEquals(previousResult, LastResult);
        }
        private static string Request() => Guid.NewGuid().ToString("N");
        private RepairClientResult Apply(RepairClientResult result)
        {
            LastResult = result;
            outcome.text = result.Success ? result.Durable ? "Đã thực hiện và lưu dữ liệu." : "Đã thực hiện."
                : result.Code == "stale_preview" ? "Thông tin đã đổi. Hãy mở lại bảng việc sửa."
                : "Chưa thực hiện được. Dữ liệu hiện tại được giữ nguyên; hãy quan sát hoặc mở lại dữ liệu.";
            Refresh(); return result;
        }
        public RepairClientResult ConfirmPreview()
        {
            if (!ModalOpen || preview == null || !preview.CanConfirm) return null;
            lastRequest = Request(); var result = session.ConfirmRepair(preview.Token, lastRequest); ClosePreview(); return Apply(result);
        }
        public RepairClientResult CancelRepair() { if (ModalOpen) return null; lastRequest = Request(); return Apply(session.CancelOwnRepair(lastRequest)); }
        public RepairClientResult Advance(long minutes) { if (ModalOpen) return null; lastRequest = Request(); return Apply(session.AdvanceBy(minutes, lastRequest)); }
        public RepairClientResult Save() => ModalOpen ? null : Apply(session.SaveSnapshot());
        public RepairClientResult Reload() => ModalOpen ? null : Apply(session.Reload());
        public RepairClientResult RetryLast() => ModalOpen || lastRequest == null ? null : Apply(session.Retry(lastRequest));
        public void SetTextScale(float value)
        {
            if (value != 1 && value != 1.25f && value != 1.5f) throw new ArgumentOutOfRangeException(nameof(value));
            textScale = value;
            root.Query<Label>().ForEach(l => l.style.fontSize = (l == previewText || l == state ? 18 : 16) * value);
            root.Query<Button>().ForEach(b => { b.style.fontSize = 17 * value; b.style.height = b.style.minHeight = 42 * value; });
        }
        public bool BlocksScenePointer(Vector2 screen)
        {
            if (ModalOpen) return true;
            var ui = new Vector2(screen.x, Screen.height - screen.y);
            return panel.worldBound.Contains(ui) || hud.worldBound.Contains(ui) || ui.y > Screen.height - 68;
        }
        public void ScenePointer(Vector2 screen)
        {
            if (BlocksScenePointer(screen)) return;
            if (scene.IsBoatAt(screen)) { ScenePickCount++; OpenPreview(); }
            else scene.MoveLocalFromScreen(screen);
        }
        private IEnumerator CompleteScenePointerGesture(Vector2 screen, int version)
        {
            // Finish native UI dispatch for the opening press/release before inserting a modal.
            // A single world click must never also become a click on its newly shown Confirm.
            yield return new WaitForEndOfFrame();
            if (!autoplayInputIsolated && !ModalOpen && version == sceneGestureVersion)
                ScenePointer(screen);
            InputObserved?.Invoke("Pointer");
        }
        private void Key(string key, Action action)
        { KeyboardEventCount++; action(); Debug.Log("S15_INPUT_KEY " + key); InputObserved?.Invoke(key); }
        private void Update()
        {
            if (session == null || autoplayInputIsolated) return;
            if (Input.GetKeyDown(KeyCode.Escape)) Key("Escape", ClosePreview);
            // The focused native Button owns Return/NavigationSubmit. This observes the OS key only;
            // manually invoking Confirm here would activate the same key twice and ignore Back focus.
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) Key("Return", () => { });
            if (!ModalOpen)
            {
                if (Input.GetKeyDown(KeyCode.R)) Key("R", OpenPreview);
                // Space is a global wait shortcut. Native focused-button Space is suppressed above;
                // Return remains the sole native submit route for the focused button.
                if (Input.GetKeyDown(KeyCode.Space)) Key("Space", () => Advance(1));
                if (Input.GetKeyDown(KeyCode.C)) Key("C", () => CancelRepair());
                if (Input.GetKeyDown(KeyCode.S)) Key("S", () => Save());
                if (Input.GetKeyDown(KeyCode.L)) Key("L", () => Reload());
            }
            if (Input.GetMouseButtonDown(0))
            {
                sceneGestureVersion++;
                scenePointerPressed = !BlocksScenePointer(Input.mousePosition);
            }
            if (Input.GetMouseButtonUp(0))
            {
                var dispatchScene = scenePointerPressed && !ModalOpen;
                scenePointerPressed = false;
                if (dispatchScene) StartCoroutine(CompleteScenePointerGesture(Input.mousePosition, sceneGestureVersion));
                else InputObserved?.Invoke("Pointer");
            }
            if (Input.GetKeyDown(KeyCode.Tab)) { KeyboardEventCount++; Debug.Log("S15_INPUT_KEY Tab"); InputObserved?.Invoke("Tab"); }
        }
    }
}
