using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

public static class AssetBundleBuilder
{
    const string OutputFolder = "AssetBundles";
    const string Menu = "Assets/Build AssetBundles/";

    static readonly (string Name, BuildTarget Target) Windows = ("Windows", BuildTarget.StandaloneWindows64);
    static readonly (string Name, BuildTarget Target) Mac = ("Mac", BuildTarget.StandaloneOSX);
    static readonly (string Name, BuildTarget Target) Linux = ("Linux", BuildTarget.StandaloneLinux64);
    static readonly (string Name, BuildTarget Target) Android = ("Android", BuildTarget.Android);

    [MenuItem(Menu + "Windows", priority = 0)]
    public static void BuildWindows() => Build(Windows);

    [MenuItem(Menu + "Mac", priority = 1)]
    public static void BuildMac() => Build(Mac);

    [MenuItem(Menu + "Linux", priority = 2)]
    public static void BuildLinux() => Build(Linux);

    [MenuItem(Menu + "Android", priority = 3)]
    public static void BuildAndroid() => Build(Android);

    [MenuItem(Menu + "All Platforms", priority = 20)]
    public static void BuildAll() => Build(Windows, Mac, Linux, Android);

    static void Build(params (string Name, BuildTarget Target)[] platforms)
    {
        foreach (var (name, target) in platforms)
            BuildPlatform(name, target);
        string folder = platforms.Length == 1 ? Path.Combine(OutputFolder, platforms[0].Name) : OutputFolder;
        if (!Application.isBatchMode && Directory.Exists(folder))
            EditorUtility.OpenWithDefaultApp(Path.GetFullPath(folder));
    }

    static void BuildPlatform(string platform, BuildTarget target)
    {
        if (!BuildPipeline.IsBuildTargetSupported(BuildPipeline.GetBuildTargetGroup(target), target))
        {
            Debug.LogWarning($"Skipped {platform} asset bundles: add {platform} Build Support to this editor through Unity Hub.");
            return;
        }
        string outputPath = Path.Combine(OutputFolder, platform);
        if (Directory.Exists(outputPath))
            Directory.Delete(outputPath, true);
        Directory.CreateDirectory(outputPath);
        AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.None, target);
        if (manifest == null)
            throw new BuildFailedException($"Failed to build {platform} asset bundles.");
        DeleteNonBundleFiles(outputPath, manifest.GetAllAssetBundles());
        Debug.Log($"Built {platform} asset bundles ({string.Join(", ", manifest.GetAllAssetBundles())}) to {Path.GetFullPath(outputPath)}");
    }

    static void DeleteNonBundleFiles(string outputPath, string[] bundles)
    {
        var bundlePaths = new HashSet<string>(bundles.Select(bundle => Path.GetFullPath(Path.Combine(outputPath, bundle))));
        foreach (string file in Directory.GetFiles(outputPath, "*", SearchOption.AllDirectories))
        {
            if (!bundlePaths.Contains(Path.GetFullPath(file)))
                File.Delete(file);
        }
    }
}
