using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }
    
    [SerializeField] private List<ItemSO> items = new List<ItemSO>();
    [SerializeField] private List<Skill> skillList = new List<Skill>();
    private List<Skill> _filteredSkillList = new List<Skill>();
    private string savePath;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        savePath = Application.persistentDataPath + "/Inventory.json";
    }

    private void Start()
    {
        LoadInventory();
        SceneManager.sceneLoaded += LoadScene;
    }

    private void OnApplicationQuit()
    {
        SaveInventory();
    }

    public List<ItemSO> GetItems() => new List<ItemSO>(items);

    public void SetupSkills()
    {
        _filteredSkillList.Clear();
        foreach (Skill skill in skillList)
        {
            var cloneSkill = skill.Clone() as Skill;
            cloneSkill.Setup(GameManager.Instance.GetPlayerEntity());
            _filteredSkillList.Add(cloneSkill);
        }
    }
    
    public List<Skill> GetSkills()
    { 
        return _filteredSkillList;
    }
    
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
    
    public void SaveInventory()
    {
        SaveData data = new SaveData();
        Equipment equipment = Equipment.Instance;

        foreach (ItemSO item in items)
        {
            data.inventoryItems.Add(item.itemName);
            data.quantityOrLevel.Add(item.quantityOrLevel);
        }
        
        if(equipment)
        {
            if(equipment.weapon)
            {
                data.equippedWeapon = equipment.weapon.itemName;
                data.weaponLevel = equipment.weapon.quantityOrLevel;
            }
            if(equipment.armor)
            {
                data.equippedArmor = equipment.armor.itemName;
                data.armorLevel = equipment.armor.quantityOrLevel;
            }
            if(equipment.helmet)
            {
                data.equippedHelmet = equipment.helmet.itemName;
                data.helmetLevel = equipment.helmet.quantityOrLevel;
            }
            if(equipment.boots)
            {
                data.equippedBoots = equipment.boots.itemName;
                data.bootsLevel = equipment.boots.quantityOrLevel;
            }
        }
        
        foreach (Skill skill in skillList)
        {
            data.skills.Add(skill.CodeName);
            data.skillLevels.Add(skill.Level);
        }
        
        if(equipment?.skill1)
            data.equippedSkill1 = equipment.skill1.CodeName;
        if(equipment?.skill2)
            data.equippedSkill2 = equipment.skill2.CodeName;
        
        File.WriteAllText(savePath, JsonUtility.ToJson(data, true));
    }

    private void LoadScene(Scene scene, LoadSceneMode mode = LoadSceneMode.Single)
    {
        SaveInventory();
        if(scene.name == "Scene_Main") LoadInventory();
    }
    
    public void LoadInventory()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            if (data == null) return;
            Equipment equipment = Equipment.Instance;
            //인벤토리 로드
            items.Clear();
            int index = 0;
            foreach (string itemName in data.inventoryItems)
            {
                ItemSO item = ItemDatabase.instance.GetItemByName(itemName);
                if (item != null)
                {
                    item.quantityOrLevel = data.quantityOrLevel[index];
                    items.Add(item);
                }
                index++;
            }

            //인벤토리 스킬 불러오기
            if (skillList.Count > 0)
            {
                //TEST
                SetupSkills();
            }
            else
            {
                skillList.Clear();
                for (int i = 0; i < data.skills.Count; i++)
                {
                    skillList.Add(Resources.Load<Skill>($"Skill/SKILL_{data.skills[i]}"));
                }
                SetupSkills();
                for (int i = 0; i < _filteredSkillList.Count; i++)
                {
                    _filteredSkillList[i].Level = data.skillLevels[i];
                }
            }
            
            //EquippedSkill 불러오기
            if(!string.IsNullOrEmpty(data.equippedSkill1))
            {
                var skill = _filteredSkillList.Find((x) => x.CodeName == data.equippedSkill1);
                equipment.Equip(skill, 0);
            }
            if(!string.IsNullOrEmpty(data.equippedSkill2))
            {
                var skill = _filteredSkillList.Find((x) => x.CodeName == data.equippedSkill2);
                equipment.Equip(skill, 1);
            }
           
            //장비창 로드
            equipment.LoadEquipment(data);
        }
    }

    [ContextMenu("Clear Inventory")]
    public void ClearInventory()
    {
        items.Clear(); // 아이템 목록 비우기
        skillList.Clear(); // 스킬 목록 비우기
        _filteredSkillList.Clear(); // 필터링된 스킬 목록 비우기

        SaveInventory(); // 변경사항 저장
    }
}

