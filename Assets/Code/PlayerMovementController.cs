using UnityEngine;
using System.Collections;
using Framework;

[RequireComponent(typeof(MovementSenses))]
public class PlayerMovementController : MovementController 
{
	[SerializeField]
	float _wallClimbSpeed = 2;
	[SerializeField]
	float _wallClimbSlideSpeed = 4;
	
	[SerializeField]
	float _wallSlideSpeed = 2;
	[SerializeField]
	float _wallRunGravityFactor = 0.75f;
	
	[SerializeField]
	float _doubleJumpFactor = 0.75f;
	
	[SerializeField]
	float _duckColliderHeight = 1;
	[SerializeField]
	float _duckMovementFactor = 0.5f;
	
	[SerializeField]
	float _dashingSpeed = 10;
	
	[SerializeField]
	float _dashingTime = 1;
	
	GameTimer _dashingTimer;
	
	bool _ableToDash = false;
	
	Vector3 _targetPosition = Vector3.zero;
	
	// OnWall variables
	bool _onWall = false;
	Vector3 _onWallNormal = Vector3.zero;
	GameTimer _letGoOfWallTimer = new GameTimer(0.2f);
	bool _ableToWallJump = false;
	
	GameTimer _inputDelayTimer = new GameTimer(0.5f);
	bool _delayInput = false;
	
	bool _readyForDoubleJump = false;
	bool _duckPressed = false;
	bool _duckHeld = false;	
	
	float _startColliderHeight = 2;
	
	Abilities _abilities = new Abilities();
	
	#region Accessors
	public bool inLedgeGrab
	{
		get { return (_movementState.currentState == "LedgeGrab"); }
	}
	
	public bool inWallSlide
	{
		get { return (_movementState.currentState == "OnWall"); }
	}
	
	public bool inWallClimb
	{
		get { return (_movementState.currentState == "OnClimbableWall"); }
	}
	
	public bool isCrouching
	{
		get { return (_movementState.currentState == "Crouching"); }
	}
	
	public Abilities playerAbilities
	{
		get { return _abilities; }
		set { _abilities = value; }
	}
	#endregion
	
	public override void Awake()
	{
		base.Awake();
		
		_startColliderHeight = _bounds.extents.y;
		
		_movementState.AddState( "LedgeGrab", LedgeGrabUpdate );
		_movementState.AddState( "ClimbingLedge", LedgeClimbUpdate );
		_movementState.AddState( "ClimbingDownLedge", LedgeClimbDownUpdate );
		_movementState.AddState( "OnWall", OnWallUpdate );
		_movementState.AddState( "OnClimbableWall", OnClimbableWallUpdate );
		_movementState.AddState( "Crouching", CrouchingUpdate );
		_movementState.AddState( "Dashing", DashingUpdate );
		
		_dashingTimer = new GameTimer(_dashingTime);
	}
	
	// Update is called once per frame
	public override void FixedUpdate () 
	{
		base.FixedUpdate();
		
		_onWall = _senses.isWallAtHandHeight || _senses.isWallAboveHandHeight || _senses.isWallAtCrotchHeight;
		_onWallNormal = _senses.wallNormal;
	}
	
	void Update()
	{
		if ( _delayInput )
		{
			if ( _inputDelayTimer.Update(GameTime.deltaTime) || !inAir || onGround )
			{
				_delayInput = false;
			}
		}
		
		if ( !_delayInput )
		{
			_runPressed = false;
			_inputVector.x = 0;
		}
		
		_duckPressed = false;
	}
	
