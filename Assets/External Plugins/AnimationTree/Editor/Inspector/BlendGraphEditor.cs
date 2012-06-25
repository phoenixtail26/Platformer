using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(AT_BlendGraph))]
public class BlendGraphEditor : ContainerNodeEditor
{
	public override void OnInspectorGUI()
	{
        EditorGUILayout.Separator();

        EditorGUIUtility.LookLikeControls();

        EditorGUI.indentLevel = 1;

        AT_State node = (target as AT_State);

        node.m_stateName = EditorGUILayout.TextField("State Name", node.m_stateName, GUILayout.ExpandWidth(true));

        base.OnInspectorGUI();
	}
}
