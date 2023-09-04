using System;
using System.Collections.Generic;
using UnityEngine;

public class GameStateMachine
{
    private Dictionary<Type, IExitableState> _states;
    private IExitableState _activeState;

    public GameStateMachine(SceneLoader sceneLoader, LoadingCurtain curtain, AllServices services)
    {
        _states = new Dictionary<Type, IExitableState>
        {
            [typeof(BootstrapState)] = new BootstrapState(this, sceneLoader, services),
            [typeof(LoadLevelState)] = new LoadLevelState(this, sceneLoader, curtain, services.Single<IGameFactory>()),
            [typeof(GameLoopState)] = new GameLoopState(this),
        };
    }
    public void Enter<TState>() where TState : class, IState
    {
        IState state = ChangeState<TState>();
        state.Enter();
    }

    public void Enter<TState, TPayLoad>(TPayLoad payLoad) where TState : class, IPayLoadedState<TPayLoad>
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