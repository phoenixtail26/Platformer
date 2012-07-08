using UnityEngine;
using System.Collections;
using Framework;

public class PlayerMovementController : MonoBehaviour 
{
	[SerializeField]
	float _runSpeed = 4;
	[SerializeField]
	float _runAccel = 10;
	[SerializeField]
	float _decel = 15;
	[SerializeField]
	float _runDirChangeDecel = 100;
	[SerializeField]
	float _jumpSpeed = 6;
	[SerializeField]
	float _shortJumpSpeed = 4;
	[SerializeField]
	float _airAccel = 20;
	[SerializeField]
	float _airDrag = 0.96875f;
	
	[SerializeField]
	float _gravity = -0.5f;
	
	[SerializeField]
	float _ledgeGrabCheckHeight = 0.2f;
	[SerializeField]
	float _ledgeGrabCheckDistance = 0.3f;
		
	Rigidbody _rigidbody;
	Transform _transform;
	Collider _collider;
	Bounds _bounds;
	
	Vector3 _moveVel = new Vector3();
	
	bool _jumpPressed = false;
	bool _runPressed = false;
	bool _onGround = false;
	
	int _groundLayerMask = 0;
	int _groundLayer = 0;
	
	GameTimer _onGroundTimer = new GameTimer(0.025f);
	GameTimer _inAirTimer = new GameTimer(0.025f);
	
	Vector3 _leftFootOffset = Vector3.zero;
	Vector3 _rightFootOffset = Vector3.zero;
	
	Vector3 _handOffset = Vector3.zero;
	
	Vector3 _lastPosition = Vector3.zero;
	
	Vector3 _direction = Vector3.right;
	
	FSM _movementState = new FSM();
		
	Vector2 _inputVector = Vector3.zero;
	
	Vector3 _targetPosition = Vector3.zero;
	
	float _leftFootDistanceToGround = 0;
	float _rightFootDistanceToGround = 0;
	
	bool _jumpWhenPossible = false;
	
	Vector3 _groundNormal = Vector3.up;
	
	public bool inAir
	{
		get
		{
			return _inAirTimer.hasFinished;
		}
	}
	
	public bool onGround
	{
		get 
		{ 
			return _onGroundTimer.hasFinished;
		}
	}
	
	public Vector3 facingDirection
	{
		get 
		{
			return _direction;
		}
	}
	
	public float minFootDistanceToGround
	{
		get
		{
			return Mathf.Min(_leftFootDistanceToGround, _rightFootDistanceToGround);
		}
	}
	
	public bool inLedgeGrab
	{
		get
		{
			return (_movementState.currentState == "LedgeGrab");
		}
	}
	
	void Awake()
	{
		_groundLayer = LayerMask.NameToLayer("Ground");
		_groundLayerMask = 1 << _groundLayer;
		_transform = transform;
		_rigidbody = rigidbody;
		_collider = collider;
		_bounds = _collider.bounds;
		
		_leftFootOffset.x = -_bounds.extents.x + 0.1f;
		_leftFootOffset.y = 0.05f;
		
		Debug.DrawLine(_transform.position, _transform.position + _leftFootOffset, Color.white, 10000);
		
		_rightFootOffset.x = _bounds.extents.x - 0.1f;
		_rightFootOffset.y = 0.05f;
		
		Debug.DrawLine(_transform.position, _transform.position + _rightFootOffset, Color.white, 10000);
		
		_handOffset.x = _bounds.extents.x - 0.1f;
		_handOffset.y = _bounds.extents.y * 2 - 0.05f;
		
		_movementState.AddState( "OnGround", OnGroundUpdate );
		_movementState.AddState( "InAir", InAirUpdate );
		_movementState.AddState( "LedgeGrab", LedgeGrabUpdate );
		_movementState.AddState( "ClimbingLedge", LedgeClimbUpdate );
		_movementState.SetState( "OnGround" );
		
		_lastPosition = _transform.position;
	}
	
