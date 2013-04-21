using UnityEngine;
using System.Collections;
using Framework;

[System.Serializable]
public class Abilities
{
	public bool doubleJumpEnabled = false;
	public bool wallJumpEnabled = false;
	public bool wallClingEnabled = false;
	public bool gunEnabled = false;
	public bool dashEnabled = false;
}

public class Player : MonoBehaviour 
{
	[SerializeField]
	PlayerMovementController _moveCon;
		
	[SerializeField]
	Gun _gun;
	
	[SerializeField]
	float _gunDrawnMovementFactor = 0.5f;
	
	[SerializeField]
	Abilities _abilities = new Abilities();
	
	// Use this for initialization
	void Start () 
	{
		_moveCon.playerAbilities = _abilities;
	}
	
	void Update()
	{
		if ( _abilities.gunEnabled )
		{
			_gun.gameObject.SetActive(true);
		}
		else
		{
			_gun.gameObject.SetActive(false);
		}
	}
	
	public void Move( Vector2 vec )
	{
		if ( _gun.isDrawn )
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
		if ( !_abilities.gunEnabled )
		{
			return;
		}
		
		if ( !_gun.isDrawn )
		{
			_gun.Draw();
		}
	}
	
	public void HolsterGun()
	{
		if ( _gun.isDrawn )
		{
			_gun.Holster();
		}
	}
	
	public void FireGun()
	{
		if ( !_abilities.gunEnabled )
		{
			return;
		}
		
		if ( !_gun.isDrawn )
		{
			DrawGun();
			_gun.Fire();
		}
		else
		{
			_gun.Fire();
		}
	}
	
	public void Dash()
	{
		if ( _abilities.dashEnabled )
		{
			_moveCon.Dash();
		}
	}
	
	void CollectPickup( Pickup pickup )
	{
		switch( pickup.type )
		{
		case Pickup.PickupType.DoubleJump:
			_abilities.doubleJumpEnabled = true;
			break;

		case Pickup.PickupType.WallJump:
			_abilities.wallJumpEnabled = true;
			break;

		case Pickup.PickupType.WallCling:
			_abilities.wallClingEnabled = true;
			break;

		case Pickup.PickupType.Gun:
			_abilities.gunEnabled = true;
			break;
			
		case Pickup.PickupType.Dash:
			_abilities.dashEnabled = true;
			break;
		}
		
		pickup.Collect();
	}
	
	
	void OnTriggerEnter( Collider col )
	{
		Pickup pickup = col.gameObject.GetComponent<Pickup>();
			
		if ( pickup != null )
		{
			CollectPickup( pickup );
		}
	}
}
