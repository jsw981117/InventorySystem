using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    [SerializeField] private Item item; // ������ ����
    [SerializeField] private int amount; // ����

    // ������
    public InventoryItem(Item item, int amount = 1)
    {
        this.item = item;
        // �������� ���� ���� ���ο� �ִ� ���� ���� ����Ͽ� ���� ����
        this.amount = Mathf.Min(amount, item.IsStackable ? item.MaxStack : 1);
    }

    // ������Ƽ
    public Item Item => item;
    public int Amount => amount;

    // ������ ������ ������Ƽ (���� �޼���)
    public string ItemName => item?.ItemName;
    public Sprite ItemSprite => item?.ItemSprite;
    public ItemData.ItemType ItemType => item != null ? item.Type : ItemData.ItemType.Material;
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

    // ���� ���� �޼���
    public void SetAmount(int newAmount)
    {
        amount = Mathf.Clamp(newAmount, 0, MaxStack);
    }

    // ����ִ��� Ȯ��
    public bool IsEmpty()
    {
        return item == null || amount <= 0;
    }

    // ��� ������ ���������� Ȯ��
    public bool IsEquippable()
    {
        return item != null && (
            item.Type == ItemData.ItemType.Weapon ||
            item.Type == ItemData.ItemType.Armor ||
            item.Type == ItemData.ItemType.Accessory
        );
    }

    // ���� ���纻 ����
    public InventoryItem Clone()
    {
        return new InventoryItem(item, amount);
    }
}