using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    public string id;
    public Sprite icon;
    public string description;
}

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    public int maxSlots = 4;
    public List<InventoryItem> items = new List<InventoryItem>();

    void Awake()
    {
        Instance = this;
    }

    public bool AddItem(string id, Sprite icon, string description)
    {
        if (HasItem(id)) return false;

        if (items.Count >= maxSlots)
        {
            if (GameUI.Instance != null) GameUI.Instance.ShowMessage("My pockets are full.");
            return false;
        }

        items.Add(new InventoryItem { id = id, icon = icon, description = description });
        Debug.Log("[Inventory] Подобран предмет '" + id + "'");
        if (GameUI.Instance != null) GameUI.Instance.RefreshInventory();
        return true;
    }

    public bool HasItem(string id)
    {
        return items.Exists(i => i.id == id);
    }

    public InventoryItem GetItem(int index)
    {
        if (index < 0 || index >= items.Count) return null;
        return items[index];
    }
}
