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

    private void Awake()
    {
        controls = new InputSystem_Actions();

        // Mapeo de controles del menú de crafteo
        controls.Craft.SelectLeft.performed += ctx => OnSelectLeft();
        controls.Craft.SelectRight.performed += ctx => OnSelectRight();
        controls.Craft.SelectUp.performed += ctx => OnSelectUp();
        controls.Craft.Deactivate.performed += ctx => CloseMenu();

        // Control para abrir/cerrar menú desde el mapa Player
        controls.Player.Activate.performed += ctx => ToggleMenu();
    }

    private void OnEnable()
    {
        controls.Player.Enable();
        controls.Craft.Disable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
        controls.Craft.Disable();
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

        // Desactivar mapa Player y activar mapa Craft
        controls.Player.Disable();
        controls.Craft.Enable();
    }

    private void CloseMenu()
    {
        if (!isMenuOpen) return;

        isMenuOpen = false;
        craftMenu.SetActive(false);
        Time.timeScale = 1f;

        // Volver a habilitar Player y desactivar Craft
        controls.Craft.Disable();
        controls.Player.Enable();

        CheckCombinations();
    }

    // =====================
    //   Selecciones Craft
    // =====================

    private void OnSelectLeft()
    {
        if (!isMenuOpen) return;
        objectManager.Instance.RemoveChip(1);
        usedChips++;
    }

    private void OnSelectRight()
    {
        if (!isMenuOpen) return;
        objectManager.Instance.RemoveGear(1);
        usedGears++;
    }

    private void OnSelectUp()
    {
        if (!isMenuOpen) return;
        objectManager.Instance.RemoveBolt(1);
        usedBolts++;
    }

    // =====================
    //   Resultado Craft
    // =====================

    private void CheckCombinations()
    {
        // 2 bolts → bomba
        if (usedBolts == 2 && Bomb != null)
        {
            Bomb.GetComponent<Bomb>().Equip = true;
        }

        // 3 bolts → super bomba
        if (usedBolts >= 3 && SuperBomb != null)
        {
            SuperBomb.GetComponent<Bomb>().Equip = true;
        }
    }
}
