using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase instance;
    public List<ItemSO> allItems; // 모든 아이템 리스트

    private void Awake()
    {
        instance = this;
    }

    public ItemSO GetItemByName(string name)
    {
        return allItems.Find(item => item.itemName == name);
    }
}