using UnityEngine;

public class MainMenuState : IState
{
    private const string MainMenu = "MainMenu";
    private readonly SceneLoader _sceneLoader;
    private readonly LoadingCurtain _loadingCurtain;
    private readonly IPersistentProgressService _progressService;
    private readonly ISaveLoadService _saveLoadService;
    private readonly IGameFactory _gameFactory;
    private readonly IAudioService _audioService;

    public MainMenuState(
        SceneLoader sceneLoader, 
        LoadingCurtain loadingCurtain, 
        IPersistentProgressService progressService, 
        ISaveLoadService saveLoadService, 
        IGameFactory gameFactory, 
        IAudioService audioService)
    {
        _sceneLoader = sceneLoader;
        _loadingCurtain = loadingCurtain;
        _progressService = progressService;
        _saveLoadService = saveLoadService;
        _gameFactory = gameFactory;
        _audioService = audioService;
    }

    public void Enter() => 
        _sceneLoader.Load(MainMenu, onLoaded: OnLoaded);

    private void OnLoaded()
    {
        _loadingCurtain.Hide(); 
        _audioService.PlayLoopSound(SoundType.Main_Theme_Music, CreateAudioSource().GetComponent<AudioSource>());
        LoadProgressOrInitNew();
    }

    public void Exit() => 
        _loadingCurtain.Show();

    private void LoadProgressOrInitNew()
    {
        _progressService.Progress = 
            _saveLoadService.LoadProgress()
                ?? NewProgress();
    }

    private PlayerProgress NewProgress() => 
        new PlayerProgress();

    private GameObject CreateAudioSource() => 
        _gameFactory.CreateAudioSource();
}
