////////////////////////////////////////////////////////////////////////////////////////
// 
// File:        AT_ControlVariables.cs
// Author:      Gavin Hayler
// Date:        23/06/2011
//
////////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

////////////////////////////////////////////////////////////////////////////////////////
// 
// Class:       AT_ControlVariables
// Description: The node which contains all the control variables for this tree. There
//              should only be one per tree.
//
////////////////////////////////////////////////////////////////////////////////////////
public class AT_ControlVariables : AT_Node 
{
    // List all all the control variables
    [SerializeField]
    public List<ControlVariable> m_controlVariables = new List<ControlVariable>();

    public List<ControlVariable> Variables
    {
        get { return m_controlVariables; }
    }

	public AT_ControlVariables()
	{
        NodeType = AT_NodeType.kCustom;
	}

    public void Refresh()
    {
    }

    // Return a variable who's name is the same as the passed string
    public ControlVariable GetVariable(string name)
    {
        foreach (ControlVariable cv in m_controlVariables)
        {
			if ( cv != null )
			{
	            if (cv.m_variableName == name)
	            {
	                return cv;
	            }
			}
        }

        return null;
    }

    // Set the value of the passed variable
    public void SetValue(string name, float value)
    {
		ControlVariable cv = GetVariable(name);
		if ( cv != null )
		{
        	cv.Value = value;
		}
    }

    // Rename the passed variable
    public void RenameVariable(ControlVariable cv, string newName)
    {
        cv.m_variableName = newName;
    }

    // Create a new control variable
    public ControlVariable AddVariable ()
    {
    	GameObject go = new GameObject ("Variable");
    	go.transform.parent = transform;
  
    	ControlVariable cv = go.AddComponent<ControlVariable> ();
    	//ScriptableObject.CreateInstance<ControlVariable> ();
    	cv.m_variableName = "";
    	cv.MinValue = 0;
    	cv.MaxValue = 1;
    	m_controlVariables.Add (cv);
    	return cv;
    }

    // Delete a control variable
    public void RemoveVariable (ControlVariable cv)
    {
    	m_controlVariables.Remove (cv);
  
		GameObject.DestroyImmediate (cv.gameObject);
        //ScriptableObject.DestroyImmediate(cv);
    }


    void Awake()
    {
        Refresh();
    }
}
