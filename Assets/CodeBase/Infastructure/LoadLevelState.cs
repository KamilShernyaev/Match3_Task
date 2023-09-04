using System;
using UnityEngine;

public class LoadLevelState : IPayLoadedState<string>
{
    private const string InitialPointTag = "InitialPoint";
    
    private readonly GameStateMachine _stateMachine;
    private readonly SceneLoader _sceneLoader;
    private readonly LoadingCurtain _curtain;
    private readonly IGameFactory _gameFactory;

    public LoadLevelState(GameStateMachine stateMachine, SceneLoader sceneLoader, LoadingCurtain curtain, IGameFactory gameFactory)
    {
        _stateMachine = stateMachine;
        _sceneLoader = sceneLoader;
        _curtain = curtain;
        _gameFactory = gameFactory;
    }

    public void Enter(string sceneName)
    {
        _curtain.Show();
        _sceneLoader.Load(sceneName, onLoaded);
    }

    public void Exit() => 
        _curtain.Hide();

    private void onLoaded()
    {
        GameObject hero =_gameFactory.CreateBoard(GameObject.FindWithTag(InitialPointTag));
        _stateMachine.Enter<GameLoopState>();
    }
    
}