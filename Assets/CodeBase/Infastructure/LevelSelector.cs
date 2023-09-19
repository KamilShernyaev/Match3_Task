using UnityEngine;

public class LevelSelector : MonoBehaviour, ISavedProgress
{
    public GameObject button;
    int HighScore;

    private void Start()
    {
        for (int starIndex = 1; starIndex <= 3; starIndex++)
        {
            Transform star = button.gameObject.transform.Find($"star{starIndex}");
            star.gameObject.SetActive(starIndex <= HighScore);                
        }
    }
        

    public void UpdateProgress(PlayerProgress progress)
    {
    }

    public void LoadProgress(PlayerProgress progress)
    {

    }
}
