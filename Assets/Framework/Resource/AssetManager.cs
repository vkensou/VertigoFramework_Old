using UnityEngine.Assertions;
using UniRx;
using System.Collections.Generic;

public class AssetManager<ManagerType, AssetType> where ManagerType : class where AssetType : UnityEngine.Object
{
    public static ManagerType SharedInstance { get; private set; }

    public AssetManager()
    {
        Assert.IsNull(SharedInstance);
        SharedInstance = this as ManagerType;
    }

    protected Dictionary<int, AssetType> assets = new Dictionary<int, AssetType>();
    Dictionary<int, System.IObservable<LoadAssetResult>> loadingList = new Dictionary<int, System.IObservable<LoadAssetResult>>();

    public System.IObservable<LoadAssetResult> LoadAsset(int id)
    {
        if (assets.ContainsKey(id))
            return Observable.Return(new LoadAssetResult(id, assets[id]));
        else if (loadingList.ContainsKey(id))
            return loadingList[id];

        var obb = ResourceBundleManager.SharedInstance.LoadAsset(id);
        loadingList.Add(id, obb);
        obb.Subscribe(result =>
        {
            if (result.asset is AssetType)
            {
                var asset = result.asset as AssetType;
                loadingList.Remove(id);
                assets.Add(id, asset);
            }
        });
        return obb;
    }

    public System.IObservable<IList<LoadAssetResult>> LoadAssets(params int[] ids)
    {
        List<System.IObservable<LoadAssetResult>> obs = new List<System.IObservable<LoadAssetResult>>();

        foreach (int id in ids)
        {
            var obb = LoadAsset(id);
            obs.Add(obb);
        }

        return Observable.Zip(obs);
    }

    public System.IObservable<IList<LoadAssetResult>> LoadRange(int from, int to)
    {
        List<System.IObservable<LoadAssetResult>> obs = new List<System.IObservable<LoadAssetResult>>();

        for (int id = from; id <= to; ++id)
        {
            var obb = LoadAsset(id);
            obs.Add(obb);
        }

        return Observable.Zip(obs);
    }

    public System.IObservable<Unit> GetLoadAssetSignal(int id)
    {
        return LoadAsset(id).Select(_ => Unit.Default);
    }

    public System.IObservable<Unit> GetLoadAssetsSignal(params int[] ids)
    {
        return LoadAssets(ids).Select(_ => Unit.Default);
    }

    public System.IObservable<Unit> GetLoadRangeSignal(int from, int to)
    {
        return LoadRange(from, to).Select(_ => Unit.Default);
    }

    public AssetType GetAsset(int id)
    {
        AssetType asset = null;
        assets.TryGetValue(id, out asset);
        return asset;
    }
}
