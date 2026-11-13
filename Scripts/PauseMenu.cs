using UnityEngine;
using UnityEngine.SceneManagement;

// Меню паузы внутри игры. Вешается на GameSystems.
// Кнопка Menu открывает панель, игра замирает (таймер похитителя тоже стоит).
public class PauseMenu : MonoBehaviour
{
    public static bool IsPaused { get; private set; }

    public GameObject pausePanel;              // PausePanel
    public string menuSceneName = "MainMenu";

    void Start()
    {
        SetPaused(false);
    }

    // Кнопка Menu (в правом верхнем углу)
    public void OpenPause()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;
        AudioManager.Play(Sfx.Click);
        SetPaused(true);
    }

    // Кнопка Continue
    public void Continue()
    {
        SetPaused(false);
        AudioManager.Play(Sfx.Click);
    }

    // Кнопка Main Menu
    public void GoToMainMenu()
    {
        SetPaused(false);
        SceneManager.LoadScene(menuSceneName);
    }

    // Кнопка Quit Game (необязательная)
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void Update()
    {
        // Кнопка/жест «Назад» на Android: открывает или закрывает паузу
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPaused) Continue();
            else OpenPause();
        }
    }

    // Игрок свернул игру или ему позвонили: ставим на паузу,
    // чтобы похититель не пришёл, пока игра в фоне
    void OnApplicationPause(bool appPaused)
    {
        if (appPaused && !IsPaused) OpenPause();
    }

    void SetPaused(bool paused)
    {
        IsPaused = paused;
        Time.timeScale = paused ? 0f : 1f;
        AudioListener.pause = paused;          // музыка и звуки тоже на паузе
        if (pausePanel != null) pausePanel.SetActive(paused);
    }

    void OnDestroy()
    {
        // при смене сцены всё возвращаем в норму
        IsPaused = false;
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }
}
