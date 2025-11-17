using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Chainsaw : MonoBehaviour
{
    private AudioManager audioManager;

    
    public bool Equip = false;
    public Transform player;
    public float offsetDistance = 1f;
    public int damage = 1;
    public float attackStep = 0.05f; 
    public int attackRepeats = 50;   

    private BoxCollider2D col;
    private SpriteRenderer spriteRenderer;
    private InputSystem_Actions controls;
    private bool isAttacking = false;
    private Vector2 currentDirection; 

    private void Awake()
    {
        col = GetComponent<BoxCollider2D>();
        if (col != null)
            col.isTrigger = true;
        col.enabled = false;

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        controls = new InputSystem_Actions();

        audioManager = Object.FindFirstObjectByType<AudioManager>();

    }

    private void OnEnable()
    {
        controls.Player.Enable();

        controls.Player.AttackUp.performed += ctx => TryStartAttack(Vector2.up, "up");
        controls.Player.AttackDown.performed += ctx => TryStartAttack(Vector2.down, "down");
        controls.Player.AttackLeft.performed += ctx => TryStartAttack(Vector2.left, "left");
        controls.Player.AttackRight.performed += ctx => TryStartAttack(Vector2.right, "right");
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }

    private void TryStartAttack(Vector2 dir, string direction)
    {
        if (!Equip || isAttacking) return;

        Equip = false;
        currentDirection = dir;
        StartCoroutine(AutoAttackSequence(direction));
        if (audioManager != null)
            audioManager.PlaySFX(audioManager.saw);
    }

    private IEnumerator AutoAttackSequence(string direction)
    {
       
       

        
        switch (direction)
        {
            case "up": transform.localEulerAngles = Vector3.zero; break;
            case "right": transform.localEulerAngles = new Vector3(0, 0, -90); break;
            case "down": transform.localEulerAngles = new Vector3(0, 0, 180); break;
            case "left": transform.localEulerAngles = new Vector3(0, 0, 90); break;
        }

       
        if (spriteRenderer != null)
            spriteRenderer.enabled = true;

       
            

        for (int i = 0; i < attackRepeats; i++)
        {
            
            transform.position = player.position + (Vector3)currentDirection * offsetDistance;

            
            col.enabled = true;
            yield return new WaitForSeconds(attackStep);

           
            col.enabled = false;
            yield return new WaitForSeconds(attackStep);
        }

       
        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        transform.localEulerAngles = Vector3.zero;
        transform.position = player.position;
        audioManager.StopSFX();
        isAttacking = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!col.enabled) return;

        if (collision.CompareTag("Enemy"))
        {
            DamageInfo info = new DamageInfo(damage, player.position);
            collision.SendMessage("TakeDamage", info, SendMessageOptions.DontRequireReceiver);
        }
    }

    private void Update()
    {
        
        if (Equip && !isAttacking)
            transform.position = player.position;
    }
}