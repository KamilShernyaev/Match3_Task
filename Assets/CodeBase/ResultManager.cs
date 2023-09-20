using System;
using UnityEngine;

public class ResultManager : MonoBehaviour
{
    private ITimerService _timerService;
    private WorldData _worldData;
    private LevelStaticData _levelStaticData;
    private IUIFactory _uIFactory;
    
    public void Construct(WorldData worldData, LevelStaticData levelStaticData, ITimerService timerService, IUIFactory uIFactory)
    {
        // Получаем ссылки на сервисы
        _timerService = timerService;
        _worldData = worldData;
        _levelStaticData = levelStaticData;
        _uIFactory = uIFactory;
        
        // Подписываемся на события
        _timerService.OnTimerComplete += OnTimerComplete;
        _worldData.ScoreData.Changed += OnScoreChanged;
    }

    private void OnTimerComplete()
    {
        int currentTime = Mathf.RoundToInt(_timerService.CurrentTime);
        LoseWindow tempLoseSceen =_uIFactory.CreateLoseScreen();
        tempLoseSceen.Construct(_worldData.ScoreData.Score, currentTime);
        _worldData.ScoreData.Changed -= OnScoreChanged;
    }

    private void OnScoreChanged()
    {
        if (_worldData.ScoreData.Score >= _levelStaticData.TargetScore)
        {
             int currentTime = Mathf.RoundToInt(_timerService.CurrentTime);
            _uIFactory.CreateVictoryScreen().Construct(_worldData.ScoreData.Score, currentTime);
            _timerService.OnTimerComplete -= OnTimerComplete;
        }
    }
}