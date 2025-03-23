using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item Data")]
public class ItemData : ScriptableObject
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


    // ������Ƽ
    public string ItemName => itemName;
    public Sprite ItemSprite => itemSprite;
    public string Description => description;
    public ItemType Type => itemType;
    public int ItemValue => itemValue;
    public bool IsStackable => isStackable;
    public int MaxStack => maxStack;

    public int AttackBonus => attackBonus;
    public int DefenseBonus => defenseBonus;
    public int HealthBonus => healthBonus;
    public float CriticalChanceBonus => criticalChanceBonus;
}