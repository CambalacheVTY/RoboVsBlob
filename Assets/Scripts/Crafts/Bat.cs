using UnityEngine;
using UnityEngine.InputSystem;

public class Bat : MonoBehaviour
{
    public bool Equip = false;
    public Transform player;
    public float offsetDistance = 1f;
    public int damage = 1;
    public float activeTime = 0.15f; // tiempo que el bat está activo

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
        if (!Equip || hasSwung) return;

        hasSwung = true;

        // ubicar bat en dirección y activar daño
        transform.position = player.position + (Vector3)dir * offsetDistance;
        col.enabled = true;

        // programar desactivación del bat
        Invoke(nameof(ResetBat), activeTime);
    }

    private void ResetBat()
    {
        col.enabled = false;
        hasSwung = false;

        // des-equipar PERMANENTEMENTE
        Equip = false;

        // volver a la posición del jugador para ocultar
        transform.position = player.position;
    }

    private void Update()
    {
        // mientras esté equipado, seguir al jugador
        if (Equip && !hasSwung)
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

            // si golpea un enemigo, cancelar antes de los 0.15 s
            ResetBat();
        }
    }
}