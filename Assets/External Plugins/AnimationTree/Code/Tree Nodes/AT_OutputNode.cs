////////////////////////////////////////////////////////////////////////////////////////
// 
// File:        AT_OutputNode.cs
// Author:      Gavin Hayler
// Date:        23/06/2011
//
////////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

////////////////////////////////////////////////////////////////////////////////////////
// 
// Class:       AT_OutputNode
// Description: A node which is used to pass values from the parent container down to 
//              the contained tree. Only has a single child because there is only one
//              desired connection to the child tree
//
////////////////////////////////////////////////////////////////////////////////////////
public class AT_OutputNode : AT_ContainerNode
{

    public AT_OutputNode()
    {
        NodeType = AT_NodeType.kOutput;
        m_children.Add(null);
        Expandable = false;
    }
}