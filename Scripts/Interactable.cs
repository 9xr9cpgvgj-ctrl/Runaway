using System.Collections;
using UnityEngine;

public enum InteractAction { ShowMessage, OpenZoom, GiveItem, Hide, OpenCodeLock, PressSequence }

// Вешается на всё, по чему можно тапнуть в мире (нужен Box Collider 2D).
[RequireComponent(typeof(Collider2D))]
public class Interactable : MonoBehaviour
{
    public InteractAction action = InteractAction.ShowMessage;

    [Header("Общее")]
    [TextArea] public string message;

    [Header("OpenZoom")]
    public ZoomView zoomTarget;

    [Header("GiveItem")]
    public string itemId;
    public Sprite itemIcon;
    [TextArea] public string itemDescription;
    public bool disableAfterUse = true;

    [Header("PressSequence (загадка с порядком)")]
    public SequencePuzzle sequencePuzzle;
    public string sequenceId;

    [Header("Шум")]
    [Tooltip("Тап по объекту шумит — похититель придёт раньше")]
    public bool makeNoise = false;

    [Header("Нужен предмет (для ЛЮБОГО действия, можно оставить пустым)")]
    public string requiredItemId;
    [TextArea] public string lockedMessage = "The door is locked. I need a key.";

    [Header("Отклик на тап")]
    [Tooltip("Объект коротко «пульсирует», когда по нему тапают")]
    public bool pulseOnTap = true;

    Vector3 baseScale;
    Coroutine pulse;

    void Awake()
    {
        baseScale = transform.localScale;
    }

    IEnumerator Pulse()
    {
        const float time = 0.18f;
        float t = 0f;
        while (t < time)
        {
            t += Time.unscaledDeltaTime;
            float k = 1f + 0.08f * Mathf.Sin(Mathf.PI * Mathf.Clamp01(t / time));
            transform.localScale = baseScale * k;
            yield return null;
        }
        transform.localScale = baseScale;
        pulse = null;
    }

    void OnDisable()
    {
        transform.localScale = baseScale == Vector3.zero ? transform.localScale : baseScale;
    }

    public void Interact()
    {
        Debug.Log("[Interactable] Клик по " + name + ", Action = " + action);

        if (pulseOnTap && gameObject.activeInHierarchy)
        {
            if (pulse != null) StopCoroutine(pulse);
            pulse = StartCoroutine(Pulse());
        }

        // Если для действия нужен предмет, а его нет в инвентаре — показываем подсказку
        string need = requiredItemId == null ? "" : requiredItemId.Trim();
        if (need.Length > 0)
        {
            bool hasItem = InventoryManager.Instance != null && InventoryManager.Instance.HasItem(need);
            if (!hasItem)
            {
                AudioManager.Play(Sfx.Locked);
                Say(lockedMessage);
                return;
            }
        }

        if (makeNoise)
        {
            AudioManager.Play(Sfx.Wrong);
            DangerTimer.MakeNoise();
        }

        switch (action)
        {
            case InteractAction.ShowMessage:
                Say(message);
                break;

            case InteractAction.OpenZoom:
                if (zoomTarget == null)
                {
                    Debug.LogWarning(name + ": поле Zoom Target не заполнено");
                    return;
                }
                AudioManager.Play(Sfx.Click);
                zoomTarget.Open();
                Say(message);
                break;

            case InteractAction.GiveItem:
                GiveItem();
                break;

            case InteractAction.Hide:
                if (GameManager.Instance != null)
                    GameManager.Instance.Hide(string.IsNullOrEmpty(message)
                        ? "You are hiding. Tap anywhere to come out."
                        : message);
                break;

            case InteractAction.PressSequence:
                if (sequencePuzzle != null) sequencePuzzle.Press(sequenceId, transform);
                else Debug.LogWarning(name + ": поле Sequence Puzzle не заполнено");
                break;

            case InteractAction.OpenCodeLock:
                if (CodeLock.Instance != null) CodeLock.Instance.Open();
                else Debug.LogWarning("[Door] На GameSystems нет компонента CodeLock!");
                Say(message);
                break;
        }
    }

    void GiveItem()
    {
        var inv = InventoryManager.Instance;
        if (inv == null) return;

        string id = itemId == null ? "" : itemId.Trim();
        if (inv.AddItem(id, itemIcon, itemDescription))
        {
            AudioManager.Play(Sfx.Pickup);
            Say(string.IsNullOrEmpty(message) ? "Picked up: " + itemDescription : message);
            if (disableAfterUse) gameObject.SetActive(false);
        }
    }

    void Say(string text)
    {
        if (!string.IsNullOrEmpty(text) && GameUI.Instance != null)
            GameUI.Instance.ShowMessage(text);
    }
}
