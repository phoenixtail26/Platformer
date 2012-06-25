using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(AnimationTree))]
public class AnimationTreeRootEditor : CustomNodeEditor
{
    public override void OnInspectorGUI ()
    {
    	EditorGUILayout.Separator ();

        AnimationTree tree = (target as AnimationTree);

        EditorGUIUtility.LookLikeInspector ();

        EditorGUI.indentLevel = 1;
  
		Animation newAnim = EditorGUILayout.ObjectField ("Subject", tree.m_subject, typeof(Animation), true, GUILayout.ExpandWidth (true)) as Animation;
    	if (tree.Subject != newAnim)
		{
    		tree.Subject = newAnim;
		}
		
		Renderer newRenderer = EditorGUILayout.ObjectField ("Renderer", tree._renderer, typeof(Renderer), true, GUILayout.ExpandWidth (true)) as Renderer;
    	if (tree._renderer != newRenderer)
		{
    		tree._renderer = newRenderer;
		}
		
		Transform newRootBone = EditorGUILayout.ObjectField ("Root Bone", tree._rootBone, typeof(Transform), true, GUILayout.ExpandWidth (true)) as Transform;
    	if (tree._rootBone != newRootBone)
		{
    		tree._rootBone = newRootBone;
		}

        EditorGUIUtility.LookLikeControls();

        /*m_showControlVariables = EditorGUILayout.Foldout( m_showControlVariables, "Control Variables" );
        if ( m_showControlVariables )
        {
            EditorGUILayout.BeginVertical();
            

            foreach ( KeyValuePair<string, ControlVariable> kvp in tree.ControlVariables )
            {
                ControlVariable cv = kvp.Value;

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space( 12 );
                cv.m_value = EditorGUILayout.Slider( cv.m_variableName, cv.m_value, 0, 1, GUILayout.ExpandWidth( true ) );
                EditorGUILayout.EndHorizontal();
            }

            /*EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space( 12 );
            GUILayout.Button( "Add Control Variable", GUILayout.MaxWidth(150) );
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }*/

        base.OnInspectorGUI();
    }
}
