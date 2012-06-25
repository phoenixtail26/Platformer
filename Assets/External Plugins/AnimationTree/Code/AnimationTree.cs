////////////////////////////////////////////////////////////////////////////////////////
// 
// File:        AnimationTree.cs
// Author:      Gavin Hayler
// Date:        23/06/2011
//
////////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


////////////////////////////////////////////////////////////////////////////////////////
// 
// Class:       AnimationSyncData
// Description: Data used to sync up the synced animations
//
////////////////////////////////////////////////////////////////////////////////////////
class AnimationSyncData
{
    public AnimationState m_state;

    // The speed factor required for this animation's speed to equal 1
    public float m_desiredSpeedFactor = 1;

    // The speed value needed to make this animation the same length as the rest of the 
    // animations in the sync layer
    public float m_normalisedSpeed = 1;
	
	public AT_Animation node = null;
}


////////////////////////////////////////////////////////////////////////////////////////
// 
// Class:       AnimationTree
// Description: Class containing all the information for this animation tree. 
//
////////////////////////////////////////////////////////////////////////////////////////
public class AnimationTree : MonoBehaviour 
{
	public delegate void SendRequestDelegate( string request );
		
    // The root state of the animation tree. Should be a state-machine.
	public AT_State m_root;

    // The animation subject affected by this tree
	public Animation m_subject;
	
	public Renderer _renderer;

    // The tree node containing all the control variables
    public AT_ControlVariables m_controlVarsNode;
	
	public AT_ControlVariables ControlVariables
	{
		get { return m_controlVarsNode; }
	}
	
	[SerializeField]
	int _numFramesBetweenUpdates = 1;
	public int numFramesBetweenUpdates
	{
		get { return _numFramesBetweenUpdates; }
		set
		{
			_numFramesBetweenUpdates = value;
			if ( SteppedUpdateManager.hasInstance )
			{
				if ( _numFramesBetweenUpdates == 0 )
				{
					SteppedUpdateManager.instance.UnregisterSteppedUpdate( typeof(AnimationTree), SteppedUpdate );
				}
				else
				{
					SteppedUpdateManager.instance.UnregisterSteppedUpdate( typeof(AnimationTree), SteppedUpdate );
					SteppedUpdateManager.instance.RegisterSteppedUpdate( typeof(AnimationTree), SteppedUpdate, numFramesBetweenUpdates, true );
				}
			}
		}
	}
	
    // Dictionary of synced animations. Key is the sync layer, and all animations in that layer are stored in the list.
    Dictionary<int, List<AnimationSyncData>> m_syncedAnimations = new Dictionary<int, List<AnimationSyncData>>();
	
	List<AnimationClip> _preexistingAnimations = new List<AnimationClip>();
		
	List<AT_Node> _treeNodes = new List<AT_Node>();

    public List<ControlVariable> ControlVariablesList
    {
        get { return m_controlVarsNode.m_controlVariables; }
    }

	public AT_Node Root
	{
		get { return m_root; }
		set { m_root = value as AT_State; }
	}
	
	public Animation Subject
	{
		get { return m_subject; }
		set 
		{
			m_subject = value;
			ApplyNewSubjectToChildAnimationNodes (m_root);
		}
	}
	
	private bool _isDirty = false;
	public bool IsDirty
	{
		get { return _isDirty; }
		set { _isDirty = value; }
	}
	
	bool _cleanedUpAnimations = false;
	
	public Transform _rootBone;
	
	List<Transform> _bonesList = new List<Transform>();
	Transform[] _bones;
	List<AT_Pose> _poses = new List<AT_Pose>();
	
	bool _started = false;
	
	public Transform[] bones
	{
		get 
		{
			if ( !_started )
			{
				Start();
			}
			return _bones;
		}
	}
	
	public bool applyPoseData = true;
	public bool sampleAnimationData = true;
	
	public SendRequestDelegate sendRequestDelegate;
	
	void OnEnable()
	{
		if ( SteppedUpdateManager.hasInstance )
		{
			SteppedUpdateManager.instance.RegisterSteppedUpdate( typeof(AnimationTree), SteppedUpdate, numFramesBetweenUpdates, true );
		}
	}
	
	void OnDisable()
	{
		if ( SteppedUpdateManager.hasInstance )
		{
			SteppedUpdateManager.instance.UnregisterSteppedUpdate( typeof(AnimationTree), SteppedUpdate );
		}
	}
	
	void OnDestroy()
	{
		if ( SteppedUpdateManager.hasInstance )
		{
			SteppedUpdateManager.instance.UnregisterSteppedUpdate( typeof(AnimationTree), SteppedUpdate );
		}
	}
	
	void Start()
	{
		if ( _started )
			return;
		
		numFramesBetweenUpdates = numFramesBetweenUpdates;
		
		FillBonesList(_rootBone);
		_bones = _bonesList.ToArray();
		
		//UpdateAnimation(1);
		
		_started = true;
			
		
		/*foreach( AnimationState state in m_subject )
		{
			_preexistingAnimations.Add(state.clip);
		}*/
	}
	
