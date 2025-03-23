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

    // �⺻ ��� ������ ������ ����
    [Header("Basic Items")]
    [SerializeField] private ItemData startingSwordData;
    [SerializeField] private ItemData startingArmorData;
    [SerializeField] private ItemData healingPotionData;
    [SerializeField] private int startingPotionCount = 3;

    // �κ��丮 �ý���
    private List<InventoryItem> inventory = new List<InventoryItem>();
    private const int MAX_INVENTORY_SIZE = 20;

    // ���� ������
    private Item equippedWeapon;
    private Item equippedArmor;
    private Item equippedAccessory;

    // ������Ƽ
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
    public IReadOnlyList<InventoryItem> Inventory => inventory;
    public int InventorySize => inventory.Count;
    public bool IsInventoryFull => inventory.Count >= MAX_INVENTORY_SIZE;

    // ���� ������ ������Ƽ
    public Item EquippedWeapon => equippedWeapon;
    public Item EquippedArmor => equippedArmor;
    public Item EquippedAccessory => equippedAccessory;

    private void Awake()
    {
        // �ʱ�ȭ ���� ����
        InitializeCharacter();
    }

    private void InitializeCharacter()
    {
        // �⺻ ������ ����
        if (startingSwordData != null && startingArmorData != null)
        {
            // �κ��丮 �ʱ�ȭ
            inventory.Clear();
            equippedWeapon = null;
            equippedArmor = null;
            equippedAccessory = null;

            // �⺻ ���� ���� �� ����
            Item sword = new Item(startingSwordData);
            AddItem(sword);
            Equip(sword);

            // �⺻ �� ���� �� ����
            Item armor = new Item(startingArmorData);
            AddItem(armor);
            Equip(armor);

            // �Ҹ�ǰ ����
            if (healingPotionData != null && startingPotionCount > 0)
            {
                AddItem(healingPotionData, startingPotionCount);
            }

            // UI ������Ʈ �ʿ�
            UpdateUI();
        }
    }

    public void SetCharacterData(string job, string name, int lvl, string desc, int atk, int hp, int def, float critChance)
    {
        CharacterJob = job;
        CharacterName = name;
        Level = lvl;
        Description = desc;
        attackPower = atk;
        healthPoints = hp;
        defense = def;
        criticalChance = critChance;

        // ĳ���� ���� �� ������ �ʱ�ȭ
        InitializeCharacter();
    }

    // ItemData�� ���� ���� ������ �߰�
    public bool AddItem(ItemData itemData, int amount = 1)
    {
        if (IsInventoryFull)
        {
            Debug.LogWarning("�κ��丮�� ���� á���ϴ�!");
            return false;
        }

        // �̹� ���� ������ �������� �ְ�, ��ø �������� Ȯ��
        if (itemData.IsStackable)
        {
            foreach (InventoryItem invItem in inventory)
            {
                if (invItem.Item.ItemName == itemData.ItemName && invItem.Amount < invItem.MaxStack)
                {
                    // ���� �����ۿ� ���� �߰�
                    int amountToAdd = Mathf.Min(amount, invItem.MaxStack - invItem.Amount);
                    invItem.AddAmount(amountToAdd);

                    // �߰� �� ���� ������ �ִٸ� �� ���Կ� �߰�
                    int remaining = amount - amountToAdd;
                    if (remaining > 0 && !IsInventoryFull)
                    {
                        Item newItem = new Item(itemData, remaining);
                        InventoryItem newInvItem = new InventoryItem(newItem);
                        inventory.Add(newInvItem);
                    }

                    UpdateUI();
                    return true;
                }
            }
        }

        // ���� ������ �������� ���ų� ��ø �Ұ����� ��� �� ���Կ� �߰�
        Item item = new Item(itemData, amount);
        InventoryItem inventoryItem = new InventoryItem(item);
        inventory.Add(inventoryItem);

        Debug.Log($"{itemData.ItemName}��(��) �κ��丮�� �߰��߽��ϴ�.");
        UpdateUI();
        return true;
    }

    // ���� Item �߰� �޼���
    public bool AddItem(Item item)
    {
        if (IsInventoryFull)
        {
            Debug.LogWarning("�κ��丮�� ���� á���ϴ�!");
            return false;
        }

        // Item�� InventoryItem���� ��ȯ
        InventoryItem inventoryItem = new InventoryItem(item);
        inventory.Add(inventoryItem);

        Debug.Log($"{item.ItemName}��(��) �κ��丮�� �߰��߽��ϴ�.");
        UpdateUI();
        return true;
    }

    // �κ��丮���� ������ ����
    public bool RemoveItem(InventoryItem inventoryItem)
    {
        bool result = inventory.Remove(inventoryItem);
        if (result) UpdateUI();
        return result;
    }

    // �κ��丮���� ������ ���
    public bool UseItem(InventoryItem inventoryItem)
    {
        if (inventoryItem == null || inventoryItem.IsEmpty())
            return false;

        Item item = inventoryItem.Item;
        bool used = item.Use(this);

        // �Ҹ�ǰ�̰� ��� ���������� ���� ����
        if (used && item.Type == ItemData.ItemType.Consumable)
        {
            if (inventoryItem.RemoveAmount(1) && inventoryItem.Amount <= 0)
            {
                // ������ 0�� �Ǹ� �κ��丮���� ����
                RemoveItem(inventoryItem);
            }
        }

        UpdateUI();
        return used;
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
            case ItemData.ItemType.Weapon:
                // �̹� ���⸦ ���� ���̸� �κ��丮�� ��������
                if (equippedWeapon != null)
                {
                    AddItem(equippedWeapon);
                    Debug.Log($"{equippedWeapon.ItemName}��(��) �����߽��ϴ�.");
                }

                equippedWeapon = item;

                // �κ��丮���� ������ ã�� ����
                RemoveItemFromInventory(item);

                Debug.Log($"{item.ItemName}��(��) �����߽��ϴ�.");
                break;

            case ItemData.ItemType.Armor:
                if (equippedArmor != null)
                {
                    AddItem(equippedArmor);
                    Debug.Log($"{equippedArmor.ItemName}��(��) �����߽��ϴ�.");
                }

                equippedArmor = item;
                RemoveItemFromInventory(item);
                Debug.Log($"{item.ItemName}��(��) �����߽��ϴ�.");
                break;

            case ItemData.ItemType.Accessory:
                if (equippedAccessory != null)
                {
                    AddItem(equippedAccessory);
                    Debug.Log($"{equippedAccessory.ItemName}��(��) �����߽��ϴ�.");
                }

                equippedAccessory = item;
                RemoveItemFromInventory(item);
                Debug.Log($"{item.ItemName}��(��) �����߽��ϴ�.");
                break;

            default:
                Debug.LogWarning($"{item.ItemName}��(��) ������ �� ���� �������Դϴ�.");
                return;
        }

        UpdateUI();
    }

    // �κ��丮���� Ư�� ������ ���� (���� �޼���)
    private void RemoveItemFromInventory(Item item)
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].Item == item)
            {
                inventory.RemoveAt(i);
                return;
            }
        }
    }

    // ������ ���� �޼���
    public void UnEquip(ItemData.ItemType itemType)
    {
        Item itemToUnequip = null;

        switch (itemType)
        {
            case ItemData.ItemType.Weapon:
                if (equippedWeapon != null)
                {
                    itemToUnequip = equippedWeapon;
                    equippedWeapon = null;
                }
                break;

            case ItemData.ItemType.Armor:
                if (equippedArmor != null)
                {
                    itemToUnequip = equippedArmor;
                    equippedArmor = null;
                }
                break;

            case ItemData.ItemType.Accessory:
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
                AddItem(itemToUnequip);
                Debug.Log($"{itemToUnequip.ItemName}��(��) �����ϰ� �κ��丮�� �����߽��ϴ�.");
            }
            else
            {
                Debug.LogWarning($"�κ��丮�� ���� ���� {itemToUnequip.ItemName}��(��) ������ �� �����ϴ�.");
                // ���⿡ ������ ��� ������ �߰��� �� ����
            }

            UpdateUI();
        }
        else
        {
            Debug.LogWarning($"������ {itemType} �������� �����ϴ�.");
        }
    }

    // �κ��丮���� �ε����� ������ ã��
    public InventoryItem GetInventoryItemAt(int index)
    {
        if (index >= 0 && index < inventory.Count)
            return inventory[index];
        return null;
    }

    // UI ������Ʈ �޼���
    private void UpdateUI()
    {
        // GameManager�� ���� UI ������Ʈ ��û
        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateUI();
        }
    }
}