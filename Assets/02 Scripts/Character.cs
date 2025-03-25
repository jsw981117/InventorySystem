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
    [SerializeField] private int gold;

    [SerializeField] private int attackPower;
    [SerializeField] private int healthPoints;
    [SerializeField] private int defense;
    [SerializeField][Range(0, 1)] private float criticalChance;

    private List<Item> inventory = new List<Item>();
    private const int MAX_INVENTORY_SIZE = 120;

    private Item equippedWeapon;
    private Item equippedArmor;
    private Item equippedAccessory;

    // �̺�Ʈ �ý��� - ���� ���� �� UI ������Ʈ�� ���� ���
    [HideInInspector] public InventoryChangedEvent OnInventoryChanged = new InventoryChangedEvent();
    [HideInInspector] public UnityEvent OnEquipmentChanged = new UnityEvent();
    [HideInInspector] public UnityEvent OnStatsChanged = new UnityEvent();

    // ĳ���� �⺻ ����
    public string CharacterJob { get => characterJob; set => characterJob = value; }
    public string CharacterName { get => characterName; set => characterName = value; }
    public int Level { get => level; set => level = value; }
    public string Description { get => description; set => description = value; }
    public int Gold { get => gold; set => gold = value; }


    // ��� ���ʽ��� ����� ���� ����
    public int AttackPower => attackPower + (equippedWeapon != null ? equippedWeapon.AttackBonus : 0);
    public int HealthPoints => healthPoints +
                             (equippedArmor != null ? equippedArmor.HealthBonus : 0) +
                             (equippedAccessory != null ? equippedAccessory.HealthBonus : 0);
    public int Defense => defense +
                        (equippedArmor != null ? equippedArmor.DefenseBonus : 0) +
                        (equippedAccessory != null ? equippedAccessory.DefenseBonus : 0);
    public float CriticalChance => criticalChance +
                                 (equippedWeapon != null ? equippedWeapon.CriticalChanceBonus : 0) +
                                 (equippedAccessory != null ? equippedAccessory.CriticalChanceBonus : 0);

    // �κ��丮 �Ӽ�
    public IReadOnlyList<Item> Inventory => inventory;
    public int InventorySize => inventory.Count;
    public bool IsInventoryFull => inventory.Count >= MAX_INVENTORY_SIZE;

    // ���� ������ 
    public Item EquippedWeapon => equippedWeapon;
    public Item EquippedArmor => equippedArmor;
    public Item EquippedAccessory => equippedAccessory;

    private void Awake()
    {
        InitializeCharacter();
    }

    /// <summary>
    /// ĳ������ �⺻ �����͸� �����մϴ�.
    /// </summary>
    public void SetCharacterData(string job, string name, int lvl, string desc, int gold, int atk, int hp, int def, float critChance)
    {
        CharacterJob = job;
        CharacterName = name;
        Level = lvl;
        Description = desc;
        Gold = gold;

        // �⺻ ���� ����
        attackPower = atk;
        healthPoints = hp;
        defense = def;
        criticalChance = critChance;

        // ĳ���� ���� �� ������ �ʱ�ȭ
        InitializeCharacter();
    }

    /// <summary>
    /// ĳ������ �ʱ� �������� �����ϰ� �κ��丮�� �ʱ�ȭ�մϴ�.
    /// </summary>
    private void InitializeCharacter()
    {
        // �κ��丮�� ���� ������ �ʱ�ȭ
        inventory.Clear();
        equippedWeapon = null;
        equippedArmor = null;
        equippedAccessory = null;

        // �̺�Ʈ �߻� - �ʱ�ȭ �Ϸ� ����
        NotifyAllChanged();
    }

    /// <summary>
    /// ��� ���� ���� �̺�Ʈ�� �� ���� �߻���Ű�� ��ƿ��Ƽ �޼���
    /// </summary>
    private void NotifyAllChanged()
    {
        OnInventoryChanged.Invoke(this);
        OnEquipmentChanged.Invoke();
        OnStatsChanged.Invoke();
    }

    #region �κ��丮 ���� �޼���
    /// <summary>
    /// ItemData�� ����Ͽ� �������� �κ��丮�� �߰��մϴ�.
    /// </summary>
    public bool AddItem(ItemData itemData, int amount = 1)
    {
        if (itemData == null)
            return false;

        // �κ��丮�� ���� á�� ������ �� ���� ���
        if (IsInventoryFull && !CanStackItem(itemData))
            return false;

        // ���� ������ �������� ��� ���� �����ۿ� �߰� �õ�
        if (itemData.IsStackable)
        {
            for (int i = 0; i < inventory.Count; i++)
            {
                Item item = inventory[i];
                if (item.ItemName == itemData.ItemName && item.Amount < item.MaxStack)
                {
                    // ���� �����ۿ� ���� �߰�
                    int amountToAdd = Mathf.Min(amount, item.MaxStack - item.Amount);
                    item.AddAmount(amountToAdd);

                    // ���� ������ �ִٸ� �� ���������� �߰�
                    int remaining = amount - amountToAdd;
                    if (remaining > 0 && !IsInventoryFull)
                    {
                        Item newItem = new Item(itemData, remaining);
                        inventory.Add(newItem);
                    }

                    // ������ �߰� �Ϸ� �˸�
                    OnInventoryChanged.Invoke(this);
                    return true;
                }
            }
        }

        // �� ������ �ν��Ͻ� ����
        Item itemToAdd = new Item(itemData, amount);

        // �κ��丮�� �߰�
        if (IsInventoryFull)
            return false;

        inventory.Add(itemToAdd);
        OnInventoryChanged.Invoke(this);
        return true;
    }

    /// <summary>
    /// Item ��ü�� �κ��丮�� �߰��մϴ�.
    /// </summary>
    public bool AddItem(Item item)
    {
        if (item == null || item.IsEmpty())
            return false;

        if (IsInventoryFull)
            return false;

        // ���� ������ �������� ��� ���� �����ۿ� �߰� �õ�
        if (item.IsStackable)
        {
            for (int i = 0; i < inventory.Count; i++)
            {
                Item existingItem = inventory[i];
                if (existingItem.ItemName == item.ItemName && existingItem.Amount < existingItem.MaxStack)
                {
                    // ���� �����ۿ� ���� �߰�
                    int amountToAdd = Mathf.Min(item.Amount, existingItem.MaxStack - existingItem.Amount);
                    existingItem.AddAmount(amountToAdd);

                    // ���� ������ �ִٸ� �� ���������� �߰�
                    int remaining = item.Amount - amountToAdd;
                    if (remaining > 0)
                    {
                        Item remainingItem = new Item(item.Data, remaining);
                        inventory.Add(remainingItem);
                    }

                    OnInventoryChanged.Invoke(this);
                    return true;
                }
            }
        }

        // �� �κ��丮 ���Կ� �߰�
        inventory.Add(item);
        OnInventoryChanged.Invoke(this);
        return true;
    }

    /// <summary>
    /// �������� ���� �κ��丮�� ���� �������� Ȯ���մϴ�.
    /// </summary>
    private bool CanStackItem(ItemData itemData)
    {
        if (itemData == null || !itemData.IsStackable)
            return false;

        for (int i = 0; i < inventory.Count; i++)
        {
            Item item = inventory[i];
            if (item.ItemName == itemData.ItemName && item.Amount < item.MaxStack)
                return true;
        }

        return false;
    }

    /// <summary>
    /// �κ��丮���� �������� �����մϴ�.
    /// </summary>
    public bool RemoveItem(Item item)
    {
        if (item == null)
            return false;

        bool result = inventory.Remove(item);

        if (result)
            OnInventoryChanged.Invoke(this);

        return result;
    }

    /// <summary>
    /// �κ��丮���� Ư�� �ε����� �������� �����մϴ�.
    /// </summary>
    public bool RemoveItemAt(int index)
    {
        if (index < 0 || index >= inventory.Count)
            return false;

        inventory.RemoveAt(index);
        OnInventoryChanged.Invoke(this);
        return true;
    }

    /// <summary>
    /// �κ��丮���� ������ �̸����� �������� ã���ϴ�.
    /// </summary>
    public Item GetItem(string itemName)
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].ItemName == itemName)
                return inventory[i];
        }
        return null;
    }

    /// <summary>
    /// �ε����� �κ��丮 �������� �����ɴϴ�.
    /// </summary>
    public Item GetItemAt(int index)
    {
        if (index >= 0 && index < inventory.Count)
            return inventory[index];
        return null;
    }
    #endregion

    #region ��� ���� �޼���
    /// <summary>
    /// �������� �����մϴ�.
    /// </summary>
    public void Equip(Item item)
    {
        if (item == null || !item.IsEquippable())
            return;

        // ������ Ÿ�Կ� ���� ������ ���Կ� ����
        switch (item.Type)
        {
            case ItemData.ItemType.Weapon:
                equippedWeapon = item;
                break;

            case ItemData.ItemType.Armor:
                equippedArmor = item;
                break;

            case ItemData.ItemType.Accessory:
                equippedAccessory = item;
                break;
        }

        // ��� ���� �˸�
        OnEquipmentChanged.Invoke();
        OnStatsChanged.Invoke();
        OnInventoryChanged.Invoke(this); // UI���� ���� ���� ǥ�ø� ����
    }

    /// <summary>
    /// Ư�� Ÿ���� ���� �������� �����մϴ�.
    /// </summary>
    public void UnEquip(ItemData.ItemType itemType)
    {
        bool itemUnequipped = false;

        switch (itemType)
        {
            case ItemData.ItemType.Weapon:
                if (equippedWeapon != null)
                {
                    equippedWeapon = null;
                    itemUnequipped = true;
                }
                break;

            case ItemData.ItemType.Armor:
                if (equippedArmor != null)
                {
                    equippedArmor = null;
                    itemUnequipped = true;
                }
                break;

            case ItemData.ItemType.Accessory:
                if (equippedAccessory != null)
                {
                    equippedAccessory = null;
                    itemUnequipped = true;
                }
                break;
        }

        if (itemUnequipped)
        {
            // ��� ���� �˸�
            OnEquipmentChanged.Invoke();
            OnStatsChanged.Invoke();
            OnInventoryChanged.Invoke(this); // UI���� ���� ���� ǥ�ø� ����
        }
    }

    /// <summary>
    /// �������� ���� ���� ������ Ȯ���մϴ�.
    /// </summary>
    public bool IsItemEquipped(Item item)
    {
        if (item == null)
            return false;

        return equippedWeapon == item || equippedArmor == item || equippedAccessory == item;
    }
    #endregion
}