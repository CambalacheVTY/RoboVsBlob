using UnityEngine;
using UnityEngine.UI;

public class displayManager : MonoBehaviour
{
    [Header("UI")]
    public Toggle fullscreenToggle;

    private const string PREF_FULLSCREEN = "fullscreen";

    private void Start()
    {
        bool isFullscreen = PlayerPrefs.GetInt(PREF_FULLSCREEN, 0) == 1;

        ApplyFullscreen(isFullscreen);

        if (fullscreenToggle != null)
        {
            fullscreenToggle.isOn = isFullscreen;
            fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggle);
        }
    }

    private void OnDestroy()
    {
        if (fullscreenToggle != null)
            fullscreenToggle.onValueChanged.RemoveListener(OnFullscreenToggle);
    }

    // ============================
    // FULLSCREEN TOGGLE
    // ============================
    public void OnFullscreenToggle(bool value)
    {
        ApplyFullscreen(value);

        PlayerPrefs.SetInt(PREF_FULLSCREEN, value ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void ApplyFullscreen(bool fullscreen)
    {
        if (fullscreen)
        {
            // Borderless fullscreen (no resize)
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        }
        else
        {
            // Windowed + resizable (drag corner)
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
    }
}
