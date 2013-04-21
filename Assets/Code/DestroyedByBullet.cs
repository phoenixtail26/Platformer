using UnityEngine;
using System.Collections;

public class DestroyedByBullet : MonoBehaviour 
{
	void OnHitByBullet()
	{
		gameObject.SetActive(false);
	}
	
}
