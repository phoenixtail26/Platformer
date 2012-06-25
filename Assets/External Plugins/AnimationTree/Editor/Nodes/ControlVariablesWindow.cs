using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class ControlVariablesWindow : NodeWindow
{
    ConnectionButton m_Out;
    ConnectionButton[] m_buttons;

    AT_ControlVariables m_varsNode;

    public ControlVariablesWindow (Rect position, AT_Node node) : base( position, "Control Variables", node )
    {
        m_varsNode = node as AT_ControlVariables;
    	if (m_varsNode != null)
        {
    		m_varsNode.Refresh ();

            m_buttons = new ConnectionButton[m_varsNode.Variables.Count];
    		ConnectionPoints = m_buttons;
    		int counter = 0;
    		foreach (ControlVariable cv in m_varsNode.Variables)
            {
    			if (cv != null)
				{
    				m_buttons[counter] = new ConnectionButton (this, ConnectionButton.ConnectionButtonType.kControlDataOut, cv.m_variableName, ConnectionButton.Alignment.kLeft);
    				counter++;
				}
            }
        }

        Rect pos = Position;
        float newHeight = 30 + 25 * m_varsNode.Variables.Count;
        pos.y = pos.y + pos.height - newHeight;
        pos.height = newHeight;
		pos.width = position.width;
        Position = pos;
    }

    public ConnectionButton GetVariableButton(string name)
    {
        foreach (ConnectionButton button in m_buttons)
        {
            if (button.Label == name)
            {
                return button;
            }
        }
        return null;
    }

    public override void Draw ()
    {
    	Rect winPos = AnimationTreeEditor.instance.m_editorWindow.position;
    	Rect pos = Position;
    	pos.x = -5;
    	pos.y = winPos.height - pos.height - 85;
    	Position = pos;

        Rect position = pos;

        base.Draw ();

        int counter = 1;
  
		if (m_varsNode != null)
		{
    		for (int i = 0; i < ConnectionPoints.Length; i++)
        	{
    			ConnectionButton button = ConnectionPoints[i];
    			if (button != null)
				{
    				ControlVariable var = m_varsNode.GetVariable (button.Label);

		            if (var != null)
		            {
	    					float val = GUI.HorizontalSlider (new Rect (position.x + 10, position.y + 25 * counter, 100, 30), var.Value, var.MinValue, var.MaxValue);
	    					m_varsNode.SetValue (var.m_variableName, val);
	    					EditorUtility.SetDirty (m_varsNode);
	 
		                if (button.Draw (position.x + position.width - 18, position.y + 25 * counter))
		                {
    						AnimationTreeEditor.instance.MakeConnection (button);
    					}
    				}
	 
		            counter++;
				}
    		}
		}


    }

}