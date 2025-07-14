using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
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

    [Tooltip("Length of ground dodge")] [SerializeField]
    private float groundDodgeLength;
    
    [Tooltip("Length of air dodge")] [SerializeField]
    private float airDodgeLength;

    [Header("References")] 
    [SerializeField] private Rigidbody2D rb;
    
    // Movement variables
    private Vector2 direction;
    [SerializeField] private int directionInt;           // Will be -1 when left, 1 when right
    public int lastNonZeroDirection = 1;   // Holds the last nonzero direction faced
    private int queuedDirection;
    
    private int maxDoubleJump = 1;
    private int doubleJump;
    private bool dodging = false;
    public bool canAirDodge = false;
    
    // Jump variables
    private bool isLanded = false;  // Can the player jump?
    private bool cancelJump = false;
    
    // Script references
    [SerializeField]  Helpers.Timer timer;

    private void Awake()
    {
        //timer = FindFirstObjectByType<Timer>();
    }

    private void FixedUpdate()
    {
        if (dodging || dodging && !isLanded) return;
        if (directionInt != 0) rb.linearVelocityX = directionInt * moveSpeed;
        rb.linearVelocityX = Mathf.Clamp(rb.linearVelocityX, -maxSpeed, maxSpeed);

        if (cancelJump) CancelJump();
    }

    private void Update()
    {
        if(directionInt != 0) lastNonZeroDirection = directionInt;
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
            if (dodging) return;
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
                canAirDodge = true;
                rb.AddForceY(jumpSpeed, ForceMode2D.Impulse); 
            }
            // If the player is in air and the number of double jumps possible is more than zero
            // Allow another jump
            else if (!isLanded && doubleJump > 0)
            {
                canAirDodge = true;
                cancelJump = false;     // Cancel accelerate fall so we get the full height again
                if(doubleJump - 1 >= 0) doubleJump--;
                rb.linearVelocityY = 0;     // Set Y velocity to 0 so the jump force being added isn't influenced
                                            // by previous jump
                rb.AddForceY(jumpSpeed, ForceMode2D.Impulse);
            }
        }

        // The player can adjust jump height by releasing the jump button early
        // As long as vertical velocity is more than 0
        if (context.performed && rb.linearVelocityY > 0)
        {
            cancelJump = true;
        }
    }

    public void Dodge(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (dodging || !canAirDodge) return;
            
            Action DodgeTimer = () =>
            {
                //rb.linearVelocityX = 0;
                rb.gravityScale = 2.5f;
                dodging = false;
            };

            // Remove gravity and stop vertical velocity
            rb.gravityScale = 0;
            rb.linearVelocityY = 0;
            dodging = true;
            
            // Dodges have different distances when used on ground/in air
            if (isLanded)
            {
                rb.linearVelocityX = 0;
                rb.AddForceX(dodgeSpeed * lastNonZeroDirection, ForceMode2D.Impulse);
                timer.DoTimer(groundDodgeLength, DodgeTimer);
            }

            if (!isLanded && canAirDodge)
            {
                canAirDodge = false;
                rb.linearVelocityX = 0;
                rb.AddForceX(dodgeSpeed * 0.80f * lastNonZeroDirection, ForceMode2D.Impulse);
                directionInt = 0;
                timer.DoTimer(airDodgeLength, DodgeTimer);
            }
        }
    }

    /// <summary>
    /// Decreases vertical velocity until its zero
    /// </summary>
    private void CancelJump()
    {
        if (rb.linearVelocityY <= 0) cancelJump = false;
        rb.linearVelocityY -= 0.5f;
    }

    /// <summary>
    /// Checks below player if they are grounded.
    /// </summary>
    private void CheckLand()
    {
        if (Physics2D.OverlapBox(rb.position + 0.50f * Vector2.down, new Vector2(0.95f, 0.75f), 0, LayerMask.GetMask("Floor")))
        {
            // Restore double jump uses
            doubleJump = maxDoubleJump;
            
            // If the player was previously midair, then landing will set their velocity to 0
            isLanded = true;
        }
        else
        {
            // If the player was previously landed, then they can air dodge (i.e. they fell off a ledge without jumping)
            if(isLanded) canAirDodge = true;
            isLanded = false;
        }
    }
}
