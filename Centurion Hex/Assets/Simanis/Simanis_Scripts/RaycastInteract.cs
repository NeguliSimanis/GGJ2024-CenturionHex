using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastInteract : MonoBehaviour
{
    public bool shouldHighlightComponent = false;
    public GameObject highlightObject;
    public Outline highlightComponent;

    private void Start()
    {
        if (!shouldHighlightComponent)
        {
            highlightObject.SetActive(false);
        }
        else
        {
            highlightComponent.enabled = false;
        }
    }

    public void ToggleHighlight()
    {
        if (shouldHighlightComponent)
        {
            highlightComponent.enabled = !highlightComponent.enabled;
            return;
        }
        highlightObject.SetActive(!highlightObject.activeInHierarchy);
    }
}
