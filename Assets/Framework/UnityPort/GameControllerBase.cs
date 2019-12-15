using UnityEngine;
using UniRx;

public abstract class GameControllerBase : MonoBehaviour
{
    public bool isEditor;
    public string bundleConfigName;

    IProcedureManager m_procedureManager;
    UIManager m_uiManager;
    SoundManager m_soundManager;
    ResourceBundleManager m_bundleManager;
    bool couldUpdate = false;

    void Start()
    {
        m_bundleManager = new ResourceBundleManager(isEditor);

        new TextureManager();
        new MaterialManager();
        new PrefabManager();

        var uiRootObj = GameObject.Find("UIRoot");
        DontDestroyOnLoad(uiRootObj);
        m_uiManager = new UIManager();
        m_uiManager.SetUIRootObj(uiRootObj);

        var audioSourceObj = GameObject.Find("SoundSource").GetComponent<AudioSource>();
        m_soundManager = new SoundManager();
        m_soundManager.SetSoundSource(audioSourceObj);

        m_procedureManager = InitialProcedureManager();

        m_bundleManager.LoadResourceBundleConfig(bundleConfigName)
            .Subscribe(_ => { couldUpdate = true; m_procedureManager.Start(); });
    }

    void Update()
    {
        m_bundleManager.Update();
        if (couldUpdate)
            m_procedureManager.Update();
    }

    protected abstract IProcedureManager InitialProcedureManager();
}