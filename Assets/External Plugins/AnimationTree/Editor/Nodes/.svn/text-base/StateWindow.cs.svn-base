using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class StateWindow : NodeWindow
{
    Rect m_adjustedWindowPos = new Rect();
	
	public StateWindow(Rect position, string title, AT_Node node ) : base( position, title, node )
	{
	}
	
    public StateWindow(Rect position, AT_Node node) : base( position, "State", node )
    {
    }

    public StateWindow()
    {
    }

    public override bool IsInWindow(Vector2 position)
    {
        return m_adjustedWindowPos.Contains(position);
    }

    public bool IsInDraggingArea(Vector2 position)
    {
        return ScreenPosition.Contains(position);
    }

    public override void Destruct ()
    {
    	AT_StateMachine sm = TreeNode.Parent as AT_StateMachine;
    	AT_State state = TreeNode as AT_State;
    	
        List<AT_StateTransition> transitionsToRemove = new List<AT_StateTransition> ();
    	foreach (AT_StateTransition trans in sm.m_transitions)
        {
    		if (trans != null)
			{
    			if (trans.m_toState == state || trans.m_fromState == state)
            	{
    				transitionsToRemove.Add (trans);
    			}
			}
        }

        foreach (AT_StateTransition trans in transitionsToRemove)
        {
            AnimationTreeEditor.instance.GetCorrespondingWindow(trans).Destruct();
            sm.RemoveTransition(trans);
        }

        sm.SetToFirstState();

        base.Destruct();
    }

    public override void Draw()
    {
        Rect position = ScreenPosition;

        GUI.skin = m_skin;

        Color oldColor = GUI.color;

        GUI.color = new Color(0, 0, 0, 0.5f);
        
        AT_State state = TreeNode as AT_State;

        if (state.Current)
        {
            GUI.color = new Color(0.5f, 0, 0, 0.5f);
        }
        m_adjustedWindowPos = new Rect(position.x - 10, position.y - 10, position.width + 20, position.height + 20);
        GUI.Box( m_adjustedWindowPos, "", GUI.skin.customStyles[0]);


        GUI.color = oldColor;

        base.Draw();

        GUIStyle style = GUI.skin.GetStyle("Star");

        AT_StateMachine machine = TreeNode.Parent as AT_StateMachine;
        if (machine != null && machine.m_startUpState == this.TreeNode)
        {
            GUI.color = new Color(1.0f, 0.6f, 0.3f);
        }

        if (GUI.Button(new Rect(position.x + position.width - 25, position.y + position.height * 0.5f - 5, 20, 20), "", style))
        {
            if (machine != null)
            {
                machine.m_startUpState = this.TreeNode as AT_State;
            }
        }

        GUI.color = oldColor;

        //GUI.Label(new Rect(Position.x, Position.y, Position.width - 50, Position.height), m_AnimationName);

      
       

    }

}
