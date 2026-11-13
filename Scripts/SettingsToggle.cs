using UnityEngine;
using UnityEngine.UI;

// Вешается на кнопку «Sound» или «Vibration» (в меню и в паузе).
// On Click настраивать НЕ нужно: кнопка сама переключает настройку и меняет надпись.
[RequireComponent(typeof(Button))]
public class SettingsToggle : MonoBehaviour
{
    public enum Kind { Sound, Vibration }
    public Kind kind = Kind.Sound;

    [Tooltip("Текст на кнопке (если пусто, найдётся сам внутри кнопки)")]
    public GameObject label;

    void Awake()
    {
        if (label == null && transform.childCount > 0) label = transform.GetChild(0).gameObject;
        GetComponent<Button>().onClick.AddListener(Toggle);
    }

    void OnEnable()
    {
        Refresh();
    }

    void Toggle()
    {
        if (kind == Kind.Sound) Settings.SoundOn = !Settings.SoundOn;
        else
        {
            Haptics.Enabled = !Haptics.Enabled;
            Haptics.Vibrate();   // короткая проверка, что вибрация включилась
        }
        AudioManager.Play(Sfx.Click);
        Refresh();
    }

    void Refresh()
    {
        bool on = kind == Kind.Sound ? Settings.SoundOn : Haptics.Enabled;
        string name = kind == Kind.Sound ? "Sound" : "Vibration";
        UIText.Set(label, name + ": " + (on ? "ON" : "OFF"));
    }
}
