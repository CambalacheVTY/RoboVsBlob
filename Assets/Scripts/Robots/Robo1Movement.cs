using UnityEngine;
using System.Collections;

public class Robo1Movement : MonoBehaviour
{
    

    public int Health = 8;
    public Transform target;
    public float moveSpeed = 3f;
    public float knockbackForce = 3f;

    
    public GameObject collectiblePrefab;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool isFlashing = false;
    private bool isKnockedBack = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        GameObject blob = GameObject.FindWithTag("Player");
        if (blob != null)
            target = blob.transform;
    }

    void FixedUpdate()
    {
        if (isKnockedBack) return;
        if (target == null) return;

        Vector2 direction = ((Vector2)target.position - (Vector2)transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed;

        if (Health <= 0)
            Die(); 
    }
    public void TakeDamage(DamageInfo info)
    {
        Health -= info.amount;

        if (!isFlashing)
            StartCoroutine(FlashRed());

        Vector2 knockDir = ((Vector2)transform.position - info.sourcePos).normalized;
        StartCoroutine(ApplyKnockback(knockDir));

        if (Health <= 0)
            Die();
    }

    public void TakeDamage(int amount, Vector2 hitSource)
    {
       
           
        Health -= amount;

        if (!isFlashing)
            StartCoroutine(FlashRed());

        Vector2 knockDir = ((Vector2)transform.position - hitSource).normalized;
        StartCoroutine(ApplyKnockback(knockDir));

        if (Health <= 0)
            Die();
    }

    private IEnumerator FlashRed()
    {
        isFlashing = true;
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = Color.green;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = originalColor;
        isFlashing = false;
    }

    private IEnumerator ApplyKnockback(Vector2 direction)
    {
        isKnockedBack = true;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.2f);
        isKnockedBack = false;
    }

    
    private void Die()
    {
        TrySpawnCollectible(); 
        Destroy(gameObject);
    }

    
    private void TrySpawnCollectible()
    {
        if (collectiblePrefab == null) return;

        float chance = Random.Range(0f, 1f);
        if (chance <= 0.5f)
        {
            Instantiate(collectiblePrefab, transform.position, Quaternion.identity);
            
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Wall"))
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collision.collider);
        }
    }
}