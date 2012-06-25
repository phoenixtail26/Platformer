////////////////////////////////////////////////////////////////////////////////////////
// 
// File:        AT_Node.cs
// Author:      Gavin Hayler
// Date:        23/06/2011
//
////////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

////////////////////////////////////////////////////////////////////////////////////////
// 
// Class:       AT_Node_Editor_Properties
// Description: Class used to store any persistent properties required by the editor
//
////////////////////////////////////////////////////////////////////////////////////////
[System.Serializable]
public class AT_Node_Editor_Properties
{
	[SerializeField]
	private Rect _position = new Rect(50,50,200,60);
    public Rect Position
	{
		get { return _position; }
		set { _position = value; }
	}
}

////////////////////////////////////////////////////////////////////////////////////////
// 
// Enum:        AT_NodeType
// Description: Each node type has its own enum which is used by the editor to display
//              it.
//
////////////////////////////////////////////////////////////////////////////////////////
public enum AT_NodeType
{
    kRoot,
    kOutput,
    kBlend,
    kAnimation,
    kAnimationState,
    kBlendGraph,
    kCustom,
    kStateMachine,
    kTransition,
    kBlendContainer,
    kAdditiveBlend,
    kBlendStateMachine,
	kPose
}

////////////////////////////////////////////////////////////////////////////////////////
// 
// Class:       AT_Node
// Description: Base class for all Animation Tree nodes.
//
////////////////////////////////////////////////////////////////////////////////////////
public class AT_Node : MonoBehaviour 
{
    public AT_Node_Editor_Properties EditorProperties = new AT_Node_Editor_Properties();

    // The blend weight of the containing parent node
	private float m_parentBlendWeight = 1.0f;
	public float parentBlendWeight
	{
		get { return m_parentBlendWeight; }
		set
		{
			if ( value != m_parentBlendWeight )
			{
				m_parentBlendWeight = Mathf.Clamp01(value);
				RecalculateGlobalBlendWeight();
			}
		}
	}
	
    // The local blend weight of this node
	private float m_localBlendWeight = 1.0f;
	public float localBlendWeight
	{
		get { return m_localBlendWeight; }
		set
		{
			if ( value != m_localBlendWeight )
			{
				m_localBlendWeight = Mathf.Clamp01(value);
				RecalculateGlobalBlendWeight();
			}
		}
	}
	
    // The actual blend weight which is calculated from the parent and local weights. 
    // This value is used when setting animation blend weights.
	private float m_actualBlendWeight = 1.0f;
	public float ActualBlendWeight
	{
		get { return m_actualBlendWeight; }
	}

    // The type of tree node
    public AT_NodeType NodeType = AT_NodeType.kAnimation;

    // The animation tree that this node belongs to
	[SerializeField]
	protected AnimationTree m_ParentTree;

    // The containing parent node
    public AT_ContainerNode Parent;

	public AnimationTree ParentTree
	{
		set { m_ParentTree = value; }
        get { return m_ParentTree; }
	}
	
	private bool _isDirty = true;
	
	public bool IsDirty
	{
		get { return _isDirty; } 
		set 
		{ 
			_isDirty = value; 
			if ( value && m_ParentTree != null )
			{
				m_ParentTree.IsDirty = true;
			}
		}
	}
	
	public void SetDirty()
	{
		IsDirty = true;
	}
	
	
	// Use this for initialization
	public virtual void Start () 
	{	
		ParentTree.RegisterNode(this);
	}
	
	void OnDestroy()
	{
		ParentTree.DeregisterNode(this);
	}
	
#if UNITY_EDITOR
	// Update is called once per frame
/*	public void Update ()
	{
        // Make sure the local blend weight is in the value range
		localBlendWeight = Mathf.Clamp01( m_localBlendWeight );
	}*/
#endif
	
	public virtual void UpdateAnimation( float timeDelta ){}
	
	public virtual void SetParentBlendWeight( float weight )
	{
#if UNITY_EDITOR
		SetDirty();
#endif
		//m_parentBlendWeight = weight;
		parentBlendWeight = weight;
	}

	public virtual void SetLocalBlendWeight( float weight )
	{
#if UNITY_EDITOR
		SetDirty();
#endif
		localBlendWeight = weight;
	}

	protected void RecalculateGlobalBlendWeight()
	{
		m_actualBlendWeight = m_localBlendWeight * m_parentBlendWeight;
	}

    public void SetParent(AT_ContainerNode node)
    {
        // Ensure we're not parenting the node to itself
        if (node != this)
        {
#if UNITY_EDITOR
			SetDirty();
#endif
            // If the node has a parent
            if (Parent != null)
            {
                // Remove the node from the parent
                Parent.RemoveChild(this);
            }

            // Set the new parent in the tree
            Parent = node;
            // Set the gameObject's parent to maintain the Unity heirarchy
            gameObject.transform.parent = node.gameObject.transform;
        }
		else
		{
			Debug.LogWarning("AT_Node: Failed to parent node because node and parent are the same node");
		}
    }

   

    
}
