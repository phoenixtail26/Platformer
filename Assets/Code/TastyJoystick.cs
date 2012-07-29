using UnityEngine;
using System.Collections;


public enum JoystickAxes
{
	LeftAnalogStick,
	RightAnalogStick,
	Triggers,
	Count			// Number of elements
}

public enum JoystickButtons
{
	LeftTrigger,
	RightTrigger,
	LeftShoulder,
	RightShoulder,
	DPadLeft,
	DPadRight,
	DPadUp,
	DPadDown,
	LeftStickButton,
	RightStickButton,
	Start,
	Select,
	A_Cross,
	B_Circle,
	X_Square,
	Y_Triangle,
	SystemButton,
	Count			// Number of elements
}

/// <summary>
/// Class TastyJoystick.
/// Description: A wrapper class to allow easy use of joysticks
/// Usage: 	The following axes need to be mapped in Unity's InputManager - Horizontal / Vertical (Left stick); R Horizontal / RVertical (Right stick); 
/// 		L Trigger / R Trigger (Each trigger respectively)
/// Issues:	Unity doesn't allow easy querying of axes on individual joysticks. So, for now this functionality isn't implemented.
/// 		Need a way to keep track of what joysticks belong to which index. Adding or taking away joysticks can rearrange joystick indices non-deterministically.
/// 		Because of these problems, for now this class is only intended to be used for a single joystick
/// </summary>
[System.Serializable]
public class TastyJoystick
{
	#region Button Mappings
	// Mappings should be in same order as buttons in JoystickButtons enum.
	// Use a value of -1 for the triggers to get values from axes instead
	int[] _osxPS3Mappings = 
	{
		8,	// LeftTrigger
		9,	// RightTrigger
		10,	// LeftShoulder
		11,	// RightShoulder
		7,	// DPadLeft
		5,	// DPadRight
		4,	// DPadUp
		6,	// DPadDown
		1,	// LeftStickButton
		2,	// RightStickButton
		3,	// Start
		0,	// Select
		14,	// A_Cross
		13,	// B_Circle
		15,	// X_Square
		12,	// Y_Triangle
		16	// SystemButton
	};
	
	int[] _osxXBox360Mappings = 
	{
		-1,	// LeftTrigger
		-1,	// RightTrigger
		13,	// LeftShoulder
		14,	// RightShoulder
		7,	// DPadLeft
		8,	// DPadRight
		5,	// DPadUp
		6,	// DPadDown
		11,	// LeftStickButton
		12,	// RightStickButton
		9,	// Start
		10,	// Select
		16,	// A_Cross
		17,	// B_Circle
		18,	// X_Square
		19,	// Y_Triangle
		15	// SystemButton
	};
	#endregion
	
	[SerializeField]
	bool _debugKeyValues = false;
	
	bool[,] _joystickButtons = new bool[4,(int)JoystickButtons.Count];
	bool[,] _joystickButtonsLastFrame = new bool[4,(int)JoystickButtons.Count];
	Vector2[,] _joystickAxes = new Vector2[4,(int)JoystickAxes.Count];
	
	int[][] _buttonMappings = new int[4][];
	
	public TastyJoystick()
	{
		for ( int i = 0; i < 4; i++ )
		{
			_buttonMappings[i] = new int[(int)JoystickButtons.Count];
		}
	}
	
	public void Update()
	{
		UpdateJoystickButtonMappings();
		UpdateJoystickButtons();
		UpdateJoystickAxes();
		
		if ( _debugKeyValues )
		{
			DebugOuputKeyCodePressed();
			OutputJoystickButtonPressed();
			OutputJoystickAxisUsed();
		}
	}
	
		// Is this button down?
	public bool GetButton( JoystickButtons button, int joystickIndex = 0 )
	{
		return _joystickButtons[joystickIndex,(int)button];
	}
	
	// Was this button pressed?
	public bool GetButtonDown( JoystickButtons button, int joystickIndex = 0 )
	{
		return (!_joystickButtonsLastFrame[joystickIndex,(int)button] && _joystickButtons[joystickIndex,(int)button]);
	}
	
	// Was this button released?
	public bool GetButtonUp( JoystickButtons button, int joystickIndex = 0 )
	{
		return (_joystickButtonsLastFrame[joystickIndex,(int)button] && !_joystickButtons[joystickIndex,(int)button]);
	}
	
	public Vector2 GetAxes( JoystickAxes axes, int joystickIndex = 0 )
	{
		return _joystickAxes[joystickIndex, (int)axes];
	}
	
