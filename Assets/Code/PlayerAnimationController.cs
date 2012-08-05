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
		
		if ( _moveControl.isCrouching )
		{
			_model.localScale = Vector3.Lerp( _model.localScale, new Vector3(1,0.5f,1), GameTime.deltaTime * 15 );
		}
		else
		{
			_model.localScale = Vector3.Lerp( _model.localScale, Vector3.one, GameTime.deltaTime * 15 );
		}
	}
}
