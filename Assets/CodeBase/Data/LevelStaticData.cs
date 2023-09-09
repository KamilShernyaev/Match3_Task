using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Static Data/Level")]

public class LevelStaticData : ScriptableObject
{
    public string LevelKey;
    public Transform InitialPoint;
    public int width;
    public int height;
}