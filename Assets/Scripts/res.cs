using UnityEngine;

public class CameraFollowY : MonoBehaviour
{
    public Transform player;   // Asigna aquí el transform del jugador
    public float smoothSpeed = 5f; // Qué tan suave sigue la cámara

    private float initialX;    // Para mantener fija la posición X
    private float initialZ;    // Para mantener fija la posición Z (profundidad de la cámara)

    void Start()
    {
        // Guarda la posición inicial en X y Z de la cámara
        initialX = transform.position.x;
        initialZ = transform.position.z;
    }

    void LateUpdate()
    {
        if (player == null) return;

        // Solo seguimos el eje Y del jugador
        float targetY = player.position.y;
        float newY = Mathf.Lerp(transform.position.y, targetY, smoothSpeed * Time.deltaTime);

        // Actualizamos solo Y, dejamos X y Z fijos
        transform.position = new Vector3(initialX, newY, initialZ);
    }
}
