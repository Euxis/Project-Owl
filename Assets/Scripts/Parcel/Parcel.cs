using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Parcel : MonoBehaviour
{
    [SerializeField] private float weight;
    private Rigidbody2D rb;
    private Rigidbody2D playerRb;
    private bool isCarrying;
    private float offset = 0.75f;
    private int lastDir = 0;

    [SerializeField] private GameObject objPlayer;
    [SerializeField] private Movement movementScript;
    [SerializeField] private FixedJoint2D joint;
    [SerializeField] private PhysicsMaterial2D pMatFrictionless;
    
    private void Awake()
    {
        Setup();
    }

    private void Setup()
    {
        rb = GetComponent<Rigidbody2D>();
        
        objPlayer = GameObject.FindGameObjectWithTag("Player");
        playerRb = objPlayer.GetComponent<Rigidbody2D>();
        
        movementScript = FindFirstObjectByType<Movement>();

        rb.gravityScale = weight;

        joint.enabled = false;
    }

    private void Update()
    {
        rb.gravityScale = weight;
        
        if (isCarrying && lastDir != movementScript.lastNonZeroDirection)
        {
            PositionParcel();
            lastDir = movementScript.lastNonZeroDirection;
        }

        if(isCarrying) rb.linearVelocity = playerRb.linearVelocity;
    }

    // When a parcel is placed, it can be stood on and pushed by the player
    private bool isPlaced;

    public void Pickup(bool b)
    {
        isCarrying = b;
        joint.enabled = b;
        movementScript.isCarryingParcel = b;

        // Place
        if (!b)
        {
            if(joint != null) joint.connectedBody = null;
            
            rb.sharedMaterial = null;
            Vector2 placePos = new Vector2(objPlayer.transform.position.x + 1.0f * movementScript.lastNonZeroDirection,
                objPlayer.transform.position.y + offset);
            gameObject.layer = LayerMask.NameToLayer("Interactable");
            gameObject.transform.position = placePos;
        }
        // Carry
        else
        {
            PositionParcel();

            rb.sharedMaterial = pMatFrictionless;
            joint.connectedBody = objPlayer.GetComponent<Rigidbody2D>();
            rb.rotation = 0;
            gameObject.layer = LayerMask.NameToLayer("Carrying");
        }
    }

    // Position self behind player
    private void PositionParcel()
    {
        Vector2 pos = new Vector2(objPlayer.transform.position.x - offset * movementScript.lastNonZeroDirection, 
            objPlayer.transform.position.y + offset);
        gameObject.transform.position = pos;
    }

    public float GetWeight()
    {
        return weight;
    }
}
