////////////////////////////////////////////////////////////////////////////////////////
// 
// File:        AT_Pose.cs
// Author:      Gavin Hayler
// Date:        01/02/2012
//
////////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

////////////////////////////////////////////////////////////////////////////////////////
// 
// Class:       AT_Pose
// Description: Tree node that contains information about pose and how to 
//              process it. Faster than using a static animation
//
////////////////////////////////////////////////////////////////////////////////////////
public class AT_Pose: AT_AnimationBase
{
	Transform[] _bones;
	Vector3[] _bonesStartPos;
	Vector3[] _bonesEndPos;
	Vector3[] _bonesPosDelta;
	Vector3[] _currentPosDelta;
	
	Quaternion[] _bonesStartRot;
	Quaternion[] _bonesEndRot;
	Quaternion[] _bonesRotDelta;
	Quaternion[] _currentRotDelta;
	
	public float totalPoseWeight = 0;
	
	bool _needsUpdate = true;
	
	float _currentlyUsedWeight = 0;
	public float currentWeight
	{
		get 
		{
			return _currentlyUsedWeight;
		}
	}
	
	public Quaternion[] DeltaRot
	{
		get 
		{
			return _currentRotDelta;
		}
	}
	
	public Vector3[] DeltaPos
	{
		get 
		{
			return _currentPosDelta;
		}
	}
	
	public AT_Pose() : base()
	{
        NodeType = AT_NodeType.kPose;
	}
	
	public override void Init()
    {
		/*if ( SteppedUpdateManager.hasInstance )
		{
			SteppedUpdateManager.instance.RegisterSteppedUpdate(GetType(), SteppedUpdate, 1);
		}*/
		
        // Set up the currently active anim and AnimState
        ChangeActiveAnimation(m_animationName);

        if ( m_animState != null )
        {
			SetupPoseData();
			RecalcCurrentDeltas();
        }
		
		ParentTree.RegisterPose(this);
    }
	
	public void OnDestroy()
	{
		ParentTree.DeregisterPose(this);
	}
	
	void GetBonesTransformDetails( ref Vector3[] positions, ref Quaternion[] rotations )
	{
		positions = new Vector3[_bones.Length];
		rotations = new Quaternion[_bones.Length];
		int i = 0;
		foreach( Transform t in _bones )
		{
			positions[i] = t.localPosition;
			rotations[i] = t.localRotation;
			i++;
		}
	}
	
	void CalculateDeltas()
	{
		_bonesPosDelta = new Vector3[_bones.Length];
		_bonesRotDelta = new Quaternion[_bones.Length];
		_currentPosDelta = new Vector3[_bones.Length];
		_currentRotDelta = new Quaternion[_bones.Length];
		for ( int i = 0; i < _bones.Length; i++ )
		{
			_bonesPosDelta[i] = _bonesEndPos[i] - _bonesStartPos[i];
			//_bonesRotDelta[i] = _bonesEndRot[i] - _bonesStartRot[i];
			//Quaternion relative = Quaternion.Inverse(a) * b;
			_bonesRotDelta[i] = Quaternion.Inverse(_bonesStartRot[i]) * _bonesEndRot[i];
			/*Debug.Log(_bones[i].name + "     -     " + _bonesStartPos[i] + "    ,    " + _bonesEndPos[i] + "     ,     " + _bonesPosDelta[i]);
			if ( _bonesStartPos[i] != _bonesEndPos[i] )
			{
				Debug.LogWarning("Positions are different");
			}
			if ( _bonesStartRot[i] != _bonesEndRot[i] )
			{
				Debug.LogWarning("Rotations are different");
			}*/
		}
	}
	
	void SetupPoseData()
	{
		_bones = m_ParentTree.bones;
		
		// Apply our various properties to the new AnimState
		m_animState.enabled = true;
		m_animState.layer = m_animationLayer + m_animationLayerOffset;
        m_animState.wrapMode = WrapMode.ClampForever;
        m_animState.normalizedTime = 0;
		m_animState.weight = 1;
		m_subject.Sample();
		
		GetBonesTransformDetails( ref _bonesStartPos, ref _bonesStartRot );
		
		m_animState.normalizedTime = 1;
		m_subject.Sample();
		
		GetBonesTransformDetails( ref _bonesEndPos, ref _bonesEndRot );
		
		CalculateDeltas();
		
		m_animState.enabled = false;
	}

    // Change what animation this node is currently using
    void ChangeActiveAnimation(string animName)
    {
        // Only do this is the application is not playing. We're going to be creating duplicates of animations and 
        // don't want those hanging around inside the editor
        if (Application.isPlaying)
        {
            if (m_animState != null)
            {               
                // Disable current AnimState
                m_animState.enabled = false;
                // Remove AnimState from our subject
                m_subject.RemoveClip(m_subject[m_actualAnimationName].clip);                
            }

            if (m_animationName != "")
            {
                // Create a unique name for our new AnimState
                m_actualAnimationName = m_animationName + "_" + m_animationLayer;
                // Duplicate the reference animation to create our unique AnimState
                m_subject.AddClip(m_subject[m_animationName].clip, m_actualAnimationName);
                m_animState = m_subject[m_actualAnimationName];
            }
            else
            {
                m_animState = null;
            }
        }
    }
	
	void RecalcCurrentDeltas()
	{
		if ( _bones != null )
		{
			for ( int i = 0; i < _bones.Length; i++ )
			{
				_currentPosDelta[i] = Vector3.Lerp(Vector3.zero,_bonesPosDelta[i], _currentlyUsedWeight /* * Mathf.Clamp01(_currentlyUsedWeight / totalPoseWeight)*/);
				_currentRotDelta[i] = Quaternion.Slerp(Quaternion.identity, _bonesRotDelta[i], _currentlyUsedWeight /** Mathf.Clamp01(_currentlyUsedWeight / totalPoseWeight)*/);
			}
			_needsUpdate = false;
		}
	}
	
	public override void UpdateAnimation( float timeDelta )
	{
		float weight = ActualBlendWeight * m_weightMultiplier;
		
		if ( !_needsUpdate )
		{
			_needsUpdate = (weight != _currentlyUsedWeight);
		}
		
		_currentlyUsedWeight = weight;
		
		if ( _needsUpdate )
		{
			RecalcCurrentDeltas();
		}
			
		/*if ( !SteppedUpdateManager.hasInstance )
		{
			SteppedUpdate();
		}*/
	}
	
	/*public void SteppedUpdate()
	{
		if ( _needsUpdate )
		{
			RecalcCurrentDeltas();
		}
	}*/
	
	/*void LateUpdate()
	{
		int i = 0;
		float weight = ActualBlendWeight * m_weightMultiplier;
		
		foreach(Transform t in _bones)
		{
			t.localPosition += _bonesPosDelta[i] * weight;
			t.localRotation = t.localRotation * Quaternion.Slerp(Quaternion.identity, _bonesRotDelta[i], weight);
			i++;
		}
	}*/
	
}
