////////////////////////////////////////////////////////////////////////////////////////
// 
// File:        ControlVariable.cs
// Author:      Gavin Hayler
// Date:        23/06/2011
//
////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using UnityEngine;

////////////////////////////////////////////////////////////////////////////////////////
// 
// Class:       ControlVariable
// Description: A user-controlled variable that can be used to trigger transitions and  
//              control blends and animations
//
////////////////////////////////////////////////////////////////////////////////////////
[System.Serializable]
public class ControlVariable : MonoBehaviour//ScriptableObject
{
    // The name of the variable
    public string m_variableName = "";

    // Current value of the variable
    [SerializeField]
    float m_value = 0;
    
    // Minimum value the variable can hold
    [SerializeField]
    float m_minValue = 0;
    
    // Maximum value the variable can hold
    [SerializeField]
    float m_maxValue = 1;

    // Difference between max and min values. Used for normalizing
    [SerializeField]
    float m_valueRange = 1;
	
	float _resetValue = 0;

    public float Value
    {
        get { return m_value; }
        set 
        { 
            m_value = Mathf.Clamp(value, m_minValue, m_maxValue); 
        }
    }

    public float MinValue
    {
        get { return m_minValue; }
        set 
        { 
            m_minValue = value;
            m_value = Mathf.Clamp(m_value, m_minValue, m_maxValue);
            m_valueRange = m_maxValue - m_minValue;
        }
    }

    public float MaxValue
    {
        get { return m_maxValue; }
        set 
        { 
            m_maxValue = value;
            m_value = Mathf.Clamp(m_value, m_minValue, m_maxValue);
            m_valueRange = m_maxValue - m_minValue;
        }
    }

    public float NormalizedValue
    {
        get 
        {
            return ((m_value - m_minValue) / m_valueRange);
        }
    }
    

    // When instantiating new ScriptableObjects, the constructor is not called
    public ControlVariable(string name, float value, float min, float max)
    {
        m_variableName = name;
        m_value = value;
        m_minValue = min;
        m_maxValue = max;
    }
	
	void Start()
	{
		_resetValue = Value;
	}
	
	public void Reset()
	{
		Value = _resetValue;
	}
}