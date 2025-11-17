using UnityEngine;

public class Bomb : MonoBehaviour
{
    private AudioManager audioManager;

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

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CircleCollider2D>();
        col.enabled = false;

        audioManager = Object.FindFirstObjectByType<AudioManager>();
    }

    
    public void SetLaunchDirection(Vector2 dir)
    {
        launchDir = dir.normalized;
    }

    private void Start()
    {
        if (audioManager != null)
            audioManager.PlaySFX(audioManager.bombTimer);
        if (!Equip)
        {
            Destroy(gameObject);
            return;
        }

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

        if (audioManager != null)
            audioManager.PlaySFX(audioManager.boom);

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, col.radius * transform.localScale.x);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                hit.SendMessage("TakeDamage", new DamageInfo(damage, transform.position), SendMessageOptions.DontRequireReceiver);
            }
        }

        if (destroyOnExplode)
            Destroy(gameObject, 0.3f);
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