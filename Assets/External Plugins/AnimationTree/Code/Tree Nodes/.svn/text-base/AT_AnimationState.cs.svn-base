////////////////////////////////////////////////////////////////////////////////////////
// 
// File:        AT_AnimationState.cs
// Author:      Gavin Hayler
// Date:        23/06/2011
//
////////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

////////////////////////////////////////////////////////////////////////////////////////
// 
// Class:       AT_AnimationState
// Description: A specialised state to simplify adding a single animation to a state-
//              machine
//
////////////////////////////////////////////////////////////////////////////////////////
public class AT_AnimationState : AT_State
{
    // The animation node that this state is storing
    public AT_Animation m_animation;

    public AT_AnimationState()
    {
        NodeType = AT_NodeType.kAnimationState;
    }

    public override void UpdateAnimation ( float timeDelta )
    {
    	base.UpdateAnimation ( timeDelta );

        if (m_animation != null)
        {
    		// Set the state name to the animation name
    		m_stateName = m_animation.m_animationName;

            if (m_animation.m_hitAnimEnd && Current)
            {
    			// If the animation has just looped and this state is current, send the "animLoop" request
    			// which is used to trigger the AT_AnimLoopCondition
    			(Parent as AT_StateMachine).SendRequest ("animLoop_" + m_animation.m_animationName, !m_animation.m_sendAnimationLoopRequest);
            }
        }
    }

    // Pass the new values down to the animation node for both local and parent weights
    public override void SetLocalBlendWeight(float weight)
    {
        base.SetLocalBlendWeight(weight);

        m_animation.SetLocalBlendWeight(weight);
    }

    public override void SetParentBlendWeight(float weight)
    {
        base.SetParentBlendWeight(weight);
        m_animation.SetParentBlendWeight( weight);
    }
}
