using UnityEngine;
using UnityEngine.SceneManagement;

// В сцене меню: вешается на Canvas.
// В игровой сцене: вешается на GameSystems (для кнопки «Menu»).
public class MainMenu : MonoBehaviour
{
    [Tooltip("Сцена, которая запускается кнопкой Start Game")]
    public string firstLevelScene = "SampleScene";

    [Tooltip("Точное имя сцены главного меню")]
    public string menuSceneName = "MainMenu";

    [Tooltip("Панель со списком уровней (только в сцене меню)")]
    public GameObject levelsPanel;

    // Если true, меню откроется сразу с панелью уровней (после последнего уровня)
    public static bool OpenLevelsOnStart;

    void Start()
    {
        Time.timeScale = 1f;
        if (levelsPanel != null)
        {
            levelsPanel.SetActive(OpenLevelsOnStart);
            OpenLevelsOnStart = false;
        }
    }

    // Кнопка Start Game
    public void StartGame()
    {
        AudioManager.Play(Sfx.Click);
        SceneManager.LoadScene(firstLevelScene);
    }

    // Кнопка Levels
    public void OpenLevels()
    {
        AudioManager.Play(Sfx.Click);
        if (levelsPanel != null) levelsPanel.SetActive(true);
    }

    // Кнопка Back внутри панели уровней
    public void CloseLevels()
    {
        AudioManager.Play(Sfx.Click);
        if (levelsPanel != null) levelsPanel.SetActive(false);
    }

    // Кнопки уровней: в On Click впишите имя сцены уровня
    public void LoadLevel(string sceneName)
    {
        AudioManager.Play(Sfx.Click);
        SceneManager.LoadScene(sceneName);
    }

    // Кнопка Menu в игре
    public void OpenMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }

    // Кнопка Quit Game
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;   // в редакторе просто останавливает Play
#else
        Application.Quit();
#endif
    }
}
