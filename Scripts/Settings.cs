using UnityEngine;

// Настройки звука. Сохраняются на телефоне между запусками.
public static class Settings
{
    const string SoundKey = "sound_on";

    public static bool SoundOn
    {
        get { return PlayerPrefs.GetInt(SoundKey, 1) == 1; }
        set
        {
            PlayerPrefs.SetInt(SoundKey, value ? 1 : 0);
            PlayerPrefs.Save();
            AudioListener.volume = value ? 1f : 0f;
        }
    }

    // Применяем сохранённую настройку при запуске игры
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void ApplyOnStart()
    {
        AudioListener.volume = SoundOn ? 1f : 0f;
    }
}
