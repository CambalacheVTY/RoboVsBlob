using UnityEngine;

public class PortalActivator : MonoBehaviour
{
    public bool activated = false;

    public Portal portal1;
    public Portal portal2;

    public Sprite button1;
    public Sprite button2;
    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (activated) return;

        if (other.CompareTag("Player"))
        {
            activated = true;

            portal1.active = true;
            portal2.active = true;

            if (button2 != null)
                sr.sprite = button2;
        }
    }

    
    public void DeactivateButton()
    {
        activated = false;
        sr.sprite = button1;
    }
}