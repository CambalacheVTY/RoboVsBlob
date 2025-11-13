using UnityEngine;
using UnityEngine.InputSystem;

public class CraftMenuManager : MonoBehaviour
{
    private InputSystem_Actions controls;
    public GameObject craftMenu;

    private bool isMenuOpen = false;

    private int usedChips = 0;
    private int usedBolts = 0;
    private int usedGears = 0;

    public GameObject Bomb;
    public GameObject SuperBomb;

    private BlobAttackController playerAttack; 

    private void Awake()
    {
        controls = new InputSystem_Actions();

       
        controls.Player.Activate.performed += ctx => ToggleMenu();

        
        controls.Player.AttackLeft.performed += ctx => OnSelectLeft();
        controls.Player.AttackRight.performed += ctx => OnSelectRight();
        controls.Player.AttackUp.performed += ctx => OnSelectUp();
    }

    private void Start()
    {
        
        playerAttack = FindFirstObjectByType<BlobAttackController>();
    }

    private void OnEnable()
    {
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }

    private void ToggleMenu()
    {
        if (isMenuOpen)
            CloseMenu();
        else
            OpenMenu();
    }

    private void OpenMenu()
    {
        isMenuOpen = true;
        craftMenu.SetActive(true);
        Time.timeScale = 0.3f;

        usedChips = 0;
        usedBolts = 0;
        usedGears = 0;

        
        if (playerAttack != null)
            playerAttack.canPerformAttack = false;
    }

    private void CloseMenu()
    {
        if (!isMenuOpen) return;

        isMenuOpen = false;
        craftMenu.SetActive(false);
        Time.timeScale = 1f;

     
        if (playerAttack != null)
            playerAttack.canPerformAttack = true;

        CheckCombinations();
    }

    // =====================
    //   Selecciones Craft
    // =====================

    private void OnSelectLeft()
    {
        if (!isMenuOpen) return;

        var obj = objectManager.Instance;

        if (obj.GetChips() > 0)
        {
            obj.RemoveChip(1);
            usedChips++;
            Debug.Log("🧩 Chip usado. Total usados: " + usedChips);
        }
        else
        {
            Debug.Log("❌ No hay más Chips disponibles.");
        }
    }

    private void OnSelectRight()
    {
        if (!isMenuOpen) return;

        var obj = objectManager.Instance;

        if (obj.GetGears() > 0)
        {
            obj.RemoveGear(1);
            usedGears++;
            Debug.Log("⚙️ Gear usado. Total usados: " + usedGears);
        }
        else
        {
            Debug.Log("❌ No hay más Gears disponibles.");
        }
    }

    private void OnSelectUp()
    {
        if (!isMenuOpen) return;

        var obj = objectManager.Instance;

        if (obj.GetBolts() > 0)
        {
            obj.RemoveBolt(1);
            usedBolts++;
            Debug.Log("🔩 Bolt usado. Total usados: " + usedBolts);
        }
        else
        {
            Debug.Log("❌ No hay más Bolts disponibles.");
        }
    }

    // =====================
    //   Resultado Craft
    // =====================

    private void CheckCombinations()
    {
        var playerAttack = FindFirstObjectByType<BlobAttackController>();
        var obj = objectManager.Instance;

        int bolts = obj.GetBolts();
        int chips = obj.GetChips();
        int gears = obj.GetGears();

        
        if (usedBolts == 2 && usedChips == 0 && usedGears == 0)
        {   
                playerAttack.bombEquipped = true;    
        }
                
        else if (usedBolts >= 3 && usedChips == 0 && usedGears == 0)
        { 
                playerAttack.superBombEquipped = true;            
        }
        else if (usedBolts == 0 && usedChips == 2 && usedGears == 0) //falta hacer que el bate solo pegue una vez
        {
            playerAttack.batEquipped = true;
            playerAttack.crafted = true;
            
        }

        else if (usedBolts == 0 && usedChips >= 3 && usedGears == 0)
        {
            playerAttack.chainsawEquipped = true;
            playerAttack.crafted = true;
            
        }

        // ❌ Si ninguna combinación es válida → reembolso
        if (playerAttack.crafted == false)
        {
            Debug.Log("❌ Receta inválida. Se reembolsan materiales usados.");

            
        }

        usedBolts = 0;
        usedChips = 0;
        usedGears = 0;

    }
}
