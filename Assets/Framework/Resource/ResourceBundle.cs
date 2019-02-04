using UnityEngine;
using System.Collections.Generic;
using UniRx;

public class ResourceBundle : IResourceBundle
{
    AssetBundle bundle;
    AssetBundleCreateRequest bundleRequest;
    public int Prefix { get; }

    public ResourceBundle(int prefix)
    {
        Prefix = prefix;
    }

    public void LoadBundle(string bundlePath)
    {
        LoadBundle(bundlePath, string.Empty);
    }

    public void LoadBundle(string bundlePath, string commonPath)
    {
        bundleRequest = AssetBundle.LoadFromFileAsync(Application.streamingAssetsPath + "/" + bundlePath + ".ab");
    }

    public bool IsReady
    {
        get
        {
            if (bundle != null)
                return true;
            else if (bundleRequest != null)
                return bundleRequest.isDone;
            else
                return false;
        }
    }

    public bool LoadUpdate()
    {
        if (bundleRequest != null && bundleRequest.isDone)
        {
            bundle = bundleRequest.assetBundle;
            return true;
        }
        return false;
    }

    public void Unload()
    {
        if (bundle != null)
            bundle.Unload(true);
    }

    class LoadTask
    {
        public int id;
        public AssetBundleRequest request;
        public ISubject<LoadAssetResult> loadCompleteObservable;
        public void InvokeCallback(int prefix)
        {
            loadCompleteObservable.OnNext(new LoadAssetResult(prefix + id, request.asset));
            loadCompleteObservable.OnCompleted();
        }
    }

    Dictionary<int, LoadTask> loadTasks = new Dictionary<int, LoadTask>();

    public System.IObservable<LoadAssetResult> LoadAsset(int id)
    {
        if (loadTasks.ContainsKey(id))
        {
            var t = loadTasks[id];
            return t.loadCompleteObservable;
        }
        bool con = bundle.Contains(id.ToString());
        var request = bundle.LoadAssetAsync(id.ToString());
        var task = new LoadTask()
        {
            id = id,
            request = request
        };
        loadTasks.Add(id, task);
        task.loadCompleteObservable = new Subject<LoadAssetResult>();
        return task.loadCompleteObservable;
    }

    List<int> finishedBuffer = new List<int>();
    public void Update()
    {
        finishedBuffer.Clear();
        foreach (var t in loadTasks)
        {
            if (t.Value.request.isDone)
            {
                t.Value.InvokeCallback(Prefix);
                finishedBuffer.Add(t.Key);
            }
        }

        foreach (var id in finishedBuffer)
            loadTasks.Remove(id);
    }
}
