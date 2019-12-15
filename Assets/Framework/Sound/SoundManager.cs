using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;
using UniRx;

public class SoundManager : AssetManager<SoundManager, AudioClip>
{
    private AudioSource source;

    public AudioClip GetAudioClip(int id)
    {
        return GetAsset(id);
    }

    public void PlaySound(int id)
    {
        if (assets.ContainsKey(id))
            source.PlayOneShot(assets[id]);
    }

    public void SetSoundSource(AudioSource source)
    {
        this.source = source;
    }
}
