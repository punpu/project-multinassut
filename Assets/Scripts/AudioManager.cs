using UnityEngine;

public class AudioManager : MonoBehaviour
{
     //[Heaader("AudioSources")] 
    public AudioSource musicSource;
    public AudioSource sfxSource;

    //[Heaader("AudioClips")] 
    public AudioClip mainBackgroundMusic;

    public static AudioManager instance;

    private void Awake() 
    {
        if (instance == null) 
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }    
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

    public void PlaySfx(AudioClip audioClip) 
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
