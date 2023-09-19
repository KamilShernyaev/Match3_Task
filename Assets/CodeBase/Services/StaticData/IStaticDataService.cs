
public interface IStaticDataService : IService
{
    void Load();
    LevelStaticData ForLevel(string sceneKey);
}