	void DebugOuputKeyCodePressed()
	{
		foreach ( KeyCode code in System.Enum.GetValues(typeof(KeyCode)) )
		{
			if ( Input.GetKey(code) )
			{
				Debug.Log( code + " pressed");
			}
		}
	}
	
	void OutputJoystickButtonPressed()
	{
		for ( int i = 0; i < 4; i++ )
		{
			for ( int j = 0; j < (int)JoystickButtons.Count; j++ )
			{
				if ( GetButton((JoystickButtons)j, i ))
				{
					Debug.Log("Joystick " + (i+1) + " pressed " + (JoystickButtons)j);
				}
			}
		}
	}
	
	void UpdateJoystickButtonMappings()
	{
		// TODO: Need to get mappings for Windows machines
		
		string[] joystickNames = Input.GetJoystickNames();
		int joystickCounter = 0;
		foreach( string name in joystickNames )
		{
			// Which mapping are we going to use?
			int[] mapping = _osxXBox360Mappings;
			string lowerName = name.ToLower();
			if ( lowerName.Contains("sony") || lowerName.Contains("ps3") )
			{
				mapping = _osxPS3Mappings;
				//Debug.Log("joystick " + joystickCounter + " is using ps3 mappings");
			}
			
			// Assign the mappings into the storage array
			foreach ( JoystickButtons button in System.Enum.GetValues(typeof(JoystickButtons)) )
			{
				if ( button == JoystickButtons.Count )
					continue;
				
				_buttonMappings[joystickCounter][(int)button] = mapping[(int)button];
			}
		}
	}
	
	void UpdateJoystickAxes()
	{
		// TODO: No easy way to get axes from each joystick
		
		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis("Vertical");
		_joystickAxes[0,(int)JoystickAxes.LeftAnalogStick].Set(h,v);
		
		h = Input.GetAxis("R Horizontal");
		v = Input.GetAxis("R Vertical");
		_joystickAxes[0,(int)JoystickAxes.RightAnalogStick].Set(h,v);
		
		// Bug: Unity returns 0 from trigger in rest position before it has been used and -1 after it has been used.
		// 		Therefore, our values will be 0.5 until the player uses the trigger :(
		float lt = Input.GetAxis("L Trigger") * 0.5f + 0.5f; // Put value in a 0 - 1 range
		float rt = Input.GetAxis("R Trigger") * 0.5f + 0.5f; // Put value in a 0 - 1 range
		
		// For when the trigger mapping to axes doesn't work, use buttons instead
		if ( _buttonMappings[0][(int)JoystickButtons.LeftTrigger] != -1 )
		{
			lt = GetButton(JoystickButtons.LeftTrigger) ? 1 : 0;
		}
		if ( _buttonMappings[0][(int)JoystickButtons.RightTrigger] != -1 )
		{
			rt = GetButton(JoystickButtons.RightTrigger) ? 1 : 0;
		}
		
		_joystickAxes[0, (int)JoystickAxes.Triggers].Set(lt,rt);
		
		/*foreach (JoystickAxes axes in System.Enum.GetValues(typeof(JoystickAxes)) )
		{
			if ( axes == JoystickAxes.Count )
				continue;
			
			Debug.Log( axes + ": " + _joystickAxes[0,(int)axes] );
		}*/
	}
	
	void UpdateJoystickButtons()
	{
		string[] joystickNames = Input.GetJoystickNames();
		int joystickCounter = 0;
		foreach( string name in joystickNames )
		{
			string prefix = "joystick " + (joystickCounter+1) + " button ";
			
			int[] mapping = _buttonMappings[joystickCounter];
			foreach ( JoystickButtons button in System.Enum.GetValues(typeof(JoystickButtons)) )
			{
				if (( button == JoystickButtons.Count ) || ( mapping[(int)button] == -1 ) )
					continue;
					
				// Keep record of last frame's value
				_joystickButtonsLastFrame[joystickCounter, (int)button] = _joystickButtons[joystickCounter, (int)button];
				
				// Store new value
				string buttonName = prefix + mapping[(int)button];
				_joystickButtons[joystickCounter, (int)button] = Input.GetKey( buttonName );
			}
			
			joystickCounter++;
		}
	}
	
	void OutputJoystickAxisUsed()
	{
		//Debug.Log( Input.GetAxis("joystick 1 x axis") );
		//Debug.Log(Input.GetAxis("L Trigger"));
	}
}