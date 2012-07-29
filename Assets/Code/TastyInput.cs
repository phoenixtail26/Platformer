using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Class TastyInput.
/// Description: A class to bind together keys and joystick buttons and axes to particular functions.
/// Usage: 	
/// 		Setup:
/// 			BindAxes( "Move", JoystickAxes.LeftAnalogStick );
///		   		BindAxesKeys( "Move", KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.DownArrow, KeyCode.UpArrow );
///		   		BindKey( "Jump", KeyCode.Z );
///				BindButton( "Jump", JoystickButtons.A_Cross );
///			Use:
///				Vector2 inputVec = GetAxes( "Move" );
///				bool jump = GetButtonDown( "Jump" );
/// </summary>
public class TastyInput : AutoSingletonBehaviour<TastyInput>
{
	internal class AxesBinding
	{
		class KeyCodeMatrix
		{
			public KeyCode negativeX;
			public KeyCode positiveX;
			public KeyCode negativeY;
			public KeyCode positiveY;
		}
		
		string _axesFunction;
		JoystickAxes _joystickAxes = JoystickAxes.Count;
		List<KeyCodeMatrix> _keyCodeMatrices = new List<KeyCodeMatrix>();
		bool _dirty = true;
		Vector2 _value;
		
		void CalcValue( TastyJoystick joystick )
		{
			if ( _joystickAxes == JoystickAxes.Count )
			{
				_value = Vector2.zero;
			}
			else
			{
				_value = joystick.GetAxes(_joystickAxes);
			}				
			
			if ( _keyCodeMatrices.Count > 0 )
			{
				foreach( KeyCodeMatrix mat in _keyCodeMatrices )
				{
					if ( Input.GetKey(mat.negativeX) )
					{
						_value.x = -1;
					}
					if ( Input.GetKey(mat.positiveX) )
					{
						_value.x = 1;
					}
					if ( Input.GetKey(mat.negativeY) )
					{
						_value.y = -1;
					}
					if ( Input.GetKey(mat.positiveY) )
					{
						_value.y = 1;
					}
				}
			}
			
			_dirty = false;
		}
		
		public Vector2 GetValue( TastyJoystick joystick )
		{
			if ( _dirty )
			{
				CalcValue( joystick );
			}
			
			return _value;
		}
		
		public AxesBinding( string function )
		{
			_axesFunction = function;
		}
		
		public void SetDirty()
		{
			_dirty = true;
		}
		
		public void SetJoystickBinding( JoystickAxes axes )
		{
			_joystickAxes = axes;
		}
		
		public void AddKeyCodeMatrix( KeyCode negativeX, KeyCode positiveX, KeyCode negativeY, KeyCode positiveY )
		{
			KeyCodeMatrix mat = new KeyCodeMatrix();
			mat.negativeX = negativeX;
			mat.positiveX = positiveX;
			mat.negativeY = negativeY;
			mat.positiveY = positiveY;
			_keyCodeMatrices.Add(mat);
		}
	}
	
	internal class ButtonBinding
	{
		string _buttonFunction;
		HashSet<KeyCode> _keyCodes = new HashSet<KeyCode>();
		HashSet<JoystickButtons> _joystickButtons = new HashSet<JoystickButtons>();
		bool _dirty = true;
		bool _value = false;
		bool _lastValue = false;
		
		public ButtonBinding( string function )
		{
			_buttonFunction = function;
		}
		
		public void SetDirty()
		{
			_dirty = true;
		}
		
		public void AddButton( JoystickButtons button )
		{
			_joystickButtons.Add(button);
		}
		
		public void AddKey( KeyCode code )
		{
			_keyCodes.Add(code);
		}
		
		// Is the button down?
		public bool GetValue( TastyJoystick joystick )
		{
			if ( _dirty )
			{
				CalcValue( joystick );
			}
			
			return _value;
		}
		
		// Was the button pressed this frame?
		public bool GetValueDown( TastyJoystick joystick )
		{
			if ( _dirty )
			{
				CalcValue( joystick );
			}
			
			return !_lastValue && _value;
		}
		
		// Was the button released this frame?
		public bool GetValueUp( TastyJoystick joystick )
		{
			if ( _dirty )
			{
				CalcValue( joystick );
			}
			
			return _lastValue && !_value;
		}
		
