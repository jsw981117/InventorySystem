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

    // ItemData 직접 접근 프로퍼티
    public ItemData Data => itemData;

    // 수량 프로퍼티
    public int Amount => amount;

    // 생성자
    public Item(ItemData data, int amount = 1)
    {
        this.itemData = data;
        this.amount = Mathf.Min(amount, data != null && data.IsStackable ? data.MaxStack : 1);
    }

    // 복제 생성자
    public Item Clone()
    {
        return new Item(itemData, amount);
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

    // 수량 설정 메서드
    public void SetAmount(int newAmount)
    {
        if (IsStackable)
        {
            amount = Mathf.Clamp(newAmount, 0, MaxStack);
        }
        else
        {
            amount = 1; // 스택 불가능한 아이템은 항상 1개
        }
    }

    // 아이템이 비었는지 확인
    public bool IsEmpty()
    {
        return itemData == null || amount <= 0;
    }

    // 장비 가능한 아이템인지 확인
    public bool IsEquippable()
    {
        return itemData != null && (
            Type == ItemData.ItemType.Weapon ||
            Type == ItemData.ItemType.Armor ||
            Type == ItemData.ItemType.Accessory
        );
    }

    // 아이템 사용 메서드
    public virtual bool Use(Character character)
    {
        if (itemData == null)
            return false;

        if (IsEquippable() && character != null)
        {
            // 장비 아이템 장착
            character.Equip(this);
            return true;
        }

        return false;
    }
}