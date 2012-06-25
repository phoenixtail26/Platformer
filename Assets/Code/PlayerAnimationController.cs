using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerMovementController))]
public class PlayerAnimationController : MonoBehaviour 
{
	[SerializeField]
	Transform _model;
	
	[SerializeField]
	float _rotateSpeed = 2;
	
	PlayerMovementController _moveControl;
	
	
	void Awake()
	{
		_moveControl = GetComponent<PlayerMovementController>();
	}
	
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		_model.rotation = Quaternion.Lerp( _model.rotation, Quaternion.LookRotation(_moveControl.facingDirection), GameTime.deltaTime * _rotateSpeed );
		//_moveControl.facingDirection
	}
}
