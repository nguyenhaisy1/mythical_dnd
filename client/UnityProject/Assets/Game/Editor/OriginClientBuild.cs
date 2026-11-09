using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class OriginClientBuild
{
    public const string ScenePath = "Assets/Game/Scenes/Sprint16Origin.unity";
    private static void Guard(Action action)
    {
        try { action(); AssetDatabase.SaveAssets(); EditorApplication.Exit(0); }
        catch (Exception error) { Debug.LogException(error); EditorApplication.Exit(1); }
    }
    private static string Argument(string key)
    {
        var args = Environment.GetCommandLineArgs(); var index = Array.IndexOf(args, key);
        if (index < 0 || index + 1 >= args.Length) throw new ArgumentException("Missing " + key);
        return args[index + 1];
    }
    public static void PrepareScene() => Guard(() => {
        if (Application.unityVersion != "6000.6.7f1") throw new InvalidOperationException("Exact Editor6000.6.7f1 required");
        if (!File.Exists("Assets/Game/Resources/Sprint16/origin-cohort-v1.json"))
            throw new InvalidOperationException("Missing generated cohort resource");
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var type = AppDomain.CurrentDomain.GetAssemblies()
            .Select(a => a.GetType("Game.Client.Sprint16.OriginClientBootstrap", false)).FirstOrDefault(t => t != null);
        if (type == null) throw new InvalidOperationException("Origin bootstrap not compiled");
        new GameObject("OriginClient").AddComponent(type);
        Directory.CreateDirectory(Path.GetDirectoryName(ScenePath));
        if (!EditorSceneManager.SaveScene(scene, ScenePath)) throw new InvalidOperationException("Scene save failed");
        // The explicit BuildPlayer scene list below preserves Sprint15 EditorBuildSettings/scene.
    });
    public static void Build() => Guard(() => {
        if (Application.unityVersion != "6000.6.7f1") throw new InvalidOperationException("Exact Editor6000.6.7f1 required");
        PlayerSettings.SetApiCompatibilityLevel(NamedBuildTarget.Standalone, ApiCompatibilityLevel.NET_Standard);
        PlayerSettings.SetScriptingBackend(NamedBuildTarget.Standalone, ScriptingImplementation.Mono2x);
        var output = Path.GetFullPath(Argument("-s16BuildPath"));
        if (File.Exists(output)) throw new IOException("Build output already exists");
        Directory.CreateDirectory(Path.GetDirectoryName(output));
        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions { scenes = new[] { ScenePath },
            locationPathName = output, target = BuildTarget.StandaloneLinux64, options = BuildOptions.None });
        Debug.Log("S16_BUILD_RESULT " + report.summary.result + " errors=" + report.summary.totalErrors + " warnings=" + report.summary.totalWarnings);
        if (report.summary.result != BuildResult.Succeeded || !File.Exists(output)) throw new InvalidOperationException("Actual Player build failed");
    });
}