	protected override void OnGroundUpdate( float timeDelta )
	{
		base.OnGroundUpdate(timeDelta);
		
		if ( !onGround )
		{
			if ( _onWall && _senses.isWallAtHandHeight )
			{
				if ( _senses.wallTypeAtHand == WallType.Climbable && _abilities.wallClingEnabled )
				{
					_movementState.SetState("OnClimbableWall");
				}
				else if ( (_senses.wallTypeAtHand == WallType.Normal || _senses.wallTypeAtHand == WallType.Climbable) && _abilities.wallJumpEnabled )
				{
					_movementState.SetState("OnWall");
				}
				else
				{
					_movementState.SetState("InAir");
				}
			}
			else
			{
				_movementState.SetState("InAir");
			}
		}
		
		// Look for climb down
		if ( _duckPressed )
		{
			if ( CheckForClimbDown() )
			{
				_movementState.SetState("ClimbingDownLedge");
				//_movementState.SetState("Nothing");
				_moveVel.y = 0;
				_moveVel.x = 0;
				
				_jumpPressed = false;
				_jumpWhenPossible = false;
				
				//Debug.LogError("stop");
				//_jumpPressed = false;
				//_jumpWhenPossible = false;

				return;
			}
		}	
		
		if ( _duckHeld )
		{
			_movementState.SetState( "Crouching" );
		}
		
		// player pressing up while against climbable wall
		if ( _inputVector.y > 0 && _onWall && _senses.isWallAtHandHeight && _senses.wallTypeAtHand == WallType.Climbable && _abilities.wallClingEnabled )
		{
			_movementState.SetState("OnClimbableWall");
		}
		
		_ableToDash = true;
	}
	
	bool CheckForClimbDown()
	{
		if ( !_senses.isGroundInFrontOfLeftFoot || !_senses.isGroundInFrontOfRightFoot )
		{
			//Debug.Log("climb down");
			
			Vector3 offset = _senses.offsets.rightFootGroundCheck;
			if ( !_senses.isGroundInFrontOfLeftFoot )
			{
				offset = _senses.offsets.leftFootGroundCheck;
			}
			
			// If both check fail, climb down in the direction the player is facing
			if ( !_senses.isGroundInFrontOfLeftFoot && ! _senses.isGroundInFrontOfRightFoot )
			{
				offset = _senses.offsets.leftFootGroundCheck;
				
				if ( _direction.x < 0 )
				{
					offset = _senses.offsets.rightFootGroundCheck;
				}
			}
			
			//Debug.DrawLine( _transform.position + _senses.offsets.rightFootGroundCheck, _transform.position + _senses.offsets.rightFootGroundCheck + Vector3.down * 0.75f, Color.black, 1000);
			// Make sure that there isn't ground close below the ledge we're about to climb down onto
			if ( !Physics.Raycast( _transform.position + offset, Vector3.down, 2f, _groundLayerMask ) )
			{
				// Turn player's back to ledge
				if ( _senses.isGroundInFrontOfRightFoot || _senses.isGroundInFrontOfLeftFoot ) // Don't turn around if both feet checks fail
				{
					_inputVector.x = Vector3.right.x;
					if ( !_senses.isGroundInFrontOfRightFoot )
					{
						_inputVector.x = Vector3.left.x;
					}
					UpdateDirection();
				}
				
				// Position player at ledge edge
				Vector3 newPos = _transform.position;
				Vector3 ledgePosition = _senses.groundLedgePosition;
				newPos.x = ledgePosition.x + _direction.x * _bounds.extents.x;
				//_transform.position = newPos;
				
				// Calculate target position
				newPos = _transform.position;
				newPos.x = ledgePosition.x - (_direction.x * _bounds.extents.x);
				newPos.y = ledgePosition.y - _bounds.extents.y * 2;
				_targetPosition = newPos;
				
				return true;
			}
		}
		
		return false;
	}
	
