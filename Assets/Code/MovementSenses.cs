using UnityEngine;
using System.Collections;

public class MovementSenses : MonoBehaviour 
{
	[System.Serializable]
	public class SenseOffsets
	{
		public Vector3 handOffsetP1 = Vector3.up * 1.5f;
		public Vector3 handOffsetP2 = Vector3.up * 1.5f + Vector3.right;
		
		public Vector3 aboveHandOffset = Vector3.up * 1.2f;
		public Vector3 aboveHandOffsetP2 = Vector3.zero;
		
		public Vector3 crotchOffset = Vector3.up;
		public Vector3 crotchOffsetP2 = Vector3.zero;
		
		public Vector3 footOffset = Vector3.zero;
		public Vector3 groundCheckOffset = Vector3.zero;
		
		public Vector3 leftFootOffset = Vector3.zero;
		public Vector3 rightFootOffset = Vector3.zero;
		
		public Vector3 leftFootGroundCheck = Vector3.zero;
		public Vector3 rightFootGroundCheck = Vector3.zero;
		
		public void RecalcValues()
		{
			leftFootOffset.z = rightFootOffset.z = handOffsetP1.z = handOffsetP2.z = 0;
			
			aboveHandOffset.x = crotchOffset.x = handOffsetP1.x;
			
			aboveHandOffsetP2 = aboveHandOffset;
			aboveHandOffsetP2.x = handOffsetP2.x;
			
			crotchOffsetP2 = crotchOffset;
			crotchOffsetP2.x = handOffsetP2.x;
			
			leftFootOffset = footOffset;
			leftFootOffset.x *= -1;
			rightFootOffset = footOffset;
			
			leftFootGroundCheck = groundCheckOffset;
			leftFootGroundCheck.x *= -1;
			rightFootGroundCheck = groundCheckOffset;
		}
	}
	
	[SerializeField]
	bool _debugSenses = false;
	
	public SenseOffsets offsets = new SenseOffsets();
		
	Transform _transform;
	
	float _leftFootDistanceToGround = 0;
	Vector3 _leftFootHitPoint = Vector3.zero;
	float _rightFootDistanceToGround = 0;
	Vector3 _rightFootHitPoint = Vector3.zero;
	
	Vector3 _groundNormal = Vector3.up;
	
	int _groundLayer = 0;
	int _groundLayerMask = 0;
	
	bool _wallAboveHandHeight = false;
	bool _wallAtHandHeight = false;
	bool _wallAtCrotchHeight = false;
	
	Vector3 _direction = Vector3.right;
	
	float _ledgeGrabDistance = 0;
	
	Vector3 _ledgePosition = Vector3.zero;
	
	bool _groundInFrontOfLeftFoot = false;
	bool _groundInFrontOfRightFoot = false;
	
	float _groundCheckDistance = 0;
	
	Vector3 _groundLedgePosition = Vector3.zero;
#region Accessors
	
	public bool isWallAtHandHeight
	{
		get { return _wallAtHandHeight; }
	}
	
	public bool isWallAboveHandHeight
	{
		get { return _wallAboveHandHeight; }
	}
	
	public bool isWallAtCrotchHeight
	{
		get { return _wallAtCrotchHeight; }
	}
	
	public Vector3 ledgePosition
	{
		get { return _ledgePosition; }
	}
		
	public Vector3 groundLedgePosition
	{
		get { return _groundLedgePosition; }
	}
	
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
	
	public bool isGroundInFrontOfLeftFoot
	{
		get { return _groundInFrontOfLeftFoot; }
	}
	
	public bool isGroundInFrontOfRightFoot
	{
		get { return _groundInFrontOfRightFoot; }
	}
	
#endregion
	
	void Start()
	{
		_transform = transform;
		
		_groundLayer = LayerMask.NameToLayer("Ground");
		_groundLayerMask = 1 << _groundLayer;
		
		RecalcValues();
	}
	
	public void RecalcValues()
	{
		offsets.RecalcValues();
		_ledgeGrabDistance = (offsets.handOffsetP2 - offsets.handOffsetP1).magnitude;
		_groundCheckDistance = (offsets.footOffset - offsets.groundCheckOffset).magnitude;
	}
	
	void Update()
	{
		if ( _debugSenses )
		{
			RecalcValues();
			
			Debug.DrawLine( transform.position, transform.position + offsets.leftFootOffset, Color.yellow );
			Debug.DrawLine( transform.position, transform.position + offsets.rightFootOffset, Color.yellow );
		}
	}
	
	public void UpdateSenses( Vector3 facingDirection )
	{
		_direction = facingDirection;
		UpdateFootDistanceToGround();
		UpdateWallChecks();
	}
	
