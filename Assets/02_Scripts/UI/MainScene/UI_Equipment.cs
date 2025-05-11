using System;
using Spine.Unity;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_Equipment : UiWindow
{
    [SerializeField] private EquipmentSlot weaponSlot, headsetSlot, armorSlot, bootsSlot;
    [SerializeField] private Button closeBtn;
    [SerializeField] private SkeletonGraphic skeleton;
    [SerializeField] private AnimationReferenceAsset emotion;
    
    protected override void Start()
    {
        base.Start();
        closeBtn.onClick.AddListener(() => CloseUI());
        Equipment equipment = Equipment.Instance;
        
        equipment.OnEquipmentChanged -= Equip;
        equipment.OnEquipmentChanged += Equip;
        
        Equip(equipment.weapon);
        Equip(equipment.helmet);
        Equip(equipment.armor);
        Equip(equipment.boots);

        skeleton.AnimationState.AddAnimation(1, emotion, true, 0);
    }

    private void Equip(ItemSO item)
    {
        if (item == null) return;
        
        switch (item.itemType)
        {
            case ItemType.Weapon:
                weaponSlot.EquipItem(item);
                break;
            case ItemType.Armor:
                armorSlot.EquipItem(item);
                break;
            case ItemType.Boots:
                bootsSlot.EquipItem(item);
                break;
            case ItemType.Helmet:
                headsetSlot.EquipItem(item);
                break;
        }
    }
    
    private void CloseUI()
    {
        UI_MainScene.Instance.CloseCurrentWindow();
        TabController.Instance.ResetTabs();
    }
}
