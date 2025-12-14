using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    private InputSystem_Actions controls;
    private bool isPaused = false;

    [Header("UI")]
    public GameObject pausePanel; 

    private void Awake()
    {
        controls = new InputSystem_Actions();

        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    private void OnEnable()
    {
        controls.Enable();

        
        controls.Player.Pause.performed += OnPausePerformed;
    }

    private void OnDisable()
    {
        controls.Player.Pause.performed -= OnPausePerformed;
        controls.Disable();
    }

    private void OnPausePerformed(InputAction.CallbackContext ctx)
    {
        TogglePause();
    }

    private void TogglePause()
    {
        isPaused = !isPaused;

        if (pausePanel != null)
            pausePanel.SetActive(isPaused);

        Time.timeScale = isPaused ? 0f : 1f;
    }

    
    public void OnPauseButtonPressed()
    {
        TogglePause();
    }

    
    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}