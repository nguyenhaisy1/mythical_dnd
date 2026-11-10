using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Game.Application.Origins;
using Game.Application.Weaving;
using UnityEngine;

namespace Game.Client.Sprint17
{
    /// <summary>Actual renderer/callback automation and separate OS-input telemetry, never a human playtest.</summary>
    public sealed class WeavingAutoplay : MonoBehaviour
    {
        private WeavingClientBootstrap host;
        private string output;
        private Report report;
        private readonly List<string> captures = new List<string>(), inputKeys = new List<string>();
        private readonly List<Layout> layouts = new List<Layout>();
        private readonly List<WorldLayout> worldLayouts = new List<WorldLayout>();
        private bool inputProbe, inputWriteQueued, resolutionReady;
        private string observedSignature;
        [Serializable] private sealed class Frame
        {
            public string actorName, household, location, recordFingerprint;
            public int age, historyCount, skillCount, claimCount;
            public long tick;
            public static Frame Read(OriginRevealView view)
            {
                if (view == null) return null;
                var record = view.ActorName + "\n" + view.Age + "\n" + view.Household + "\n" + view.Location
                    + "\n" + string.Join("\n", view.History.Select(h => h.AtAge + "|" + h.Description + "|" + h.Source))
                    + "\n" + string.Join("\n", view.Skills.Select(s => s.Label + "|" + s.Value + "|" + s.SourceHistory))
                    + "\n" + string.Join("\n", view.Claims.Select(c => c.Text + "|" + c.Source + "|" + c.AtAge))
                    + "\n" + string.Join("\n", view.Needs);
                using var sha = SHA256.Create();
                return new Frame { actorName = view.ActorName, household = view.Household, location = view.Location,
                    age = view.Age, historyCount = view.History.Count, skillCount = view.Skills.Count, claimCount = view.Claims.Count,
                    tick = view.Tick, recordFingerprint = string.Concat(sha.ComputeHash(Encoding.UTF8.GetBytes(record)).Select(b => b.ToString("x2"))) };
            }
        }
        [Serializable] private sealed class WorkFrame
        {
            public bool qualified, canStart, canCancel;
            public int skillValue, requiredSkill;
            public long fiberOnHand, fiberReserved, cordOnHand, tick, dueTick;
            public string status;
            public static WorkFrame Read(WeavingWorkView value) => value == null ? null : new WorkFrame {
                qualified = value.Qualified, canStart = value.CanStart, canCancel = value.CanCancel,
                skillValue = value.SkillValue, requiredSkill = value.RequiredSkill, fiberOnHand = value.FiberOnHand,
                fiberReserved = value.FiberReserved, cordOnHand = value.CordOnHand, tick = value.Tick,
                dueTick = value.DueTick, status = value.Status };
        }
        [Serializable] private sealed class Layout
        {
            public int width, height; public float scale;
            public bool fits, bottomVisible, scrollable;
            public Rect header, card, footer, scroll, begin;
        }
        [Serializable] private sealed class WorldLayout
        {
            public int width, height; public float scale;
            public bool fits, bottomVisible, scrollable, controlsReachable;
            public Rect panel, viewport;
        }
        [Serializable] private sealed class Target { public string name; public Rect boundsRect; }
        [Serializable] private sealed class Report
        {
            public string profile = "personal-weaving-v1", scope = "actual Unity LinuxMono callback/render automation; not human playtest";
            public string unityVersion, platform, backend, error = "", screen, focus, lastResultCode;
            public bool passed, inputReady, publicationHeld, hasCampaign, recordOpen, previewOpen, lastResultSuccess, lastResultDurable;
            public bool qualifiedScenario, previewPure, previewLayoutValid, startValid, saveRunningValid, reopenRunning, beforeDueValid, completionValid, cancelValid, nonWeaverDenied;
            public WorkFrame workInitial, workRunning, workBeforeDue, workCompleted, workCancelled, workAfterContinue, workCurrent;
            public bool menuValid, cancelBeforePublishValid, duplicateNewBlocked, newCampaignValid,
                beginNoMutation, waitOneValid, waitSixtyValid, saveValid, reloadValid, continueSameOrigin,
                delayedStateStable, selfRecordPure, modalFocusContained, modalBlocksActions,
                layoutValid, scrollValid, renderedCaptures, autoplayInputIsolated, noDeliveredExternalInput;
            public int width, height, keyboardEvents, pointerEvents, interceptedInputEvents;
            public Frame initial, afterWait, afterContinue, current;
            public string[] captures, inputKeys;
            public Layout[] layouts;
            public WorldLayout[] worldLayouts;
            public bool worldLayoutValid, worldScrollValid, worldFocusValid;
            public Target[] targets;
        }
        private void Setup(WeavingClientBootstrap bootstrap, string directory)
        {
            host = bootstrap; output = Path.GetFullPath(directory);
            if (Directory.Exists(output) && Directory.GetFileSystemEntries(output).Length != 0)
                throw new IOException("QA output must be new or empty");
            Directory.CreateDirectory(output);
            report = new Report { unityVersion = UnityEngine.Application.unityVersion,
                platform = UnityEngine.Application.platform.ToString(),
                backend = Type.GetType("Mono.Runtime") != null ? "Mono" : "Unknown" };
        }
        public void Begin(WeavingClientBootstrap bootstrap, string directory)
        { Setup(bootstrap, directory); host.View.IsolateAutoplayInput(); StartCoroutine(RunGuarded()); }
        public void BeginInputProbe(WeavingClientBootstrap bootstrap, string directory)
        {
            Setup(bootstrap, directory); inputProbe = true;
            report.scope = "actual OS-input telemetry; markers emitted by application handlers";
            host.View.InputObserved += value => { inputKeys.Add(value); QueueInputWrite(); };
            StartCoroutine(InputReady());
        }
        private IEnumerator InputReady()
        {
            yield return null; yield return null; yield return Capture("input-initial.png");
            report.inputReady = true; Write();
        }
        private void Write()
        {
            var view = host.View; report.screen = view.ScreenName; report.focus = view.FocusedControl;
            report.publicationHeld = host.PublicationHeld; report.hasCampaign = view.HasCampaign; report.recordOpen = view.RecordOpen;
            report.current = Frame.Read(view.Current); report.workCurrent = WorkFrame.Read(view.CurrentWork); report.previewOpen = view.PreviewOpen; report.captures = captures.ToArray(); report.inputKeys = inputKeys.ToArray();
            report.layouts = layouts.ToArray(); report.worldLayouts = worldLayouts.ToArray(); report.width = Screen.width; report.height = Screen.height;
            report.keyboardEvents = view.KeyboardEvents; report.pointerEvents = view.PointerEvents; report.interceptedInputEvents = view.InterceptedInputEvents;
            report.lastResultSuccess = view.LastResult != null && view.LastResult.Success;
            report.lastResultDurable = view.LastResult != null && view.LastResult.Durable; report.lastResultCode = view.LastResult?.Code;
            report.targets = new[] { "NewCampaign", "ContinueCampaign", "BeginWorld", "CancelGeneration", "WaitOne",
                "WaitSixty", "Save", "Reload", "SelfRecord", "BackMenu", "CloseRecord", "PreviewWeaving", "ConfirmWeaving", "BackPreview", "CancelWeaving" }
                .Select(name => new Target { name = name, boundsRect = view.ControlBounds(name) })
                .Where(target => target.boundsRect.width > 0 && target.boundsRect.height > 0).ToArray();
            File.WriteAllText(Path.Combine(output, "qa.json"), JsonUtility.ToJson(report, true));
        }
        private void QueueInputWrite()
        {
            if (!inputProbe || inputWriteQueued) return;
            inputWriteQueued = true; StartCoroutine(WriteInputAfterFrame());
        }
        private IEnumerator WriteInputAfterFrame()
        { yield return null; yield return new WaitForEndOfFrame(); inputWriteQueued = false; if (inputProbe) Write(); }
        private void Update()
        {
            if (!inputProbe) return;
            var view = host.View; var current = view.Current;
            var work = view.CurrentWork;
            var signature = view.ScreenName + "|" + host.PublicationHeld + "|" + view.RecordOpen + "|" + view.PreviewOpen + "|" + view.FocusedControl
                + "|" + work?.Status + "|" + work?.FiberOnHand + "|" + work?.FiberReserved + "|" + work?.CordOnHand
                + "|" + current?.ActorName + "|" + current?.Tick + "|" + view.LastResult?.Code + "|" + view.HasCampaign;
            if (signature != observedSignature) { observedSignature = signature; QueueInputWrite(); }
            if (Input.GetKeyDown(KeyCode.F10)) { inputProbe = false; StartCoroutine(FinishInput()); }
        }
        private IEnumerator FinishInput()
        {
            yield return Capture("input-final.png"); report.passed = inputKeys.Count > 0; Write();
            Debug.Log("S17_INPUT_PROBE_FINISHED"); UnityEngine.Application.Quit(0);
        }
        private static void Require(bool value, string code)
        { if (!value) throw new InvalidOperationException(code); }
        private static bool Same(Frame a, Frame b) => JsonUtility.ToJson(a) == JsonUtility.ToJson(b);
        private static bool SameWork(WorkFrame a, WorkFrame b) => JsonUtility.ToJson(a) == JsonUtility.ToJson(b);
        private IEnumerator WaitFor(Func<bool> ready, string code)
        {
            var deadline = Time.realtimeSinceStartup + 30;
            while (!ready() && Time.realtimeSinceStartup < deadline) yield return null;
            Require(ready(), code);
        }
        private IEnumerator Capture(string name)
        {
            yield return new WaitForEndOfFrame();
            var texture = ScreenCapture.CaptureScreenshotAsTexture();
            try
            {
                Require(texture != null && texture.width == Screen.width && texture.height == Screen.height, "CAPTURE_SIZE");
                File.WriteAllBytes(Path.Combine(output, name), texture.EncodeToPNG()); captures.Add(name);
            }
            finally { if (texture != null) Destroy(texture); }
        }
        private IEnumerator Resolution(int width, int height)
        {
            var mode = width >= 1920 ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
            Screen.SetResolution(width, height, mode); var stable = 0; var deadline = Time.realtimeSinceStartup + 15;
            while (stable < 12 && Time.realtimeSinceStartup < deadline)
            {
                stable = Screen.width == width && Screen.height == height && Screen.fullScreenMode == mode ? stable + 1 : 0;
                yield return null;
            }
            resolutionReady = stable >= 12; Require(resolutionReady, "RESOLUTION");
            yield return new WaitForEndOfFrame();
        }
        private IEnumerator RunGuarded()
        {
            var stack = new Stack<IEnumerator>(); stack.Push(Run());
            while (stack.Count > 0)
            {
                object current;
                try
                {
                    if (!stack.Peek().MoveNext()) { stack.Pop(); continue; }
                    current = stack.Peek().Current;
                }
                catch (Exception error)
                {
                    report.error = error.GetType().Name + ": " + error.Message;
                    report.passed = false; host.ReleasePublication(); Debug.LogException(error); Write();
                    UnityEngine.Application.Quit(1); yield break;
                }
                if (current is IEnumerator child) { stack.Push(child); continue; }
                yield return current;
            }
            Write(); Debug.Log("S17_AUTOPLAY_PASSED " + report.passed); UnityEngine.Application.Quit(report.passed ? 0 : 1);
        }
        private IEnumerator Run()
        {
            var view = host.View;
            yield return Resolution(1280, 720);
            report.menuValid = view.ScreenName == "Menu" && !view.HasCampaign;
            Require(report.menuValid, "FRESH_MENU"); yield return Capture("capture-1280-menu.png");
            host.HoldNextPublication(); Require(view.BeginNewCampaign(), "BEGIN_CANCEL_CASE");
            report.duplicateNewBlocked = !view.BeginNewCampaign(); Require(report.duplicateNewBlocked, "DUPLICATE_NEW");
            yield return WaitFor(() => host.PublicationHeld, "PUBLICATION_BARRIER");
            view.CancelGeneration(); yield return WaitFor(() => view.ScreenName == "Menu", "CANCEL_COMPLETE");
            report.cancelBeforePublishValid = !view.HasCampaign && view.LastResult != null && !view.LastResult.Success && view.LastResult.Code == "cancelled";
            Require(report.cancelBeforePublishValid, "CANCEL_BEFORE_PUBLISH"); yield return Capture("capture-1280-cancelled.png");
            Require(view.BeginNewCampaign(), "BEGIN_CAMPAIGN"); yield return WaitFor(() => view.ScreenName != "Generating", "CREATION_COMPLETE");
            report.initial = Frame.Read(view.Current);
            report.newCampaignValid = view.ScreenName == "Reveal" && view.LastResult.Success && view.LastResult.Durable &&
                report.initial != null && report.initial.tick == 0 && report.initial.historyCount > 0 && report.initial.skillCount > 0 && report.initial.claimCount > 0;
            Require(report.newCampaignValid, "NEW_REVEAL");
            report.layoutValid = report.scrollValid = report.previewPure = report.previewLayoutValid = report.worldLayoutValid = report.worldScrollValid = report.worldFocusValid = true;
            foreach (var resolution in new[] { new Vector2Int(1280, 720), new Vector2Int(1920, 1080) })
            {
                yield return Resolution(resolution.x, resolution.y);
                foreach (var scale in new[] { 1f, 1.25f, 1.5f })
                {
                    view.SetTextScale(scale); view.ScrollRecordToTop(); yield return null; yield return null;
                    var sample = new Layout { width = Screen.width, height = Screen.height, scale = scale,
                        fits = view.RevealLayoutFits, header = view.HeaderBounds, card = view.CardBounds, footer = view.FooterBounds,
                        scroll = view.ScrollBounds, begin = view.BeginBounds, scrollable = view.BiographyScrollable };
                    yield return Capture("capture-" + resolution.x + "-reveal-" + (scale * 100).ToString("0") + "-top.png");
                    view.ScrollRecordToBottom(); yield return null; yield return null;
                    sample.bottomVisible = view.BiographyBottomVisible; layouts.Add(sample);
                    report.layoutValid &= sample.fits; report.scrollValid &= sample.bottomVisible;
                    yield return Capture("capture-" + resolution.x + "-reveal-" + (scale * 100).ToString("0") + "-bottom.png");
                }
                // Exercise both panels at this drawable before moving to fullscreen.
                // Returning from fullscreen to a decorated window can produce 683px under this WM.
                view.SetTextScale(1); view.BeginWorld(); yield return null; yield return null;
                foreach (var scale in new[] { 1f, 1.25f, 1.5f })
                {
                    view.SetTextScale(scale); view.ScrollWorldToTop(); yield return null; yield return null;
                    var sample = new WorldLayout { width = Screen.width, height = Screen.height, scale = scale,
                        fits = view.WorldLayoutFits, scrollable = view.WorldScrollable,
                        panel = view.WorldPanelBounds, viewport = view.WorldViewportBounds, controlsReachable = true };
                    yield return Capture("capture-" + resolution.x + "-world-layout-" + (scale * 100).ToString("0") + "-top.png");
                    foreach (var control in new[] { "PreviewWeaving", "WaitOne", "WaitSixty", "Save", "Reload", "SelfRecord", "BackMenu" })
                    {
                        if (!view.FocusWorldControl(control))
                        {
                            Require(control == "PreviewWeaving" && !view.CurrentWork.CanStart, "WORLD_EXPECTED_CONTROL");
                            continue; // Only the nonweaver craft button is intentionally disabled here.
                        }
                        yield return null; yield return null;
                        sample.controlsReachable &= view.ControlBounds(control).width > 0 && view.FocusedControl == control;
                    }
                    view.ScrollWorldToBottom(); yield return null; yield return null;
                    sample.bottomVisible = view.WorldBottomVisible;
                    report.worldLayoutValid &= sample.fits; report.worldScrollValid &= sample.bottomVisible; report.worldFocusValid &= sample.controlsReachable;
                    worldLayouts.Add(sample);
                    yield return Capture("capture-" + resolution.x + "-world-layout-" + (scale * 100).ToString("0") + "-bottom.png");
                }
                view.SetTextScale(1); view.ScrollWorldToTop(); yield return null; yield return null;
                if (view.CurrentWork.Qualified)
                {
                    var before = WorkFrame.Read(view.CurrentWork);
                    Require(view.OpenPreview(), "LAYOUT_PREVIEW_OPEN"); yield return null; yield return null;
                    report.previewPure &= SameWork(before, WorkFrame.Read(view.CurrentWork));
                    foreach (var scale in new[] { 1f, 1.25f, 1.5f })
                    {
                        view.SetTextScale(scale); yield return null; yield return null;
                        report.previewLayoutValid &= view.PreviewLayoutFits;
                        yield return Capture("capture-" + resolution.x + "-preview-" + (scale * 100).ToString("0") + ".png");
                    }
                    view.ClosePreview();
                }
                view.SetTextScale(1); view.ShowMenu(); view.ContinueCampaign(); yield return null; yield return null;
                Require(view.ScreenName == "Reveal", "LAYOUT_RETURN_REVEAL");
            }
            Require(worldLayouts.Count == 6 && report.worldLayoutValid && report.worldScrollValid && report.worldFocusValid, "WORLD_LAYOUT_SCROLL_FOCUS");
            Require(layouts.Count == 6 && report.layoutValid && report.scrollValid && layouts.Any(x => x.scrollable), "REVEAL_LAYOUT_SCROLL");
            view.SetTextScale(1); view.BeginWorld(); yield return null; yield return null;
            report.beginNoMutation = Same(report.initial, Frame.Read(view.Current)); Require(report.beginNoMutation, "BEGIN_PURE");
            yield return Capture("capture-1920-world-initial.png");
            view.OpenRecord(); yield return null; yield return null;
            report.modalFocusContained = view.RecordFocusContained;
            report.modalBlocksActions = view.Advance(1) == null && view.Save() == null && view.Reload() == null && view.ProbeModalBackgroundSubmit();
            report.selfRecordPure = Same(report.initial, Frame.Read(view.Current));
            Require(report.modalFocusContained && report.modalBlocksActions && report.selfRecordPure, "SELF_RECORD_MODAL");
            yield return Capture("capture-1920-self-record.png"); view.CloseRecord();
            report.workInitial = WorkFrame.Read(view.CurrentWork);
            report.qualifiedScenario = report.workInitial.qualified;
            Require(report.workInitial.fiberOnHand == 8 && report.workInitial.fiberReserved == 0 && report.workInitial.cordOnHand == 0, "STARTER_PERSONAL_STOCK");
            if (report.qualifiedScenario)
            {
                var beforePreview = WorkFrame.Read(view.CurrentWork);
                Require(view.OpenPreview(), "PREVIEW_OPEN"); yield return null; yield return null;
                report.previewPure &= SameWork(beforePreview, WorkFrame.Read(view.CurrentWork));
                report.previewLayoutValid &= view.PreviewLayoutFits;
                report.modalFocusContained &= view.RecordFocusContained;
                report.modalBlocksActions &= view.Advance(1) == null && view.Save() == null && view.Reload() == null
                    && view.CancelWork() == null && view.ProbeModalBackgroundSubmit();
                Require(report.previewPure && report.previewLayoutValid && report.modalFocusContained && report.modalBlocksActions, "PREVIEW_PURE_LAYOUT_MODAL");
                view.SetTextScale(1);
                report.startValid = view.ConfirmPreview().Success;
                report.workRunning = WorkFrame.Read(view.CurrentWork);
                report.startValid &= report.workRunning.status == "running" && report.workRunning.fiberOnHand == 8
                    && report.workRunning.fiberReserved == 2 && report.workRunning.cordOnHand == 0 && report.workRunning.dueTick == 30;
                Require(report.startValid, "START_RESERVATION"); yield return Capture("capture-1920-running.png");
                report.saveRunningValid = view.Save().Success && view.LastResult.Durable && view.Reload().Success
                    && SameWork(report.workRunning, WorkFrame.Read(view.CurrentWork));
                Require(report.saveRunningValid, "SAVE_RUNNING_RELOAD");
                view.ShowMenu(); view.ContinueCampaign(); yield return null; yield return null;
                var originAtStart = Frame.Read(view.Current);
                report.reopenRunning = view.ScreenName == "Reveal" && Same(report.initial, originAtStart)
                    && SameWork(report.workRunning, WorkFrame.Read(view.CurrentWork));
                Require(report.reopenRunning, "CONTINUE_RUNNING"); view.BeginWorld(); yield return null; yield return null;
                yield return Capture("capture-1920-continued-running.png");
                Require(view.Advance(29).Success, "WAIT_BEFORE_DUE"); report.workBeforeDue = WorkFrame.Read(view.CurrentWork);
                report.beforeDueValid = report.workBeforeDue.status == "running" && report.workBeforeDue.tick == 29
                    && report.workBeforeDue.fiberOnHand == 8 && report.workBeforeDue.fiberReserved == 2 && report.workBeforeDue.cordOnHand == 0;
                Require(report.beforeDueValid, "NO_EARLY_OUTPUT"); yield return Capture("capture-1920-before-due.png");
                Require(view.Advance(1).Success, "WAIT_DUE"); report.workCompleted = WorkFrame.Read(view.CurrentWork);
                report.completionValid = report.workCompleted.status == "completed" && report.workCompleted.tick == 30
                    && report.workCompleted.fiberOnHand == 6 && report.workCompleted.fiberReserved == 0 && report.workCompleted.cordOnHand == 1;
                Require(report.completionValid, "COMPLETE_CONSERVATION"); yield return Capture("capture-1920-completed.png");
                Require(view.OpenPreview() && view.ConfirmPreview().Success, "START_CANCEL_CASE");
                Require(view.CancelWork().Success, "CANCEL_WORK"); report.workCancelled = WorkFrame.Read(view.CurrentWork);
                report.cancelValid = report.workCancelled.status == "cancelled" && report.workCancelled.tick == 30
                    && report.workCancelled.fiberOnHand == 6 && report.workCancelled.fiberReserved == 0 && report.workCancelled.cordOnHand == 1;
                Require(report.cancelValid, "CANCEL_NO_CONSUME"); yield return Capture("capture-1920-cancelled-work.png");
            }
            else
            {
                var before = WorkFrame.Read(view.CurrentWork);
                report.nonWeaverDenied = !view.OpenPreview() && !view.PreviewOpen && view.ConfirmPreview() == null
                    && SameWork(before, WorkFrame.Read(view.CurrentWork)) && !view.CurrentWork.CanStart;
                Require(report.nonWeaverDenied, "NON_WEAVER_NOT_GRANTED_SKILL");
                yield return Capture("capture-1920-non-weaver-denied.png");
            }
            var beforeWait = view.Current.Tick;
            report.waitOneValid = view.Advance(1).Success && view.Current.Tick == beforeWait + 1;
            report.waitSixtyValid = view.Advance(60).Success && view.Current.Tick == beforeWait + 61;
            report.saveValid = view.Save().Success && view.LastResult.Durable; report.afterWait = Frame.Read(view.Current);
            var finalWork = WorkFrame.Read(view.CurrentWork);
            report.reloadValid = view.Reload().Success && Same(report.afterWait, Frame.Read(view.Current))
                && SameWork(finalWork, WorkFrame.Read(view.CurrentWork));
            Require(report.waitOneValid && report.waitSixtyValid && report.saveValid && report.reloadValid, "WAIT_SAVE_RELOAD");
            yield return Capture("capture-1920-world-waited.png");
            view.ShowMenu(); view.ContinueCampaign(); yield return null; yield return null;
            report.afterContinue = Frame.Read(view.Current); report.workAfterContinue = WorkFrame.Read(view.CurrentWork);
            report.continueSameOrigin = view.ScreenName == "Reveal" && Same(report.afterWait, report.afterContinue)
                && SameWork(finalWork, report.workAfterContinue);
            Require(report.continueSameOrigin, "CONTINUE_ORIGIN_WORK"); yield return Capture("capture-1920-continued-reveal.png");
            view.BeginWorld(); yield return null; yield return Capture("capture-1920-continued-world.png");
            report.delayedStateStable = view.Reload().Success && Same(report.afterWait, Frame.Read(view.Current))
                && SameWork(finalWork, WorkFrame.Read(view.CurrentWork));
            report.renderedCaptures = captures.Count == (report.qualifiedScenario ? 42 : 32); report.autoplayInputIsolated = view.AutoplayInputIsolated;
            report.noDeliveredExternalInput = view.KeyboardEvents == 0 && view.PointerEvents == 0;
            report.passed = report.menuValid && report.cancelBeforePublishValid && report.duplicateNewBlocked && report.newCampaignValid &&
                report.beginNoMutation && report.waitOneValid && report.waitSixtyValid && report.saveValid && report.reloadValid &&
                report.continueSameOrigin && report.delayedStateStable && report.selfRecordPure && report.modalFocusContained &&
                report.modalBlocksActions && report.layoutValid && report.scrollValid && report.renderedCaptures &&
                report.autoplayInputIsolated && report.noDeliveredExternalInput && report.worldLayoutValid && report.worldScrollValid && report.worldFocusValid
                && (report.qualifiedScenario ? report.previewPure && report.previewLayoutValid && report.startValid && report.saveRunningValid
                    && report.reopenRunning && report.beforeDueValid && report.completionValid && report.cancelValid : report.nonWeaverDenied);
        }
    }
}
