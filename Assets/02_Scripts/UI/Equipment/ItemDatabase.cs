using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoSingleton<ItemDatabase>
{
    public List<ItemSO> allItems; // 모든 아이템 리스트

    public ItemSO GetItemByName(string name)
    {
        return allItems.Find(item => item.itemName == name);
    }
}