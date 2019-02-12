using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UniRx;

public class UIPanel
{
    public int id;
    public GameObject obj;

    public UIPanel(int id, GameObject obj)
    {
        this.id = id;
        this.obj = obj;
    }
}

public class UIManager
{
    public static UIManager SharedInstance { get; private set; }

    private GameObject uiRootObj;
    private Dictionary<int, GameObject> uiPrefabs = new Dictionary<int, GameObject>();
    private class UIItem
    {
        public int id;
        public UIPanel panel;
        public bool recycled;
        public bool released;
    }
    private Dictionary<int, UIItem> singleteonUIInstance = new Dictionary<int, UIItem>();

    public UIManager(GameObject uiRootObj)
    {
        Assert.IsNull(SharedInstance);
        SharedInstance = this;

        this.uiRootObj = uiRootObj;
    }

    public UIPanel OpenSingletonPanel(int id, bool recycled = false)
    {
        if (singleteonUIInstance.ContainsKey(id))
        {
            var item = singleteonUIInstance[id];
            if (item.released)
            {
                item.released = false;
                item.panel.obj.SetActive(true);
            }
            return item.panel;
        }
        else if (uiPrefabs.ContainsKey(id))
        {
            var obj = Object.Instantiate(uiPrefabs[id], uiRootObj.transform);
            var panel = new UIPanel(id, obj);
            var item = new UIItem() { id = id, panel = panel, recycled = recycled };
            singleteonUIInstance.Add(id, item);
            return panel;
        }
        return null;
    }

    public void CloseSingletonPanel(UIPanel panel)
    {
        if (singleteonUIInstance.ContainsKey(panel.id))
        {
            var item = singleteonUIInstance[panel.id];
            if (item.recycled)
            {
                item.panel.obj.SetActive(false);
                item.released = true;
            }
            else
            {
                Object.Destroy(panel.obj);
                singleteonUIInstance.Remove(panel.id);
            }
        }
    }

    public System.IObservable<LoadAssetResult> LoadUIPrefab(int id)
    {
        var obb = ResourceBundleManager.SharedInstance.LoadAsset(id);
        obb.Subscribe(result =>
        {
            if (result.asset is GameObject && !uiPrefabs.ContainsKey(result.id))
                uiPrefabs.Add(result.id, result.asset as GameObject);
        });
        return obb;
    }
}
