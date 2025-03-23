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

    // ItemData 에셋 참조 (인스펙터에서 할당)
    [Header("기본 아이템")]
    [SerializeField] private ItemData ironSwordData;
    [SerializeField] private ItemData leatherArmorData;
    [SerializeField] private ItemData smallPotionData;

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

        if (_playerCharacter == null)
        {
            _playerCharacter = FindObjectOfType<Character>();

            if (_playerCharacter == null)
            {
                GameObject characterObj = new GameObject("Player");
                _playerCharacter = characterObj.AddComponent<Character>();
            }
        }

        // 기본 캐릭터 데이터 설정
        SetData("전사", "플레이어", 1, "기본 캐릭터입니다.", 10, 100, 5, 0.05f);
    }

    public void SetData(string job, string name, int level, string description, int attackPower, int healthPoints, int defense, float criticalChance)
    {
        if (_playerCharacter == null)
        {
            GameObject characterObj = new GameObject("Player");
            _playerCharacter = characterObj.AddComponent<Character>();
        }

        // 캐릭터 기본 데이터 설정
        _playerCharacter.SetCharacterData(job, name, level, description, attackPower, healthPoints, defense, criticalChance);

        // 기본 아이템 지급
        GiveStartingItems();

        UpdateUIWithCharacter(_playerCharacter);
    }

    // 기본 아이템 지급
    private void GiveStartingItems()
    {
        // 기본 무기 지급
        if (ironSwordData != null)
        {
            Item sword = new Item(ironSwordData);
            _playerCharacter.AddItem(sword);
            _playerCharacter.Equip(sword);
        }

        // 기본 방어구 지급
        if (leatherArmorData != null)
        {
            Item armor = new Item(leatherArmorData);
            _playerCharacter.AddItem(armor);
            _playerCharacter.Equip(armor);
        }

        // 소모품 지급
        if (smallPotionData != null)
        {
            // 소모품은 아이템 데이터와 수량 지정 (3개)
            _playerCharacter.AddItem(smallPotionData, 3);
        }

        Debug.Log("기본 아이템 지급 완료");
    }

    // 캐릭터 정보를 UI에 업데이트
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
            UIManager.Instance.Inventory.ClearSlots();

            // 캐릭터 인벤토리의 각 아이템을 UI 슬롯에 표시
            for (int i = 0; i < _playerCharacter.Inventory.Count; i++)
            {
                UIManager.Instance.Inventory.AddItemToSlot(_playerCharacter.Inventory[i].Item);
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