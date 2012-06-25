using UnityEngine;
using System.Collections;
using Framework;
using System.Collections.Generic;

public class AudioChannel
{
	public AudioSource m_source;
	public float m_volume;
}

public class SoundManager : SingletonBehaviour<SoundManager> 
{
    class FadeInfo
    {
        public AudioSource m_source;
        public float m_targetVolume;
        public float m_fadeSpeed;
		public bool m_freeOnCompletion;
    }
		
    public int m_numberOfChannels = 32;
    public GameObject m_channelTemplate;
	public AudioClip m_buttonClickSound;

    List<AudioChannel> m_freeChannels = new List<AudioChannel>();
    List<AudioChannel> m_usedChannels = new List<AudioChannel>();
    List<AudioChannel> m_channelsToFree = new List<AudioChannel>();
    List<AudioChannel> m_musicChannels = new List<AudioChannel>();
    List<AudioChannel> m_externalSources = new List<AudioChannel>();
    List<FadeInfo> m_fadingChannels = new List<FadeInfo>();
    List<FadeInfo> m_fadingChannelsToFree = new List<FadeInfo>();
	
	
	float m_baseMusicLevel = 1.0f;
	float m_baseSoundLevel = 1.0f;
	
	public float MusicLevel
	{
		get { return m_baseMusicLevel; }
		set
		{
			m_baseMusicLevel = value;
			SetNewMusicLevel(value);
		}
	}
	
	public float SoundLevel 
	{
		get { return m_baseSoundLevel; }
		set 
		{ 
			m_baseSoundLevel = value; 
			SetNewSoundLevel(value);
		}
	}
	
	public void SetNewMusicLevel( float level )
	{
		foreach ( AudioChannel channel in m_musicChannels )
		{
			channel.m_source.volume = channel.m_volume * m_baseMusicLevel;
		}
	}
	
	public void SetNewSoundLevel( float level )
	{
		foreach ( AudioChannel channel in m_usedChannels )
		{
			channel.m_source.volume = channel.m_volume * m_baseSoundLevel;
		}
		
		foreach ( AudioChannel channel in m_externalSources )
		{
			channel.m_source.volume = channel.m_volume * m_baseSoundLevel;
		}
	}
	
    // Use this for initialization 	
    public override void Awake ()      
    {
        if (SoundManager.hasInstance)
        {
            return;
        }

        base.Awake();

        GameObject channelsParent = new GameObject();
        channelsParent.name = "Channels";
        channelsParent.transform.parent = transform;
        channelsParent.transform.position = Vector3.zero;

        for ( int i = 0; i < m_numberOfChannels; i++ )
        {
            GameObject newChannel = GameObject.Instantiate( m_channelTemplate ) as GameObject;
            newChannel.transform.parent = channelsParent.transform;
            newChannel.transform.position = Vector3.zero;
            newChannel.name = "AudioChannel " + ( i + 1 );

            AudioChannel chan = new AudioChannel();
			chan.m_source = newChannel.audio;
			
			m_freeChannels.Add( chan );
			
			newChannel.active = false;
        }
    }

    public void RegisterExternalSource(AudioSource source)
    {
		AudioChannel channel = new AudioChannel();
		channel.m_source = source;
		channel.m_volume = source.volume;
		channel.m_source.volume = channel.m_volume * m_baseSoundLevel;
        m_externalSources.Add(channel);
    }

    public void DeregisterExternalSource(AudioSource source)
    {
		AudioChannel channelToRemove = null;
		foreach ( AudioChannel channel in m_externalSources )
		{
			if ( channel.m_source == source )
			{
				channelToRemove = channel;
			}
		}
		
		if ( channelToRemove != null )
		{
        	m_externalSources.Remove(channelToRemove);
		}
		
		FadeInfo infoToRemove = null;
		foreach( FadeInfo info in m_fadingChannels)
		{
			if ( info.m_source == source )
			{
				infoToRemove = info;
			}
		}
		
		if ( infoToRemove != null )
		{
			m_fadingChannels.Remove(infoToRemove);
		}
    }
    
    // Update is called once per frame 	
    void Update ()      
    {
        m_channelsToFree.Clear();
        foreach( AudioChannel channel in m_usedChannels )
        {
            if ( !channel.m_source.isPlaying )
            {
                m_channelsToFree.Add( channel );
            }
        }

        foreach ( AudioChannel channel in m_channelsToFree )
        {
            m_usedChannels.Remove( channel );
            m_freeChannels.Add( channel );
			channel.m_source.gameObject.active = false;
        }

        // Update any fading channels
        m_fadingChannelsToFree.Clear();
        foreach (FadeInfo info in m_fadingChannels)
        {
			if (info.m_source.volume != info.m_targetVolume)
            {
                float delta = info.m_fadeSpeed;
                if (info.m_targetVolume - info.m_source.volume < 0)
                {
                    delta *= -1;
                }

                info.m_source.volume += delta * NonStopTime.deltaTime;

                if ((delta > 0 && info.m_source.volume >= info.m_targetVolume)
                    || (delta < 0 && info.m_source.volume <= info.m_targetVolume))
                {
                    info.m_source.volume = info.m_targetVolume;
                    m_fadingChannelsToFree.Add(info);
                }
            }
            else
            {
                m_fadingChannelsToFree.Add(info);
            }
        }
        // Remove fading channels that are finished from queue
        foreach (FadeInfo info in m_fadingChannelsToFree)
        {
            if ( info.m_freeOnCompletion )
			{
				info.m_source.volume = 0;
				AudioChannel channel = FindChannelFromSource( m_usedChannels, info.m_source );
				if ( channel != null )
				{
					m_usedChannels.Remove(channel);
					channel.m_volume = 0;
					channel.m_source.volume = 0;
					channel.m_source.gameObject.SetActiveRecursively(false);
					m_freeChannels.Add(channel);
				}
				
				channel = FindChannelFromSource( m_musicChannels, info.m_source );
				if ( channel != null )
				{
					m_musicChannels.Remove(channel);
					channel.m_volume = 0;
					channel.m_source.volume = 0;
					channel.m_source.gameObject.SetActiveRecursively(false);
					m_freeChannels.Add(channel);
				}
			}
			
			m_fadingChannels.Remove(info);
			
        }
        
    }

