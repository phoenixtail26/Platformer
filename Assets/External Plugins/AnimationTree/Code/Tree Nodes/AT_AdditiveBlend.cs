////////////////////////////////////////////////////////////////////////////////////////
// 
// File:        AT_AdditiveBlend.cs
// Author:      Gavin Hayler
// Date:        23/06/2011
//
////////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

////////////////////////////////////////////////////////////////////////////////////////
// 
// Class:       AT_AdditiveBlend
// Description: A simple blend node which sets all of its children's weights to 1
//
////////////////////////////////////////////////////////////////////////////////////////
public class AT_AdditiveBlend : AT_ContainerNode
{
    public AT_AdditiveBlend()
	{
		m_expandable = false;
        NodeType = AT_NodeType.kAdditiveBlend;
        m_children.Add(null);
        m_children.Add(null);
	}
   
	public override void UpdateAnimation( float timeDelta )
	{
		base.UpdateAnimation(timeDelta);
		
        // Loop through all children and set their blend weights to 1
		//int numChildren = Children.Count;
		foreach ( AT_Node node in Children )
		{
			if ( node != null )
			{
				node.SetLocalBlendWeight(1);
			}
		}
	}
}
