using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevelState : IPayloadedState<string>
{
  private const string InitialPointTag = "InitialPoint";

  private readonly GameStateMachine _stateMachine;
  private readonly SceneLoader _sceneLoader;
  private readonly LoadingCurtain _loadingCurtain;
  private readonly IGameFactory _gameFactory;
  private readonly IStaticDataService _staticData;
  private readonly IPersistentProgressService _progressService;
  private readonly IAudioService _audioService;
  private readonly ITimerService _timerService;
  private readonly IUIFactory _uIFactory;

    public LoadLevelState(
      GameStateMachine gameStateMachine, 
      SceneLoader sceneLoader, 
      LoadingCurtain loadingCurtain, 
      IGameFactory gameFactory, 
      IStaticDataService staticDataService,
      IPersistentProgressService progressService,
      IAudioService audioService,
      ITimerService timerService,
      IUIFactory uIFactory)
  {
    _stateMachine = gameStateMachine;
    _sceneLoader = sceneLoader;
    _loadingCurtain = loadingCurtain;
    _gameFactory = gameFactory;
    _staticData = staticDataService;
    _progressService = progressService;
    _audioService = audioService;
    _timerService = timerService;
    _uIFactory = uIFactory;
  }

  public void Enter(string sceneName)
  {
    _sceneLoader.Load(sceneName, OnLoaded);
  }

  public void Exit() =>
    _loadingCurtain.Hide();

  private void OnLoaded()
  {
      InitGameWorld();
      InformProgressReaders();
      _stateMachine.Enter<GameLoopState>();
  } 

  private void InformProgressReaders()
  {
      foreach (ISavedProgressReader progressReader in _gameFactory.ProgressReaders)
      {
          progressReader.LoadProgress(_progressService.Progress);
      }
  }
  
  private void InitGameWorld()
    {
        LevelStaticData levelData = LevelStaticData();
        _audioService.PlayLoopSound(SoundType.Main_Theme_Music, CreateAudioSourceForMusic().GetComponent<AudioSource>());
        AudioSource tempAudioSourceForSound = CreateAudioSourceForSound();
        ShapesManager tempShape = CreateBoard();
        tempShape.Construct(GameObject.FindWithTag(InitialPointTag).transform.position, _audioService, _progressService, tempAudioSourceForSound, levelData);
        tempShape.Init();
        CreateTimerStarter().Construct(_timerService, levelData);
        CreateResultManager().Construct(_progressService.Progress.WorldData, levelData, _timerService, _uIFactory);
    }

    private ShapesManager CreateBoard() => 
      _gameFactory.CreateBoard(GameObject.FindWithTag(InitialPointTag).transform).GetComponent<ShapesManager>();

    private LevelStaticData LevelStaticData() => 
      _staticData.ForLevel(SceneManager.GetActiveScene().name);

    private AudioSource CreateAudioSourceForSound() => 
      _gameFactory.CreateAudioSource().GetComponent<AudioSource>();

    private AudioSource CreateAudioSourceForMusic() => 
      _gameFactory.CreateAudioSource().GetComponent<AudioSource>();

    private TimerStarter CreateTimerStarter() => 
      _gameFactory.CreateTimerStarter().GetComponent<TimerStarter>();

    private ResultManager CreateResultManager() => 
      _gameFactory.CreateResultManager().GetComponent<ResultManager>();
}
