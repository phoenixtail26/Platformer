using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour 
{
	public PlayerMovementController player;
	
	Vector2 _input = Vector2.zero;
	
	// Use this for initialization
	void Start () 
	{
		TastyInput i = TastyInput.instance;
		i.BindAxes( "Move", JoystickAxes.LeftAnalogStick );
		i.BindAxesKeys( "Move", KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.DownArrow, KeyCode.UpArrow );
		
		i.BindKey( "Jump", KeyCode.Z );
		i.BindButton( "Jump", JoystickButtons.A_Cross );
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
			player.StartJump();
		}
		
		if ( i.GetButtonUp( "Jump" ) )
		{
			player.EndJump();
		}
	}
}