	public void UpdateFloorChecks( float playerWidth )
	{
		_groundInFrontOfLeftFoot = false;
		_groundInFrontOfRightFoot = false;
		
		Ray ray = new Ray( _transform.position + offsets.leftFootOffset, offsets.leftFootGroundCheck - offsets.leftFootOffset );
		if ( Physics.Raycast( ray, _groundCheckDistance, _groundLayerMask ) )
		{
			_groundInFrontOfLeftFoot = true;
		}
		/*else
		{
			ray = new Ray( _transform.position + offsets.leftFootGroundCheck, Vector3.right );
			RaycastHit info;
			if ( Physics.Raycast( ray, out info, _groundCheckDistance + playerWidth, _groundLayerMask ) )
			{
				Vector3 newPos = _rightFootHitPoint;
				
				newPos.x = info.point.x;
				
				_groundLedgePosition = newPos;
			}
			else
			{
				Debug.LogWarning("Couldn't find X position of ledge");
			}
		}*/
		
		ray = new Ray( _transform.position + offsets.rightFootOffset, offsets.rightFootGroundCheck - offsets.rightFootOffset );
		if ( Physics.Raycast( ray, _groundCheckDistance, _groundLayerMask ) )
		{
			_groundInFrontOfRightFoot = true;
		}
		/*else
		{
			ray = new Ray( _transform.position + offsets.rightFootGroundCheck, Vector3.left );
			RaycastHit info;
			if ( Physics.Raycast( ray, out info, _groundCheckDistance + playerWidth, _groundLayerMask ) )
			{
				Vector3 newPos = _leftFootHitPoint;
				
				newPos.x = info.point.x;
				
				_groundLedgePosition = newPos;
			}
			else
			{
				Debug.LogWarning("Couldn't find X position of ledge");
			}
		}*/
		
		if ( !_groundInFrontOfLeftFoot || !_groundInFrontOfRightFoot )
		//if (( _groundInFrontOfLeftFoot && !_groundInFrontOfRightFoot ) || ( !_groundInFrontOfLeftFoot && _groundInFrontOfRightFoot ) )
		{
			bool doLeftFoot = !_groundInFrontOfLeftFoot;
			if ( !_groundInFrontOfLeftFoot && !_groundInFrontOfRightFoot )
			{
				if ( _direction.x < 0 )
				{
					doLeftFoot = false;
				}
			}
			
			if ( doLeftFoot )
			{
				ray = new Ray( _transform.position + offsets.leftFootGroundCheck, Vector3.right );
				RaycastHit info;
				if ( Physics.Raycast( ray, out info, _groundCheckDistance + playerWidth, _groundLayerMask ) )
				{
					Vector3 newPos = _transform.position;
					
					newPos.x = info.point.x;
					
					_groundLedgePosition = newPos;
				}
				else
				{
					//Debug.LogWarning("Couldn't find X position of ledge (LEFT)");
				}
			}
			else
			{
				ray = new Ray( _transform.position + offsets.rightFootGroundCheck, Vector3.left );
				RaycastHit info;
				if ( Physics.Raycast( ray, out info, _groundCheckDistance + playerWidth, _groundLayerMask ) )
				{
					Vector3 newPos = _transform.position;
					
					newPos.x = info.point.x;
					
					_groundLedgePosition = newPos;
				}
				else
				{
					//Debug.LogWarning("Couldn't find X position of ledge (RIGHT)");
				}
			}
		}
	}
	
	void UpdateWallChecks()
	{
		_wallAboveHandHeight = false;
		_wallAtHandHeight = false;
		_wallAtCrotchHeight = false;
		
		Vector3 offset = offsets.handOffsetP1;
		offset.x *= _direction.x;
		
		Vector3 checkDirection = (offsets.handOffsetP2 - offsets.handOffsetP1) * _direction.x;
		
		Ray handRay = new Ray( _transform.position + offset, checkDirection );
		// if there's a wall at hand height
		
		Debug.DrawLine( handRay.origin, handRay.origin + handRay.direction * _ledgeGrabDistance, Color.red );
		
		RaycastHit info;
		if ( Physics.Raycast( handRay, out info, _ledgeGrabDistance, _groundLayerMask ) )
		{
			_wallAtHandHeight = true;
			
			offset = offsets.aboveHandOffset;
			offset.x *= _direction.x;
			
			Ray aboveHandRay = new Ray( _transform.position + offset, checkDirection );
			
			Debug.DrawLine( aboveHandRay.origin, aboveHandRay.origin + aboveHandRay.direction * _ledgeGrabDistance, Color.cyan );
			if ( !Physics.Raycast( aboveHandRay, out info, _ledgeGrabDistance, _groundLayerMask ) )
			{
				Vector3 newPos = _transform.position;
					
				newPos.x = info.point.x;
				Vector3 testPos = aboveHandRay.origin;
				testPos.x = newPos.x + _direction.x * 0.05f;
				
				if ( Physics.Raycast( testPos, Vector3.down, out info, offsets.aboveHandOffset.y - offsets.handOffsetP1.y, _groundLayerMask ) )
				{
					newPos.y = info.point.y;
				}
				else
				{
					Debug.LogWarning("Couldn't find Y position of ledge");
				}
				
				_ledgePosition = newPos;
			}
			else
			{
				_wallAboveHandHeight = true;
			}
			
			offset = offsets.crotchOffset;
			offset.x *= _direction.x;
			
			Ray crotchRay = new Ray( _transform.position + offset, checkDirection );
			Debug.DrawLine( crotchRay.origin, crotchRay.origin + crotchRay.direction * _ledgeGrabDistance, Color.magenta );
			if ( Physics.Raycast( crotchRay, out info, _ledgeGrabDistance, _groundLayerMask ) )
			{
				_wallAtCrotchHeight = true;
			}
		}
		else
		{
			_wallAtHandHeight = false;
		}
	}
	
	void UpdateFootDistanceToGround()
	{
		RaycastHit info;
		
		Vector3 leftFootNormal = Vector3.up;
		Vector3 rightFootNormal = Vector3.up;
		
		if ( Physics.Raycast( _transform.position + offsets.leftFootOffset, Vector3.down, out info, Mathf.Infinity, _groundLayerMask ) )
		{
			_leftFootDistanceToGround = info.distance;
			leftFootNormal = info.normal;
			_leftFootHitPoint = info.point;
		}
		else
		{
			_leftFootDistanceToGround = Mathf.Infinity;
		}
		
		if ( Physics.Raycast( _transform.position + offsets.rightFootOffset, Vector3.down, out info, Mathf.Infinity, _groundLayerMask ) )
		{
			_rightFootDistanceToGround = info.distance;
			rightFootNormal = info.normal;
			_rightFootHitPoint = info.point;
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
