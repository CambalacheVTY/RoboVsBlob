using UnityEngine;

public class CameraFollowY : MonoBehaviour
{
    public Transform player;   
    public float smoothSpeed = 5f; 

    private float initialX;    
    private float initialZ;    

    void Start()
    {
        
        initialX = transform.position.x;
        initialZ = transform.position.z;
    }

    void LateUpdate()
    {
        if (player == null) return;

       
        float targetY = player.position.y;
        float newY = Mathf.Lerp(transform.position.y, targetY, smoothSpeed * Time.deltaTime);

       
        transform.position = new Vector3(initialX, newY, initialZ);
    }
}
