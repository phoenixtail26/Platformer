using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour 
{
	public Player player;
	public PlayerMovementController playerMove;
	
	Vector2 _input = Vector2.zero;
	
	// Use this for initialization
	void Start () 
	{
		TastyInput i = TastyInput.instance;
		i.BindAxes( "Move", JoystickAxes.LeftAnalogStick );
		i.BindAxesKeys( "Move", KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.DownArrow, KeyCode.UpArrow );
		
		i.BindKey( "Jump", KeyCode.Z );
		i.BindButton( "Jump", JoystickButtons.A_Cross );
		
		i.BindKey( "Duck", KeyCode.C );
		i.BindButton( "Duck", JoystickButtons.B_Circle );
		
		i.BindKey( "DrawGun", KeyCode.LeftShift );
		
		i.BindKey( "FireGun", KeyCode.X );
	}
	
	// Update is called once per frame
	void Update () 
	{
		TastyInput i = TastyInput.instance;
		
		Vector2 inputVec = i.GetAxes( "Move" );
		
		// Debounce analog stick
		if ( inputVec.x * _input.x < 0 )
		{
			inputVec.x = 0;
		}
		
		_input = inputVec;
		
		player.Move( _input );
		
		if ( i.GetButtonDown( "Jump" ) )
		{
			playerMove.StartJump();
		}
		
		if ( i.GetButtonUp( "Jump" ) )
		{
			playerMove.EndJump();
		}
		
		if ( i.GetButtonDown( "Duck" ) )
		{
			playerMove.Duck();
		}
		
		playerMove.DuckHeld( i.GetButton( "Duck" ) );
		
		if ( i.GetButtonDown( "DrawGun" ) )
		{
			player.DrawGun();
		}
		
		if ( i.GetButtonDown( "FireGun" ) )
		{
			player.FireGun();
		}

	}
}
