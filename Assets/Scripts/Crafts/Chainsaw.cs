using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Chainsaw : MonoBehaviour
{
    public bool Equip = false;
    public Transform player;
    public float offsetDistance = 1f;
    public int damage = 1;

    private BoxCollider2D col;
    private InputSystem_Actions controls;
    private bool isAttacking = false;

    private void Awake()
    {
        col = GetComponent<BoxCollider2D>();
        col.enabled = false;
        controls = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        controls.Player.Enable();

        controls.Player.AttackUp.performed += ctx => TryStartAttack(Vector2.up);
        controls.Player.AttackDown.performed += ctx => TryStartAttack(Vector2.down);
        controls.Player.AttackLeft.performed += ctx => TryStartAttack(Vector2.left);
        controls.Player.AttackRight.performed += ctx => TryStartAttack(Vector2.right);
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }

    private void TryStartAttack(Vector2 dir)
    {
        if (!Equip || isAttacking) return;
        Equip = false;
        StartCoroutine(AutoAttackSequence(dir));
    }

    private IEnumerator AutoAttackSequence(Vector2 dir)
    {
        isAttacking = true;

        for (int i = 0; i < 50; i++)
        {
           
            transform.position = player.position + (Vector3)dir * offsetDistance;

        
            col.enabled = true;
            yield return new WaitForSeconds(0.05f);

          
            col.enabled = false;
            transform.position = player.position;
            yield return new WaitForSeconds(0.05f);
        }

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
}