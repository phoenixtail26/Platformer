using UnityEngine;
using System.Collections;
using Framework;

public class Bullet : MonoBehaviour 
{
	[SerializeField]
	float _speed = 5;
	
	[SerializeField]
	float _timeToLive = 3;
	
	Rigidbody _rigidbody;
	
	public GenericPool<Bullet> pool;
	
	GameTimer _aliveTimer;
	
	void Awake()
	{
		_aliveTimer = new GameTimer(_timeToLive);
		_rigidbody = rigidbody;
	}
	
	public void Shoot( Vector3 dir )
	{
		_aliveTimer.Reset();
		_rigidbody.velocity = dir * _speed;
	}
	
	void Update()
	{
		if ( _aliveTimer.Update(GameTime.deltaTime) )
		{
			Destroy();
		}
	}
	
	public void Destroy()
	{
		pool.ReturnItem(this);
	}
	
	void OnCollisionEnter( Collision other )
	{
		Destroy();
	}
	
}
