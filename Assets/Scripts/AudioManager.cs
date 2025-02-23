using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;
using FishNet.Object.Synchronizing;
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
           //musicSource.Play();
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

    public void PlayContinuosSfx(string sfxName, Vector3 location)
    {
        Debug.Log("Setting continuos sound");
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
        Debug.Log("Sending continuos sound");
        playContinuosSfx(sfxName, location);
    }

    [ObserversRpc]
    private void playContinuosSfx(string sfxName, Vector3 location)
    {
        Debug.Log("playing continuos sound");
        if (sfxSource != null)
        {
            Debug.Log("we have source");
            if (sfxDict.ContainsKey(sfxName))
            {
                Debug.Log("we play continuos!");
                sfxSource.clip = sfxDict[sfxName];
                sfxSource.loop = true;
                sfxSource.Play();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendEnvSound(string sfxName, Vector3 location)
    {
        Debug.Log("Playing sound");
        playEnvSfx(sfxName, location);
    }

    [ObserversRpc]
    private void playEnvSfx(string sfxName, Vector3 location)
    {
        Debug.Log("Playing sound over network " + sfxName);
        if (sfxDict.ContainsKey(sfxName))
        {
            Debug.Log("AUDIOOOO");
            AudioClip clip = sfxDict[sfxName];
            AudioSource.PlayClipAtPoint(clip, location);
        }
        else 
        {
            Debug.Log("NOU AUDIOOOO");
        }
    }
}
