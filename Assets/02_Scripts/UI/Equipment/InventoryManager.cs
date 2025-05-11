using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InventoryManager : ISaveable
{
    private List<ItemSO> items = new List<ItemSO>();

    public List<ItemSO> GetItems() => items;
    
    public void AddItem(ItemSO item)
    {
        if (items.Contains(item))
        {
            int index = items.IndexOf(item);
            if (items[index].itemType == ItemType.Stuff)
                items[index].quantityOrLevel += item.quantityOrLevel;
            else
                items.Add(item);
        }
        else
        {
            items.Add(item);
        }
        
        List<int> itemList = new List<int>();
        foreach (ItemSO tem in items)
        {
            itemList.Add(tem.itemCode);
        }

        Managers.BackendManager.UpdateField("inventoryItems", items.Select(i => i.itemCode).ToList());
    }
    
    public void RemoveItem(ItemSO item)
    {
        if (items.Contains(item))
        {
            int index = items.IndexOf(item);
            if (items[index].itemType == ItemType.Stuff)
            {
                items[index].quantityOrLevel -= item.quantityOrLevel;
                if (items[index].quantityOrLevel <= 0)
                    items.RemoveAt(index);
            }
            else
                items.RemoveAt(index);
        }
    }

    [ContextMenu("Clear Inventory")]
    public void ClearInventory()
    {
        items.Clear(); // 아이템 목록 비우기
    }

    public void Save(GameData data)
    {
    }

    public void Load(GameData data)
    {
        items.Clear();
        int index = 0;
        foreach (int itemCode in data.inventoryItems)
        {
            ItemSO item = ItemDatabase.Instance.GetItemById(itemCode);
            if (item != null)
            {
                item.quantityOrLevel = 1;
                items.Add(item);
            }
        }
    }
}

