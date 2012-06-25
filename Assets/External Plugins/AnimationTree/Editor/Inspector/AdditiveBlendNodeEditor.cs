using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(AT_AdditiveBlend))]
public class AdditiveBlendNodeEditor : CustomNodeEditor
{
	public override void OnInspectorGUI()
	{
        EditorGUILayout.Separator();

        EditorGUIUtility.LookLikeControls();

        EditorGUI.indentLevel = 1;

        AT_AdditiveBlend node = (target as AT_AdditiveBlend);
		
        int[] options = {1,2,3,4,5,6,7,8,9};
        string[] optionsStr = { "1", "2", "3", "4", "5", "6", "7", "8", "9" };
        int newNum = EditorGUILayout.IntPopup("Inputs", node.Children.Count, optionsStr, options, GUILayout.ExpandWidth(true));

        if (newNum != node.Children.Count)
        {
            AnimationTreeEditor.instance.GetCorrespondingWindow(node).SetNewChildSize(newNum);
            AnimationTreeEditor.instance.RebuildTreeGraph();
        }

        base.OnInspectorGUI();
	}
}