	void CrouchingUpdate( float timeDelta )
	{
		BoxCollider col = _collider as BoxCollider;
		Vector3 extents = col.extents;	
		Vector3 center = col.center;
		
		extents.y = _duckColliderHeight * 0.5f;
		
		
		UpdateDirection();
		
		_senses.UpdateFloorChecks( _bounds.extents.x * 2 );
		
		// Apply running acceleration
		float accelVal = _runAccel * _duckMovementFactor;
		
		// if moving in opposite direction to input
		if ( (_inputVector.x * _moveVel.x) < 0 )
		{
			accelVal = _runDirChangeDecel;
		}
		
		_moveVel.x += (_inputVector.x * accelVal) * timeDelta;
		_moveVel.x = Mathf.Clamp(_moveVel.x,-_runSpeed * _duckMovementFactor, _runSpeed * _duckMovementFactor);
		
		DecelIfNoInput( timeDelta );
		
		// Apply gravity
		_moveVel += -_senses.groundNormal * (_gravity * timeDelta);
		
		// Check that one foot is on the ground
		if ( !IsOneFootOnTheGround() )
		{
			_onGroundTimer.Reset();
		}
		
		_inAirTimer.Reset();
		
		//Debug.Log(_senses.distanceToCeiling + "," + _startColliderHeight);
		
		if ( !onGround )
		{
			_movementState.SetState("InAir");
		}		
		else if ( !_duckHeld && (_senses.distanceToCeiling > (_startColliderHeight * 2)) )
		{
			_movementState.SetState("OnGround");
		}
		
		if ( _movementState.currentState != "Crouching" )
		{
			extents.y = _startColliderHeight;
		}
		
		center.y = extents.y;
		col.extents = extents;
		col.center = center;
		_bounds = col.bounds;
	}
	
	protected override void InAirUpdate( float timeDelta )
	{
		base.InAirUpdate(timeDelta);
		
		DoLedgeGrabCheck();
		
		if ( !onGround && _onWall && _senses.isWallAtHandHeight && _senses.isWallAtCrotchHeight )
		{
			if ( _senses.wallTypeAtHand == WallType.Climbable && _abilities.wallClingEnabled )
			{
				_movementState.SetState("OnClimbableWall");
			}
			else if ( (_senses.wallTypeAtHand == WallType.Normal || _senses.wallTypeAtHand == WallType.Climbable) && _abilities.wallJumpEnabled )
			{
				_movementState.SetState("OnWall");
			}
		}
	}
	
	void OnWallUpdate( float timeDelta )
	{
		if ( !_jumpPressed )
		{
			_ableToWallJump = true;
		}
		
		_readyForDoubleJump = false;
		_moveVel.x = 0;
		
		// if pulling away from the wall
		if ( _inputVector.x * _onWallNormal.x > 0 )
		{
			if ( _letGoOfWallTimer.Update( timeDelta ) )
			{
				UpdateDirection();
				_movementState.SetState("InAir");
				_ableToWallJump = false;
				_readyForDoubleJump = false;
				return;
			}
		}
		else
		{
			_letGoOfWallTimer.Reset();
		}
		
		float accelVal = _airAccel;
		
		if ( _inputVector.x * _onWallNormal.x < 0 )
		{
			_moveVel.x += (_inputVector.x * accelVal) * GameTime.deltaTime;
			_moveVel.x = Mathf.Clamp(_moveVel.x,-_runSpeed, _runSpeed);
		}
				
		// if the player pressed the jump button while attached to wall
		if ( _ableToWallJump && _jumpPressed )
		{
			DoWallJump();			
			return;
		}
		
		
		float grav = _gravity * _wallRunGravityFactor;
		
		/*if ( _inputVector.y < -0.1f )
		{
			grav = _gravity;
		}*/
		
		_moveVel.y += (-grav * GameTime.deltaTime);
		
		// Make sure the player doesn't fall too fast		
		float slideSpeed = _wallSlideSpeed;
		if ( _inputVector.y < -0.1f )
		{
			slideSpeed = _wallSlideSpeed * 4;
		}
		
		_moveVel.y = Mathf.Clamp(_moveVel.y, -slideSpeed, _maxVerticalSpeed );
				
		if ( DoLedgeGrabCheck() )
		{
			_ableToWallJump = false;
		}		
		
		_inAirTimer.Update(timeDelta);
		
		Vector3 pos = _transform.position;
		pos.x = _senses.wallIntersectPoint.x + _onWallNormal.x * _bounds.extents.x;
		_transform.position = pos;
		
		if ( onGround )
		{
			// if the player isn't trying to move when landing, decelerate quickly to help them stick landings easier
			if ( _inputVector.x == 0 )
			{
				_moveVel.x *= 0.5f;
			}
			_movementState.SetState( "OnGround" );
			_ableToWallJump = false;
		} 
		else if ( _direction.x * _onWallNormal.x > 0 || !_onWall || !_senses.isWallAtCrotchHeight )
		{
			// player is facing away from wall
			_movementState.SetState("InAir" );
			_ableToWallJump = false;
			_readyForDoubleJump = false;
		}
		
		CheckForPossibleJump();
		
		_ableToDash = true;
	}
	
