using UnityEngine;

public class BlobColorController : MonoBehaviour
{
    private SpriteRenderer sprite;
    public BlobMovement blob;

    [Header("Colores")]
    public Color fullHpColor = new Color(1f, 0.6f, 0.8f);
    public Color lowHpColor = Color.white;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (blob == null) return;

        
        if (blob.isInvulnerable)
            return;

        
        float t = 1f - (float)blob.hp / blob.maxHp;
        sprite.color = Color.Lerp(fullHpColor, lowHpColor, t);
    }
}