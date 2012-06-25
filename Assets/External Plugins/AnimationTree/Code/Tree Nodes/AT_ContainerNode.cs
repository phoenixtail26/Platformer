////////////////////////////////////////////////////////////////////////////////////////
// 
// File:        AT_ContainerNode.cs
// Author:      Gavin Hayler
// Date:        23/06/2011
//
////////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

////////////////////////////////////////////////////////////////////////////////////////
// 
// Class:       AT_ContainerNode
// Description: A tree node which is capable of containing other nodes
//
////////////////////////////////////////////////////////////////////////////////////////
public class AT_ContainerNode : AT_Node 
{
    // The children nodes that inherit this node's blend weight as their parent blend weight
	[SerializeField]
	protected List<AT_Node> m_children = new List<AT_Node>();
    // The nodes that are inactive, but still belong to this node
    [SerializeField]
    private List<AT_Node> m_unassignedChildren = new List<AT_Node>();

    // Can this container be opened to reveal a deeper level of the tree
	[SerializeField]
	protected bool m_expandable = true;

	public ReadOnlyCollection<AT_Node> Children
	{
		get { return m_children.AsReadOnly(); }
	}
	
	public List<AT_Node> Children_Writable
	{
		get 
		{ 
#if UNITY_EDITOR
			SetDirty();
#endif
			return m_children; 
		}
	}

    public ReadOnlyCollection<AT_Node> UnassignedChildren
    {
        get { return m_unassignedChildren.AsReadOnly(); }
    }
	
	public List<AT_Node> UnassignedChildren_Writable
	{
		get 
		{ 
#if UNITY_EDITOR
			SetDirty();
#endif
			return m_unassignedChildren; 
		}
	}

	public bool Expandable
	{
		get { return m_expandable; }
        set { m_expandable = value; }
	}
		
	void Awake()
	{
	}

	public override void UpdateAnimation( float timeDelta )
	{
        // Set each child's parent blend weight
		foreach ( AT_Node node in Children )
		{
            if (node != null)
            {
				//Profiler.BeginSample("set children blend weight");
                node.SetParentBlendWeight(ActualBlendWeight);
				//Profiler.EndSample();
            }
		}
		
#if UNITY_EDITOR
		//Profiler.BeginSample("set non-children blend weight");
		foreach ( AT_Node node in UnassignedChildren )
		{
			if ( node != null )
			{
				node.SetParentBlendWeight(0);
			}
		}
		//Profiler.EndSample();
#endif
			
	}

    // Insert a child node at a specific point in the child list
    public void InsertChild(AT_Node node, int index)
    {
        Children_Writable[index] = node;
        node.SetParent(this);
    }

    // Add a child to the container
	public virtual void AddChild( AT_Node node, bool unassigned )
	{
        if (node == null)
        {
			Debug.LogWarning("AT_ContainerNode: Tried to add null node as child");
            return;
        }
		
		 // Set this as the new child's parent
        node.SetParent(this);

        if (unassigned)
        {
            if (!UnassignedChildren_Writable.Contains(node))
            {
                UnassignedChildren_Writable.Add(node);
            }
			else
			{
				Debug.LogWarning("AT_ContainerNode: Failed to add unassigned child because node is already a unassigned child");
			}
        }
        else
        {
            if (!Children_Writable.Contains(node))
            {
                Children_Writable.Add(node);
            }
			else
			{
				Debug.LogWarning("AT_ContainerNode: Failed to add child because node is already a child");
			}
        }
       
	}

    // Remove the node from this container
    public virtual void RemoveChild(AT_Node node)
    {
        for (int i = 0; i < Children_Writable.Count; i++)
        {
            if (Children_Writable[i] == node)
            {
                Children_Writable[i] = null;
            }
        }

        UnassignedChildren_Writable.Remove(node);
    }

    public virtual void SendRequest(string request)
    {
        SendRequest(request, true);
    }

    // Send any request received to any child node that is also a container
	public virtual void SendRequest (string request, bool sendToChildren, bool sendUpwards = true )
	{
		if (sendToChildren)
		{
			foreach (AT_Node node in m_children)
			{
				if ( node != null )
				{
					AT_ContainerNode cNode = node as AT_ContainerNode;
	
					if (cNode != null)
					{
						cNode.SendRequest (request, sendToChildren, false);
					}
				}
			}
		}
		
		if ( sendUpwards )
		{
			if (Parent != null)
			{
				Parent.SendRequest (request, sendToChildren);
			}
			else
			{
				ParentTree.SendExternalRequest( request );
			}
		}
	}
	
}