	public void ResetControlVariables()
	{
		foreach (ControlVariable cv in ControlVariablesList)
		{
			cv.Reset();
		}
	}
	
	void FillBonesList( Transform parent )
	{
		_bonesList.Add(parent);
		
		int childCount = parent.childCount;
		for ( int i = 0; i < childCount; i++ )
		{
			Transform t = parent.GetChild(i);
			FillBonesList(t);
		}
	}
	
	void ApplyNewSubjectToChildAnimationNodes (AT_Node node)
	{
		AT_AnimationBase anim = node as AT_AnimationBase;
		AT_AnimationState state = node as AT_AnimationState;
		if (state != null)
		{
			anim = state.m_animation;
		}
		if (anim != null)
		{
			anim.m_subject = m_subject;
			anim.Init ();
		}
		
		AT_ContainerNode cont = node as AT_ContainerNode;
		if (cont != null)
		{
			foreach (AT_Node cNode in cont.Children)
			{
				ApplyNewSubjectToChildAnimationNodes (cNode);
			}
		}
	}
	
    // Remove a synced animation from the sync dictionary
    public void RemoveSyncedAnimation(int layer, AnimationState state)
    {
        if (!m_syncedAnimations.ContainsKey(layer))
        {
            return;
        }

        foreach (AnimationSyncData sData in m_syncedAnimations[layer])
        {
            if (sData.m_state == state)
            {
                m_syncedAnimations[layer].Remove(sData);
                break;
            }
        }

        // Calculate new speed factors for layer
        RecalculateSpeedVariablesForLayer(layer);
    }

    // Add a synced animation to the sync dictionary
    public void AddSyncedAnimation(int layer, AnimationState state, AT_Animation animNode )
    {
        if (state == null)
        {
            Debug.Log("Adding null animation state for syncing");
            return;
        }

        
        // If the layer doesn't exist in the dictionary yet, add it
        if (!m_syncedAnimations.ContainsKey(layer))
        {
            m_syncedAnimations.Add(layer, new List<AnimationSyncData>());
        }
        // Create the new AnimationSyncData object for this animation
        AnimationSyncData data = new AnimationSyncData();
        data.m_state = state;
		data.node = animNode;

        // Add in the new state to the sync layer
        m_syncedAnimations[layer].Add(data);

        // Calculate new speed factors for layer
        RecalculateSpeedVariablesForLayer(layer);
    }

    void RecalculateSpeedVariablesForLayer(int layer)
    {
        float averageLength = 0;
        int count = 0;
		//Debug.Log("recalculate");
        // Calculate the average length of all the animations in this layer
        foreach (AnimationSyncData sData in m_syncedAnimations[layer])
        {
            averageLength += sData.m_state.length;
			//Debug.Log(sData.m_state.length);
            count++;
        }
        averageLength /= count;
		//Debug.Log("average: " + averageLength + " => " + layer);
        // Recalculate speed variables for each animation based on average length
        foreach (AnimationSyncData sData in m_syncedAnimations[layer])
        {
            sData.m_desiredSpeedFactor = averageLength / sData.m_state.length;
            sData.m_normalisedSpeed = sData.m_state.length / averageLength;
			//Debug.Log("desired: " + sData.m_desiredSpeedFactor);
			//Debug.Log("normalized: " + sData.m_normalisedSpeed);
        }
    }

    // Sync up all the synced animations
    void SyncAnimations( float timeDelta )
    {
        // Loop through each layer
        foreach( KeyValuePair<int, List<AnimationSyncData>> kvp in m_syncedAnimations )
        {
            float speed = 0;
            float nTime = 0;
            int counter = 0;
            int index = 0;
			
			// Find number of additive animations in this sync layer
			int additiveCount = 0;
			foreach (AnimationSyncData sData in kvp.Value)
            {
				if ( sData.m_state.blendMode == AnimationBlendMode.Additive )
				{
					additiveCount++;
				}
			}
			
            // Calculate the speed factor
            foreach (AnimationSyncData sData in kvp.Value)
            {
				//TODO: Not sure how I should handle synced additives. doing it like this for now.
				//if ( sData.m_state.blendMode != AnimationBlendMode.Additive )
				//{
                	speed += sData.m_state.weight * sData.m_desiredSpeedFactor;
				//}
				//else
				//{
				//	speed += sData.m_desiredSpeedFactor / additiveCount;	
				//}
				
                // Store a normalised time from one of the currently enabled animations
                if (sData.m_state.enabled)
                {
                    nTime = sData.m_state.normalizedTime;
                    index = counter;
					counter++;
                }
                
            } 
			
			if (counter != 0)
			{
				//speed /= counter;
			}
			
			
			//Debug.Log("speed factor: " + speed + " - " + kvp.Key);
			//Debug.Log("sync");
            counter = 0;
            // Apply the new speed
            foreach (AnimationSyncData sData in kvp.Value)
            {
                sData.m_state.speed = sData.m_normalisedSpeed * speed * GameTime.timeScale;
				if ( sData.node.animSpeedControlled )
				{
					sData.m_state.speed *= sData.node.speedFactor;
				}
				//Debug.Log(sData.m_state.name + ": " + sData.m_state.speed);
                // Sync animations by adjusting normalised time
                if (counter != index)
                {
                    sData.m_state.normalizedTime = nTime;
					//Debug.Log(nTime);
                }
                counter++;
            }
        }
    }

