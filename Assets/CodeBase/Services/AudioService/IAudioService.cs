using UnityEngine;

public interface IAudioService : IService
{
    void PlayOneShotSound(SoundType soundType, AudioSource audioSource);
    void PlayLoopSound(SoundType soundType, AudioSource audioSource);
}
