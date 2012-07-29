using UnityEngine;
using System.Collections;

public class PrototypeController : MonoBehaviour 
{
	public PlayerMovementController player;
	

		
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
		Vector2 inputVec = Vector2.zero;
		
		inputVec = i.GetAxes( "Move" );
				
		player.Move( inputVec );
		
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
