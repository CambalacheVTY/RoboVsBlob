using UnityEngine;
using UnityEngine.InputSystem;

public class Bat : MonoBehaviour
{
    public bool Equip = false;
    public Transform player;
    public float offsetDistance = 1f;
    public int damage = 1;

    private BoxCollider2D col;
    private InputSystem_Actions controls;
    private bool hasSwung = false;

    private void Awake()
    {
        col = GetComponent<BoxCollider2D>();
        col.enabled = false;
        controls = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        controls.Player.Enable();

        controls.Player.AttackUp.performed += ctx => TrySwing(Vector2.up);
        controls.Player.AttackDown.performed += ctx => TrySwing(Vector2.down);
        controls.Player.AttackLeft.performed += ctx => TrySwing(Vector2.left);
        controls.Player.AttackRight.performed += ctx => TrySwing(Vector2.right);
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }

    private void TrySwing(Vector2 dir)
    {
        // Solo puede golpear si está equipado y no está en cooldown
        if (!Equip || hasSwung) return;

        hasSwung = true;

        transform.position = player.position + (Vector3)dir * offsetDistance;
        col.enabled = true;

        // Desactiva el collider después de un tiempo corto
        Invoke(nameof(ResetBat), 0.15f);
    }

    private void ResetBat()
    {
        col.enabled = false;
        hasSwung = false;

        // Siempre vuelve a seguir al jugador
        transform.position = player.position;
    }

    private void Update()
    {
        // Si está equipado, que siempre siga la posición del jugador
        if (Equip)
        {
            transform.position = player.position;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!col.enabled) return;

        if (collision.CompareTag("Enemy"))
        {
            DamageInfo info = new DamageInfo(damage, player.position);
            collision.SendMessage("TakeDamage", info, SendMessageOptions.DontRequireReceiver);
        }
    }
}