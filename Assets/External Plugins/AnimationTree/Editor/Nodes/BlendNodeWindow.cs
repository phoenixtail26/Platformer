using UnityEngine;
using System.Collections;
using UnityEditor;

public class BlendNodeWindow : NodeWindow
{
	ConnectionButton m_Out;

    ConnectionButton m_controlVariable;

	public BlendNodeWindow( Rect position, AT_Node node ) : base( position, "Blend", node )
	{
		m_Out = new ConnectionButton( this, ConnectionButton.ConnectionButtonType.kAnimDataOut, "Out", ConnectionButton.Alignment.kLeft );
        m_controlVariable = new ConnectionButton( this, ConnectionButton.ConnectionButtonType.kControlDataIn, "Control", ConnectionButton.Alignment.kRight);

        AT_Blend blendNode = (AT_Blend)node;
        int childCount = blendNode.Children.Count;

        position.height = 80 + (30 * childCount);
        Position = position;

        ConnectionPoints = new ConnectionButton[childCount + 1];
        ConnectionPoints[0] = m_Out;

        for (int i = 0; i < childCount; i++)
        {
            ConnectionPoints[i + 1] = new ConnectionButton(this, ConnectionButton.ConnectionButtonType.kAnimDataIn, "In " + (i + 1), ConnectionButton.Alignment.kRight);
        }
	}

	public override void Draw()
	{
		base.Draw();

        // Draw blend position indicator
        GUIStyle style = GUI.skin.GetStyle("Rectangle");
        Color oldCol = GUI.color;

        float barHeight = 2*18 + 30 * (ConnectionPoints.Length-3);

        Rect indic = new Rect(ScreenPosition.x + 45, ScreenPosition.y + 33, 15, barHeight);
        GUI.color = new Color(0, 0, 0, 0.25f);
        GUI.Box(indic, "", style);
        indic.x += 1;
        indic.y += 1;
        indic.width -= 2;
        indic.height -= 2;
        GUI.color = new Color(0.5f, 0.5f, 0.5f, 0.25f);
        GUI.Box(indic, "", style);

        GUI.color = new Color(1.0f, 0, 0, 0.75f);
        AT_Blend blend = TreeNode as AT_Blend;
        indic.y = indic.y + (indic.height-3) * blend.m_controlVariableValue;
        indic.height = 3;
        indic.x += 1;
        indic.width -= 2;
        GUI.Box(indic, "", style);

        GUI.color = oldCol;

        Rect position = ScreenPosition;

        if (m_Out.Draw(position.x + position.width - 18, position.y + position.height / 2.0f))
        {
            AnimationTreeEditor.instance.DetachNode(TreeNode);
            AnimationTreeEditor.instance.MakeConnection(m_Out);
        }

        int counter = 1;
        for (int i = 1; i < ConnectionPoints.Length; i++ )
        {
            ConnectionButton button = ConnectionPoints[i];
            if (button.Draw(position.x + 5, position.y + 30 * counter))
            {
                int targetChild = FindChildIndexFromButton(button);
                AT_ContainerNode cont = TreeNode as AT_ContainerNode;

                if (cont.Children[targetChild] != null)
                {
                    AnimationTreeEditor.instance.DetachNode(cont.Children[targetChild]);
                }
                AnimationTreeEditor.instance.MakeConnection(button);
            }
            counter++;
        }

        if (m_controlVariable.Draw(position.x + 5, position.y + position.height - 30))
        {
            AT_Blend blendNode = (AT_Blend)TreeNode;
            blendNode.m_controlVariable = null;
            AnimationTreeEditor.instance.MakeConnection(m_controlVariable);
        }
	}

	public override void DrawConnections()
	{
		base.DrawConnections();

        // Data connection
        ControlVariablesWindow win = AnimationTreeEditor.instance.m_controlVarsWindow;
        AT_Blend blend = TreeNode as AT_Blend;

        if (blend.m_controlVariable != null)
        {
            ConnectionButton button = win.GetVariableButton(blend.m_controlVariable.m_variableName);
            if (button != null)
            {
                AnimationTreeEditor.instance.DrawConnectionLine(button.GUICorrectPosition, m_controlVariable.LastPosition, new Color(0.5f, 0.5f, 1, 1));
            }
        }

        /*AT_ContainerNode resNode = (AT_ContainerNode)TreeNode;
        if (resNode.Children.Count > 0)
        {
            for (int i = 0; i < resNode.Children.Count; i++)
            {
                NodeWindow nodeWin = AnimationTreeEditor.instance.GetCorrespondingWindow(resNode.Children[i]);
                if (nodeWin != null)
                {
                    ConnectionButton button = nodeWin.GetOutConnectionPoint();
                    if (button != null && ConnectionPoints[i+1] != null)
                    {
                        AnimationTreeEditor.instance.DrawConnectionLine(button.LastPosition, ConnectionPoints[i+1].LastPosition);
                    }
                }
            }
        }*/
	}

    int FindChildIndexFromButton(ConnectionButton button)
    {
        int targetChild = 0;
        for (int i = 1; i < ConnectionPoints.Length; i++)
        {
            if (ConnectionPoints[i] == button)
            {
                targetChild = i - 1;
            }
        }

        return targetChild;
    }

    public override void MakeConnection(ConnectionButton src, ConnectionButton dest)
    {
        if (src.m_type == ConnectionButton.ConnectionButtonType.kAnimDataOut)
        {
            int targetChild = FindChildIndexFromButton(dest);

            AT_ContainerNode cont = TreeNode as AT_ContainerNode;

            AT_Node srcNode = src.ParentWindow.GetTreeNode();
            cont.InsertChild(srcNode, targetChild);

            cont.SetDirty();
            srcNode.SetDirty(); 
        }
        else
        {
            if (dest == m_controlVariable)
            {
                //ControlVariablesWindow win = src.ParentWindow as ControlVariablesWindow;
                AT_Blend blend = TreeNode as AT_Blend;
                blend.m_controlVariable = blend.ParentTree.m_controlVarsNode.GetVariable(src.Label);
                //blend.m_var = blend.ParentTree.ControlVariables[src.Label];
            }
        }
    }
}

