using UnityEngine;

/// <summary>
/// Класс GameBootstrapper представляет точку входа в игру.
/// </summary>
/// <remarks>
/// В методе Awake() создается экземпляр класса Game, инициализируется игровой автомат состояний,
/// запускается процесс инициализации игры с помощью состояния BootstrapState и сохранение объекта на сцене.
/// </remarks>
public partial class GameBootstrapper : MonoBehaviour, ICoroutineRunner
{
    public LoadingCurtain Curtain;
    private Game _game;

    private void Awake() 
    {
        _game = new Game(this, Curtain);
        _game.StateMachine.Enter<BootstrapState>();
        
        DontDestroyOnLoad(this);
    }
}