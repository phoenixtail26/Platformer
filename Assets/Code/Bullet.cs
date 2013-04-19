using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour 
{
	[SerializeField]
	float _speed = 5;
	
	Rigidbody _rigidbody;
	
	void Awake()
	{
		_rigidbody = rigidbody;
	}
	
	public void Shoot( Vector3 dir )
	{
		_rigidbody.velocity = dir * _speed;
	}
	
}
