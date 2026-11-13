using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Главное меню. Вешается на пустой объект в сцене MainMenu.
/// Кнопка «Play» вызывает Play().
/// </summary>
public class MenuController : MonoBehaviour
{
    [SerializeField] string gameSceneName = "Room1";

    public void Play()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void Quit()
    {
        Application.Quit();
    }
}