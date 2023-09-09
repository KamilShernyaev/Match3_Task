using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameFactory : IGameFactory
{
    private readonly IAssetProvider _assets;
    private GameObject _rowGameObject;
    private GameObject _tileGameObject;
    private GameObject _audioSource;
    public List<ISavedProgressReader> ProgressReaders{get; } = new List<ISavedProgressReader>();
    public List<ISavedProgress> ProgressWriters{get; } = new List<ISavedProgress>();

    public GameFactory(IAssetProvider assets) => 
        _assets = assets;

    public GameObject CreateRow(Vector3 at, GameObject parent) => 
        _rowGameObject = _assets.Instantiate(AssetPath.RowPath, at, parent.transform);

    public GameObject CreateTile(Vector3 at, GameObject parent) => 
        _tileGameObject =_assets.Instantiate(AssetPath.TilePath, at, parent.transform);

    public GameObject CreateBoard(GameObject parent, LevelStaticData levelData)
    {
        GameObject boardGameObject = _assets.Instantiate(AssetPath.BoardPath, parent.transform.position, parent.transform);
        Board board = boardGameObject.GetComponent<Board>();

        for (int i = 0; i < levelData.width; i++)
        {
            CreateRow(boardGameObject.transform.position, boardGameObject);
            Row row = _rowGameObject.GetComponent<Row>();
            board.Construct(row, CreateAudioSource().GetComponent<AudioSource>());
            for (int j = 0; j < levelData.height; j++)
            {
                CreateTile(row.transform.position, row.gameObject);
                Tile tile = _tileGameObject.GetComponent<Tile>();
                row.Construct(tile);
            }
        }
        board.Init();
        RegisterProgressWatchers(boardGameObject);
        return boardGameObject;
    }

    public GameObject CreateAudioSource() => 
        _audioSource =_assets.Instantiate(AssetPath.AudioSourcePath);

    private void RegisterProgressWatchers(GameObject gameObject)
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
