using UnityEngine;
using System.Collections;
using Framework;

public class FrameworkObject : SingletonBehaviour<FrameworkObject> 
{ 	
    public override void Awake()
    {
        if ( FrameworkObject.hasInstance )
        {
            GameObject.Destroy( this.gameObject );
            return;
        }

        base.Awake();

        DontDestroyOnLoad( this.gameObject );
		
		// Maybe move this somewhere else
		//Debug.Log("Application frame rate is " + Application.targetFrameRate);
		Application.targetFrameRate = 60;
    }
	
	public void PurgeUnusedAssets()
	{
		Debug.Log("Unloading unused assets...");
		Resources.UnloadUnusedAssets();
	}
    
} 