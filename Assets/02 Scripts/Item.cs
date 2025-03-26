using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    [SerializeField] private ItemData itemData;
    [SerializeField] private int amount = 1;

    // ������ �Ӽ�
    public string ItemName => itemData != null ? itemData.ItemName : "Unknown Item";
    public Sprite ItemSprite => itemData != null ? itemData.ItemSprite : null;
    public string Description => itemData != null ? itemData.Description : "";
    public ItemData.ItemType Type => itemData != null ? itemData.Type : ItemData.ItemType.Material;
    public bool IsStackable => itemData != null && itemData.IsStackable;
    public int MaxStack => itemData != null ? itemData.MaxStack : 1;

    // ��� ����
    public int AttackBonus => itemData != null ? itemData.AttackBonus : 0;
    public int DefenseBonus => itemData != null ? itemData.DefenseBonus : 0;
    public int HealthBonus => itemData != null ? itemData.HealthBonus : 0;
    public float CriticalChanceBonus => itemData != null ? itemData.CriticalChanceBonus : 0f;
    public int Amount => amount;

    public Item(ItemData data, int amount = 1)
    {
        this.itemData = data;
        this.amount = Mathf.Min(amount, data != null && data.IsStackable ? data.MaxStack : 1);
    }

    /// <summary>
    /// ������ ���� �߰�
    /// </summary>
    /// <param name="amountToAdd"></param>
    /// <returns></returns>
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


    /// <summary>
    /// �������� ����ִ��� Ȯ��
    /// </summary>
    /// <returns></returns>
    public bool IsEmpty()
    {
        return itemData == null || amount <= 0;
    }

    /// <summary>
    /// �������� ���� �������� Ȯ��
    /// </summary>
    /// <returns></returns>
    public bool IsEquippable()
    {
        return itemData != null && (
            Type == ItemData.ItemType.Weapon ||
            Type == ItemData.ItemType.Armor ||
            Type == ItemData.ItemType.Accessory
        );
    }

    /// <summary>
    /// ������ ���
    /// </summary>
    /// <param name="character"></param>
    /// <returns></returns>
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