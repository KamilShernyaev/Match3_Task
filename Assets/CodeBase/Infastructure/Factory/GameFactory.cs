using UnityEngine;

public class GameFactory : IGameFactory
{
    private readonly IAssetProvider _assets;

    public GameFactory(IAssetProvider assets) => 
        _assets = assets;

    public GameObject CreateBoard(GameObject at) => 
         _assets.Instantiate(AssetPath.BoardPath, Vector3.zero);

         //TODO: Здесь в реализации ошибка исправь, объект тупо не создается, Vector3.Zero временный должен быть at.transform.position;
}
