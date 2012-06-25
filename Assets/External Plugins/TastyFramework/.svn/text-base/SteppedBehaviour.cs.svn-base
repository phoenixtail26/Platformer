using UnityEngine;
using System.Collections;

namespace Framework
{						  
	public abstract class SteppedBehaviour : MonoBehaviour
	{
		[SerializeField]
		private float _defaultStepFrequency = 1.0f;
		
		public virtual float stepFrequency
		{
			get 
			{ 
				return _defaultStepFrequency; 
			}
			set
			{
				_defaultStepFrequency = value;
				
				if(this.enabled && SteppedUpdateManager.hasInstance)
				{
					//Debug.LogWarning("registering again: " + this.GetType().ToString());
					SteppedUpdateManager.instance.UnregisterSteppedUpdate( this.GetType(), SteppedUpdate);
					SteppedUpdateManager.instance.RegisterSteppedUpdate( this.GetType(), SteppedUpdate, stepFrequency );
				}
			}
		}
		
		void OnEnable()
		{
			if(SteppedUpdateManager.hasInstance)
			{
				SteppedUpdateManager.instance.RegisterSteppedUpdate( this.GetType(), SteppedUpdate, stepFrequency );
			}
		}
		
		void OnDisable()
		{
			if(SteppedUpdateManager.hasInstance)
			{
				//Debug.Log(this.GetType());
				SteppedUpdateManager.instance.UnregisterSteppedUpdate( this.GetType(), SteppedUpdate);
			}
		}
		
		protected abstract void SteppedUpdate(float timeDelta);
	}
}