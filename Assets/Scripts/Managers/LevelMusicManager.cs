using UnityEngine;

public class LevelMusicPlayer : MonoBehaviour
{
    
    public AudioClip levelMusic;

   
    [Range(0f, 1f)]
    public float volume = 0.5f;

    private AudioSource musicSource;

    private void Awake()
    {
        
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.clip = levelMusic;
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.volume = volume;

        if (levelMusic != null)
            musicSource.Play();
    }

   
    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
        if (musicSource != null)
            musicSource.volume = volume;
    }

   
    public void ChangeMusic(AudioClip newClip, bool playImmediately = true)
    {
        if (musicSource == null) return;

        musicSource.Stop();
        musicSource.clip = newClip;

        if (playImmediately && newClip != null)
            musicSource.Play();
    }
}