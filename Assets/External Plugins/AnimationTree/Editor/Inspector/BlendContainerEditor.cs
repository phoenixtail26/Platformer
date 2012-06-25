using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(AT_BlendContainer))]
public class BlendContainerEditor : ContainerNodeEditor
{
	public override void OnInspectorGUI()
	{
        EditorGUILayout.Separator();

        EditorGUIUtility.LookLikeControls();

        EditorGUI.indentLevel = 1;

        AT_BlendContainer node = (target as AT_BlendContainer);

        node.m_stateName = EditorGUILayout.TextField("Name", node.m_stateName, GUILayout.ExpandWidth(true));

        base.OnInspectorGUI();
	}
}
