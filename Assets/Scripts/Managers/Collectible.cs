using UnityEngine;
using System.Collections;

public enum CollectibleType { Chip, Bolt, Gear }

public class Collectible : MonoBehaviour
{
    private AudioManager audioManager;

    public CollectibleType type;
    public float lifeTime = 10f;

    private SpriteRenderer sr;
    private bool isFading = false;

    
    public float floatHeight = 0.15f;   
    public float floatSpeed = 3f;      
    private Vector3 startPos;          

    private void Start()
    {
        

        sr = GetComponentInChildren<SpriteRenderer>();
        startPos = transform.position; 

        audioManager = Object.FindFirstObjectByType<AudioManager>();
        if (audioManager != null && audioManager.consumableSpawn != null)
        {
            audioManager.PlaySFX(audioManager.consumableSpawn);
        }

        Destroy(gameObject, lifeTime);
        Invoke(nameof(StartFade), 8f);
    }

    private void Update()
    {
       
        float offset = Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = startPos + new Vector3(0, offset, 0);
    }

    private void StartFade()
    {
        if (!isFading)
        {
            isFading = true;
            StartCoroutine(FadeLoop());
        }
    }

    private IEnumerator FadeLoop()
    {
        float fadeSpeed = 2f;

        while (true)
        {
           
            for (float a = 1f; a >= 0.3f; a -= Time.deltaTime * fadeSpeed)
            {
                SetAlpha(a);
                yield return null;
            }

            
            for (float a = 0.3f; a <= 1f; a += Time.deltaTime * fadeSpeed)
            {
                SetAlpha(a);
                yield return null;
            }
        }
    }

    private void SetAlpha(float alpha)
    {
        if (sr != null)
        {
            Color c = sr.color;
            c.a = alpha;
            sr.color = c;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            bool collected = false;

            switch (type)
            {
                case CollectibleType.Chip:
                    collected = objectManager.Instance.AddChip();
                    break;
                case CollectibleType.Bolt:
                    collected = objectManager.Instance.AddBolt();
                    break;
                case CollectibleType.Gear:
                    collected = objectManager.Instance.AddGear();
                    break;
            }

            if (collected)
                if (audioManager != null)
                    audioManager.PlaySFX(audioManager.pickUp);
            Destroy(gameObject);
        }
    }
}