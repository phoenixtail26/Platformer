using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(AT_StateMachine))]
public class StateMachineEditor : ContainerNodeEditor
{
	public override void OnInspectorGUI()
	{
        EditorGUILayout.Separator();

        EditorGUIUtility.LookLikeControls();

        EditorGUI.indentLevel = 1;

        AT_StateMachine sm = (target as AT_StateMachine);

        sm.m_stateName = EditorGUILayout.TextField("State Name", sm.m_stateName, GUILayout.ExpandWidth(true));


        
        List<AT_State> states = new List<AT_State>();
        string[] options = { "" };

        // Compile list of states
        if (sm != null)
        {
            foreach (AT_Node node in sm.Children)
            {
                AT_State state = node as AT_State;
                if (state != null)
                {
                    states.Add(state);
                }
            }
            foreach (AT_Node node in sm.UnassignedChildren)
            {
                AT_State state = node as AT_State;
                if (state != null)
                {
                    states.Add(state);
                }
            }
        }

        // fill up the popup options
        options = new string[states.Count];
        int counter = 0;
        int selectedOption = 0;
        foreach (AT_State state in states)
        {
            options[counter] = state.m_stateName;
            if (options[counter] == "")
            {
                options[counter] = " ";
            }

            if (state == sm.m_startUpState)
            {
                selectedOption = counter;
            }

            counter++;
        }

        // Draw the state popups
        int newSelection = EditorGUILayout.Popup("Start State", selectedOption, options, GUILayout.ExpandWidth(true));
        if (states.Count - 1 >= newSelection)
        {
            sm.m_startUpState = states[newSelection];
			sm.SetDirty();
        }


        base.OnInspectorGUI();
	}
}
