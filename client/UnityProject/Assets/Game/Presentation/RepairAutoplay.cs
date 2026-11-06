using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Game.Application.Projection;
using UnityEngine;

namespace Game.Client.Sprint15
{
    /// <summary>Actual Player callback automation and optional event-driven OS-input evidence. Never a human playtest.</summary>
    public sealed class RepairAutoplay : MonoBehaviour
    {
        private RepairClientBootstrap host;
        private string output;
        private Report report;
        private readonly List<string> captures = new List<string>();
        private readonly List<string> inputKeys = new List<string>();
        private readonly List<LayoutSample> layouts = new List<LayoutSample>();
        private bool inputProbe, resolutionReady;
        [Serializable] private sealed class LayoutSample
        {
            public int width, height; public float scale; public bool fits;
            public Rect dialog, cost, confirm;
        }
        [Serializable] private sealed class Frame
        {
            public long tick, timber, reserved; public int condition; public string activity;
            public static Frame Read(RepairPlayerView v) => new Frame { tick = v.Tick,
                timber = v.TimberOnHand ?? -1, reserved = v.TimberReserved ?? -1,
                condition = v.BoatCondition ?? -1, activity = v.ActivityStatus };
        }
        [Serializable] private sealed class Report
        {
            public string profile = "repair-client-2d-v1", scope = "actual Linux Mono Player; callback automation, not human playtest";
            public string unityVersion, platform, backend, error;
            public bool passed, modalBlocksScene, modalBlocksActions, modalFocusContained, modalBackgroundSubmitBlocked,
                purePreview, startValid, retryValid, snapshotValid, reopenRunning,
                beforeDueValid, completionValid, cancelValid, waitValid, safePicking, occlusionBefore, occlusionBehind,
                textScaleValid, renderedCaptures, workingFeedbackValid, mainPanelScrollValid,
                autoplayInputIsolated, noDeliveredExternalInput, completionStableAfterCaptures, inputReady;
            public Frame initial, started, beforeDue, completed, cancelled, waited;
            public string[] captures, inputKeys;
            public LayoutSample[] layouts;
            public bool lastResultSuccess, lastResultDurable;
            public string lastResultCode;
            public int keyboardEvents, scenePicks, width, height, interceptedInputEvents;
        }
        private void Setup(RepairClientBootstrap bootstrap, string directory)
        {
            host = bootstrap; output = Path.GetFullPath(directory);
            if (Directory.Exists(output) && Directory.GetFileSystemEntries(output).Length != 0)
                throw new IOException("QA output must be new or empty");
            Directory.CreateDirectory(output);
            report = new Report { unityVersion = UnityEngine.Application.unityVersion,
                platform = UnityEngine.Application.platform.ToString(), backend = Type.GetType("Mono.Runtime") != null ? "Mono" : "Unknown" };
        }
        public void Begin(RepairClientBootstrap bootstrap, string directory)
        { Setup(bootstrap, directory); host.View.IsolateAutoplayInput(); StartCoroutine(RunGuarded()); }
        public void BeginInputProbe(RepairClientBootstrap bootstrap, string directory)
        {
            Setup(bootstrap, directory); inputProbe = true;
            report.scope = "actual OS-input telemetry; key markers are emitted by application handlers";
            report.initial = Frame.Read(host.Session.Read());
            host.View.InputObserved += OnInput;
            StartCoroutine(PrepareInputProbe());
        }
        private IEnumerator PrepareInputProbe()
        {
            yield return null; yield return Capture("input-initial.png");
            report.completed = Frame.Read(host.Session.Read());
            report.inputReady = true; Write();
        }
        private static void Require(bool value, string code)
        { if (!value) throw new InvalidOperationException(code); }
        private void Write()
        {
            report.captures = captures.ToArray(); report.inputKeys = inputKeys.ToArray();
            report.layouts = layouts.ToArray();
            report.keyboardEvents = host.View.KeyboardEventCount; report.scenePicks = host.View.ScenePickCount;
            report.interceptedInputEvents = host.View.InterceptedInputEvents;
            report.width = Screen.width; report.height = Screen.height;
            var lastResult = host.View.LastResult;
            report.lastResultSuccess = lastResult != null && lastResult.Success;
            report.lastResultDurable = lastResult != null && lastResult.Durable;
            report.lastResultCode = lastResult?.Code;
            File.WriteAllText(Path.Combine(output, "qa.json"), JsonUtility.ToJson(report, true));
        }
        private IEnumerator Capture(string name)
        {
            yield return new WaitForEndOfFrame();
            var texture = ScreenCapture.CaptureScreenshotAsTexture();
            try
            {
                if (texture == null || texture.width != Screen.width || texture.height != Screen.height)
                    throw new InvalidOperationException("CAPTURE_DIMENSIONS");
                File.WriteAllBytes(Path.Combine(output, name), texture.EncodeToPNG()); captures.Add(name);
            }
            finally { if (texture != null) Destroy(texture); }
        }
        private IEnumerator RunGuarded()
        {
            var routine = Run();
            while (true)
            {
                object current;
                try { if (!routine.MoveNext()) break; current = routine.Current; }
                catch (Exception error)
                {
                    report.error = error.GetType().Name + ": " + error.Message; report.passed = false;
                    Debug.LogException(error); Write(); UnityEngine.Application.Quit(1); yield break;
                }
                yield return current;
            }
            Write(); Debug.Log("S15_AUTOPLAY_PASSED " + report.passed); UnityEngine.Application.Quit(report.passed ? 0 : 1);
        }
        private IEnumerator Run()
        {
            yield return CaptureResolution(1280, 720); Require(resolutionReady, "INITIAL_RESOLUTION");
            var view = host.View; var scene = host.Scene; var start = host.Session.Read(); report.initial = Frame.Read(start);
            Require(start.Tick == 0 && start.BoatCondition == 500 && start.TimberOnHand == 10 && start.TimberReserved == 0, "FRESH_FIXTURE");
            yield return Capture("capture-1280-initial.png");
            view.ScrollToLowerControls(); yield return null; yield return null;
            report.mainPanelScrollValid = view.LowerControlsVisible;
            Require(report.mainPanelScrollValid, "MAIN_PANEL_LOWER_CONTROLS_SCROLL");
            yield return Capture("capture-1280-scroll-bottom.png"); view.ScrollToTop(); yield return null; yield return null;
            var before = Frame.Read(host.Session.Read()); view.OpenPreview(); var after = Frame.Read(host.Session.Read());
            report.purePreview = JsonUtility.ToJson(before) == JsonUtility.ToJson(after);
            Require(view.ModalOpen && report.purePreview && view.CurrentPreview.CanConfirm, "PREVIEW");
            var oldFoot = scene.ActorFoot; var picks = view.ScenePickCount;
            view.ScenePointer(scene.SceneCamera.WorldToScreenPoint(scene.BoatFoot));
            report.modalBlocksScene = oldFoot == scene.ActorFoot && picks == view.ScenePickCount;
            Require(report.modalBlocksScene, "MODAL_CLICK_THROUGH");
            report.modalBlocksActions = view.Advance(1) == null && view.CancelRepair() == null && view.Save() == null &&
                view.Reload() == null && view.RetryLast() == null && view.ModalOpen &&
                JsonUtility.ToJson(before) == JsonUtility.ToJson(Frame.Read(host.Session.Read()));
            Require(report.modalBlocksActions, "MODAL_ACTION_THROUGH");
            report.modalBackgroundSubmitBlocked = view.ProbeModalBackgroundSubmit();
            Require(report.modalBackgroundSubmitBlocked, "MODAL_BACKGROUND_NAVIGATION_SUBMIT");
            yield return null;
            report.modalFocusContained = view.ModalBackgroundDisabled && view.ModalFocusContained;
            Require(report.modalFocusContained, "MODAL_FOCUS_ESCAPE");
            report.textScaleValid = true;
            foreach (var resolution in new[] { new Vector2Int(1280, 720), new Vector2Int(1920, 1080) })
            {
                yield return CaptureResolution(resolution.x, resolution.y); Require(resolutionReady, "PREVIEW_RESOLUTION");
                foreach (var scale in new[] { 1f, 1.25f, 1.5f })
                {
                    view.ClosePreview(); view.SetTextScale(scale); view.OpenPreview(); yield return null; yield return null;
                    var sample = new LayoutSample { width = Screen.width, height = Screen.height, scale = scale,
                        fits = view.PreviewLayoutFits && Screen.width == resolution.x && Screen.height == resolution.y,
                        dialog = view.PreviewDialogBounds, cost = view.PreviewCostBounds, confirm = view.PreviewConfirmBounds };
                    layouts.Add(sample); report.textScaleValid &= sample.fits;
                    yield return Capture("capture-" + resolution.x + "-preview-" + (scale * 100).ToString("0") + ".png");
                }
            }
            Require(report.textScaleValid && layouts.Count == 6, "PREVIEW_LAYOUT_BOUNDS");
            // Both resolution matrices were exercised above. Continue the action flow in the
            // current borderless 1920 frame; decorated-window restoration is not this proof's gate.
            view.SetTextScale(1); yield return null; yield return null;
            var confirmed = view.ConfirmPreview(); report.started = Frame.Read(host.Session.Read());
            report.startValid = confirmed != null && confirmed.Success && confirmed.Durable && report.started.timber == 10 && report.started.reserved == 2;
            Require(report.startValid, "START");
            report.workingFeedbackValid = scene.WorkingFeedbackActive;
            Require(report.workingFeedbackValid, "WORKING_PRESENTATION_FEEDBACK");
            var retry = view.RetryLast(); report.retryValid = retry != null && retry.Success && host.Session.Read().TimberReserved == 2;
            Require(report.retryValid, "RETRY");
            report.snapshotValid = view.Save().Success; Require(report.snapshotValid, "SNAPSHOT");
            host.ReopenCurrentQaWorld();
            report.reopenRunning = host.Session.Read().ActivityStatus == "running" && host.Session.Read().TimberReserved == 2;
            Require(report.reopenRunning && host.View.Reload().Success, "REOPEN_RUNNING");
            Require(host.View.Advance(59).Success, "ADVANCE_59"); report.beforeDue = Frame.Read(host.Session.Read());
            report.beforeDueValid = report.beforeDue.tick == 59 && report.beforeDue.condition == 500 && report.beforeDue.reserved == 2;
            Require(report.beforeDueValid, "EARLY_REPAIR");
            Require(host.View.Advance(1).Success, "ADVANCE_1"); report.completed = Frame.Read(host.Session.Read());
            report.completionValid = report.completed.tick == 60 && report.completed.condition == 700 && report.completed.timber == 8 && report.completed.reserved == 0 && report.completed.activity == "completed";
            Require(report.completionValid && !scene.WorkingFeedbackActive && host.View.Reload().Success, "COMPLETION");
            yield return CaptureResolution(1920, 1080); Require(resolutionReady, "COMPLETED_RESOLUTION");
            host.View.SetTextScale(1.25f); yield return null; yield return Capture("capture-1920-completed-125.png"); host.View.SetTextScale(1);
            var post = scene.ForegroundFoot;
            scene.PlaceLocalFoot(new Vector3(post.x + .32f, post.y - .18f, 0));
            report.occlusionBefore = scene.ActorSortingOrder > scene.ForegroundSortingOrder;
            yield return Capture("capture-1920-occlusion-front.png");
            scene.PlaceLocalFoot(new Vector3(post.x + .32f, post.y + .18f, 0));
            report.occlusionBehind = scene.ActorSortingOrder < scene.ForegroundSortingOrder;
            yield return Capture("capture-1920-occlusion-behind.png");
            Require(report.occlusionBefore && report.occlusionBehind, "OCCLUSION_SORT");
            // Only the observed bound boat has a target renderer. An off-scene point cannot pick a hidden fixture entity.
            report.safePicking = !scene.IsBoatAt(new Vector2(-1000, -1000)); Require(report.safePicking, "SAFE_PICK");
            // Validate the state again after all rendered frames and a durable reload. An early
            // sample alone can miss an unintended second command arriving during screenshot waits.
            yield return null; yield return new WaitForEndOfFrame();
            Require(host.View.Reload().Success, "POST_CAPTURE_RELOAD");
            report.completionStableAfterCaptures = JsonUtility.ToJson(report.completed) == JsonUtility.ToJson(Frame.Read(host.Session.Read()));
            report.autoplayInputIsolated = host.View.AutoplayInputIsolated;
            report.noDeliveredExternalInput = host.View.KeyboardEventCount == 0;
            Require(report.completionStableAfterCaptures && report.autoplayInputIsolated && report.noDeliveredExternalInput,
                "POST_CAPTURE_COMPLETION_STABLE");
            host.SwitchQaBranch("cancel"); host.View.OpenPreview(); Require(host.View.ConfirmPreview().Success, "CANCEL_START");
            Require(host.View.Save().Success && host.View.Reload().Success, "CANCEL_SAVE_RELOAD");
            var cancelled = host.View.CancelRepair(); report.cancelled = Frame.Read(host.Session.Read());
            report.cancelValid = cancelled.Success && cancelled.Durable && report.cancelled.timber == 10 && report.cancelled.reserved == 0 && report.cancelled.condition == 500 && report.cancelled.activity == "cancelled";
            Require(report.cancelValid, "CANCEL");
            host.SwitchQaBranch("wait"); Require(host.View.Advance(60).Success, "WAIT"); report.waited = Frame.Read(host.Session.Read());
            report.waitValid = report.waited.tick == 60 && report.waited.timber == 10 && report.waited.reserved == 0 && report.waited.condition == 500 && report.waited.activity == "none";
            Require(report.waitValid, "WAIT_WITHOUT_REPAIR");
            yield return Capture("capture-1920-waited.png");
            report.renderedCaptures = captures.Count >= 6;
            report.passed = report.renderedCaptures && report.modalBlocksScene && report.modalBlocksActions && report.modalFocusContained &&
                report.modalBackgroundSubmitBlocked && report.purePreview && report.startValid && report.retryValid && report.snapshotValid && report.reopenRunning && report.beforeDueValid && report.completionValid && report.cancelValid && report.waitValid && report.safePicking && report.occlusionBefore && report.occlusionBehind && report.textScaleValid && report.workingFeedbackValid && report.mainPanelScrollValid &&
                report.autoplayInputIsolated && report.noDeliveredExternalInput && report.completionStableAfterCaptures;
        }
        private IEnumerator CaptureResolution(int width, int height)
        {
            // A 1920x1080 decorated window cannot fit a 1920x1080 monitor. Borderless fullscreen
            // produces a real drawable of the target size instead of capturing a clipped swapchain.
            var mode = width >= 1920 ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
            resolutionReady = false; Screen.SetResolution(width, height, mode);
            var deadline = Time.realtimeSinceStartup + 15; var stable = 0;
            while (stable < 12 && Time.realtimeSinceStartup < deadline)
            {
                if (Screen.width == width && Screen.height == height && Screen.fullScreenMode == mode) stable++;
                else stable = 0;
                yield return null;
            }
            resolutionReady = stable >= 12; yield return new WaitForEndOfFrame();
        }
        private void OnInput(string key)
        {
            inputKeys.Add(key); StartCoroutine(RecordInput(key));
        }
        private IEnumerator RecordInput(string key)
        {
            yield return null; yield return new WaitForEndOfFrame();
            report.completed = Frame.Read(host.Session.Read());
            report.modalBlocksScene = host.View.ModalOpen;
            Write();
        }
        private void Update()
        {
            if (inputProbe && Input.GetKeyDown(KeyCode.F10)) StartCoroutine(FinishInput());
        }
        private IEnumerator FinishInput()
        {
            inputProbe = false; yield return Capture("input-final.png"); report.completed = Frame.Read(host.Session.Read());
            report.passed = inputKeys.Count > 0; Write(); Debug.Log("S15_INPUT_PROBE_FINISHED"); UnityEngine.Application.Quit(0);
        }
    }
}
