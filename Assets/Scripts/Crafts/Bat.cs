using UnityEngine;
using UnityEngine.InputSystem;

public class Bat : MonoBehaviour
{
   
    public bool Equip = false;
    public Transform player;
    public float offsetDistance = 1f;
    public int damage = 1;
    public float activeTime = 0.15f; 
    public float visibleTime = 0.5f; 

    private BoxCollider2D col;
    private SpriteRenderer spriteRenderer;
    private InputSystem_Actions controls;
    private bool hasSwung = false;
    private bool isSwinging = false;

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
    }

    private void OnEnable()
    {
        controls.Player.Enable();

        controls.Player.AttackUp.performed += ctx => TrySwing(Vector2.up, "up");
        controls.Player.AttackDown.performed += ctx => TrySwing(Vector2.down, "down");
        controls.Player.AttackLeft.performed += ctx => TrySwing(Vector2.left, "left");
        controls.Player.AttackRight.performed += ctx => TrySwing(Vector2.right, "right");
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }

    private void TrySwing(Vector2 dir, string direction)
    {
        if (!Equip || hasSwung || isSwinging) return;

        isSwinging = true;
        hasSwung = true;

      
        transform.position = player.position + (Vector3)dir * offsetDistance;

        
        switch (direction)
        {
            case "up": transform.localEulerAngles = new Vector3(0, 0, 0); break;
            case "right": transform.localEulerAngles = new Vector3(0, 0, -90); break;
            case "down": transform.localEulerAngles = new Vector3(0, 0, 180); break;
            case "left": transform.localEulerAngles = new Vector3(0, 0, 90); break;
        }

       
        col.enabled = true;
        if (spriteRenderer != null)
            spriteRenderer.enabled = true;

       
        Invoke(nameof(DisableCollider), activeTime);

       
        Invoke(nameof(HideSprite), visibleTime);
    }

    private void DisableCollider()
    {
        col.enabled = false;
    }

    private void HideSprite()
    {
        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        hasSwung = false;
        isSwinging = false;
        Equip = false;

       
        transform.localEulerAngles = Vector3.zero;

        transform.position = player.position;
    }

    private void Update()
    {
        if (Equip && !hasSwung)
            transform.position = player.position;
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
}