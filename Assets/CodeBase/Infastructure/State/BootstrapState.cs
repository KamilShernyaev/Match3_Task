/// <summary>
/// Класс BootstrapState представляет состояние инициализации игры.
/// </summary>
/// <remarks>
/// В этом состоянии выполняются необходимые действия при запуске игры,
/// такие как регистрация сервисов и загрузка начальной сцены игры.
/// После загрузки начальной сцены, игровой автомат переходит в состояние MainMenuState.
/// </remarks>
public class BootstrapState : IState
{
    private const string Initial = "Initial";
    private readonly GameStateMachine _stateMachine;
    private readonly SceneLoader _sceneLoader;
    private readonly AllServices _services;

    public BootstrapState(
        GameStateMachine stateMachine, 
        SceneLoader sceneLoader, 
        AllServices services)
    {
        _stateMachine = stateMachine;
        _sceneLoader = sceneLoader;
        _services = services;

        RegisterServices();
    }

    public void Enter() => 
        _sceneLoader.Load(Initial, onLoaded: EnterLoadLevel);

    public void Exit()
    {
    }

    private void EnterLoadLevel() => 
        _stateMachine.Enter<MainMenuState>();

    private void RegisterServices()
    {
        _services.RegisterSingle<IGameStateMachine>(_stateMachine);
        _services.RegisterSingle<IAssetProvider>(new AssetProvider());
        _services.RegisterSingle<IPersistentProgressService>(new PersistentProgressService());
        _services.RegisterSingle<IGameFactory>(new GameFactory(_services.Single<IAssetProvider>()));
        _services.RegisterSingle<ISaveLoadService>(new SaveLoadService(_services.Single<IPersistentProgressService>(), _services.Single<IGameFactory>()));
        _services.RegisterSingle<IAudioService>(new AudioService(_services.Single<IAssetProvider>()));
        _services.RegisterSingle<ITimerService>(new TimerService());
        _services.RegisterSingle<IUIFactory>(new UIFactory(_services.Single<IAssetProvider>()));
        RegisterStaticData();
    }

    private void RegisterStaticData()
    {
        IStaticDataService staticData = new StaticDataService();
        staticData.Load();
        _services.RegisterSingle(staticData);
    }
}
