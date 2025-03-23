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

    [Header("기본 정보")]
    [SerializeField] private string itemName;
    [SerializeField] private Sprite itemSprite;
    [SerializeField] private string description;
    [SerializeField] private ItemType itemType;
    [SerializeField] private int itemValue; // 판매 가격
    [SerializeField] private bool isStackable; // 중첩 가능 여부
    [SerializeField] private int maxStack = 1; // 최대 중첩 수량

    [Header("장비 스탯")]
    [SerializeField] private int attackBonus;
    [SerializeField] private int defenseBonus;
    [SerializeField] private int healthBonus;
    [SerializeField] private float criticalChanceBonus;

    [Header("소모품 효과")]
    [SerializeField] private int healAmount;

    // 프로퍼티
    public string ItemName => itemName;
    public Sprite ItemSprite => itemSprite;
    public string Description => description;
    public ItemType Type => itemType;
    public int ItemValue => itemValue;
    public bool IsStackable => isStackable;
    public int MaxStack => maxStack;

    // 장비 스탯 프로퍼티
    public int AttackBonus => attackBonus;
    public int DefenseBonus => defenseBonus;
    public int HealthBonus => healthBonus;
    public float CriticalChanceBonus => criticalChanceBonus;

    // 소모품 효과 프로퍼티
    public int HealAmount => healAmount;

    // 아이템 사용 메서드
    public virtual bool Use(Character character)
    {
        switch (itemType)
        {
            case ItemType.Consumable:
                // 소모품 사용 로직
                if (healAmount > 0)
                {
                    character.HealthPoints += healAmount;
                    Debug.Log($"{character.CharacterName}이(가) {itemName}을(를) 사용하여 체력을 {healAmount} 회복했습니다.");
                    return true;
                }
                break;

            case ItemType.Weapon:
            case ItemType.Armor:
            case ItemType.Accessory:
                // 장비 아이템 장착
                character.Equip(this);
                return true;

            default:
                Debug.Log($"{itemName}은(는) 사용할 수 없는 아이템입니다.");
                break;
        }

        return false;
    }

    // 아이템 정보 문자열 반환
    public override string ToString()
    {
        string info = $"{itemName} - {description}\n";

        if (itemType == ItemType.Weapon || itemType == ItemType.Armor || itemType == ItemType.Accessory)
        {
            if (attackBonus != 0) info += $"공격력: +{attackBonus} ";
            if (defenseBonus != 0) info += $"방어력: +{defenseBonus} ";
            if (healthBonus != 0) info += $"체력: +{healthBonus} ";
            if (criticalChanceBonus != 0) info += $"치명타: +{criticalChanceBonus * 100}% ";
        }
        else if (itemType == ItemType.Consumable && healAmount > 0)
        {
            info += $"회복량: {healAmount}";
        }

        return info;
    }
}