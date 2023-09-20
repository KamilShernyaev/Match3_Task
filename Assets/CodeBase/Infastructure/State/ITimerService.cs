using System;

public interface ITimerService : IService
{
    float CurrentTime { get; set; }

    event Action<int> OnTimerTick;
    event Action OnTimerComplete;

    void StartTimer(int duration);
    void StopTimer();
    void UpdateTimer();
}
