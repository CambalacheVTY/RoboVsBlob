using UnityEngine;
using UnityEngine.InputSystem;

public class Bomb : MonoBehaviour
{
    public bool Equip = false;
    public float moveSpeed = 6f;
    public float moveTime = 0.3f;
    public float armDelay = 1.5f;
    public int damage = 1;
    public bool destroyOnExplode = true;

    private Rigidbody2D rb;
    private CircleCollider2D col;
    private bool hasStopped = false;
    private bool exploded = false;
    private float timer = 0f;
    private Vector2 launchDir = Vector2.right;
    private InputSystem_Actions controls;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CircleCollider2D>();
        col.enabled = false;

        controls = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void Start()
    {
        if (!Equip)
        {
            Destroy(gameObject);
            return;
        }

        Vector2 inputDir = controls.Player.Move.ReadValue<Vector2>();
        if (inputDir == Vector2.zero)
            inputDir = Vector2.right;

        launchDir = inputDir.normalized;
        rb.linearVelocity = launchDir * moveSpeed;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (!hasStopped && timer >= moveTime)
        {
            rb.linearVelocity = Vector2.zero;
            hasStopped = true;
        }

        if (!exploded && timer >= armDelay)
        {
            Explode();
        }
    }

    private void Explode()
    {
        exploded = true;
        col.enabled = true;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, col.radius * transform.localScale.x);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                hit.SendMessage("TakeDamage", new DamageInfo(damage, transform.position), SendMessageOptions.DontRequireReceiver);
            }
        }

        if (destroyOnExplode)
            Destroy(gameObject, 0.05f);
    }
}

public struct DamageInfo
{
    public int amount;
    public Vector2 sourcePos;

    public DamageInfo(int amount, Vector2 sourcePos)
    {
        this.amount = amount;
        this.sourcePos = sourcePos;
    }
}