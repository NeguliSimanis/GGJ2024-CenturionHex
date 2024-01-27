using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycast_Simanis : MonoBehaviour
{
    CenturionGame centurionGame;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        // Cast a ray from the mouse position
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Check if the ray hits an object
        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log("Mouse is hovering over° " + hit.collider.gameObject.name);
            // Check if the hit object has a specific tag or layer if needed
            if (hit.collider.CompareTag("YourObjectTag"))
            {
                // Object is being hovered
                Debug.Log("Mouse is hovering over the object");

                // You can perform additional actions here, such as changing the object's color, displaying a tooltip, etc.
            }
        }
        else
        {
            // Mouse is not hovering over any object
            // You can perform additional actions here if needed
        }
    }
}
