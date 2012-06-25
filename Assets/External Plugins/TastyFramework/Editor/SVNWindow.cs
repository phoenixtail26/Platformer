using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class SVNWindow : EditorWindow 
{
	[MenuItem ("Window/SVN")]
    static void ShowWindow () 
	{
        SVNWindow window = EditorWindow.GetWindow<SVNWindow>();
		window.autoRepaintOnSceneChange = true;
    }	
	
	private SVNUtils.SVNFileInfo _sceneInfo = null;
	private System.DateTime _checkedTime = System.DateTime.Now;
	private const int _checkFrequency = 30;
	private SVNConfig _config = null;
	private bool _showConfig = false;
	private List<SVNUtils.SVNFileInfo> _selectionInfo = null;
	private bool _errorConnecting = false;
	
	private string GetSceneName()
	{
		return  "\"" + EditorApplication.currentScene + "\"";
	}
	 
	void UpdateSelectionInfo()
	{ 
		_selectionInfo = new List<SVNUtils.SVNFileInfo>();
		
		if(_errorConnecting)
			return;
		
		foreach(GameObject obj in Selection.gameObjects)
		{
			Object prefab = null;
			
			if(PrefabUtility.GetPrefabType(obj) == PrefabType.Prefab)
			{
				prefab = obj;
			}
			else
			{
				prefab = PrefabUtility.GetPrefabParent(obj);
			}
			
			if(prefab != null)
			{
				string assetPath = AssetDatabase.GetAssetPath(prefab);
				
				if(assetPath != null && assetPath != "")
				{
					SVNUtils.SVNFileInfo prefabInfo = SVNUtils.GetStatus(_config, "\"" + assetPath + "\"");
					
					if(prefabInfo == null)
					{
						_errorConnecting = true;
						_selectionInfo.Clear();
						return;
					}
					
					_selectionInfo.Add(prefabInfo);
				}
			}
		}
	}
	
	void UpdateSceneInfo()
	{
		if(EditorApplication.currentScene != "")
		{
			_sceneInfo = SVNUtils.GetStatus(_config, GetSceneName());
			_errorConnecting = (_sceneInfo == null);
		}
		else
		{
			_sceneInfo = null;
			_errorConnecting = false;
		}
		
		_checkedTime = System.DateTime.Now;
	}
		
	void OnSelectionChange()
	{
		if(!_errorConnecting && _config != null && _config.disablePrefabChecking == false)
		{
			UpdateSelectionInfo();
			this.Repaint();
		}
	}
	
	void Update()
	{
		if(!_errorConnecting && (System.DateTime.Now - _checkedTime).Seconds >= _checkFrequency )
		{
			UpdateSelectionInfo();
			UpdateSceneInfo();
			
			this.Repaint();
		}
	}
	
	void OnGUI()
	{
		if(_config == null)
		{
			_config = Resources.Load("SVNConfig", typeof(SVNConfig)) as SVNConfig;
		
			if(_config == null)
			{
				_config = ScriptableObject.CreateInstance<SVNConfig>();
				
				System.IO.DirectoryInfo resourcesDirectory = new System.IO.DirectoryInfo("Assets/Editor/Resources");
				if(!resourcesDirectory.Exists)
				{
					resourcesDirectory.Create(); 
				}
				AssetDatabase.CreateAsset(_config, "Assets/Editor/Resources/SVNConfig.asset");
				AssetDatabase.SaveAssets();
			}
		}
		
		string sceneName = GetSceneName();
		
		if(GUILayout.Button("Refresh"))
		{
			UpdateSceneInfo();
			UpdateSelectionInfo();
		}
		
		if(_errorConnecting)
		{
			EditorGUILayout.HelpBox("There was an error connecting to the subversion system", MessageType.Error, true);
		}
		else if(EditorApplication.currentScene == "")
		{
			_sceneInfo = null;
			EditorGUILayout.HelpBox("No subversion meta-data is available until the scene is saved", MessageType.Warning, true);
		}
		else
		{
			if(_sceneInfo == null || _sceneInfo.filePath != sceneName)
			{
				UpdateSceneInfo();
				UpdateSelectionInfo();
			}
			
			if(_sceneInfo != null)
			{
				if(_sceneInfo.outOfDate)
				{
					EditorGUILayout.HelpBox("This scene is out of date and should be updated. Last modified by " + _sceneInfo.lastChangedAuthor, MessageType.Error, true);
				}
				else if(_sceneInfo.locked)
				{
					string unlockButtonText = "Unlock scene";
					
					if(_sceneInfo.lockOwner != _config.username)
					{
						EditorGUILayout.HelpBox("This scene is currently locked by " + _sceneInfo.lockOwner, MessageType.Error, true);
						unlockButtonText = "Force unlock scene";
					}
					else
					{
						EditorGUILayout.HelpBox("You have locked this scene", MessageType.Info, true);
					}
					
					if(GUILayout.Button(unlockButtonText))
					{
						string message = "";
						bool result = SVNUtils.Unlock(_config, sceneName, out message);
						
						if(!result)
						{
							EditorUtility.DisplayDialog("Unlock failed", "Unlock failed with the following message: " + message, "OK");
						}
						else
						{
							if(message != "")
							{
								EditorUtility.DisplayDialog("Unlock successful", "Unlock succeeded with the following message: " + message, "OK");
							}
						}
						
						UpdateSceneInfo();
					}
				}
				else 
				{
					bool allowLock = false;
					
					switch(_sceneInfo.status)
					{
					case SVNUtils.ChangeStatus.Modified:
						EditorGUILayout.HelpBox("Scene is modified but not locked It is recommended to lock the scene",  MessageType.Warning, true);
						allowLock = true;
						break;
					case SVNUtils.ChangeStatus.NoModifications:
						EditorGUILayout.HelpBox("It is recommended to lock the scene if you are planning on changing anything",  MessageType.Info, true);
						allowLock = true;
						break;
					case SVNUtils.ChangeStatus.Conflicted:
						EditorGUILayout.HelpBox("Scene is in a conflicted state!",  MessageType.Error, true);
						break;
					case SVNUtils.ChangeStatus.Unversioned:
						EditorGUILayout.HelpBox("Scene has not been added to the respository",  MessageType.Warning, true);
						if(GUILayout.Button("Add scene to repository"))
						{
							string message = "";
							bool result = SVNUtils.Add(_config, sceneName, out message);
							
							if(!result)
							{
								EditorUtility.DisplayDialog("Add failed", "Add failed with the following message: " + message, "OK");
							}
							else
							{
								if(message != "")
								{
									EditorUtility.DisplayDialog("Add successful", "Add succeeded with the following message: " + message, "OK");
								}
							}
							
							UpdateSceneInfo();							
							Repaint();
						}
						break;
					case SVNUtils.ChangeStatus.Added:
						EditorGUILayout.HelpBox("Scene has been marked for add",  MessageType.Info, true);
						break;
					}
						
					if(allowLock && GUILayout.Button("Lock scene"))
					{
						string message = "";
						bool result = SVNUtils.Lock(_config, sceneName, out message);
						
						if(!result)
						{
							EditorUtility.DisplayDialog("Lock failed", "Lock failed with the following message: " + message, "OK");
						}
						else
						{
							if(message != "")
							{
								EditorUtility.DisplayDialog("Lock successful", "Lock succeeded with the following message: " + message, "OK");
							}
						}
						
						UpdateSceneInfo();
					}
				}
				
				if(_sceneInfo.status != SVNUtils.ChangeStatus.Unversioned && _sceneInfo.status != SVNUtils.ChangeStatus.Added)
				{
					EditorGUILayout.LabelField("Scene was last modified by " + _sceneInfo.lastChangedAuthor);
				}
			}
			else
			{
				EditorGUILayout.HelpBox("No subversion meta-data found for this scene file or error connecting to the server", MessageType.Error, true);
			}
			
		}
		
		if(!_errorConnecting)
		{
			if(_selectionInfo == null)
			{
				UpdateSelectionInfo();			
			}
		}
		else
		{
			_selectionInfo = new List<SVNUtils.SVNFileInfo>();
		}
			
		foreach(SVNUtils.SVNFileInfo prefabInfo in _selectionInfo)
		{
			if(prefabInfo.outOfDate)
			{
				string prefabName = prefabInfo.filePath;
				
				if(prefabInfo.status == SVNUtils.ChangeStatus.Modified)
				{
					EditorGUILayout.HelpBox("You have made changes to the prefab '" + prefabName + "' and it is out of date. Next update will be a conflict. Last modified by " + prefabInfo.lastChangedAuthor,  MessageType.Error, true);
				}
				else
				{
					EditorGUILayout.HelpBox("Prefab '" + prefabName + "' is out of date. Last modified by " + prefabInfo.lastChangedAuthor,  MessageType.Warning, true);
				}
			}
		}
				
		EditorGUILayout.BeginVertical("Box");
		_showConfig = EditorGUILayout.Foldout(_showConfig, "SVN Configuration");
		
		if(_showConfig)
		{
			// Username / password
			GUILayout.Label("Username:");
			_config.username = GUILayout.TextField(_config.username);
			GUILayout.Label("Password:");
			_config.password = GUILayout.PasswordField(_config.password, '*');
			if(GUILayout.Button("Select SVN executable"))
			{
				string extension = "";
				if(Application.platform == RuntimePlatform.WindowsEditor)
				{
					extension = "exe";
				}
				_config.svnPath = EditorUtility.OpenFilePanel("Select SVN executable", _config.svnPath, extension);
			}
			
			_config.enableDebugging = GUILayout.Toggle(_config.enableDebugging, "Enable debugging");
		}
		EditorGUILayout.EndVertical();
			
		EditorUtility.SetDirty(_config);

		
		
		
	}
}
