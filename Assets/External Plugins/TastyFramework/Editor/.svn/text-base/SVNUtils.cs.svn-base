using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public static class SVNUtils
{
	public enum ChangeStatus
	{
		NoModifications,
		Added,
		Conflicted,
		Deleted,
		Ignored,
		Modified,
		Replaced,
		External,
		Unversioned,
		Missing,
		Obstructed
	}
	
	static ChangeStatus GetChangeStatus(char code)
	{
		switch(code)
		{
		case ' ':
			return ChangeStatus.NoModifications;
		case 'A':
			return ChangeStatus.Added;
		case 'C':
			return ChangeStatus.Conflicted;
		case 'D':
			return ChangeStatus.Deleted;
		case 'I':
			return ChangeStatus.Ignored;
		case 'M':
			return ChangeStatus.Modified;
		case 'R':
			return ChangeStatus.Replaced;
		case 'X':
			return ChangeStatus.External;
		case '?':
			return ChangeStatus.Unversioned;
		case '!':
			return ChangeStatus.Missing;
		case '~':
			return ChangeStatus.Obstructed;
			
		default:
			return ChangeStatus.NoModifications;
		}
	}
	
	public class SVNFileInfo
	{
		public string filePath;
		public ChangeStatus status;
		public bool outOfDate;
		public Dictionary<string, string> info;
		
		public string lastChangedAuthor
		{
			get
			{
				if(info.ContainsKey("Last Changed Author"))
					return info["Last Changed Author"];
				else return "[Unknown]";
			}
		}
		
		public bool locked
		{
			get 
			{
				return lockOwner != "";
			}
		}

		public string lockOwner
		{
			get
			{
				if(info.ContainsKey("Lock Owner"))
					return info["Lock Owner"].Replace(" ", "");
				else return "";
			}
		}
		
		public override string ToString()
		{
			return "SVN Status of " + filePath + ": status [" + status.ToString() + "] " + (outOfDate ? "[OUT OF DATE]" : "");
		}
	}
	
	private static bool StartProcess(SVNConfig config, System.Diagnostics.Process process, out string stdout, out string stderr)
	{
		if(config.enableDebugging)
		{
			Debug.Log(process.StartInfo.FileName + " " + process.StartInfo.Arguments);
		}
		
		bool result = false;
		
		try
		{
			result = process.Start();
		}
		catch(System.Exception e)
		{
			Debug.LogError(e.Message);
			stdout = "";
			stderr = e.Message;
			return false;
		}

		stdout = process.StandardOutput.ReadToEnd();
		stderr = process.StandardError.ReadToEnd();
		
		if(config.enableDebugging && stdout != "")
		{
			Debug.Log(stdout);
		}
		
		if(stderr != "")
		{
			Debug.LogError(stderr);
		}
		
		process.WaitForExit();
		
		return result;
	}
	
	private static System.Diagnostics.Process CreateSVNProcess(SVNConfig config)
	{
		if(config.svnPath == "")
		{
			if(Application.platform == RuntimePlatform.WindowsEditor)
			{
				config.svnPath = "svn.exe";
			}
			else
			{
				config.svnPath = "svn";
			}
		}
		
		System.Diagnostics.Process process = new System.Diagnostics.Process();
		process.StartInfo.FileName = config.svnPath;
		process.StartInfo.RedirectStandardOutput = true;
		process.StartInfo.RedirectStandardError = true;
		process.StartInfo.UseShellExecute = false;
		process.StartInfo.CreateNoWindow = true;
		return process;
	}
	
	private static Dictionary<string, string> GetSVNInfo(SVNConfig config, string filePath)
	{
		// Run an svn info
		System.Diagnostics.Process infoProcess = CreateSVNProcess(config);
		infoProcess.StartInfo.Arguments = "info " + filePath;
		
		string stdout, stderr;
		StartProcess(config, infoProcess, out stdout, out stderr);
		
		stdout = stdout.Replace("\r", "");
		string[] infoLines = stdout.Split('\n');
		Dictionary<string, string> output = new Dictionary<string, string>();
		string lastEntry = "";
		
		foreach(string line in infoLines)
		{
			int seperator = line.IndexOf(':');
			
			if(seperator == -1 && lastEntry != "")
			{
				output[lastEntry] += line;
			}
			else if(seperator != -1 && seperator + 2 < line.Length)
			{
				string key = line.Substring(0, seperator);
				string val = line.Substring(seperator + 2);
				
				output[key] = val;
				
				lastEntry = key;
			}
		}		
		
		return output;
	}
	
	public static bool Unlock(SVNConfig config, string filePath, out string message)
	{
		// Run an svn unlock
		System.Diagnostics.Process process = CreateSVNProcess(config);
		process.StartInfo.Arguments = "unlock --force --username " + config.username + " --password " + config.password + " " + filePath;

		string stdout, stderr;
		StartProcess(config, process, out stdout, out stderr);
		
		if(stderr.Length != 0)
		{
			message = stderr;
			return false;
		}
		
		message = stdout;
		return true;
	}
	
	public static bool Add(SVNConfig config, string filePath, out string message)
	{
		// Run an svn add
		System.Diagnostics.Process process = CreateSVNProcess(config);
		process.StartInfo.Arguments = "add --username " + config.username + " --password " + config.password + " " + filePath;

		string stdout, stderr;
		StartProcess(config, process, out stdout, out stderr);

		if(stderr.Length != 0)
		{
			message = stderr;
			return false;
		}
		
		message = stdout;
		return true;
	}
	
	public static bool Lock(SVNConfig config, string filePath, out string message)
	{
		// Run an svn lock
		System.Diagnostics.Process process = CreateSVNProcess(config);
		process.StartInfo.Arguments = "lock --username " + config.username + " --password " + config.password + " " + filePath;

		string stdout, stderr;
		StartProcess(config, process, out stdout, out stderr);
		
		if(stderr.Length != 0)
		{
			message = stderr;
			return false;
		}
		
		message = stdout;
		return true;
	}
	
	public static SVNFileInfo GetStatus(SVNConfig config, string filePath)
	{
		// Run an svn status
		System.Diagnostics.Process process = CreateSVNProcess(config);
		process.StartInfo.Arguments = "status -v -u " + filePath;

		string statusOutput, stderr;
		StartProcess(config, process, out statusOutput, out stderr);
		
		if(stderr.Length > 0)
		{
			return null;
		}

		SVNFileInfo status = new SVNFileInfo();
		status.filePath = filePath;
		
		if(statusOutput.Length > 9)
		{
			char[] fileFlags = statusOutput.ToCharArray(0, 9);
			status.status = GetChangeStatus(fileFlags[0]);
			status.outOfDate = fileFlags[8] == '*';
		}
		
		if(status.status == ChangeStatus.Unversioned || status.status == ChangeStatus.Added)
		{
			status.info = new Dictionary<string, string>();
		}
		else
		{
			status.info = GetSVNInfo(config, filePath);
			
			if(status.info.ContainsKey("URL"))
			{
				status.info = GetSVNInfo(config, status.info["URL"]);
			}
		}
		
		return status;
	}
}
