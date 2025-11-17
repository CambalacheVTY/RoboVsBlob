using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

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

    public AudioClip select;
    public AudioClip Confirm;

    public AudioClip level1Music;
    public AudioClip level2Music;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }


    /* el playsfx original
     public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
            sfxSource.PlayOneShot(clip);
    }
    */

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
         
            GameObject tempGO = new GameObject("SFX_" + clip.name);
            AudioSource aSource = tempGO.AddComponent<AudioSource>();
            aSource.clip = clip;
            aSource.pitch = Random.Range(0.8f, 1.2f); 
            aSource.Play();
            Destroy(tempGO, clip.length / aSource.pitch); 
        }
    }



    public void PlayMusic(AudioClip clip)
    {
        if (clip != null)
        {
            musicSource.clip = clip;
            musicSource.Play();
        }
    }

    public void StopMusic()
    {
        if (musicSource != null)
            musicSource.Stop();
    }

    public void StopSFX()
    {
        if (sfxSource != null)
            sfxSource.Stop();
    }
}