using System;

[Serializable]
public class ScoreData
{
    public int Score;

    public Action Changed;

    public void Add(int score)
    {
      Score += score;
      Changed?.Invoke();
    }
}

