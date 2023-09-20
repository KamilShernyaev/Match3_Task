using TMPro;
using UnityEngine;

public class TimerCounter : MonoBehaviour
{
    public TextMeshProUGUI Counter;
    private ITimerService _timerService;

    public void Awake()
    {
        _timerService = AllServices.Container.Single<ITimerService>();
        _timerService.OnTimerTick += UpdateTimer;
    }

    private void UpdateTimer(int obj)
    {
        Counter.text = $"{obj}";
    }
}
