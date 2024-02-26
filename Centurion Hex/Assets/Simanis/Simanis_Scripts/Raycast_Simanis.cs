using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycast_Simanis : MonoBehaviour
{
    CenturionGame centurionGame;
    public HUD_Simanis hudManager;
    private Camera mainCamera;

    private void Start()
    {
        hudManager = GetComponent<HUD_Simanis>();
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
            // Check if the hit object has a specific tag or layer if needed
            if (hit.collider.gameObject.GetComponent<RaycastInteract>())
            {
                // Object is being hovered
                if (CenturionGame.Instance.mRoundState == CenturionGame.RoundState.rsMovingCharacters)
                {
                    hudManager.UpdateMovementPhaseHighlight(hit.collider.gameObject.GetComponent<RaycastInteract>());
                }
                else if (CenturionGame.Instance.mRoundState == CenturionGame.RoundState.rsManagement)
                {
                    hudManager.UpdateManagementPhaseHighlights(hit.collider.gameObject.GetComponent<RaycastInteract>());
                }
            }
        }
        else
        {
            // Mouse is not hovering over any object
            // You can perform additional actions here if needed
        }
    }
}
