using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Загадка с банками. Вешается на родителя крупного плана (ZoomView_Shelf).
// Предметы загадки — объекты с Interactable, Action = Press Sequence.
//
// Режимы:
//   PressInOrder — нажать предметы в правильном порядке;
//   Arrange      — переставить предметы на полке: тап по одной банке (она приподнимется),
//                  тап по другой — они меняются местами. Когда слева направо порядок верный,
//                  загадка решена.
public class SequencePuzzle : MonoBehaviour
{
    public enum Mode { PressInOrder, Arrange }
    public Mode mode = Mode.Arrange;

    [Tooltip("Правильный порядок (Sequence Id), например Flour, Sugar, Salt. В режиме Arrange — слева направо")]
    public string[] correctOrder = { "Flour", "Sugar", "Salt" };

    [Tooltip("Что включится, когда загадка решена (тайник, ключ, записка)")]
    public GameObject[] activateOnSolve;
    [Tooltip("Что выключится, когда загадка решена")]
    public GameObject[] deactivateOnSolve;

    [Tooltip("Ошибка шумит — похититель придёт раньше (только режим PressInOrder)")]
    public bool noiseOnMistake = true;

    [Header("Режим Arrange")]
    [Tooltip("На сколько приподнимается выбранная банка")]
    public float liftHeight = 0.4f;

    [TextArea] public string solvedMessage = "Click! A hidden compartment opens behind the jars.";
    [TextArea] public string mistakeMessage = "Wrong order... the jars rattle loudly.";
    [TextArea] public string arrangeHint = "Tap two jars to swap them.";

    public bool Solved { get; private set; }

    int step;
    Transform selected;
    Vector3 selectedStartPos;
    readonly List<Interactable> items = new List<Interactable>();

    void OnEnable()
    {
        if (!Solved && mode == Mode.Arrange && GameUI.Instance != null)
            GameUI.Instance.ShowMessage(arrangeHint, 2.5f);
    }

    void OnDisable()
    {
        Deselect();
    }

    public void Press(string id, Transform item)
    {
        if (Solved) return;
        id = id == null ? "" : id.Trim();
        if (mode == Mode.PressInOrder) PressInOrder(id);
        else Arrange(item);
    }

    // ---------- PressInOrder ----------
    void PressInOrder(string id)
    {
        if (step < correctOrder.Length && id == correctOrder[step].Trim())
        {
            step++;
            AudioManager.Play(Sfx.Click);
            if (GameUI.Instance != null)
                GameUI.Instance.ShowMessage(id + " (" + step + "/" + correctOrder.Length + ")", 1.5f);
            if (step >= correctOrder.Length) Solve();
        }
        else
        {
            step = 0;
            AudioManager.Play(Sfx.Wrong);
            if (GameUI.Instance != null) GameUI.Instance.ShowMessage(mistakeMessage, 2f);
            if (noiseOnMistake) DangerTimer.MakeNoise();
        }
    }

    // ---------- Arrange (перестановка) ----------
    void Arrange(Transform item)
    {
        if (item == null) return;
        AudioManager.Play(Sfx.Click);

        if (selected == null)
        {
            selected = item;
            selectedStartPos = item.localPosition;
            item.localPosition = selectedStartPos + Vector3.up * liftHeight;
            return;
        }

        if (selected == item)
        {
            Deselect();
            return;
        }

        // меняем местами
        Vector3 other = item.localPosition;
        item.localPosition = selectedStartPos;
        selected.localPosition = new Vector3(other.x, selectedStartPos.y, other.z);
        selected = null;

        if (IsCorrect()) Solve();
    }

    void Deselect()
    {
        if (selected != null) selected.localPosition = selectedStartPos;
        selected = null;
    }

    bool IsCorrect()
    {
        items.Clear();
        items.AddRange(GetComponentsInChildren<Interactable>()
            .Where(i => i.action == InteractAction.PressSequence && i.sequencePuzzle == this));
        var ordered = items.OrderBy(i => i.transform.localPosition.x)
                           .Select(i => (i.sequenceId ?? "").Trim()).ToArray();
        if (ordered.Length != correctOrder.Length) return false;
        for (int k = 0; k < ordered.Length; k++)
            if (ordered[k] != correctOrder[k].Trim()) return false;
        return true;
    }

    void Solve()
    {
        Solved = true;
        AudioManager.Play(Sfx.Pickup);
        foreach (var go in activateOnSolve) if (go != null) go.SetActive(true);
        foreach (var go in deactivateOnSolve) if (go != null) go.SetActive(false);
        if (GameUI.Instance != null) GameUI.Instance.ShowMessage(solvedMessage, 3f);
    }
}
