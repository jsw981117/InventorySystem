using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// �κ��丮 ���� �̺�Ʈ�� ���� Ŭ����
[System.Serializable]
public class InventoryChangedEvent : UnityEvent<Character> { }

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
    [SerializeField] private ItemData startingMaterialData; // ��� ���������� ����
    [SerializeField] private int startingMaterialCount = 5; // ��� ������ �ʱ� ����

    // �κ��丮 �ý���
    private List<InventoryItem> inventory = new List<InventoryItem>();
    private const int MAX_INVENTORY_SIZE = 20;

    // ���� ���� �̺�Ʈ
    [HideInInspector] public InventoryChangedEvent OnInventoryChanged = new InventoryChangedEvent();
    [HideInInspector] public UnityEvent OnEquipmentChanged = new UnityEvent();
    [HideInInspector] public UnityEvent OnStatsChanged = new UnityEvent();

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
        set
        {
            attackPower = value;
            OnStatsChanged.Invoke();
        }
    }

    public int HealthPoints
    {
        get => healthPoints +
               (equippedArmor != null ? equippedArmor.HealthBonus : 0) +
               (equippedAccessory != null ? equippedAccessory.HealthBonus : 0);
        set
        {
            healthPoints = value;
            OnStatsChanged.Invoke();
        }
    }

    public int Defense
    {
        get => defense +
               (equippedArmor != null ? equippedArmor.DefenseBonus : 0) +
               (equippedAccessory != null ? equippedAccessory.DefenseBonus : 0);
        set
        {
            defense = value;
            OnStatsChanged.Invoke();
        }
    }

    public float CriticalChance
    {
        get => criticalChance +
               (equippedWeapon != null ? equippedWeapon.CriticalChanceBonus : 0) +
               (equippedAccessory != null ? equippedAccessory.CriticalChanceBonus : 0);
        set
        {
            criticalChance = value;
            OnStatsChanged.Invoke();
        }
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
        if (startingSwordData != null || startingArmorData != null || startingMaterialData != null)
        {
            // �κ��丮 �ʱ�ȭ
            inventory.Clear();
            equippedWeapon = null;
            equippedArmor = null;
            equippedAccessory = null;

            // �⺻ ���� ���� �� ����
            if (startingSwordData != null)
            {
                Item sword = new Item(startingSwordData);
                AddItemToInventory(new InventoryItem(sword));
                Equip(sword);
            }

            // �⺻ �� ���� �� ����
            if (startingArmorData != null)
            {
                Item armor = new Item(startingArmorData);
                AddItemToInventory(new InventoryItem(armor));
                Equip(armor);
            }

            // �⺻ ��� ����
            if (startingMaterialData != null && startingMaterialCount > 0)
            {
                AddItem(startingMaterialData, startingMaterialCount);
            }

            // �̺�Ʈ �߻�
            OnInventoryChanged.Invoke(this);
            OnEquipmentChanged.Invoke();
            OnStatsChanged.Invoke();
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
        if (itemData == null)
            return false;

        // �κ��丮�� ���� á���� Ȯ���ϰ� ���� ���� ���� Ȯ��
        if (IsInventoryFull && !CanStackItem(itemData))
        {
            Debug.LogWarning("�κ��丮�� ���� á���ϴ�!");
            return false;
        }

        // �̹� ���� ������ �������� �ְ�, ��ø �������� Ȯ��
        if (itemData.IsStackable)
        {
            for (int i = 0; i < inventory.Count; i++)
            {
                InventoryItem invItem = inventory[i];
                if (invItem.Item.ItemName == itemData.ItemName && invItem.Amount < invItem.MaxStack)
                {
                    // ���� �����ۿ� ���� �߰�
                    int amountToAdd = Mathf.Min(amount, invItem.MaxStack - invItem.Amount);
                    invItem.AddAmount(amountToAdd);

                    // �߰� �� ���� ������ �ִٸ� �� ���Կ� �߰�
                    int remaining = amount - amountToAdd;
                    if (remaining > 0 && !IsInventoryFull)
                    {
                        Item newItem = new Item(itemData);
                        InventoryItem newInvItem = new InventoryItem(newItem, remaining);
                        AddItemToInventory(newInvItem);
                    }

                    // �̺�Ʈ �߻�
                    OnInventoryChanged.Invoke(this);
                    return true;
                }
            }
        }

        // ���� ������ �������� ���ų� ��ø �Ұ����� ��� �� ���Կ� �߰�
        Item item = new Item(itemData);
        InventoryItem inventoryItem = new InventoryItem(item, amount);
        AddItemToInventory(inventoryItem);

        Debug.Log($"{itemData.ItemName}��(��) �κ��丮�� �߰��߽��ϴ�.");

        // �̺�Ʈ �߻�
        OnInventoryChanged.Invoke(this);
        return true;
    }

    // �κ��丮�� ������ �߰� (���� �޼���)
    private bool AddItemToInventory(InventoryItem item)
    {
        if (IsInventoryFull)
            return false;

        inventory.Add(item);
        return true;
    }

    // ������ ���� ���� ���� Ȯ��
    private bool CanStackItem(ItemData itemData)
    {
        if (itemData == null || !itemData.IsStackable)
            return false;

        foreach (InventoryItem invItem in inventory)
        {
            if (invItem.Item.ItemName == itemData.ItemName && invItem.Amount < invItem.MaxStack)
            {
                return true;
            }
        }

        return false;
    }

    // �κ��丮���� ������ ����
    public bool RemoveItem(InventoryItem inventoryItem)
    {
        if (inventoryItem == null)
            return false;

        bool result = inventory.Remove(inventoryItem);

        if (result)
            OnInventoryChanged.Invoke(this);

        return result;
    }

    // �κ��丮���� �ε����� ������ ����
    public bool RemoveItemAt(int index)
    {
        if (index < 0 || index >= inventory.Count)
            return false;

        inventory.RemoveAt(index);
        OnInventoryChanged.Invoke(this);
        return true;
    }

    // ������ ���� �޼���
    public void Equip(Item item)
    {
        if (item == null || !item.IsEquippable())
        {
            Debug.LogWarning("������ �� ���� �������Դϴ�.");
            return;
        }

        // ������ Ÿ�Կ� ���� ������ ���Կ� ����
        switch (item.Type)
        {
            case ItemData.ItemType.Weapon:
                // �̹� ���⸦ ���� ���̸� ����
                if (equippedWeapon != null)
                {
                    Debug.Log($"{equippedWeapon.ItemName}��(��) �����߽��ϴ�.");
                }

                equippedWeapon = item;
                break;

            case ItemData.ItemType.Armor:
                if (equippedArmor != null)
                {
                    Debug.Log($"{equippedArmor.ItemName}��(��) �����߽��ϴ�.");
                }

                equippedArmor = item;
                break;

            case ItemData.ItemType.Accessory:
                if (equippedAccessory != null)
                {
                    Debug.Log($"{equippedAccessory.ItemName}��(��) �����߽��ϴ�.");
                }

                equippedAccessory = item;
                break;
        }

        // �κ��丮���� �������� �������� ���� (�� �κ� ����)
        // RemoveItemByInstance(item);

        Debug.Log($"{item.ItemName}��(��) �����߽��ϴ�.");

        // �̺�Ʈ �߻�
        OnEquipmentChanged.Invoke();
        OnStatsChanged.Invoke();
        // �κ��丮 UI�� �����ؾ� �� (���� ���� ǥ�ø� ����)
        OnInventoryChanged.Invoke(this);
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
            // �������� �̹� �κ��丮�� �����Ƿ� �߰����� ����
            Debug.Log($"{itemToUnequip.ItemName}��(��) �����߽��ϴ�.");

            // �̺�Ʈ �߻�
            OnEquipmentChanged.Invoke();
            OnStatsChanged.Invoke();
            // �κ��丮 UI�� �����ؾ� �� (���� ���� ǥ�ø� ����)
            OnInventoryChanged.Invoke(this);
        }
        else
        {
            Debug.LogWarning($"������ {itemType} �������� �����ϴ�.");
        }
    }

    // ���ο� ���� �޼��� �߰� - �������� ���� ���� ������ Ȯ��
    public bool IsItemEquipped(Item item)
    {
        if (item == null)
            return false;

        return (equippedWeapon == item ||
                equippedArmor == item ||
                equippedAccessory == item);
    }


    // �κ��丮����, �����۰� �ش� ������ ã�� ��ȯ
    public InventoryItem GetInventoryItem(string itemName)
    {
        foreach (InventoryItem item in inventory)
        {
            if (item.ItemName == itemName)
                return item;
        }
        return null;
    }

    // �ε����� �κ��丮 ������ ��������
    public InventoryItem GetInventoryItemAt(int index)
    {
        if (index >= 0 && index < inventory.Count)
            return inventory[index];
        return null;
    }
}