	// Use this for initialization
	void Start () 
	{
		_onGroundTimer.Reset();
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		_moveVel = _rigidbody.velocity;
		
		/*if ( _moveVel.x != 0 )
		{
			_direction = _moveVel;
			_direction.y = 0;
			_direction.Normalize();
		}*/
		
		//Debug.Log(_inputVector.x);
		_movementState.Update(GameTime.deltaTime);
				
		// Determine if the player is on the ground
		/*if ( _onGroundTimer.hasFinished )
		{
			_onGround = true;
		}
		else
		{
			_onGround = false;
		}*/
		
		_runPressed = false;
		_inputVector.x = 0;
		
		UpdateFootDistanceToGround();
		
		//Debug.Log(_inAirTimer.GetTimeRemaining());
		
		_rigidbody.velocity = _moveVel;		
		_lastPosition = _transform.position;
	}
	
	void OnGroundUpdate( float timeDelta )
	{
		//Debug.Log(_inputVector.x);
		
		UpdateDirection();
		
		// Apply running acceleration
		float accelVal = _runAccel;
		// if moving in opposite direction to input
		if ( (_inputVector.x * _moveVel.x) < 0 )
		{
			accelVal = _runDirChangeDecel;
		}
		
	/*	Vector3 forwardDir = new Vector3(-_groundNormal.y, _groundNormal.x, 0);
		forwardDir *= -_direction.x;
		Debug.DrawLine(_transform.position, _transform.position + forwardDir, Color.cyan);*/
		
		_moveVel.x += (_inputVector.x * accelVal) * timeDelta;
		//_moveVel += forwardDir * (_inputVector.x * accelVal) * timeDelta;
		_moveVel.x = Mathf.Clamp(_moveVel.x,-_runSpeed, _runSpeed);
		
		
		
		//Debug.Log(_inputVector.x);
		
		DecelIfNoInput( timeDelta );
		
		//Debug.Log(_moveVel.x);
		
		// Apply gravity
		_moveVel += -_groundNormal * (_gravity * timeDelta);
		
		// Check that one foot is on the ground
		if ( !IsOneFootOnTheGround() )
		{
			_onGroundTimer.Reset();
		}
		
		if ( !onGround )
		{
			_movementState.SetState("InAir");
		}
		
		//Debug.DrawLine(_transform.position, _transform.position + _groundNormal, Color.white );
		
		_inAirTimer.Reset();
		CheckForPossibleJump();
	}
	
	void UpdateDirection()
	{
		if ( _inputVector.x != 0 )
		{
			_direction.x = _inputVector.x;
			_direction.y = 0;
			_direction.Normalize();
		}
	}
	
	void DecelIfNoInput( float timeDelta )
	{
		// if not running, then start to decelerate
		if ( _inputVector.x == 0 )
		{
			float accelVal = _decel;
			
			if ( Mathf.Abs(_moveVel.x) > accelVal * timeDelta )
			{
				if ( _moveVel.x > 0 )
				{
					accelVal *= -1;
				}
				_moveVel.x += accelVal * timeDelta;
			}
			else
			{
				_moveVel.x = 0;
			}
		}
	}
	
	void InAirUpdate( float timeDelta )
	{
		UpdateDirection();
		
		float accelVal = _airAccel;
		_moveVel.x += (_inputVector.x * accelVal) * GameTime.deltaTime;
		_moveVel.x = Mathf.Clamp(_moveVel.x,-_runSpeed, _runSpeed);
		
		Color debugColor = Color.red;
		
		// Apply air drag
		if ( _moveVel.y < _shortJumpSpeed && _moveVel.y > 0 )
		{
			if ( Mathf.Abs(_moveVel.x) > 0.125f )
			{
				debugColor = Color.green;
				_moveVel.x *= _airDrag;
			}
		}
		
		DecelIfNoInput( timeDelta );
		
		
		// Apply gravity
		_moveVel.y += -_gravity * GameTime.deltaTime;
		
		// Make sure the player doesn't fall too fast
		_moveVel.y = Mathf.Clamp(_moveVel.y, -16, 16 );
		
		if ( !onGround )
		{
			Debug.DrawLine(_lastPosition, _transform.position, debugColor, 10000);
		}
		
		// Falling, so check for ledges to grab hold of
		if ( _inputVector.y >= 0 )// _moveVel.y <= 0 && _inputVector.y >= 0 )
		{
			Vector3 ledgePosition = Vector3.zero;
			if ( CheckForLedgeGrab( ref ledgePosition ) )
			{
				// Make sure that there isn't ground close below the ledge we're about to grab
				if ( !Physics.Raycast( _transform.position + Vector3.up * 0.25f, Vector3.down, 0.75f, _groundLayerMask ) )
				{
					Vector3 newPos = _transform.position;
					
					newPos.x = ledgePosition.x - (_direction.x * _bounds.extents.x);
					newPos.y = ledgePosition.y - _bounds.extents.y * 2;
					
					_targetPosition = newPos;
					
					_movementState.SetState( "LedgeGrab" );
					_moveVel.y = 0;
					_moveVel.x = 0;
					
					_jumpPressed = false;
				}
			}
		}
		
		_inAirTimer.Update(timeDelta);
		
		if ( onGround )
		{
			// if the player isn't trying to move when landing, decelerate quickly to help them stick landings easier
			if ( _inputVector.x == 0 )
			{
				_moveVel.x *= 0.5f;
			}
			_movementState.SetState( "OnGround" );
		}
		
		CheckForPossibleJump();
	}
	
