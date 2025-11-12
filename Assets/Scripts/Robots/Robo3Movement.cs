using UnityEngine;
using System.Collections;

public class Robo3Movement : MonoBehaviour
{
    public int Health = 8;
    public Transform target;
    public float moveSpeed = 3f;
    public float knockbackForce = 3f;
    public float moveDuration = 0.5f;   
    public float stopDuration = 0.5f;   

   
    public GameObject collectiblePrefab;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool isFlashing = false;
    private bool isKnockedBack = false;
    private bool isMoving = false; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

       
        StartCoroutine(MoveCycle());
    }

    void Update()
    {
        GameObject blob = GameObject.FindWithTag("Player");
        if (blob != null)
            target = blob.transform;
    }

    private IEnumerator MoveCycle()
    {
        while (true)
        {
            if (!isKnockedBack && target != null)
            {
                // 🌀 fase de movimiento
                isMoving = true;
                yield return new WaitForSeconds(moveDuration);
            }

            // 🌀 fase de espera
            isMoving = false;
            rb.linearVelocity = Vector2.zero;
            yield return new WaitForSeconds(stopDuration);
        }
    }

    void FixedUpdate()
    {
        if (isKnockedBack) return;
        if (target == null) return;

        if (isMoving)
        {
            Vector2 direction = ((Vector2)target.position - (Vector2)transform.position).normalized;
            rb.linearVelocity = direction * moveSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }

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
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = originalColor;
        isFlashing = false;
    }

    private IEnumerator ApplyKnockback(Vector2 direction)
    {
        isKnockedBack = true;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.1f);
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
        if (chance <= 0.25f)
        {
            Instantiate(collectiblePrefab, transform.position, Quaternion.identity);
            Debug.Log("Coleccionable spawneado!");
        }
    }
}