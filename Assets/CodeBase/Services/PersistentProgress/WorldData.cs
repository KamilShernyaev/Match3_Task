using System;

[Serializable]
public class WorldData
{
    public ScoreData ScoreData;

    public WorldData()
    {
        ScoreData = new ScoreData();
    }
}

