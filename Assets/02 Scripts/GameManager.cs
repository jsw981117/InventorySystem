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

    // ��� ������ �����͸� ������ ��ųʸ�
    private Dictionary<string, Item> itemDatabase = new Dictionary<string, Item>();

    // �׽�Ʈ�� ������ �̹���
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
        SetData("�ڵ�������", "��������", 78, "�⺻ ĳ�����Դϴ�.", 10, 100, 5, 0.05f);
    }

    // ������ �����ͺ��̽� �ʱ�ȭ
    private void InitializeItemDatabase()
    {
        // ���� ������
        Item ironSword = new Item("ö��", defaultWeaponSprite, "�⺻���� ö���Դϴ�.", Item.ItemType.Weapon, 100, 5, 0, 0, 0);
        Item steelSword = new Item("��ö��", defaultWeaponSprite, "�ܴ��� ��ö�� ���� ���Դϴ�.", Item.ItemType.Weapon, 500, 15, 0, 0, 0.05f);

        // �� ������
        Item leatherArmor = new Item("���� ����", defaultArmorSprite, "�⺻���� ���� �����Դϴ�.", Item.ItemType.Armor, 120, 0, 5, 10, 0);
        Item plateArmor = new Item("�Ǳ� ����", defaultArmorSprite, "���ſ� �Ǳ� �����Դϴ�.", Item.ItemType.Armor, 600, 0, 20, 30, 0);

        // ��ű� ������
        Item luckyCharm = new Item("����� ����", defaultAccessorySprite, "ġ��Ÿ Ȯ���� �����ݴϴ�.", Item.ItemType.Accessory, 300, 0, 0, 0, 0.1f);

        // �Ҹ�ǰ ������
        Item smallPotion = new Item("���� ����", defaultPotionSprite, "ü���� ���� ȸ���մϴ�.", 50, 20, true, 99);
        Item largePotion = new Item("���� ����", defaultPotionSprite, "ü���� ���� ȸ���մϴ�.", 150, 50, true, 99);

        // ������ �����ͺ��̽��� �߰�
        AddItemToDatabase(ironSword);
        AddItemToDatabase(steelSword);
        AddItemToDatabase(leatherArmor);
        AddItemToDatabase(plateArmor);
        AddItemToDatabase(luckyCharm);
        AddItemToDatabase(smallPotion);
        AddItemToDatabase(largePotion);

        Debug.Log($"������ �����ͺ��̽� �ʱ�ȭ �Ϸ�: {itemDatabase.Count}���� ������ ���");
    }

    // �������� �����ͺ��̽��� �߰�
    private void AddItemToDatabase(Item item)
    {
        if (!itemDatabase.ContainsKey(item.ItemName))
        {
            itemDatabase.Add(item.ItemName, item);
        }
        else
        {
            Debug.LogWarning($"�̹� �����ϴ� �������Դϴ�: {item.ItemName}");
        }
    }

    // ������ �̸����� ������ ��������
    public Item GetItem(string itemName)
    {
        if (itemDatabase.TryGetValue(itemName, out Item item))
        {
            return item;
        }

        Debug.LogWarning($"�������� ã�� �� �����ϴ�: {itemName}");
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

    // �⺻ ������ ����
    private void GiveStartingItems()
    {
        // �⺻ ��� ����
        Item ironSword = GetItem("ö��");
        if (ironSword != null)
        {
            _playerCharacter.AddItem(ironSword);
            _playerCharacter.Equip(ironSword);
        }

        Item leatherArmor = GetItem("���� ����");
        if (leatherArmor != null)
        {
            _playerCharacter.AddItem(leatherArmor);
            _playerCharacter.Equip(leatherArmor);
        }

        // �Ҹ�ǰ ����
        Item smallPotion = GetItem("���� ����");
        if (smallPotion != null)
        {
            for (int i = 0; i < 3; i++) // 3�� ����
            {
                _playerCharacter.AddItem(smallPotion);
            }
        }

        Debug.Log("�⺻ ������ ���� �Ϸ�");
    }

    private void UpdateUIWithCharacter(Character character)
    {
        if (UIManager.Instance != null)
        {
            // ĳ���� ���� UI ������Ʈ
            if (UIManager.Instance.MainMenu != null)
            {
                UIManager.Instance.MainMenu.UpdateCharacterInfo(character);
            }

            if (UIManager.Instance.Status != null)
            {
                UIManager.Instance.Status.UpdateCharacterInfo(character);
            }

            // �κ��丮 UI ������Ʈ
            if (UIManager.Instance.Inventory != null)
            {
                UpdateInventoryUI();
            }
        }
    }

    // �κ��丮 UI ������Ʈ
    public void UpdateInventoryUI()
    {
        if (UIManager.Instance != null && UIManager.Instance.Inventory != null && _playerCharacter != null)
        {
            // �κ��丮 ���� �ʱ�ȭ �� �籸��
            UIManager.Instance.Inventory.ClearSlots();

            // ĳ���� �κ��丮�� �� �������� UI ���Կ� ǥ��
            foreach (Item item in _playerCharacter.Inventory)
            {
                //UIManager.Instance.Inventory.AddItemToSlot(item);
            }

            // ���� ������ ǥ�� (�ʿ�� ����)
        }
    }

    // UI ������Ʈ (�ܺο��� ȣ�� ����)
    public void UpdateUI()
    {
        UpdateUIWithCharacter(_playerCharacter);
    }
}
