using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Ground))]
public class GroundEditor : Editor 
{
	public enum BrushType
	{
		Ground,
		ClimbableWall,
		Eraser
	}
	
	const string groundLayerName = "Ground";
	const string climbableWallTag = "ClimbableWall";
	
	public static BrushType brushType = BrushType.Ground;
	
	bool _painting = false;
	
	bool _enablePainting = true;
	
	float _gridScale = 1;
	
	Ground _ground;
	
	BrushType _prevBrushType = BrushType.Ground;
	
	public override void OnInspectorGUI()
	{
		_ground = (target as Ground);
		
		if ( _enablePainting )
		{
			GUI.color = Color.yellow;
		}
		else
		{
			GUI.color = Color.white;
		}
		
		if ( GUILayout.Button( "Painting" ) )
		{
			_enablePainting = !_enablePainting;
		}
		
		GUI.color = Color.white;
		
		EditorGUILayout.Separator();
		
		EditorGUILayout.LabelField("Brushes");
		foreach ( BrushType type in System.Enum.GetValues(typeof(BrushType)) )
		{
			if ( brushType == type )
			{
				GUI.color = Color.yellow;
			}
			else
			{
				GUI.color = Color.white;
			}
			
			if ( GUILayout.Button( type.ToString() ) )
			{
				brushType = type;
			}
		}
		
		GUI.color = Color.white;
		
		EditorGUILayout.Separator();
		
		_ground.normalGroundMaterial = EditorGUILayout.ObjectField( "Ground Material", _ground.normalGroundMaterial, typeof(Material), false ) as Material;
		_ground.climbableWallMaterial = EditorGUILayout.ObjectField( "Climable Wall Material", _ground.climbableWallMaterial, typeof(Material), false ) as Material;
		
		EditorGUILayout.Separator();
		
		if ( GUILayout.Button( "Clear Grid" ) )
		{
			if ( EditorUtility.DisplayDialog( "Clear Grid?", "Are you sure you want to clear this grid?", "Yes", "No" ) )
			{
				int count = _ground.transform.childCount;
				for ( int i = 0; i < count; i++ )
				{
					GameObject.DestroyImmediate(_ground.transform.GetChild(0).gameObject);
				}
			}
		}
	}
	
	void DrawGrid()
	{
		// Draw Grid
		Camera cam = Camera.current;
		
		Vector3 bottomLeft = cam.ViewportToWorldPoint( Vector3.zero );
		Vector3 topRight = cam.ViewportToWorldPoint( Vector3.one );
		
		bottomLeft.x = Mathf.Floor(bottomLeft.x) - 1;
		bottomLeft.y = Mathf.Floor(bottomLeft.y);
		bottomLeft.z = 0;
		
		topRight.x = Mathf.Ceil(topRight.x);
		topRight.y = Mathf.Ceil(topRight.y);
		topRight.z = 0;
		
		float xDiff = topRight.x - bottomLeft.x;
		float yDiff = topRight.y - bottomLeft.y;
				
		Vector3 p1, p2;
		
		Handles.color = new Color(1,1,1,0.25f);
		for ( int i = 0; i < xDiff; i++ )
		{
			p1 = bottomLeft;
			p2 = bottomLeft;
			
			p1.x += i * _gridScale;
			p2.x += i * _gridScale;
			p2.y = topRight.y;
			
			Handles.DrawLine( p1, p2 );	
		}
		
		for ( int i = 0; i < yDiff; i++ )
		{
			p1 = bottomLeft;
			p2 = bottomLeft;
			
			p1.y += i * _gridScale;
			p2.y += i * _gridScale;
			p2.x = topRight.x;
			
			Handles.DrawLine( p1, p2 );	
		}
	}
	
