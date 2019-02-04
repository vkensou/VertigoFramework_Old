using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(menuName = "Framework/Resource Unit")]
public class ResourceTable : ScriptableObject {
    public List<string> names = new List<string>();
    public List<Object> objects = new List<Object>();

    public Object LoadAsset(string name)
    {
#if UNITY_EDITOR
        for(int i = 0; i < names.Count; ++i)
        {
            if (name == names[i])
                return objects[i];
        }
#endif
        return null;
    }

    public T LoadAsset<T>(string name) where T : Object
    {
        return LoadAsset(name) as T;
    }
}
