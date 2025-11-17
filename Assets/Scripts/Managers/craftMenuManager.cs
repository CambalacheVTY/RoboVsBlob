using UnityEngine;
using UnityEngine.InputSystem;

public class CraftMenuManager : MonoBehaviour
{
    private AudioManager audioManager;

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

        audioManager = Object.FindFirstObjectByType<AudioManager>();
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

   

    private void OnSelectLeft()
    {
        if (!isMenuOpen) return;

        var obj = objectManager.Instance;

        if (obj.GetChips() > 0)
        {
            obj.RemoveChip(1);
            usedChips++;
            Debug.Log("🧩 Chip usado.");
        }
        else
        {
            Debug.Log("❌ No hay más Chips.");
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
            Debug.Log("⚙️ Gear usado.");
        }
        else
        {
            Debug.Log("❌ No hay más Gears.");
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
            Debug.Log("🔩 Bolt usado.");
        }
        else
        {
            Debug.Log("❌ No hay más Bolts.");
        }
    }

    

    private void CheckCombinations()
    {
        var obj = objectManager.Instance;
        BlobMovement blob = FindFirstObjectByType<BlobMovement>();
        var playerAttack = FindFirstObjectByType<BlobAttackController>();

        
        if (usedChips == 1 && usedGears == 1 && usedBolts == 0)
        {
            if (blob != null)
                blob.laserEquip = true;

            Debug.Log("🔫 Laser activado (1 Chip + 1 Gear)");
        }

       
        else if (usedBolts == 1 && usedGears == 1 && usedChips == 0)
        {
            if (blob != null)
            {
                blob.SetDashActive(true);
                Debug.Log("⚡ Dash activado con 1 Bolt + 1 Gear");
            }
        }

        else if (usedBolts == 2 && usedChips == 0 && usedGears == 0)
        {
            playerAttack.bombEquipped = true;
            Debug.Log("💣 Bomb creada");
        }

        
        else if (usedBolts >= 3 && usedChips == 0 && usedGears == 0)
        {
            playerAttack.superBombEquipped = true;
            Debug.Log("💥 SuperBomb creada");
        }

       
        else if (usedBolts == 0 && usedChips == 2 && usedGears == 0)
        {
            playerAttack.batEquipped = true;
            playerAttack.crafted = true;
            Debug.Log("🪓 Bat creado");
        }

        else if (usedBolts == 0 && usedChips >= 3 && usedGears == 0)
        {
            playerAttack.chainsawEquipped = true;
            playerAttack.crafted = true;
            Debug.Log("🔪 Chainsaw creada");
        }

       
        else if (usedBolts == 0 && usedChips == 0 && usedGears == 2)
        {
            if (blob != null)
                blob.hp = Mathf.Min(blob.hp + 5, blob.maxHp);

            if (audioManager != null)
                audioManager.PlaySFX(audioManager.heal);

            Debug.Log("❤️ +3 HP restaurado");
        }

       
        else if (usedBolts == 0 && usedChips == 0 && usedGears == 3)
        {
            if (blob != null)
                blob.Invulnerability(5f);
            if (audioManager != null)
                audioManager.PlaySFX(audioManager.shield);

            Debug.Log("🛡 Invulnerabilidad activada");
        }

        else
        {
            Debug.Log("❌ Receta inválida. Se reembolsan materiales.");
        }

        usedBolts = 0;
        usedChips = 0;
        usedGears = 0;
    }
}