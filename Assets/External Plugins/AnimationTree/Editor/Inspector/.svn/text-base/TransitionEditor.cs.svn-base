using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(AT_StateTransition))]
public class TransitionEditor : CustomNodeEditor
{
    bool m_conditionsFoldout = true;
    List<bool> m_conditionDetails = new List<bool>();
    int m_conditionCounter = 0;
    List<AT_TransitionCondition> m_conditionsToRemove = new List<AT_TransitionCondition>();
	List<AT_TransitionCondition> m_conditionsToMoveUp = new List<AT_TransitionCondition> ();
	List<AT_TransitionCondition> m_conditionsToMoveDown = new List<AT_TransitionCondition> ();
	
    AT_StateTransition m_transition = null;

	public override void OnInspectorGUI ()
	{
		EditorGUILayout.Separator ();

        EditorGUIUtility.LookLikeControls ();

        EditorGUI.indentLevel = 1;

        m_transition = target as AT_StateTransition;

        AT_StateMachine sm = m_transition.Parent as AT_StateMachine;
		List<AT_State> states = new List<AT_State> ();
		string[] options = { "" };

        // Compile list of states
		if (sm != null)
        {
			foreach (AT_Node node in sm.Children)
            {
				AT_State state = node as AT_State;
				if (state != null)
                {
					states.Add (state);
				}
			}
			foreach (AT_Node node in sm.UnassignedChildren)
            {
				AT_State state = node as AT_State;
				if (state != null)
                {
					states.Add (state);
				}
			}
		}

        // fill up the popup options
		options = new string[states.Count];
		int counter = 0;
		int fromSelectedOption = 0;
		int toSelectedOption = 0;
		foreach (AT_State state in states)
        {
			options[counter] = state.m_stateName;

            if (state == m_transition.m_fromState)
            {
				fromSelectedOption = counter;
			}
			if (state == m_transition.m_toState)
            {
				toSelectedOption = counter;
			}

            counter++;
		}

        // Draw the state popups
		int newFromSelection = EditorGUILayout.Popup ("From State", fromSelectedOption, options, GUILayout.ExpandWidth (true));
		if (states.Count - 1 >= newFromSelection)
        {
			m_transition.m_fromState = states[newFromSelection];
			AnimationTreeEditor.instance.RebuildTreeGraph ();
		}

        int newToSelection = EditorGUILayout.Popup ("To State", toSelectedOption, options, GUILayout.ExpandWidth (true));
		if (states.Count - 1 >= newToSelection)
        {
			m_transition.m_toState = states[newToSelection];
			AnimationTreeEditor.instance.RebuildTreeGraph ();
		}
		
        EditorGUILayout.Separator ();

        // Draw the transition time
		m_transition.m_transitionTime = Mathf.Max (0, EditorGUILayout.FloatField ("Transition Time", m_transition.m_transitionTime, GUILayout.ExpandWidth (true)));

        EditorGUILayout.Separator ();

        bool reinit = false;
		
		if ( m_transition.m_conditions != null )
		{
			if (m_conditionDetails.Count != m_transition.m_conditions.Count)
	        {
				reinit = true;
			}

	        // Draw conditions
			m_conditionsToRemove.Clear ();
			m_conditionsToMoveUp.Clear ();
			m_conditionsToMoveDown.Clear ();
			
			m_conditionCounter = 0;
			m_conditionsFoldout = EditorGUILayout.Foldout (m_conditionsFoldout, "Conditions");
			AT_TransitionCondition[] tempList = m_transition.m_conditions.ToArray ();
			if (m_conditionsFoldout)
	        {
				foreach (AT_TransitionCondition cond in tempList)
	            {
					if (reinit)
	                {
						m_conditionDetails.Add (true);
					}
					DrawConditionGUI (cond);
					m_conditionCounter++;
				}
	
	            EditorGUILayout.Space ();
				EditorGUILayout.BeginHorizontal ();
				EditorGUI.indentLevel++;
				GUILayout.Space (EditorGUI.indentLevel * 12);
				if (GUILayout.Button ("Add Condition", GUILayout.MaxWidth (100)))
	            {
					AddNewCondition (m_transition);
				}
				EditorGUI.indentLevel--;
				EditorGUILayout.EndHorizontal ();
			}
	
	        // Remove deleted conditions
			foreach (AT_TransitionCondition cond in m_conditionsToRemove)
	        {
				m_transition.RemoveCondition (cond);
				GameObject.DestroyImmediate (cond.gameObject);
			}
			
			foreach (AT_TransitionCondition cond in m_conditionsToMoveUp) 
			{
				m_transition.MoveConditionUp (cond);
			}
			
			foreach (AT_TransitionCondition cond in m_conditionsToMoveDown) 
			{
				m_transition.MoveConditionDown (cond);
			}
		}
		
		EditorGUILayout.Space ();
		
		if ( m_transition != null )
		{			
			EditorGUI.indentLevel++;
			m_transition.fireRequestOnTransitionStart = EditorGUILayout.BeginToggleGroup("Fire Request On Transition Start", m_transition.fireRequestOnTransitionStart);
			if ( m_transition.fireRequestOnTransitionStart )
			{
				EditorGUI.indentLevel++;
				m_transition.transitionStartRequest = EditorGUILayout.TextField("Start Request", m_transition.transitionStartRequest, GUILayout.ExpandWidth(true));
				EditorGUI.indentLevel--;
			}
			EditorGUILayout.EndToggleGroup();
			
			m_transition.fireRequestOnTransitionEnd = EditorGUILayout.BeginToggleGroup("Fire Request On Transition End", m_transition.fireRequestOnTransitionEnd);
			if ( m_transition.fireRequestOnTransitionEnd)
			{
				EditorGUI.indentLevel++;
				m_transition.transitionEndRequest = EditorGUILayout.TextField("End Request", m_transition.transitionEndRequest, GUILayout.ExpandWidth(true));
				EditorGUI.indentLevel--;
			}
			EditorGUILayout.EndToggleGroup();
			
			EditorGUI.indentLevel--;
		}

        base.OnInspectorGUI();
	}

