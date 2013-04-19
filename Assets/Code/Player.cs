using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour 
{
	[SerializeField]
	PlayerMovementController _moveCon;
		
	[SerializeField]
	Gun _gun;
	
	bool _gunDrawn = false;
	
	[SerializeField]
	float _gunDrawnMovementFactor = 0.5f;
	
	// Use this for initialization
	void Start () 
	{
		_gun.Draw(false);
	}
	
	public void Move( Vector2 vec )
	{
		if ( _gunDrawn )
		{
			_moveCon.Move(vec * _gunDrawnMovementFactor);
		}
		else
		{
			_moveCon.Move(vec);
		}
	}
	
	public void DrawGun()
	{
		if ( !_gunDrawn )
		{
			_gun.Draw( true );
			_gunDrawn = true;
		}
		else
		{
			_gun.Draw( false );
			_gunDrawn = false;
		}
	}
	
	public void FireGun()
	{
		if ( !_gunDrawn )
		{
			DrawGun();
		}
		else
		{
			_gun.Fire();
		}
	}
}
