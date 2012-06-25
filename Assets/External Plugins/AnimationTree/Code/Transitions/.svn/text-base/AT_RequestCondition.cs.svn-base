////////////////////////////////////////////////////////////////////////////////////////
// 
// File:        AT_RequestCondition.cs
// Author:      Gavin Hayler
// Date:        23/06/2011
//
////////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

////////////////////////////////////////////////////////////////////////////////////////
// 
// Class:       AT_RequestCondition
// Description: A transition condition that will be met if the correct request is passed
//              to it by the containing state-machine, but will be set false immediately
//              after the current update
//
////////////////////////////////////////////////////////////////////////////////////////
[System.Serializable]
public class AT_RequestCondition : AT_TransitionCondition
{
    // The request that will cause this condition to be met
    [SerializeField]
    public string m_request = "";
	
    public AT_RequestCondition()
    {
    }

	public AT_RequestCondition( string request )
	{
		m_request = request;
        m_conditionType = ConditionType.kRequest;
	}

	public void SendRequest( string request )
	{
		if ( debug )
		{
			Debug.Log("request received: " + request + " - desired: " + m_request);
		}
		
		if ( request == m_request )
		{
			if ( debug )
			{
				Debug.Log("condition met");
			}
			m_conditionMet = true;
		}
	}

    public override void PostProcess( float timeDelta )
    {
       // m_conditionMet = false;
    }
}
