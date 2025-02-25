using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;
using FishNet.Object.Synchronizing;
using System.Runtime.InteropServices;
public class AudioManager : NetworkBehaviour
{

    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioClip mainBackgroundMusic;
    public AudioClip hngh;
    public AudioClip reukkuShot;
    public AudioClip auh;
    public AudioClip hitWall;
    public AudioClip hit;
    public AudioClip walking;
    public AudioClip morso;
    public AudioClip miss;

    public static AudioManager instance;

    private Dictionary<string, AudioClip> sfxDict = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> musicDict = new Dictionary<string, AudioClip>();
    private void Awake()
    {
        sfxDict.Add("hngh", hngh);
        sfxDict.Add("auh", auh);
        sfxDict.Add("reukku-shot", reukkuShot);
        sfxDict.Add("hit", hit);
        sfxDict.Add("hit-wall", hitWall);
        sfxDict.Add("morso", morso);
        sfxDict.Add("walking", walking);
        sfxDict.Add("miss", miss); 

        musicDict.Add("mainBgMusic", mainBackgroundMusic);
    }
    
    public void PlayMusic(string musicName)
    {
        if (musicSource != null)
        {
            if (musicDict.ContainsKey(musicName))
            {
                musicSource.clip = musicDict[musicName];
                musicSource.loop = true;
                musicSource.Play();
            }
        }
    }

    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    public void ChangeTrack(AudioClip clip)
    {
        musicSource.clip = clip;
    }

    public void PlaySfx(string sfxName, Vector3 location)
    {
        Debug.Log("Playing sound");
        SendEnvSound(sfxName, location);
    }

    public void PlayObjectSfx(string sfxName, NetworkObject sourceObject)
    {
        SendObjectSfx(sfxName, sourceObject);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendObjectSfx(string sfxName, NetworkObject sourceObject) {
        playObjectSfx(sfxName, sourceObject);
    }

    [ObserversRpc]
    private void playObjectSfx(string sfxName, NetworkObject sourceObject)
    {
        var audioSource = sourceObject.GetComponentInChildren<AudioSource>();
        if(audioSource.clip == sfxDict[sfxName] && audioSource.isPlaying)
        {
            return;
        }
            audioSource.clip = sfxDict[sfxName];
            audioSource.loop = true;
            audioSource.Play();
    }

    public void StopObjectSfx(string sfxName, GameObject sourceObject)
    {
        var audioSource = sourceObject.GetComponentInChildren<AudioSource>();
        if(audioSource.clip == sfxDict[sfxName] && audioSource.isPlaying)
        {
           audioSource.Stop();
        }
    }

    public void PlayContinuosSfx(string sfxName, Vector3 location)
    {
        SendContinuosSfx(sfxName, location);
    }

    public void PlayLocalSfx(string sfxName)
    {
        if (sfxDict.ContainsKey(sfxName))
        {
            AudioClip clip = sfxDict[sfxName];
            sfxSource.PlayOneShot(clip);
        }     
    }
    [ServerRpc(RequireOwnership = false)]
    private void SendContinuosSfx(string sfxName, Vector3 location) {
        playContinuosSfx(sfxName, location);
    }

    [ObserversRpc]
    private void playContinuosSfx(string sfxName, Vector3 location)
    {
        if (sfxSource != null)
        {
            if (sfxDict.ContainsKey(sfxName))
            {
                sfxSource.clip = sfxDict[sfxName];
                sfxSource.loop = true;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendEnvSound(string sfxName, Vector3 location)
    {
        playEnvSfx(sfxName, location);
    }

    [ObserversRpc]
    private void playEnvSfx(string sfxName, Vector3 location)
    {
        if (sfxDict.ContainsKey(sfxName))
        {
            AudioClip clip = sfxDict[sfxName];
            AudioSource.PlayClipAtPoint(clip, location);
        }
    }
}
