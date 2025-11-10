using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public int targetFPS = 100;

    private void Awake()
    {
        

        Application.targetFrameRate = targetFPS;
        
     

        QualitySettings.vSyncCount = 0; 

    }
}
