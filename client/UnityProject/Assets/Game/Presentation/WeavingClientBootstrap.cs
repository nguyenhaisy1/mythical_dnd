using System;
using System.IO;
using System.Threading;
using Game.Application.Origins;
using Game.Infrastructure.Origins;
using Game.Infrastructure.Weaving;
using UnityEngine;

namespace Game.Client.Sprint17
{
    /// <summary>Trusted composition only; presenters receive safe ports and never authoring records.</summary>
    public sealed class WeavingClientBootstrap : MonoBehaviour
    {
        private readonly ManualResetEventSlim publicationRelease = new ManualResetEventSlim(false);
        private volatile bool holdNextPublication, publicationHeld;
        public WeavingClientView View { get; private set; }
        public bool PublicationHeld => publicationHeld;
        public bool IsQa => Argument("-s17Autoplay") != null || Argument("-s17InputProbe") != null;

        public static string Argument(string key)
        {
            var args = Environment.GetCommandLineArgs(); var index = Array.IndexOf(args, key);
            if (index < 0) return null;
            if (index + 1 >= args.Length || args[index + 1].StartsWith("-", StringComparison.Ordinal))
                throw new ArgumentException("Missing argument value");
            return args[index + 1];
        }
        private void Start()
        {
            try
            {
                var resource = Resources.Load<TextAsset>("Sprint16/origin-cohort-v1");
                if (resource == null) throw new InvalidOperationException("Trusted cohort resource missing");
                var cohort = OriginCohortCompiler.Compile(JsonUtility.FromJson<OriginCohortDocument>(resource.text));
                var parameters = Resources.Load<TextAsset>("Sprint17/personal-weaving-v1")
                    ?? throw new InvalidOperationException("Trusted weaving parameters missing");
                var registry = WeavingContentRegistry.Compile(cohort, JsonUtility.FromJson<WeavingProfileDocument>(parameters.text));
                var root = Path.GetFullPath(Argument("-s17SaveRoot") ??
                    Path.Combine(UnityEngine.Application.persistentDataPath, "personal-weaving-v1"));
                var seed = Argument("-s17QaSeed");
                if (seed != null && !IsQa) throw new InvalidOperationException("Seed injection only available in QA");
                if (Argument("-s17QaHoldFirstPublication") != null)
                {
                    if (!IsQa) throw new InvalidOperationException("Publication barrier only available in QA");
                    holdNextPublication = true;
                }
                var gateway = new WeavingCampaignGateway(root, registry,
                    seed == null ? (Func<string>)null : () => seed, IsQa ? PublicationProbe : (Action<string>)null);
                BuildBackground();
                View = gameObject.AddComponent<WeavingClientView>(); View.Initialize(gateway);
                View.GenerationCancelled += ReleasePublication;
                var autoplay = Argument("-s17Autoplay"); var input = Argument("-s17InputProbe");
                if (autoplay != null || input != null)
                {
                    var qa = gameObject.AddComponent<WeavingAutoplay>();
                    if (autoplay != null) qa.Begin(this, autoplay); else qa.BeginInputProbe(this, input);
                }
                Debug.Log("S17_CLIENT_READY profile=personal-weaving-v1");
            }
            catch (Exception error)
            {
                Debug.LogException(error);
                var failed = gameObject.AddComponent<WeavingClientView>();
                failed.ShowFatal("Không mở được bản thử. Dữ liệu hiện có được giữ nguyên; hãy kiểm tra nhật ký kỹ thuật.");
                if (IsQa) UnityEngine.Application.Quit(1);
            }
        }
        private void BuildBackground()
        {
            var camera = new GameObject("OriginFixed2DCamera").AddComponent<Camera>();
            camera.orthographic = true; camera.orthographicSize = 4.5f;
            camera.transform.position = new Vector3(0, 0, -10); camera.nearClipPlane = .1f; camera.farClipPlane = 30;
            camera.clearFlags = CameraClearFlags.SolidColor; camera.backgroundColor = new Color(.18f, .25f, .21f);
            var sprite = Resources.Load<Sprite>("Sprint15/Art/base") ?? throw new InvalidOperationException("2D foundation missing");
            var renderer = new GameObject("BenVacFoundation").AddComponent<SpriteRenderer>(); renderer.sprite = sprite;
            renderer.transform.localScale = new Vector3(16 / sprite.bounds.size.x, 9 / sprite.bounds.size.y, 1);
        }
        private void PublicationProbe(string phase)
        {
            if (phase != "before_publish" || !holdNextPublication) return;
            holdNextPublication = false; publicationRelease.Reset(); publicationHeld = true;
            try { publicationRelease.Wait(); }
            finally { publicationHeld = false; }
        }
        public void HoldNextPublication()
        {
            if (!IsQa || publicationHeld) throw new InvalidOperationException("QA publication barrier unavailable");
            holdNextPublication = true;
        }
        public void ReleasePublication() { publicationRelease.Set(); }
        private void OnDestroy() { publicationRelease.Set(); }
    }
}
