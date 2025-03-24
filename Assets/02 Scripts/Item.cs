using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    [SerializeField] private ItemData itemData; // ItemData SO 참조
    [SerializeField] private int amount = 1; // 아이템 수량

    // 프로퍼티 - 모든 데이터는 ItemData에서 가져옴
    public string ItemName => itemData != null ? itemData.ItemName : "Unknown Item";
    public Sprite ItemSprite => itemData != null ? itemData.ItemSprite : null;
    public string Description => itemData != null ? itemData.Description : "";
    public ItemData.ItemType Type => itemData != null ? itemData.Type : ItemData.ItemType.Material;
    public bool IsStackable => itemData != null && itemData.IsStackable;
    public int MaxStack => itemData != null ? itemData.MaxStack : 1;

    // 장비 스탯 프로퍼티
    public int AttackBonus => itemData != null ? itemData.AttackBonus : 0;
    public int DefenseBonus => itemData != null ? itemData.DefenseBonus : 0;
    public int HealthBonus => itemData != null ? itemData.HealthBonus : 0;
    public float CriticalChanceBonus => itemData != null ? itemData.CriticalChanceBonus : 0f;

    // 수량 프로퍼티
    public int Amount => amount;

    // 생성자
    public Item(ItemData data, int amount = 1)
    {
        this.itemData = data;
        this.amount = Mathf.Min(amount, data != null && data.IsStackable ? data.MaxStack : 1);
    }

    // 수량 증가 메서드
    public bool AddAmount(int amountToAdd)
    {
        if (!IsStackable)
            return false;

        if (amount + amountToAdd <= MaxStack)
        {
            amount += amountToAdd;
            return true;
        }
        return false;
    }

    // 수량 감소 메서드
    public bool RemoveAmount(int amountToRemove)
    {
        if (amount >= amountToRemove)
        {
            amount -= amountToRemove;
            return true;
        }
        return false;
    }

    // 아이템이 비었는지 확인
    public bool IsEmpty()
    {
        return itemData == null || amount <= 0;
    }

    // 아이템 사용 메서드 - 장비 아이템만 처리
    public virtual bool Use(Character character)
    {
        if (itemData == null)
            return false;

        switch (Type)
        {
            case ItemData.ItemType.Weapon:
            case ItemData.ItemType.Armor:
            case ItemData.ItemType.Accessory:
                // 장비 아이템 장착
                character.Equip(this);
                return true;

            default:
                Debug.Log($"{ItemName}은(는) 장착할 수 없는 아이템입니다.");
                break;
        }

        return false;
    }

    // 아이템 정보 문자열 반환
    public override string ToString()
    {
        if (itemData == null)
            return "빈 아이템";

        string info = $"{ItemName} - {Description}\n";

        if (Type == ItemData.ItemType.Weapon || Type == ItemData.ItemType.Armor || Type == ItemData.ItemType.Accessory)
        {
            if (AttackBonus != 0) info += $"공격력: +{AttackBonus} ";
            if (DefenseBonus != 0) info += $"방어력: +{DefenseBonus} ";
            if (HealthBonus != 0) info += $"체력: +{HealthBonus} ";
            if (CriticalChanceBonus != 0) info += $"치명타: +{CriticalChanceBonus * 100}% ";
        }

        return info;
    }
}