	// Update is called once per frame
	void Update () 
	{
		if ( !_cleanedUpAnimations )
		{
			foreach ( AnimationClip clip in _preexistingAnimations )
			{
				m_subject.RemoveClip(clip);
			}
			_cleanedUpAnimations = true;
		}
		
		/*int activeAnims = 0;
		List<string> animNames = new List<string>();
		foreach ( AnimationState state in m_subject )
		{
			if ( state.enabled )
			{
				if ( activeAnims > 1 )
				{
					state.enabled = false;
				}
				
				activeAnims++;
				//animNames.Add(state.clip.name);
				//Debug.Log(state.clip.name);
			}
		}*/
		//Debug.Log("Active Animations: " + activeAnims);
		//Debug.Log("Animations: " + animNames.ToArray().ToString());
		
		/*if ( _renderer.isVisible )
		{
			UpdateTree();
			ApplyPoseData();
		}*/
	}
	
	public void RegisterPose( AT_Pose pose )
	{
		_poses.Add(pose);
	}
	
	public void DeregisterPose( AT_Pose pose )
	{
		_poses.Remove(pose);
	}
	
	void SteppedUpdate( float timeDelta )
	{
		
		if ( _renderer == null || (_renderer != null && _renderer.isVisible) )
		{
			UpdateAnimation(timeDelta);
		}
	}
	
	void UpdateAnimation( float timeDelta )
	{
		m_subject.enabled = false;
		
		UpdateTree( timeDelta );
			
		if ( sampleAnimationData )
		{
			// Get animation data onto model from animation component
			m_subject.Sample();
					
			if ( applyPoseData )
			{
				ApplyPoseData();
			}
		}
	}
	
	void LateUpdate()
	{
		if ( SteppedUpdateManager.hasInstance || (_renderer != null && !_renderer.isVisible) )
		{
			// Don't apply pose data if the object isn't visible because there is nothing resetting the 
			// model to a base pose, so the added pose offsets will make it go mental
			return;
		}
		
		UpdateAnimation(GameTime.deltaTime);
	}
	
	public void RegisterNode( AT_Node node )
	{
		_treeNodes.Add(node);
	}

	public void DeregisterNode( AT_Node node )
	{
		_treeNodes.Remove(node);
	}

	
	void UpdateTree( float timeDelta )
	{
		Profiler.BeginSample("Update Tree");
		m_root.SetParentBlendWeight( 1 );
        SyncAnimations( timeDelta );
		
		m_root.UpdateAnimation(timeDelta);
		
		foreach ( AT_Node node in _treeNodes )
		{
			node.UpdateAnimation( timeDelta );
		}
		Profiler.EndSample();
	}
	
	void ApplyPoseData()
	{
		if ( _bones == null )
		{
			return;
		}
		
		if ( _renderer != null && !_renderer.isVisible )
		{
			// Don't apply pose data if the object isn't visible because there is nothing resetting the 
			// model to a base pose, so the added pose offsets will make it go mental
			return;
		}
		
		Profiler.BeginSample("Apply Pose Data");
		
		int numPoses = 0;
		float totalWeight = 0;
		
		for ( int i = 0; i < _bones.Length; i++ )
		{
			Quaternion deltaRot = Quaternion.identity;
			Vector3 deltaPos = Vector3.zero;
			foreach(AT_Pose pose in _poses)
			{
				//Profiler.BeginSample("GetDeltaRot");
				deltaRot *= pose.DeltaRot[i];
				//Profiler.EndSample();
				//Profiler.BeginSample("GetDeltaPos");
				deltaPos += pose.DeltaPos[i];
				//Profiler.EndSample();
				
				if ( i == 0 )
				{
					if ( pose.currentWeight > 0 )
					{
						numPoses++;
						totalWeight += pose.currentWeight;
					}
				}
			}
			
			//Profiler.BeginSample("SetRot");
			_bones[i].localRotation *= deltaRot;
			//Profiler.EndSample();
			//Profiler.BeginSample("SetPos");
			_bones[i].localPosition += deltaPos;
			//Profiler.EndSample();
		}
		
		foreach(AT_Pose pose in _poses)
		{
			pose.totalPoseWeight = totalWeight;
		}
		
		Profiler.EndSample();
	}

    // Send passed requests to the root node
	public void SendRequest( string request )
	{
		if ( m_root != null )
		{
			m_root.SendRequest( request, true, false );
		}
	}
	
	public void SendExternalRequest( string request )
	{
		if ( sendRequestDelegate != null )
		{
			sendRequestDelegate(request);
		}
	}
}
