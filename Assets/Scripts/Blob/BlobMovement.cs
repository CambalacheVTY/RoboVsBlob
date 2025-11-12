using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class BlobMovement : MonoBehaviour
{
    private InputSystem_Actions controls;
    private Vector2 moveInput;
    public float moveSpeed = 5f;
    public int hp = 5;
    private Rigidbody2D rb;
    public bool canMove = true;
    private bool touching = false;
    private float damageTimer = 0f;

    public bool canBeDamaged = true;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private void Awake()
    {
        controls = new InputSystem_Actions();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    private void OnEnable()
    {
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }

    private void Update()
    {
        if (!canMove)
        {
            moveInput = Vector2.zero;
            return;
        }

        moveInput = controls.Player.Move.ReadValue<Vector2>();

        if (touching && canBeDamaged)
        {
            damageTimer += Time.deltaTime;
            if (damageTimer >= 0.3f)
            {
                hp -= 1;
                damageTimer = 0f;
            }
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;

        if (hp <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && canBeDamaged)
        {
            touching = true;
            hp -= 1;
            damageTimer = 0f;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            touching = false;
            damageTimer = 0f;
        }
    }

    // 🔷 Invulnerabilidad con cambio de color
    public void Invulnerability(float duration = 5f)
    {
        StartCoroutine(InvulnerabilityCoroutine(duration));
    }

    private IEnumerator InvulnerabilityCoroutine(float duration)
    {
        canBeDamaged = false;
        if (spriteRenderer != null)
            spriteRenderer.color = Color.blue;

        yield return new WaitForSeconds(duration);

        canBeDamaged = true;
        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;
    }
}