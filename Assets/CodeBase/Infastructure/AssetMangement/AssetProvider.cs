using UnityEngine;

public class AssetProvider : IAssetProvider
{
    /// <summary>
    /// Методы для создания экземпляра игрового объекта из префаба с заданным путем.
    /// </summary>
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

    /// <summary>
    /// Метод для загрузки контейнера аудио из ресурсов с заданным путем.
    /// </summary>
    /// <param name="path">Путь к контейнеру аудио</param>
    /// <returns>Загруженный контейнер аудио</returns>
    public AudioContainer LoadAudioContainer(string path)
    {
        var audioContainer = Resources.Load<AudioContainer>(path);
        return audioContainer;
    }
}
