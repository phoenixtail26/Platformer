using UnityEngine;
using System.Collections;

public class CoroutineUtils : AutoSingletonBehaviour<CoroutineUtils>
{
	// Use like this:
	// yield return StartCoroutine(CoroutineUtils.UntilTrue(() => (lives > 3)));
	public static IEnumerator UntilTrue(System.Func<bool> fn)
	{
		while(!fn()) 
		{
			yield return null;
		}
	}
	
	// yield return StartCoroutine(CoroutineUtils.WhileAnimating(animation));
	public static IEnumerator WhileAnimating(Animation animation)
	{
		while(animation.isPlaying)
		{
			yield return null;
		}
	}
	
	// A wait function using GameTime instead of Time
	public static IEnumerator WaitForGameSecondsCoroutine( float time )
	{
		time = Mathf.Max(time, 0);
		float timeElapsed = 0;
		while ( timeElapsed < time )
		{
			timeElapsed += GameTime.deltaTime;
			
			yield return 0;
		}
	}
	
	public static Coroutine WaitForGameSeconds( float time )
	{
		return CoroutineUtils.instance.StartCoroutine(WaitForGameSecondsCoroutine(time));		
	}
	
	// yield return StartCoroutine(CoroutineUtils.OnNextFrame(() => { doSomething() } ));
	public static IEnumerator OnNextFrame(System.Action fn)
	{
		yield return null;
		fn();
	}
}
