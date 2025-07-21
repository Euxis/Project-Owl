using Unity.VisualScripting;
using UnityEngine;

public class InteractTrigger : MonoBehaviour
{
    
    [SerializeField] private Movement movementScript;
    
    [SerializeField] private Interact interactScript;
    
    [SerializeField] private Transform interactCollider;

    [SerializeField] private GameObject objPlayer;

    
    private float offset = 1.25f;

    private ContactFilter2D filter = new ContactFilter2D();

    
    private void Update()
    {
        SendCast();
    }

    private void Awake()
    {
        filter.layerMask = LayerMask.GetMask("Interactable");
        filter.useLayerMask = true;
    }

    /// <summary>
    /// Gets the last nonzero direction stored in the Movement script to
    /// turn the interact trigger so its facing the same way.
    /// </summary>
    private void SendCast()
    {
        Vector2 pos = new Vector2(
            objPlayer.transform.position.x + offset * movementScript.lastNonZeroDirection, 
            objPlayer.transform.position.y);

        interactCollider.position = pos;
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        interactScript.UpdateListCount();

        if (collision.CompareTag("Parcel"))
        {
            interactScript.parcelList.Add(collision.GetComponent<Parcel>());
            interactScript.SpawnPointer(true);
        }
    }

    // Will update conditions later for non-parcel interactables
    private void OnTriggerExit2D(Collider2D collision)
    {
        // Remove any null elements in the list
        if(interactScript.parcelList.Count > 0 && interactScript.parcelList[0] == null) interactScript.parcelList.RemoveAt(0);
        
        interactScript.parcelList.Remove(collision.GetComponent<Parcel>());
        interactScript.UpdateListCount();
        
        if(interactScript.parcelList.Count <= 0) interactScript.SpawnPointer(false);
    }
}
