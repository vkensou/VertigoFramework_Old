using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;
using UniRx;

public class SoundManager
{
    public static SoundManager SharedInstance { get; private set; }

    public SoundManager(AudioSource source)
    {
        Assert.IsNull(SharedInstance);
        SharedInstance = this;

        this.source = source;
    }

    private Dictionary<int, AudioClip> clips = new Dictionary<int, AudioClip>();
    private AudioSource source;

    public System.IObservable<LoadAssetResult> LoadSound(int id)
    {
        var obb = ResourceBundleManager.SharedInstance.LoadAsset(id);
        obb.Subscribe(result =>
        {
            if (result.asset is AudioClip && !clips.ContainsKey(id))
                clips.Add(id, result.asset as AudioClip);
        });
        return obb;
    }

    public AudioClip GetAudioClip(int id)
    {
        if (clips.ContainsKey(id))
            return clips[id];
        else
            return null;
    }

    public void PlaySound(int id)
    {
        if (clips.ContainsKey(id))
            source.PlayOneShot(clips[id]);
    }
}
