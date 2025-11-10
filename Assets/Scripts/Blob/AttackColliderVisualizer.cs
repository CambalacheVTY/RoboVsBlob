using UnityEngine;

[ExecuteAlways] // Para que se vea tanto en Editor como en Play
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

        // 🔵 Azul si está activo en modo de ataque, 🔴 rojo si no.
        if (Application.isPlaying)
        {
            Gizmos.color = (col.enabled && col.isTrigger) ? Color.cyan : Color.red;
        }
        else
        {
            // En modo Editor, simplemente muestra si el collider está habilitado
            Gizmos.color = col.enabled ? new Color(0, 0.8f, 1f) : Color.red;
        }

        // Dibuja el collider como un rectángulo
        if (col is BoxCollider2D box)
        {
            Vector3 size = new Vector3(box.size.x, box.size.y, 0f);
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(box.offset, size);
        }
    }
}