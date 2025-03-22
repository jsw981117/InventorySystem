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

    // 인벤토리 추가
    private List<Item> inventory = new List<Item>();
    private const int MAX_INVENTORY_SIZE = 20;

    // 장착 중인 아이템
    private Item equippedWeapon;
    private Item equippedArmor;
    private Item equippedAccessory;

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
    public List<Item> Inventory => inventory;
    public int InventorySize => inventory.Count;
    public bool IsInventoryFull => inventory.Count >= MAX_INVENTORY_SIZE;

    // 장착 아이템 프로퍼티
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
        
        // 인벤토리 초기화
        inventory.Clear();
        equippedWeapon = null;
        equippedArmor = null;
        equippedAccessory = null;
    }

    // 아이템 추가 메서드
    public bool AddItem(Item item)
    {
        // 인벤토리가 가득 찼는지 확인
        if (IsInventoryFull)
        {
            Debug.LogWarning("인벤토리가 가득 찼습니다!");
            return false;
        }

        // 아이템을 인벤토리에 추가
        inventory.Add(item);
        Debug.Log($"{item.ItemName}을(를) 인벤토리에 추가했습니다.");

        return true;
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
            case Item.ItemType.Weapon:
                // 이미 무기를 장착 중이면 인벤토리로 돌려보냄
                if (equippedWeapon != null)
                {
                    inventory.Add(equippedWeapon);
                    Debug.Log($"{equippedWeapon.ItemName}을(를) 해제했습니다.");
                }

                equippedWeapon = item;
                inventory.Remove(item); // 인벤토리에서 제거
                Debug.Log($"{item.ItemName}을(를) 장착했습니다.");
                break;

            case Item.ItemType.Armor:
                if (equippedArmor != null)
                {
                    inventory.Add(equippedArmor);
                    Debug.Log($"{equippedArmor.ItemName}을(를) 해제했습니다.");
                }

                equippedArmor = item;
                inventory.Remove(item);
                Debug.Log($"{item.ItemName}을(를) 장착했습니다.");
                break;

            case Item.ItemType.Accessory:
                if (equippedAccessory != null)
                {
                    inventory.Add(equippedAccessory);
                    Debug.Log($"{equippedAccessory.ItemName}을(를) 해제했습니다.");
                }

                equippedAccessory = item;
                inventory.Remove(item);
                Debug.Log($"{item.ItemName}을(를) 장착했습니다.");
                break;

            default:
                Debug.LogWarning($"{item.ItemName}은(는) 장착할 수 없는 아이템입니다.");
                return;
        }

        // UI 업데이트 로직 (옵션)
        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateUI();
        }
    }

    // 아이템 해제 메서드
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
            // 인벤토리가 가득 차지 않았으면 인벤토리에 추가
            if (!IsInventoryFull)
            {
                inventory.Add(itemToUnequip);
                Debug.Log($"{itemToUnequip.ItemName}을(를) 해제하고 인벤토리에 보관했습니다.");
            }
            else
            {
                Debug.LogWarning($"인벤토리가 가득 차서 {itemToUnequip.ItemName}을(를) 보관할 수 없습니다.");
                // 여기에 아이템 드롭 로직을 추가할 수 있음
            }

            // UI 업데이트 로직 (옵션)
            if (GameManager.Instance != null)
            {
                GameManager.Instance.UpdateUI();
            }
        }
        else
        {
            Debug.LogWarning($"해제할 {itemType} 아이템이 없습니다.");
        }
    }
}
