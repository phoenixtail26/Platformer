using UnityEngine;
using System.Collections;

public class ShaderLOD : MonoBehaviour 
{
	public bool _forceFallback = false;
	private const int _highShaderValue = 300;
	private const int _lowShaderValue = 200;
	
	void Start () 
	{
		if(Application.isEditor)
		{
		}
		else
		{
			if ( GameInfo.hasInstance )
			{
				if(GameInfo.instance.qualityLevel >= QualityLevel.Medium)
				{
					Shader.globalMaximumLOD = _highShaderValue;
				}
				else
				{
					Shader.globalMaximumLOD = _lowShaderValue;
				}
			}
		}
		//Camera.main.SetReplacementShader(Shader.Find("Debug/Simple flat shaded"), "");
		//Shader.globalMaximumLOD = 100;
	}
	
#if UNITY_EDITOR
	void Update()
	{
		Shader.globalMaximumLOD = _forceFallback ? _lowShaderValue : _highShaderValue;
	}
#endif
}
