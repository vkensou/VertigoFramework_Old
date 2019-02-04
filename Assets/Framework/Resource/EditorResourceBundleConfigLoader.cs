using UnityEngine;
using System.Collections.Generic;

public static class EditorResourceConfigLoader
{
    public static List<KeyValuePair<string, int>> LoadResourceBundleConfig(string configName)
    {
        var r = new List<KeyValuePair<string, int>>();
#if UNITY_EDITOR
        var configPath = string.Format("Assets/ResourceTables/{0}.asset", configName);
        var config = UnityEditor.AssetDatabase.LoadAssetAtPath<ResourceBundleConfig>(configPath);
        foreach(var b in config.bundles)
        {
            var bundlePath = UnityEditor.AssetDatabase.GetAssetPath(b.bundle);
            var bundleName = System.IO.Path.GetFileNameWithoutExtension(bundlePath);
            var p = new KeyValuePair<string, int>(bundleName, b.prefix);
            r.Add(p);
        }
#endif
        return r;
    }
}
