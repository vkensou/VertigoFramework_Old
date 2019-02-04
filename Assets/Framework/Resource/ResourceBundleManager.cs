using UnityEngine;
using System.Collections.Generic;
using UniRx;

public class ResourceBundleManager
{
    public static ResourceBundleManager SharedInstance { get; private set; }

    bool isEditor;
    Dictionary<int, IResourceBundle> bundles = new Dictionary<int, IResourceBundle>();

    public ResourceBundleManager(bool isEditor)
    {
        SharedInstance = this;

#if UNITY_EDITOR
        this.isEditor = isEditor;
#else
        this.isEditor = false;
#endif
    }

    public System.IObservable<Unit> LoadResourceBundleConfig(string configName)
    {
        if (loadingBundles.Count != 0)
            return Observable.Throw<Unit>(new System.Exception("Current is loading config!!!"));

        List<KeyValuePair<string, int>> config;
        if (isEditor)
            config = EditorResourceConfigLoader.LoadResourceBundleConfig(configName);
        else
            config = RuntimeResourceBundleConfigLoader.LoadResourceBundleConfig(configName);

        foreach(var b in config)
        {
            LoadBundle(b.Key, b.Value);
        }

        if (loadingBundles.Count == 0)
        {
            return Observable.ReturnUnit();
        }

        BundlesLoadCompleteObservable = new Subject<Unit>();
        return BundlesLoadCompleteObservable;
    }

    Dictionary<int, IResourceBundle> loadingBundles = new Dictionary<int, IResourceBundle>();
    Subject<Unit> BundlesLoadCompleteObservable;

    private void LoadBundle(string bundleName, int prefix)
    {
        if (bundles.ContainsKey(prefix) || loadingBundles.ContainsKey(prefix))
            return;

        IResourceBundle bundle;
        if (isEditor)
            bundle = new EditorResourceBundle(prefix);
        else
            bundle = new ResourceBundle(prefix);

        bundle.LoadBundle(bundleName);
        if (bundle.IsReady)
            bundles.Add(prefix, bundle);
        else
            loadingBundles.Add(prefix, bundle);
    }

    Dictionary<int, IResourceBundle> finishedBuffer = new Dictionary<int, IResourceBundle>();
    public void Update()
    {
        finishedBuffer.Clear();
        foreach (var l in loadingBundles)
        {
            if (l.Value.LoadUpdate())
                finishedBuffer.Add(l.Key, l.Value);
        }

        if (finishedBuffer.Count > 0)
        {
            foreach (var l in finishedBuffer)
            {
                loadingBundles.Remove(l.Key);
                bundles.Add(l.Key, l.Value);
            }
            if (loadingBundles.Count == 0)
            {
                BundlesLoadCompleteObservable.OnNext(Unit.Default);
                BundlesLoadCompleteObservable.OnCompleted();
                BundlesLoadCompleteObservable = null;
            }
        }

        foreach (var b in bundles)
        {
            b.Value.Update();
        }
    }

    public System.IObservable<LoadAssetResult> LoadAsset(int id)
    {
        int prefix = id / 10000 * 10000;
        int rid = id - prefix;
        IResourceBundle bundle;
        if (bundles.TryGetValue(prefix, out bundle))
        {
            return bundle.LoadAsset(rid);
        }
        return Observable.Throw<LoadAssetResult>(new System.Exception("Don't have bundle!"));
    }

    public void UnloadBundles()
    {
        foreach(var bkv in bundles)
        {
            bkv.Value.Unload();
        }
    }
}
