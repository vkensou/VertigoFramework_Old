using UnityEngine;
using System.Collections;
using System;
using UniRx;

public class EditorResourceBundle : IResourceBundle
{
    ResourceTable table;
    public int Prefix { get; }

    public bool IsReady
    {
        get
        {
            return true;
        }
    }

    public EditorResourceBundle(int prefix)
    {
        Prefix = prefix;
    }

    public void Unload()
    {
    }

    public void LoadBundle(string bundlePath)
    {
        LoadBundle(bundlePath, string.Empty);
    }

    public void LoadBundle(string bundlePath, string commonPath)
    {
#if UNITY_EDITOR
        table = UnityEditor.AssetDatabase.LoadAssetAtPath<ResourceTable>("Assets/ResourceTables/" + bundlePath + ".asset");
#endif
    }

    public bool LoadUpdate()
    {
        return true;
    }

    public IObservable<LoadAssetResult> LoadAsset(int id)
    {
        var obj = table.LoadAsset(id.ToString());
        return Observable.Return(new LoadAssetResult(Prefix + id, obj));
    }

    public void Update()
    {
    }
}
