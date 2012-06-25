using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(AT_OutputNode))]
public class OutputNodeEditor : ContainerNodeEditor
{
	public override void OnInspectorGUI()
	{
        EditorGUIUtility.LookLikeControls();

        EditorGUI.indentLevel = 1;

        base.OnInspectorGUI();
	}
}
