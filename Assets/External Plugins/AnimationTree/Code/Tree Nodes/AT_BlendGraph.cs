////////////////////////////////////////////////////////////////////////////////////////
// 
// File:        AT_BlendGraph.cs
// Author:      Gavin Hayler
// Date:        23/06/2011
//
////////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

////////////////////////////////////////////////////////////////////////////////////////
// 
// Class:       AT_BlendGraph
// Description: A container state which can be added to state-machines. Can contain 
//              animation blend trees
//
////////////////////////////////////////////////////////////////////////////////////////
public class AT_BlendGraph : AT_State 
{
    public AT_BlendGraph()
    {
        NodeType = AT_NodeType.kBlendGraph;
        m_stateName = "";
    }
}
