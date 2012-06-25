////////////////////////////////////////////////////////////////////////////////////////
// 
// File:        AT_MathCondition.cs
// Author:      Gavin Hayler
// Date:        23/06/2011
//
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

////////////////////////////////////////////////////////////////////////////////////////
// 
// Class:       AT_MathCondition
// Description: A transistion condition which will perform mathematical operations on
//              constants and control variables to determine if it's true
//
////////////////////////////////////////////////////////////////////////////////////////
[System.Serializable]
public class AT_MathCondition : AT_TransitionCondition
{
    // The different types of operators currently supported
    public enum ConditionOperator
    {
        kGreaterThan,
        kGreaterThanOrEqualTo,
        kLessThan,
        kLessThanOrEqualTo,
        kEqualTo
    }

    // The control variable used in the calculation
    public ControlVariable m_controlVariable;

    // The operator
    public ConditionOperator m_opeator;

    // The constant value
    public float m_constant = 0;

    public AT_MathCondition()
	{
        m_conditionType = ConditionType.kMath;
	}

    public override void Process( float timeDelta )
    {
		if ( m_controlVariable == null )
		{
			return;
		}
		
        // Depending on the operator, perform different tests
        switch (m_opeator)
        {
            case ConditionOperator.kGreaterThan:
                m_conditionMet = m_controlVariable.Value > m_constant;
                break;
            case ConditionOperator.kGreaterThanOrEqualTo:
                m_conditionMet = m_controlVariable.Value >= m_constant;
                break;
            case ConditionOperator.kLessThan:
                m_conditionMet = m_controlVariable.Value < m_constant;
                break;
            case ConditionOperator.kLessThanOrEqualTo:
                m_conditionMet = m_controlVariable.Value <= m_constant;
                break;
            case ConditionOperator.kEqualTo:
                m_conditionMet = m_controlVariable.Value == m_constant;
                break;
        }
    }
}
