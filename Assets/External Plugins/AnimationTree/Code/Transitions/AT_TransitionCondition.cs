////////////////////////////////////////////////////////////////////////////////////////
// 
// File:        AT_TransitionCondition.cs
// Author:      Gavin Hayler
// Date:        23/06/2011
//
////////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

////////////////////////////////////////////////////////////////////////////////////////
// 
// Class:       AT_TransitionCondition
// Description: Base class that all transition conditions derive from
//
////////////////////////////////////////////////////////////////////////////////////////
[System.Serializable]
public abstract class AT_TransitionCondition : MonoBehaviour//ScriptableObject
{
    // The different types of conditions available
    public enum ConditionType
    {
        kRequest,
        kProbability,
        kTimed,
        kMath,
        kAnimationLoop
    }

    public ConditionType m_conditionType = ConditionType.kRequest;
	public bool m_resetPreviousCondition = false;
	
    // Has this condition been met
	protected bool m_conditionMet = false;
	
	public bool debug = false;
	
	public bool m_resetWhenFromStateNotCurrent = true;

    // Called before condition is checked 
    public virtual void Process( float timeDelta )
    {
    }

    // Called after condition is checked 
    public virtual void PostProcess( float timeDelta )
    {
    }

	public bool IsConditionMet()
	{
		return m_conditionMet;
	}

    // Reset the condition to it's un-met state
	public virtual void ResetCondition()
	{
		if ( debug )
		{
			Debug.Log("reset condition");
		}
		m_conditionMet = false;
	}

}
