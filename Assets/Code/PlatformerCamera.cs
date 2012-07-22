using UnityEngine;
using System.Collections;
using Framework;

public class PlatformerCamera : MonoBehaviour 
{
	public PlayerMovementController player;
	
	Transform _transform;
	Transform _playerTrans;
	
	[SerializeField]
	float _xTrackingSpeed = 5;
	[SerializeField]
	float _upwardYTrackingSpeed = 3;
	[SerializeField]
	float _downwardYTrackingSpeed = 5;
	
	[SerializeField]
	float _additionalViewingDirectionAmount = 1;
	[SerializeField]
	float _viewDirectionChangeTrackingSpeed = 1;
	[SerializeField]
	float _viewDirectionDelay = 0.75f;
	
	Vector3 _tartgetPos = Vector3.zero;
	Vector3 _cameraOffset = Vector3.zero;
	
	GameTimer _viewingDirectionChangeTimer;
	
	int _lastViewingDirection = 0;
	bool _updateViewingDirection = false;
	
	bool _continueToTrack = false;
	
	// Use this for initialization
	void Start () 
	{
		_transform = transform;
		_playerTrans = player.transform;
		_tartgetPos = _playerTrans.position;
		
		_transform.position = _tartgetPos;
		
		_viewingDirectionChangeTimer = new GameTimer(_viewDirectionDelay);
	}
	
	// Update is called once per frame
	void Update () 
	{
		Vector3 pos = _transform.position;
		Vector3 playerPos = _playerTrans.position;
		
		_tartgetPos.x = playerPos.x;
		
		if ( player.inLedgeGrab || player.inWallSlide || player.inWallClimb )
		{
			_continueToTrack = true;
		}
		
		if ( player.onGround )
		{
			_continueToTrack = false;
		}
		
		// Only update the camera Y in certain situations
		if ( player.onGround || _continueToTrack || playerPos.y < pos.y )
		{
			_tartgetPos.y = playerPos.y;
		}	
		
		// Handle viewing direction offset
		int facingDirection = Mathf.RoundToInt(player.facingDirection.x);
		
		// If the player has changed viewing direction, start the countdown
		if ( _lastViewingDirection != facingDirection )
		{
			_viewingDirectionChangeTimer.Reset();
			_updateViewingDirection = false;
		}
		if ( _viewingDirectionChangeTimer.running )
		{
			// If the countdown finishes, start updating the camera offset
			if ( _viewingDirectionChangeTimer.Update(GameTime.deltaTime) )
			{
				_viewingDirectionChangeTimer.StopTimer();
				
				_updateViewingDirection = true;
			}
		}
		else
		{
			if ( _updateViewingDirection )
			{
				// Lerp the camera offset
				_cameraOffset.x = Mathf.Lerp(_cameraOffset.x, facingDirection * _additionalViewingDirectionAmount, GameTime.deltaTime * _viewDirectionChangeTrackingSpeed);
			}
		}
		
		float ySpeed = _upwardYTrackingSpeed;
		// Use the faster falling tracking speed if the player is below the camera
		if ( _tartgetPos.y < pos.y )
		{
			ySpeed = _downwardYTrackingSpeed;
		}
				
		pos.x = Mathf.Lerp( pos.x, _tartgetPos.x + _cameraOffset.x, _xTrackingSpeed * GameTime.deltaTime );
		pos.y = Mathf.Lerp( pos.y, _tartgetPos.y, ySpeed * GameTime.deltaTime );
		
		_transform.position = pos;
		_lastViewingDirection = facingDirection;
	}
}
