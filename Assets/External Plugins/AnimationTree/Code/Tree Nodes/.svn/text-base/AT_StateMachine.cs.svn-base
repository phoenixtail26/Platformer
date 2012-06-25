////////////////////////////////////////////////////////////////////////////////////////
// 
// File:        AT_StateMachine.cs
// Author:      Gavin Hayler
// Date:        23/06/2011
//
////////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

////////////////////////////////////////////////////////////////////////////////////////
// 
// Class:       AT_StateMachine
// Description: A state-machine contains a number of states that it can be in. The 
//              current state is changed when transitions are triggered.
//
////////////////////////////////////////////////////////////////////////////////////////
public class AT_StateMachine : AT_State 
{
    // The state the this machine will start in and default back to when it loses focus
	public AT_State m_startUpState;

    // The parent in the Unity hierarchy that holds all the transitions
    public GameObject m_transitionsParent;

    // List containing all transitions pertaining to this machine
    [SerializeField]
    public List<AT_StateTransition> m_transitions = new List<AT_StateTransition>();

    public AT_StateMachine()
    {
        NodeType = AT_NodeType.kStateMachine;
        m_stateName = "";
    }

	public override void Start()
	{
		base.Start();
		
        // Set the start-up state as active on start
		if ( m_startUpState != null )
		{
			SetStateActive( m_startUpState );
		}
	}

    // Add a new transition to this state-machine
    public AT_StateTransition AddTransition(AT_State fromState, AT_State toState)
	{
        // Create and init the new transition
        AT_StateTransition trans = AT_Factory.CreateNode<AT_StateTransition>("Transition", this, m_ParentTree);
        RemoveChild(trans);
        trans.transform.parent = m_transitionsParent.transform;
        trans.NodeType = AT_NodeType.kTransition;
        trans.m_fromState = fromState;
        trans.m_toState = toState;

        // Add it to the list
        m_transitions.Add(trans);
        return trans;
	}

    // Delete a transition
    public void RemoveTransition( AT_StateTransition trans )
    {
        m_transitions.Remove(trans);
    }

	public override void UpdateAnimation ( float timeDelta )
	{
		base.UpdateAnimation(timeDelta);
		
        // If this state-machine isn't active, set it back to it's start-up state
		if (!Current && !Transitioning)
        {
			if (m_startUpState != null)
            {
				SetStateActive (m_startUpState);
			}
			
			foreach (AT_StateTransition transition in m_transitions)
			{
				transition.ResetConditions(false);
			}
		}
		
		List<AT_StateTransition> metTransitions = new List<AT_StateTransition> ();
		
		//Debug.Log ("checking");
		// Check through the transitions to see if any have all their conditions met
		bool transitionFiring = false;
		
		foreach (AT_StateTransition transition in m_transitions)
		{
			if (transition != null)
			{
				// Don't fire any other transitions if one is already firing
				if (transition.Firing)
				{
					transitionFiring = true;
				}
				
				if (transition.Process (timeDelta))
				{
					// Only allow one transition in a state machine to fire at once
					//break;
				
				// Add the met transitions to the list
					metTransitions.Add (transition);
					//Debug.Log (transition.name);
				}
			}
		}
		
		// Choose a random transition from the met list to fire
		if ( !transitionFiring && metTransitions.Count > 0)
		{
			AT_StateTransition chosenTrans = null;
			int randInd = Random.Range (0, metTransitions.Count);
			chosenTrans = metTransitions[randInd];
			chosenTrans.Trigger ();
		}
	}

    // Remove a child state. Needs to be different to a normal container node
    public override void RemoveChild(AT_Node node)
    {
        Children_Writable.Remove(node);
        UnassignedChildren_Writable.Remove(node);
    }
	
    // Set which state is currently active
	public void SetStateActive( AT_State state )
	{
		foreach ( AT_Node node in Children )
		{
			AT_State childState = node as AT_State;
            if (childState != null)
            {
                if (state == childState)
                {
                    childState.SetLocalBlendWeight(1);
                }
                else
                {
                    childState.SetLocalBlendWeight(0);
                }
            }
		}
	}

    // Short hand for finding any child state and setting it as active
    public void SetToFirstState()
    {
        foreach (AT_Node node in Children)
        {
            AT_State childState = node as AT_State;
            if (childState != null)
            {
                SetStateActive(childState);
                return;
            }
        }
    }

    // Send any requests received to transitions as well as child states
	public override void SendRequest( string request, bool sendToChildren, bool sendUpwards = true )
	{
		base.SendRequest( request, sendToChildren, sendUpwards );

		foreach ( AT_StateTransition transition in m_transitions )
		{
            if (transition != null)
            {
                transition.SendRequest(request);
            }
		}
	}
}
