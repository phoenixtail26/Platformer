using UnityEngine;
using System.Collections;

public class Pickup : MonoBehaviour 
{
	public enum PickupType
	{
		DoubleJump,
		WallJump,
		WallCling,
		Gun,
		Dash,
	}
	
	public PickupType type;
	
	public void Collect()
	{
		gameObject.SetActive(false);
		
		GameObject.Destroy(gameObject);
	}
}
