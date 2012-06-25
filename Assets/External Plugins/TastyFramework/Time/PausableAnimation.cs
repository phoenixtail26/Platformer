using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PausableAnimationState
{
	public AnimationState state;
	float _speed = 1;
	
	public float speed 
	{
		get 
		{
			return _speed;
		}
		
		set
		{
			_speed = value;
			Update();
		}
	}
	
	public PausableAnimationState( AnimationState state )
	{
		this.state = state;
	}
	
	public void Update()
	{
		state.speed = _speed * GameTime.timeScale;
	}
}

public class PausableAnimation : MonoBehaviour
{
	Animation _animation;
	public new Animation animation
	{
		get
		{
			return _animation;
		}
	}
	
	Dictionary<string, PausableAnimationState> _animationStates;
	
	public PausableAnimationState this[string key]
	{
		get
		{
			if ( animation != null )
			{
				if ( !_animationStates.ContainsKey(key) )
				{
					AnimationState state = animation[key];
					if ( state != null )
					{
						_animationStates.Add(key, new PausableAnimationState(state));
					}
					else
					{
						return null;
					}
				}
				
				return _animationStates[key];
			}
			
			return null;
		}
	}
	
	void Awake()
	{
		_animation = GetComponent<Animation>();
		if ( animation != null )
		{
			// Fill dictionary with current animation states
			_animationStates = new Dictionary<string, PausableAnimationState>();
			foreach ( AnimationState state in animation )
			{
				if ( state != null )
				{
					_animationStates.Add(state.name, new PausableAnimationState(state));
				}				
			}
		}
	}
	
	
	// Update is called once per frame
	void Update () 
	{
		foreach( KeyValuePair<string, PausableAnimationState> kvp in _animationStates )
		{			
			kvp.Value.Update();
		}
	}
}
