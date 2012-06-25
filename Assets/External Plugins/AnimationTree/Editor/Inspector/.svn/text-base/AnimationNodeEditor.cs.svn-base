using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(AT_Animation))]
public class AnimationNodeEditor : CustomNodeEditor
{
	public override void OnInspectorGUI ()
	{
		EditorGUILayout.Separator ();

        EditorGUIUtility.LookLikeControls ();

        EditorGUI.indentLevel = 1;

		AT_Animation node = (target as AT_Animation);
		Animation anim = node.m_subject;
		int count = 0;
		if (anim != null)
        {
			count = anim.GetClipCount ();
		}
		string[] options = new string[count + 2];
		options[0] = "None";
		options[1] = "";

		int counter = 2;
		int selectedOption = 0;
		if (anim != null)
        {
			foreach (AnimationState state in anim)
            {
				options[counter] = state.name;
				if (state.name == node.m_animationName)
                {
					selectedOption = counter;
				}
				counter++;
			}
		}

		int newSelection = EditorGUILayout.Popup ("Animation", selectedOption, options, GUILayout.ExpandWidth (true));

        WrapMode mode = (WrapMode)EditorGUILayout.EnumPopup ("Wrap Mode", node.m_wrapMode, GUILayout.ExpandWidth (true));
		bool init = false;
		if (mode != node.m_wrapMode)
        {
			init = true;
		}

        node.m_wrapMode = mode;

		if (newSelection != selectedOption)
		{
			if (newSelection >= 2)
			{
				node.SetAnimation (options[newSelection]);
			}
			else
			{
				node.SetAnimation ("");
			}
			init = true;
		}
				
		node.m_weightMultiplier = EditorGUILayout.FloatField("Weight Factor", node.m_weightMultiplier, GUILayout.ExpandWidth(true));
		node.m_weightMultiplier = Mathf.Clamp01(node.m_weightMultiplier);		
		
		node._randomTimeStart = EditorGUILayout.Toggle ("Random Start Time", node._randomTimeStart, GUILayout.ExpandWidth (true));
		
        node.m_syncAnimation = EditorGUILayout.BeginToggleGroup ("Sync Animation", node.m_syncAnimation);
		if (node.m_syncAnimation)
        {
			node.m_animTimeControlled = false;
		}
			EditorGUI.indentLevel++;
		
        	node.m_syncAnimationLayer = EditorGUILayout.IntField ("Sync Layer", node.m_syncAnimationLayer, GUILayout.ExpandWidth (true));
		
			EditorGUI.indentLevel--;

        EditorGUILayout.EndToggleGroup ();
		
		int layerOffset = EditorGUILayout.IntField( "Layer Offset", node.m_animationLayerOffset, GUILayout.ExpandWidth(true) );
		if ( layerOffset != node.m_animationLayerOffset )
		{
			node.m_animationLayerOffset = layerOffset;
			init = true;
		}
		
        bool additive = EditorGUILayout.Toggle ("Additive", node.m_blendMode == AnimationBlendMode.Additive, GUILayout.ExpandWidth (true));
		if (additive != (node.m_blendMode == AnimationBlendMode.Additive))
        {
			if (additive)
            {
				node.m_blendMode = AnimationBlendMode.Additive;
			}
            else
            {
				node.m_blendMode = AnimationBlendMode.Blend;
			}
			init = true;
		}


        EditorGUIUtility.LookLikeInspector ();
		
		Transform mixTrans = (Transform)EditorGUILayout.ObjectField ("Mix Transform", node.m_mixTransform, typeof(Transform), true, GUILayout.ExpandWidth (true));
		if (node.m_mixTransform != mixTrans)
        {
			node.m_mixTransform = mixTrans;
			init = true;
		}

        EditorGUILayout.Space ();
		
		EditorGUI.indentLevel--;

        bool controlled = EditorGUILayout.Toggle (new GUIContent ("Time Controlled", "Use a variable to control the animation's time"), node.m_animTimeControlled, GUILayout.ExpandWidth (true));
		if (controlled != node.m_animTimeControlled)
        {
			node.m_animTimeControlled = controlled;
			init = true;

            if (controlled)
            {
				node.m_syncAnimation = false;
			}
		}
		
		if ( controlled )
		{
			EditorGUI.indentLevel++;
			node.m_timePosition = EditorGUILayout.FloatField("Time", node.m_timePosition, GUILayout.ExpandWidth(true));
			node.m_timePosition = Mathf.Clamp01(node.m_timePosition);
			EditorGUI.indentLevel--;
		}
		
		bool speedControlled = EditorGUILayout.Toggle (new GUIContent ("Speed Controlled", "Use a variable to control the animation's speed"), node.animSpeedControlled, GUILayout.ExpandWidth (true));
		if (speedControlled != node.animSpeedControlled)
        {
			node.animSpeedControlled = speedControlled;
			init = true;

            if (speedControlled)
            {
				node.m_syncAnimation = false;
			}
		}
		
		if ( speedControlled )
		{
			EditorGUI.indentLevel++;
			node.speedFactor = EditorGUILayout.FloatField("Speed Factor", node.speedFactor, GUILayout.ExpandWidth(true));
			EditorGUI.indentLevel--;
		}
		
		
		node.m_sendAnimationLoopRequest = EditorGUILayout.Toggle (new GUIContent ("Send Loop Request", "Send an animation loop request up the tree at the end of the animation"), node.m_sendAnimationLoopRequest, GUILayout.ExpandWidth (true));
			
		if ( node.m_sendAnimationLoopRequest )
		{
			EditorGUI.indentLevel++;
			node.m_allowRequestToPropagate = EditorGUILayout.Toggle (new GUIContent ("Propagate Request", "Send the animation loop request to all the branches in the tree"), node.m_allowRequestToPropagate, GUILayout.ExpandWidth (true));
			EditorGUI.indentLevel--;
		}
		
        if (init)
        {
            node.Init();
            AnimationTreeEditor.instance.RebuildTreeGraph();
        }

        base.OnInspectorGUI();
	}
}
