using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class BlobAnimationController : MonoBehaviour
{
    private Animator animator;
    private InputSystem_Actions controls;

    private Vector3 baseScale;
    private BlobMovement movement;

    
    private bool attackLocked = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        controls = new InputSystem_Actions();
        movement = GetComponentInParent<BlobMovement>();
    }

    private void Start()
    {
        baseScale = transform.localScale;
    }

    private void OnEnable()
    {
        controls.Player.Move.performed += OnMove;
        controls.Player.Move.canceled += ctx => UpdateMoveAnimation(Vector2.zero);

        controls.Player.AttackUp.performed += _ => TryAttack("up");
        controls.Player.AttackDown.performed += _ => TryAttack("down");
        controls.Player.AttackLeft.performed += _ => TryAttack("left");
        controls.Player.AttackRight.performed += _ => TryAttack("right");

        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void Update()
    {
        if (!CanAnimate())
            return;

        if (movement != null && movement.isDashing)
        {
            animator.SetBool("isDashing", true);
            transform.localEulerAngles = GetDashRotation(movement.dashDirection);
            return;
        }
        else
        {
            animator.SetBool("isDashing", false);
        }
    }

   
    private bool CanAnimate()
    {
        if (movement == null) return false;
        if (movement.isDead) return false;
        if (Time.timeScale == 0f) return false;
        return true;
    }

   
    private void OnMove(InputAction.CallbackContext ctx)
    {
        if (!CanAnimate())
            return;

        Vector2 move = ctx.ReadValue<Vector2>();
        UpdateMoveAnimation(move);
    }

    private void UpdateMoveAnimation(Vector2 move)
    {
        bool isMoving = move.sqrMagnitude > 0.01f;
        animator.SetBool("isMoving", isMoving);

        if (move.x > 0.1f)
            transform.localScale = new Vector3(Mathf.Abs(baseScale.x), baseScale.y, baseScale.z);
        else if (move.x < -0.1f)
            transform.localScale = new Vector3(-Mathf.Abs(baseScale.x), baseScale.y, baseScale.z);
    }

   
    private void TryAttack(string dir)
    {
        if (!CanAnimate())
            return;

        if (attackLocked) return;
        if (movement != null && movement.isDashing) return;

        StartCoroutine(AttackCooldown());
        StartCoroutine(AttackRoutine(dir));
    }

    private IEnumerator AttackCooldown()
    {
        attackLocked = true;
        yield return new WaitForSeconds(0.5f);
        attackLocked = false;
    }

    private IEnumerator AttackRoutine(string dir)
    {
        animator.SetTrigger("isAttacking");

        animator.SetBool("lookingUp", false);
        animator.SetBool("lookingDown", false);
        animator.SetBool("lookingRight", false);
        animator.SetBool("lookingLeft", false);

        switch (dir)
        {
            case "up":
                animator.SetBool("lookingUp", true);
                transform.localEulerAngles = Vector3.zero;
                break;

            case "down":
                animator.SetBool("lookingDown", true);
                transform.localEulerAngles = new Vector3(0, 0, 180);
                break;

            case "left":
                animator.SetBool("lookingLeft", true);
                transform.localEulerAngles = new Vector3(0, 0, 90);
                break;

            case "right":
                animator.SetBool("lookingRight", true);
                transform.localEulerAngles = new Vector3(0, 0, -90);
                break;
        }

        yield return new WaitForSeconds(0.5f);

        if (CanAnimate())
            transform.localEulerAngles = Vector3.zero;
    }

   
    private Vector3 GetDashRotation(Vector2 dir)
    {
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            return dir.x > 0 ? new Vector3(0, 0, -90) : new Vector3(0, 0, 90);
        else
            return dir.y > 0 ? Vector3.zero : new Vector3(0, 0, 180);
    }

 
    public void PlayDeathAnimation()
    {
        if (animator != null)
            animator.SetTrigger("dead");
    }

    public void DisableAnimationControl()
    {
        controls.Disable();
        enabled = false;
    }
}