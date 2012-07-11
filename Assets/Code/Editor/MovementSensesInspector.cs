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
		
		_senses._footOffset = Handles.PositionHandle( t.position + _senses._footOffset, Quaternion.identity ) - t.position;
		
		_senses.handOffsetP1 = Handles.PositionHandle( t.position + _senses.handOffsetP1, Quaternion.identity ) - t.position;
		_senses.handOffsetP2 = Handles.PositionHandle( t.position + _senses.handOffsetP2, Quaternion.identity ) - t.position;
		
		_senses.handOffsetP2.y = _senses.handOffsetP1.y;
		
		_senses.aboveHandOffset = Handles.PositionHandle( t.position + _senses.aboveHandOffset, Quaternion.identity ) - t.position;
		
		_senses.crotchOffset = Handles.PositionHandle( t.position + _senses.crotchOffset, Quaternion.identity ) - t.position;
		
		_senses.RecalcValues();
		
		Handles.DrawLine(t.position, t.position + _senses._leftFootOffset);
		Handles.DrawLine(t.position, t.position + _senses._rightFootOffset);
		
		Handles.DrawLine(t.position + _senses.handOffsetP1, t.position + _senses.handOffsetP2);
		
		Handles.color = Color.red;
		Handles.DrawLine(t.position + _senses.aboveHandOffset, t.position + _senses.aboveHandOffsetP2);
		
		Handles.color = Color.blue;
		Handles.DrawLine(t.position + _senses.crotchOffset, t.position + _senses.crotchOffsetP2);
		
	}
}
