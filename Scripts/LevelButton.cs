using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Вешается на каждую кнопку уровня (Level1Button, Level2Button...).
// Сама решает, открыт уровень или закрыт, и сама загружает сцену.
// On Click у кнопки настраивать НЕ нужно.
[RequireComponent(typeof(Button))]
public class LevelButton : MonoBehaviour
{
    public int levelNumber = 1;
    public string sceneName = "SampleScene";
    [Tooltip("Значок замка (необязательно): показывается, пока уровень закрыт")]
    public GameObject lockIcon;

    Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(Load);
    }

    void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (button == null) button = GetComponent<Button>();
        bool open = LevelProgress.IsUnlocked(levelNumber) && !string.IsNullOrEmpty(sceneName);
        button.interactable = open;
        if (lockIcon != null) lockIcon.SetActive(!open);
    }

    void Load()
    {
        if (!button.interactable) return;
        AudioManager.Play(Sfx.Click);
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }
}
