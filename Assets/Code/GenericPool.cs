using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GenericPool<T> where T : MonoBehaviour
{
	static Transform _allPoolsRoot;
	
	Transform _poolRoot;
	
	T _prefab = null;
	
	List<T> _availableItems = new List<T>();
	List<T> _usedItems  = new List<T>();
	
	private GenericPool( T prefab, int numPoolItems = 10 )
	{
		if ( _allPoolsRoot == null )
		{
			GameObject go = new GameObject("_Pools");
			_allPoolsRoot = go.transform;
		}
		
		_prefab = prefab;
		GameObject poolObj = new GameObject( prefab.GetType().ToString() + " Pool" );
		
		_poolRoot = poolObj.transform;
		_poolRoot.parent = _allPoolsRoot;
		
		SetPoolSize(numPoolItems);
	}
	
	public void SetPoolSize( int numItems )
	{
		if ( numItems < 0 )
		{
			Debug.LogError("GenericPool: Cannot set pool size to less than zero");
			return;
		}
		
		int currentItemNumber = _availableItems.Count + _usedItems.Count;
		int numNeededItems = numItems - currentItemNumber;
		
		if ( numNeededItems < 0 )
		{
			// Remove unwanted items
			int numToRemove = Mathf.Abs(numNeededItems);
			
			// First remove items that aren't in use
			int availableToRemove = Mathf.Min(numToRemove, _availableItems.Count);
			for ( int i = _availableItems.Count - availableToRemove; i < _availableItems.Count; i++ )
			{
				GameObject.Destroy(_availableItems[i].gameObject);
			}
			_availableItems.RemoveRange(_availableItems.Count - availableToRemove, availableToRemove);
			numToRemove -= availableToRemove;
			
			// Next remove from in use items if needed
			for ( int i = 0; i < numToRemove; i++ )
			{
				GameObject.Destroy(_usedItems[i]);
			}
			_usedItems.RemoveRange(0, numToRemove);
		}
		else
		{
			// Add new items
			for ( int i = 0; i < numNeededItems; i++ )
			{
				T newItem = GameObject.Instantiate(_prefab) as T;
				newItem.name += " " + (currentItemNumber + i);
				newItem.transform.parent = _poolRoot;
				newItem.gameObject.SetActive(false);
				_availableItems.Add(newItem);
			}
		}
	}
	
	
	public T GetItem()
	{
		T item = null;
		if ( _availableItems.Count > 0 )
		{
			// Take from the end of the list
			item = _availableItems[_availableItems.Count-1];
			_availableItems.RemoveAt(_availableItems.Count-1);
		}
		else
		{
			// Remove the item that's been around the longest (ie: the first in the list)
			item = _usedItems[0];
			_usedItems.RemoveAt(0);
		}
		
		_usedItems.Add(item);
		item.gameObject.SetActive(true);
		item.SendMessage("PoolInstantiate", SendMessageOptions.DontRequireReceiver);
		item.transform.parent = null;
		return item;
	}
	
	public void ReturnItem( T item )
	{
		item.SendMessage("PoolDestroy", SendMessageOptions.DontRequireReceiver);
		item.gameObject.SetActive(false);
		_usedItems.Remove(item);
		_availableItems.Add(item);
		item.transform.parent = _poolRoot;
	}
	
	public static GenericPool<T> CreatePool( T prefab, int numPoolItems = 10 )
	{
		GenericPool<T> pool = new GenericPool<T>( prefab, numPoolItems );
		return pool;
	}
	
	
	
}
