using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SP_RaycastControl : MonoBehaviour
{
    public static SP_RaycastControl instance;

    private Camera mainCamera;

    [HideInInspector] public SP_RaycastInteract previousRaycast = null;
    [HideInInspector] public SP_RaycastInteract previousClicked = null;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        CheckForRaycast();
        if (Input.GetMouseButtonDown(0))
        {
            ProcessClick();
        }
    }

    private void ProcessClick()
    {
        if (previousRaycast != null)
        {
            previousRaycast.ProcessClick();
        }    
    }


    private void CheckForRaycast()
    {
        // Cast a ray from the mouse position
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Check if the ray hits an object
        if (Physics.Raycast(ray, out hit))
        {
            // Check if the hit object has a specific tag or layer if needed
            if (hit.collider.gameObject.GetComponent<SP_RaycastInteract>())
            {
                ProcessRaycastInteract(hit.collider.gameObject.GetComponent<SP_RaycastInteract>());
            }
        }
        else
        {
            ProcessNoRaycast();
        }
    }

    private void ProcessNoRaycast()
    {
        if (previousRaycast != null)
        {
            previousRaycast.HighlightThis(false);
            previousRaycast = null;
        }
    }

    private void ProcessRaycastInteract(SP_RaycastInteract newRaycast)
    {
        if (previousRaycast == newRaycast)
            return;
        if(previousRaycast != null)
        {
            previousRaycast.HighlightThis(false);
        }
        newRaycast.HighlightThis(true);
        previousRaycast = newRaycast;
    }
}
