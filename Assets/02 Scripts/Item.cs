using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    [SerializeField] private ItemData itemData; // ItemData SO ����
    [SerializeField] private int amount = 1; // ������ ����

    // ������Ƽ - ��� �����ʹ� ItemData���� ������
    public string ItemName => itemData != null ? itemData.ItemName : "Unknown Item";
    public Sprite ItemSprite => itemData != null ? itemData.ItemSprite : null;
    public string Description => itemData != null ? itemData.Description : "";
    public ItemData.ItemType Type => itemData != null ? itemData.Type : ItemData.ItemType.Material;
    public bool IsStackable => itemData != null && itemData.IsStackable;
    public int MaxStack => itemData != null ? itemData.MaxStack : 1;

    // ��� ���� ������Ƽ
    public int AttackBonus => itemData != null ? itemData.AttackBonus : 0;
    public int DefenseBonus => itemData != null ? itemData.DefenseBonus : 0;
    public int HealthBonus => itemData != null ? itemData.HealthBonus : 0;
    public float CriticalChanceBonus => itemData != null ? itemData.CriticalChanceBonus : 0f;

    // ItemData ���� ���� ������Ƽ
    public ItemData Data => itemData;

    // ���� ������Ƽ
    public int Amount => amount;

    // ������
    public Item(ItemData data, int amount = 1)
    {
        this.itemData = data;
        this.amount = Mathf.Min(amount, data != null && data.IsStackable ? data.MaxStack : 1);
    }

    // ���� ������
    public Item Clone()
    {
        return new Item(itemData, amount);
    }

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
        if (IsStackable)
        {
            amount = Mathf.Clamp(newAmount, 0, MaxStack);
        }
        else
        {
            amount = 1; // ���� �Ұ����� �������� �׻� 1��
        }
    }

    // �������� ������� Ȯ��
    public bool IsEmpty()
    {
        return itemData == null || amount <= 0;
    }

    // ��� ������ ���������� Ȯ��
    public bool IsEquippable()
    {
        return itemData != null && (
            Type == ItemData.ItemType.Weapon ||
            Type == ItemData.ItemType.Armor ||
            Type == ItemData.ItemType.Accessory
        );
    }

    // ������ ��� �޼���
    public virtual bool Use(Character character)
    {
        if (itemData == null)
            return false;

        if (IsEquippable() && character != null)
        {
            // ��� ������ ����
            character.Equip(this);
            return true;
        }

        return false;
    }
}