using UnityEngine;
using System.Collections;
using Framework;

public class JumpTester : MonoBehaviour 
{
	public PlayerMovementController player;
	
	GameTimer _jumpTimer = new GameTimer(0.5f);
	GameTimer _jumpStopTimer = new GameTimer(0.2f);
	
	// Use this for initialization
	void Start () 
	{
		_jumpTimer.Reset();
		_jumpStopTimer.Reset();
	}
	
	// Update is called once per frame
	void Update () 
	{
		player.Move(new Vector2(1,0));
		
		if ( _jumpTimer.running )
		{
			if ( _jumpTimer.Update(GameTime.deltaTime) )
			{
				if ( player.onGround )
				{
					player.StartJump();
					_jumpTimer.StopTimer();
					_jumpStopTimer.Reset();
				}
			}	
		}
		else
		{
			if ( _jumpStopTimer.Update(GameTime.deltaTime) )
			{
				_jumpStopTimer.StopTimer();
				player.EndJump();
			}
			
			if (player.onGround )//( player.transform.position.x > 18 )
			{
				player.Stop();
				_jumpTimer.Reset();
				Vector3 pos = player.transform.position;
				pos.x = 8;
				pos.y = 0.01f;
				player.transform.position = pos;
			}
		}
	}
}
