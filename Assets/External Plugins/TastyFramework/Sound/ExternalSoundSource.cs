using UnityEngine;
using System.Collections;

public class ExternalSoundSource : MonoBehaviour 
{
    void Start()
    {
        SoundManager.instance.RegisterExternalSource(audio);
    }

    public virtual void OnDestroy()
    {
        if (SoundManager.hasInstance)
        {
            SoundManager.instance.DeregisterExternalSource(audio);
        }
    }
} 