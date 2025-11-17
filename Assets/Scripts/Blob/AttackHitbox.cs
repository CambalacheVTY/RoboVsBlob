using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    private AudioManager audioManager;
    public int damage = 3;

    private void Awake()
    {
        audioManager = Object.FindFirstObjectByType<AudioManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (audioManager != null)
                audioManager.PlaySFX(audioManager.robotHit);

            Robo1Movement enemy = other.GetComponent<Robo1Movement>();
            if (enemy != null)
            {
                Vector2 hitSource = transform.position; 
                enemy.TakeDamage(damage, hitSource);    
            }

            Robo2Movement enemy2 = other.GetComponent<Robo2Movement>();
            if (enemy2 != null)
            {
                Vector2 hitSource = transform.position; 
                enemy2.TakeDamage(damage, hitSource);   
            }

            Robo3Movement enemy3 = other.GetComponent<Robo3Movement>();
            if (enemy3 != null)
            {
                Vector2 hitSource = transform.position; 
                enemy3.TakeDamage(damage, hitSource);    
            }
        }
    }
}