////////////////////////////////////////////////////////////////////////////////////////
// 
// File:        AT_State.cs
// Author:      Gavin Hayler
// Date:        23/06/2011
//
////////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

////////////////////////////////////////////////////////////////////////////////////////
// 
// Class:       AT_State
// Description: A discreet state that a state-machine can be in. Any child of a 
//              state-machine must inherit from this class.
//
////////////////////////////////////////////////////////////////////////////////////////
public class AT_State : AT_OutputBasedContainer 
{
    public string m_stateName = "";

	// Is this state currently active or transitioning?
	public bool Transitioning
	{
        get { return (ActualBlendWeight > 0); }
	}

	// Is this the currently active state?
	public bool Current
	{
		get { return ( localBlendWeight == 1 && parentBlendWeight > 0 ); }
	}
}