	void OnClimbableWallUpdate( float timeDelta )
	{
		if ( !_jumpPressed )
		{
			_ableToWallJump = true;
		}
		
		_readyForDoubleJump = false;
		
		_moveVel.x = 0;
		
		if ( _inputVector.y >= 0 )
		{
			_moveVel.y = _inputVector.y * _wallClimbSpeed;
		}
		else
		{
			_moveVel.y += (-_gravity * timeDelta);
		}
		
		// Make sure the player doesn't fall too fast
		_moveVel.y = Mathf.Clamp(_moveVel.y, -_wallClimbSlideSpeed, 16 );
		
		// Snap player to wall
		Vector3 pos = _transform.position;
		pos.x = _senses.wallIntersectPoint.x + _onWallNormal.x * _bounds.extents.x;
		_transform.position = pos;
		
		// if the player pressed the jump button while attached to wall
		if ( _ableToWallJump && _jumpPressed )
		{
			DoWallJump();
			return;
		}
		
		// if pulling away from the wall
		if ( _inputVector.x * _onWallNormal.x > 0 )
		{
			if ( _letGoOfWallTimer.Update( timeDelta ) )
			{
				UpdateDirection();
				_movementState.SetState("InAir");
				_ableToWallJump = false;
				_readyForDoubleJump = false;
				return;
			}
		}
		else
		{
			_letGoOfWallTimer.Reset();
		}
		
		if ( DoLedgeGrabCheck() )
		{
			_ableToWallJump = false;
		}	
		
		if ( onGround )
		{
			_movementState.SetState( "OnGround" );
			_ableToWallJump = false;
		} 
		else if ( _direction.x * _onWallNormal.x > 0 || !_onWall || !_senses.isWallAtCrotchHeight )
		{
			// player is facing away from wall
			_movementState.SetState("InAir" );
			_ableToWallJump = false;
			_readyForDoubleJump = false;
		}
		
		_ableToDash = true;
	}
	
	void DoWallJump()
	{
		_inputVector.x = _onWallNormal.x;
			
		UpdateDirection();
		
		_moveVel.x = Mathf.Lerp(0,_direction.x * _runSpeed, Mathf.Abs(_inputVector.x));
		_moveVel.y = _jumpSpeed;// Mathf.Lerp(0, _jumpSpeed, yVal);
		
		_movementState.SetState( "InAir");
		_ableToWallJump = false;
		
		_inputDelayTimer.Reset();
		_delayInput = true;
		
		_readyForDoubleJump = true;
	}
	
