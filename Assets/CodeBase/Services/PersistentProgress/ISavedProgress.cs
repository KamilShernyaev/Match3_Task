public interface ISavedProgress : ISavedProgressReader
{
    void UpdateProgress(PlayerProgress progress);
}

public interface ISavedProgressReader : IService
{
    void LoadProgress(PlayerProgress progress);
}
