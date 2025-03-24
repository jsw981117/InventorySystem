using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// 인벤토리 변경 이벤트를 위한 클래스
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

    // 기본 장비 아이템 데이터 참조
    [Header("Basic Items")]
    [SerializeField] private ItemData startingSwordData;
    [SerializeField] private ItemData startingArmorData;
    [SerializeField] private ItemData startingMaterialData; // 재료 아이템으로 변경
    [SerializeField] private int startingMaterialCount = 5; // 재료 아이템 초기 수량

    // 인벤토리 시스템
    private List<InventoryItem> inventory = new List<InventoryItem>();
    private const int MAX_INVENTORY_SIZE = 20;

    // 상태 변경 이벤트
    [HideInInspector] public InventoryChangedEvent OnInventoryChanged = new InventoryChangedEvent();
    [HideInInspector] public UnityEvent OnEquipmentChanged = new UnityEvent();
    [HideInInspector] public UnityEvent OnStatsChanged = new UnityEvent();

    // 장착 아이템
    private Item equippedWeapon;
    private Item equippedArmor;
    private Item equippedAccessory;

    // 프로퍼티
    public string CharacterJob { get => characterJob; set => characterJob = value; }
    public string CharacterName { get => characterName; set => characterName = value; }
    public int Level { get => level; set => level = value; }
    public string Description { get => description; set => description = value; }

    // 기본 스탯 + 장비 보너스를 포함한 최종 스탯 프로퍼티
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

    // 인벤토리 프로퍼티
    public IReadOnlyList<InventoryItem> Inventory => inventory;
    public int InventorySize => inventory.Count;
    public bool IsInventoryFull => inventory.Count >= MAX_INVENTORY_SIZE;

    // 장착 아이템 프로퍼티
    public Item EquippedWeapon => equippedWeapon;
    public Item EquippedArmor => equippedArmor;
    public Item EquippedAccessory => equippedAccessory;

    private void Awake()
    {
        // 초기화 로직 수행
        InitializeCharacter();
    }

    private void InitializeCharacter()
    {
        // 기본 아이템 지급
        if (startingSwordData != null || startingArmorData != null || startingMaterialData != null)
        {
            // 인벤토리 초기화
            inventory.Clear();
            equippedWeapon = null;
            equippedArmor = null;
            equippedAccessory = null;

            // 기본 무기 생성 및 장착
            if (startingSwordData != null)
            {
                Item sword = new Item(startingSwordData);
                AddItemToInventory(new InventoryItem(sword));
                Equip(sword);
            }

            // 기본 방어구 생성 및 장착
            if (startingArmorData != null)
            {
                Item armor = new Item(startingArmorData);
                AddItemToInventory(new InventoryItem(armor));
                Equip(armor);
            }

            // 기본 재료 지급
            if (startingMaterialData != null && startingMaterialCount > 0)
            {
                AddItem(startingMaterialData, startingMaterialCount);
            }

            // 이벤트 발생
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

        // 캐릭터 설정 후 아이템 초기화
        InitializeCharacter();
    }

    // ItemData를 통해 직접 아이템 추가
    public bool AddItem(ItemData itemData, int amount = 1)
    {
        if (itemData == null)
            return false;

        // 인벤토리가 가득 찼는지 확인하고 스택 가능 여부 확인
        if (IsInventoryFull && !CanStackItem(itemData))
        {
            Debug.LogWarning("인벤토리가 가득 찼습니다!");
            return false;
        }

        // 이미 같은 종류의 아이템이 있고, 중첩 가능한지 확인
        if (itemData.IsStackable)
        {
            for (int i = 0; i < inventory.Count; i++)
            {
                InventoryItem invItem = inventory[i];
                if (invItem.Item.ItemName == itemData.ItemName && invItem.Amount < invItem.MaxStack)
                {
                    // 기존 아이템에 수량 추가
                    int amountToAdd = Mathf.Min(amount, invItem.MaxStack - invItem.Amount);
                    invItem.AddAmount(amountToAdd);

                    // 추가 후 남은 수량이 있다면 새 슬롯에 추가
                    int remaining = amount - amountToAdd;
                    if (remaining > 0 && !IsInventoryFull)
                    {
                        Item newItem = new Item(itemData);
                        InventoryItem newInvItem = new InventoryItem(newItem, remaining);
                        AddItemToInventory(newInvItem);
                    }

                    // 이벤트 발생
                    OnInventoryChanged.Invoke(this);
                    return true;
                }
            }
        }

        // 같은 종류의 아이템이 없거나 중첩 불가능한 경우 새 슬롯에 추가
        Item item = new Item(itemData);
        InventoryItem inventoryItem = new InventoryItem(item, amount);
        AddItemToInventory(inventoryItem);

        Debug.Log($"{itemData.ItemName}을(를) 인벤토리에 추가했습니다.");

        // 이벤트 발생
        OnInventoryChanged.Invoke(this);
        return true;
    }

    // 인벤토리에 아이템 추가 (내부 메서드)
    private bool AddItemToInventory(InventoryItem item)
    {
        if (IsInventoryFull)
            return false;

        inventory.Add(item);
        return true;
    }

    // 아이템 스택 가능 여부 확인
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

    // 인벤토리에서 아이템 제거
    public bool RemoveItem(InventoryItem inventoryItem)
    {
        if (inventoryItem == null)
            return false;

        bool result = inventory.Remove(inventoryItem);

        if (result)
            OnInventoryChanged.Invoke(this);

        return result;
    }

    // 인벤토리에서 인덱스로 아이템 제거
    public bool RemoveItemAt(int index)
    {
        if (index < 0 || index >= inventory.Count)
            return false;

        inventory.RemoveAt(index);
        OnInventoryChanged.Invoke(this);
        return true;
    }

    // 아이템 장착 메서드
    public void Equip(Item item)
    {
        if (item == null || !item.IsEquippable())
        {
            Debug.LogWarning("장착할 수 없는 아이템입니다.");
            return;
        }

        // 아이템 타입에 따라 적절한 슬롯에 장착
        switch (item.Type)
        {
            case ItemData.ItemType.Weapon:
                // 이미 무기를 장착 중이면 해제
                if (equippedWeapon != null)
                {
                    Debug.Log($"{equippedWeapon.ItemName}을(를) 해제했습니다.");
                }

                equippedWeapon = item;
                break;

            case ItemData.ItemType.Armor:
                if (equippedArmor != null)
                {
                    Debug.Log($"{equippedArmor.ItemName}을(를) 해제했습니다.");
                }

                equippedArmor = item;
                break;

            case ItemData.ItemType.Accessory:
                if (equippedAccessory != null)
                {
                    Debug.Log($"{equippedAccessory.ItemName}을(를) 해제했습니다.");
                }

                equippedAccessory = item;
                break;
        }

        // 인벤토리에서 아이템을 제거하지 않음 (이 부분 제거)
        // RemoveItemByInstance(item);

        Debug.Log($"{item.ItemName}을(를) 장착했습니다.");

        // 이벤트 발생
        OnEquipmentChanged.Invoke();
        OnStatsChanged.Invoke();
        // 인벤토리 UI도 갱신해야 함 (장착 상태 표시를 위해)
        OnInventoryChanged.Invoke(this);
    }


    // 아이템 해제 메서드
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
            // 아이템은 이미 인벤토리에 있으므로 추가하지 않음
            Debug.Log($"{itemToUnequip.ItemName}을(를) 해제했습니다.");

            // 이벤트 발생
            OnEquipmentChanged.Invoke();
            OnStatsChanged.Invoke();
            // 인벤토리 UI도 갱신해야 함 (장착 상태 표시를 위해)
            OnInventoryChanged.Invoke(this);
        }
        else
        {
            Debug.LogWarning($"해제할 {itemType} 아이템이 없습니다.");
        }
    }

    // 새로운 헬퍼 메서드 추가 - 아이템이 현재 장착 중인지 확인
    public bool IsItemEquipped(Item item)
    {
        if (item == null)
            return false;

        return (equippedWeapon == item ||
                equippedArmor == item ||
                equippedAccessory == item);
    }


    // 인벤토리에서, 아이템과 해당 수량을 찾아 반환
    public InventoryItem GetInventoryItem(string itemName)
    {
        foreach (InventoryItem item in inventory)
        {
            if (item.ItemName == itemName)
                return item;
        }
        return null;
    }

    // 인덱스로 인벤토리 아이템 가져오기
    public InventoryItem GetInventoryItemAt(int index)
    {
        if (index >= 0 && index < inventory.Count)
            return inventory[index];
        return null;
    }
}