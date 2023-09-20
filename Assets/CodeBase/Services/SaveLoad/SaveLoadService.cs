using UnityEngine;

public class SaveLoadService : ISaveLoadService
{
    private const string ProgressKey = "Progress";
    private readonly IPersistentProgressService _progressService;
    private readonly IGameFactory _gameFactory;

    public SaveLoadService(IPersistentProgressService progressService, IGameFactory gameFactory)
    {
        _progressService = progressService;
        _gameFactory = gameFactory;
    }
    public PlayerProgress LoadProgress()
    {
        string json = PlayerPrefs.GetString(ProgressKey);
        PlayerProgress progress = JsonUtility.FromJson<PlayerProgress>(json);
        return progress;
    }

    public void SaveProgress()
    {
        foreach (ISavedProgress progressWriter in _gameFactory.ProgressWriters)
            progressWriter.UpdateProgress(_progressService.Progress);

        PlayerPrefs.SetString(ProgressKey, JsonUtility.ToJson(_progressService.Progress));
    }

    public void DeleteProgress()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}