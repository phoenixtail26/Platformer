using UnityEngine;
using System.Collections;
using UnityEditor;

[ExecuteInEditMode()]
public class NodeWindow
{
	public Rect Position
    {
        get 
		{
			if ( TreeNode != null )
			{
				return TreeNode.EditorProperties.Position; 
			}
			else
			{
				return new Rect(0,0,100,100);
			}
		}
        set 
		{ 
			if ( TreeNode != null )
			{
				TreeNode.EditorProperties.Position = value; 
				TreeNode.SetDirty();
			}
		} 
    }
	
	public Rect ScreenPosition
	{
		get 
		{
			Rect pos = TreeNode.EditorProperties.Position; 
			pos.x += AnimationTreeEditor.instance.m_cameraOffset.x;
			pos.y += AnimationTreeEditor.instance.m_cameraOffset.y;
			return pos;
		}
	}

	public string Title { get; protected set; }

	public bool Active { get; set; }

	protected AT_Node TreeNode;

	protected ConnectionButton[] ConnectionPoints;
    
    public static GUISkin m_skin = null;

	public NodeWindow()
	{
	}

	public NodeWindow( Rect position, string title )
	{
		Position = position;
		Title = title;
	}
	
	public NodeWindow( Rect position, string title, AT_Node node)
	{
		Title = title;
        TreeNode = node;
		
		//Debug.Log(Position);
		
		/*Rect pos = Position;
        pos.width = position.width;
		pos.height = position.height;
        Position = pos;*/		
		Position = position;
	}

	public ConnectionButton GetOutConnectionPoint()
	{
		if ( ConnectionPoints != null && ConnectionPoints.Length > 0 )
		{
			return ConnectionPoints[0];
		}

		return null;
	}

	public virtual AT_Node GetTreeNode()
	{
		return TreeNode;
	}

	public ConnectionButton GetConnectionPoint( int i )
	{
		return ConnectionPoints[i];
	}

	public virtual bool IsInWindow( Vector2 position )
	{
		return ScreenPosition.Contains( position );
	}

    public Vector2 LocalToGlobalCoord(Vector2 coord)
    {
        return new Vector2(coord.x + ScreenPosition.x, coord.y + ScreenPosition.y);
    }

    public Vector2 GlobalToLocalCoord(Vector2 coord)
    {
        return new Vector2(coord.x - ScreenPosition.x, coord.y - ScreenPosition.y);
    }
	
	public virtual void Draw()
	{
        if (m_skin == null)
        {
            m_skin = (GUISkin)Resources.Load("NodeWindowSkin", typeof(GUISkin));
        }

        GUI.skin = m_skin;
		Color oldColor = GUI.color;
        if (Active)
        {
            GUI.color = new Color(0.75f, 0.75f, 1, 0.8f);
        }
        else
        {
            GUI.color = new Color(1, 1, 1, 0.9f);
        }

        AT_ContainerNode cont = this.TreeNode as AT_ContainerNode;
        if (cont != null && cont.Expandable)
        {
            GUI.contentColor = Color.yellow;
        }

        GUI.Box(ScreenPosition, Title);

        GUI.contentColor = Color.white;

		GUI.color = oldColor;
	}

	public virtual void DrawConnections()
	{
        Color color = new Color(0.5f, 1, 0.5f, 1);

        AT_ContainerNode resNode = TreeNode as AT_ContainerNode;
        if (resNode != null)
        {
            if (resNode.Children.Count > 0)
            {
                for (int i = 0; i < resNode.Children.Count; i++)
                {
                    NodeWindow nodeWin = AnimationTreeEditor.instance.GetCorrespondingWindow(resNode.Children[i]);
                    if (nodeWin != null)
                    {
                        ConnectionButton button = nodeWin.GetOutConnectionPoint();
                        if (button != null && ConnectionPoints[i + 1] != null)
                        {
                            AnimationTreeEditor.instance.DrawConnectionLine(button.LastPosition, ConnectionPoints[i + 1].LastPosition, color);
                        }
                    }
                }
            }
        }
	}
	
	public void DeltaMove( Vector2 delta )
	{
		var newPos = Position;
		newPos.x += delta.x;
		newPos.y += delta.y;
		Position = newPos;

		if ( TreeNode != null )
		{
			Rect pos = Position;
			pos.x = newPos.x;
			pos.y = newPos.y;
			Position = pos;
		}
		else
		{
			Debug.Log("no node");
		}

       TreeNode.SetDirty();
	}


    public virtual void MakeConnection(ConnectionButton src, ConnectionButton dest)
    {
    }

    public virtual void Destruct()
    {
        AT_ContainerNode cont = TreeNode as AT_ContainerNode;
        if (cont != null)
        {
            if (!cont.Expandable)
            {
                foreach (AT_Node node in cont.Children)
                {
                    AnimationTreeEditor.instance.DetachNode(node);
                }
            }
        }
        AnimationTreeEditor.instance.DetachNode(TreeNode);
        TreeNode.Parent.RemoveChild(TreeNode);

        GameObject.DestroyImmediate(TreeNode.gameObject);
    }

    public void SetNewChildSize(int size)
    {
        AT_ContainerNode cont = TreeNode as AT_ContainerNode;
        if (cont == null)
        {
            return;
        }

        if (size < cont.Children.Count)
        {
            for (int i = 0; i < cont.Children.Count; i++)
            {
                if (i >= size)
                {
                    if (cont.Children[i] != null)
                    {
                        AnimationTreeEditor.instance.DetachNode(cont.Children[i]);
                    }
                }
            }

            cont.Children_Writable.RemoveRange(size, cont.Children.Count - size);
        }
        else
        {
            for (int i = cont.Children.Count; i < size; i++)
            {
                cont.Children_Writable.Add(null);
            }
        }

       
    }
}
