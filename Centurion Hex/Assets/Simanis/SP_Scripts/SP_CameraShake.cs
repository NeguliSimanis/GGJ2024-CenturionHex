using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SP_CameraShake : MonoBehaviour
{

    public static SP_CameraShake instance;
    public float shakeDuration = 0.5f;
    public float shakeMagnitude = 0.7f;

    private Vector3 originalPosition;
    private float timeElapsed = 10f;
    private bool allowShake = false;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        originalPosition = transform.localPosition;
    }

    void Update()
    {
        if (timeElapsed < shakeDuration && allowShake) 
        {
            // Generate random offset within a unit sphere
            Vector3 randomOffset = Random.insideUnitSphere * shakeMagnitude;

            // Apply the offset to the camera's position
            transform.localPosition = originalPosition + randomOffset;

            // Increment time elapsed
            timeElapsed += Time.deltaTime;
        }
        else
        {
            // Reset camera position
            transform.localPosition = originalPosition;
            timeElapsed = 0f;
            allowShake = false;
        }
    }

    public void ShakeCamera()
    {
        allowShake = true;
        timeElapsed = 0f;
    }
}
