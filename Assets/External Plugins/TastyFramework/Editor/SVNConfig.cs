using UnityEngine;
using System.Collections;

public class SVNConfig : ScriptableObject 
{
	public string username = "Enter username here";
	public string password = "";
	public string svnPath = "/usr/bin/svn";
	public bool enableDebugging = false;
	public bool disablePrefabChecking = false;
}
