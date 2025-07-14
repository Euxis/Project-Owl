using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Parcel : MonoBehaviour
{
    [SerializeField] private float weight;
    private Rigidbody2D rb;
    private bool isCarrying;
    private float offset = 0.75f;

    [SerializeField] private GameObject objPlayer;

    [SerializeField] private Movement movementScript;
    
    private void Awake()
    {
        Setup();
    }

    private void Setup()
    {
        rb = GetComponent<Rigidbody2D>();
        
        objPlayer = GameObject.FindGameObjectWithTag("Player");
        
        movementScript = FindFirstObjectByType<Movement>();

        rb.gravityScale = weight;
    }

    private void Update()
    {
        rb.gravityScale = weight;
        
        if(isCarrying) CarryParcel();
    }

    // When a parcel is placed, it can be stood on and pushed by the player
    private bool isPlaced;

    public void PlaceParcel(bool b)
    {
        isCarrying = !b;
        if (b)
        {
            Vector2 placePos = new Vector2(objPlayer.transform.position.x + 1.0f * movementScript.lastNonZeroDirection,
                objPlayer.transform.position.y + offset);
            gameObject.layer = LayerMask.NameToLayer("Interactable");
            gameObject.transform.position = placePos;
        }
        else gameObject.layer = LayerMask.NameToLayer("Carrying");
    }

    // Position self behind player
    private void CarryParcel()
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
