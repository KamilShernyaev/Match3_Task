using System.Collections.Generic;
using UnityEngine;

public interface IGameFactory : IService
{
    List<ISavedProgressReader> ProgressReaders{get;}
    List<ISavedProgress> ProgressWriters{get;}

    GameObject CreateBoard(Transform parent);
    GameObject CreateTimerStarter();
    GameObject CreateAudioSource();
    GameObject CreateResultManager();
}