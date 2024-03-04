using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera_Simanis : MonoBehaviour
{
    public Transform targetCamera;

    private void Start()
    {
        if (!targetCamera)
        {
            // If no camera is assigned, try to find the main camera
            targetCamera = Camera.main.transform;
        }
    }

    private void LateUpdate()
    {
        // Ensure the camera has been assigned
        if (targetCamera)
        {
            // Set the forward direction of the canvas to be the direction from the canvas to the camera
            transform.forward = targetCamera.forward;
        }
    }
}
