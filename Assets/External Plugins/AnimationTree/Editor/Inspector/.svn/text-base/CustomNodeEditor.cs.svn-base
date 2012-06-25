using UnityEngine;
using UnityEditor;
using System.Collections;

public class CustomNodeEditor : Editor
{
    bool m_showDefault = false;

    public override void OnInspectorGUI()
    {
        EditorGUILayout.Separator();

        m_showDefault = EditorGUILayout.Foldout( m_showDefault, "Advanced" );
        if ( m_showDefault )
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space( 12 );
            EditorGUILayout.BeginVertical();
			DrawDefaultInspector();
			EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }

        if ( GUI.changed )
        {
            EditorUtility.SetDirty( target );
        }
    }
}