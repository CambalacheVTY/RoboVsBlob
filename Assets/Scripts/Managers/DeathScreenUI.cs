using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class DeathScreenUI : MonoBehaviour
{
    public static DeathScreenUI Instance;

   
    public GameObject panel;          
    public TMP_Text timeText;         
    public Button retryButton;
    public Button mainMenuButton;

    private float cachedTime;

    private void Awake()
    {
        
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        panel.SetActive(false);
    }

    public void Show(float timeSurvived)
    {
        cachedTime = timeSurvived;
        ShowTime();
        panel.SetActive(true);
        Time.timeScale = 0f; 
    }

    private void ShowTime()
    {
        if (timeText != null)
            timeText.text = "Time: " + cachedTime.ToString("0.0") + " s";
        else
            Debug.LogError("DeathScreenUI → timeText NO asignado en Inspector!");
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.buildIndex);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}