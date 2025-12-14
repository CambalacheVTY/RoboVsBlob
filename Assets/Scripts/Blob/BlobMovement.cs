using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class BlobMovement : MonoBehaviour
{
    private BlobAnimationController animationController;

    private AudioManager audioManager;

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

    private Animator animator;

   
    private Coroutine contactBlinkCoroutine;

   
    public bool canDash = false;
    public bool isDashing = false;
    private bool waitingForDashInput = false;
    public float dashSpeedMultiplier = 2f;
    public float dashDuration = 2f;
    public int dashDamage = 2;
    public Vector2 dashDirection;

  
    public bool laserEquip = false;
    public int laserDamage = 4;
    public float laserMaxDistance = 100f;
    private Vector2 lastDirection = Vector2.right;
    public GameObject laserVisual;
    private bool laserActive = false;

  
    public GameObject deathScreen;
    public bool isDead = false;
    private float gameTimer = 0f;

    private void Awake()
    {
        controls = new InputSystem_Actions();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;

        if (laserVisual != null)
            laserVisual.SetActive(false);

        audioManager = Object.FindFirstObjectByType<AudioManager>();

        animationController = GetComponentInChildren<BlobAnimationController>();

    }

    private void OnEnable()
    {
        controls.Player.Enable();

        controls.Player.AttackUp.performed += OnAttackUp;
        controls.Player.AttackDown.performed += OnAttackDown;
        controls.Player.AttackLeft.performed += OnAttackLeft;
        controls.Player.AttackRight.performed += OnAttackRight;
    }

    private void OnDisable()
    {
        controls.Player.AttackUp.performed -= OnAttackUp;
        controls.Player.AttackDown.performed -= OnAttackDown;
        controls.Player.AttackLeft.performed -= OnAttackLeft;
        controls.Player.AttackRight.performed -= OnAttackRight;

        controls.Player.Disable();
    }

    private void OnAttackUp(InputAction.CallbackContext ctx)
    {
        if (!CanAct()) return;
        FireLaser(Vector2.up);
    }

    private void OnAttackDown(InputAction.CallbackContext ctx)
    {
        if (!CanAct()) return;
        FireLaser(Vector2.down);
    }

    private void OnAttackLeft(InputAction.CallbackContext ctx)
    {
        if (!CanAct()) return;
        FireLaser(Vector2.left);
    }

    private void OnAttackRight(InputAction.CallbackContext ctx)
    {
        if (!CanAct()) return;
        FireLaser(Vector2.right);
    }

    private bool CanAct()
    {
        return !isDead && Time.timeScale > 0f;
    }

    private void HandleRotation()
    {
        if (moveInput == Vector2.zero)
            return;

        if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
            lastDirection = new Vector2(Mathf.Sign(moveInput.x), 0);
        else
            lastDirection = new Vector2(0, Mathf.Sign(moveInput.y));
    }

    private void Update()
    {
        
        if (!isDead)
            gameTimer += Time.deltaTime;

        if (laserActive && laserVisual != null)
            laserVisual.transform.position = transform.position;

        if (isDashing) return;

        if (!canMove)
        {
            moveInput = Vector2.zero;
            return;
        }

        moveInput = controls.Player.Move.ReadValue<Vector2>();


        moveInput = controls.Player.Move.ReadValue<Vector2>();

        if (CanAct())
            HandleRotation();




        if (canDash && waitingForDashInput && moveInput != Vector2.zero)
        {
            waitingForDashInput = false;
            StartDash(moveInput.normalized);
            if (audioManager != null)
                audioManager.PlaySFX(audioManager.drill);
            return;
        }

        
        if (touching && canBeDamaged)
        {
            damageTimer += Time.deltaTime;
            if (damageTimer >= 0.3f)
            {
                hp -= 1;
                damageTimer = 0f;
            }
        }

        
        if (hp <= 0 && !isDead)
        {
            Die();
            return;
        }
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            rb.linearVelocity = dashDirection * moveSpeed * dashSpeedMultiplier;
            return;
        }

        if (!isDead)
            rb.linearVelocity = moveInput * moveSpeed;
        else
            rb.linearVelocity = Vector2.zero;
    }

    public void SetGodMode(bool value)
    {
        isInvulnerable = value;
        canBeDamaged = !value;


        spriteRenderer.color = value ? Color.magenta : originalColor;


        Debug.Log("GOD MODE: " + (value ? "ON" : "OFF"));
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

            if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("innerWall"))
            {
                if (audioManager != null)
                {
                    audioManager.StopSFX();

                }
                StopDash();
            }

            return;
        }

        if (collision.gameObject.CompareTag("Enemy") && canBeDamaged)
        {
            if (audioManager != null)
                audioManager.PlaySFX(audioManager.takeDamage); ;
            touching = true;
            hp -= 1;
            damageTimer = 0f;

            if (contactBlinkCoroutine == null)
                contactBlinkCoroutine = StartCoroutine(ContactDamageBlink());
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {


        if (collision.gameObject.CompareTag("Enemy"))
        {
            touching = false;
            damageTimer = 0f;

            if (contactBlinkCoroutine != null)
            {
                StopCoroutine(contactBlinkCoroutine);
                contactBlinkCoroutine = null;

                if (spriteRenderer != null)
                    spriteRenderer.color = originalColor;
            }
        }if(!isInvulnerable)
        { audioManager.StopSFX(); }
            
        
    }

  


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

        if (animator != null)
            animator.SetBool("isDashing", true);

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

        if (animator != null)
            animator.SetBool("isDashing", false);

        canBeDamaged = true;
        isInvulnerable = false;

        rb.linearVelocity = Vector2.zero;

        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;
    }

   
    private void FireLaser(Vector2 dir)
    {
        lastDirection = dir.normalized;

        if (!laserEquip)
            return;

        StartCoroutine(LaserVisualCoroutine(dir));

        
            audioManager.PlaySFX(audioManager.laser);

        Vector2 origin = (Vector2)transform.position + lastDirection * 0.1f;
        float maxDist = laserMaxDistance;

        RaycastHit2D wallHit = Physics2D.Raycast(origin, lastDirection, maxDist, ~0);
        Vector2 endPoint;

        if (wallHit.collider != null && wallHit.collider.CompareTag("Wall"))
            endPoint = wallHit.point;
        else
            endPoint = origin + lastDirection * maxDist;

        float distance = Vector2.Distance(origin, endPoint);
        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, lastDirection, distance);

        foreach (RaycastHit2D h in hits)
        {
            if (h.collider == null) continue;
            if (h.collider.gameObject == this.gameObject) continue;

            Robo1Movement r1 = h.collider.GetComponent<Robo1Movement>();
            Robo2Movement r2 = h.collider.GetComponent<Robo2Movement>();
            Robo3Movement r3 = h.collider.GetComponent<Robo3Movement>();

            if (r1 != null) r1.TakeDamage(laserDamage, origin);
            if (r2 != null) r2.TakeDamage(laserDamage, origin);
            if (r3 != null) r3.TakeDamage(laserDamage, origin);
        }
        

        laserEquip = false;
    }

    private IEnumerator LaserVisualCoroutine(Vector2 dir)
    {
        if (laserVisual == null)
            yield break;

        laserActive = true;
        laserVisual.SetActive(true);
        laserVisual.transform.position = transform.position;

        float angle =
            (dir == Vector2.right) ? 0f :
            (dir == Vector2.up) ? 90f :
            (dir == Vector2.down) ? 270f :
            180f;

        laserVisual.transform.localRotation = Quaternion.Euler(0, 0, angle);

        yield return new WaitForSeconds(0.5f);

        laserVisual.SetActive(false);
        laserActive = false;
    }

    
    private IEnumerator ContactDamageBlink()
    {
        while (touching && canBeDamaged)
        {
            if (spriteRenderer != null)
                spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.3f);

            yield return new WaitForSeconds(0.1f);

            if (spriteRenderer != null)
                spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);

            yield return new WaitForSeconds(0.1f);
        }

        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;

        contactBlinkCoroutine = null;
    }

    
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


   
    private void Die()
    {
        if (isDead) return;
        audioManager.StopSFX();

        isDead = true;
        if (audioManager != null)
            audioManager.PlaySFX(audioManager.death);

        canMove = false;
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;

        if (animationController != null)
            animationController.PlayDeathAnimation();

        if (deathScreen != null)
            deathScreen.SetActive(true);

        
        DeathScreenUI ui = deathScreen.GetComponent<DeathScreenUI>();
        if (ui != null)
            ui.Show(gameTimer);  

       
        Time.timeScale = 0f;
    }
    
}