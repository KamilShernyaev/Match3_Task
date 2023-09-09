using System.Collections.Generic;
using UnityEngine;

public interface IGameFactory : IService
{
    List<ISavedProgressReader> ProgressReaders{get;}
    List<ISavedProgress> ProgressWriters{get;}

    GameObject CreateRow(Vector3 at, GameObject parent);
    GameObject CreateTile(Vector3 at, GameObject parent);
    GameObject CreateBoard(GameObject parent, LevelStaticData levelData);
    GameObject CreateAudioSource();
}