    AudioChannel GetFreeChannel()
    {
        AudioChannel channel = null;
        if ( m_freeChannels.Count > 0 )
        {
            channel = m_freeChannels[0];

            m_usedChannels.Add( channel );
            m_freeChannels.Remove( channel );
        }

        return channel;
    }
	
	public void StopAllSounds( bool stopExternal )
	{
		foreach( AudioChannel channel in m_usedChannels )
        {
        	channel.m_source.Stop();
        }
		
		if ( stopExternal )
		{
			foreach ( AudioChannel channel in m_externalSources )
			{
				channel.m_source.Stop();
			}
		}
	}

    public void MuteAllSounds()
    {
        foreach (AudioChannel channel in m_usedChannels)
        {
            FadeChannel(channel, 0, 4);
        }		
		
		for ( int i = 0; i < m_externalSources.Count; i++ )
		{
			m_externalSources[i].m_volume = m_externalSources[i].m_source.volume;
			FadeChannel(m_externalSources[i], 0, 4);
		}		
    }
	
	public void UnmuteAllSounds()
	{
		foreach (AudioChannel channel in m_externalSources )
        {
            FadeChannel(channel, channel.m_volume, 4);
        }
	}
	
	AudioChannel FindChannelFromSource( List<AudioChannel> list, AudioSource source )
	{
		AudioChannel channelFound = null;
		foreach ( AudioChannel channel in list )
		{
			if ( channel.m_source == source )
			{
				channelFound = channel;
			}
		}
		
		return channelFound;
	}
	
    public void PlayRandomAt( SoundBank bank, Vector3 pos, float volume )
    {
        AudioClip clip = bank.GetRandomClip();
        PlaySoundAt( clip, pos, volume );
    }

    public void PlayAt( SoundBank bank, int index, Vector3 pos, float volume )
    {
        AudioClip clip = bank.GetClip( index );
        PlaySoundAt( clip, pos, volume );
    }

    public void PlaySoundAt( AudioClip clip, Vector3 pos, float volume )
    {
        AudioChannel channel = GetFreeChannel();
        if ( channel != null )
        {
			channel.m_source.gameObject.active = true;
			
            channel.m_source.transform.position = pos;
            channel.m_source.loop = false;
            //channel.PlayOneShot( clip );

            channel.m_source.volume = volume * SoundLevel;
            channel.m_source.clip = clip;
            channel.m_source.Play();
			channel.m_volume = volume;
        }
    }

    public AudioChannel PlayMusic(AudioClip clip, float startVolume, float endVolume, float fadeSpeed)
    {
        AudioChannel channel = GetFreeChannel();
        if (channel != null)
        {
            m_usedChannels.Remove(channel);
			
            m_musicChannels.Add(channel);

            channel.m_source.gameObject.active = true;
            channel.m_source.loop = true;
            channel.m_source.volume = startVolume * MusicLevel;
            channel.m_source.clip = clip;

            channel.m_source.Play();
			channel.m_volume = endVolume;

            FadeInfo info = new FadeInfo();
            info.m_source = channel.m_source;
            info.m_targetVolume = endVolume * MusicLevel;
            info.m_fadeSpeed = fadeSpeed;

            m_fadingChannels.Add(info);
        }
        return channel;
    }
	
	void FadeSource( AudioSource source, float endVolume, float fadeSpeed)
	{
		foreach (FadeInfo info in m_fadingChannels)
        {
            if (info.m_source == source )
            {
                info.m_targetVolume = endVolume;
                info.m_fadeSpeed = fadeSpeed;
                return;
            }
        }

        FadeInfo finfo = new FadeInfo();
        finfo.m_source = source;
        finfo.m_targetVolume = endVolume * MusicLevel;
        finfo.m_fadeSpeed = fadeSpeed;

        m_fadingChannels.Add(finfo);
	}

    FadeInfo FadeChannel( AudioChannel source, float endVolume, float fadeSpeed)
    {
        foreach (FadeInfo info in m_fadingChannels)
        {
            if (info.m_source == source.m_source)
            {
                info.m_targetVolume = endVolume;
                info.m_fadeSpeed = fadeSpeed;
                return info;
            }
        }

        FadeInfo finfo = new FadeInfo();
        finfo.m_source = source.m_source;
        finfo.m_targetVolume = endVolume * MusicLevel;
        finfo.m_fadeSpeed = fadeSpeed;
		finfo.m_freeOnCompletion = false;
		
        m_fadingChannels.Add(finfo);
		
		return finfo;
    }

    public void FadeMusic( AudioChannel source, float endVolume, float fadeSpeed )
    {
        FadeInfo finfo = FadeChannel(source, endVolume, fadeSpeed);
		
		finfo.m_freeOnCompletion = true;
    }
	
	public void PlayButtonClickSound()
	{
		PlaySoundAt( m_buttonClickSound, Vector3.zero, 1 );
	}
} 