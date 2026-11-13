using UnityEngine;

// Вешается на родителя крупного плана (ZoomView_Painting, ZoomView_Desk).
public class ZoomView : MonoBehaviour
{
    public static ZoomView Current { get; private set; }

    void Awake()
    {
        // Если забыли выключить объект в сцене, он выключится сам при старте
        if (Current != this) gameObject.SetActive(false);
    }

    public void Open()
    {
        if (Current != null && Current != this) Current.Close();
        Current = this;
        gameObject.SetActive(true);
        if (GameUI.Instance != null) GameUI.Instance.RefreshBackButton();
    }

    public void Close()
    {
        if (Current == this) Current = null;
        gameObject.SetActive(false);
        if (GameUI.Instance != null) GameUI.Instance.RefreshBackButton();
    }

    public static void CloseCurrent()
    {
        if (Current != null) Current.Close();
    }

    void OnDisable()
    {
        if (Current == this) Current = null;
    }
}
