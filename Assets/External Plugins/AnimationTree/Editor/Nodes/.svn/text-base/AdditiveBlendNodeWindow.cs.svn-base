using UnityEngine;
using System.Collections;
using UnityEditor;

public class AdditiveBlendNodeWindow : NodeWindow
{
	ConnectionButton m_Out;

    public AdditiveBlendNodeWindow(Rect position, AT_Node node) : base( position, "Additive Blend", node )
	{
		m_Out = new ConnectionButton( this, ConnectionButton.ConnectionButtonType.kAnimDataOut, "Out", ConnectionButton.Alignment.kLeft );

        AT_AdditiveBlend blendNode = (AT_AdditiveBlend)node;
        int childCount = blendNode.Children.Count;

        position.height = 30 + (30 * childCount);
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

        if (m_Out.Draw(ScreenPosition.x + ScreenPosition.width - 18, ScreenPosition.y + ScreenPosition.height / 2.0f))
        {
            AnimationTreeEditor.instance.DetachNode(TreeNode);
            AnimationTreeEditor.instance.MakeConnection(m_Out);
        }

        int counter = 1;
        for (int i = 1; i < ConnectionPoints.Length; i++ )
        {
            ConnectionButton button = ConnectionPoints[i];
            if (button.Draw(ScreenPosition.x + 5, ScreenPosition.y + 30 * counter))
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
    }
}

