using UnityEngine;
using System.Collections;

public class GenericPool<T> : MonoBehaviour where T : MonoBehaviour
{
	static Transform _poolRoot;
	
	T _prefab = null;
	
	void Awake()
	{
		if (  _poolRoot == null )
		{
			GameObject go = new GameObject("_pools");
			_poolRoot = go.transform;
		}
		
		
	}
	
	public static GenericPool<T> CreatePool()
	{
		GameObject go = new GameObject("pool");
		GenericPool<T> pool = go.AddComponent<GenericPool<T>>() as GenericPool<T>;
		return pool;
	}
	
	public T Instantiate( T prefab )
	{
		if ( _prefab == null )
		{
			_prefab = prefab;
		}
		
		return null;
	}
	
}
