using UnityEngine;
using UnityEditor;

public static class ResourceBundleConfigCompiler
{
    public static string Compile(ResourceBundleConfig config)
    {
        string result = string.Empty;
        string rowFormat = "{0}\t{1}\n";
        foreach(var b in config.bundles)
        {
            var bundlePath = UnityEditor.AssetDatabase.GetAssetPath(b.bundle);
            var bundleName = System.IO.Path.GetFileNameWithoutExtension(bundlePath);

            result += string.Format(rowFormat, bundleName, b.prefix);
        }

        return result;
    }
}