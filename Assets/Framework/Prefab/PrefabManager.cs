using UnityEngine;
using System.Collections.Generic;
using UniRx;
using UnityEngine.Assertions;

public class PrefabManager
{
    public static PrefabManager SharedInstance { get; private set; }

    public PrefabManager()
    {
        Assert.IsNull(SharedInstance);
        SharedInstance = this;
    }

    Dictionary<int, GameObject> loadedPrefabs = new Dictionary<int, GameObject>();
    Dictionary<int, System.IObservable<Unit>> loadingPrefabs = new Dictionary<int, System.IObservable<Unit>>();

    public System.IObservable<Unit> LoadPrefab(int id)
    {
        var o = ResourceBundleManager.SharedInstance.LoadAsset(id).Take(1);
        o.Subscribe(r => { loadingPrefabs.Remove(id); loadedPrefabs.Add(id, r.asset as GameObject); });
        var or = o.Select(_ => Unit.Default);
        loadingPrefabs.Add(id, or);
        return or;
    }

    public System.IObservable<Unit> LoadPrefabs(params int[] ids)
    {
        System.IObservable<Unit>[] os = new System.IObservable<Unit>[ids.Length];
        for (int i = 0; i < ids.Length; ++i)
        {
            int id = ids[i];
            if (loadedPrefabs.ContainsKey(id))
            {
                os[i] = Observable.ReturnUnit();
            }
            else if (loadingPrefabs.ContainsKey(id))
            {
                os[i] = loadingPrefabs[id];
            }
            else
                os[i] = LoadPrefab(id);
        }

        return Observable.Zip(os).Take(1).Select(_ => Unit.Default);
    }

    public GameObject GetPrefab(int id)
    {
        if (loadedPrefabs.ContainsKey(id))
        {
            return loadedPrefabs[id];
        }
        return null;
    }
}
