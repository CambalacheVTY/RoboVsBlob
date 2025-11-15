using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class BlobMovement : MonoBehaviour
{
    private InputSystem_Actions controls;
    private Vector2 moveInput;
    public float moveSpeed = 5f;

    public int hp = 5;
    public int maxHp = 7;

    private Rigidbody2D rb;

    public bool canMove = true;
    private bool touching = false;
    private float damageTimer = 0f;

    public bool canBeDamaged = true;
    public bool isInvulnerable = false;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    // ============================
    //        DASH SYSTEM
    // ============================
    public bool canDash = false;
    private bool isDashing = false;
    private bool waitingForDashInput = false;
    public float dashSpeedMultiplier = 2f;
    public float dashDuration = 2f;
    public int dashDamage = 2;
    private Vector2 dashDirection;

    // ============================
    //        LASER SYSTEM
    // ============================
    public bool laserEquip = false;      // se activa por crafting
    public int laserDamage = 4;
    public float laserMaxDistance = 100f;
    private Vector2 lastDirection = Vector2.right; // fallback

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

        // Subscribir handlers nombrados (fácil unsubscribe)
        controls.Player.AttackUp.performed += OnAttackUp;
        controls.Player.AttackDown.performed += OnAttackDown;
        controls.Player.AttackLeft.performed += OnAttackLeft;
        controls.Player.AttackRight.performed += OnAttackRight;
    }

    private void OnDisable()
    {
        // Unsubscribe
        controls.Player.AttackUp.performed -= OnAttackUp;
        controls.Player.AttackDown.performed -= OnAttackDown;
        controls.Player.AttackLeft.performed -= OnAttackLeft;
        controls.Player.AttackRight.performed -= OnAttackRight;

        controls.Player.Disable();
    }

    // ---------- Attack handlers ----------
    private void OnAttackUp(InputAction.CallbackContext ctx) { FireLaser(Vector2.up); }
    private void OnAttackDown(InputAction.CallbackContext ctx) { FireLaser(Vector2.down); }
    private void OnAttackLeft(InputAction.CallbackContext ctx) { FireLaser(Vector2.left); }
    private void OnAttackRight(InputAction.CallbackContext ctx) { FireLaser(Vector2.right); }

    // ---------- Update / movement ----------
    private void Update()
    {
        if (isDashing) return;

        if (!canMove)
        {
            moveInput = Vector2.zero;
            return;
        }

        moveInput = controls.Player.Move.ReadValue<Vector2>();

        // Guardar última dirección usada por movimiento (opcional)
        if (moveInput != Vector2.zero)
        {
            if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
                lastDirection = new Vector2(Mathf.Sign(moveInput.x), 0);
            else
                lastDirection = new Vector2(0, Mathf.Sign(moveInput.y));
        }

        // Detectar primer input para dash
        if (canDash && waitingForDashInput && moveInput != Vector2.zero)
        {
            waitingForDashInput = false;
            StartDash(moveInput.normalized);
            return;
        }

        // daño por contacto
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
        if (isDashing)
        {
            rb.linearVelocity = dashDirection * moveSpeed * dashSpeedMultiplier;
            return;
        }

        rb.linearVelocity = moveInput * moveSpeed;

        if (hp <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDashing)
        {
            Robo1Movement r1 = collision.gameObject.GetComponent<Robo1Movement>();
            Robo2Movement r2 = collision.gameObject.GetComponent<Robo2Movement>();
            Robo3Movement r3 = collision.gameObject.GetComponent<Robo3Movement>();

            Vector2 hitSource = transform.position;

            if (r1 != null) r1.TakeDamage(dashDamage, hitSource);
            if (r2 != null) r2.TakeDamage(dashDamage, hitSource);
            if (r3 != null) r3.TakeDamage(dashDamage, hitSource);

            // Solo parar dash con pared
            if (collision.gameObject.CompareTag("Wall"))
            {
                StopDash();
            }

            return;
        }

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

    // ============================
    //       DASH SYSTEM
    // ============================
    public void SetDashActive(bool value)
    {
        canDash = value;
        waitingForDashInput = value;
    }

    private void StartDash(Vector2 direction)
    {
        if (!canDash || isDashing) return;
        dashDirection = direction;
        StartCoroutine(DashCoroutine());
    }

    private IEnumerator DashCoroutine()
    {
        isDashing = true;
        canBeDamaged = false;
        isInvulnerable = true;

        if (spriteRenderer != null)
            spriteRenderer.color = Color.cyan;

        float timer = 0f;

        while (timer < dashDuration && isDashing)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        StopDash();
    }

    private void StopDash()
    {
        isDashing = false;
        canBeDamaged = true;
        isInvulnerable = false;

        rb.linearVelocity = Vector2.zero;

        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;
    }

    // ============================
    //       LASER: FIRE + DEBUG
    // ============================
    private void FireLaser(Vector2 dir)
    {
        // Guardas la dirección "última" para debug/visuales
        lastDirection = dir.normalized;

        // DEBUG: intento de disparo
        Debug.Log($"[Laser] Intento de disparo. laserEquip={laserEquip}, dir={lastDirection}");

        if (!laserEquip)
        {
            Debug.Log("[Laser] No equipado o ya usado.");
            return;
        }

        Vector2 origin = (Vector2)transform.position + lastDirection * 0.1f; // evitar auto-hit
        float maxDist = laserMaxDistance;

        // Raycast para encontrar primer wall (si existe)
        RaycastHit2D wallHit = Physics2D.Raycast(origin, lastDirection, maxDist, ~0); // todas las capas
        Vector2 endPoint;

        if (wallHit.collider != null && wallHit.collider.CompareTag("Wall"))
        {
            endPoint = wallHit.point;
            Debug.Log($"[Laser] Wall hit at {endPoint}. Collider: {wallHit.collider.name}");
        }
        else
        {
            endPoint = origin + lastDirection * maxDist;
            Debug.Log("[Laser] No wall hit, usando distancia máxima.");
        }

        // Dibujo la línea para debug (1s visible en Scene)
        Debug.DrawLine(origin, endPoint, Color.red, 1f);

        // RaycastAll entre origin y endPoint (usamos distancia calculada)
        float distance = Vector2.Distance(origin, endPoint);
        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, lastDirection, distance);

        int enemyHits = 0;
        foreach (RaycastHit2D h in hits)
        {
            if (h.collider == null) continue;

            // Ignorar colisiones con el player (por si acaso)
            if (h.collider.gameObject == this.gameObject) continue;

            // DEBUG nombre del collider
            Debug.Log($"[Laser] Hit collider: {h.collider.name} (tag: {h.collider.tag})");

            // Intentar golpear tus tipos de enemigos
            Robo1Movement r1 = h.collider.GetComponent<Robo1Movement>();
            Robo2Movement r2 = h.collider.GetComponent<Robo2Movement>();
            Robo3Movement r3 = h.collider.GetComponent<Robo3Movement>();

            if (r1 != null) { r1.TakeDamage(laserDamage, origin); enemyHits++; Debug.Log("[Laser] Hit Robo1"); }
            if (r2 != null) { r2.TakeDamage(laserDamage, origin); enemyHits++; Debug.Log("[Laser] Hit Robo2"); }
            if (r3 != null) { r3.TakeDamage(laserDamage, origin); enemyHits++; Debug.Log("[Laser] Hit Robo3"); }
        }

        Debug.Log($"[Laser] Total enemies hit: {enemyHits}");

        // Usar el láser una sola vez
        laserEquip = false;
    }

    // ============================
    //    INVULNERABILIDAD BASE
    // ============================
    public void Invulnerability(float duration = 5f)
    {
        StartCoroutine(InvulnerabilityCoroutine(duration));
    }

    private IEnumerator InvulnerabilityCoroutine(float duration)
    {
        canBeDamaged = false;
        isInvulnerable = true;

        if (spriteRenderer != null)
            spriteRenderer.color = Color.blue;

        yield return new WaitForSeconds(duration);

        canBeDamaged = true;
        isInvulnerable = false;

        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;
    }
}