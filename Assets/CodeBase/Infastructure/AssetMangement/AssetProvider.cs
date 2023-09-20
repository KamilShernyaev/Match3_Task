using UnityEngine;

public class AssetProvider : IAssetProvider
{
    public GameObject Instantiate(string path,Vector3 at, Transform parent)
    {
        var prefab = Resources.Load<GameObject>(path);
        return Object.Instantiate(prefab, at, Quaternion.identity,parent);
    }

    public GameObject Instantiate(string path, Transform parent)
    {
        var prefab = Resources.Load<GameObject>(path);
        return Object.Instantiate(prefab, parent);
    }
    
    public GameObject Instantiate(string path)
    {
        var prefab = Resources.Load<GameObject>(path);
        return Object.Instantiate(prefab);
    }

    


    public AudioContainer LoadAudioContainer(string path)
    {
        var audioContainer = Resources.Load<AudioContainer>(path);
        return audioContainer;
    }
}
