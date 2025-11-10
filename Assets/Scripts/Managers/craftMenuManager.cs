using UnityEngine;
using UnityEngine.InputSystem;

public class CraftMenuManager : MonoBehaviour
{
    private InputSystem_Actions controls;
    private InputAction playerActivate;
    private InputAction craftDeactivate;

    [Header("UI")]
    public GameObject craftMenu;

    private bool isMenuOpen = false;

    private void Awake()
    {
        controls = new InputSystem_Actions();

        // Usar propiedades generadas directamente
        playerActivate = controls.Player.Activate;
        craftDeactivate = controls.Craft.Deactivate;

        // Comenzamos con Craft deshabilitado y Player activo
        controls.Craft.Disable();
        controls.Player.Enable();

        if (craftMenu != null)
            craftMenu.SetActive(false);
    }

    private void OnEnable()
    {
        if (playerActivate != null)
            playerActivate.performed += OnPlayerActivate;

        if (craftDeactivate != null)
            craftDeactivate.performed += OnCraftDeactivate;
    }

    private void OnDisable()
    {
        if (playerActivate != null)
            playerActivate.performed -= OnPlayerActivate;

        if (craftDeactivate != null)
            craftDeactivate.performed -= OnCraftDeactivate;
    }

    private void OnPlayerActivate(InputAction.CallbackContext context)
    {
        SetMenuState(true); // Abrir menú
    }

    private void OnCraftDeactivate(InputAction.CallbackContext context)
    {
        SetMenuState(false); // Cerrar menú
    }

    private void SetMenuState(bool open)
    {
        isMenuOpen = open;

        // Activar o desactivar menú UI
        if (craftMenu != null)
            craftMenu.SetActive(open);

        // Cursor y time scale
        Cursor.visible = open;
        Cursor.lockState = open ? CursorLockMode.None : CursorLockMode.Locked;
        Time.timeScale = open ? 0.25f : 1f;

        // Bloquear movimiento del Player
        var playerMovement = FindFirstObjectByType<BlobMovement>();
        if (playerMovement != null)
            playerMovement.canMove = !open;

        // Bloquear ataques del Player
        var attackController = FindFirstObjectByType<BlobAttackController>();
        if (attackController != null)
            attackController.canPerformAttack = !open;

        // Habilitar Craft para detectar la acción de cerrar
        if (open)
            controls.Craft.Enable();
        else
            controls.Craft.Disable();
    }
}