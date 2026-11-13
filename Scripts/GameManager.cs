using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Уровень")]
    public int levelNumber = 1;
    [Tooltip("Сцена следующего уровня. Если пусто или имя неверное — берётся следующая сцена из Build Profiles")]
    public string nextLevelScene = "";
    [Tooltip("Через сколько секунд после победы сам начнётся следующий уровень")]
    public float autoNextDelay = 4f;
    public string menuSceneName = "MainMenu";

    [Header("Экран победы (необязательно)")]
    [Tooltip("Текст на WinPanel для обратного отсчёта / «Все уровни пройдены»")]
    public GameObject winInfoText;
    [Tooltip("Кнопка Next Level (показывается, если есть следующий уровень)")]
    public GameObject nextLevelButton;
    [Tooltip("Кнопка Levels (показывается после последнего уровня)")]
    public GameObject levelsButton;

    [Header("Тексты")]
    [TextArea] public string startMessage = "You wake up in a strange room. The door is locked...";
    [TextArea] public string unhideMessage = "You quietly step out.";
    [TextArea] public string allLevelsDoneText = "All levels complete!";

    public bool IsHidden { get; private set; }
    public bool IsGameOver { get; private set; }

    public bool KeypadOpen => CodeLock.Instance != null && CodeLock.Instance.IsOpen;

    bool loading;

    void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;
    }

    void Start()
    {
        if (nextLevelButton != null) nextLevelButton.SetActive(false);
        if (levelsButton != null) levelsButton.SetActive(false);
        UIText.Set(winInfoText, "");
        if (GameUI.Instance != null) GameUI.Instance.ShowMessage(startMessage, 5f);
    }

    public void Hide(string message)
    {
        if (IsGameOver || IsHidden) return;
        ZoomView.CloseCurrent();
        IsHidden = true;
        AudioManager.Play(Sfx.Hide);
        if (GameUI.Instance != null)
        {
            GameUI.Instance.SetHidden(true);
            GameUI.Instance.ShowMessage(message, 0f);
        }
    }

    public void Unhide()
    {
        if (!IsHidden || IsGameOver) return;
        IsHidden = false;
        AudioManager.Play(Sfx.Hide);
        if (GameUI.Instance != null)
        {
            GameUI.Instance.SetHidden(false);
            GameUI.Instance.ShowMessage(unhideMessage, 2f);
        }
    }

    public void Win()
    {
        if (IsGameOver) return;
        IsGameOver = true;
        ZoomView.CloseCurrent();
        LevelProgress.CompleteLevel(levelNumber);
        AudioManager.StopMusic();
        AudioManager.Play(Sfx.Win);
        if (GameUI.Instance != null) GameUI.Instance.ShowWin();

        string next = ResolveNextScene();
        bool hasNext = next != "";
        Debug.Log("[GameManager] Level " + levelNumber + " won. Next scene: " + (hasNext ? next : "(none, last level)"));
        if (nextLevelButton != null) nextLevelButton.SetActive(hasNext);
        if (levelsButton != null) levelsButton.SetActive(!hasNext);

        if (hasNext) StartCoroutine(AutoNextLevel());
        else UIText.Set(winInfoText, allLevelsDoneText);
    }

    IEnumerator AutoNextLevel()
    {
        float t = autoNextDelay;
        while (t > 0f && !loading)
        {
            UIText.Set(winInfoText, "Next level in " + Mathf.CeilToInt(t) + "...");
            t -= Time.unscaledDeltaTime;
            yield return null;
        }
        NextLevel();
    }

    public void Lose()
    {
        if (IsGameOver) return;
        IsGameOver = true;
        ZoomView.CloseCurrent();
        if (CodeLock.Instance != null) CodeLock.Instance.Close();
        AudioManager.StopMusic();
        AudioManager.Play(Sfx.Lose);
        Haptics.Vibrate();
        if (GameUI.Instance != null) GameUI.Instance.ShowLose();
    }

    public void Restart()
    {
        if (loading) return;
        loading = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Кнопка Next Level (или автоматически после отсчёта)
    public void NextLevel()
    {
        if (loading) return;
        string next = ResolveNextScene();
        if (next == "")
        {
            Debug.LogWarning("[GameManager] No next level to load.");
            return;
        }
        loading = true;
        Time.timeScale = 1f;
        Debug.Log("[GameManager] Loading next level: " + next);
        SceneManager.LoadScene(next);
    }

    // Находит следующий уровень:
    // 1) имя из поля Next Level Scene (лишние пробелы и ; убираются),
    // 2) если имя пустое или неверное — следующая сцена по списку Build Profiles.
    string ResolveNextScene()
    {
        string s = (nextLevelScene ?? "").Trim().TrimEnd(';').Trim();
        if (s != "")
        {
            if (Application.CanStreamedLevelBeLoaded(s)) return s;
            Debug.LogWarning("[GameManager] Scene '" + s + "' is not in Build Profiles. Using the next scene in the build list.");
        }

        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextIndex <= 0 || nextIndex >= SceneManager.sceneCountInBuildSettings) return "";

        string path = SceneUtility.GetScenePathByBuildIndex(nextIndex);
        string name = System.IO.Path.GetFileNameWithoutExtension(path);
        if (name == menuSceneName) return "";
        return name;
    }

    // Кнопка Levels после последнего уровня: меню сразу с открытой сеткой уровней
    public void OpenLevelSelect()
    {
        if (loading) return;
        loading = true;
        MainMenu.OpenLevelsOnStart = true;
        SceneManager.LoadScene(menuSceneName);
    }
}