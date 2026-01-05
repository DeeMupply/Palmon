using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip musicClipMainGame;
    [SerializeField] private AudioClip scanCompleteClip;
    [SerializeField] private AudioClip toolUseClip;
    [SerializeField] private AudioClip buttonClip;
    [SerializeField] private AudioClip playerHurtClip;
    [SerializeField] private AudioClip flameOxClip;
    [SerializeField] private AudioClip duckClip;
    [SerializeField] private AudioClip mantisClip;

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
        PlayMusic();
    }

    private void PlayMusic()
    {
        if (musicSource != null && musicClipMainGame != null)
        {
            musicSource.clip = musicClipMainGame;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void PlayScanComplete()
    {
        if (sfxSource != null && scanCompleteClip != null)
        {
            sfxSource.PlayOneShot(scanCompleteClip);
        }
    }

    public void PlayToolUse()
    {
        if (sfxSource != null && toolUseClip != null)
        {
            sfxSource.PlayOneShot(toolUseClip);
        }
    }

    public void PlayButton()
    {
        if (sfxSource != null && buttonClip != null)
        {
            sfxSource.PlayOneShot(buttonClip);
        }
    }

    public void PlayPlayerHurt()
    {
        if (sfxSource != null && playerHurtClip != null)
        {
            sfxSource.PlayOneShot(playerHurtClip);
        }
    }

    public void PlayFlameOx()
    {
        if (sfxSource != null && flameOxClip != null)
        {
            sfxSource.PlayOneShot(flameOxClip);
        }
    }

    public void PlayDuck()
    {
        if (sfxSource != null && duckClip != null)
        {
            sfxSource.PlayOneShot(duckClip);
        }
    }

    public void PlayMantis()
    {
        if (sfxSource != null && mantisClip != null)
        {
            sfxSource.PlayOneShot(mantisClip);
        }
    }

    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    public void SetMusicVolume(float volume)
    {
        if (musicSource != null)
        {
            musicSource.volume = Mathf.Clamp01(volume);
        }
    }

    public void SetSFXVolume(float volume)
    {
        if (sfxSource != null)
        {
            sfxSource.volume = Mathf.Clamp01(volume);
        }
    }
}