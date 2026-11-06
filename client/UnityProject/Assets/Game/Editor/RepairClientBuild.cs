using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>Executed by the real Editor; generated settings, scene and metadata are not hand-authored evidence.</summary>
public static class RepairClientBuild
{
    public const string ScenePath = "Assets/Game/Scenes/Sprint15Repair.unity";
    private static string Argument(string key)
    {
        var args = Environment.GetCommandLineArgs(); var index = Array.IndexOf(args, key);
        if (index < 0 || index + 1 >= args.Length) throw new ArgumentException("Missing " + key);
        return args[index + 1];
    }
    private static void Guard(Action action)
    {
        try { action(); AssetDatabase.SaveAssets(); EditorApplication.Exit(0); }
        catch (Exception error) { Debug.LogException(error); EditorApplication.Exit(1); }
    }
    public static void Configure() => Guard(() => {
        if (Application.unityVersion != "6000.6.7f1") throw new InvalidOperationException("Exact verified Editor6000.6.7f1 required");
        PlayerSettings.SetApiCompatibilityLevel(NamedBuildTarget.Standalone, ApiCompatibilityLevel.NET_Standard);
        PlayerSettings.SetScriptingBackend(NamedBuildTarget.Standalone, ScriptingImplementation.Mono2x);
        PlayerSettings.companyName = "MythicalDnD"; PlayerSettings.productName = "Hong Bang - Ben Vac Technical Proof";
        PlayerSettings.defaultScreenWidth = 1280; PlayerSettings.defaultScreenHeight = 720;
        PlayerSettings.fullScreenMode = FullScreenMode.Windowed; PlayerSettings.resizableWindow = true;
        PlayerSettings.runInBackground = true; PlayerSettings.colorSpace = ColorSpace.Gamma;
    });
    public static void PrepareScene() => Guard(() => {
        var artRoot = "Assets/Game/Resources/Sprint15/Art";
        foreach (var name in new[] { "base", "foreground", "boat-damaged", "boat-repaired", "actor-idle-0", "actor-idle-1" })
            if (!File.Exists(Path.Combine(artRoot, name + ".png"))) throw new InvalidOperationException("Missing separate runtime art: " + name);
        foreach (var file in Directory.GetFiles(artRoot, "*.png"))
        {
            var path = file.Replace('\\', '/'); var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null) throw new InvalidOperationException("Missing importer: " + path);
            importer.textureType = TextureImporterType.Sprite; importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaIsTransparency = true; importer.mipmapEnabled = false; importer.filterMode = FilterMode.Bilinear;
            importer.npotScale = TextureImporterNPOTScale.None; importer.maxTextureSize = 4096; importer.spritePixelsPerUnit = 120;
            var settings = new TextureImporterSettings(); importer.ReadTextureSettings(settings);
            settings.spriteAlignment = (int)SpriteAlignment.Custom;
            settings.spritePivot = Path.GetFileNameWithoutExtension(file) == "base" ? new Vector2(.5f, .5f) : VisibleFootPivot(file);
            importer.SetTextureSettings(settings); importer.SaveAndReimport();
        }
        foreach (var file in Directory.GetFiles("Assets/Plugins/MythicalRuntime", "*.so"))
        {
            var importer = AssetImporter.GetAtPath(file.Replace('\\', '/')) as PluginImporter;
            if (importer == null) throw new InvalidOperationException("Missing native importer");
            importer.SetCompatibleWithAnyPlatform(false); importer.SetCompatibleWithEditor(true);
            importer.SetEditorData("OS", "Linux"); importer.SetEditorData("CPU", "x86_64");
            importer.SetCompatibleWithPlatform(BuildTarget.StandaloneLinux64, true);
            importer.SetPlatformData(BuildTarget.StandaloneLinux64, "CPU", "x86_64"); importer.SaveAndReimport();
        }
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("Game.Client.Sprint15.RepairClientBootstrap", false)).FirstOrDefault(t => t != null);
        if (type == null) throw new InvalidOperationException("Runtime bootstrap not compiled");
        new GameObject("RepairClient").AddComponent(type);
        Directory.CreateDirectory(Path.GetDirectoryName(ScenePath));
        if (!EditorSceneManager.SaveScene(scene, ScenePath)) throw new InvalidOperationException("Scene save failed");
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
    });
    public static void Build() => Guard(() => {
        var output = Path.GetFullPath(Argument("-s15BuildPath"));
        if (File.Exists(output)) throw new IOException("Build output already exists");
        Directory.CreateDirectory(Path.GetDirectoryName(output));
        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions {
            scenes = new[] { ScenePath }, locationPathName = output,
            target = BuildTarget.StandaloneLinux64, options = BuildOptions.None });
        Debug.Log("S15_BUILD_RESULT " + report.summary.result + " errors=" + report.summary.totalErrors + " warnings=" + report.summary.totalWarnings);
        if (report.summary.result != BuildResult.Succeeded || !File.Exists(output)) throw new InvalidOperationException("Actual Player build failed");
    });
    private static Vector2 VisibleFootPivot(string file)
    {
        var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        try
        {
            if (!texture.LoadImage(File.ReadAllBytes(file))) throw new InvalidOperationException("Invalid PNG: " + file);
            var pixels = texture.GetPixels32(); var left = texture.width; var right = -1; var bottom = texture.height;
            for (var y = 0; y < texture.height; y++) for (var x = 0; x < texture.width; x++)
                if (pixels[y * texture.width + x].a >= 16) { left = Math.Min(left, x); right = Math.Max(right, x); bottom = Math.Min(bottom, y); }
            if (right < left) throw new InvalidOperationException("Empty runtime sprite: " + file);
            return new Vector2((left + right + 1) * .5f / texture.width, (bottom + .5f) / texture.height);
        }
        finally { UnityEngine.Object.DestroyImmediate(texture); }
    }
}
