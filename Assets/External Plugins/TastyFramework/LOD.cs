using UnityEngine;
using System.Collections;

public enum LODLevel
{
	LOD0,
	LOD1,
	LOD2,
	LODCulled
}

public class LOD : MonoBehaviour 
{
	public delegate void LODLevelChangedFunction( LOD lod );
	
	//public Renderer _renderer;
	public Renderer[] _renderers = new Renderer[3];
	public float[] _LODSizes = new float[3];
	public LODLevel _LODLevel;
	public LODLevelChangedFunction LODLevelChangedEvent;
	public float percentageSizeOnScreen = 1;
	
	[SerializeField]
	Bounds _bounds;
	
	Transform _transform;
	bool _started = false;
	
	public Vector3 boundsMin = Vector3.zero;
	public Vector3 boundsMax = Vector3.zero;
	public Vector3 boundsCenter = Vector3.zero;
	
	public Renderer currentRenderer { get; set; }
	
	public Renderer[] renderers
	{
		get { return _renderers; }
	}
	
	public Bounds bounds
	{
		get 
		{
			return _bounds; 
		}
	}
	
	public LODLevel level
	{
		get { return _LODLevel; }
		set 
		{
			if ( value != _LODLevel )
			{
				_LODLevel = value;
				if ( LODLevelChangedEvent != null )
				{
					LODLevelChangedEvent(this);
				}
			}
		}
	}
	
	void Start()
	{
		_transform = transform;
		_started = true;
	}
	
	public void RecalcBounds()
	{
		if ( !_started )
		{
			Start ();
		}
		
		_bounds = _renderers[0].bounds;
		
		boundsCenter = _transform.InverseTransformPoint(bounds.center);
	}
	
	public void RecalcLOD( Camera cam )
	{
		if ( !_started )
		{
			Start();
		}
		
		boundsMin = boundsCenter - _bounds.extents;
		boundsMax = boundsCenter + _bounds.extents;
		boundsMin = _transform.TransformPoint(boundsMin);
		boundsMax = _transform.TransformPoint(boundsMax);
		
		Vector3 screenMin = cam.WorldToScreenPoint(boundsMin);
		Vector3 screenMax = cam.WorldToScreenPoint(boundsMax);
		
		Vector3 diff = screenMax - screenMin;
		
		float heightPerc = Mathf.Abs(diff.y) / cam.GetScreenHeight();
		float widthPerc = Mathf.Abs(diff.x) / cam.GetScreenWidth();
		
		percentageSizeOnScreen = heightPerc > widthPerc ? heightPerc : widthPerc;
		
		LODLevel newLevel = LODLevel.LOD0;
		
		if ( percentageSizeOnScreen >= _LODSizes[2] )
		{
			newLevel = LODLevel.LOD0;
		}
		else if ( percentageSizeOnScreen >= _LODSizes[1] )
		{
			newLevel = LODLevel.LOD1;
		}
		else if ( percentageSizeOnScreen >= _LODSizes[0] )
		{
			newLevel = LODLevel.LOD2;
		}
		else
		{
			newLevel = LODLevel.LODCulled;
		}
		
		newLevel = EnableLODRenderers(newLevel);
		
		level = newLevel;
	}
	
	LODLevel EnableLODRenderers( LODLevel newLevel )
	{
		LODLevel retLevel = newLevel;
		currentRenderer = null;
		
		for ( int i = 0; i < (int)LODLevel.LODCulled; i++ )
		{
			if ( i == (int)newLevel )
			{
				if ( _renderers[i] != null )
				{
					_renderers[i].enabled = true;
					currentRenderer = _renderers[i];
					if ( level == newLevel && !_renderers[i].isVisible )
					{
						retLevel = LODLevel.LODCulled;
					}
				}
			}
			else
			{
				if ( _renderers[i] != null )
				{
					_renderers[i].enabled = false;
				}
			}
		}
		
		return retLevel;
	}
	
	void Update ()
	{
		RecalcLOD( Camera.main );		
	}
}
