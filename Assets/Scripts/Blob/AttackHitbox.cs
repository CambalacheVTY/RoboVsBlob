using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    public int damage = 3;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Robo1Movement enemy = other.GetComponent<Robo1Movement>();
            if (enemy != null)
            {
                Vector2 hitSource = transform.position; // posición del atacante
                enemy.TakeDamage(damage, hitSource);    // ✅ usa la función del enemigo
            }

            Robo2Movement enemy2 = other.GetComponent<Robo2Movement>();
            if (enemy2 != null)
            {
                Vector2 hitSource = transform.position; // posición del atacante
                enemy2.TakeDamage(damage, hitSource);    // ✅ usa la función del enemigo
            }

            Robo3Movement enemy3 = other.GetComponent<Robo3Movement>();
            if (enemy3 != null)
            {
                Vector2 hitSource = transform.position; // posición del atacante
                enemy3.TakeDamage(damage, hitSource);    // ✅ usa la función del enemigo
            }
        }
    }
}