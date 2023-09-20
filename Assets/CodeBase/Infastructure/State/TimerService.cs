using System;
using UnityEngine;

public class TimerService : ITimerService
{
    public float CurrentTime{get; set;}
    private bool _isRunning;

    public event Action<int> OnTimerTick;
    public event Action OnTimerComplete;

    public void StartTimer(int duration)
    {
        CurrentTime = duration;
        _isRunning = true;
    }

    public void StopTimer()
    {
        _isRunning = false;
        CurrentTime = 0;
    }

    public void UpdateTimer()
    {
        if (_isRunning)
        {
            CurrentTime -= Time.deltaTime;

            int currentIntTime = Mathf.RoundToInt(CurrentTime);
            OnTimerTick?.Invoke(currentIntTime);

            if (currentIntTime <= 0)
            {
                StopTimer();
                OnTimerComplete?.Invoke();
            }
        }
    }
}