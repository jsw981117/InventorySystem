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

    // 기본 장비 아이템 데이터 참조
    [Header("Basic Items")]
    [SerializeField] private ItemData startingSwordData;
    [SerializeField] private ItemData startingArmorData;
    [SerializeField] private ItemData healingPotionData;
    [SerializeField] private int startingPotionCount = 3;

    // 인벤토리 시스템
    private List<InventoryItem> inventory = new List<InventoryItem>();
    private const int MAX_INVENTORY_SIZE = 20;

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
        if (startingSwordData != null && startingArmorData != null)
        {
            // 인벤토리 초기화
            inventory.Clear();
            equippedWeapon = null;
            equippedArmor = null;
            equippedAccessory = null;

            // 기본 무기 생성 및 장착
            Item sword = new Item(startingSwordData);
            AddItem(sword);
            Equip(sword);

            // 기본 방어구 생성 및 장착
            Item armor = new Item(startingArmorData);
            AddItem(armor);
            Equip(armor);

            // 소모품 지급
            if (healingPotionData != null && startingPotionCount > 0)
            {
                AddItem(healingPotionData, startingPotionCount);
            }

            // UI 업데이트 필요
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

        // 캐릭터 설정 후 아이템 초기화
        InitializeCharacter();
    }

    // ItemData를 통해 직접 아이템 추가
    public bool AddItem(ItemData itemData, int amount = 1)
    {
        if (IsInventoryFull)
        {
            Debug.LogWarning("인벤토리가 가득 찼습니다!");
            return false;
        }

        // 이미 같은 종류의 아이템이 있고, 중첩 가능한지 확인
        if (itemData.IsStackable)
        {
            foreach (InventoryItem invItem in inventory)
            {
                if (invItem.Item.ItemName == itemData.ItemName && invItem.Amount < invItem.MaxStack)
                {
                    // 기존 아이템에 수량 추가
                    int amountToAdd = Mathf.Min(amount, invItem.MaxStack - invItem.Amount);
                    invItem.AddAmount(amountToAdd);

                    // 추가 후 남은 수량이 있다면 새 슬롯에 추가
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

        // 같은 종류의 아이템이 없거나 중첩 불가능한 경우 새 슬롯에 추가
        Item item = new Item(itemData, amount);
        InventoryItem inventoryItem = new InventoryItem(item);
        inventory.Add(inventoryItem);

        Debug.Log($"{itemData.ItemName}을(를) 인벤토리에 추가했습니다.");
        UpdateUI();
        return true;
    }

    // 기존 Item 추가 메서드
    public bool AddItem(Item item)
    {
        if (IsInventoryFull)
        {
            Debug.LogWarning("인벤토리가 가득 찼습니다!");
            return false;
        }

        // Item을 InventoryItem으로 변환
        InventoryItem inventoryItem = new InventoryItem(item);
        inventory.Add(inventoryItem);

        Debug.Log($"{item.ItemName}을(를) 인벤토리에 추가했습니다.");
        UpdateUI();
        return true;
    }

    // 인벤토리에서 아이템 제거
    public bool RemoveItem(InventoryItem inventoryItem)
    {
        bool result = inventory.Remove(inventoryItem);
        if (result) UpdateUI();
        return result;
    }

    // 인벤토리에서 아이템 사용
    public bool UseItem(InventoryItem inventoryItem)
    {
        if (inventoryItem == null || inventoryItem.IsEmpty())
            return false;

        Item item = inventoryItem.Item;
        bool used = item.Use(this);

        // 소모품이고 사용 성공했으면 개수 감소
        if (used && item.Type == ItemData.ItemType.Consumable)
        {
            if (inventoryItem.RemoveAmount(1) && inventoryItem.Amount <= 0)
            {
                // 수량이 0이 되면 인벤토리에서 제거
                RemoveItem(inventoryItem);
            }
        }

        UpdateUI();
        return used;
    }

    // 아이템 장착 메서드
    public void Equip(Item item)
    {
        if (item == null)
        {
            Debug.LogWarning("장착할 아이템이 없습니다.");
            return;
        }

        // 아이템 타입에 따라 적절한 슬롯에 장착
        switch (item.Type)
        {
            case ItemData.ItemType.Weapon:
                // 이미 무기를 장착 중이면 인벤토리로 돌려보냄
                if (equippedWeapon != null)
                {
                    AddItem(equippedWeapon);
                    Debug.Log($"{equippedWeapon.ItemName}을(를) 해제했습니다.");
                }

                equippedWeapon = item;

                // 인벤토리에서 아이템 찾아 제거
                RemoveItemFromInventory(item);

                Debug.Log($"{item.ItemName}을(를) 장착했습니다.");
                break;

            case ItemData.ItemType.Armor:
                if (equippedArmor != null)
                {
                    AddItem(equippedArmor);
                    Debug.Log($"{equippedArmor.ItemName}을(를) 해제했습니다.");
                }

                equippedArmor = item;
                RemoveItemFromInventory(item);
                Debug.Log($"{item.ItemName}을(를) 장착했습니다.");
                break;

            case ItemData.ItemType.Accessory:
                if (equippedAccessory != null)
                {
                    AddItem(equippedAccessory);
                    Debug.Log($"{equippedAccessory.ItemName}을(를) 해제했습니다.");
                }

                equippedAccessory = item;
                RemoveItemFromInventory(item);
                Debug.Log($"{item.ItemName}을(를) 장착했습니다.");
                break;

            default:
                Debug.LogWarning($"{item.ItemName}은(는) 장착할 수 없는 아이템입니다.");
                return;
        }

        UpdateUI();
    }

    // 인벤토리에서 특정 아이템 제거 (내부 메서드)
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
            // 인벤토리가 가득 차지 않았으면 인벤토리에 추가
            if (!IsInventoryFull)
            {
                AddItem(itemToUnequip);
                Debug.Log($"{itemToUnequip.ItemName}을(를) 해제하고 인벤토리에 보관했습니다.");
            }
            else
            {
                Debug.LogWarning($"인벤토리가 가득 차서 {itemToUnequip.ItemName}을(를) 보관할 수 없습니다.");
                // 여기에 아이템 드롭 로직을 추가할 수 있음
            }

            UpdateUI();
        }
        else
        {
            Debug.LogWarning($"해제할 {itemType} 아이템이 없습니다.");
        }
    }

    // 인벤토리에서 인덱스로 아이템 찾기
    public InventoryItem GetInventoryItemAt(int index)
    {
        if (index >= 0 && index < inventory.Count)
            return inventory[index];
        return null;
    }

    // UI 업데이트 메서드
    private void UpdateUI()
    {
        // GameManager를 통해 UI 업데이트 요청
        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateUI();
        }
    }
}