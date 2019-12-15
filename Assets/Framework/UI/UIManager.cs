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

public class UIManager : AssetManager<UIManager, GameObject>
{
    private GameObject uiRootObj;
    private class UIItem
    {
        public int id;
        public UIPanel panel;
        public bool recycled;
        public bool released;
    }
    private Dictionary<int, UIItem> singleteonUIInstance = new Dictionary<int, UIItem>();

    public void SetUIRootObj(GameObject uiRootObj)
    {
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
        else if (assets.ContainsKey(id))
        {
            var obj = Object.Instantiate(assets[id], uiRootObj.transform);
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
}
