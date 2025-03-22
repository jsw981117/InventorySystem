using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private string characterJob;
    [SerializeField] private string characterName;
    [SerializeField] private int level;
    [SerializeField] private string description;

    [SerializeField] private int attackPower;
    [SerializeField] private int healthPoints;
    [SerializeField] private int defense;
    [SerializeField][Range(0, 1)] private float criticalChance;

    // �κ��丮 �߰�
    private List<Item> inventory = new List<Item>();
    private const int MAX_INVENTORY_SIZE = 20;

    // ���� ���� ������
    private Item equippedWeapon;
    private Item equippedArmor;
    private Item equippedAccessory;

    public string CharacterJob { get => characterJob; set => characterJob = value; }
    public string CharacterName { get => characterName; set => characterName = value; }
    public int Level { get => level; set => level = value; }
    public string Description { get => description; set => description = value; }

    // �⺻ ���� + ��� ���ʽ��� ������ ���� ���� ������Ƽ
    public int AttackPower
    {
        get => attackPower + (equippedWeapon != null ? equippedWeapon.AttackBonus : 0);
        set => attackPower = value;
    }

    public int HealthPoints
    {
        get => healthPoints +
               (equippedArmor != null ? equippedArmor.HealthBonus : 0) +
               (equippedAccessory != null ? equippedAccessory.HealthBonus : 0);
        set => healthPoints = value;
    }

    public int Defense
    {
        get => defense +
               (equippedArmor != null ? equippedArmor.DefenseBonus : 0) +
               (equippedAccessory != null ? equippedAccessory.DefenseBonus : 0);
        set => defense = value;
    }

    public float CriticalChance
    {
        get => criticalChance +
               (equippedWeapon != null ? equippedWeapon.CriticalChanceBonus : 0) +
               (equippedAccessory != null ? equippedAccessory.CriticalChanceBonus : 0);
        set => criticalChance = value;
    }

    // �κ��丮 ������Ƽ
    public List<Item> Inventory => inventory;
    public int InventorySize => inventory.Count;
    public bool IsInventoryFull => inventory.Count >= MAX_INVENTORY_SIZE;

    // ���� ������ ������Ƽ
    public Item EquippedWeapon => equippedWeapon;
    public Item EquippedArmor => equippedArmor;
    public Item EquippedAccessory => equippedAccessory;

    public void SetCharacterData(string job, string name, int lvl, string desc, int atk, int hp, int def, float critChance)
    {
        CharacterJob = job;
        CharacterName = name;
        Level = lvl;
        Description = desc;
        AttackPower = atk;
        HealthPoints = hp;
        Defense = def;
        CriticalChance = critChance;
        
        // �κ��丮 �ʱ�ȭ
        inventory.Clear();
        equippedWeapon = null;
        equippedArmor = null;
        equippedAccessory = null;
    }

    // ������ �߰� �޼���
    public bool AddItem(Item item)
    {
        // �κ��丮�� ���� á���� Ȯ��
        if (IsInventoryFull)
        {
            Debug.LogWarning("�κ��丮�� ���� á���ϴ�!");
            return false;
        }

        // �������� �κ��丮�� �߰�
        inventory.Add(item);
        Debug.Log($"{item.ItemName}��(��) �κ��丮�� �߰��߽��ϴ�.");

        return true;
    }

    // ������ ���� �޼���
    public void Equip(Item item)
    {
        if (item == null)
        {
            Debug.LogWarning("������ �������� �����ϴ�.");
            return;
        }

        // ������ Ÿ�Կ� ���� ������ ���Կ� ����
        switch (item.Type)
        {
            case Item.ItemType.Weapon:
                // �̹� ���⸦ ���� ���̸� �κ��丮�� ��������
                if (equippedWeapon != null)
                {
                    inventory.Add(equippedWeapon);
                    Debug.Log($"{equippedWeapon.ItemName}��(��) �����߽��ϴ�.");
                }

                equippedWeapon = item;
                inventory.Remove(item); // �κ��丮���� ����
                Debug.Log($"{item.ItemName}��(��) �����߽��ϴ�.");
                break;

            case Item.ItemType.Armor:
                if (equippedArmor != null)
                {
                    inventory.Add(equippedArmor);
                    Debug.Log($"{equippedArmor.ItemName}��(��) �����߽��ϴ�.");
                }

                equippedArmor = item;
                inventory.Remove(item);
                Debug.Log($"{item.ItemName}��(��) �����߽��ϴ�.");
                break;

            case Item.ItemType.Accessory:
                if (equippedAccessory != null)
                {
                    inventory.Add(equippedAccessory);
                    Debug.Log($"{equippedAccessory.ItemName}��(��) �����߽��ϴ�.");
                }

                equippedAccessory = item;
                inventory.Remove(item);
                Debug.Log($"{item.ItemName}��(��) �����߽��ϴ�.");
                break;

            default:
                Debug.LogWarning($"{item.ItemName}��(��) ������ �� ���� �������Դϴ�.");
                return;
        }

        // UI ������Ʈ ���� (�ɼ�)
        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateUI();
        }
    }

    // ������ ���� �޼���
    public void UnEquip(Item.ItemType itemType)
    {
        Item itemToUnequip = null;

        switch (itemType)
        {
            case Item.ItemType.Weapon:
                if (equippedWeapon != null)
                {
                    itemToUnequip = equippedWeapon;
                    equippedWeapon = null;
                }
                break;

            case Item.ItemType.Armor:
                if (equippedArmor != null)
                {
                    itemToUnequip = equippedArmor;
                    equippedArmor = null;
                }
                break;

            case Item.ItemType.Accessory:
                if (equippedAccessory != null)
                {
                    itemToUnequip = equippedAccessory;
                    equippedAccessory = null;
                }
                break;
        }

        if (itemToUnequip != null)
        {
            // �κ��丮�� ���� ���� �ʾ����� �κ��丮�� �߰�
            if (!IsInventoryFull)
            {
                inventory.Add(itemToUnequip);
                Debug.Log($"{itemToUnequip.ItemName}��(��) �����ϰ� �κ��丮�� �����߽��ϴ�.");
            }
            else
            {
                Debug.LogWarning($"�κ��丮�� ���� ���� {itemToUnequip.ItemName}��(��) ������ �� �����ϴ�.");
                // ���⿡ ������ ��� ������ �߰��� �� ����
            }

            // UI ������Ʈ ���� (�ɼ�)
            if (GameManager.Instance != null)
            {
                GameManager.Instance.UpdateUI();
            }
        }
        else
        {
            Debug.LogWarning($"������ {itemType} �������� �����ϴ�.");
        }
    }
}
