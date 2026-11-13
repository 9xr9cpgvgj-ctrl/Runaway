using UnityEngine;

// Вибрация телефона. В редакторе Unity ничего не делает.
public static class Haptics
{
    const string Key = "vibration_on";

    public static bool Enabled
    {
        get { return PlayerPrefs.GetInt(Key, 1) == 1; }
        set { PlayerPrefs.SetInt(Key, value ? 1 : 0); PlayerPrefs.Save(); }
    }

    public static void Vibrate()
    {
        if (!Enabled) return;
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
        Handheld.Vibrate();
#endif
    }
}
