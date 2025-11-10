using UnityEngine;
using System.Collections;

public class Robo2Movement : MonoBehaviour
{
    public int Health = 8;
    public Transform target;
    public float moveSpeed = 3f;
    public float knockbackForce = 3f;
    public float slideSmoothness = 5f; // 🌀 qué tan suave es el deslizamiento (más alto = más suave)

    // 🆕 Prefab del coleccionable
    public GameObject collectiblePrefab;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool isFlashing = false;
    private bool isKnockedBack = false;

    private Vector2 currentVelocity; // 🌀 para interpolar suavemente

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
        if (isKnockedBack || target == null) return;

        // 🌀 Dirección hacia el jugador
        Vector2 desiredDirection = ((Vector2)target.position - (Vector2)transform.position).normalized;
        Vector2 desiredVelocity = desiredDirection * moveSpeed;

        // 🌀 Interpolación suave entre la velocidad actual y la deseada
        currentVelocity = Vector2.Lerp(currentVelocity, desiredVelocity, Time.fixedDeltaTime * slideSmoothness);

        rb.linearVelocity = currentVelocity;

        if (Health <= 0)
            Die(); // 🆕 llamamos a Die() antes de destruir
    }

    public void TakeDamage(int amount, Vector2 hitSource)
    {
        Health -= amount;

        if (!isFlashing)
            StartCoroutine(FlashRed());

        Vector2 knockDir = ((Vector2)transform.position - hitSource).normalized;
        StartCoroutine(ApplyKnockback(knockDir));

        if (Health <= 0)
            Die(); // 🆕 llamamos a Die() antes de destruir
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
        yield return new WaitForSeconds(0.3f);
        isKnockedBack = false;
    }

    // 🆕 Función que maneja la muerte del enemigo
    private void Die()
    {
        TrySpawnCollectible(); // 🆕 intento de spawnear collectible antes de destruir
        Destroy(gameObject);
    }

    // 🆕 Función que tiene 25% de probabilidad de spawnear un coleccionable
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