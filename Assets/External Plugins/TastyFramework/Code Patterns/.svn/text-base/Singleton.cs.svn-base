using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

// Normal singleton base class
public abstract class Singleton<T> where T : class, new()
{
    // Singleton data
    protected static T _instance = null;
    public static T instance
    {
        get
        {
            if ( _instance == null )
            {
                _instance = new T();
            }
            return _instance;
        }
    }
}

// Singleton base class for MonoBehaviours
public abstract class SingletonBehaviour<T> : MonoBehaviour 
                                              where T : MonoBehaviour
{
    protected static T _instance;
    public static T instance
    {
        get { return _instance; }
    }

    public static bool hasInstance
    {
        get { return ( SingletonBehaviour<T>.instance != null ); }
    }

    public virtual void Awake()
    {
        if ( _instance == null )
        {
            _instance = this as T;
        }
        else
        {
            GameObject.Destroy(this.gameObject);
            return;
        }
    }

    public virtual void OnDestroy()
    {
        if (_instance == this as T)
        {
            _instance = null;
        }
    }
}

// Singleton base class for ScriptableObject
public abstract class ScriptableObjectSingleton<T> : ScriptableObject where T : ScriptableObject
{
	protected static T _instance = null;
	
	public static T instance
	{
		get
		{
			if(_instance != null)
			{
				return _instance;
			}
			
			_instance = Resources.Load(typeof(T).Name, typeof(T)) as T;
			
			if(_instance == null)
			{
#if UNITY_EDITOR
				Debug.LogWarning(typeof(T).Name + " resource does not exist. Creating in Assets/Resources");
				_instance = ScriptableObject.CreateInstance<T>();
				
				System.IO.DirectoryInfo directory = new System.IO.DirectoryInfo("Assets/Resources");
				if(!directory.Exists)
				{
					directory.Create();
				}
				
				AssetDatabase.CreateAsset(_instance, "Assets/Resources/" + typeof(T).Name + ".asset");
				AssetDatabase.SaveAssets();
#else		
				Debug.LogError("Error getting the " + typeof(T).Name + " resource");
#endif
			}
			
			return _instance;
		}
	}
}

// A singleton behaviour that will create itself if it doesn't already exist
public abstract class AutoSingletonBehaviour<T> : MonoBehaviour 
                                              	  where T : MonoBehaviour
{
    protected static T _instance = null;
    public static T instance
    {
        get 
		{ 
			if ( _instance == null )
			{
				GameObject go = GameObject.Find("_Singletons");
				if ( go == null )
				{
					go = new GameObject("_Singletons");
				}					
				GameObject newGo = new GameObject("" + typeof(T));
				newGo.transform.parent = go.transform;
				_instance = newGo.AddComponent<T>();
			}
			return _instance; 
		}
    }
	
	public static bool hasInstance
    {
        get { return ( AutoSingletonBehaviour<T>.instance != null ); }
    }
   
    public virtual void Awake()
    {
        if ( _instance == null )
        {
            _instance = this as T;
        }
        else
        {
            GameObject.Destroy(this.gameObject);
            return;
        }
    }

    public virtual void OnDestroy()
    {
        if (_instance == this as T)
        {
            GameObject.Destroy(_instance.gameObject);
			_instance = null;			
        }
    }
}