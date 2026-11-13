using UnityEngine;

// Запоминает, какие уровни открыты. Сохраняется на телефоне между запусками.
public static class LevelProgress
{
    const string Key = "unlocked_level";

    // Самый большой открытый уровень (уровень 1 открыт всегда)
    public static int MaxUnlocked
    {
        get { return Mathf.Max(1, PlayerPrefs.GetInt(Key, 1)); }
    }

    public static bool IsUnlocked(int level)
    {
        return level <= MaxUnlocked;
    }

    // Вызывается при победе на уровне: открывает следующий
    public static void CompleteLevel(int level)
    {
        if (level + 1 > MaxUnlocked)
        {
            PlayerPrefs.SetInt(Key, level + 1);
            PlayerPrefs.Save();
        }
    }

    public static void ResetProgress()
    {
        PlayerPrefs.DeleteKey(Key);
        PlayerPrefs.Save();
    }
}
