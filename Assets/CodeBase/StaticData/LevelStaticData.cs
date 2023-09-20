using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Static Data/Level")]

public class LevelStaticData : ScriptableObject
{
    public string LevelKey;
    public int TimeValue;
    public int TargetScore;
    public int Columns;
    public int Rows;
    public PrefabsData PrefabsData;
}
