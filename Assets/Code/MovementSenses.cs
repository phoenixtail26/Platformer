using UnityEngine;
using System.Collections;

public class MovementSenses : MonoBehaviour 
{
	[SerializeField]
	bool _debugSenses = false;
		
	[SerializeField]
	float _ledgeGrabCheckHeight = 0.2f;
	[SerializeField]
	float _ledgeGrabCheckDistance = 0.3f;
	
	[SerializeField]
	Vector3 _footOffset = Vector3.zero;
	
	Vector3 _leftFootOffset = Vector3.zero;
	Vector3 _rightFootOffset = Vector3.zero;
	
	Transform _transform;
	
	void Start()
	{
		RecalcValues();
	}
	
	void RecalcValues()
	{
		_leftFootOffset = _footOffset;
		_leftFootOffset.x *= -1;
		_rightFootOffset = _footOffset;
	}
	
	void Update()
	{
		if ( _debugSenses )
		{
			RecalcValues();
			
			Debug.DrawLine( transform.position, transform.position + _leftFootOffset, Color.yellow );
			Debug.DrawLine( transform.position, transform.position + _rightFootOffset, Color.yellow );
		}
	}
	
}
