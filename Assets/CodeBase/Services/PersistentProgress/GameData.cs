using System;
using UnityEngine.SocialPlatforms.Impl;

[Serializable]
public class GameData
{
    public int Score;
    public string Level;

    public GameData(string initialLevel, int score)
    {
        Level = initialLevel;
        Score = score;
    }
}