		void CalcValue( TastyJoystick joystick )
		{
			_lastValue = _value;
			
			_value = false;
			
			foreach( KeyCode code in _keyCodes )
			{
				if ( Input.GetKey(code) )
				{
					_value = true;
					return;
				}
			}
			
			foreach( JoystickButtons button in _joystickButtons )
			{
				if ( joystick.GetButton(button) )
				{
					_value = true;
					return;
				}
			}
			
			_dirty = false;
		}
	}
	
	public TastyJoystick joystick; 
	
	Dictionary<string, AxesBinding> _axesBindings = new Dictionary<string, AxesBinding>();
	Dictionary<string, ButtonBinding> _buttonBindings = new Dictionary<string, ButtonBinding>();
	
	// Use this for initialization
	public override void Awake () 
	{
		base.Awake();
		
		joystick = new TastyJoystick();
	}
	
	// Update is called once per frame
	void Update () 
	{
		DirtyBindings();
		joystick.Update();		
	}
	
	void DirtyBindings()
	{
		foreach( KeyValuePair<string,AxesBinding> kvp in _axesBindings )
		{
			kvp.Value.SetDirty();
		}
		
		foreach( KeyValuePair<string,ButtonBinding> kvp in _buttonBindings )
		{
			kvp.Value.SetDirty();
		}
	}
	
	public void BindAxes( string axesFunction, JoystickAxes axes )
	{
		if ( !_axesBindings.ContainsKey(axesFunction) )
		{
			AxesBinding newBinding = new AxesBinding( axesFunction );
			_axesBindings.Add( axesFunction, newBinding );
		}
		
		AxesBinding binding = _axesBindings[axesFunction];
		binding.SetJoystickBinding(axes);		
	}
	
	public void BindAxesKeys( string axesFunction, KeyCode negativeX, KeyCode positiveX, KeyCode negativeY, KeyCode positiveY )
	{
		if ( !_axesBindings.ContainsKey(axesFunction) )
		{
			AxesBinding newBinding = new AxesBinding( axesFunction );
			_axesBindings.Add( axesFunction, newBinding );
		}
		
		AxesBinding binding = _axesBindings[axesFunction];
		binding.AddKeyCodeMatrix( negativeX, positiveX, negativeY, positiveY );
	}
	
	public void BindButton( string buttonFunction, JoystickButtons button )
	{
		if ( !_buttonBindings.ContainsKey(buttonFunction) )
		{
			ButtonBinding newBinding = new ButtonBinding( buttonFunction );
			_buttonBindings.Add( buttonFunction, newBinding );
		}
		
		ButtonBinding binding = _buttonBindings[buttonFunction];
		binding.AddButton( button );
	}
	
	public void BindKey( string buttonFunction, KeyCode key )
	{
		if ( !_buttonBindings.ContainsKey(buttonFunction) )
		{
			ButtonBinding newBinding = new ButtonBinding( buttonFunction );
			_buttonBindings.Add( buttonFunction, newBinding );
		}
		
		ButtonBinding binding = _buttonBindings[buttonFunction];
		binding.AddKey(key);
	}
	
	public bool GetButton( string buttonFunction )
	{
		if ( _buttonBindings.ContainsKey(buttonFunction) )
		{
			bool val = _buttonBindings[buttonFunction].GetValue( joystick );
			return val;
		}
		
		return false;
	}
	
	public bool GetButtonDown( string buttonFunction )
	{
		if ( _buttonBindings.ContainsKey(buttonFunction) )
		{
			bool val = _buttonBindings[buttonFunction].GetValueDown( joystick );
			return val;
		}
		
		return false;
	}
	
	public bool GetButtonUp( string buttonFunction )
	{
		if ( _buttonBindings.ContainsKey(buttonFunction) )
		{
			bool val = _buttonBindings[buttonFunction].GetValueUp( joystick );
			return val;
		}
		
		return false;
	}
	
	public Vector2 GetAxes( string axesFunction )
	{
		if ( _axesBindings.ContainsKey(axesFunction) )
		{
			Vector2 vec = _axesBindings[axesFunction].GetValue( joystick );
			return vec;
		}
		
		return Vector2.zero;
	}
}
