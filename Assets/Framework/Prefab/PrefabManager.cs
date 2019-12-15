using UnityEngine;
using System.Collections.Generic;
using UniRx;
using UnityEngine.Assertions;

public class PrefabManager : AssetManager<PrefabManager, GameObject>
{
    public System.IObservable<Unit> LoadPrefab(int id)
    {
        return GetLoadAssetSignal(id);
    }

    public System.IObservable<Unit> LoadPrefabs(params int[] ids)
    {
        return GetLoadAssetsSignal(ids);
    }

    public GameObject GetPrefab(int id)
    {
        return GetAsset(id);
    }
}
