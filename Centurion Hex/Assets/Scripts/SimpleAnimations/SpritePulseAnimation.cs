using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class SpritePulseAnimation : MonoBehaviour
{
	private SpriteRenderer _spriteRenderer;
	[SerializeField]
	private Color _color = Color.white;

	[SerializeField]
	private float _speed = 1;

	private void OnEnable()
	{
		TryGetComponent(out _spriteRenderer);
	}
	private void Update()
	{
		float a = Mathf.Abs(Mathf.Sin(Time.time / Mathf.PI * _speed));
		_spriteRenderer.color = new(_color.r, _color.g, _color.b, a * a);
	}
}
