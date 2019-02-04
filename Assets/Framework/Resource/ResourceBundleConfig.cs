using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Framework/Resource Bundle Config")]
public class ResourceBundleConfig : ScriptableObject {
    [System.Serializable]
    public struct BundleParam
    {
        public ResourceTable bundle;
        public int prefix;
    }
    public List<BundleParam> bundles = new List<BundleParam>();
}
