using System.Collections.Generic;
using UnityEngine;

public class AudioService : IAudioService
{
    private readonly IAssetProvider _assetProvider;
    private Dictionary<SoundType, AudioClip> cachedAudio;

    public AudioService(IAssetProvider assetProvider)
    {
        _assetProvider = assetProvider;
        RegisterAudio();
    }
    
    private void RegisterAudio()
    {
        cachedAudio = new Dictionary<SoundType, AudioClip>();
        foreach (AudioItem audioItem in _assetProvider.LoadAudioContainer(ConstantsAssetPath.AudioPath).AudioItems)
        {
            cachedAudio.Add(audioItem.SoundType, audioItem.Sound);
        }
    }

    public void PlayLoopSound(SoundType soundType, AudioSource audioSource)
    {
        if (cachedAudio.ContainsKey(soundType))
        {
            AudioClip audioClip = cachedAudio[soundType];
            if (audioSource == null)
            {
                Debug.LogError("No AudioSource found in the scene!");
                return;
            }
            audioSource.clip = audioClip;
            audioSource.loop = true;
            audioSource.Play();
        }
        else
        {
            Debug.LogError("SoundType " + soundType + " не найден в кэше аудио.");
        }
    }

    public void PlayOneShotSound(SoundType soundType, AudioSource audioSource)
    {
        if (cachedAudio.ContainsKey(soundType))
        {
            AudioClip audioClip = cachedAudio[soundType];
            audioSource.PlayOneShot(audioClip);
        }
        else
        {
            Debug.LogError("SoundType " + soundType + " не найден в кэше аудио.");
        }
    }
}