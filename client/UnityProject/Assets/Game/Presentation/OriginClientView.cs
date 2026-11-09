using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Game.Application.Origins;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.Client.Sprint16
{
    /// <summary>Presentation only. Native buttons own submit; no hidden state or raw registry enters this component.</summary>
    public sealed class OriginClientView : MonoBehaviour
    {
        private IOriginCampaignGateway gateway;
        private IOriginCampaignSession session;
        private VisualElement root, header, content, card, footer, modal, worldPanel;
        private ScrollView biography;
        private Label lastBiography, worldStatus, outcome;
        private Button begin, waitOne, closeRecord;
        private CancellationTokenSource generationCancellation;
        private Task<OriginOpenResult> generationTask;
        private string createRequest, retryRequest;
        private float textScale = 1;
        private bool isolated, probingBackground;
        private VisualElement focusBeforeModal;
        public string ScreenName { get; private set; } = "Menu";
        public bool IsGenerating => ScreenName == "Generating";
        public bool RecordOpen => modal != null && modal.style.display.value != DisplayStyle.None;
        public bool HasCampaign { get; private set; }
        public bool AutoplayInputIsolated => isolated;
        public int InterceptedInputEvents { get; private set; }
        public int KeyboardEvents { get; private set; }
        public int PointerEvents { get; private set; }
        public float TextScale => textScale;
        public OriginResult LastResult { get; private set; }
        public OriginRevealView Current => session?.Read();
        public string FocusedControl => (root?.panel?.focusController.focusedElement as VisualElement)?.name ?? "";
        public Rect HeaderBounds => header?.worldBound ?? Rect.zero;
        public Rect CardBounds => card?.worldBound ?? Rect.zero;
        public Rect FooterBounds => footer?.worldBound ?? Rect.zero;
        public Rect ScrollBounds => biography?.contentViewport.worldBound ?? Rect.zero;
        public Rect BeginBounds => begin?.worldBound ?? Rect.zero;
        public bool RecordFocusContained => !RecordOpen || modal.Contains(root.panel?.focusController.focusedElement as VisualElement);
        public bool RevealLayoutFits => ScreenName == "Reveal" && Fits(root.worldBound, CardBounds, 8) &&
            CardBounds.yMin >= HeaderBounds.yMax + 8 &&
            Fits(CardBounds, FooterBounds, 0) && Fits(CardBounds, ScrollBounds, 0) && Fits(FooterBounds, BeginBounds, 0);
        public bool BiographyBottomVisible => biography != null && lastBiography != null &&
            Fits(biography.contentViewport.worldBound, lastBiography.worldBound, -1);
        public bool BiographyScrollable => biography != null && biography.verticalScroller.highValue > 0;
        public event Action GenerationCancelled;
        public event Action<string> InputObserved;
        private static readonly Color Paper = new Color(.10f, .15f, .13f, .97f), Ink = new Color(.94f, .90f, .79f);
        private static readonly Color Edge = new Color(.66f, .62f, .42f), Fill = new Color(.24f, .31f, .24f);

        private static bool Fits(Rect parent, Rect child, float margin) => child.width > 0 && child.height > 0 &&
            child.xMin >= parent.xMin + margin && child.yMin >= parent.yMin + margin &&
            child.xMax <= parent.xMax - margin && child.yMax <= parent.yMax - margin;
        /// <summary>Only fully visible enabled native buttons in the topmost UI surface are pointer targets.</summary>
        public Rect ControlBounds(string name)
        {
            var surface = RecordOpen ? modal : content;
            var button = surface?.Q<Button>(name);
            if (button == null || !button.enabledInHierarchy || !button.visible) return Rect.zero;
            var bounds = button.worldBound;
            if (!Finite(bounds.x) || !Finite(bounds.y) || !Finite(bounds.width) || !Finite(bounds.height) ||
                root == null || !Fits(root.worldBound, bounds, 0)) return Rect.zero;
            for (VisualElement item = button; item != null; item = item.parent)
            {
                if (item.resolvedStyle.display == DisplayStyle.None || item.resolvedStyle.visibility != Visibility.Visible ||
                    item.resolvedStyle.opacity <= 0) return Rect.zero;
                if (item != button && item.style.overflow.value == Overflow.Hidden && !Fits(item.worldBound, bounds, 0))
                    return Rect.zero;
            }
            return bounds;
        }
        private static bool Finite(float value) => !float.IsNaN(value) && !float.IsInfinity(value);
        public void Initialize(IOriginCampaignGateway port)
        {
            gateway = port ?? throw new ArgumentNullException(nameof(port)); BuildRoot(); RegisterInput(); ShowMenu();
        }
        private void BuildRoot()
        {
            var settings = ScriptableObject.CreateInstance<PanelSettings>();
            settings.scaleMode = PanelScaleMode.ConstantPixelSize;
            settings.themeStyleSheet = Resources.Load<ThemeStyleSheet>("Sprint15/DefaultRuntimeTheme")
                ?? throw new InvalidOperationException("Runtime UI theme missing");
            var document = gameObject.AddComponent<UIDocument>(); document.panelSettings = settings;
            root = document.rootVisualElement; root.name = "OriginClientRoot";
            root.style.position = Position.Absolute; root.style.left = root.style.right = root.style.top = root.style.bottom = 0;
            root.style.color = Ink; root.style.unityFont = Resources.Load<Font>("Sprint15/Fonts/NotoSans-Regular")
                ?? throw new InvalidOperationException("Vietnamese font missing");
            header = new VisualElement(); header.style.position = Position.Absolute;
            header.style.left = header.style.right = 20; header.style.top = 16;
            header.style.backgroundColor = Paper; Padding(header, 14);
            header.Add(Text("HỒNG BÀNG  /  BẾN VẠC", 24));
            header.Add(Text("Một cuộc đời được trao, tương lai do bạn chọn.", 15)); root.Add(header);
            content = new VisualElement(); content.name = "Screen"; content.style.position = Position.Absolute;
            content.style.left = content.style.right = 0; content.style.top = 112; content.style.bottom = 16; root.Add(content);
            header.RegisterCallback<GeometryChangedEvent>(_ =>
                content.style.top = header.worldBound.yMax - root.worldBound.yMin + 12);
        }
        private static void Padding(VisualElement item, float value)
        { item.style.paddingLeft = item.style.paddingRight = item.style.paddingTop = item.style.paddingBottom = value; }
        private static void Border(VisualElement item)
        {
            item.style.borderLeftWidth = item.style.borderRightWidth = item.style.borderTopWidth = item.style.borderBottomWidth = 1;
            item.style.borderLeftColor = item.style.borderRightColor = item.style.borderTopColor = item.style.borderBottomColor = Edge;
        }
        private Label Text(string value, int size = 18)
        {
            var label = new Label(value); label.userData = (float)size; label.style.fontSize = size * textScale;
            label.style.whiteSpace = WhiteSpace.Normal; label.style.color = Ink; label.style.flexShrink = 0;
            label.style.marginBottom = 10; return label;
        }
        private Button ActionButton(string name, string label, Action action)
        {
            var button = new Button(action) { name = name, text = label, userData = 17f };
            button.style.fontSize = 17 * textScale; button.style.whiteSpace = WhiteSpace.Normal;
            button.style.minHeight = 42 * textScale; button.style.height = 42 * textScale;
            button.style.flexShrink = 0; button.style.marginTop = 4; button.style.marginBottom = 4;
            button.style.color = Ink; button.style.backgroundColor = Fill; Border(button); return button;
        }
        private static void Focus(Button button)
        { button.Focus(); button.schedule.Execute(() => button.Focus()); }
        private VisualElement CenterCard(float width)
        {
            var wrapper = new VisualElement(); wrapper.style.flexGrow = 1;
            wrapper.style.alignItems = Align.Center; wrapper.style.justifyContent = Justify.Center;
            wrapper.style.position = Position.Absolute; wrapper.style.left = wrapper.style.right = wrapper.style.top = wrapper.style.bottom = 0;
            content.Add(wrapper);
            var result = new VisualElement(); result.style.width = width; result.style.maxWidth = Length.Percent(94);
            result.style.maxHeight = Length.Percent(98); result.style.backgroundColor = Paper;
            Padding(result, 22); Border(result); wrapper.Add(result); card = result; return result;
        }
        private void ClearScreen()
        {
            content.Clear(); modal = null; worldPanel = null; biography = null; lastBiography = null;
            card = footer = null; begin = waitOne = closeRecord = null;
        }
        public void ShowMenu()
        {
            if (IsGenerating) return;
            ScreenName = "Menu"; ClearScreen();
            var status = gateway.ReadMenu(); HasCampaign = status.HasCampaign;
            var panel = CenterCard(620); panel.style.height = Length.Percent(98);
            var menuBody = BodyScroll(panel);
            menuBody.Add(Text("Bắt đầu một cuộc đời", 30));
            menuBody.Add(Text("Nhận một người trong nhóm nhân vật đã biên soạn tại Bến Vạc. Bạn không chỉnh tuổi, gia đình hoặc quá khứ."));
            var resume = ActionButton("ContinueCampaign", "Tiếp tục cuộc đời đã lưu", ContinueCampaign); resume.SetEnabled(HasCampaign); menuBody.Add(resume);
            var create = ActionButton("NewCampaign", retryRequest == null ? "Bắt đầu cuộc đời mới" : "Thử tạo lại cuộc đời mới", () => BeginNewCampaign()); menuBody.Add(create);
            menuBody.Add(Text("Mỗi cuộc đời mới được lưu riêng. Cuộc đời đã nhận được giữ lại khi trở về đây.", 15));
            if (status.Code != "ok") menuBody.Add(Text(ErrorText(status.Code), 15));
            if (LastResult != null && !LastResult.Success) menuBody.Add(Text(ErrorText(LastResult.Code), 15));
            Focus(HasCampaign ? resume : create);
        }
        public bool BeginNewCampaign()
        {
            if (ScreenName != "Menu" || generationTask != null || RecordOpen) return false;
            createRequest = retryRequest ?? Guid.NewGuid().ToString("N");
            generationCancellation = new CancellationTokenSource();
            ScreenName = "Generating"; ClearScreen(); var panel = CenterCard(620);
            panel.Add(Text("Đang chuẩn bị cuộc đời", 28));
            panel.Add(Text("Đang tạo và lưu dữ liệu. Bạn có thể hủy trước khi cuộc đời được lưu hoàn tất."));
            var cancel = ActionButton("CancelGeneration", "Hủy tạo cuộc đời", CancelGeneration); panel.Add(cancel);
            Focus(cancel);
            var token = generationCancellation.Token;
            generationTask = Task.Run(() => gateway.NewCampaign(createRequest, token));
            StartCoroutine(CompleteGeneration()); return true;
        }
        public void CancelGeneration()
        {
            if (!IsGenerating || generationCancellation == null) return;
            generationCancellation.Cancel(); GenerationCancelled?.Invoke();
        }
        private IEnumerator CompleteGeneration()
        {
            while (!generationTask.IsCompleted) yield return null;
            OriginOpenResult result;
            try { result = generationTask.GetAwaiter().GetResult(); }
            catch (Exception error) { Debug.LogException(error); result = new OriginOpenResult(new OriginResult(false, "storage_error")); }
            generationTask = null; generationCancellation.Dispose(); generationCancellation = null;
            LastResult = result.Result;
            retryRequest = result.Result.Success || result.Result.Code == "cancelled" ? null : createRequest;
            if (result.Result.Success)
            { session = result.Session; HasCampaign = true; ShowReveal(); }
            else { ScreenName = "Menu"; ShowMenu(); }
        }
        public void ContinueCampaign()
        {
            if (ScreenName != "Menu" || generationTask != null) return;
            var result = gateway.Continue(); LastResult = result.Result;
            if (result.Result.Success) { session = result.Session; ShowReveal(); } else ShowMenu();
        }
        private ScrollView BodyScroll(VisualElement parent)
        {
            var scroll = new ScrollView(ScrollViewMode.Vertical);
            scroll.style.flexGrow = 1; scroll.style.minHeight = 0; scroll.style.flexShrink = 1;
            scroll.style.overflow = Overflow.Hidden; scroll.verticalScrollerVisibility = ScrollerVisibility.AlwaysVisible;
            scroll.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
            scroll.contentViewport.style.overflow = Overflow.Hidden; scroll.contentViewport.style.minHeight = 0;
            scroll.contentContainer.style.flexShrink = 0;
            scroll.RegisterCallback<FocusInEvent>(evt => {
                if (evt.target is VisualElement target && scroll.contentContainer.Contains(target))
                    scroll.schedule.Execute(() => scroll.ScrollTo(target));
            });
            parent.Add(scroll); return scroll;
        }
        private void AddOwnRecord(ScrollView scroll, OriginRevealView view)
        {
            scroll.Add(Text(view.ActorName + " · " + view.Age + " tuổi", 27));
            scroll.Add(Text(view.Household + "  /  " + view.Location, 18));
            scroll.Add(Text("Những điều bạn đã trải qua", 21));
            foreach (var item in view.History)
                scroll.Add(Text(item.AtAge + " tuổi — " + item.Description + "\nNguồn: " + item.Source));
            scroll.Add(Text("Kỹ năng đã học", 21));
            foreach (var skill in view.Skills)
                scroll.Add(Text(skill.Label + ": " + skill.Value + "\nHọc từ: " + skill.SourceHistory));
            scroll.Add(Text("Ký ức và nhận định riêng", 21));
            foreach (var claim in view.Claims)
                scroll.Add(Text(claim.Text + "\nNguồn: " + claim.Source + " · " + claim.AtAge + " tuổi"));
            scroll.Add(Text("Điều bạn đang mong muốn", 21));
            foreach (var need in view.Needs) scroll.Add(Text("• " + need));
            lastBiography = Text("Nhận định riêng có thể chưa phản ánh toàn bộ sự thật. Bạn chỉ thấy những gì gắn với cuộc đời mình.", 15);
            scroll.Add(lastBiography);
        }
        public void ShowReveal()
        {
            if (session == null || IsGenerating && generationTask != null) return;
            ScreenName = "Reveal"; ClearScreen();
            var panel = CenterCard(920); panel.style.height = Length.Percent(98);
            panel.Add(Text("Cuộc đời bạn nhận được", 27));
            biography = BodyScroll(panel); AddOwnRecord(biography, session.Read());
            footer = new VisualElement(); footer.style.flexShrink = 0; footer.style.flexDirection = FlexDirection.Row;
            begin = ActionButton("BeginWorld", "Bắt đầu ở Bến Vạc", BeginWorld); begin.style.flexGrow = 1; footer.Add(begin);
            var menu = ActionButton("BackMenu", "Về menu", ShowMenu); menu.style.flexGrow = 1; footer.Add(menu); panel.Add(footer);
            panel.Add(Text(session.Read().ScopeNote, 14)); Focus(begin);
        }
        public void BeginWorld()
        {
            if (ScreenName != "Reveal" || session == null) return;
            ScreenName = "World"; ClearScreen();
            var token = Text("●\nBạn", 25); token.name = "NeutralLocationMarker";
            token.style.position = Position.Absolute; token.style.left = Length.Percent(36); token.style.top = Length.Percent(50);
            token.style.unityTextAlign = TextAnchor.MiddleCenter; token.style.backgroundColor = Paper; Padding(token, 9); Border(token);
            content.Add(token);
            worldPanel = new VisualElement(); worldPanel.style.position = Position.Absolute;
            worldPanel.style.right = 20; worldPanel.style.width = 360; worldPanel.style.maxWidth = Length.Percent(44);
            worldPanel.style.top = 0; worldPanel.style.bottom = 0; worldPanel.style.backgroundColor = Paper;
            Padding(worldPanel, 16); Border(worldPanel); content.Add(worldPanel);
            var scroll = BodyScroll(worldPanel); worldStatus = Text("", 21); scroll.Add(worldStatus);
            scroll.Add(Text("Dấu vị trí trung tính đại diện cho bạn tại bến.", 15));
            waitOne = ActionButton("WaitOne", "Chờ 1 phút", () => Advance(1)); scroll.Add(waitOne);
            scroll.Add(ActionButton("WaitSixty", "Chờ 60 phút", () => Advance(60)));
            scroll.Add(ActionButton("Save", "Lưu mốc hiện tại", () => Save()));
            scroll.Add(ActionButton("Reload", "Mở lại dữ liệu đã lưu", () => Reload()));
            scroll.Add(ActionButton("SelfRecord", "Nhật ký bản thân", OpenRecord));
            scroll.Add(ActionButton("BackMenu", "Về menu", ShowMenu));
            outcome = Text("", 15); scroll.Add(outcome);
            scroll.Add(Text("Bản thử hiện cho đọc bản thân, chờ và lưu dữ liệu. Nghề nghiệp và cuộc sống trong làng sẽ được mở rộng sau.", 15));
            var scales = new VisualElement(); scales.style.flexDirection = FlexDirection.Row;
            foreach (var value in new[] { 1f, 1.25f, 1.5f })
            {
                var scale = value; var button = ActionButton("Scale" + (value * 100).ToString("0"), (value * 100).ToString("0") + "%", () => SetTextScale(scale));
                button.style.flexGrow = 1; scales.Add(button);
            }
            scroll.Add(scales); RefreshWorld(); Focus(waitOne);
        }
        private void RefreshWorld()
        {
            if (ScreenName != "World" || worldStatus == null) return;
            var view = session.Read(); worldStatus.text = view.ActorName + "\n" + view.Location + "\nPhút trong game: " + view.Tick;
            outcome.text = LastResult == null ? "" : LastResult.Success ?
                LastResult.Durable ? "Đã thực hiện và lưu dữ liệu." : "Đã thực hiện." : ErrorText(LastResult.Code);
        }
        private static string ErrorText(string code) => code == "cancelled" ? "Đã hủy trước khi lưu cuộc đời mới."
            : code == "busy" ? "Dữ liệu đang được xử lý. Hãy thử lại sau."
            : "Chưa thực hiện được. Dữ liệu hiện có được giữ nguyên; hãy thử mở lại hoặc kiểm tra nhật ký kỹ thuật.";
        private OriginResult Apply(OriginResult result) { LastResult = result; RefreshWorld(); return result; }
        public OriginResult Advance(long minutes) => ScreenName != "World" || RecordOpen ? null : Apply(session.AdvanceBy(minutes, Guid.NewGuid().ToString("N")));
        public OriginResult Save() => ScreenName != "World" || RecordOpen ? null : Apply(session.Save());
        public OriginResult Reload() => ScreenName != "World" || RecordOpen ? null : Apply(session.Reload());
        public void OpenRecord()
        {
            if (ScreenName != "World" || RecordOpen) return;
            focusBeforeModal = root.panel?.focusController.focusedElement as VisualElement; worldPanel.SetEnabled(false);
            modal = new VisualElement(); modal.name = "SelfRecordModal"; modal.style.position = Position.Absolute;
            modal.style.left = modal.style.right = modal.style.top = modal.style.bottom = 0;
            modal.style.alignItems = Align.Center; modal.style.justifyContent = Justify.Center;
            modal.style.backgroundColor = new Color(.02f, .03f, .025f, .84f);
            modal.RegisterCallback<PointerDownEvent>(evt => evt.StopPropagation()); root.Add(modal);
            var dialog = new VisualElement(); dialog.style.width = 920; dialog.style.maxWidth = Length.Percent(94);
            dialog.style.height = Length.Percent(82); dialog.style.backgroundColor = Paper; Padding(dialog, 22); Border(dialog);
            modal.Add(dialog); dialog.Add(Text("Nhật ký bản thân", 27));
            biography = BodyScroll(dialog); AddOwnRecord(biography, session.Read());
            closeRecord = ActionButton("CloseRecord", "Trở lại bến", CloseRecord); dialog.Add(closeRecord); Focus(closeRecord);
        }
        public void CloseRecord()
        {
            if (!RecordOpen) return;
            modal.RemoveFromHierarchy(); modal = null; biography = null; lastBiography = null; worldPanel.SetEnabled(true);
            if (focusBeforeModal != null && focusBeforeModal.enabledInHierarchy) focusBeforeModal.Focus(); else waitOne.Focus();
            focusBeforeModal = null;
        }
        public bool ProbeModalBackgroundSubmit()
        {
            if (!RecordOpen) return false;
            var previous = LastResult; probingBackground = true;
            try { using (var evt = NavigationSubmitEvent.GetPooled()) { evt.target = waitOne; waitOne.SendEvent(evt); } }
            finally { probingBackground = false; }
            return RecordOpen && !waitOne.enabledInHierarchy && ReferenceEquals(previous, LastResult);
        }
        public void ScrollRecordToBottom() { if (biography != null && lastBiography != null) biography.ScrollTo(lastBiography); }
        public void ScrollRecordToTop() { if (biography != null) biography.scrollOffset = Vector2.zero; }
        public void SetTextScale(float value)
        {
            if (value != 1 && value != 1.25f && value != 1.5f) throw new ArgumentOutOfRangeException(nameof(value));
            textScale = value;
            root.Query<Label>().ForEach(label => label.style.fontSize = (label.userData is float size ? size : 18f) * value);
            root.Query<Button>().ForEach(button => {
                button.style.fontSize = 17 * value; button.style.height = button.style.minHeight = 42 * value;
            });
        }
        public void IsolateAutoplayInput() { isolated = true; }
        private void Block(EventBase evt)
        { InterceptedInputEvents++; root.panel?.focusController.IgnoreEvent(evt); evt.StopImmediatePropagation(); }
        private void RegisterInput()
        {
            root.RegisterCallback<KeyDownEvent>(evt => { if (isolated) Block(evt); }, TrickleDown.TrickleDown);
            root.RegisterCallback<KeyUpEvent>(evt => { if (isolated) Block(evt); }, TrickleDown.TrickleDown);
            root.RegisterCallback<NavigationSubmitEvent>(evt => { if (isolated && !probingBackground) Block(evt); }, TrickleDown.TrickleDown);
            root.RegisterCallback<NavigationCancelEvent>(evt => { if (isolated) Block(evt); }, TrickleDown.TrickleDown);
            root.RegisterCallback<NavigationMoveEvent>(evt => { if (isolated) Block(evt); }, TrickleDown.TrickleDown);
            root.RegisterCallback<PointerDownEvent>(evt => { if (isolated) Block(evt); }, TrickleDown.TrickleDown);
            root.RegisterCallback<PointerUpEvent>(evt => { if (isolated) Block(evt); }, TrickleDown.TrickleDown);
            root.RegisterCallback<ClickEvent>(evt => { if (isolated) Block(evt); }, TrickleDown.TrickleDown);
            root.RegisterCallback<WheelEvent>(evt => { if (isolated) Block(evt); }, TrickleDown.TrickleDown);
        }
        private void Update()
        {
            if (gateway == null || isolated) return;
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                KeyboardEvents++;
                if (IsGenerating) CancelGeneration(); else if (RecordOpen) CloseRecord();
                else if (ScreenName == "Reveal" || ScreenName == "World") ShowMenu();
                Observe("Escape");
            }
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            { KeyboardEvents++; Observe("Return"); }
            if (Input.GetKeyDown(KeyCode.Tab)) { KeyboardEvents++; Observe("Tab"); }
            if (Input.GetKeyDown(KeyCode.Space)) { KeyboardEvents++; Observe("Space"); }
            if (Input.GetMouseButtonUp(0)) { PointerEvents++; Observe("Pointer"); }
        }
        private void Observe(string value) { Debug.Log("S16_INPUT_KEY " + value); InputObserved?.Invoke(value); }
        public void ShowFatal(string message)
        {
            BuildRoot(); ClearScreen(); CenterCard(720).Add(Text(message, 22));
        }
        private void OnDestroy() { generationCancellation?.Cancel(); GenerationCancelled?.Invoke(); }
    }
}
