using System;
using System.Collections.Generic;
using UnityEngine;
using Framework;

public class GameTime : AutoSingletonBehaviour<GameTime>
{
	[SerializeField]
    bool _paused = false;
	
	float _gameDeltaTime = 0;
	
	[SerializeField]
	public float _timeScale = 1;
	
	float _timeScaleBeforePause = 1;
	
	public bool sendPauseEvents = false;
	
    public static bool isPaused
    {
        get 
		{ 
			return GameTime.instance._paused; 
		}
		
        set 
		{ 
			GameTime.instance.Pause(value);
		}
    }


    public static float deltaTime
    {
        get
        {
			if ( !GameTime.instance._paused )
        	{
            	return GameTime.instance._gameDeltaTime;
        	}
            else
            {
                return 0;
            }
        }
    }
	
	public static float timeScale
	{
		get
		{
			return GameTime.instance._timeScale;
		}
		
		set
		{
			if ( GameTime.isPaused )
			{
				GameTime.instance._timeScaleBeforePause = value;
			}
			else
			{
				GameTime.instance._timeScale = value;
			}
		}
	}
	
	void Pause( bool value )
	{
		if ( value )
		{
			_timeScaleBeforePause = _timeScale;
			_timeScale = 0;
		}
		else
		{
			_timeScale = _timeScaleBeforePause;
		}
			
		bool pauseValueChanged = (_paused != value);
		
		_paused = value; 
		
		if ( sendPauseEvents && pauseValueChanged )
		{
			GameObject[] objects = GameObject.FindSceneObjectsOfType( typeof(GameObject) ) as GameObject[];
			foreach( GameObject obj in objects )
			{
				string message = "OnPause";
				if ( !value )
				{
					message = "OnUnpause";
				}
				
				obj.SendMessage( message, SendMessageOptions.DontRequireReceiver );
			}
		}
	}
	
	void Update()
	{
		_gameDeltaTime = Time.deltaTime * _timeScale;
	}
}
