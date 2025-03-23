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

        // �⺻ ĳ���� ������ ����
        SetData("����", "�÷��̾�", 1, "�⺻ ĳ�����Դϴ�.", 10, 100, 5, 0.05f);
    }

    public void SetData(string job, string name, int level, string description, int attackPower, int healthPoints, int defense, float criticalChance)
    {
        if (_playerCharacter == null)
        {
            GameObject characterObj = new GameObject("Player");
            _playerCharacter = characterObj.AddComponent<Character>();
        }

        // ĳ���� �⺻ ������ ���� (������ �ʱ�ȭ�� Character ���ο��� ó��)
        _playerCharacter.SetCharacterData(job, name, level, description, attackPower, healthPoints, defense, criticalChance);

        // UI ������Ʈ
        UpdateUIWithCharacter(_playerCharacter);
    }

    // UI ������Ʈ �޼���
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
            UIManager.Instance.Inventory.ClearSlots();

            // ĳ���� �κ��丮�� �� �������� UI ���Կ� ǥ��
            for (int i = 0; i < _playerCharacter.Inventory.Count; i++)
            {
                UIManager.Instance.Inventory.AddItemToSlot(_playerCharacter.Inventory[i].Item);
            }

            // ���� ������ ǥ�� (�߰� ���� ����)
        }
    }

    // UI ������Ʈ (�ܺο��� ȣ�� ����)
    public void UpdateUI()
    {
        UpdateUIWithCharacter(_playerCharacter);
    }
}