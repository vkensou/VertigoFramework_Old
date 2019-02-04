using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.Assertions;

public class BuildBundleTestWindow : EditorWindow
{
    [SerializeField]
    ResourceBundleConfig config;
    [SerializeField]
    List<ResourceTable> resUnits;
    protected SerializedObject _serializedObject;
    protected SerializedProperty _assetLstProperty;
    protected SerializedProperty _configProperty;
    
    [MenuItem("Framework/Build Bundle")]
    static void showWindow()
    {
        EditorWindow.GetWindow(typeof(BuildBundleTestWindow));
    }

    private void OnEnable()
    {
        _serializedObject = new SerializedObject(this);
        _assetLstProperty = _serializedObject.FindProperty("resUnits");
        _configProperty = _serializedObject.FindProperty("config");
    }

    private void OnGUI()
    {
        _serializedObject.Update();

        //开始检查是否有修改
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.PropertyField(_configProperty);

        bool needBuild = false;
        if (GUILayout.Button("Build") && config != null && config.bundles.Count > 0)
        {
            needBuild = true;
        }

        //结束检查是否有修改
        if (EditorGUI.EndChangeCheck())
        {//提交修改
            _serializedObject.ApplyModifiedProperties();
        }

        if (needBuild)
        {
            BuildConfig(config);
            BuildBundles(config, BuildTarget.StandaloneWindows);
        }
    }

    private void BuildBundle(ResourceTable ru, BuildTarget target)
    {
        string path = AssetDatabase.GetAssetPath(ru);
        string filename = Path.GetFileNameWithoutExtension(path);
        string bundlePathFormat = "{0}.ab";
        string bundlePath = string.Format(bundlePathFormat, filename);

        AssetBundleBuild abb = new AssetBundleBuild();
        abb.assetBundleName = bundlePath;
        List<string> assets = new List<string>();
        foreach(var obj in ru.objects)
        {
            string resPath = AssetDatabase.GetAssetPath(obj);
            assets.Add(resPath);
        }
        abb.assetNames = assets.ToArray();

        BuildAssetBundleOptions opt = BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.ChunkBasedCompression;
        BuildPipeline.BuildAssetBundles("Assets/StreamingAssets", new AssetBundleBuild[] { abb }, opt, target);
    }

    private void BuildConfig(ResourceBundleConfig config)
    {
        var configPath = AssetDatabase.GetAssetPath(config);
        var configFilename = Path.GetFileNameWithoutExtension(configPath);
        var r = ResourceBundleConfigCompiler.Compile(config);

        if (false == Directory.Exists("Assets/StreamingAssets"))
        {
            //创建pic文件夹
            Directory.CreateDirectory("Assets/StreamingAssets");
        }

        string path = "Assets/StreamingAssets/" + configFilename.ToLower() + ".txt";
        StreamWriter s = new StreamWriter(path, false);
        s.Write(r);
        s.Close();
        s.Dispose();
    }

    private void BuildBundles(ResourceBundleConfig config, BuildTarget target)
    {
        List<HashSet<Object>> dss = new List<HashSet<Object>>(config.bundles.Count);
        List<Object> commonAssets = new List<Object>();

        for(int i = 0; i < config.bundles.Count; ++i)
        {
            var ru = config.bundles[i].bundle;
            //BuildBundle(ru, target);
            if (ru != null)
            {
                var dsets = GetUnitDependencies(ru);
                dss.Add(dsets);
            }
            else
                dss.Add(null);
        }

        {
            Dictionary<Object, int> allObjs = new Dictionary<Object, int>();
            foreach (var ds in dss)
            {
                foreach (var s in ds)
                {
                    if (allObjs.ContainsKey(s))
                    {
                        allObjs[s]++;
                    }
                    else
                    {
                        allObjs.Add(s, 1);
                    }
                }
            }

            foreach (var s in allObjs)
            {
                if (s.Value > 1)
                    commonAssets.Add(s.Key);
            }
        }

        var commonABB = GetAssetBundleBuild(commonAssets, "common");
        List<AssetBundleBuild> abbs = new List<AssetBundleBuild>();
        abbs.Add(commonABB);
        foreach(var ru in config.bundles)
        {
            var abb = GetAssetBundleBuild(ru.bundle, commonAssets);
            abbs.Add(abb);
        }

        BuildAssetBundleOptions opt = BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.ChunkBasedCompression;
        BuildPipeline.BuildAssetBundles("Assets/StreamingAssets", abbs.ToArray(), opt, target);
    }

    private HashSet<Object> GetUnitDependencies(ResourceTable ru)
    {
        var ds = EditorUtility.CollectDependencies(ru.objects.ToArray());
        HashSet<Object> sets = new HashSet<Object>();
        foreach(var d in ds)
        {
            if (!(d is Component) && !sets.Contains(d) && d != null)
                sets.Add(d);
        }
        return sets;
    }

    private AssetBundleBuild GetAssetBundleBuild(ResourceTable ru)
    {
        string path = AssetDatabase.GetAssetPath(ru);
        string filename = Path.GetFileNameWithoutExtension(path);
        return GetAssetBundleBuild(ru.objects, ru.names, filename);
    }

    private AssetBundleBuild GetAssetBundleBuild(ResourceTable ru, List<Object> commonAssets)
    {
        string path = AssetDatabase.GetAssetPath(ru);
        string filename = Path.GetFileNameWithoutExtension(path);
        List<Object> assets = ru.objects;
        List<string> names = ru.names;
        foreach(var a in commonAssets)
        {
            int id = assets.IndexOf(a);
            if (id != -1)
            {
                assets.RemoveAt(id);
                names.RemoveAt(id);
            }
        }
        return GetAssetBundleBuild(assets, names, filename);
    }

    private AssetBundleBuild GetAssetBundleBuild(List<Object> assets, string name)
    {
        return GetAssetBundleBuild(assets, null, name);
    }

    private AssetBundleBuild GetAssetBundleBuild(List<Object> assets, List<string> names, string name)
    {
        Assert.IsTrue(names == null || names.Count == assets.Count);

        string bundlePathFormat = "{0}.ab";
        string bundlePath = string.Format(bundlePathFormat, name);

        AssetBundleBuild abb = new AssetBundleBuild();
        abb.assetBundleName = bundlePath;
        List<string> paths = new List<string>();
        List<string> alias = new List<string>();
        for(int i = 0; i < assets.Count; ++i)
        {
            string resPath = AssetDatabase.GetAssetPath(assets[i]);
            paths.Add(resPath);
            if (names != null)
                alias.Add(names[i]);
        }

        abb.assetNames = paths.ToArray();
        if (names != null)
            abb.addressableNames = alias.ToArray();
        return abb;
    }
}
