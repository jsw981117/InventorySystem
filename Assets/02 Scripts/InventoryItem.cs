using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    [SerializeField] private Item item; // 아이템 SO 참조
    [SerializeField] private int amount; // 수량

    // 생성자
    public InventoryItem(Item item, int amount = 1)
    {
        this.item = item;
        this.amount = Mathf.Min(amount, item.IsStackable ? item.MaxStack : 1);
    }

    // 프로퍼티
    public Item Item => item;
    public int Amount => amount;
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

    // 아이템이 비었는지 확인
    public bool IsEmpty()
    {
        return item == null || amount <= 0;
    }
}