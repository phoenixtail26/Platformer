using UnityEngine;
using System.Collections;

public enum QualityLevel
{
	Low,
	Medium,
	High
}

public class GameInfo : SingletonBehaviour<GameInfo>
{
	public QualityLevel qualityLevel = QualityLevel.Medium;
	
	public override void Awake () 
	{
		base.Awake();
		
		if ( Application.isEditor )
		{
		}
		else
		{
			qualityLevel = QualityLevel.Low;
#if UNITY_IPHONE
			switch( iPhone.generation )
			{
			case iPhoneGeneration.iPad2Gen:
			case iPhoneGeneration.iPhone4S:
				qualityLevel = QualityLevel.Medium;
				break;
			
			default:
				qualityLevel = QualityLevel.Low;
				break;
			}
#endif
		}
		
	}
}
