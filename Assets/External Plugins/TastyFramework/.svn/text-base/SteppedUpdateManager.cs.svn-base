using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Stepped;

public delegate void SteppedUpdateFunction(float timeDelta);

namespace Stepped
{
	internal enum UpdateType
	{
		Update,
		LateUpdate
	}
	
	internal class SteppedFunctionWrapper
	{
		public SteppedUpdateFunction function;
		public float timeDeltaSinceLastUpdate = 0;
		public bool useGameTime = false;
	}
	
	internal class SteppedDetails
	{
		public List<SteppedFunctionWrapper> functions = new List<SteppedFunctionWrapper>();
		public float framesBetweenUpdates = 1.0f;
		public System.Type type;
		float _numToExecuteEachFrame = 0;
		float _totalExecutionCounter = 0;
		int _executionCounter = 0;
			
		public void AddFunction( SteppedUpdateFunction function, bool useGameTime = false )
		{
			SteppedFunctionWrapper wrapper = new SteppedFunctionWrapper();
			wrapper.function = function;
			wrapper.useGameTime = useGameTime;
			functions.Add(wrapper);
			_numToExecuteEachFrame = functions.Count / framesBetweenUpdates;
			_totalExecutionCounter = 0;
			_executionCounter = 0;
		}
		
		public void RemoveFunction( SteppedUpdateFunction function )
		{
			int indexToRemove = -1;
			for ( int i = 0; i < functions.Count; i++ )
			{
				if ( functions[i].function == function )
				{
					indexToRemove = i;
					break;
				}
			}
			
			if ( indexToRemove != -1 )
			{
				//Debug.LogWarning("removing function");
				functions.RemoveAt(indexToRemove);
				_numToExecuteEachFrame = functions.Count / framesBetweenUpdates;
				_totalExecutionCounter = 0;
				_executionCounter = 0;
			}
		}
		
		public void ExecuteSteppedUpdate()
		{
			int totalFunctions = functions.Count;
			if ( totalFunctions == 0 )
			{
				return;
			}
			
			for ( int i = 0; i < totalFunctions; i++ )
			{
				if ( functions[i].useGameTime )
				{
					functions[i].timeDeltaSinceLastUpdate += GameTime.deltaTime;
				}
				else
				{
					functions[i].timeDeltaSinceLastUpdate += Time.deltaTime;
				}
			}
			
			if ( _numToExecuteEachFrame == 0 )
			{
				_numToExecuteEachFrame = 1;
			}
			
			_totalExecutionCounter += _numToExecuteEachFrame;
			int targetExecutionNumber = Mathf.FloorToInt(_totalExecutionCounter);
			
			while ( _executionCounter < targetExecutionNumber )
			{
				//Debug.Log("Executing stepped update: " + type.ToString());
				Profiler.BeginSample( type.ToString() );
				try
				{
					int index = 0;
					if ( totalFunctions > 0 )
					{
						index = _executionCounter%totalFunctions;
					}
					SteppedFunctionWrapper wrapper = functions[index];
					wrapper.function(wrapper.timeDeltaSinceLastUpdate);
					wrapper.timeDeltaSinceLastUpdate = 0;
				}
				catch ( System.Exception exception )
				{
					Debug.LogError(exception);
				}
				
				Profiler.EndSample();
				_executionCounter++;
			}
			
			if (_executionCounter >= functions.Count)
			{
				_executionCounter = 0;
				_totalExecutionCounter = 0;
			}
		}
	}
	
	internal class SteppedCollection
	{
		Dictionary<float,SteppedDetails> _collection = new Dictionary<float, SteppedDetails>();
		
		public void Add( System.Type type, SteppedUpdateFunction function, float framesBetweenUpdates, bool useGameTime = false )
		{
			if ( framesBetweenUpdates == 0 )
			{
				Debug.LogError("framesBetweenUpdates was zero!");
				return;
			}
			
			SteppedDetails details = null;
			
			RemoveFunction(function);
			
			if ( !_collection.ContainsKey(framesBetweenUpdates) )
			{
				details = new SteppedDetails();
				details.framesBetweenUpdates = framesBetweenUpdates;
				details.type = type;
				_collection.Add(framesBetweenUpdates, details);
			}
			
			details = _collection[framesBetweenUpdates];
			
			details.AddFunction(function, useGameTime);
		}
		
		public void RemoveFunction(SteppedUpdateFunction function)
		{
			foreach( KeyValuePair<float,SteppedDetails> kvp in _collection )
			{
				kvp.Value.RemoveFunction(function);
			}
		}
		
		public void ExecuteSteppedUpdate()
		{
			foreach( KeyValuePair<float,SteppedDetails> kvp in _collection )
			{
				kvp.Value.ExecuteSteppedUpdate();
			}
		}
	}
}


public class SteppedUpdateManager : SingletonBehaviour<SteppedUpdateManager>
{
	Dictionary<System.Type, SteppedCollection> _registeredCollections = new Dictionary<System.Type, SteppedCollection>();
	Dictionary<System.Type, SteppedCollection> _registeredLateCollections = new Dictionary<System.Type, SteppedCollection>();
	
	public void RegisterSteppedUpdate( System.Type type, SteppedUpdateFunction function, float framesBetweenUpdates, bool useGameTime = false )
	{
		RegisterSteppedFunction( type, function, framesBetweenUpdates, UpdateType.Update, useGameTime );
	}
	
	public void RegisterSteppedLateUpdate( System.Type type, SteppedUpdateFunction function, float framesBetweenUpdates, bool useGameTime = false )
	{
		RegisterSteppedFunction( type, function, framesBetweenUpdates, UpdateType.LateUpdate, useGameTime );
	}
	
	void RegisterSteppedFunction( System.Type type, SteppedUpdateFunction function, float framesBetweenUpdates, UpdateType updateType, bool useGameTime = false )
	{
		//Debug.LogWarning("Registering type: " + type.ToString());
		SteppedCollection collection = null;
		Dictionary<System.Type, SteppedCollection> collections = _registeredCollections;
		if ( updateType == UpdateType.LateUpdate )
		{
			collections = _registeredLateCollections;
		}		
		
		if ( !collections.ContainsKey(type) )
		{
			collection = new SteppedCollection();
			collections.Add(type, collection);
		}
		collection = collections[type];
		
		collection.Add( type, function, framesBetweenUpdates, useGameTime );
	}

	public void UnregisterSteppedUpdate( System.Type type, SteppedUpdateFunction function )
	{
		//Debug.LogWarning("Unregistering type: " + type.ToString());
		if ( _registeredCollections.ContainsKey(type) )
		{
			_registeredCollections[type].RemoveFunction(function);
		}
		if ( _registeredLateCollections.ContainsKey(type) )
		{
			_registeredLateCollections[type].RemoveFunction(function);
		}
	}
	
	public void Update()
	{
		foreach ( KeyValuePair<System.Type,SteppedCollection> kvp in _registeredCollections)
		{
			kvp.Value.ExecuteSteppedUpdate();
		}
	}
	
	public void LateUpdate()
	{
		foreach ( KeyValuePair<System.Type,SteppedCollection> kvp in _registeredLateCollections)
		{
			kvp.Value.ExecuteSteppedUpdate();
		}
	}
	
}
