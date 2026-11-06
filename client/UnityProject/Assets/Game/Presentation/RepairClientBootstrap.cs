using System;
using System.Collections.Generic;
using System.IO;
using Game.Application.Projection;
using Game.Infrastructure.Repair;
using UnityEngine;

namespace Game.Client.Sprint15
{
    /// <summary>The only client composition point allowed to bind infrastructure and trusted resources.</summary>
    public sealed class RepairClientBootstrap : MonoBehaviour
    {
        private RepairContentRegistry registry;
        private string saveRoot;
        public IRepairClientSession Session { get; private set; }
        public RepairClientView View { get; private set; }
        public RepairSceneView Scene { get; private set; }

        public static string Argument(string key)
        {
            var args = Environment.GetCommandLineArgs(); var index = Array.IndexOf(args, key);
            if (index < 0) return null;
            if (index + 1 >= args.Length || args[index + 1].StartsWith("-", StringComparison.Ordinal))
                throw new ArgumentException("Missing value for " + key);
            return args[index + 1];
        }
        private void Start()
        {
            try
            {
                var source = Resources.Load<TextAsset>("Sprint15/boat-repair-v1");
                if (source == null) throw new InvalidOperationException("Missing trusted repair resource");
                var authored = JsonUtility.FromJson<Authoring>(source.text);
                if (authored == null || authored.recipe == null) throw new InvalidOperationException("Invalid trusted repair resource");
                registry = RepairContentRegistry.Compile(authored.ToDictionary());
                saveRoot = Path.GetFullPath(Argument("-s15SaveRoot") ?? Path.Combine(UnityEngine.Application.persistentDataPath, RepairDemoFactory.ProfileId));
                Directory.CreateDirectory(saveRoot);
                Session = RepairDemoFactory.Open(Path.Combine(saveRoot, "world.db"), registry);
                Scene = gameObject.AddComponent<RepairSceneView>(); Scene.Initialize();
                View = gameObject.AddComponent<RepairClientView>(); View.Initialize(Session, Scene);
                var autoplay = Argument("-s15Autoplay"); var probe = Argument("-s15InputProbe");
                if (autoplay != null || probe != null)
                {
                    var qa = gameObject.AddComponent<RepairAutoplay>();
                    if (autoplay != null) qa.Begin(this, autoplay);
                    else qa.BeginInputProbe(this, probe);
                }
                Debug.Log("S15_CLIENT_READY profile=repair-client-2d-v1 fixedActor=true");
            }
            catch (Exception error)
            {
                Debug.LogException(error);
                var failed = gameObject.AddComponent<RepairClientView>();
                failed.ShowFatal("Không mở được bản thử. Dữ liệu lưu được giữ nguyên; hãy kiểm tra nhật ký kỹ thuật.");
                if (Argument("-s15Autoplay") != null || Argument("-s15InputProbe") != null) UnityEngine.Application.Quit(1);
            }
        }
        // Only QA composition uses a second fixed world. The normal UI has no reset/reroll/path control.
        public void SwitchQaBranch(string branch)
        {
            if (Argument("-s15Autoplay") == null || (branch != "cancel" && branch != "wait"))
                throw new InvalidOperationException("QA branch unavailable");
            var path = Path.Combine(saveRoot, "qa-" + branch + ".db");
            if (File.Exists(path)) throw new IOException("QA branch must be new");
            Session = RepairDemoFactory.Open(path, registry); View.Bind(Session);
        }
        public void ReopenCurrentQaWorld()
        {
            if (Argument("-s15Autoplay") == null) throw new InvalidOperationException("QA reopen unavailable");
            Session = RepairDemoFactory.Open(Path.Combine(saveRoot, "world.db"), registry); View.Bind(Session);
        }
        [Serializable] private sealed class Authoring
        {
            public long schema_version; public string id, profile, status; public Recipe recipe;
            public Dictionary<string, object> ToDictionary() => new Dictionary<string, object> {
                ["schema_version"] = schema_version, ["id"] = id, ["profile"] = profile, ["status"] = status,
                ["recipe"] = new Dictionary<string, object> { ["id"] = recipe.id, ["version"] = recipe.version,
                    ["handler"] = recipe.handler, ["material"] = recipe.material, ["material_quantity"] = recipe.material_quantity,
                    ["duration_ticks"] = recipe.duration_ticks, ["condition_gain"] = recipe.condition_gain, ["condition_max"] = recipe.condition_max } };
        }
        [Serializable] private sealed class Recipe
        {
            public string id, handler, material; public long version, material_quantity, duration_ticks, condition_gain, condition_max;
        }
    }
}
