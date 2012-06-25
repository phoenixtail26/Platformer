////////////////////////////////////////////////////////////////////////////////////////
// 
// File:        AT_StateTransition.cs
// Author:      Gavin Hayler
// Date:        23/06/2011
//
////////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

////////////////////////////////////////////////////////////////////////////////////////
// 
// Class:       AT_StateTransition
// Description: A transition between two states, which will be triggered if all of it's 
//              conditions are met at the same time
//
////////////////////////////////////////////////////////////////////////////////////////
public class AT_StateTransition : AT_Node
{
    // Positions used for drawing connections in the editor window
    public Vector2 m_startConnection = new Vector2();
    public Vector2 m_endConnection = new Vector2();

    // The time taken for the transition to complete
    public float m_transitionTime = 1.0f;
    
    // The states that the transition occurs between
	[SerializeField]
	public AT_State m_fromState;
	[SerializeField]
	public AT_State m_toState;
	
	public bool fireRequestOnTransitionEnd = false;
	public string transitionEndRequest = "";
	public bool fireRequestOnTransitionStart = false;
	public string transitionStartRequest = "";
	
	public bool debug = false;
	
    // Used to store how far through the transition we are (if it is currently firing)
	float m_progress = 0;
    // Have all the conditions been met and has the transition fired
	bool valid = false;
	bool m_firing = false;

    // A list of all the conditions in this transition
    [SerializeField]
    public List<AT_TransitionCondition> m_conditions = new List<AT_TransitionCondition>();

    public float Progress
    {
    	get { return m_progress; }
    }
	
	public bool Firing
	{
		get { return m_firing; }
	}

	public AT_StateTransition( AT_State fromState, AT_State toState )
	{
		m_fromState = fromState;
		m_toState = toState;
        NodeType = AT_NodeType.kTransition;
	}

    // Functions mostly used by editor to add/remove or change certain conditions
	public void AddCondition( AT_TransitionCondition condition )
	{
        m_conditions.Add(condition);
	}

    public void RemoveCondition (AT_TransitionCondition condition)
    {
    	m_conditions.Remove (condition);
    }
	
	public void MoveConditionUp (AT_TransitionCondition condition)
	{
		int index = m_conditions.IndexOf (condition);
		index = Mathf.Clamp (index - 1, 0, m_conditions.Count - 1);
		m_conditions.Remove (condition);
		m_conditions.Insert (index, condition);
	}
	
	public void MoveConditionDown (AT_TransitionCondition condition)
	{
		int index = m_conditions.IndexOf (condition);
		index = Mathf.Clamp (index + 1, 0, m_conditions.Count - 1);
		m_conditions.Remove (condition);
		m_conditions.Insert (index, condition);
	}

    public void ReplaceCondition(AT_TransitionCondition oldCond, AT_TransitionCondition newCond)
    {
        int index = m_conditions.IndexOf(oldCond);
        m_conditions.Insert(index, newCond);
        RemoveCondition(oldCond);
    }

	// Process transition. Returns true if this transition is active, false otherwise
	public bool Process (float timeDelta)
	{
		if ( m_fromState == null )
		{
			return false;
		}
		
		// If the from state is currently active, then test the conditions of this transition
		if (m_fromState.Current)
        {
			valid = AreConditionsMet ( timeDelta );
			
			PostProcess ( timeDelta );
		}
        else
        {
			if ( m_fromState.Transitioning )
			{
				// Reset all conditions if from state is not current
				ResetConditions (false);
			}
			
			valid = false;
		}

        // Does the from state have some weight and has this transition been fired?
		if (m_fromState.Transitioning && m_firing)
		{
			// Adjust our progress through the transition
			float inc = timeDelta / m_transitionTime;
			m_progress += inc;
			m_progress = Mathf.Clamp (m_progress, 0, 1);

            // Update the states' weights based on our progress
			m_fromState.SetLocalBlendWeight (1 - m_progress);
			m_toState.SetLocalBlendWeight (m_progress);

            // If we have finished transitioning, reset this transition
			if (m_progress >= 1)
			{
				if ( fireRequestOnTransitionEnd)
				{
					m_ParentTree.SendExternalRequest(transitionEndRequest);
				}
				
				m_progress = 0;
				valid = false;
				m_firing = false;
				ResetConditions(true);
			}
		}
		else
		{
			m_progress = 0;
		}

		return valid;
	}

	public void Trigger ()
	{
		m_firing = true;
		if ( fireRequestOnTransitionStart )
		{
			m_ParentTree.SendExternalRequest(transitionStartRequest);
		}
	}
	
    // Perform any processing required after all conditions have been checked
    void PostProcess ( float timeDelta )
    {
    	if (m_conditions != null)
        {
    		foreach (AT_TransitionCondition condition in m_conditions)
            {
    			if (condition != null)
				{
    				condition.PostProcess (timeDelta);
    			}
            }
        }
    }

    // Reset all conditions to "un-met" state
	public void ResetConditions ( bool force )
	{
		if (m_conditions != null)
        {
			foreach (AT_TransitionCondition condition in m_conditions)
            {
				if (condition != null)
				{
					if ((!force && condition.m_resetWhenFromStateNotCurrent) || force)
					{
						if ( condition.debug )
						{
							Debug.Log(force + " - " + condition.m_resetWhenFromStateNotCurrent);
						}
						condition.ResetCondition ();
					}
				}
            }
        }
	} 

    // Have all the conditions been met on this update loop
	bool AreConditionsMet ( float timeDelta )
	{
		bool conditionsMet = true;
		
        if (m_conditions != null)
        {
			foreach (AT_TransitionCondition condition in m_conditions)
            {
				if ( condition	!= null )
				{
					if (conditionsMet)
					{
						condition.Process ( timeDelta );
	
	                	// If any conditions are not met, return false
						if (!condition.IsConditionMet ())
	                	{
							conditionsMet = false;
						}
					}
					/*else 
					{
						condition.ResetCondition ();
					}*/
				}
            }
        }

        // All conditions have been met
        return conditionsMet;
	}

    // Pass any requests from the state-machine through to the conditions
	public void SendRequest( string request )
	{
		// Only send requests if the from state is actually active
		if ( m_fromState != null && m_fromState.Transitioning )
		{
            foreach (AT_TransitionCondition condition in m_conditions)
			{
                AT_RequestCondition req = condition as AT_RequestCondition;
                if (req != null)
                {
                    req.SendRequest(request);
                }
			}
		}
	}
}
