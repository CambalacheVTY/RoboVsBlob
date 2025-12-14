using UnityEngine;
using UnityEngine.InputSystem;

public class CheatManager : MonoBehaviour
{
    private InputSystem_Actions controls;
    private bool cheatMode = false;
    private bool godMode = false;

    private BlobMovement player;

    [Header("UI")]
    [SerializeField] private GameObject cheatText;

    private void Awake()
    {
        controls = new InputSystem_Actions();
        player = FindFirstObjectByType<BlobMovement>();

        if (cheatText != null)
            cheatText.SetActive(false);
    }

    private void OnEnable()
    {
        controls.Enable();

        controls.Player.CheatMode.performed += ToggleCheatMode;
        controls.Player.CheatBolt.performed += CheatBolt;
        controls.Player.CheatChip.performed += CheatChip;
        controls.Player.CheatGear.performed += CheatGear;
        controls.Player.CheatGodMode.performed += ToggleGodMode;
        controls.Player.CheatNuke.performed += CheatNuke;
    }

    private void OnDisable()
    {
        controls.Player.CheatMode.performed -= ToggleCheatMode;
        controls.Player.CheatBolt.performed -= CheatBolt;
        controls.Player.CheatChip.performed -= CheatChip;
        controls.Player.CheatGear.performed -= CheatGear;
        controls.Player.CheatGodMode.performed -= ToggleGodMode;
        controls.Player.CheatNuke.performed -= CheatNuke;

        controls.Disable();
    }

   
    private void ToggleCheatMode(InputAction.CallbackContext ctx)
    {
        cheatMode = !cheatMode;

        Debug.Log("CHEAT MODE: " + (cheatMode ? "ON" : "OFF"));

        if (cheatText != null)
            cheatText.SetActive(cheatMode);

        if (!cheatMode && player != null)
        {
            godMode = false;
            player.isInvulnerable = false;
            player.canBeDamaged = true;
        }
    }

   
    private void CheatBolt(InputAction.CallbackContext ctx)
    {
        if (!cheatMode) return;
        objectManager.Instance.AddBolt();
    }

    private void CheatChip(InputAction.CallbackContext ctx)
    {
        if (!cheatMode) return;
        objectManager.Instance.AddChip();
    }

    private void CheatGear(InputAction.CallbackContext ctx)
    {
        if (!cheatMode) return;
        objectManager.Instance.AddGear();
    }

    private void ToggleGodMode(InputAction.CallbackContext ctx)
    {
        if (!cheatMode || player == null) return;

        godMode = !godMode;
        player.isInvulnerable = godMode;
        player.canBeDamaged = !godMode;
    }

    private void CheatNuke(InputAction.CallbackContext ctx)
    {
        if (!cheatMode) return;

        foreach (var r in FindObjectsByType<Robo1Movement>(FindObjectsSortMode.None))
            r.TakeDamage(999, transform.position);

        foreach (var r in FindObjectsByType<Robo2Movement>(FindObjectsSortMode.None))
            r.TakeDamage(999, transform.position);

        foreach (var r in FindObjectsByType<Robo3Movement>(FindObjectsSortMode.None))
            r.TakeDamage(999, transform.position);
    }
}