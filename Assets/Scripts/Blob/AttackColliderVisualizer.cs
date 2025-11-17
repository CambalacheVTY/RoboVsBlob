using UnityEngine;

[ExecuteAlways] 
[RequireComponent(typeof(Collider2D))]
public class AttackColliderVisualizer : MonoBehaviour
{
    private Collider2D col;

    void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    void OnDrawGizmos()
    {
        if (col == null)
            col = GetComponent<Collider2D>();

        
        if (Application.isPlaying)
        {
            Gizmos.color = (col.enabled && col.isTrigger) ? Color.cyan : Color.red;
        }
        else
        {
            
            Gizmos.color = col.enabled ? new Color(0, 0.8f, 1f) : Color.red;
        }

        
        if (col is BoxCollider2D box)
        {
            Vector3 size = new Vector3(box.size.x, box.size.y, 0f);
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(box.offset, size);
        }
    }
}