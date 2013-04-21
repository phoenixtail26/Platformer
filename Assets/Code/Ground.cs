using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GroundType
{
	Ground,
	ClimbableWall
}

public class Ground : MonoBehaviour 
{
	public Material climbableWallMaterial;
	public Material normalGroundMaterial;
}
