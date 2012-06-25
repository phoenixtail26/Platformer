using UnityEngine;
using System.Collections;
using UnityEditor;

public class ResultNodeWindow : NodeWindow
{
	ConnectionButton m_In;

	public ResultNodeWindow( Rect position, AT_Node node ) : base( position, "Output", node )
	{
		m_In = new ConnectionButton( this, ConnectionButton.ConnectionButtonType.kAnimDataIn, "In", ConnectionButton.Alignment.kRight );
		ConnectionPoints = new ConnectionButton[2];
		ConnectionPoints[1] = m_In;
	}

	public override void Draw()
	{
		base.Draw();

        if (m_In.Draw(ScreenPosition.x + 6, ScreenPosition.y + ScreenPosition.height / 2.0f))
		{
            AT_ContainerNode cont = TreeNode as AT_ContainerNode;

            if (cont.Children[0] != null)
            {
                AnimationTreeEditor.instance.DetachNode(cont.Children[0]);
            }

            AnimationTreeEditor.instance.MakeConnection(m_In);
		}
	}

    public override void MakeConnection(ConnectionButton src, ConnectionButton dest)
    {
        AT_ContainerNode cont = TreeNode as AT_ContainerNode;
        AT_Node srcNode = src.ParentWindow.GetTreeNode();
        cont.InsertChild(srcNode, 0);

        cont.SetDirty();
        srcNode.SetDirty();
    }

    public override void Destruct()
    {
    }

	/*public override void DrawConnections()
	{
		AT_OutputNode resNode = ( AT_OutputNode )TreeNode;
		if ( resNode.Children.Count > 0 )
		{
			NodeWindow nodeWin = AnimationTreeEditor.instance.GetCorrespondingWindow( resNode.Children[0] );
			if ( nodeWin != null )
			{
				ConnectionButton button = nodeWin.GetOutConnectionPoint();
				if ( button != null && m_In != null )
				{
					AnimationTreeEditor.instance.DrawConnectionLine( button.LastPosition, m_In.LastPosition );
				}
			}
		}
	}*/
}
