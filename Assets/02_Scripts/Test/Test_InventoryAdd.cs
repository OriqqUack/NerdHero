using System;
using UnityEngine;

public class Test_InventoryAdd : MonoBehaviour
{
    public UI_Inventory inventory;

    public void AddItem(ItemSO item)
    {
        Managers.InventoryManager.AddItem(item);
        inventory.UpdatUI();
    }
}
