public interface ISaveLoadService : IService
{
    void SaveProgress();
    void DeleteProgress();
    PlayerProgress LoadProgress();
}