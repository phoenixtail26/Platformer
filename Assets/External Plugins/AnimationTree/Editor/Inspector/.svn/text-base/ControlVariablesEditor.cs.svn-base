using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(AT_ControlVariables))]
public class ControlVariablesEditor : CustomNodeEditor
{
	public override void OnInspectorGUI ()
	{
		EditorGUILayout.Separator ();

        EditorGUIUtility.LookLikeControls ();

        EditorGUI.indentLevel = 1;

        AT_ControlVariables node = (target as AT_ControlVariables);

        EditorGUILayout.BeginVertical ();

        List<ControlVariable> changedVars = new List<ControlVariable> ();
		List<ControlVariable> varsToRemove = new List<ControlVariable> ();
		List<string> newNames = new List<string> ();
		bool dirty = false;

        EditorGUILayout.BeginHorizontal ();
		GUILayout.Space (12);
		GUILayout.Label ("Name", GUILayout.ExpandWidth (true));
		GUILayout.Label ("Value", GUILayout.MaxWidth (40));
		GUILayout.Label ("Min", GUILayout.MaxWidth (40));
		GUILayout.Label ("Max", GUILayout.MaxWidth (40));
		GUILayout.Space (3);
		GUILayout.Label ("Delete", GUILayout.MaxWidth (37));
		EditorGUILayout.EndHorizontal ();


        foreach (ControlVariable cv in node.Variables)
        {
			if (cv != null)
			{
				EditorGUILayout.BeginHorizontal ();
				GUILayout.Space (12);
				//cv.m_value = EditorGUILayout.Slider(cv.m_variableName, cv.m_value, 0, 1, GUILayout.ExpandWidth(true));
				string name = EditorGUILayout.TextField (cv.m_variableName, GUILayout.ExpandWidth (true));
				if (name != cv.m_variableName)
            	{
					changedVars.Add (cv);
					newNames.Add (name);
					dirty = true;
				}

            	cv.Value = EditorGUILayout.FloatField (cv.Value, GUILayout.MaxWidth (40));
				cv.MinValue = EditorGUILayout.FloatField (cv.MinValue, GUILayout.MaxWidth (40));
				cv.MaxValue = EditorGUILayout.FloatField (cv.MaxValue, GUILayout.MaxWidth (40));

            	GUILayout.Space (10);

            	if (GUILayout.Button ("-", GUILayout.MaxWidth (20)))
            	{
					varsToRemove.Add (cv);
					dirty = true;
				}

            	GUILayout.Space (10);

            	EditorGUILayout.EndHorizontal ();
			}
		}

        /*foreach (ControlVariable cv in node.Variables)
	       {
	           EditorGUILayout.BeginHorizontal();
	           GUILayout.Space(10);
	           if (GUILayout.Button("-", GUILayout.MaxWidth(20)))
	           {
	               varsToRemove.Add(cv);
	               dirty = true;
	           }
	           EditorGUILayout.EndHorizontal();
	       }
	       EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();*/

        /*foreach (KeyValuePair<string, ControlVariable> kvp in node.ParentTree.ControlVariables)
	       {
	           ControlVariable cv = kvp.Value;

            EditorGUILayout.BeginHorizontal();
	           GUILayout.Space(12);
	           //cv.m_value = EditorGUILayout.Slider(cv.m_variableName, cv.m_value, 0, 1, GUILayout.ExpandWidth(true));
	           string name = EditorGUILayout.TextField(cv.m_variableName, GUILayout.ExpandWidth(true));
	           if (name != cv.m_variableName)
	           {
	               changedVars.Add(cv);
	               newNames.Add(name);
	           }
	           EditorGUILayout.EndHorizontal();
	       }*/

        
        for (int i = 0; i < changedVars.Count; i++)
        {
			node.RenameVariable (changedVars[i], newNames[i]);
			AnimationTreeEditor.instance.RefreshVariablesWindow ();
		}

        foreach (ControlVariable cv in varsToRemove)
        {
			node.RemoveVariable (cv);
		}

        if (GUILayout.Button ("Add Variable", GUILayout.MaxWidth (100)))
        {
			ControlVariable cv = node.AddVariable ();
			EditorUtility.SetDirty(cv);
			dirty = true;
		}

        /*EditorGUILayout.Separator();
	       EditorGUILayout.BeginHorizontal();
	       GUILayout.Space( 12 );
	       GUILayout.Button( "Add Control Variable", GUILayout.MaxWidth(150) );
	       EditorGUILayout.EndHorizontal();*/

        EditorGUILayout.EndVertical ();

        //node.m_stateName = EditorGUILayout.TextField("State Name", node.m_stateName, GUILayout.ExpandWidth(true));

        base.OnInspectorGUI ();

        if (dirty)
        {
			AnimationTreeEditor.instance.RefreshVariablesWindow ();
			//AnimationTreeEditor.instance.m_controlVarsWindow
        }
	}
}
