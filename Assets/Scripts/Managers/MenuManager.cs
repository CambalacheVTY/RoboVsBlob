using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainPanel;
    public GameObject levelSelectPanel;
    public GameObject creditsPanel;
    public GameObject deathPanel;

  
    public void OpenLevelSelect()
    {
        mainPanel.SetActive(false);
        levelSelectPanel.SetActive(true);
    }

    public void OpenCredits()
    {
        mainPanel.SetActive(false);
        creditsPanel.SetActive(true);
    }

    public void BackToMain()
    {
        mainPanel.SetActive(true);
        levelSelectPanel.SetActive(false);
        creditsPanel.SetActive(false);
    }

   
    public void OpenDeathMenu()
    {
        if (deathPanel != null)
        {
            deathPanel.SetActive(true);
        }
    }

    public void RetryLevel()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    
    public void QuitGame()
    {
        Time.timeScale = 1f; 
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void LoadLevel(string levelName)
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(levelName);
    }
}