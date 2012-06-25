////////////////////////////////////////////////////////////////////////////////////////
// 
// File:        AT_ProbabilityCondition.cs
// Author:      Gavin Hayler
// Date:        23/06/2011
//
////////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

////////////////////////////////////////////////////////////////////////////////////////
// 
// Class:       AT_ProbabilityCondition
// Description: A transition condition which will be met based on the probability 
//              provided
//
////////////////////////////////////////////////////////////////////////////////////////
[System.Serializable]
public class AT_ProbabilityCondition : AT_TransitionCondition
{
    // The probability of the condition being met (Range: 0 - 1)
    public float m_probability = 0;

    public AT_ProbabilityCondition()
	{
        m_conditionType = ConditionType.kProbability;
	}

    public override void Process( float timeDelta )
    {
        //m_conditionMet = false;
        float rand = Random.Range(0.0f, 1.0f);
        if (rand < m_probability)
        {
            m_conditionMet = true;
        }
    }
}
