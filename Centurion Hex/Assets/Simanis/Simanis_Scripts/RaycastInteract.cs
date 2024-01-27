using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastInteract : MonoBehaviour
{
    public GameObject highlight;

    private void Start()
    {
        highlight.SetActive(false);
    }

    public void ToggleHighlight()
    {
        highlight.SetActive(!highlight.activeInHierarchy);
    }
}