    void AddNewCondition (AT_StateTransition trans)
    {
    	AT_TransitionCondition cond = CreateConditionOfType (AT_TransitionCondition.ConditionType.kRequest, trans);//ScriptableObject.CreateInstance<AT_RequestCondition>();
        trans.AddCondition(cond);
    }

    string GetTitleForCondition (AT_TransitionCondition cond)
    {
    	if (cond != null)
		{
    		switch (cond.m_conditionType)
			{
	    		case AT_TransitionCondition.ConditionType.kRequest:
	    			return "Request Condition";
	
	            case AT_TransitionCondition.ConditionType.kProbability:
	    			return "Probability Condition";
	
	            case AT_TransitionCondition.ConditionType.kTimed:
	    			return "Timed Condition";
	
	            case AT_TransitionCondition.ConditionType.kMath:
	    			return "Math Condition";
	
	            case AT_TransitionCondition.ConditionType.kAnimationLoop:
	    			return "Animation Loop Condition";
    		}
		}

        return "Unknown Condition";
    }

    void DrawConditionGUI (AT_TransitionCondition cond)
    {
    	if (cond == null)
		{
    		return;
    	}
  
        EditorGUILayout.BeginHorizontal ();
    	GUILayout.Space (EditorGUI.indentLevel * 12);
    	m_conditionDetails[m_conditionCounter] = EditorGUILayout.Foldout (m_conditionDetails[m_conditionCounter], GetTitleForCondition (cond));
    	
		// Move Condition Up
    	if (GUILayout.Button ("^", GUILayout.MaxWidth (20), GUILayout.MaxHeight (15)))
		{
    		m_conditionsToMoveUp.Add (cond);
    	}
    	
		// Move Condition Down
    	if (GUILayout.Button ("v", GUILayout.MaxWidth (20), GUILayout.MaxHeight (15))) 
		{
    		m_conditionsToMoveDown.Add (cond);
    	}
  
		// Delete button
    	if (GUILayout.Button ("-", GUILayout.MaxWidth (20), GUILayout.MaxHeight (15)))
        {
    		m_conditionsToRemove.Add (cond);
    	}
    	EditorGUILayout.EndHorizontal ();

        if (m_conditionDetails[m_conditionCounter])
        {
    		EditorGUILayout.Space ();

            EditorGUI.indentLevel++;

            EditorGUILayout.BeginHorizontal ();

            // Indent space
    		EditorGUILayout.BeginVertical (GUILayout.MaxWidth (EditorGUI.indentLevel * 8));
    		EditorGUILayout.Space ();
    		EditorGUILayout.EndVertical ();

            EditorGUILayout.BeginVertical ();

            AT_TransitionCondition.ConditionType type = (AT_TransitionCondition.ConditionType)EditorGUILayout.EnumPopup ("Type", cond.m_conditionType, GUILayout.ExpandWidth (true));
    		if (type != cond.m_conditionType)
            {
    			// Change condition type
    			ChangeConditionType (cond, type);
    		}

            EditorGUILayout.Separator ();
    		
            switch (cond.m_conditionType)
            {
    		case AT_TransitionCondition.ConditionType.kRequest:
    			DrawRequestCondition (cond as AT_RequestCondition);
    			break;

                case AT_TransitionCondition.ConditionType.kProbability:
    			DrawProbabilityCondition (cond as AT_ProbabilityCondition);
    			break;

                case AT_TransitionCondition.ConditionType.kTimed:
    			DrawTimedCondition (cond as AT_TimedCondition);
    			break;

                case AT_TransitionCondition.ConditionType.kMath:
    			DrawMathCondition (cond as AT_MathCondition);
    			break;

                case AT_TransitionCondition.ConditionType.kAnimationLoop:
    			DrawAnimationLoopCondition (cond as AT_AnimLoopCondition);
    			break;

                default:
    			EditorGUILayout.PrefixLabel ("Unknown");
    			break;
    		}
   
    		//EditorGUILayout.EndVertical ();
   
			cond.m_resetWhenFromStateNotCurrent = GUILayout.Toggle (cond.m_resetWhenFromStateNotCurrent, "Reset when 'from' state is not current", GUILayout.ExpandWidth (true));
			//EditorGUILayout.BeginVertical (GUILayout.MaxWidth (EditorGUI.indentLevel * 8));
			GUILayout.Toggle (cond.IsConditionMet (), "Met", GUILayout.ExpandWidth (true));
			EditorGUILayout.EndVertical ();
			

            EditorGUILayout.EndHorizontal ();
   
			EditorGUILayout.Separator ();

            EditorGUI.indentLevel--;
        }
    }

