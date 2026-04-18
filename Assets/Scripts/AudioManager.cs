using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("Audio Clips")]
    public AudioClip bgm;
    public AudioClip clickSound;
    public AudioClip blockSound;
    public AudioClip winSound;
    public AudioClip buttonSound;

    public void PlayButton()
    {
        if (sfxSource != null && buttonSound != null)
            sfxSource.PlayOneShot(buttonSound);
    }
    void Awake()
    {
   
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        PlayBGM();
    }


    public void PlayBGM()
    {
        if (bgmSource != null && bgm != null)
        {
            bgmSource.clip = bgm;
            bgmSource.loop = true;
            bgmSource.volume = 0.3f;
            bgmSource.Play();
        }
    }

    public void PlayClick()
    {
        if (sfxSource != null && clickSound != null)
            sfxSource.PlayOneShot(clickSound);
    }

    public void PlayBlock()
    {
        if (sfxSource != null && blockSound != null)
            sfxSource.PlayOneShot(blockSound);
    }

    public void PlayWin()
    {
        if (sfxSource != null && winSound != null)
            sfxSource.PlayOneShot(winSound);
    }
}