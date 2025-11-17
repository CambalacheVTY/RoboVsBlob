using UnityEngine;
using UnityEngine.InputSystem;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;

    private InputSystem_Actions controls;
    public bool IsPaused { get; private set; } = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        controls = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }

    public void PauseGame()
    {
        if (IsPaused) return;

        IsPaused = true;
        Time.timeScale = 0.25f;

        
        controls.Player.Disable();

        Debug.Log("🎮 Game paused");
    }

    public void ResumeGame()
    {
        if (!IsPaused) return;

        IsPaused = false;
        Time.timeScale = 1f;

        controls.Player.Enable();

        Debug.Log("🎮 Game resumed");
    }

    public InputSystem_Actions Controls => controls;
}