    AT_TransitionCondition CreateConditionOfType (AT_TransitionCondition.ConditionType newType, AT_StateTransition trans)
    {
    	AT_TransitionCondition cond = null;
  
		GameObject go = new GameObject ("Condition");
    	go.transform.parent = trans.transform;
  
        switch (newType)
        {
    		case AT_TransitionCondition.ConditionType.kProbability:
	    		cond = go.AddComponent<AT_ProbabilityCondition> ();
	    		break;
   
	    	case AT_TransitionCondition.ConditionType.kRequest:
	    		cond = go.AddComponent<AT_RequestCondition> ();
	    		break;
   
            case AT_TransitionCondition.ConditionType.kTimed:
	    		cond = go.AddComponent<AT_TimedCondition> ();
	    		break;

            case AT_TransitionCondition.ConditionType.kMath:
	    		cond = go.AddComponent<AT_MathCondition> ();
	    		break;

            case AT_TransitionCondition.ConditionType.kAnimationLoop:
    			cond = go.AddComponent<AT_AnimLoopCondition> ();
                break;
        }

        return cond;
    }

    void ChangeConditionType (AT_TransitionCondition cond, AT_TransitionCondition.ConditionType newType)
    {
    	AT_TransitionCondition newCond = CreateConditionOfType (newType, m_transition);

        m_transition.ReplaceCondition (cond, newCond);

    	GameObject.DestroyImmediate (cond.gameObject);
    }

    void DrawAnimationLoopCondition(AT_AnimLoopCondition cond)
    {
        if (cond == null)
        {
            return;
        }
		
		Animation anim = m_transition.ParentTree.Subject;
		int count = 0;
		if (anim != null)
        {
			count = anim.GetClipCount ();
		}
		string[] options = new string[count+2];

		options[0] = "None";
		options[1] = "";

		int counter = 2;
		int selectedOption = 0;
		if (anim != null)
        {
			foreach (AnimationState state in anim)
            {
				options[counter] = state.name;
				if ( state.name == cond.animationName )
				{
					selectedOption = counter;
				}
				counter++;
			}
			
			int newSelection = EditorGUILayout.Popup ("Animation", selectedOption, options, GUILayout.ExpandWidth (true));
			if ( newSelection != selectedOption )
			{
				if (newSelection >= 2)
				{
					cond.animationName = options[newSelection];
				}
				else
				{
					cond.animationName = "";
				}
				EditorUtility.SetDirty(cond);
				EditorUtility.SetDirty(cond.gameObject);
				EditorUtility.SetDirty(this);
			}
		}
		else
		{
			Debug.Log("can't find animation");
		}
    }

    void DrawMathCondition(AT_MathCondition cond)
    {
        if (cond == null)
        {
            return;
        }

        // List of control variables
        List<ControlVariable> vars = m_transition.ParentTree.ControlVariablesList;

        string[] options = new string[vars.Count];
        int counter = 0;
        int selected = 0;
        foreach( ControlVariable cv in vars )
        {
            options[counter] = cv.m_variableName;

            if (cv == cond.m_controlVariable)
            {
                selected = counter;
            }

            counter++;
        }

        selected = EditorGUILayout.Popup("Variable", selected, options, GUILayout.ExpandWidth(true));
        cond.m_controlVariable = vars[selected];

        cond.m_opeator = (AT_MathCondition.ConditionOperator)EditorGUILayout.EnumPopup("Operator", cond.m_opeator, GUILayout.ExpandWidth(true));

        cond.m_constant = EditorGUILayout.FloatField("Constant", cond.m_constant, GUILayout.ExpandWidth(true));

        //cond.m_targetTime = EditorGUILayout.FloatField("Time Length", cond.m_targetTime, GUILayout.ExpandWidth(true));
    }

    void DrawTimedCondition(AT_TimedCondition cond)
    {
        if (cond == null)
        {
            return;
        }

        cond.m_targetTime = EditorGUILayout.FloatField("Time Length", cond.m_targetTime, GUILayout.ExpandWidth(true));
    }

    void DrawProbabilityCondition(AT_ProbabilityCondition cond)
    {
        if (cond == null)
        {
            return;
        }

        cond.m_probability = EditorGUILayout.Slider("Probability",cond.m_probability, 0, 1);
    }

    void DrawRequestCondition(AT_RequestCondition cond)
    {
        if (cond == null)
        {
            return;
        }

        string req = "";
        if ( cond != null && cond.m_request != null)
        {
            req = cond.m_request;
        }
        cond.m_request = EditorGUILayout.TextField("Request", req, GUILayout.ExpandWidth(true));
    }
}
