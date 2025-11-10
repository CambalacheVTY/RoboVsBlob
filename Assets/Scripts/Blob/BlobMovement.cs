using UnityEngine;
using UnityEngine.InputSystem;

public class BlobMovement : MonoBehaviour
{
    private InputSystem_Actions controls;
    private Vector2 moveInput;
    public float moveSpeed = 5f;

    private Rigidbody2D rb;

    // Nuevo bool para permitir/deshabilitar movimiento
    public bool canMove = true;

    private void Awake()
    {
        controls = new InputSystem_Actions();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }

    private void Update()
    {
        if (!canMove)
        {
            moveInput = Vector2.zero; // Ignorar input
            return;
        }

        moveInput = controls.Player.Move.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }
}