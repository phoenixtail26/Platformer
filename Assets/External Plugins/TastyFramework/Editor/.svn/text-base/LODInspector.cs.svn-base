using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(LOD))]
public class LODInspector : Editor
{
	LOD _lod = null;
	
	//bool moveBack = false;
	
	void OnSceneGUI()
	{
		_lod = target as LOD;
		
		if ( _lod != null )
		{
			if ( !Application.isPlaying )
			{
				//_lod.RecalcBounds();
				_lod.RecalcLOD(Camera.current);		
				/*
				//if ( moveBack )
				//{
					Camera cam = SceneView.lastActiveSceneView.camera;
					Vector3 pos = cam.transform.localPosition;
					pos.z = -10.1f;
					cam.transform.localPosition = pos;
				
					cam.
					Vector3 moveDir = cam.transform.InverseTransformDirection( Vector3.forward );	
				 	Vector3 position = SceneView.lastActiveSceneView.pivot;
        			position.z += 1;//moveDir;
        			SceneView.lastActiveSceneView.pivot = position;
				
				 	SceneView.lastActiveSceneView.Repaint();
				//}
				*/

			
			}	
			
			DrawBoundingBox();
			
		}
	}
	
	void DrawBoundingBox()
	{
		Vector3 min = _lod.boundsMin;
		Vector3 max = _lod.boundsMax;
		Vector3[] verts = new Vector3[8];
		verts[0] = new Vector3( min.x, min.y, min.z );
		verts[1] = new Vector3( max.x, min.y, min.z );
		verts[2] = new Vector3( max.x, min.y, max.z );
		verts[3] = new Vector3( min.x, min.y, max.z );
		verts[4] = new Vector3( min.x, max.y, min.z );
		verts[5] = new Vector3( max.x, max.y, min.z );
		verts[6] = new Vector3( max.x, max.y, max.z );
		verts[7] = new Vector3( min.x, max.y, max.z );
		
		Color col = Color.white;
		switch ( _lod.level )
		{
		case LODLevel.LOD0:
			col = Color.blue;
			break;
		case LODLevel.LOD1:
			col = Color.green;
			break;
		case LODLevel.LOD2:
			col = Color.yellow;
			break;
		case LODLevel.LODCulled:
			col = Color.red;
			break;
		}
		Handles.color = col;
		
		Handles.DrawLine(verts[0], verts[1] );
		Handles.DrawLine(verts[1], verts[2] );
		Handles.DrawLine(verts[2], verts[3] );
		Handles.DrawLine(verts[3], verts[0] );

		Handles.DrawLine(verts[4], verts[5] );
		Handles.DrawLine(verts[5], verts[6] );
		Handles.DrawLine(verts[6], verts[7] );
		Handles.DrawLine(verts[7], verts[4] );
		
		Handles.DrawLine(verts[0], verts[4] );
		Handles.DrawLine(verts[1], verts[5] );
		Handles.DrawLine(verts[2], verts[6] );
		Handles.DrawLine(verts[3], verts[7] );
		
		Handles.Label( new Vector3( min.x, max.y, min.z ), _lod.level.ToString());
	}
	
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		
		_lod = target as LOD;
		
		float newPerc = EditorGUILayout.Slider( "Size Percentage", _lod.percentageSizeOnScreen, 1.0f, 0.0f, GUILayout.ExpandWidth(true));
		if ( newPerc != _lod.percentageSizeOnScreen )
		{
			if ( newPerc < _lod.percentageSizeOnScreen )
			{
							
			}
			//moveBack = true;	
		}
		else
		{
			//moveBack = false;
		}
		
		_lod.percentageSizeOnScreen = newPerc;
		
		for( int i = 0; i < _lod.renderers.Length; i++ )
		{
			_lod.renderers[i] = EditorGUILayout.ObjectField(((LODLevel)i).ToString(), _lod.renderers[i], typeof(Renderer), true, GUILayout.ExpandWidth(true)) as Renderer;
		}
		
		
		if ( GUILayout.Button( "Recalculate Bounds", GUILayout.ExpandWidth(true) ) )
		{
			_lod.RecalcBounds();
		}
		
	}
}
