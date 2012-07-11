using UnityEngine;
using System.Collections;

public class MovementSenses : MonoBehaviour 
{
	[SerializeField]
	bool _debugSenses = false;
	
	public Vector3 handOffsetP1 = Vector3.up * 1.5f;
	public Vector3 handOffsetP2 = Vector3.up * 1.5f + Vector3.right;
	
	public Vector3 aboveHandOffset = Vector3.up * 1.2f;
	public Vector3 aboveHandOffsetP2 = Vector3.zero;
	
	public Vector3 crotchOffset = Vector3.up;
	public Vector3 crotchOffsetP2 = Vector3.zero;
	
	public Vector3 _footOffset = Vector3.zero;
	
	public Vector3 _leftFootOffset = Vector3.zero;
	public Vector3 _rightFootOffset = Vector3.zero;
	
	Transform _transform;
	
	float _leftFootDistanceToGround = 0;
	float _rightFootDistanceToGround = 0;
	
	Vector3 _groundNormal = Vector3.up;
	
	int _groundLayer = 0;
	int _groundLayerMask = 0;
	
	bool _wallAboveHandHeight = false;
	bool _wallAtHandHeight = false;
	bool _wallAtCrotchHeight = false;
	
	Vector3 _direction = Vector3.right;
	
	public Vector3 groundNormal
	{
		get { return _groundNormal; }
	}
	
	public float minFootDistanceToGround
	{
		get
		{
			return Mathf.Min(_leftFootDistanceToGround, _rightFootDistanceToGround);
		}
	}
	
	void Start()
	{
		_transform = transform;
		
		_groundLayer = LayerMask.NameToLayer("Ground");
		_groundLayerMask = 1 << _groundLayer;
		
		RecalcValues();
	}
	
	public void RecalcValues()
	{
		_leftFootOffset.z = _rightFootOffset.z = handOffsetP1.z = handOffsetP2.z = 0;
		
		aboveHandOffset.x = crotchOffset.x = handOffsetP1.x;
		
		aboveHandOffsetP2 = aboveHandOffset;
		aboveHandOffsetP2.x = handOffsetP2.x;
		
		crotchOffsetP2 = crotchOffset;
		crotchOffsetP2.x = handOffsetP2.x;
		
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
	
	public void UpdateSenses( Vector3 facingDirection )
	{
		_direction = facingDirection;
		UpdateFootDistanceToGround();
		UpdateWallChecks();
	}
	
	void UpdateWallChecks()
	{
		
	}
	
	void UpdateFootDistanceToGround()
	{
		RaycastHit info;
		
		Vector3 leftFootNormal = Vector3.up;
		Vector3 rightFootNormal = Vector3.up;
		
		if ( Physics.Raycast( _transform.position + _leftFootOffset, Vector3.down, out info, Mathf.Infinity, _groundLayerMask ) )
		{
			_leftFootDistanceToGround = info.distance;
			leftFootNormal = info.normal;
		}
		else
		{
			_leftFootDistanceToGround = Mathf.Infinity;
		}
		
		if ( Physics.Raycast( _transform.position + _rightFootOffset, Vector3.down, out info, Mathf.Infinity, _groundLayerMask ) )
		{
			_rightFootDistanceToGround = info.distance;
			rightFootNormal = info.normal;
		}
		else
		{
			_rightFootDistanceToGround = Mathf.Infinity;
		}
		
		if ( _leftFootDistanceToGround < _rightFootDistanceToGround )
		{
			_groundNormal = leftFootNormal;
		}
		else
		{
			_groundNormal = rightFootNormal;
		}
	}
	
}