	void CheckForPossibleJump()
	{
		if ( onGround || IsOneFootOnTheGround() )
		{
			if ( _jumpWhenPossible )
			{
				_jumpWhenPossible = false;
				ExecuteJump();
			}
		}
	}
	
	void LedgeGrabUpdate( float timeDelta )
	{
		_moveVel.y = 0;
		_moveVel.x = 0;
		
		if ( _jumpPressed )
		{
			// if pulling away from ledge
			if ( ((_inputVector.x * _direction.x) < 0)  || (_inputVector.y < 0) )
			{
				// jump away from ledge
				UpdateDirection();
				
				float yVal = (_inputVector.y + 1) / 2.0f;
				
				_moveVel.x = Mathf.Lerp(0,_direction.x * _runSpeed, Mathf.Abs(_inputVector.x));
				_moveVel.y = Mathf.Lerp(0, _jumpSpeed, yVal);
				
				_movementState.SetState( "InAir");
			}
			// if pulling down
			else if ( _inputVector.y < 0 )
			{
				_movementState.SetState( "InAir");
			}
			else
			{
				_targetPosition = _transform.position;
				_targetPosition.x += _direction.x * 1;
				_targetPosition.y += 2;
				_movementState.SetState("ClimbingLedge");
			}	
			
		}
		else
		{
			Vector3 pos = _transform.position;
			pos = Vector3.Lerp( pos, _targetPosition, timeDelta * 30 );
			_transform.position = pos;
		}
		
	}
	
	void LedgeClimbUpdate( float timeDelta )
	{
		Vector3 pos = _transform.position;
		if ( Mathf.Abs(_targetPosition.y - pos.y) > 0.1f )
		{
			pos.y = Mathf.Lerp(pos.y, _targetPosition.y, 15 * timeDelta);
		}
		else
		{
			pos.x = Mathf.Lerp(pos.x, _targetPosition.x, 15 * timeDelta);
		}
		
		_transform.position = pos;
		
		if ( (_transform.position - _targetPosition).magnitude < 0.1f )
		{	
			// if the player is trying to run at the end of the climb, give them a velocity head start
			if ( _runPressed )
			{
				_moveVel.x = _runSpeed * 0.5f * _direction.x;
			}
			_transform.position = _targetPosition;
			_movementState.SetState("OnGround");
		}
	}
	
