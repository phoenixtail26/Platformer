using UnityEngine;
using System.Collections;
using Framework;

[RequireComponent(typeof(MovementSenses))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class MovementController : MonoBehaviour 
{	
	[SerializeField]
	protected float _runSpeed = 4;
	[SerializeField]
	protected float _runAccel = 10;
	[SerializeField]
	protected float _decel = 15;
	[SerializeField]
	protected float _runDirChangeDecel = 100;
	[SerializeField]
	protected float _jumpSpeed = 6;
	[SerializeField]
	protected float _shortJumpSpeed = 4;
	[SerializeField]
	protected float _airAccel = 20;
	[SerializeField]
	protected float _airDrag = 0.96875f;
	
	[SerializeField]
	protected float _gravity = -0.5f;
	
	protected Rigidbody _rigidbody;
	protected Transform _transform;
	protected Collider _collider;
	protected Bounds _bounds;
	
	protected Vector3 _moveVel = new Vector3();
	
	protected bool _jumpPressed = false;
	protected bool _runPressed = false;
	protected bool _onGround = false;
	
	protected bool _jumpWhenPossible = false;
	
	protected int _groundLayerMask = 0;
	protected int _groundLayer = 0;
	
	protected GameTimer _onGroundTimer = new GameTimer(0.025f);
	protected GameTimer _inAirTimer = new GameTimer(0.05f);
	
	protected Vector3 _lastPosition = Vector3.zero;
	
	protected Vector3 _direction = Vector3.right;
	
	protected FSM _movementState = new FSM();
		
	protected Vector2 _inputVector = Vector3.zero;
	
	protected MovementSenses _senses;
	
	#region Accessors
	public Vector3 moveVel
	{
		get { return _moveVel; }			
	}
	
	public bool inAir
	{
		get { return _inAirTimer.hasFinished; }
	}
	
	public bool onGround
	{
		get { return _onGroundTimer.hasFinished; }
	}
	
	public Vector3 facingDirection
	{
		get { return _direction; }
	}
	#endregion
	
	public virtual void Awake()
	{
		_groundLayer = LayerMask.NameToLayer("Ground");
		_groundLayerMask = 1 << _groundLayer;
		_transform = transform;
		_rigidbody = rigidbody;
		_collider = collider;
		_bounds = _collider.bounds;
		
		_senses = GetComponent<MovementSenses>();
		
		_lastPosition = _transform.position;
		
		_movementState.AddState( "Nothing" , NothingUpdate );
		_movementState.AddState( "OnGround", OnGroundUpdate );
		_movementState.AddState( "InAir", InAirUpdate );
		_movementState.SetState( "OnGround" );
	}
	
	// Use this for initialization
	public virtual void Start () 
	{
		_onGroundTimer.Reset();
	}
	
	// Update is called once per frame
	public virtual void FixedUpdate () 
	{
		_moveVel = _rigidbody.velocity;
				
		_senses.UpdateSenses(_direction);
		
		_movementState.Update(GameTime.deltaTime);
		
		_rigidbody.velocity = _moveVel;		
		
		_lastPosition = _transform.position;
	}
	
	protected virtual void NothingUpdate( float timeDelta )
	{
		_moveVel = Vector3.zero;
	}
	
	protected virtual void OnGroundUpdate( float timeDelta )
	{
		UpdateDirection();
		
		_senses.UpdateFloorChecks( _bounds.extents.x * 2 );
		
		// Apply running acceleration
		float accelVal = _runAccel;
		
		// if moving in opposite direction to input
		if ( (_inputVector.x * _moveVel.x) < 0 )
		{
			accelVal = _runDirChangeDecel;
		}
		
		_moveVel.x += (_inputVector.x * accelVal) * timeDelta;
		_moveVel.x = Mathf.Clamp(_moveVel.x,-_runSpeed, _runSpeed);
		
		DecelIfNoInput( timeDelta );
		
		// Apply gravity
		_moveVel += -_senses.groundNormal * (_gravity * timeDelta);
		
		// Check that one foot is on the ground
		if ( !IsOneFootOnTheGround() )
		{
			_onGroundTimer.Reset();
		}
		
		_inAirTimer.Reset();
		CheckForPossibleJump();
	}
	
	protected virtual void InAirUpdate( float timeDelta )
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
	
	
	protected void UpdateDirection()
	{
		if ( Mathf.Abs(_inputVector.x) > 0.05f )
		{
			_direction.x = _inputVector.x;
			_direction.y = 0;
			_direction.Normalize();
		}
	}
	
	protected void DecelIfNoInput( float timeDelta )
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
	
	protected void CheckForPossibleJump()
	{
		if ( onGround || IsOneFootOnTheGround() || !inAir )
		{
			if ( _jumpWhenPossible )
			{
				_jumpWhenPossible = false;
				ExecuteJump();
			}
		}
	}
	
	public virtual void Move( Vector2 inputVec )
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
	
	protected virtual void ExecuteJump( float jumpFactor = 1 )
	{
		_moveVel = _rigidbody.velocity;
		_moveVel.y = _jumpSpeed * jumpFactor;
		_rigidbody.velocity = _moveVel;
		_onGroundTimer.Reset();
		_inAirTimer.hasFinished = true;
	}
	
	protected virtual void CheckForJump()
	{
		if ( !inAir )
		{
			_jumpWhenPossible = true;
			//ExecuteJump();
		}
		else if ( onGround || IsOneFootOnTheGround() )
		{
			_jumpWhenPossible = true;
			//ExecuteJump();
		}
		else if ( !onGround && _senses.minFootDistanceToGround < 0.75f )	// if close to ground, jump when on ground
		{
			_jumpWhenPossible = true;
		}
	}
	
	public void StartJump()
	{
		_jumpPressed = true;
		
		if ( _movementState.currentState == "InAir" || _movementState.currentState == "OnGround" )
		{
			CheckForJump();
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
	
	protected bool IsOneFootOnTheGround()
	{
		if ( _senses.minFootDistanceToGround < 0.3f )
		{
			return true;
		}
		
		return false;
	}
	
	public virtual void OnCollisionEnter( Collision other )
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
	
	public virtual void OnCollisionStay( Collision other )
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
