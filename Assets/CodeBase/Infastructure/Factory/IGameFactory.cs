using UnityEngine;

public interface IGameFactory : IService
{
    GameObject CreateBoard(GameObject at);
}