using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Вешается на Canvas.
public class GameUI : MonoBehaviour
{
    public static GameUI Instance { get; private set; }

    public GameObject messageText;
    public Image[] slotIcons;
    public GameObject backButton;
    public GameObject winPanel;
    public GameObject losePanel;
    public GameObject dangerOverlay;
    public GameObject hiddenOverlay;
    public RectTransform dangerBar;

    Coroutine messageRoutine;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        SetActive(winPanel, false);
        SetActive(losePanel, false);
        SetActive(hiddenOverlay, false);
        SetDanger(0f, false);
        RefreshInventory();
        RefreshBackButton();
    }

    // duration = 0 → сообщение висит, пока его не заменят
    public void ShowMessage(string text, float duration = 3f)
    {
        if (messageText == null) return;
        if (messageRoutine != null) StopCoroutine(messageRoutine);
        messageText.SetActive(true);
        UIText.Set(messageText, text);
        messageRoutine = duration > 0f ? StartCoroutine(ClearAfter(duration)) : null;
    }

    IEnumerator ClearAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        UIText.Set(messageText, "");
        messageRoutine = null;
    }

    public void RefreshInventory()
    {
        if (slotIcons == null) return;
        var inv = InventoryManager.Instance;

        for (int i = 0; i < slotIcons.Length; i++)
        {
            if (slotIcons[i] == null) continue;
            InventoryItem item = inv != null ? inv.GetItem(i) : null;
            slotIcons[i].enabled = item != null;
            if (item != null)
            {
                slotIcons[i].sprite = item.icon;   // без картинки будет белый квадрат
                slotIcons[i].preserveAspect = true;
            }
        }
    }

    // ---------- Кнопки (On Click) ----------

    public void OnSlotPressed(int index)
    {
        var inv = InventoryManager.Instance;
        InventoryItem item = inv != null ? inv.GetItem(index) : null;
        if (item == null) return;
        AudioManager.Play(Sfx.Click);
        ShowMessage(item.description, 4f);
    }

    public void OnBackPressed()
    {
        AudioManager.Play(Sfx.Click);
        if (CodeLock.Instance != null && CodeLock.Instance.IsOpen) CodeLock.Instance.Close();
        else ZoomView.CloseCurrent();
        RefreshBackButton();
    }

    public void OnRestartPressed()
    {
        if (GameManager.Instance != null) GameManager.Instance.Restart();
    }

    // ---------- Вызывается из других скриптов ----------

    public void RefreshBackButton()
    {
        bool over = GameManager.Instance != null && GameManager.Instance.IsGameOver;
        bool keypad = CodeLock.Instance != null && CodeLock.Instance.IsOpen;
        SetActive(backButton, !over && (ZoomView.Current != null || keypad));
    }

    public void SetHidden(bool hidden)
    {
        SetActive(hiddenOverlay, hidden);
    }

    public void SetDanger(float progress, bool visible)
    {
        SetActive(dangerOverlay, visible);
        if (dangerBar != null)
        {
            SetActive(dangerBar.gameObject, visible);
            dangerBar.localScale = new Vector3(Mathf.Clamp01(progress), 1f, 1f);
        }
    }

    public void ShowWin()
    {
        EndScreen();
        SetActive(winPanel, true);
    }

    public void ShowLose()
    {
        EndScreen();
        SetActive(losePanel, true);
    }

    void EndScreen()
    {
        SetActive(backButton, false);
        SetActive(hiddenOverlay, false);
        SetDanger(0f, false);
    }

    static void SetActive(GameObject go, bool value)
    {
        if (go != null && go.activeSelf != value) go.SetActive(value);
    }
}
