using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class Movement : MonoBehaviour
{
    [Header("Movement parameters")] 
    [Tooltip("How much force is applied to the player when moving")]
    [SerializeField] private float moveSpeed;
    
    [Tooltip("How much force is applied to the player when jumping")]
    [SerializeField] private float jumpSpeed;
    
    [Tooltip("How much force is applied to the player when dodging")]
    [SerializeField] private float dodgeSpeed;
    
    [Tooltip("Player's maximum movement speed")]
    [SerializeField] private float maxSpeed;

    [Header("References")] 
    [SerializeField] private Rigidbody2D rb;

    // Movement variables
    private Vector2 direction;
    private int directionInt;

    // Jump variables
    // Can the player jump?
    private bool isLanded = false;

    private bool accelerateFall = false;

    private void FixedUpdate()
    {
        if (directionInt != 0) rb.linearVelocityX = directionInt * moveSpeed;
        rb.linearVelocityX = Mathf.Clamp(rb.linearVelocityX, -maxSpeed, maxSpeed);

        if (accelerateFall) AccelerateFall();
    }

    private void Update()
    {
        CheckLand();

    }

    /// <summary>
    /// Controls ground movement for player.
    /// </summary>
    /// <param name="context"></param>
    public void GroundMovement(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();

        if (context.performed)
        {
            if (input.x > 0)
            {
                directionInt = 1;
            }
            else if (input.x < 0)
            {
                directionInt = -1;
            }
        }
        else
        {
            rb.linearVelocityX = 0;
            directionInt = 0;
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (isLanded)
            {
                rb.AddForceY(jumpSpeed, ForceMode2D.Impulse); 
            }
        }

        // The player can adjust jump height by releasing the jump button early
        // As long as vertical velocity is more than 0
        if (context.performed && rb.linearVelocityY > 0)
        {
            accelerateFall = true;
        }
    }

    /// <summary>
    /// Decreases vertical velocity until its zero
    /// </summary>
    private void AccelerateFall()
    {
        if (rb.linearVelocityY <= 0) accelerateFall = false;
        rb.linearVelocityY -= 0.5f;
    }

    /// <summary>
    /// Checks below player if they are grounded.
    /// </summary>
    private void CheckLand()
    {
        if (Physics2D.OverlapBox(rb.position + 0.75f * Vector2.down, new Vector2(0.95f, 0.75f), 0, LayerMask.GetMask("Floor")))
        {
            isLanded = true;
        }
        else isLanded = false;
    }
}
