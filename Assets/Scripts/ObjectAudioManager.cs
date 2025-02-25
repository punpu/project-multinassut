using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;
using FishNet.Object.Synchronizing;
using System.Runtime.InteropServices;
public class ObjectAudioManager : NetworkBehaviour 
{
    public AudioClip walking;
    private Dictionary<string, AudioClip> sfxDict = new Dictionary<string, AudioClip>();
    private void Awake()
    {
        sfxDict.Add("walking", walking);
    }
    public void PlayObjectSfx(string sfxName)
    {
        var audioSource = gameObject.GetComponentInParent<AudioSource>();
        if(audioSource.clip == sfxDict[sfxName] && audioSource.isPlaying)
        {
            return;
        }
            audioSource.clip = sfxDict[sfxName];
            audioSource.loop = true;
            audioSource.Play();
    }
    public void StopObjectSfx(string sfxName)
    {
        var audioSource = gameObject.GetComponentInParent<AudioSource>();
        if(audioSource.clip == sfxDict[sfxName] && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}
