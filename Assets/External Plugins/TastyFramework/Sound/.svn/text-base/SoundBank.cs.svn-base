using UnityEngine;
using System.Collections;

public class SoundBank : MonoBehaviour 
{
    public AudioClip[] m_sounds;

    public AudioClip GetRandomClip()
    {
        if ( m_sounds.Length > 0 )
        {
            int ind = Random.Range( 0, m_sounds.Length );
            return m_sounds[ind];
        }

        return null;
    }

    public AudioClip GetClip( int index )
    {
        if ( index < m_sounds.Length )
        {
            return m_sounds[index];
        }
        return null;
    }
} 