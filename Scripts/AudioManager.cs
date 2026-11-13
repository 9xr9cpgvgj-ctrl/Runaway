using UnityEngine;

public enum Sfx { Click, Pickup, Locked, Wrong, Footsteps, Door, Hide, Win, Lose }

// Вешается на GameSystems (в игре) и на Canvas (в меню).
// Любое поле можно оставить пустым: игра работает и без звука.
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Фоновая музыка (играет по кругу)")]
    public AudioClip music;
    [Range(0f, 1f)] public float musicVolume = 0.4f;

    [Header("Звуки")]
    [Range(0f, 1f)] public float sfxVolume = 1f;
    public AudioClip click;
    public AudioClip pickup;
    public AudioClip locked;
    public AudioClip wrong;
    public AudioClip footsteps;
    public AudioClip door;
    public AudioClip hide;
    public AudioClip win;
    public AudioClip lose;

    AudioSource sfxSource;
    AudioSource musicSource;

    void Awake()
    {
        Instance = this;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = true;
    }

    void Start()
    {
        if (music != null)
        {
            musicSource.clip = music;
            musicSource.volume = musicVolume;
            musicSource.Play();
        }
    }

    public static void Play(Sfx sfx)
    {
        if (Instance == null) return;
        AudioClip clip = Instance.GetClip(sfx);
        if (clip != null) Instance.sfxSource.PlayOneShot(clip, Instance.sfxVolume);
    }

    public static void StopMusic()
    {
        if (Instance != null) Instance.musicSource.Stop();
    }

    AudioClip GetClip(Sfx sfx)
    {
        switch (sfx)
        {
            case Sfx.Click: return click;
            case Sfx.Pickup: return pickup;
            case Sfx.Locked: return locked;
            case Sfx.Wrong: return wrong;
            case Sfx.Footsteps: return footsteps;
            case Sfx.Door: return door;
            case Sfx.Hide: return hide;
            case Sfx.Win: return win;
            case Sfx.Lose: return lose;
            default: return null;
        }
    }
}
