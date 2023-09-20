using System.Collections.Generic;
using UnityEngine;

public class GameFactory : IGameFactory
{
    private readonly IAssetProvider _assets;
    public List<ISavedProgressReader> ProgressReaders{get; } = new List<ISavedProgressReader>();
    public List<ISavedProgress> ProgressWriters{get; } = new List<ISavedProgress>();

    public GameFactory(IAssetProvider assets) => 
        _assets = assets;

    public GameObject CreateBoard(Transform parent) => 
        _assets.Instantiate(ConstantsAssetPath.BoardPath, parent.transform.position, parent);
    
    public GameObject CreateAudioSource() => 
        _assets.Instantiate(ConstantsAssetPath.AudioSourcePath); 

    public GameObject CreateTimerStarter() =>
        _assets.Instantiate(ConstantsAssetPath.TimerStarterPath);

    public GameObject CreateResultManager() =>
        _assets.Instantiate(ConstantsAssetPath.ResultManagerPath);

    private void RegisterProgressReader(GameObject gameObject)
    {
        foreach (ISavedProgressReader progressReader in gameObject.GetComponentsInChildren<ISavedProgressReader>())
        {
            Register(progressReader);
        }
    }

    private void Register(ISavedProgressReader progressReader)
    {
        if(progressReader is ISavedProgress progressWriter)
        ProgressWriters.Add(progressWriter);

        ProgressReaders.Add(progressReader);
    }
}
