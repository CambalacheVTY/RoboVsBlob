using UnityEngine;
using TMPro;

public class ObjectUI : MonoBehaviour
{
    public TextMeshProUGUI chipsText;
    public TextMeshProUGUI boltsText;
    public TextMeshProUGUI gearsText;

    private void Update()
    {
        if (objectManager.Instance == null) return;

        chipsText.text = "Chips: " + GetChips();
        boltsText.text = "Bolts: " + GetBolts();
        gearsText.text = "Gears: " + GetGears();
    }

    private int GetChips()
    {
        return typeof(objectManager)
            .GetField("chips", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .GetValue(objectManager.Instance) is int value ? value : 0;
    }

    private int GetBolts()
    {
        return typeof(objectManager)
            .GetField("bolts", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .GetValue(objectManager.Instance) is int value ? value : 0;
    }

    private int GetGears()
    {
        return typeof(objectManager)
            .GetField("gears", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .GetValue(objectManager.Instance) is int value ? value : 0;
    }
}