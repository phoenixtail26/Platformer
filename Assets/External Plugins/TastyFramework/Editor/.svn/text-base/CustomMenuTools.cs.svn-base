using UnityEngine;
using UnityEditor;
using System.Collections;

public static class CustomMenuTools
{
	[MenuItem("Edit/Remove Prefab Connection")]
	static void RemovePrefabConnection()
	{
		bool canDisconnect = false;
		
		foreach(GameObject obj in Selection.gameObjects)
		{
			if(PrefabUtility.GetPrefabType(obj) != PrefabType.None)
			{
				canDisconnect = true;
			}
		}
		
		if(canDisconnect == false)
		{
			EditorUtility.DisplayDialog("Remove Prefab Connection", "No prefab selected!", "OK");
			return;
		}
		
		Undo.RegisterSceneUndo("Remove Prefab Connection");
		
		foreach(GameObject obj in Selection.gameObjects)
		{
			PrefabUtility.DisconnectPrefabInstance(obj);
		}
	}
	
	[MenuItem("Edit/Revert Prefab")]
	static void RevertPrefabConnection()
	{
		bool canRevert = false;
		
		foreach(GameObject obj in Selection.gameObjects)
		{
			if(PrefabUtility.GetPrefabType(obj) != PrefabType.None)
			{
				canRevert = true;
			}
		}
		
		if(canRevert == false)
		{
			EditorUtility.DisplayDialog("Revert Prefab", "No prefab selected!", "OK");
			return;
		}
		
		Undo.RegisterSceneUndo("Revert Prefab");
		
		foreach(GameObject obj in Selection.gameObjects)
		{
			PrefabUtility.RevertPrefabInstance(obj);
		}
	}
}
