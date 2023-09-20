using UnityEngine;

public class MainMenuAudioSourceSound : MonoBehaviour
{
    private IAudioService _audioService;
    private AudioSource _audioSource;
    private void Awake() 
    {
        _audioService = AllServices.Container.Single<IAudioService>();
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayButtonSound()
    {
        _audioService.PlayOneShotSound(SoundType.UI_Button_Click_Sound, _audioSource);
    }
}
