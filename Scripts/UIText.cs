using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Пишет текст и в TextMeshPro, и в обычный (Legacy) Text.
// Можно передать сам текстовый объект или его родителя.
public static class UIText
{
    public static void Set(GameObject target, string value)
    {
        if (target == null) return;

        var tmp = target.GetComponentInChildren<TMP_Text>(true);
        if (tmp != null) { tmp.text = value; return; }

        var legacy = target.GetComponentInChildren<Text>(true);
        if (legacy != null) legacy.text = value;
    }
}
