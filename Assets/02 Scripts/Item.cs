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

    // ���� ������Ƽ
    public int Amount => amount;

    // ������
    public Item(ItemData data, int amount = 1)
    {
        this.itemData = data;
        this.amount = Mathf.Min(amount, data != null && data.IsStackable ? data.MaxStack : 1);
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

    // �������� ������� Ȯ��
    public bool IsEmpty()
    {
        return itemData == null || amount <= 0;
    }

    // ������ ��� �޼��� - ��� �����۸� ó��
    public virtual bool Use(Character character)
    {
        if (itemData == null)
            return false;

        switch (Type)
        {
            case ItemData.ItemType.Weapon:
            case ItemData.ItemType.Armor:
            case ItemData.ItemType.Accessory:
                // ��� ������ ����
                character.Equip(this);
                return true;

            default:
                Debug.Log($"{ItemName}��(��) ������ �� ���� �������Դϴ�.");
                break;
        }

        return false;
    }

    // ������ ���� ���ڿ� ��ȯ
    public override string ToString()
    {
        if (itemData == null)
            return "�� ������";

        string info = $"{ItemName} - {Description}\n";

        if (Type == ItemData.ItemType.Weapon || Type == ItemData.ItemType.Armor || Type == ItemData.ItemType.Accessory)
        {
            if (AttackBonus != 0) info += $"���ݷ�: +{AttackBonus} ";
            if (DefenseBonus != 0) info += $"����: +{DefenseBonus} ";
            if (HealthBonus != 0) info += $"ü��: +{HealthBonus} ";
            if (CriticalChanceBonus != 0) info += $"ġ��Ÿ: +{CriticalChanceBonus * 100}% ";
        }

        return info;
    }
}