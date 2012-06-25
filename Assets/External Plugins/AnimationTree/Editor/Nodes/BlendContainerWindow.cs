using UnityEngine;
using System.Collections;

public class BlendContainerWindow : NodeWindow
{
    ConnectionButton m_Out;

    public BlendContainerWindow(Rect position, AT_Node node) : base( position, "Blend Graph", node )
    {
        m_Out = new ConnectionButton(this, ConnectionButton.ConnectionButtonType.kAnimDataOut,"Out", ConnectionButton.Alignment.kLeft);
        ConnectionPoints = new ConnectionButton[1];
        ConnectionPoints[0] = m_Out;
    }

    public override void Draw()
    {
        base.Draw();

        GUI.Label(new Rect(ScreenPosition.x, ScreenPosition.y, ScreenPosition.width - 50, ScreenPosition.height), (TreeNode as AT_BlendContainer).m_stateName);

        if (m_Out.Draw(ScreenPosition.x + ScreenPosition.width - 18, ScreenPosition.y + ScreenPosition.height / 2.0f))
        {
            AnimationTreeEditor.instance.DetachNode(TreeNode);
            TreeNode.SetLocalBlendWeight(1);
            AnimationTreeEditor.instance.MakeConnection( m_Out );
        }


    }

}