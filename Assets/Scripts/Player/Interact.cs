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
                
                carryingParcel.PlaceParcel(true);
                parcelList.Remove(carryingParcel);
                isCarrying = false;
                carryingParcel = null;
            }
            // Else pick it up
            else
            {
                if (parcelList.Count <= 0) return;
                
                // Pick up the parcel that is currently selected
                parcelList[parcelIndex].PlaceParcel(false);
                
                // Save it as the currently carried parcel and remove it from the list
                carryingParcel = parcelList[parcelIndex];
                parcelList.RemoveAt(parcelIndex);
                isCarrying = true;
            }
            UpdateListCount();
        }
    }

    public void UpdateListCount()
    {
        if (parcelList.Count == 0) parcelIndex = 0;
        else if (parcelIndex < parcelList.Count)
        {
            parcelIndex = parcelList.Count - 1;
        }
    }
}
