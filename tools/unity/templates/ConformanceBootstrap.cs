using System;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
public static class ConformanceBootstrap
{
    public static void Configure()
    {
        try
        {
            if (!Application.unityVersion.StartsWith("6000.")) throw new Exception("Unity 6 Editor required for this candidate profile");
            PlayerSettings.SetApiCompatibilityLevel(NamedBuildTarget.Standalone, ApiCompatibilityLevel.NET_Standard);
            AssetDatabase.SaveAssets();
            EditorApplication.Exit(0);
        }
        catch (Exception e) { Debug.LogException(e); EditorApplication.Exit(1); }
    }
}
