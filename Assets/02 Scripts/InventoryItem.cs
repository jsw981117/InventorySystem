using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    [SerializeField] private Item item; // ������ SO ����
    [SerializeField] private int amount; // ����

    // ������
    public InventoryItem(Item item, int amount = 1)
    {
        this.item = item;
        this.amount = Mathf.Min(amount, item.IsStackable ? item.MaxStack : 1);
    }

    // ������Ƽ
    public Item Item => item;
    public int Amount => amount;
    public bool IsStackable => item != null && item.IsStackable;
    public int MaxStack => item != null ? item.MaxStack : 1;

    // ���� ���� �޼���
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

    // ���� ���� �޼���
    public bool RemoveAmount(int amountToRemove)
    {
        if (amount >= amountToRemove)
        {
            amount -= amountToRemove;
            return true;
        }
        return false;
    }

    // �������� ������� Ȯ��
    public bool IsEmpty()
    {
        return item == null || amount <= 0;
    }
}