using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Volumes")]
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    [Header("Toggles")]
    public bool musicEnabled = true;
    public bool sfxEnabled = true;

    private float defaultSfxPitch = 1f;

    [Header("SFX Clips")]
    public AudioClip hit;
    public AudioClip swing;
    public AudioClip takeDamage;
    public AudioClip consumableSpawn;
    public AudioClip bomb;
    public AudioClip superBomb;
    public AudioClip drill;
    public AudioClip saw;
    public AudioClip launch;
    public AudioClip robotHit;
    public AudioClip pickUp;
    public AudioClip boom;
    public AudioClip bombTimer;
    public AudioClip shield;
    public AudioClip heal;
    public AudioClip laser;
    public AudioClip death;

    [Header("UI")]
    public AudioClip select;
    public AudioClip confirm;

    [Header("Music")]
    public AudioClip level1Music;
    public AudioClip level2Music;

   
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
        }

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }

        
        musicSource.ignoreListenerPause = true;
        sfxSource.ignoreListenerPause = true;

        defaultSfxPitch = sfxSource.pitch;

        ApplySettings();
    }

  
    public void PlaySFX(AudioClip clip)
    {
        if (!sfxEnabled || clip == null || sfxSource == null)
            return;

        if (!sfxSource.enabled)
            sfxSource.enabled = true;

        sfxSource.pitch = Random.Range(0.9f, 1.1f);
        sfxSource.PlayOneShot(clip, sfxVolume);
        sfxSource.pitch = defaultSfxPitch;
    }

    public void StopSFX()
    {
        if (sfxSource != null)
            sfxSource.Stop();
    }

    
    public void PlayMusic(AudioClip clip)
    {
        if (!musicEnabled || clip == null || musicSource == null)
            return;

        if (musicSource.clip == clip && musicSource.isPlaying)
            return;

        musicSource.clip = clip;
        musicSource.volume = musicVolume;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource != null)
            musicSource.Stop();
    }


    public void SetMusicVolume(float value)
    {
        musicVolume = value;
        ApplySettings();
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = value;
        ApplySettings();
    }

    public void ToggleMusic(bool value)
    {
        musicEnabled = value;

        if (!musicEnabled)
            StopMusic();
        else if (musicSource.clip != null)
            musicSource.Play();
    }

    public void ToggleSFX(bool value)
    {
        sfxEnabled = value;

        if (!sfxEnabled)
            StopSFX();
    }

    private void ApplySettings()
    {
        if (musicSource != null)
            musicSource.volume = musicEnabled ? musicVolume : 0f;

        if (sfxSource != null)
            sfxSource.volume = sfxEnabled ? sfxVolume : 0f;
    }
}