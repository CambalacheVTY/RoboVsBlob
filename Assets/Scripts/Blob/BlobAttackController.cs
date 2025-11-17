using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;



public class BlobAttackController : MonoBehaviour
{
    private AudioManager audioManager;

    public Collider2D attackUp;
    public Collider2D attackDown;
    public Collider2D attackLeft;
    public Collider2D attackRight;

    
    public float attackDuration = 0.2f;
    public float attackCooldown = 0.5f;
    public int damage = 2;

   
    public GameObject bombPrefab;
    public GameObject superBombPrefab;
    [HideInInspector] public bool bombEquipped = false;
    [HideInInspector] public bool superBombEquipped = false;

    
    public Bat bat;
    public Chainsaw chainsaw;
    public bool batEquipped = false;
    public bool chainsawEquipped = false;

    [HideInInspector] public bool crafted = false;

    private bool canAttack = true;
    private InputSystem_Actions input;
    public bool canPerformAttack = true;

    private void Awake()
    {
        
        input = new InputSystem_Actions();

        GameObject audioObj = GameObject.FindWithTag("Audio");
        if (audioObj != null)
            audioManager = audioObj.GetComponent<AudioManager>();
    
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

        input.Player.AttackUp.performed += ctx => HandleAttack(Vector2.up, attackUp);
        input.Player.AttackDown.performed += ctx => HandleAttack(Vector2.down, attackDown);
        input.Player.AttackLeft.performed += ctx => HandleAttack(Vector2.left, attackLeft);
        input.Player.AttackRight.performed += ctx => HandleAttack(Vector2.right, attackRight);
    }

    private void OnDisable()
    {
        input.Disable();
    }

    private void HandleAttack(Vector2 dir, Collider2D attackCollider)
    {
        if (!canPerformAttack)
            return;

       
        if (bombEquipped)
        {
            LaunchBomb(dir, bombPrefab);
            if(audioManager != null)
                audioManager.PlaySFX(audioManager.launch);
            bombEquipped = false;
            return;
        }

        
        if (superBombEquipped)
        {
            LaunchBomb(dir, superBombPrefab);
            if (audioManager != null)
                audioManager.PlaySFX(audioManager.launch);
            superBombEquipped = false;
            return;
        }

       
        if (batEquipped && bat != null)
        {
            bat.Equip = true;
            if (audioManager != null)
                audioManager.PlaySFX(audioManager.swing); 
            batEquipped = false;
            return;
            
        }
        

        if (chainsawEquipped && chainsaw != null)
        {
            chainsaw.Equip = true;
            if (audioManager != null)
                audioManager.PlaySFX(audioManager.saw);
           
            chainsawEquipped = false;
            audioManager.StopSFX();
            return;
            
        }

       
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

   
    private void LaunchBomb(Vector2 dir, GameObject prefab)
    {
        if (prefab == null) return;

        Vector3 spawnPos = transform.position + (Vector3)(dir * 0.5f);
        GameObject bombObj = Instantiate(prefab, spawnPos, Quaternion.identity);

        Bomb newBomb = bombObj.GetComponent<Bomb>();
        if (newBomb != null)
        {
            newBomb.Equip = true;
            newBomb.SetLaunchDirection(dir);
        }

       
    }
}