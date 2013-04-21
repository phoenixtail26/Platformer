using UnityEngine;
using System.Collections;
using Framework;

public class Gun : MonoBehaviour 
{
	[SerializeField]
	Animation _gunAnimation;
	
	[SerializeField]
	float _fireDelay = 0.25f;
	
	float _drawingTime = 0.25f;
	
	GameTimer _fireDelayTimer;
	
	[SerializeField]
	Bullet _bulletPrefab;
	
	GenericPool<Bullet> _bulletPool;
	
	[SerializeField]
	Transform _bulletStartPoint;
	
	GameTimer _drawingTimer;
	bool _drawn = false;
	
	bool _fireWhenAble = false;
	
	public bool isDrawn
	{
		get { return _drawn; }
	}
	
	void Awake()
	{
		_fireDelayTimer = new GameTimer(_fireDelay);
		
		_drawingTime = _gunAnimation["Gun Draw"].length;
		_drawingTimer = new GameTimer(_drawingTime);
		
		_bulletPool = GenericPool<Bullet>.CreatePool(_bulletPrefab, 30);
		
		_gunAnimation.Play("Gun Draw");
		_gunAnimation.Sample();
		_gunAnimation.Stop();
	}
	
	public void Draw()
	{
		if ( !_drawingTimer.running )
		{
			_fireWhenAble = false;
			_drawingTimer.Reset();
			
			gameObject.SetActive(true);
			
			/*_gunAnimation.Play("Gun Recoil");
			_gunAnimation["Gun Recoil"].normalizedTime = 0;
			_gunAnimation.Sample();
			_gunAnimation.Stop();*/
			_gunAnimation.Play("Gun Draw");
		}
	}
	
	public void Holster()
	{
		if ( _fireDelayTimer.running )
		{
			return;
		}
		
		if ( isDrawn )
		{
			_gunAnimation.Play("Gun Holster");
			_drawn = false;
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
		
		if ( _drawingTimer.running )
		{
			if ( _drawingTimer.Update(GameTime.deltaTime) )
			{
				_drawingTimer.StopTimer();
				_drawn = true;
				
				if ( _fireWhenAble )
				{
					Fire();
				}
			}
		}
	}
	
	public void Fire()
	{
		if ( !isDrawn )
		{
			Draw();
			_fireWhenAble = true;
			return;
		}
		
		if ( !_fireDelayTimer.running )
		{
			_gunAnimation.Stop();
			_gunAnimation.Play("Gun Recoil");
			_fireDelayTimer.Reset();
			Bullet bullet = _bulletPool.GetItem();
			bullet.pool = _bulletPool;
			bullet.transform.position = _bulletStartPoint.position;
			bullet.Shoot(transform.right);
		}
	}
	
	
}
