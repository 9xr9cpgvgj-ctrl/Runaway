using UnityEngine;

public class CodeLock : MonoBehaviour
{
    public static CodeLock Instance { get; private set; }

    public GameObject panel;     // KeypadPanel
    public GameObject display;   // CodeText
    public string correctCode = "442";
    [TextArea] public string wrongMessage = "Wrong code.";
    [Tooltip("Неверный код шумит — похититель придёт раньше")]
    public bool noiseOnWrong = false;

    string input = "";
    float lastPressTime = -1f;

    // На телефоне и в эмуляторе одно касание иногда приходит дважды.
    // Нажатия чаще, чем раз в 0.2 секунды, считаем одним.
    bool TooFast()
    {
        if (Time.unscaledTime - lastPressTime < 0.2f) return true;
        lastPressTime = Time.unscaledTime;
        return false;
    }

    public bool IsOpen => panel != null && panel.activeSelf;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Close();
    }

    public void Open()
    {
        if (panel == null)
        {
            Debug.LogWarning("CodeLock: поле Panel не заполнено");
            return;
        }
        ZoomView.CloseCurrent();
        input = "";
        UpdateDisplay();
        panel.SetActive(true);
        Debug.Log("[CodeLock] Панель открыта: " + panel.name + ", активна в иерархии: " + panel.activeInHierarchy);
        if (GameUI.Instance != null) GameUI.Instance.RefreshBackButton();
    }

    public void Close()
    {
        if (panel != null) panel.SetActive(false);
        input = "";
        if (GameUI.Instance != null) GameUI.Instance.RefreshBackButton();
    }

    public void PressDigit(int digit)
    {
        if (!IsOpen || input.Length >= correctCode.Length) return;
        if (TooFast()) return;
        input += digit.ToString();
        AudioManager.Play(Sfx.Click);
        UpdateDisplay();
    }

    public void Clear()
    {
        if (TooFast()) return;
        input = "";
        AudioManager.Play(Sfx.Click);
        UpdateDisplay();
    }

    public void Submit()
    {
        if (TooFast()) return;
        if (input == correctCode)
        {
            Close();
            if (GameManager.Instance != null) GameManager.Instance.Win();
        }
        else
        {
            AudioManager.Play(Sfx.Wrong);
            Haptics.Vibrate();
            if (noiseOnWrong) DangerTimer.MakeNoise();
            if (GameUI.Instance != null) GameUI.Instance.ShowMessage(wrongMessage, 2f);
            input = "";
            UpdateDisplay();
        }
    }

    void UpdateDisplay()
    {
        string s = "";
        for (int i = 0; i < correctCode.Length; i++)
        {
            s += i < input.Length ? input[i].ToString() : "_";
            if (i < correctCode.Length - 1) s += " ";
        }
        UIText.Set(display, s);
    }
}
