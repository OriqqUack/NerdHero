using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;
    public List<ItemSO> items = new List<ItemSO>(); // ScriptableObject 아이템
    public Transform inventoryPanel;
    public GameObject inventorySlotPrefab;
    [SerializeField] private Button sortButton;
    [SerializeField] private Button equipButton;

    private string savePath;

    private ItemSO currentItem;
    
    private void Awake()
    {
        instance = this;
        UpdateInventoryUI();
        
        sortButton.onClick.AddListener(() => SortItem());
        equipButton.onClick.AddListener(() => EquipItem(currentItem));
    }

    public void AddItem(ItemSO item)
    {
        items.Add(item);
        SaveInventory();
        UpdateInventoryUI();
    }

    public void RemoveItem(ItemSO item)
    {
        items.Remove(item);
        SaveInventory();
        UpdateInventoryUI();
    }

    public void UpdateInventoryUI()
    {
        foreach (Transform child in inventoryPanel) Destroy(child.gameObject);

        foreach (ItemSO item in items)
        {
            GameObject slotObj = Instantiate(inventorySlotPrefab, inventoryPanel);
            ItemSlot slot = slotObj.GetComponent<ItemSlot>();
            slot.SetItem(item);
        }
    }

    public void EquipItem(ItemSO item)
    {
        Equipment.instance.Equip(item);
        SaveInventory();
        UpdateInventoryUI();
    }

    public void SaveInventory()
    {
        /*SaveData data = new SaveData();
        
        foreach (ItemSO item in items)
        {
            data.inventoryItems.Add(item.itemName); // 아이템 이름 저장
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);*/
    }

    public void LoadInventory()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            items.Clear();

            foreach (string itemName in data.inventoryItems)
            {
                ItemSO item = ItemDatabase.instance.GetItemByName(itemName);
                if (item != null)
                {
                    items.Add(item);
                }
            }

            Equipment.instance.LoadEquipment(data);
        }
        UpdateInventoryUI();
    }

    public void OnClickItem(ItemSO item)
    {
        currentItem = item;
    }

    private void SortItem()
    {
        
    }
}
