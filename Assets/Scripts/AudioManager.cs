using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;
using FishNet.Object.Synchronizing;
public class AudioManager : NetworkBehaviour {
    
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioClip mainBackgroundMusic;
    public AudioClip reukkuShot;
    public AudioClip hngh;

    public static AudioManager instance;

    private Dictionary<string, AudioClip> sfxDict = new Dictionary<string, AudioClip>();
    private void Awake()
    {
        sfxDict.Add("hngh", hngh);
        sfxDict.Add("reukku-shot", reukkuShot); 
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayMusic(mainBackgroundMusic);
    }

    public void PlayMusic(AudioClip musicClip) 
    {
        if (musicSource != null && musicClip != null)
        {
            musicSource.clip = musicClip;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void StopMusic()
    {
        if(musicSource != null)
        { 
            musicSource.Stop();
        }
    }

    public void ChangeTrack(AudioClip clip)
    {
        musicSource.clip = clip; 
    }

    public void PlaySfx_old(AudioClip sfxClip) 
    {
        if(sfxSource != null && sfxClip != null) 
        {
            sfxSource.PlayOneShot(sfxClip);
        }
    }

    public void test()
    {
        
    }

    public void PlaySfx(string sfxName, Vector3 location)
    {
        Debug.Log("Playing sound");
        SendEnvSound(sfxName, location);
    }   

    [ServerRpc(RequireOwnership = false)]
    private void SendEnvSound(string sfxName, Vector3 location) 
    {
        Debug.Log("Playing sound");
        playEnvSfx(sfxName, location);
    }

    [ObserversRpc]
    //[ObserversRpc(ExcludeOwner = true)]
    private void playEnvSfx(string sfxName, Vector3 location)
    {
        Debug.Log("Playing sound over network");
        if(sfxDict.ContainsKey(sfxName))
        {
            AudioClip clip = sfxDict[sfxName];
            AudioSource.PlayClipAtPoint(clip, location);
        }
    }
}
