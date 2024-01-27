using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera_Simanis : MonoBehaviour
{
    private void LateUpdate()
    {
        // Get the camera's position
        Vector3 targetPosition = Camera.main.transform.position;

        // Calculate the rotation needed to face the camera
        Quaternion targetRotation = Quaternion.LookRotation(transform.position - targetPosition);

        // Apply the rotation only on the Y-axis
        transform.rotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
    }
}
