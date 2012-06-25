using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(AT_Blend))]
public class BlendNodeEditor : CustomNodeEditor
{
	public override void OnInspectorGUI()
	{
        EditorGUILayout.Separator();

        EditorGUIUtility.LookLikeControls();

        EditorGUI.indentLevel = 1;

        AT_Blend node = (target as AT_Blend);
		
        int[] options = {1,2,3,4,5,6,7,8,9};
        string[] optionsStr = { "1", "2", "3", "4", "5", "6", "7", "8", "9" };
        int newNum = EditorGUILayout.IntPopup("Inputs", node.Children.Count, optionsStr, options, GUILayout.ExpandWidth(true));

        if (newNum != node.Children.Count)
        {
            AnimationTreeEditor.instance.GetCorrespondingWindow(node).SetNewChildSize(newNum);
            AnimationTreeEditor.instance.RebuildTreeGraph();
        }

        /*Animation anim = node.m_subject;

		int count = anim.GetClipCount();
		string[] options = new string[count+2];
		options[0] = "None";
		options[1] = "";

		int counter = 2;
		int selectedOption = 0;
		foreach ( AnimationState state in anim )
		{
			options[counter] = state.name;
			if ( state.name == node.m_animationName )
			{
				selectedOption = counter;
			}
			counter++;
		}

		int newSelection = EditorGUILayout.Popup( "Animation", selectedOption, options, GUILayout.ExpandWidth( true ) );

        WrapMode mode = (WrapMode)EditorGUILayout.EnumPopup("Wrap Mode", node.m_wrapMode, GUILayout.ExpandWidth(true));
        bool init = false;
        if (mode != node.m_wrapMode)
        {
            init = true;
        }

        node.m_wrapMode = mode;

		if ( newSelection != selectedOption )
		{
			if ( newSelection >= 2 )
			{
				node.SetAnimation( options[newSelection] );
			}
			else
			{
				node.SetAnimation( "" );
			}
            init = true;
		}

        if (init)
        {
            node.Init();
            AnimationTreeEditor.instance.RebuildTreeGraph();
        }*/

        base.OnInspectorGUI();
	}
}
