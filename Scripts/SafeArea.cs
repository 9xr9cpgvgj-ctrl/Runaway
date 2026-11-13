using UnityEngine;

// Вешается на объект SafeArea внутри Canvas.
// Подгоняет его под «безопасную» часть экрана телефона:
// без выреза камеры (чёлки), закруглённых углов и системных панелей.
// Всё, что лежит внутри SafeArea, не залезет под вырез.
[RequireComponent(typeof(RectTransform))]
public class SafeArea : MonoBehaviour
{
    RectTransform rect;
    Rect lastSafeArea;
    Vector2Int lastScreen;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        Apply();
    }

    void Update()
    {
        // Телефон могли повернуть, поэтому проверяем каждый кадр (это дёшево)
        if (Screen.safeArea != lastSafeArea || Screen.width != lastScreen.x || Screen.height != lastScreen.y)
            Apply();
    }

    void Apply()
    {
        Rect safe = Screen.safeArea;
        lastSafeArea = safe;
        lastScreen = new Vector2Int(Screen.width, Screen.height);
        if (Screen.width <= 0 || Screen.height <= 0) return;

        Vector2 min = safe.position;
        Vector2 max = safe.position + safe.size;
        min.x /= Screen.width;  min.y /= Screen.height;
        max.x /= Screen.width;  max.y /= Screen.height;

        rect.anchorMin = min;
        rect.anchorMax = max;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
