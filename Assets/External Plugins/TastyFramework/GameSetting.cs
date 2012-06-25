/*
 * GameSetting class
 * A wrapper for the Unity PlayerPrefs that caches values and implicitly converts to the value it is wrapping
 * Also works for enums and booleans!
 * If the key is not found in the PlayerPrefs it is set to the default value
 * 
 * e.g.
 * GameSettingInt myGameSetting = new GameSettingInt("MY_SETTING_KEY", 5);	// Would create an int with MY_SETTING_KEY as the key, 5 as a default value
 * GameSettingEnum<MyEnumType> myEnumSetting = new GameSettingEnum<MyEnumType>("MY_ENUM_SETTING", MyEnumType.Foo);
 */

using UnityEngine;
using System.Collections;

namespace Framework
{
	// Generic base class
	public abstract class GameSetting<T> 
	{
		public GameSetting(string key, T defaultValue)
		{
			_key = key;
			_defaultValue = defaultValue;
			
			if(PlayerPrefs.HasKey(_key))
			{
				_cachedValue = GetPlayerPref();
			}
			else
			{
				_cachedValue = _defaultValue;
				Set(_cachedValue);
			}
		}
		
		public void Reset()
		{
			Set(_defaultValue);
		}
		
		public void Set(T newValue)
		{
			_cachedValue = newValue;
			SetPlayerPref(newValue);
		}
		
		public static implicit operator T (GameSetting<T> setting) 
		{ 
			return setting._cachedValue; 
		}
		
		protected abstract void SetPlayerPref(T newValue);
		protected abstract T GetPlayerPref();
		
		public override string ToString() 
		{
			return _cachedValue.ToString();
		}
		
		public T defaultValue { get { return _defaultValue; } }
		
		protected string _key;
		protected T _defaultValue;
		private T _cachedValue;
	}
	
	// Implementations for wrapping PlayerPrefs.Get/Set()
	
	public class GameSettingBool : GameSetting<bool>
	{
		public GameSettingBool(string key, bool defaultValue) : base(key, defaultValue) {}
			
		protected override void SetPlayerPref(bool newValue)
		{
			PlayerPrefs.SetInt(_key, newValue ? 1 : 0);
		}
		
		protected override bool GetPlayerPref()
		{
			return PlayerPrefs.GetInt(_key) != 0;
		}
	}

	public class GameSettingInt : GameSetting<int>
	{
		public GameSettingInt(string key, int defaultValue) : base(key, defaultValue) {}
			
		protected override void SetPlayerPref(int newValue)
		{
			PlayerPrefs.SetInt(_key, newValue);
		}
		
		protected override int GetPlayerPref()
		{
			return PlayerPrefs.GetInt(_key);
		}
	}

	public class GameSettingFloat : GameSetting<float>
	{
		public GameSettingFloat(string key, float defaultValue) : base(key, defaultValue) {}
			
		protected override void SetPlayerPref(float newValue)
		{
			PlayerPrefs.SetFloat(_key, newValue);
		}
		
		protected override float GetPlayerPref()
		{
			return PlayerPrefs.GetFloat(_key);
		}
	}

	public class GameSettingString : GameSetting<string>
	{
		public GameSettingString(string key, string defaultValue) : base(key, defaultValue) {}
			
		protected override void SetPlayerPref(string newValue)
		{
			PlayerPrefs.SetString(_key, newValue);
		}
		
		protected override string GetPlayerPref()
		{
			return PlayerPrefs.GetString(_key);
		}
	}

	public class GameSettingEnum<E> : GameSetting<E> where E : struct, System.IConvertible
	{
		public GameSettingEnum(string key, E defaultValue) : base(key, defaultValue) {}
			
		protected override void SetPlayerPref(E newValue)
		{
			int index = EnumToInt(newValue);
			PlayerPrefs.SetInt(_key, index);
			//PlayerPrefs.SetString(_key, newValue.ToString());
		}
		
		private int EnumToInt(E val)
		{
			System.Array values = System.Enum.GetValues(typeof(E));
			return System.Array.IndexOf(values, val);
		}
		
		protected override E GetPlayerPref()
		{
			System.Array values = System.Enum.GetValues(typeof(E));
			int defaultIndex = EnumToInt(_defaultValue);
			int index = PlayerPrefs.GetInt(_key, defaultIndex);
			
			if(index < values.Length)
			{
				return (E) values.GetValue(index);			
			}
			else
			{
				SetPlayerPref(_defaultValue);
				return _defaultValue;
			}
			
			
			/*try
			{
				E asValue = (E) System.Enum.Parse(typeof(E), PlayerPrefs.GetString(_key));
				return asValue;
			}
			catch(System.ArgumentException ae)
			{
				Debug.LogError(ae.ToString());
				
			}*/
		}
	}
	
	/*public class GameSettingArray<A> : GameSetting<A[]> where A : struct, System.IConvertible 
	{
		public GameSettingArray(string key, A[] defaultValue) : base(key, defaultValue) {}
			
		protected override void SetPlayerPref(A[] newValue)
		{
			if(newValue.Length == 0)
				return;
			
			byte[] valSize = System.BitConverter.GetBytes(newValue[0]);
			byte[] bytes = new byte[valSize.Length * newValue.Length];
			int index = 0;
			
			foreach(A val in newValues)
			{
				byte[] valBytes = System.BitConverter.GetBytes(val);
				System.Array.Copy(valBytes, 0, bytes, index, valBytes.Length);
				index += valBytes.Length;
			}
						
			PlayerPrefs.SetString(_key, System.Convert.ToBase64String(bytes));
		}
		
		protected override A[] GetPlayerPref()
		{
			string prefString = PlayerPrefs.GetString(_key);
			byte[] bytes = System.Convert.FromBase64String(prefString);
			
			int arraySize = bytes.Length / sizeof(A);
			A[] loadedArray = new A[arraySize];
			
			for(int x = 0; x<arraySize; x++)
			{
				//loadedArray[x] = System.BitConverter.
			}
			
			return _cachedValue;
//			return PlayerPrefs.GetString(_key);
		}		
	}*/
	
}
