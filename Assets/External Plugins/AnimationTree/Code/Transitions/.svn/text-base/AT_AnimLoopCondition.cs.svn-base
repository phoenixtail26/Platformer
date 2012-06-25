////////////////////////////////////////////////////////////////////////////////////////
// 
// File:        AT_AnimLoopCondition.cs
// Author:      Gavin Hayler
// Date:        23/06/2011
//
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;


////////////////////////////////////////////////////////////////////////////////////////
// 
// Class:       AT_AnimLoopCondition
// Description: A transition condition that will be set true everytime an adjacent
//              animation loops
//
////////////////////////////////////////////////////////////////////////////////////////
[System.Serializable]
public class AT_AnimLoopCondition : AT_RequestCondition
{
	[SerializeField]
	string _animationName = "";
	
	public string animationName
	{
		get { return _animationName; }
		set
		{
			_animationName = value;
			m_request = "animLoop_" + value;
		}
	}
	
    public AT_AnimLoopCondition()
	{
        m_conditionType = ConditionType.kAnimationLoop;

        // Functions the same as a AT_RequestCondition, but the request string is fixed
        animationName = _animationName;
	}
}
