using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();

                if (_instance == null)
                {
                    GameObject obj = new GameObject("GameManager");
                    _instance = obj.AddComponent<GameManager>();
                }
            }
            return _instance;
        }
    }

    [SerializeField] private Character _playerCharacter;

    // 모든 아이템 데이터를 관리할 딕셔너리
    private Dictionary<string, Item> itemDatabase = new Dictionary<string, Item>();

    // 테스트용 아이템 이미지
    [SerializeField] private Sprite defaultWeaponSprite;
    [SerializeField] private Sprite defaultArmorSprite;
    [SerializeField] private Sprite defaultAccessorySprite;
    [SerializeField] private Sprite defaultPotionSprite;

    public Character PlayerCharacter
    {
        get { return _playerCharacter; }
        private set { _playerCharacter = value; }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeItemDatabase();

        if (_playerCharacter == null)
        {
            _playerCharacter = FindObjectOfType<Character>();

            if (_playerCharacter == null)
            {
                GameObject characterObj = new GameObject("Player");
                _playerCharacter = characterObj.AddComponent<Character>();
            }
        }
        SetData("코딩광전사", "정수누언", 78, "기본 캐릭터입니다.", 10, 100, 5, 0.05f);
    }

    // 아이템 데이터베이스 초기화
    private void InitializeItemDatabase()
    {
        // 무기 아이템
        Item ironSword = new Item("철검", defaultWeaponSprite, "기본적인 철검입니다.", Item.ItemType.Weapon, 100, 5, 0, 0, 0);
        Item steelSword = new Item("강철검", defaultWeaponSprite, "단단한 강철로 만든 검입니다.", Item.ItemType.Weapon, 500, 15, 0, 0, 0.05f);

        // 방어구 아이템
        Item leatherArmor = new Item("가죽 갑옷", defaultArmorSprite, "기본적인 가죽 갑옷입니다.", Item.ItemType.Armor, 120, 0, 5, 10, 0);
        Item plateArmor = new Item("판금 갑옷", defaultArmorSprite, "무거운 판금 갑옷입니다.", Item.ItemType.Armor, 600, 0, 20, 30, 0);

        // 장신구 아이템
        Item luckyCharm = new Item("행운의 부적", defaultAccessorySprite, "치명타 확률을 높여줍니다.", Item.ItemType.Accessory, 300, 0, 0, 0, 0.1f);

        // 소모품 아이템
        Item smallPotion = new Item("소형 포션", defaultPotionSprite, "체력을 조금 회복합니다.", 50, 20, true, 99);
        Item largePotion = new Item("대형 포션", defaultPotionSprite, "체력을 많이 회복합니다.", 150, 50, true, 99);

        // 아이템 데이터베이스에 추가
        AddItemToDatabase(ironSword);
        AddItemToDatabase(steelSword);
        AddItemToDatabase(leatherArmor);
        AddItemToDatabase(plateArmor);
        AddItemToDatabase(luckyCharm);
        AddItemToDatabase(smallPotion);
        AddItemToDatabase(largePotion);

        Debug.Log($"아이템 데이터베이스 초기화 완료: {itemDatabase.Count}개의 아이템 등록");
    }

    // 아이템을 데이터베이스에 추가
    private void AddItemToDatabase(Item item)
    {
        if (!itemDatabase.ContainsKey(item.ItemName))
        {
            itemDatabase.Add(item.ItemName, item);
        }
        else
        {
            Debug.LogWarning($"이미 존재하는 아이템입니다: {item.ItemName}");
        }
    }

    // 아이템 이름으로 아이템 가져오기
    public Item GetItem(string itemName)
    {
        if (itemDatabase.TryGetValue(itemName, out Item item))
        {
            return item;
        }

        Debug.LogWarning($"아이템을 찾을 수 없습니다: {itemName}");
        return null;
    }

    public void SetData(string job, string name, int level, string description, int attackPower, int healthPoints, int defense, float criticalChance)
    {
        if (_playerCharacter == null)
        {
            GameObject characterObj = new GameObject("Player");
            _playerCharacter = characterObj.AddComponent<Character>();
        }

        _playerCharacter.SetCharacterData(job, name, level, description, attackPower, healthPoints, defense, criticalChance);
        GiveStartingItems();
        UpdateUIWithCharacter(_playerCharacter);
    }

    // 기본 아이템 지급
    private void GiveStartingItems()
    {
        // 기본 장비 지급
        Item ironSword = GetItem("철검");
        if (ironSword != null)
        {
            _playerCharacter.AddItem(ironSword);
            _playerCharacter.Equip(ironSword);
        }

        Item leatherArmor = GetItem("가죽 갑옷");
        if (leatherArmor != null)
        {
            _playerCharacter.AddItem(leatherArmor);
            _playerCharacter.Equip(leatherArmor);
        }

        // 소모품 지급
        Item smallPotion = GetItem("소형 포션");
        if (smallPotion != null)
        {
            for (int i = 0; i < 3; i++) // 3개 지급
            {
                _playerCharacter.AddItem(smallPotion);
            }
        }

        Debug.Log("기본 아이템 지급 완료");
    }

    private void UpdateUIWithCharacter(Character character)
    {
        if (UIManager.Instance != null)
        {
            // 캐릭터 정보 UI 업데이트
            if (UIManager.Instance.MainMenu != null)
            {
                UIManager.Instance.MainMenu.UpdateCharacterInfo(character);
            }

            if (UIManager.Instance.Status != null)
            {
                UIManager.Instance.Status.UpdateCharacterInfo(character);
            }

            // 인벤토리 UI 업데이트
            if (UIManager.Instance.Inventory != null)
            {
                UpdateInventoryUI();
            }
        }
    }

    // 인벤토리 UI 업데이트
    public void UpdateInventoryUI()
    {
        if (UIManager.Instance != null && UIManager.Instance.Inventory != null && _playerCharacter != null)
        {
            // 인벤토리 내용 초기화 및 재구성
            UIManager.Instance.Inventory.ClearSlots();

            // 캐릭터 인벤토리의 각 아이템을 UI 슬롯에 표시
            foreach (Item item in _playerCharacter.Inventory)
            {
                //UIManager.Instance.Inventory.AddItemToSlot(item);
            }

            // 장착 아이템 표시 (필요시 구현)
        }
    }

    // UI 업데이트 (외부에서 호출 가능)
    public void UpdateUI()
    {
        UpdateUIWithCharacter(_playerCharacter);
    }
}
