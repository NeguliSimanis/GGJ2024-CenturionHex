using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct_Simanis : MonoBehaviour
{

    public bool allowSelfDestruct = true;
    public void DestroySelf()
    {
        if (allowSelfDestruct)
        Destroy(gameObject);
    }
}
