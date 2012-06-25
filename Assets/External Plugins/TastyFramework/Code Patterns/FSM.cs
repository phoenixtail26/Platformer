using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class FSMState
{
	public delegate void UpdateDelegate( float timeDelta );
	
	public string name;
	UpdateDelegate _updateDelegate;
	
	public FSMState( string stateName, UpdateDelegate del = null )
	{
		name = stateName;
		_updateDelegate = del;
	}
	
	public void Update( float timeDelta )
	{
		if ( _updateDelegate != null )
		{
			_updateDelegate( timeDelta );
		}
	}
}

[System.Serializable]
public class FSM
{
	Dictionary<string, FSMState> _states = new Dictionary<string, FSMState>();
	FSMState _currentState = null;	
	
	public string currentState
	{
		get 
		{
			if ( _currentState != null )
			{
				return _currentState.name;
			}
			
			return "";
		}
	}
	
	public FSM()
	{
	}
	
	public void SetState( string newState )
	{
		if ( _states.ContainsKey(newState) )
		{
			_currentState = _states[newState];
		}
		else
		{
			Debug.LogWarning("FSM: Trying to set state that doesn't exist: " + newState);
		}
	}
	
	public void AddState( string name, FSMState.UpdateDelegate updateDelegate = null )
	{
		_states.Add( name, new FSMState( name, updateDelegate ) );
		if ( _states.Count == 1 )
		{
			_currentState = _states[name];
		}
	}
	
	
	public void Update( float timeDelta )
	{
		if ( _currentState != null )
		{
			_currentState.Update( timeDelta );
		}
	}
}