	public bool CheckForLedgeGrab( ref Vector3 ledgePosition )
	{
		Vector3 offset = _handOffset;
		
		if ( _direction.x < 0 )
		{
			offset.x *= -1;
		}
		
		Vector3 handPos = _transform.position + offset - Vector3.up * _ledgeGrabCheckHeight / 2.0f;
		Debug.DrawLine( handPos, handPos + _direction, Color.magenta, 10);
		
		// if there's a wall at hand height
		RaycastHit info;
		if ( Physics.Raycast( handPos, _direction, out info, _ledgeGrabCheckDistance, _groundLayerMask ) )
		{
			Vector3 aboveHandPos = _transform.position + offset + Vector3.up * _ledgeGrabCheckHeight / 2.0f;
			Debug.DrawLine(aboveHandPos, aboveHandPos + _direction, Color.cyan, 10);
			// if there's no wall just above the hand height
			if ( !Physics.Raycast( aboveHandPos, _direction, _ledgeGrabCheckDistance, _groundLayerMask ) )
			{
				
				Vector3 newPos = _transform.position;
					
				newPos.x = info.point.x;
				Vector3 testPos = aboveHandPos;
				testPos.x = newPos.x + _direction.x * 0.05f;
				
				if ( Physics.Raycast( testPos, Vector3.down, out info, _ledgeGrabCheckHeight, _groundLayerMask ) )
				{
					newPos.y = info.point.y;
				}
				else
				{
					Debug.LogWarning("Couldn't find Y position of ledge");
				}
				
				ledgePosition = newPos;
				
				// then there's a ledge to grab
				return true;
			}
		}
		
		return false;
	}
	
	public void Move( Vector2 inputVec )
	{
		_inputVector = inputVec;
		if ( _inputVector.x != 0 )
		{
			_runPressed = true;
		}
	}
	
	public void Stop()
	{
		_rigidbody.velocity = Vector3.zero;
	}
	
	void ExecuteJump()
	{
		_moveVel = _rigidbody.velocity;
		_moveVel.y = _jumpSpeed;
		_rigidbody.velocity = _moveVel;
		_onGroundTimer.Reset();
		_inAirTimer.hasFinished = true;
	}
	
	public void StartJump()
	{
		_jumpPressed = true;
		
		if ( _movementState.currentState == "InAir" || _movementState.currentState == "OnGround" )
		{
			if ( !inAir )
			{
				ExecuteJump();
			}
			else if ( onGround || IsOneFootOnTheGround() )
			{
				ExecuteJump();
			}
			else if ( !onGround && minFootDistanceToGround < 0.75f )	// if close to ground, jump when on ground
			{
				_jumpWhenPossible = true;
			}
		}
	}
	
	public void EndJump()
	{
		_jumpPressed = false;
		_jumpWhenPossible = false;
		
		_moveVel = _rigidbody.velocity;
		if ( _moveVel.y > _shortJumpSpeed )
		{
			_moveVel.y = _shortJumpSpeed;
		}
		_rigidbody.velocity = _moveVel;
	}
	
	void UpdateFootDistanceToGround()
	{
		RaycastHit info;
		Vector3 normal = Vector3.zero;
		
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
	
	bool IsOneFootOnTheGround()
	{
		/*bool oneFootOnTheGround = false;
		
		Debug.DrawLine( _transform.position + _leftFootOffset, _transform.position + _leftFootOffset + Vector3.down * 0.3f, Color.white, 10 );
		
		if ( Physics.Raycast( _transform.position + _leftFootOffset, Vector3.down, 0.3f, _groundLayerMask ) )
		{
			oneFootOnTheGround = true;
		}
		
		Debug.DrawLine( _transform.position + _rightFootOffset, _transform.position + _rightFootOffset + Vector3.down * 0.3f, Color.white, 10 );
		
		if ( Physics.Raycast( _transform.position + _rightFootOffset, Vector3.down, 0.3f, _groundLayerMask ) )
		{
			oneFootOnTheGround = true;
		}
		return oneFootOnTheGround;*/
		
		if ( minFootDistanceToGround < 0.3f )
		{
			return true;
		}
		
		return false;
		
	}
	
	void OnCollisionEnter( Collision other )
	{
		if ( other.gameObject.layer == _groundLayer )
		{
			if ( IsOneFootOnTheGround() )
			{
				_onGroundTimer.Update(GameTime.deltaTime);
				_inAirTimer.Reset();
			}
		}
	}
	
	void OnCollisionStay( Collision other )
	{
		if ( other.gameObject.layer == _groundLayer )
		{
			if ( IsOneFootOnTheGround() )
			{
				_onGroundTimer.Update(GameTime.deltaTime);
				//_inAirTimer.Reset();
			}
		}
	}
}
