using UnityEngine;
using System.Collections;
using UnityEditor;

public class ContainerNodeEditor : CustomNodeEditor
{
 	public override void OnInspectorGUI()
    {
        EditorGUILayout.Separator();

       	AT_Node newChildNode = EditorGUILayout.ObjectField("Add Child",null, typeof(AT_Node), true, GUILayout.ExpandWidth(true)) as AT_Node;
		if ( newChildNode != null )
		{
			AT_ContainerNode cont = target as AT_ContainerNode;
			cont.AddChild( newChildNode, true );
			//newChildNode.SetParent(target as AT_ContainerNode);
		}
		
		base.OnInspectorGUI();
    }
}

