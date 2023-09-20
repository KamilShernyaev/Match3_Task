using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class WindowsBase : MonoBehaviour
{
    public Button MainMenuButton;
    private IGameStateMachine _stateMachine;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;

    private void Awake() 
    {
        OnAwake();
    }

    public virtual void Construct(int score, int valueTime)
    {
        scoreText.text = score.ToString();
        timerText.text = valueTime.ToString();
    }
    
    protected virtual void OnAwake()
    {
        _stateMachine = AllServices.Container.Single<IGameStateMachine>();
        MainMenuButton.onClick.AddListener(()=>Change("MainMenu"));
    }

    public void Change(string TransferTo)
    {
        _stateMachine.Enter<LoadLevelState, string>(TransferTo);
    }
}
