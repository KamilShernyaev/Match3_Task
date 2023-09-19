using System;
using System.Collections.Generic;

public class GameStateMachine : IGameStateMachine
{
    private Dictionary<Type, IExitableState> _states;
    private IExitableState _activeState;

    public GameStateMachine(SceneLoader sceneLoader, LoadingCurtain curtain, AllServices services)
    {
        _states = new Dictionary<Type, IExitableState>
        {
            [typeof(BootstrapState)] = new BootstrapState(this, sceneLoader, services),
            [typeof(MainMenuState)] = new MainMenuState(sceneLoader, curtain, services.Single<IPersistentProgressService>(), services.Single<ISaveLoadService>(), services.Single<IGameFactory>(), services.Single<IAudioService>()),
            [typeof(LoadLevelState)] = new LoadLevelState(this, sceneLoader, curtain, services.Single<IGameFactory>(), services.Single<IStaticDataService>(), services.Single<IPersistentProgressService>(), services.Single<IAudioService>()),
            [typeof(GameLoopState)] = new GameLoopState(this),
        };
    }
    public void Enter<TState>() where TState : class, IState
    {
        IState state = ChangeState<TState>();
        state.Enter();
    }

    public void Enter<TState, TPayLoad>(TPayLoad payLoad) where TState : class, IPayloadedState<TPayLoad>
    {
        TState state = ChangeState<TState>();
        state.Enter(payLoad);
    }

    private TState ChangeState<TState>() where TState : class, IExitableState
    {
        _activeState?.Exit();
        
        TState state = GetState<TState>();
        _activeState = state;

        return state;
    }

     private TState GetState<TState>() where TState : class, IExitableState => 
        _states[typeof(TState)] as TState;
}