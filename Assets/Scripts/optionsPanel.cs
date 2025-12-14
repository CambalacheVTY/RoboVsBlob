using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class OptionsPanel : MonoBehaviour
{
    
    public Toggle fullscreenToggle;

    private Resolution[] resolutions;
    private bool isInitializing = false;

    void OnEnable()
    {
        isInitializing = true;
        InitResolutions();
        LoadFullscreen();
        isInitializing = false;
    }

    // =========================
    // RESOLUTIONS
    // =========================
    void InitResolutions()
    {
        resolutions = Screen.resolutions;
       

        List<string> options = new List<string>();
        int currentIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string label = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(label);

            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                currentIndex = i;
            }
        }

       
    }

    // =========================
    // CALLBACKS
    // =========================
    public void OnResolutionChanged(int index)
    {
        if (isInitializing) return;

        Resolution r = resolutions[index];
        Screen.SetResolution(r.width, r.height, Screen.fullScreenMode);
    }

    public void OnFullscreenToggle(bool value)
    {
        if (isInitializing) return;

        Screen.fullScreenMode = value
            ? FullScreenMode.FullScreenWindow // ✅ CLAVE
            : FullScreenMode.Windowed;

        PlayerPrefs.SetInt("Fullscreen", value ? 1 : 0);
    }

    void LoadFullscreen()
    {
        bool fullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        fullscreenToggle.isOn = fullscreen;
        Screen.fullScreenMode = fullscreen
            ? FullScreenMode.FullScreenWindow
            : FullScreenMode.Windowed;
    }
}