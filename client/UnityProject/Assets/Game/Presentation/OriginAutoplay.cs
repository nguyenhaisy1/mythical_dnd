using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Game.Application.Origins;
using UnityEngine;

namespace Game.Client.Sprint16
{
    /// <summary>Actual renderer/callback automation and separate OS-input telemetry, never a human playtest.</summary>
    public sealed class OriginAutoplay : MonoBehaviour
    {
        private OriginClientBootstrap host;
        private string output;
        private Report report;
        private readonly List<string> captures = new List<string>(), inputKeys = new List<string>();
        private readonly List<Layout> layouts = new List<Layout>();
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
        [Serializable] private sealed class Layout
        {
            public int width, height; public float scale;
            public bool fits, bottomVisible, scrollable;
            public Rect header, card, footer, scroll, begin;
        }
        [Serializable] private sealed class Target { public string name; public Rect boundsRect; }
        [Serializable] private sealed class Report
        {
            public string profile = "origin-cohort-v1", scope = "actual Unity LinuxMono callback/render automation; not human playtest";
            public string unityVersion, platform, backend, error = "", screen, focus, lastResultCode;
            public bool passed, inputReady, publicationHeld, hasCampaign, recordOpen, lastResultSuccess, lastResultDurable;
            public bool menuValid, cancelBeforePublishValid, duplicateNewBlocked, newCampaignValid,
                beginNoMutation, waitOneValid, waitSixtyValid, saveValid, reloadValid, continueSameOrigin,
                delayedStateStable, selfRecordPure, modalFocusContained, modalBlocksActions,
                layoutValid, scrollValid, renderedCaptures, autoplayInputIsolated, noDeliveredExternalInput;
            public int width, height, keyboardEvents, pointerEvents, interceptedInputEvents;
            public Frame initial, afterWait, afterContinue, current;
            public string[] captures, inputKeys;
            public Layout[] layouts;
            public Target[] targets;
        }
        private void Setup(OriginClientBootstrap bootstrap, string directory)
        {
            host = bootstrap; output = Path.GetFullPath(directory);
            if (Directory.Exists(output) && Directory.GetFileSystemEntries(output).Length != 0)
                throw new IOException("QA output must be new or empty");
            Directory.CreateDirectory(output);
            report = new Report { unityVersion = UnityEngine.Application.unityVersion,
                platform = UnityEngine.Application.platform.ToString(),
                backend = Type.GetType("Mono.Runtime") != null ? "Mono" : "Unknown" };
        }
        public void Begin(OriginClientBootstrap bootstrap, string directory)
        { Setup(bootstrap, directory); host.View.IsolateAutoplayInput(); StartCoroutine(RunGuarded()); }
        public void BeginInputProbe(OriginClientBootstrap bootstrap, string directory)
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
            report.current = Frame.Read(view.Current); report.captures = captures.ToArray(); report.inputKeys = inputKeys.ToArray();
            report.layouts = layouts.ToArray(); report.width = Screen.width; report.height = Screen.height;
            report.keyboardEvents = view.KeyboardEvents; report.pointerEvents = view.PointerEvents; report.interceptedInputEvents = view.InterceptedInputEvents;
            report.lastResultSuccess = view.LastResult != null && view.LastResult.Success;
            report.lastResultDurable = view.LastResult != null && view.LastResult.Durable; report.lastResultCode = view.LastResult?.Code;
            report.targets = new[] { "NewCampaign", "ContinueCampaign", "BeginWorld", "CancelGeneration", "WaitOne",
                "WaitSixty", "Save", "Reload", "SelfRecord", "BackMenu", "CloseRecord" }
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
            var signature = view.ScreenName + "|" + host.PublicationHeld + "|" + view.RecordOpen + "|" + view.FocusedControl
                + "|" + current?.ActorName + "|" + current?.Tick + "|" + view.LastResult?.Code + "|" + view.HasCampaign;
            if (signature != observedSignature) { observedSignature = signature; QueueInputWrite(); }
            if (Input.GetKeyDown(KeyCode.F10)) { inputProbe = false; StartCoroutine(FinishInput()); }
        }
        private IEnumerator FinishInput()
        {
            yield return Capture("input-final.png"); report.passed = inputKeys.Count > 0; Write();
            Debug.Log("S16_INPUT_PROBE_FINISHED"); UnityEngine.Application.Quit(0);
        }
        private static void Require(bool value, string code)
        { if (!value) throw new InvalidOperationException(code); }
        private static bool Same(Frame a, Frame b) => JsonUtility.ToJson(a) == JsonUtility.ToJson(b);
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
            Write(); Debug.Log("S16_AUTOPLAY_PASSED " + report.passed); UnityEngine.Application.Quit(report.passed ? 0 : 1);
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
            report.layoutValid = report.scrollValid = true;
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
            }
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
            report.waitOneValid = view.Advance(1).Success && view.Current.Tick == 1;
            report.waitSixtyValid = view.Advance(60).Success && view.Current.Tick == 61;
            report.saveValid = view.Save().Success && view.LastResult.Durable; report.afterWait = Frame.Read(view.Current);
            report.reloadValid = view.Reload().Success && Same(report.afterWait, Frame.Read(view.Current));
            Require(report.waitOneValid && report.waitSixtyValid && report.saveValid && report.reloadValid, "WAIT_SAVE_RELOAD");
            yield return Capture("capture-1920-world-waited.png");
            view.ShowMenu(); view.ContinueCampaign(); yield return null; yield return null;
            report.afterContinue = Frame.Read(view.Current);
            report.continueSameOrigin = view.ScreenName == "Reveal" && Same(report.afterWait, report.afterContinue);
            Require(report.continueSameOrigin, "CONTINUE_ORIGIN"); yield return Capture("capture-1920-continued-reveal.png");
            view.BeginWorld(); yield return null; yield return Capture("capture-1920-continued-world.png");
            report.delayedStateStable = view.Reload().Success && Same(report.afterWait, Frame.Read(view.Current));
            report.renderedCaptures = captures.Count >= 19; report.autoplayInputIsolated = view.AutoplayInputIsolated;
            report.noDeliveredExternalInput = view.KeyboardEvents == 0 && view.PointerEvents == 0;
            report.passed = report.menuValid && report.cancelBeforePublishValid && report.duplicateNewBlocked && report.newCampaignValid &&
                report.beginNoMutation && report.waitOneValid && report.waitSixtyValid && report.saveValid && report.reloadValid &&
                report.continueSameOrigin && report.delayedStateStable && report.selfRecordPure && report.modalFocusContained &&
                report.modalBlocksActions && report.layoutValid && report.scrollValid && report.renderedCaptures &&
                report.autoplayInputIsolated && report.noDeliveredExternalInput;
        }
    }
}
