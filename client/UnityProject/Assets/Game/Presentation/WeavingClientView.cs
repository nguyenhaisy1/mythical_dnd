using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Game.Application.Origins;
using Game.Application.Weaving;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.Client.Sprint17
{
    /// <summary>Presentation only. Native buttons own submit; no hidden state or raw registry enters this component.</summary>
    public sealed class WeavingClientView : MonoBehaviour
    {
        private IWeavingCampaignGateway gateway;
        private IWeavingCampaignSession session;
        private VisualElement root, header, content, card, footer, modal, worldPanel, previewDialog, previewFooter;
        private ScrollView biography, worldScroll;
        private VisualElement worldLast;
        private Label lastBiography, worldStatus, workStatus, outcome;
        private Button previewWork, cancelWork, confirmWork;
        private WeavingPreview preview, retryStartPreview;
        private string retryStartRequest, retryCancelRequest, retryAdvanceRequest;
        private long retryAdvanceMinutes;
        private string modalKind;
        private bool confirmPress;
        private int modalOpenedFrame;
        private Button begin, waitOne, closeRecord;
        private CancellationTokenSource generationCancellation;
        private Task<WeavingOpenResult> generationTask;
        private string createRequest, retryRequest;
        private float textScale = 1;
        private bool isolated, probingBackground;
        private VisualElement focusBeforeModal;
        public string ScreenName { get; private set; } = "Menu";
        public bool IsGenerating => ScreenName == "Generating";
        public bool ModalOpen => modal != null && modal.style.display.value != DisplayStyle.None;
        public bool RecordOpen => ModalOpen && modalKind == "record";
        public bool PreviewOpen => ModalOpen && modalKind == "preview";
        public bool PreviewAllowed => PreviewOpen && preview != null && preview.Allowed;
        public WeavingWorkView CurrentWork => session?.ReadWork();
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
        public Rect PreviewCardBounds => previewDialog?.worldBound ?? Rect.zero;
        public Rect ConfirmBounds => confirmWork?.worldBound ?? Rect.zero;
        public Rect PreviewFooterBounds => previewFooter?.worldBound ?? Rect.zero;
        public bool PreviewLayoutFits => PreviewOpen && Fits(root.worldBound, PreviewCardBounds, 8)
            && Fits(PreviewCardBounds, PreviewFooterBounds, 0) && Fits(PreviewFooterBounds, ConfirmBounds, 0);
        public bool RecordFocusContained => !ModalOpen || modal.Contains(root.panel?.focusController.focusedElement as VisualElement);
        public bool RevealLayoutFits => ScreenName == "Reveal" && Fits(root.worldBound, CardBounds, 8) &&
            CardBounds.yMin >= HeaderBounds.yMax + 8 &&
            Fits(CardBounds, FooterBounds, 0) && Fits(CardBounds, ScrollBounds, 0) && Fits(FooterBounds, BeginBounds, 0);
        public bool BiographyBottomVisible => biography != null && lastBiography != null &&
            Fits(biography.contentViewport.worldBound, lastBiography.worldBound, -1);
        public bool BiographyScrollable => biography != null && biography.verticalScroller.highValue > 0;
        public Rect WorldPanelBounds => worldPanel?.worldBound ?? Rect.zero;
        public Rect WorldViewportBounds => worldScroll?.contentViewport.worldBound ?? Rect.zero;
        public bool WorldLayoutFits => ScreenName == "World" && Fits(root.worldBound, WorldPanelBounds, 8)
            && WorldPanelBounds.yMin >= HeaderBounds.yMax + 8 && Fits(WorldPanelBounds, WorldViewportBounds, 0);
        public bool WorldScrollable => worldScroll != null && worldScroll.verticalScroller.highValue > 0;
        public bool WorldBottomVisible => worldScroll != null && worldLast != null && Fits(WorldViewportBounds, worldLast.worldBound, -1);
        public void ScrollWorldToTop() { if (worldScroll != null) worldScroll.scrollOffset = Vector2.zero; }
        public void ScrollWorldToBottom() { if (worldScroll != null) worldScroll.scrollOffset = new Vector2(0, worldScroll.verticalScroller.highValue); }
        public bool FocusWorldControl(string name)
        {
            if (ScreenName != "World" || ModalOpen || worldScroll == null) return false;
            var button = worldPanel.Q<Button>(name);
            if (button == null || !button.enabledInHierarchy) return false;
            button.Focus(); worldScroll.ScrollTo(button); return true;
        }
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
            var surface = ModalOpen ? modal : content;
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
        public void Initialize(IWeavingCampaignGateway port)
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
            content.Clear(); if (modal != null) modal.RemoveFromHierarchy(); modal = null; modalKind = null; preview = null; worldPanel = null; worldScroll = null; worldLast = null; biography = null; lastBiography = null;
            card = footer = null; begin = waitOne = closeRecord = null;
        }
        public void ShowMenu()
        {
            if (IsGenerating || ModalOpen) return;
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
            if (ScreenName != "Menu" || generationTask != null || ModalOpen) return false;
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
            WeavingOpenResult result;
            try { result = generationTask.GetAwaiter().GetResult(); }
            catch (Exception error) { Debug.LogException(error); result = new WeavingOpenResult(new OriginResult(false, "storage_error")); }
            generationTask = null; generationCancellation.Dispose(); generationCancellation = null;
            LastResult = result.Result;
            retryRequest = result.Result.Success || result.Result.Code == "cancelled" ? null : createRequest;
            if (result.Result.Success)
            { session = result.Session; ResetActionRetries(); HasCampaign = true; ShowReveal(); }
            else { ScreenName = "Menu"; ShowMenu(); }
        }
        public void ContinueCampaign()
        {
            if (ScreenName != "Menu" || generationTask != null) return;
            var result = gateway.Continue(); LastResult = result.Result;
            if (result.Result.Success) { session = result.Session; ResetActionRetries(); ShowReveal(); } else ShowMenu();
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
            token.style.position = Position.Absolute; token.style.left = Length.Percent(24); token.style.top = Length.Percent(50);
            token.style.unityTextAlign = TextAnchor.MiddleCenter; token.style.backgroundColor = Paper; Padding(token, 9); Border(token);
            content.Add(token);
            worldPanel = new VisualElement(); worldPanel.style.position = Position.Absolute;
            worldPanel.style.right = 20; worldPanel.style.width = 560; worldPanel.style.maxWidth = Length.Percent(58);
            worldPanel.style.top = 0; worldPanel.style.bottom = 0; worldPanel.style.backgroundColor = Paper;
            Padding(worldPanel, 16); Border(worldPanel); content.Add(worldPanel);
            var scroll = BodyScroll(worldPanel); worldScroll = scroll; worldStatus = Text("", 21); scroll.Add(worldStatus);
            workStatus = Text("", 18); scroll.Add(workStatus);
            previewWork = ActionButton("PreviewWeaving", "Xem việc đan dây", OpenWorkPreview);
            cancelWork = ActionButton("CancelWeaving", "Hủy việc đang làm", () => CancelWork());
            AddRow(scroll, previewWork, cancelWork);
            waitOne = ActionButton("WaitOne", "Chờ 1 phút", () => Advance(1));
            AddRow(scroll, waitOne, ActionButton("WaitSixty", "Chờ 60 phút", () => Advance(60)));
            AddRow(scroll, ActionButton("Save", "Lưu mốc hiện tại", () => Save()),
                ActionButton("Reload", "Mở dữ liệu đã lưu", () => Reload()));
            AddRow(scroll, ActionButton("SelfRecord", "Nhật ký bản thân", OpenRecord),
                ActionButton("BackMenu", "Về menu", ShowMenu));
            outcome = Text("", 15); scroll.Add(outcome);
            scroll.Add(Text("Vật liệu cá nhân được cấp sẵn trong bản thử mới; không phải quyền dùng kho gia đình. Dùng tay đan, chưa mô phỏng dụng cụ hay tiến bộ nghề.", 15));
            scroll.Add(Text("Dấu vị trí trung tính đại diện cho bạn tại bến; chưa là tạo hình hoàn chỉnh của nhân vật.", 14));
            var scales = new VisualElement(); scales.style.flexDirection = FlexDirection.Row;
            foreach (var value in new[] { 1f, 1.25f, 1.5f })
            {
                var scale = value; var button = ActionButton("Scale" + (value * 100).ToString("0"), (value * 100).ToString("0") + "%", () => SetTextScale(scale));
                button.style.flexGrow = 1; scales.Add(button);
            }
            scroll.Add(scales); worldLast = scales; RefreshWorld(); Focus(waitOne);
        }
        private static VisualElement AddRow(VisualElement parent, Button first, Button second)
        {
            var row = new VisualElement(); row.style.flexDirection = FlexDirection.Row; row.style.flexShrink = 0;
            first.style.flexGrow = second.style.flexGrow = 1; first.style.flexBasis = second.style.flexBasis = 0;
            first.style.minWidth = second.style.minWidth = 0; row.Add(first); row.Add(second); parent.Add(row); return row;
        }
        private void RefreshWorld()
        {
            if (ScreenName != "World" || worldStatus == null) return;
            var view = session.Read(); var work = session.ReadWork();
            worldStatus.text = view.ActorName + " · " + view.Location + "\nPhút trong game: " + view.Tick;
            workStatus.text = "Sợi cá nhân: " + work.FiberOnHand + " · đang giữ cho việc: " + work.FiberReserved
                + "\nDây đã đan: " + work.CordOnHand + "\n"
                + (work.Qualified ? "Kỹ năng đan: " + work.SkillValue + " / cần " + work.RequiredSkill
                    : "Bản thử chưa hỗ trợ công việc phù hợp với xuất thân này.")
                + "\n" + (work.Status == "running" ? "Đang đan dây — hoàn tất ở phút " + work.DueTick
                    : work.Status == "completed" ? "Đã hoàn tất một đoạn dây."
                    : work.Status == "cancelled" ? "Đã hủy việc; phần nguyên liệu đang giữ được trả về khả dụng."
                    : "Chưa có công việc đang làm.");
            previewWork.SetEnabled(work.CanStart || retryStartRequest != null);
            previewWork.text = retryStartRequest == null ? "Xem việc đan dây" : "Xác nhận lại việc vừa rồi";
            cancelWork.SetEnabled(work.CanCancel || retryCancelRequest != null);
            outcome.text = LastResult == null ? "" : LastResult.Success ?
                LastResult.Durable ? "Đã thực hiện và lưu dữ liệu." : "Đã thực hiện." : ErrorText(LastResult.Code);
        }
        private static string ErrorText(string code) => code == "cancelled" ? "Đã hủy trước khi lưu cuộc đời mới."
            : code == "busy" ? "Dữ liệu đang được xử lý. Hãy thử lại sau."
            : code == "stale_preview" ? "Dữ liệu đã thay đổi. Hãy xem lại công việc trước khi xác nhận."
            : code == "not_qualified" || code == "unsupported_origin" ? "Bản thử chưa hỗ trợ công việc phù hợp với xuất thân này."
            : "Chưa thực hiện được. Dữ liệu hiện có được giữ nguyên; hãy thử mở lại hoặc kiểm tra nhật ký kỹ thuật.";
        private OriginResult Apply(OriginResult result) { LastResult = result; RefreshWorld(); return result; }
        private static bool Uncertain(OriginResult value) => !value.Success && (value.Code == "session_error" || value.Code == "storage_error");
        private void ResetActionRetries()
        { retryStartPreview = null; retryStartRequest = retryCancelRequest = retryAdvanceRequest = null; }
        public OriginResult Advance(long minutes)
        {
            if (ScreenName != "World" || ModalOpen) return null;
            var request = retryAdvanceRequest != null && retryAdvanceMinutes == minutes ? retryAdvanceRequest : Guid.NewGuid().ToString("N");
            var result = session.AdvanceBy(minutes, request);
            retryAdvanceRequest = Uncertain(result) ? request : null; retryAdvanceMinutes = minutes; return Apply(result);
        }
        public OriginResult Save() => ScreenName != "World" || ModalOpen ? null : Apply(session.Save());
        public OriginResult Reload() => ScreenName != "World" || ModalOpen ? null : Apply(session.Reload());
        public void OpenRecord()
        {
            if (ScreenName != "World" || ModalOpen) return;
            focusBeforeModal = root.panel?.focusController.focusedElement as VisualElement; worldPanel.SetEnabled(false);
            modalKind = "record"; modal = new VisualElement(); modal.name = "SelfRecordModal"; modal.style.position = Position.Absolute;
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
            modal.RemoveFromHierarchy(); modal = null; modalKind = null; biography = null; lastBiography = null; worldPanel.SetEnabled(true);
            RestoreWorldFocus();
        }
        public bool OpenPreview()
        {
            if (ScreenName != "World" || ModalOpen) return false;
            preview = retryStartPreview ?? session.Preview();
            if (!preview.Allowed) { Apply(new OriginResult(false, preview.Code)); preview = null; return false; }
            focusBeforeModal = root.panel?.focusController.focusedElement as VisualElement;
            worldPanel.SetEnabled(false); modalKind = "preview"; modalOpenedFrame = Time.frameCount; confirmPress = false;
            modal = new VisualElement(); modal.name = "WeavingPreviewModal";
            modal.style.position = Position.Absolute; modal.style.left = modal.style.right = modal.style.top = modal.style.bottom = 0;
            modal.style.alignItems = Align.Center; modal.style.justifyContent = Justify.Center;
            modal.style.backgroundColor = new Color(.02f, .03f, .025f, .84f);
            modal.RegisterCallback<PointerDownEvent>(evt => evt.StopPropagation()); root.Add(modal);
            var dialog = new VisualElement(); dialog.style.width = 760; dialog.style.maxWidth = Length.Percent(94);
            dialog.style.height = Length.Percent(66); dialog.style.maxHeight = Length.Percent(90); dialog.style.backgroundColor = Paper; Padding(dialog, 22); Border(dialog); modal.Add(dialog); previewDialog = dialog;
            dialog.Add(Text("Đan một đoạn dây cá nhân", 27));
            var body = BodyScroll(dialog);
            if (retryStartRequest != null) body.Add(Text("Thử xác nhận lại công việc vừa rồi.", 17));
            body.Add(Text("Giữ " + preview.FiberCost + " bó sợi trong lúc làm. Sau " + preview.DurationTicks
                + " phút, dùng số sợi đó để nhận " + preview.CordYield + " đoạn dây."));
            body.Add(Text("Dùng tay đan tại Bến Vạc. Hủy trước khi hoàn tất sẽ bỏ phần giữ chỗ, không tạo sản phẩm. Xem trước chưa tiêu hao nguyên liệu hoặc thời gian.", 17));
            confirmWork = ActionButton("ConfirmWeaving", "Xác nhận bắt đầu đan", () => {
                if (!confirmPress) return; confirmPress = false; ConfirmPreview();
            });
            // Observe the fresh press in capture phase before Button's Clickable consumes it.
            confirmWork.RegisterCallback<PointerDownEvent>(_ => { if (Time.frameCount > modalOpenedFrame) confirmPress = true; }, TrickleDown.TrickleDown);
            confirmWork.RegisterCallback<NavigationSubmitEvent>(evt => {
                if (Time.frameCount > modalOpenedFrame && ReferenceEquals(evt.target, confirmWork)) confirmPress = true;
            }, TrickleDown.TrickleDown);
            confirmWork.RegisterCallback<KeyDownEvent>(evt => {
                if (Time.frameCount > modalOpenedFrame && (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter || evt.keyCode == KeyCode.Space)) confirmPress = true;
            }, TrickleDown.TrickleDown);
            var back = ActionButton("BackPreview", "Quay lại, chưa làm", ClosePreview);
            previewFooter = AddRow(dialog, confirmWork, back); Focus(back); return true;
        }
        private void OpenWorkPreview() { OpenPreview(); }
        // Explicit callback QA entry; normal native Button uses the fresh-press guard above.
        public OriginResult ConfirmPreview()
        {
            if (!PreviewOpen || preview == null || !preview.Allowed) return null;
            var request = retryStartRequest ?? Guid.NewGuid().ToString("N");
            var attempt = preview; var result = session.Start(attempt.Token, request);
            if (Uncertain(result)) { retryStartRequest = request; retryStartPreview = attempt; }
            else { retryStartRequest = null; retryStartPreview = null; }
            ClosePreview(); return Apply(result);
        }
        public void ClosePreview()
        {
            if (!PreviewOpen) return;
            modal.RemoveFromHierarchy(); modal = null; modalKind = null; preview = null; confirmWork = null; previewDialog = previewFooter = null; confirmPress = false;
            worldPanel.SetEnabled(true); RestoreWorldFocus();
        }
        private void RestoreWorldFocus()
        {
            if (focusBeforeModal != null && focusBeforeModal.enabledInHierarchy) focusBeforeModal.Focus(); else waitOne.Focus();
            focusBeforeModal = null;
        }
        public OriginResult CancelWork()
        {
            if (ScreenName != "World" || ModalOpen) return null;
            var request = retryCancelRequest ?? Guid.NewGuid().ToString("N"); var result = session.Cancel(request);
            retryCancelRequest = Uncertain(result) ? request : null; return Apply(result);
        }
        public bool ProbeModalBackgroundSubmit()
        {
            if (!ModalOpen) return false;
            var previous = LastResult; probingBackground = true;
            try { using (var evt = NavigationSubmitEvent.GetPooled()) { evt.target = waitOne; waitOne.SendEvent(evt); } }
            finally { probingBackground = false; }
            return ModalOpen && !waitOne.enabledInHierarchy && ReferenceEquals(previous, LastResult);
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
                if (IsGenerating) CancelGeneration(); else if (PreviewOpen) ClosePreview(); else if (RecordOpen) CloseRecord();
                else if (ScreenName == "Reveal" || ScreenName == "World") ShowMenu();
                Observe("Escape");
            }
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            { KeyboardEvents++; Observe("Return"); }
            if (Input.GetKeyDown(KeyCode.Tab)) { KeyboardEvents++; Observe("Tab"); }
            if (Input.GetKeyDown(KeyCode.Space)) { KeyboardEvents++; Observe("Space"); }
            if (Input.GetMouseButtonUp(0)) { PointerEvents++; Observe("Pointer"); }
        }
        private void Observe(string value) { Debug.Log("S17_INPUT_KEY " + value); InputObserved?.Invoke(value); }
        public void ShowFatal(string message)
        {
            BuildRoot(); ClearScreen(); CenterCard(720).Add(Text(message, 22));
        }
        private void OnDestroy() { generationCancellation?.Cancel(); GenerationCancelled?.Invoke(); }
    }
}
