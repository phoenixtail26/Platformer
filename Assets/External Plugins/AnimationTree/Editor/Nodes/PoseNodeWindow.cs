using UnityEngine;
using System.Collections;

public class PoseNodeWindow : NodeWindow
{
	protected string m_AnimationName = "";
    protected ConnectionButton m_Out;
	
    public PoseNodeWindow(Rect position, string animation, AT_Node node) : base( position, "Pose", node )
    {
		m_AnimationName = animation;
        m_Out = new ConnectionButton(this, ConnectionButton.ConnectionButtonType.kAnimDataOut,"Out", ConnectionButton.Alignment.kLeft);
       // m_controlVariable = new ConnectionButton(this, ConnectionButton.ConnectionButtonType.kControlDataIn, "Time Control", ConnectionButton.Alignment.kRight);

        ConnectionPoints = new ConnectionButton[1];
        ConnectionPoints[0] = m_Out;

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
            //TreeNode.SetParent( null );
            AnimationTreeEditor.instance.DetachNode(TreeNode);
            TreeNode.SetLocalBlendWeight(1);
            AnimationTreeEditor.instance.MakeConnection( m_Out );
        }
    }

    public override void DrawConnections()
    {
        base.DrawConnections();

        // Data connection
        //ControlVariablesWindow win = AnimationTreeEditor.instance.m_controlVarsWindow;
        /*AT_Pose anim = TreeNode as AT_Pose;

        if (anim.m_animTimeControlled && anim.m_animTimeControlVar != null)
        {
            ConnectionButton button = win.GetVariableButton(anim.m_animTimeControlVar.m_variableName);
            if (button != null)
            {
                AnimationTreeEditor.instance.DrawConnectionLine(button.GUICorrectPosition, m_controlVariable.LastPosition, new Color(0.5f, 0.5f, 1, 1));
            }
        }*/
    }

    /*public override void MakeConnection(ConnectionButton src, ConnectionButton dest)
    {
        if (dest == m_controlVariable)
        {
            //ControlVariablesWindow win = src.ParentWindow as ControlVariablesWindow;
            AT_Animation anim = TreeNode as AT_Animation;
            anim.m_animTimeControlVar = anim.ParentTree.m_controlVarsNode.GetVariable(src.Label);
            //blend.m_var = blend.ParentTree.ControlVariables[src.Label];
        }
    }*/

}