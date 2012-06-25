using System;
using System.Collections.Generic;
using UnityEngine;
using Framework;

public class NonStopTime : SingletonBehaviour<NonStopTime>
{
    bool m_paused = false;
	float m_gameDeltaTime = 0;

    float m_lastTimeSinceStartup = 0;

    public static bool Paused
    {
        get { return NonStopTime.instance.m_paused; }
        set { NonStopTime.instance.m_paused = value; }
    }


    public static float deltaTime
    {
        get
        {
            NonStopTime gt = NonStopTime.instance;
            if ( !gt.m_paused )
            {
                return gt.m_gameDeltaTime;
            }
            else
            {
                return 0;
            }
        }
    }
	
	void Update()
	{

        m_gameDeltaTime = Time.realtimeSinceStartup - m_lastTimeSinceStartup;

        m_lastTimeSinceStartup = Time.realtimeSinceStartup;
	}

}
