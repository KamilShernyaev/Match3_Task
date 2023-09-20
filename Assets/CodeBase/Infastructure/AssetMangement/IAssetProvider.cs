using UnityEngine;

public interface IAssetProvider: IService
{
    GameObject Instantiate(string path, Vector3 at, Transform parent);
    GameObject Instantiate(string path, Transform parent);
    GameObject Instantiate(string path);
    AudioContainer LoadAudioContainer(string path);
}