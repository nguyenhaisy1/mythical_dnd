using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using Game.Infrastructure.Conformance;
public static class ConformanceEntry
{
    [Serializable] private sealed class Report
    {
        public string status, profile, fingerprint, sqliteVersion, unityVersion, backend, platform, api;
        public string[] checks;
    }
    private static string Arg(string name)
    {
        var args = Environment.GetCommandLineArgs();
        int i = Array.IndexOf(args, name);
        if (i < 0 || i + 1 == args.Length) throw new ArgumentException(name);
        return args[i + 1];
    }
    public static void Run()
    {
        try
        {
            if (Application.platform != RuntimePlatform.LinuxEditor || IntPtr.Size != 8 || Type.GetType("Mono.Runtime") == null)
                throw new Exception("Only actual Linux x64 Editor Mono is supported by this candidate profile");
            var api = PlayerSettings.GetApiCompatibilityLevel(NamedBuildTarget.Standalone);
            if (api != ApiCompatibilityLevel.NET_Standard) throw new Exception("API profile mismatch");
            var result = RuntimeConformance.Run(Arg("-conformanceFixture"), Arg("-conformanceDatabase"));
            var report = new Report { status = "Passed", profile = RuntimeConformance.Profile, checks = result.Checks.ToArray(),
                fingerprint = result.Fingerprint, sqliteVersion = result.SqliteVersion, unityVersion = Application.unityVersion,
                backend = "Editor-Mono", platform = Application.platform.ToString(), api = api.ToString() };
            using (var output = new StreamWriter(new FileStream(Arg("-conformanceOutput"), FileMode.CreateNew))) output.Write(JsonUtility.ToJson(report));
            EditorApplication.Exit(0);
        }
        catch (Exception e) { Debug.LogException(e); EditorApplication.Exit(1); }
    }
}
