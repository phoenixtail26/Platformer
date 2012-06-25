using UnityEngine;
using System.Collections;

public class CameraFollow2D : MonoBehaviour 
{
	[SerializeField]
	Transform _targetTransform;
	[SerializeField]
	Vector3 _lerpSpeed = Vector3.one;
	[SerializeField]
	Vector3 _offset = Vector3.zero;
	
	Transform _transform;
		
	void Awake()
	{
		_transform = transform;
	}
	
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void LateUpdate () 
	{
		Vector3 pos = _transform.position;
		pos.x = Mathf.Lerp( pos.x, _targetTransform.position.x + _offset.x, GameTime.deltaTime * _lerpSpeed.x );
		pos.y = Mathf.Lerp( pos.y, _targetTransform.position.y + _offset.y, GameTime.deltaTime * _lerpSpeed.y );
		_transform.position = pos;
	}
}