	void PlaceTile( Vector2 screenPos )
	{
		GameObject currentGo = GetTileAt( screenPos );
				
		Vector3 worldPos = Camera.current.ScreenToWorldPoint(screenPos);
		worldPos.z = 0;
		
		Vector3 pos = worldPos;
		pos.x = Mathf.Floor(pos.x) + (_gridScale * 0.5f);
		pos.y = Mathf.Floor(pos.y) + (_gridScale * 0.5f);
		
		if ( currentGo == null )
		{
			switch ( brushType )
			{
			case BrushType.Ground:
			case BrushType.ClimbableWall:
				{
					GameObject newGo = GameObject.CreatePrimitive(PrimitiveType.Cube);
					newGo.transform.position = pos;
					newGo.transform.parent = _ground.transform;
					currentGo = newGo;
					break;
				}	
			}
		}
		
		ChangeTile(currentGo);
	}
	
	void ChangeTile( GameObject tile )
	{
		switch( brushType )
		{
		case BrushType.Eraser:
			GameObject.DestroyImmediate(tile);
			break;
			
		case BrushType.Ground:
			tile.name = "Ground";
			tile.layer = LayerMask.NameToLayer(groundLayerName);
			tile.renderer.material = _ground.normalGroundMaterial;
			tile.tag = "Untagged";
			break;
			
		case BrushType.ClimbableWall:
			tile.name = "Climbable Wall";
			tile.layer = LayerMask.NameToLayer(groundLayerName);
			tile.renderer.material = _ground.climbableWallMaterial;
			tile.tag = climbableWallTag;
			break;
		}
	}
	
	GameObject GetTileAt( Vector2 screenPos )
	{
		Ray ray = Camera.current.ScreenPointToRay(screenPos);
		RaycastHit hitInfo;
		
		if ( Physics.Raycast(ray, out hitInfo ) )
		{
			GameObject go = hitInfo.collider.gameObject;
			if ( go.transform.parent == _ground.transform )
			{
				return go;
			}
		}
			
		return null;
	}
	
	
	public void OnSceneGUI()
	{
		_ground = (target as Ground);
					
		DrawGrid();
		
		if (!_enablePainting )
		{
			return;
		}
		
		
		Camera cam = Camera.current;
		Event e = Event.current;
 
		Vector2 mpos = e.mousePosition;
		mpos.y = -(mpos.y - cam.pixelHeight);
		
		
		
	    if ( e.type == EventType.MouseDown )
	    {
			if ( e.button == 0 || e.button == 1 )
			{
				_painting = true;
				
				Undo.RegisterSceneUndo("Undo Tile Painting");
				
				if ( e.button == 1 )
				{
					_prevBrushType = brushType;
					brushType = BrushType.Eraser;
				}
				
				PlaceTile( mpos );
				
				e.Use();
			}
			
	       //if we're clicking within our desired area, block regular input
	      /* if ( isInArea( e.mousePosition )
	       {
	         blockingMouseInput = true;
	       }*/
	    }
	    else if ( e.type == EventType.MouseDrag )
	    {
			if ( _painting )
			{
				float maxDist = 5;
				if ( Mathf.Abs(e.delta.x) > maxDist || Mathf.Abs(e.delta.y) > maxDist )
				{
					//Vector2 diff = mpos - _lastMousePosition;
					Vector2 dir = -e.delta.normalized;
					
					float mag = e.delta.magnitude;
					
					int it = Mathf.FloorToInt(mag / maxDist);
					
					for ( int i = 0; i < it; i++ )
					{
						PlaceTile(mpos + dir * i * maxDist );
					}
				}
				
				PlaceTile(mpos);
				
				e.Use();
			}
	    }
	    else if ( e.type == EventType.MouseMove )
	    {
			
			//Vector3 wpoint = cam.ScreenToWorldPoint(pos);
			//Debug.Log(wpoint);
	    }
	    else if ( e.type == EventType.MouseUp )
	    {
			if ( e.button == 0 || e.button == 1 )
			{
				_painting = false;
				
				if ( e.button == 1 )
				{
					brushType = _prevBrushType;
				}
				
				e.Use();
				
				Repaint();
			}
			
			
	       /*if ( blockingMouseInput )
	       {
	         e.Use();
	       }
	       blockingMouseInput = false;*/
	    }
	    else if ( e.type == EventType.Layout )
	    {
	       //somehow this allows e.Use() to actually function and block mouse input
	       HandleUtility.AddDefaultControl( GUIUtility.GetControlID( GetHashCode(), FocusType.Passive ) );
	    }
		
	}
}
