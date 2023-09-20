using UnityEngine;

public class TimerStarter : MonoBehaviour
{
    ITimerService _timerService;

    public void Construct(ITimerService timerService, LevelStaticData levelStaticData)
    {
        _timerService = timerService;
        _timerService.StartTimer(levelStaticData.TimeValue);
    }

    private void FixedUpdate() 
    {
        _timerService.UpdateTimer();
    }
}
