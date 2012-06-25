////////////////////////////////////////////////////////////////////////////////////////
// 
// File:        AT_Animation.cs
// Author:      Gavin Hayler
// Date:        23/06/2011
//
////////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

////////////////////////////////////////////////////////////////////////////////////////
// 
// Class:       AT_Animation
// Description: Tree node that contains information about an animation and how to 
//              process it.
//
////////////////////////////////////////////////////////////////////////////////////////
public class AT_Animation : AT_AnimationBase
{
    // The wrap mode used for animation playback
	public WrapMode m_wrapMode = WrapMode.Loop;

    // Is this animation synced with any other animation
    public bool m_syncAnimation = false;
    // The sync layer that this animation is on. All animations on this layer will be time synced.
    public int m_syncAnimationLayer = -1;

    // Blend mode applied to animation
    public AnimationBlendMode m_blendMode = AnimationBlendMode.Blend;

    // Are we using only part of the animation and if so, which part?
    public Transform m_mixTransform;

    // A counter used to determine if an animation has hit its end point or looped
    float m_timeProgress = 0;
    // Has the animation hit its end point or looped
    public bool m_hitAnimEnd = false;
	
    // Is the time position of the animation controlled by a control variable?
    public bool m_animTimeControlled = false;
    // The control variable used to control the animation's time position
    public ControlVariable m_animTimeControlVar;
	
	// Manually set time position of animation
	public float m_timePosition = 0;
	
	public bool m_sendAnimationLoopRequest = false;
	
	public bool m_allowRequestToPropagate = false;
	
	// Is the speed of the animation controlled by a control variable?
	public bool animSpeedControlled = false;
	public float speedFactor = 1;
    // The control variable used to control the animation's time position
    public ControlVariable m_animSpeedControlVar;

	
	public AT_Animation() : base()
	{
        NodeType = AT_NodeType.kAnimation;
	}

/*	void Start()
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
    }*/

    public override void Init()
    {
        // Set up the currently active anim and AnimState
        ChangeActiveAnimation(m_animationName);

        if ( m_animState != null )
        {
            // If we're using a mixing transform, apply it now
            if (m_mixTransform != null)
            {
                m_animState.AddMixingTransform(m_mixTransform,true);
            }
			
			if ( m_mixTransform != null || m_blendMode == AnimationBlendMode.Additive )
			{
                // Animations using mixing transforms need to be on top of animations that aren't (in terms of animation layers)
                // This is not a great system and could malfunction depending on init order of other animations.
                // TODO: Find a better/more reliable method to accomplish this layering
                m_animationLayer = m_animationLayerCounter++;
			}

            // Apply our various properties to the new AnimState
            m_animState.layer = m_animationLayer + m_animationLayerOffset;
            m_animState.wrapMode = m_wrapMode;
            m_animState.time = 0;
			
			if ( _randomTimeStart )
			{
				m_animState.normalizedTime = Random.Range(0,1);	
			}
			
            m_animState.blendMode = m_blendMode;
			
			if ( m_animState.wrapMode == WrapMode.ClampForever )
			{
				m_animState.time = m_animState.length;
			}
        }
    }

    // Change what animation this node is currently using
    void ChangeActiveAnimation(string animName)
    {
        // Only do this is the application is not playing. We're going to be creating duplicates of animations and 
        // don't want those hanging around inside the editor
        if (Application.isPlaying)
        {
            if (m_animState != null)
            {
                // If our current animation is synced, remove it from the parent tree's sync list
                if (m_syncAnimation)
                {
                    m_ParentTree.RemoveSyncedAnimation(m_syncAnimationLayer, m_animState);
                }

                // Disable current AnimState
                m_animState.enabled = false;
                // Remove AnimState from our subject
                m_subject.RemoveClip(m_subject[m_actualAnimationName].clip);                
            }

            if (m_animationName != "")
            {
                // Create a unique name for our new AnimState
                m_actualAnimationName = m_animationName + "_" + m_animationLayer;
                // Duplicate the reference animation to create our unique AnimState
                m_subject.AddClip(m_subject[m_animationName].clip, m_actualAnimationName);
                m_animState = m_subject[m_actualAnimationName];
                
                // If our animation is synced, add it to the parent tree's sync list
                if (m_syncAnimation)
                {
                    m_ParentTree.AddSyncedAnimation(m_syncAnimationLayer, m_animState, this);
                }
            }
            else
            {
                m_animState = null;
            }
        }
    }

	

	public override void UpdateAnimation ( float timeDelta )
	{
        if (m_animState != null)
        {
			// If this animation is currently being blending into our final result
			if (ActualBlendWeight > 0)
            {
				if (!( m_animState.wrapMode == WrapMode.ClampForever && m_hitAnimEnd ))
				{
					// Recalculate each frame
					m_hitAnimEnd = false;
					
					// Has the animation just been blended in and we're not syncing
					if ( m_animState.wrapMode == WrapMode.ClampForever )
					{
						//Debug.Log(m_animState.time);
						//Debug.Log("clamp forever: " + m_animationName );
						//m_animState.time = m_animState.length;
					}
					else if (!m_animState.enabled && !m_syncAnimation)
	                {
						// Start anim from begining
						m_animState.time = 0;
						m_timeProgress = 0;
					}
	
	                m_animState.enabled = true;
	
	                // Check whether we've passed the end point of the anim and store a bool if we have (used for sending requests on anim loop)
					float progress = m_animState.time % m_animState.length;
					progress += timeDelta * m_animState.speed;
					
					if (progress < m_timeProgress)
	                {
						if ( m_animState.wrapMode == WrapMode.ClampForever )
						{
							progress = m_animState.length;
						}
						
						m_hitAnimEnd = true;
						
						if ( m_sendAnimationLoopRequest )
						{
							AT_ContainerNode cont = Parent as AT_ContainerNode;
						
							if (cont != null)
							{
								cont.SendRequest ("animLoop_" + m_animationName, m_allowRequestToPropagate);
							}
							
							/*if ( m_sendAnimationLoopRequest )
							{
								ParentTree.SendExternalRequest( "animLoop_" + m_animationName );
							}*/
						}
						
	                }
	                m_timeProgress = progress;
	
	                // If we are using a control variable to set the anim time, do it now
	                if (m_syncAnimation)
	                {
						m_timePosition = m_animState.normalizedTime;
	                }				
					else if (m_animTimeControlled)
	                {
						// Need to set speed to 0. Otherwise the character will twitch as it fights against our control
	                	m_animState.speed = 0;
						if ( m_animTimeControlVar != null )
						{
	                    	m_animState.normalizedTime = m_animTimeControlVar.NormalizedValue;
						}
						else
						{
							m_timePosition = Mathf.Clamp01(m_timePosition);
							m_animState.normalizedTime = m_timePosition;
						}
						m_timeProgress = m_animState.time;
	                }
					else if ( animSpeedControlled )
					{
						if ( m_animSpeedControlVar != null )
						{
	                    	speedFactor = m_animSpeedControlVar.Value;
						}
						m_animState.speed = speedFactor;// * GameTime.timeScale;
					}
	                else
	                {
						
	                    m_animState.speed = 1;// * GameTime.timeScale;
	                }
	            }
	            
			}
			else
            {
                // Anim is not being used, so disable and reset it
                m_animState.enabled = false;
                m_timeProgress = 0;
                m_hitAnimEnd = false;
            }
			
			m_animState.time = m_timeProgress;
            // Apply our calculated blend weight to the AnimState
            m_animState.weight = ActualBlendWeight * m_weightMultiplier;
        }

	}
}
