using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(AT_AnimationState))]
public class AnimationStateEditor : CustomNodeEditor
{
	public override void OnInspectorGUI()
	{
        EditorGUILayout.Separator();

        EditorGUIUtility.LookLikeControls();

        EditorGUI.indentLevel = 1;

		AT_AnimationState node = ( target as AT_AnimationState );

        node.m_stateName = node.m_animation.m_animationName;

        EditorGUILayout.TextField("State Name", node.m_stateName, GUILayout.ExpandWidth(true));

        base.OnInspectorGUI();
	}
}
