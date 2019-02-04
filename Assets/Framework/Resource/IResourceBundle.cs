public struct LoadAssetResult
{
    public int id;
    public UnityEngine.Object asset;

    public LoadAssetResult(int id, UnityEngine.Object asset)
    {
        this.id = id;
        this.asset = asset;
    }
}

public interface IResourceBundle
{
    bool IsReady { get; }

    bool LoadUpdate();

    void Unload();

    void LoadBundle(string bundlePath);

    void LoadBundle(string bundlePath, string commonPath);

    System.IObservable<LoadAssetResult> LoadAsset(int id);

    void Update();
}
