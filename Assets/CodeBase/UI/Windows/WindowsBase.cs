using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class WindowsBase : MonoBehaviour
{
    public Button MainMenuButton;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    
    private IGameStateMachine _stateMachine;
    private IAudioService _audioService;
    private AudioSource _audioSource;

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
        _audioSource = GetComponent<AudioSource>();
        _stateMachine = AllServices.Container.Single<IGameStateMachine>();
        _audioService = AllServices.Container.Single<IAudioService>();
        MainMenuButton.onClick.AddListener(()=>Change("MainMenu"));
    }

    public void Change(string TransferTo)
    {
        _audioService.PlayOneShotSound(SoundType.UI_Button_Click_Sound, _audioSource);
        _stateMachine.Enter<LoadLevelState, string>(TransferTo);
        
    }
}
