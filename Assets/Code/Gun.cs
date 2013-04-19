using UnityEngine;
using System.Collections;
using Framework;

public class Gun : MonoBehaviour 
{
	[SerializeField]
	Animation _gunAnimation;
	
	[SerializeField]
	float _fireDelay = 0.25f;
	
	GameTimer _fireDelayTimer;
	
	[SerializeField]
	Bullet _bulletPrefab;
	
	GenericPool<Bullet> _bulletPool;
	
	void Awake()
	{
		_fireDelayTimer = new GameTimer(_fireDelay);
		
		GenericPool<Bullet>.CreatePool();
		
	}
	
	public void Draw( bool draw )
	{
		gameObject.SetActiveRecursively(draw);
		
		if ( draw )
		{
			_gunAnimation.Play("Gun Recoil");
			_gunAnimation["Gun Recoil"].normalizedTime = 0;
			_gunAnimation.Sample();
			_gunAnimation.Stop();
		}
	}
	
	void Update()
	{
		if ( _fireDelayTimer.running )
		{
			if ( _fireDelayTimer.Update(GameTime.deltaTime) )
			{
				_fireDelayTimer.StopTimer();
			}
		}
	}
	
	public void Fire()
	{
		//Debug.Log("bang!");
		if ( !_fireDelayTimer.running )
		{
			_gunAnimation.Stop();
			_gunAnimation.Play("Gun Recoil");
			_fireDelayTimer.Reset();
			//Bullet bullet = GenericPool<Bullet>.instance.Instantiate( _bulletPrefab );
		}
	}
	
	
}
