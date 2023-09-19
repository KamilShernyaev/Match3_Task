using TMPro;
using UnityEngine;

public class ScoreCounter : MonoBehaviour
{
    public TextMeshProUGUI Counter;
    private WorldData _worldData;

    public void Construct(WorldData worldData)
    {
        _worldData = worldData;
        _worldData.ScoreData.Changed += UpdateScore;

        UpdateScore();
    }

    private void UpdateScore()
    {
        Counter.text = $"{_worldData.ScoreData.Score}";
    }
}
