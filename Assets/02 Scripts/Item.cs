using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public enum ItemType
    {
        Weapon,
        Armor,
        Accessory,
        Consumable,
        Quest,
        ETC
    }

    [Header("�⺻ ����")]
    [SerializeField] private string itemName;
    [SerializeField] private Sprite itemSprite;
    [SerializeField] private string description;
    [SerializeField] private ItemType itemType;
    [SerializeField] private int itemValue; // �Ǹ� ����
    [SerializeField] private bool isStackable; // ��ø ���� ����
    [SerializeField] private int maxStack = 1; // �ִ� ��ø ����

    [Header("��� ����")]
    [SerializeField] private int attackBonus;
    [SerializeField] private int defenseBonus;
    [SerializeField] private int healthBonus;
    [SerializeField] private float criticalChanceBonus;

    [Header("�Ҹ�ǰ ȿ��")]
    [SerializeField] private int healAmount;

    // ������Ƽ
    public string ItemName => itemName;
    public Sprite ItemSprite => itemSprite;
    public string Description => description;
    public ItemType Type => itemType;
    public int ItemValue => itemValue;
    public bool IsStackable => isStackable;
    public int MaxStack => maxStack;

    // ��� ���� ������Ƽ
    public int AttackBonus => attackBonus;
    public int DefenseBonus => defenseBonus;
    public int HealthBonus => healthBonus;
    public float CriticalChanceBonus => criticalChanceBonus;

    // �Ҹ�ǰ ȿ�� ������Ƽ
    public int HealAmount => healAmount;

    // ������ ��� �޼���
    public virtual bool Use(Character character)
    {
        switch (itemType)
        {
            case ItemType.Consumable:
                // �Ҹ�ǰ ��� ����
                if (healAmount > 0)
                {
                    character.HealthPoints += healAmount;
                    Debug.Log($"{character.CharacterName}��(��) {itemName}��(��) ����Ͽ� ü���� {healAmount} ȸ���߽��ϴ�.");
                    return true;
                }
                break;

            case ItemType.Weapon:
            case ItemType.Armor:
            case ItemType.Accessory:
                // ��� ������ ����
                character.Equip(this);
                return true;

            default:
                Debug.Log($"{itemName}��(��) ����� �� ���� �������Դϴ�.");
                break;
        }

        return false;
    }

    // ������ ���� ���ڿ� ��ȯ
    public override string ToString()
    {
        string info = $"{itemName} - {description}\n";

        if (itemType == ItemType.Weapon || itemType == ItemType.Armor || itemType == ItemType.Accessory)
        {
            if (attackBonus != 0) info += $"���ݷ�: +{attackBonus} ";
            if (defenseBonus != 0) info += $"����: +{defenseBonus} ";
            if (healthBonus != 0) info += $"ü��: +{healthBonus} ";
            if (criticalChanceBonus != 0) info += $"ġ��Ÿ: +{criticalChanceBonus * 100}% ";
        }
        else if (itemType == ItemType.Consumable && healAmount > 0)
        {
            info += $"ȸ����: {healAmount}";
        }

        return info;
    }
}