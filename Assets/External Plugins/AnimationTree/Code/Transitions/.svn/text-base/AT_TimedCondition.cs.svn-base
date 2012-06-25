////////////////////////////////////////////////////////////////////////////////////////
// 
// File:        AT_TimedCondition.cs
// Author:      Gavin Hayler
// Date:        23/06/2011
//
////////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

////////////////////////////////////////////////////////////////////////////////////////
// 
// Class:       AT_TimedCondition
// Description: A transition condition that becomes true after a certain amount of 
//              time has passed
//
////////////////////////////////////////////////////////////////////////////////////////
[System.Serializable]
public class AT_TimedCondition : AT_TransitionCondition
{
    // The amount of time that needs to pass for this condition to be true
    public float m_targetTime = 0;
    // The amount of time that has passed so far
    public float m_passedTime = 0;

    public AT_TimedCondition()
	{
        m_conditionType = ConditionType.kTimed;
	}

    public override void Process ( float timeDelta )
    {
    	if (m_conditionMet)
		{
    		m_passedTime = 0;
    		m_conditionMet = false;
    	}
		
        // Increment our passed-time counter
        m_passedTime += timeDelta;
        // If the required amount of time has passed, this condition is true
        if (m_passedTime >= m_targetTime)
        {
            m_conditionMet = true;
        }
    }

    public override void ResetCondition()
    {
        base.ResetCondition();

        m_passedTime = 0;
    }
}
