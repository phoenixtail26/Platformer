using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(MovementSenses))]
public class MovementSensesInspector : Editor 
{
	MovementSenses _senses;
	
	void OnSceneGUI()
	{
		_senses = target as MovementSenses;
				
		Transform t = _senses.transform;
		
		MovementSenses.SenseOffsets offsets = _senses.offsets;
		
		offsets.footOffset = Handles.PositionHandle( t.position + offsets.footOffset, Quaternion.identity ) - t.position;
		Handles.Label(t.position + offsets.footOffset, "Foot Check");
		
		offsets.handOffsetP1 = Handles.PositionHandle( t.position + offsets.handOffsetP1, Quaternion.identity ) - t.position;
		offsets.handOffsetP2 = Handles.PositionHandle( t.position + offsets.handOffsetP2, Quaternion.identity ) - t.position;
		Handles.Label(t.position + offsets.handOffsetP1, "Hand Wall Check");
		
		offsets.handOffsetP2.y = offsets.handOffsetP1.y;
		
		offsets.aboveHandOffset = Handles.PositionHandle( t.position + offsets.aboveHandOffset, Quaternion.identity ) - t.position;
		Handles.Label(t.position + offsets.aboveHandOffset, "Above Hand Wall Check");
		
		offsets.crotchOffset = Handles.PositionHandle( t.position + offsets.crotchOffset, Quaternion.identity ) - t.position;
		Handles.Label(t.position + offsets.crotchOffset, "Crotch Wall Check");
		
		offsets.groundCheckOffset = Handles.PositionHandle( t.position + offsets.groundCheckOffset, Quaternion.identity ) - t.position;
		Handles.Label(t.position + offsets.groundCheckOffset, "Ground Check");
		
		_senses.RecalcValues();
		
		Handles.DrawLine(t.position, t.position + offsets.leftFootOffset);
		Handles.DrawLine(t.position, t.position + offsets.rightFootOffset);
		
		Handles.DrawLine(t.position + offsets.handOffsetP1, t.position + offsets.handOffsetP2);
		
		Handles.color = Color.red;
		Handles.DrawLine(t.position + offsets.aboveHandOffset, t.position + offsets.aboveHandOffsetP2);
		
		Handles.color = Color.blue;
		Handles.DrawLine(t.position + offsets.crotchOffset, t.position + offsets.crotchOffsetP2);
		
		Handles.DrawLine(t.position + offsets.leftFootOffset, t.position + offsets.leftFootGroundCheck);
		Handles.DrawLine(t.position + offsets.rightFootOffset, t.position + offsets.rightFootGroundCheck);
		
	}
}
