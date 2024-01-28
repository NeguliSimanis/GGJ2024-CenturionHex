using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobAndRotate : MonoBehaviour
{
	private Transform _transform;
	private Vector3 _initialPosition;
	[SerializeField]
	private float _bobSpeed = 1f;
	[SerializeField]
	private float _bobAmpletude = .1f;
	private Quaternion _initialRotation;
	[SerializeField]
	private float _roatationSpeed = 5f;

	private void OnEnable()
    {
        _transform = transform;
		_initialPosition = _transform.localPosition;
		_initialRotation = _transform.localRotation;
    }
	private void Update()
	{
		_transform.localPosition = _initialPosition + Mathf.Sin(Time.time * _bobSpeed) * _bobAmpletude * Vector3.up;
		_transform.localRotation = _initialRotation * Quaternion.AngleAxis(Time.time * _roatationSpeed, Vector3.up);
	}
}
