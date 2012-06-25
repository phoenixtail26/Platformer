using UnityEngine;
using System.Collections;

public class AnimationNodeWindow : NodeWindow
{
    protected string m_AnimationName = "";
    protected ConnectionButton m_Out;

    protected ConnectionButton m_controlVariable;
	protected ConnectionButton m_speedControlVariable;

    public AnimationNodeWindow(Rect position, string animation, AT_Node node) : base( position, "Animation", node )
    {
        m_AnimationName = animation;
        m_Out = new ConnectionButton(this, ConnectionButton.ConnectionButtonType.kAnimDataOut,"Out", ConnectionButton.Alignment.kLeft);
        m_controlVariable = new ConnectionButton(this, ConnectionButton.ConnectionButtonType.kControlDataIn, "Time Control", ConnectionButton.Alignment.kRight);
		m_speedControlVariable = new ConnectionButton(this, ConnectionButton.ConnectionButtonType.kControlDataIn, "Speed Control", ConnectionButton.Alignment.kRight);
		
        ConnectionPoints = new ConnectionButton[1];
        ConnectionPoints[0] = m_Out;

        if ((node as AT_Animation).m_animTimeControlled)
        {
            position.height = 100;
        }
		
		if ((node as AT_Animation).animSpeedControlled)
        {
            position.height = 100;
        }
		
        Position = position;
    }

    public override void Draw()
    {
        base.Draw();
        Rect position = TreeNode.EditorProperties.Position;
		position.x += AnimationTreeEditor.instance.m_cameraOffset.x;
		position.y += AnimationTreeEditor.instance.m_cameraOffset.y;
		

        GUI.Label(new Rect(position.x, position.y - position.height*0.5f + 30, position.width - 50, position.height), m_AnimationName);

        if (m_Out.Draw(position.x + position.width - 18, position.y + position.height / 2.0f))
        {
            /*TreeNode.SetParent( null );*/
            AnimationTreeEditor.instance.DetachNode(TreeNode);
            TreeNode.SetLocalBlendWeight(1);
            AnimationTreeEditor.instance.MakeConnection( m_Out );
        }

        AT_Animation animNode = (AT_Animation)TreeNode;
        if (animNode.m_animTimeControlled)
        {
            if (m_controlVariable.Draw(position.x + 5, position.y + position.height - 30))
            {

                animNode.m_animTimeControlVar = null;
                AnimationTreeEditor.instance.MakeConnection(m_controlVariable);
            }
        }
		
		if ( animNode.animSpeedControlled )
		{
			if (m_speedControlVariable.Draw(position.x + 5, position.y + position.height - 30))
            {
                animNode.m_animSpeedControlVar = null;
                AnimationTreeEditor.instance.MakeConnection(m_speedControlVariable);
            }
		}
    }

    public override void DrawConnections()
    {
        base.DrawConnections();

        // Data connection
        ControlVariablesWindow win = AnimationTreeEditor.instance.m_controlVarsWindow;
        AT_Animation anim = TreeNode as AT_Animation;

        if (anim.m_animTimeControlled && anim.m_animTimeControlVar != null)
        {
            ConnectionButton button = win.GetVariableButton(anim.m_animTimeControlVar.m_variableName);
            if (button != null)
            {
                AnimationTreeEditor.instance.DrawConnectionLine(button.GUICorrectPosition, m_controlVariable.LastPosition, new Color(0.5f, 0.5f, 1, 1));
            }
        }
		
		if (anim.m_animSpeedControlVar && anim.m_animSpeedControlVar != null)
        {
            ConnectionButton button = win.GetVariableButton(anim.m_animSpeedControlVar.m_variableName);
            if (button != null)
            {
                AnimationTreeEditor.instance.DrawConnectionLine(button.GUICorrectPosition, m_speedControlVariable.LastPosition, new Color(0.5f, 0.5f, 1, 1));
            }
        }
    }

    public override void MakeConnection(ConnectionButton src, ConnectionButton dest)
    {
        if (dest == m_controlVariable)
        {
            //ControlVariablesWindow win = src.ParentWindow as ControlVariablesWindow;
            AT_Animation anim = TreeNode as AT_Animation;
            anim.m_animTimeControlVar = anim.ParentTree.m_controlVarsNode.GetVariable(src.Label);
            //blend.m_var = blend.ParentTree.ControlVariables[src.Label];
        }
		else if ( dest == m_speedControlVariable )
		{
			AT_Animation anim = TreeNode as AT_Animation;
            anim.m_animSpeedControlVar = anim.ParentTree.m_controlVarsNode.GetVariable(src.Label);
		}
    }

}