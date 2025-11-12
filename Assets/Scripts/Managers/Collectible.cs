using UnityEngine;

public enum CollectibleType { Chip, Bolt, Gear }

public class Collectible : MonoBehaviour
{
    public CollectibleType type;
    public float lifeTime = 8f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
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
                Destroy(gameObject);
        }
    }
}