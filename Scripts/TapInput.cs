using UnityEngine;
using UnityEngine.EventSystems;

// Вешается на Main Camera. Ловит тапы/клики по объектам в мире.
[RequireComponent(typeof(Camera))]
public class TapInput : MonoBehaviour
{
    Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        Vector2 screenPos;
        int pointerId = -1;

        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase != TouchPhase.Began) return;
            screenPos = t.position;
            pointerId = t.fingerId;
        }
        else if (Input.GetMouseButtonDown(0))
        {
            screenPos = Input.mousePosition;
        }
        else return;

        // Тап по кнопке интерфейса не должен попадать в мир
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(pointerId)) return;

        HandleTap(cam.ScreenToWorldPoint(screenPos));
    }

    void HandleTap(Vector2 worldPos)
    {
        var gm = GameManager.Instance;
        if (gm == null || gm.IsGameOver || gm.KeypadOpen || PauseMenu.IsPaused) return;

        // Когда спрятались, любой тап = выйти из укрытия
        if (gm.IsHidden)
        {
            gm.Unhide();
            return;
        }

        Collider2D[] hits = Physics2D.OverlapPointAll(worldPos);
        Interactable best = null;
        int bestOrder = int.MinValue;

        foreach (var hit in hits)
        {
            var it = hit.GetComponent<Interactable>();
            if (it == null) continue;

            // Пока открыт зум, работают только предметы внутри него
            if (ZoomView.Current != null && !hit.transform.IsChildOf(ZoomView.Current.transform)) continue;

            var sr = hit.GetComponent<SpriteRenderer>();
            int order = sr != null ? sr.sortingOrder : 0;
            if (order > bestOrder)
            {
                bestOrder = order;
                best = it;
            }
        }

        if (best != null) best.Interact();
    }
}
