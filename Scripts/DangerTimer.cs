using System.Collections;
using UnityEngine;

// Периодически приходит похититель. Если игрок не спрятался — проигрыш.
// Шум (неверный код, грохот посуды, ошибка в загадке) заставляет его прийти раньше.
public class DangerTimer : MonoBehaviour
{
    public static DangerTimer Instance { get; private set; }

    public GameObject kidnapper;

    [Header("Время в секундах")]
    public float firstDelay = 20f;     // до первого прихода
    public float repeatDelay = 30f;    // между приходами
    public float warningTime = 6f;     // сколько длится предупреждение
    public float kidnapperStay = 3f;   // сколько он стоит в комнате

    [Header("Шум")]
    [Tooltip("После шума похититель придёт не позже чем через столько секунд")]
    public float noiseDelay = 3f;
    [TextArea] public string noiseMessage = "That was loud... Someone heard you!";

    [Header("Тексты")]
    [TextArea] public string warningMessage = "Footsteps! Someone is coming... Hide!";
    [TextArea] public string leftMessage = "He's gone. That was close...";

    float waitLeft;
    bool coming;   // идёт предупреждение или похититель уже в комнате

    bool Over => GameManager.Instance == null || GameManager.Instance.IsGameOver;
    GameUI UI => GameUI.Instance;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (kidnapper != null) kidnapper.SetActive(false);
        StartCoroutine(Loop());
    }

    // Вызывается при любом шуме
    public static void MakeNoise()
    {
        if (Instance == null || Instance.Over || Instance.coming) return;
        if (Instance.waitLeft > Instance.noiseDelay)
        {
            Instance.waitLeft = Instance.noiseDelay;
            Haptics.Vibrate();
            if (Instance.UI != null) Instance.UI.ShowMessage(Instance.noiseMessage, 2.5f);
        }
    }

    IEnumerator Loop()
    {
        waitLeft = firstDelay;

        while (!Over)
        {
            // Ожидание (на паузе стоит, шум может сократить)
            coming = false;
            while (waitLeft > 0f)
            {
                if (Over) yield break;
                waitLeft -= Time.deltaTime;
                yield return null;
            }
            coming = true;

            // 1. Предупреждение: шаги, красный экран, полоска растёт
            AudioManager.Play(Sfx.Footsteps);
            Haptics.Vibrate();
            if (UI != null) UI.ShowMessage(warningMessage, warningTime);

            float t = 0f;
            while (t < warningTime)
            {
                if (Over) { if (UI != null) UI.SetDanger(0f, false); yield break; }
                t += Time.deltaTime;
                if (UI != null) UI.SetDanger(t / warningTime, true);
                yield return null;
            }
            if (UI != null) UI.SetDanger(0f, false);

            // 2. Похититель в комнате
            AudioManager.Play(Sfx.Door);
            if (kidnapper != null) kidnapper.SetActive(true);

            float s = 0f;
            while (s < kidnapperStay)
            {
                if (Over) yield break;
                if (!GameManager.Instance.IsHidden)
                {
                    GameManager.Instance.Lose();
                    yield break;
                }
                s += Time.deltaTime;
                yield return null;
            }

            // 3. Ушёл
            if (kidnapper != null) kidnapper.SetActive(false);
            if (UI != null) UI.ShowMessage(leftMessage, 3f);
            waitLeft = repeatDelay;
        }
    }
}