	bool DoLedgeGrabCheck()
	{
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
					
					return true;
				}
			}
		}
		
		return false;
	}
		
	void LedgeGrabUpdate( float timeDelta )
	{
		_moveVel.y = 0;
		_moveVel.x = 0;
		
		if ( _jumpPressed )
		{
			// if pulling away from ledge
			if ( ((_inputVector.x * _direction.x) < 0)  /*|| (_inputVector.y < 0)*/ )
			{
				DoWallJump();
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
				_targetPosition.y += _collider.bounds.size.y;
				_movementState.SetState("ClimbingLedge");
			}
		}
		else if ( _duckPressed )
		{
			_movementState.SetState( "InAir");
		}
		else
		{
			Vector3 pos = _transform.position;
			pos = Vector3.Lerp( pos, _targetPosition, timeDelta * 30 );
			_transform.position = pos;
		}
		
		_ableToDash = true;
	}
	
	void LedgeClimbUpdate( float timeDelta )
	{
		///////////
		// TEMP
		///////////
		
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
		
		Debug.DrawLine(_transform.position, _targetPosition);
		
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
	
	void LedgeClimbDownUpdate( float timeDelta )
	{
		///////////
		// TEMP
		///////////
		
		Vector3 pos = _transform.position;
		if ( Mathf.Abs(_targetPosition.x - pos.x) > 0.1f )
		{
			pos.x = Mathf.Lerp(pos.x, _targetPosition.x, 15 * timeDelta);
		}
		else
		{			
			pos.y = Mathf.Lerp(pos.y, _targetPosition.y, 15 * timeDelta);
		}
		
		_transform.position = pos;
		
		if ( (_transform.position - _targetPosition).magnitude < 0.1f )
		{
			_transform.position = _targetPosition;
			_movementState.SetState("LedgeGrab");
		}
	}
	
	void DashingUpdate( float timeDelta )
	{
		if ( _dashingTimer.Update(timeDelta) )
		{
			_movementState.SetState("InAir");
			return;
		}
		
		
		
		_moveVel.y = 0;
		
		_moveVel.x = _direction.x * _dashingSpeed;
	}
	
	public bool CheckForLedgeGrab( ref Vector3 ledgePosition )
	{
		if ( _senses.isWallAtHandHeight && !_senses.isWallAboveHandHeight )
		{
			ledgePosition = _senses.ledgePosition;
			return true;
		}
		
		return false;
	}
	
	public void Duck()
	{
		_duckPressed = true;
	}
	
	public void DuckHeld( bool val )
	{
		_duckHeld = val;
	}
	
	public void Dash()
	{
		if ( _ableToDash && _movementState.currentState != "Dashing" )
		{
			_movementState.SetState("Dashing");
			_dashingTimer.Reset();
			_ableToDash = false;
		}
	}
	
	public override void Move( Vector2 inputVec )
	{
		if ( !_delayInput )
		{
			base.Move(inputVec);
		}
	}
		
	protected override void CheckForJump()
	{
		if ( inAir && _readyForDoubleJump && _abilities.doubleJumpEnabled )
		{
			ExecuteJump( _doubleJumpFactor );
			_readyForDoubleJump = false;
		}
		else
		{
			base.CheckForJump();
		}
	}
	
	protected override void ExecuteJump( float jumpFactor = 1 )
	{
		if ( _movementState.currentState == "ClimbingLedge" )
		{
			return;
		}
		
		base.ExecuteJump(jumpFactor);
		
		_readyForDoubleJump = true;
	}
	
	public override void OnCollisionEnter( Collision other )
	{
		base.OnCollisionEnter(other);
		
		if ( other.gameObject.layer == _groundLayer )
		{
			if ( Mathf.Approximately(Mathf.Abs(other.contacts[0].normal.x), 1 ) )
			{
				_onWall = true;	
				_onWallNormal = other.contacts[0].normal;
			}
		}
	}
	
	public override void OnCollisionStay( Collision other )
	{
		base.OnCollisionStay(other);
		
		if ( other.gameObject.layer == _groundLayer )
		{
			if ( Mathf.Approximately(Mathf.Abs(other.contacts[0].normal.x), 1 ) )
			{
				_onWall = true;	
				_onWallNormal = other.contacts[0].normal;
			}
		}
	}
}
