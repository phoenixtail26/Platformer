
////////////////////////////////////////////////////////////////////////////////////////
// 
// File:        AT_AnimationBase.cs
// Author:      Gavin Hayler
// Date:        01/02/2012
//
////////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

////////////////////////////////////////////////////////////////////////////////////////
// 
// Class:       AT_AnimationBase
// Description: Base class for AT_Animation and AT_Pose classes
//
////////////////////////////////////////////////////////////////////////////////////////
public class AT_AnimationBase: AT_Node 
{
	// Static counter used to make sure all animations are on different animation layers
	static protected int m_animationLayerCounter = 0;
	
	 // Name of the original animation
	public string m_animationName = "";

    // The Animation instance that this animation belongs to
	public Animation m_subject;
	
	// Animation state used for this animation
	protected AnimationState m_animState;
	
	// The animation layer this animation is on
	protected int m_animationLayer = 0;
	
	// Offset applied to animation layer allows user control
	public int m_animationLayerOffset = 0;
	
	 // Name of the animation after a unique AnimState has been created
    protected string m_actualAnimationName = "";
	
	// Factor to multiply node weight by before applying to animation state
	public float m_weightMultiplier = 1;
	
	public bool _randomTimeStart = false;
	
	public AT_AnimationBase()
	{
        // Get the animation layer for this animation and then increase the layer counter
		m_animationLayer = m_animationLayerCounter++;
	}
	
	public void Awake()
	{
		// When we start running, do the necessary setup
		if ( m_animationName != "" && m_subject != null )
		{
			SetAnimation( m_animationName );
		}
	}
	
	public void SetAnimation(string animName)
    {
        // Set the new anim name and then reinitialize
        m_animationName = animName;
        Init();
    }
	
	
	public virtual void Init()
    {
       
    }
}
