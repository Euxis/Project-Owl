using System;
using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    enum CameraMode {Follow, TempFollow, Talk}
    
    private CameraMode mode = CameraMode.Follow;
    
    [SerializeField] private GameObject target;

    private GameObject secondTarget;

    /// <summary>
    /// Set the target that the camera should follow
    /// </summary>
    /// <param name="t"></param>
    public void SetTarget(GameObject t)
    {
        target = t;
    }

    // Different camera behavior depending on CameraMode
    private void MoveCamera()
    {
        Transform finalPos;
        // Smoothly follow target
        if (mode == CameraMode.Follow)
        {
            transform.position = Vector2.Lerp(transform.position, target.transform.position, 0.05f);
        }
        // follow a second target temporarily
        if (mode == CameraMode.TempFollow)
        {
            if (!secondTarget) return;
            transform.position = Vector2.Lerp(transform.position, secondTarget.transform.position, 0.1f);
        }
        // center camera between two targets
        if (mode == CameraMode.Talk)
        {
            // find midpoint between targets
            Vector2 midpoint = (target.transform.position - secondTarget.transform.position)/2;
            transform.position = Vector2.Lerp(transform.position, midpoint, 0.1f);
        }
    }

    private void FixedUpdate()
    {
        MoveCamera(); 
    }
}
