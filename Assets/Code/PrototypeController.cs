using UnityEngine;
using System.Collections;

public class PrototypeController : MonoBehaviour 
{
	public PlayerMovementController player;
	
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		Vector2 inputVec = Vector2.zero;
		
		if ( Input.GetKey(KeyCode.LeftArrow) )
		{
			inputVec.x = -1;
		}
		
		if ( Input.GetKey(KeyCode.RightArrow) )
		{
			inputVec.x = 1;
		}
		
		if ( Input.GetKey(KeyCode.UpArrow) )
		{
			inputVec.y = 1;
		}

		if ( Input.GetKey(KeyCode.DownArrow) )
		{
			inputVec.y = -1;
		}
		
		player.Move( inputVec );
		
		if ( Input.GetKeyDown(KeyCode.Z) )
		{
			player.StartJump();
		}

		if ( Input.GetKeyUp(KeyCode.Z) )
		{
			player.EndJump();
		}

	}
}
