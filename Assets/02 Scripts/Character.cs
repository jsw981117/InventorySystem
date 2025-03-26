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

    [HideInInspector] public InventoryChangedEvent OnInventoryChanged = new InventoryChangedEvent();
    [HideInInspector] public UnityEvent OnEquipmentChanged = new UnityEvent();
    [HideInInspector] public UnityEvent OnStatsChanged = new UnityEvent();

    // 캐릭터 기본 정보
    public string CharacterJob { get => characterJob; set => characterJob = value; }
    public string CharacterName { get => characterName; set => characterName = value; }
    public int Level { get => level; set => level = value; }
    public string Description { get => description; set => description = value; }
    public int Gold { get => gold; set => gold = value; }


    // 장비 보너스가 적용된 최종 스탯
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

    // 인벤토리 설정
    public IReadOnlyList<Item> Inventory => inventory;
    public int InventorySize => inventory.Count;
    public bool IsInventoryFull => inventory.Count >= MAX_INVENTORY_SIZE;

    // 장착중인 아이템 
    public Item EquippedWeapon => equippedWeapon;
    public Item EquippedArmor => equippedArmor;
    public Item EquippedAccessory => equippedAccessory;

    private void Awake()
    {
        InitializeCharacter();
    }

    /// <summary>
    /// 캐릭터의 기본 데이터를 설정합니다.
    /// </summary>
    public void SetCharacterData(string job, string name, int lvl, string desc, int gold, int atk, int hp, int def, float critChance)
    {
        CharacterJob = job;
        CharacterName = name;
        Level = lvl;
        Description = desc;
        Gold = gold;

        // 기본 스탯 설정
        attackPower = atk;
        healthPoints = hp;
        defense = def;
        criticalChance = critChance;

        // 캐릭터 설정 후 아이템 초기화
        InitializeCharacter();
    }

    /// <summary>
    /// 캐릭터의 초기 아이템을 설정하고 인벤토리를 초기화합니다.
    /// </summary>
    private void InitializeCharacter()
    {
        // 인벤토리와 장착 아이템 초기화
        inventory.Clear();
        equippedWeapon = null;
        equippedArmor = null;
        equippedAccessory = null;

        // 이벤트 발생 - 초기화 완료 통지
        NotifyAllChanged();
    }

    /// <summary>
    /// 모든 상태 변경 이벤트를 한 번에 발생시키는 유틸리티 메서드
    /// </summary>
    private void NotifyAllChanged()
    {
        OnInventoryChanged.Invoke(this);
        OnEquipmentChanged.Invoke();
        OnStatsChanged.Invoke();
    }

    #region 인벤토리 관리 메서드
    /// <summary>
    /// ItemData를 사용하여 아이템을 인벤토리에 추가합니다.
    /// </summary>
    public bool AddItem(ItemData itemData, int amount = 1)
    {
        if (itemData == null)
            return false;

        // 인벤토리가 가득 찼고 스택할 수 없는 경우
        if (IsInventoryFull && !CanStackItem(itemData))
            return false;

        // 스택 가능한 아이템인 경우 기존 아이템에 추가 시도
        if (itemData.IsStackable)
        {
            for (int i = 0; i < inventory.Count; i++)
            {
                Item item = inventory[i];
                if (item.ItemName == itemData.ItemName && item.Amount < item.MaxStack)
                {
                    // 기존 아이템에 수량 추가
                    int amountToAdd = Mathf.Min(amount, item.MaxStack - item.Amount);
                    item.AddAmount(amountToAdd);

                    // 남은 수량이 있다면 새 아이템으로 추가
                    int remaining = amount - amountToAdd;
                    if (remaining > 0 && !IsInventoryFull)
                    {
                        Item newItem = new Item(itemData, remaining);
                        inventory.Add(newItem);
                    }

                    // 아이템 추가 완료 알림
                    OnInventoryChanged.Invoke(this);
                    return true;
                }
            }
        }

        // 새 아이템 인스턴스 생성
        Item itemToAdd = new Item(itemData, amount);

        // 인벤토리에 추가
        if (IsInventoryFull)
            return false;

        inventory.Add(itemToAdd);
        OnInventoryChanged.Invoke(this);
        return true;
    }


    /// <summary>
    /// 아이템이 기존 인벤토리에 스택 가능한지 확인합니다.
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
    #endregion

    #region 장비 관리 메서드
    /// <summary>
    /// 아이템을 장착합니다.
    /// </summary>
    public void Equip(Item item)
    {
        if (item == null || !item.IsEquippable())
            return;

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

        OnEquipmentChanged.Invoke();
        OnStatsChanged.Invoke();
        OnInventoryChanged.Invoke(this); // UI에서 장착 상태 표시를 위해
    }

    /// <summary>
    /// 장착템 해제
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
            OnEquipmentChanged.Invoke();
            OnStatsChanged.Invoke();
            OnInventoryChanged.Invoke(this); // UI에서 장착 상태 표시를 위해
        }
    }

    /// <summary>
    /// 아이템이 장착 중인지 확인
    /// </summary>
    public bool IsItemEquipped(Item item)
    {
        if (item == null)
            return false;

        return equippedWeapon == item || equippedArmor == item || equippedAccessory == item;
    }
    #endregion
}