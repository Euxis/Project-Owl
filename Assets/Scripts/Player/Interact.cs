using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class Interact : MonoBehaviour
{
    public Parcel carryingParcel;

    private bool isCarrying = false;

    public List<Parcel> parcelList;

    [SerializeField] private int parcelIndex = 0;
    [SerializeField] private FixedJoint2D joint;
    [SerializeField] private GameObject pointer;
    private GameObject newPointer;

    /// <summary>
    /// Allows player to select which object to interact with
    /// </summary>
    /// <param name="context"></param>
    public void SelectInteractable(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            float input = context.ReadValue<float>();

            if (parcelList.Count <= 0 || input == 0) return;
        
            // Scrolling
            if (input > 0)
            {
                parcelIndex ++;   
            }
            else if(input < 0)
            {
                parcelIndex --;
            }

            // index can only be between 0 and parcelList length, wrapped
            parcelIndex = (parcelIndex + parcelList.Count) % parcelList.Count;
            
            MovePointer(parcelList[parcelIndex].gameObject);
        }
    }

    public void PickupParcel(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // If the player is currently carrying a parcel, drop it
            if (isCarrying)
            {
                if (carryingParcel == null) return;
                
                carryingParcel.Pickup(false);
                parcelList.Remove(carryingParcel);
                isCarrying = false;
                carryingParcel = null;
            }
            // Else pick it up
            else
            {
                if (parcelList.Count <= 0) return;
                
                // Pick up the parcel that is currently selected
                parcelList[parcelIndex].Pickup(true);
                
                // Save it as the currently carried parcel and remove it from the list
                carryingParcel = parcelList[parcelIndex];
                parcelList.RemoveAt(parcelIndex);
                isCarrying = true;
            }
            UpdateListCount();
        }
    }

    /// <summary>
    /// Makes sure the index is within bounds
    /// </summary>
    public void UpdateListCount()
    {
        if (parcelList.Count == 0) parcelIndex = 0;
        else if (parcelIndex < parcelList.Count)
        {
            parcelIndex = parcelList.Count - 1;
        }
    }

    /// <summary>
    /// Instantiates/Destroys pointer
    /// </summary>
    public void SpawnPointer(bool b)
    {
        if (b)
        {
            // Won't make another pointer if one already exists
            if (newPointer != null) return;
            
            Vector2 spawnPos = new Vector2(parcelList[parcelIndex].transform.position.x, parcelList[parcelIndex].transform.position.y + 1f);
            newPointer = Instantiate(pointer, spawnPos, Quaternion.identity);
            newPointer.transform.SetParent(parcelList[parcelIndex].transform);
        }
        else if(newPointer != null) Destroy(newPointer);
    }

    /// <summary>
    /// Moves pointer to new selected object
    /// </summary>
    /// <param name="o"></param>
    private void MovePointer(GameObject o)
    {
        if (newPointer == null) return;
        
        Vector2 spawnPos = new Vector2(o.transform.position.x, o.transform.position.y + 1f);
        newPointer.transform.position = spawnPos;
        newPointer.transform.SetParent(o.transform);
    }
}
