////////////////////////////////////////////////////////////////////////////////////////
// 
// File:        AT_Factory.cs
// Author:      Gavin Hayler
// Date:        23/06/2011
//
////////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

////////////////////////////////////////////////////////////////////////////////////////
// 
// Class:       AT_Factory
// Description: Static class with functions useful for creating and maintaining
//              animation trees
//
////////////////////////////////////////////////////////////////////////////////////////
public static class AT_Factory
{
    // Creates a node of a certain type, assigns its parent tree and parent node
	public static T CreateNode<T>( string nodeName, AT_ContainerNode parent, AnimationTree tree ) where T : AT_Node
	{
		GameObject go = new GameObject();
		T node = go.AddComponent<T>();
		
		if ( nodeName != "" )
		{
			nodeName += " ";
		}

		node.ParentTree = tree;

        go.name = nodeName;// +"<" + typeof(T).ToString() + ">";

		if ( parent != null )
		{
			parent.AddChild( node, true );
		}

		return node;
	}
}
