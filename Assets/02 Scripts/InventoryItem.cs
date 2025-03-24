using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    [SerializeField] private Item item; // 아이템 참조
    [SerializeField] private int amount; // 수량

    // 생성자
    public InventoryItem(Item item, int amount = 1)
    {
        this.item = item;
        // 아이템의 스택 가능 여부와 최대 스택 수를 고려하여 수량 설정
        this.amount = Mathf.Min(amount, item.IsStackable ? item.MaxStack : 1);
    }

    // 프로퍼티
    public Item Item => item;
    public int Amount => amount;

    // 아이템 데이터 프로퍼티 (편의 메서드)
    public string ItemName => item?.ItemName;
    public Sprite ItemSprite => item?.ItemSprite;
    public ItemData.ItemType ItemType => item != null ? item.Type : ItemData.ItemType.Material;
    public bool IsStackable => item != null && item.IsStackable;
    public int MaxStack => item != null ? item.MaxStack : 1;

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
        amount = Mathf.Clamp(newAmount, 0, MaxStack);
    }

    // 비어있는지 확인
    public bool IsEmpty()
    {
        return item == null || amount <= 0;
    }

    // 장비 가능한 아이템인지 확인
    public bool IsEquippable()
    {
        return item != null && (
            item.Type == ItemData.ItemType.Weapon ||
            item.Type == ItemData.ItemType.Armor ||
            item.Type == ItemData.ItemType.Accessory
        );
    }

    // 깊은 복사본 생성
    public InventoryItem Clone()
    {
        return new InventoryItem(item, amount);
    }
}