using UnityEngine;

public class objectManager : MonoBehaviour
{
    public static objectManager Instance;

    private int chips = 0;
    private int bolts = 0;
    private int gears = 0;

    private const int maxAmount = 3;

    private void Awake()
    {
        
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool AddChip()
    {
        if (chips < maxAmount)
        {
            chips++;
            Debug.Log("Chips: " + chips);
            return true;
        }
        else
        {
            Debug.Log("No puedes recoger más Chips");
            return false;
        }
    }

    public bool AddBolt()
    {
        if (bolts < maxAmount)
        {
            bolts++;
            Debug.Log("Bolts: " + bolts);
            return true;
        }
        else
        {
            Debug.Log("No puedes recoger más Bolts");
            return false;
        }
    }

    public bool AddGear()
    {
        if (gears < maxAmount)
        {
            gears++;
            Debug.Log("Gears: " + gears);
            return true;
        }
        else
        {
            Debug.Log("No puedes recoger más Gears");
            return false;
        }
    }

    public void RemoveChip(int amount)
    {
        chips = Mathf.Max(0, chips - amount);
    }

    public void RemoveBolt(int amount)
    {
        bolts = Mathf.Max(0, bolts - amount);
    }

    public void RemoveGear(int amount)
    {
        gears = Mathf.Max(0, gears - amount);
    }
}