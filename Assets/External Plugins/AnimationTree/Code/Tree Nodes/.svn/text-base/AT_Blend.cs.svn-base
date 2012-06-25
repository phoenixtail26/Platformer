////////////////////////////////////////////////////////////////////////////////////////
// 
// File:        AT_Blend.cs
// Author:      Gavin Hayler
// Date:        23/06/2011
//
////////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

////////////////////////////////////////////////////////////////////////////////////////
// 
// Class:       AT_Blend
// Description: A tree node used to blend animations together using a user controlled
//              variable.
//
////////////////////////////////////////////////////////////////////////////////////////
public class AT_Blend : AT_ContainerNode
{
    // The normalized value used to control the blend weights
	public float m_controlVariableValue = 0;

    // The control variable that is controlling the blend
    public ControlVariable m_controlVariable = null;

	public AT_Blend()
	{
		m_expandable = false;
        NodeType = AT_NodeType.kBlend;
        m_children.Add(null);
        m_children.Add(null);
	}

	public override void UpdateAnimation( float timeDelta )
	{
		base.UpdateAnimation(timeDelta);
		
		if ( m_controlVariable != null )
		{
            // Get the normalized value out of the control variable every frame
            m_controlVariableValue = m_controlVariable.NormalizedValue;
		}
		m_controlVariableValue = Mathf.Clamp( m_controlVariableValue, 0, 1 );
		
        int numChildren = Children.Count;

        // step is the value change required to go from blending only one input, to blending only the next one.
		float step = 1;
		if ( numChildren > 1 )
		{
			step = 1.0f / (numChildren-1);
		}
		
        // There will only be 2 inputs active at any one time.
        // blendValue is how much of the 2nd input is blended in
		float blendValue = (m_controlVariableValue % step) / step;
        // The index of the 1st active input
		int indexValue = (int)(m_controlVariableValue / step);

        // Loop through all the inputs
		//for ( int i = 0; i < numChildren; i++ )
		int i = 0;
		foreach ( AT_Node node in Children )
		{
            // If the input is not active, it's blend weight is 0
			float childLocalValue = 0;
			
            // 1st active input
			if ( i == indexValue )
			{
				childLocalValue = 1 - blendValue;
			}
            // 2nd active input
			if ( i == indexValue + 1 )
			{
				childLocalValue = blendValue;
			}

            // Assign the calculated blend weight to the input
            if (node != null)
            {
                node.SetLocalBlendWeight(childLocalValue);
            }
			i++;
		}
	}
}
