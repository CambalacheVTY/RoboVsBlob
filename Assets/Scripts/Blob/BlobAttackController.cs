using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class BlobAttackController : MonoBehaviour
{
    public Collider2D attackUp;
    public Collider2D attackDown;
    public Collider2D attackLeft;
    public Collider2D attackRight;

    public float attackDuration = 0.1f;
    public float attackCooldown = 0.2f;
    public int damage = 2;

    private bool canAttack = true;
    private InputSystem_Actions input; 

    
    public bool canPerformAttack = true;

    private void Awake()
    {
        input = new InputSystem_Actions();
    }

    private void Start()
    {
        
        if (attackUp != null) attackUp.enabled = false;
        if (attackDown != null) attackDown.enabled = false;
        if (attackLeft != null) attackLeft.enabled = false;
        if (attackRight != null) attackRight.enabled = false;
    }

    private void OnEnable()
    {
        input.Enable();

        
        input.Player.AttackUp.performed += ctx => TryAttack(attackUp);
        input.Player.AttackDown.performed += ctx => TryAttack(attackDown);
        input.Player.AttackLeft.performed += ctx => TryAttack(attackLeft);
        input.Player.AttackRight.performed += ctx => TryAttack(attackRight);
    }

    private void OnDisable()
    {
        input.Disable();
    }

    private void TryAttack(Collider2D attackCollider)
    {
        if (!canPerformAttack)
            return;

        StartCoroutine(Attack(attackCollider));
    }

    private IEnumerator Attack(Collider2D attackCollider)
    {
        if (attackCollider == null || !canAttack)
            yield break;

        canAttack = false;
        attackCollider.enabled = true;

        
        var hitbox = attackCollider.GetComponent<AttackHitbox>();
        if (hitbox != null)
            hitbox.damage = damage;

        yield return new WaitForSeconds(attackDuration);
        attackCollider.enabled = false;

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}