using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<UIManager>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject("UIManager");
                    _instance = obj.AddComponent<UIManager>();
                }
            }
            return _instance;
        }
    }

    // �����̺� �ʵ�
    [SerializeField] private UIMainMenu _mainMenu;
    [SerializeField] private UIStatus _status;
    [SerializeField] private UIInventory _inventory;

    // ���� ĳ��
    private Character playerCharacter;

    public UIMainMenu MainMenu
    {
        get { return _mainMenu; }
        private set { _mainMenu = value; }
    }

    public UIStatus Status
    {
        get { return _status; }
        private set { _status = value; }
    }

    public UIInventory Inventory
    {
        get { return _inventory; }
        private set { _inventory = value; }
    }

    private void Awake()
    {
        // �ߺ� �ν��Ͻ� ����
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject); // �� ��ȯ�ÿ��� ����

        // �ʿ��� ������Ʈ�� ������ ã��
        if (_mainMenu == null)
            _mainMenu = FindObjectOfType<UIMainMenu>();

        if (_status == null)
            _status = FindObjectOfType<UIStatus>();

        if (_inventory == null)
            _inventory = FindObjectOfType<UIInventory>();
    }

    private void Start()
    {
        // GameManager�κ��� ĳ���� ���� ��������
        if (GameManager.Instance != null)
        {
            SetPlayerCharacter(GameManager.Instance.PlayerCharacter);
        }
    }

    // �÷��̾� ĳ���� ���� �� �̺�Ʈ ����
    public void SetPlayerCharacter(Character character)
    {
        if (character == null)
            return;

        // ���� ĳ������ �̺�Ʈ ���� ����
        if (playerCharacter != null)
        {
            playerCharacter.OnInventoryChanged.RemoveListener(OnPlayerInventoryChanged);
            playerCharacter.OnEquipmentChanged.RemoveListener(OnPlayerEquipmentChanged);
            playerCharacter.OnStatsChanged.RemoveListener(OnPlayerStatsChanged);
        }

        // �� ĳ���� ����
        playerCharacter = character;

        // �� ĳ������ �̺�Ʈ ����
        playerCharacter.OnInventoryChanged.AddListener(OnPlayerInventoryChanged);
        playerCharacter.OnEquipmentChanged.AddListener(OnPlayerEquipmentChanged);
        playerCharacter.OnStatsChanged.AddListener(OnPlayerStatsChanged);

        // �ʱ� UI ������Ʈ
        UpdateAllUI();
    }

    // �κ��丮 ���� �̺�Ʈ �ڵ鷯
    private void OnPlayerInventoryChanged(Character character)
    {
        if (Inventory != null && character == playerCharacter)
        {
            if (Inventory.gameObject.activeSelf)
            {
                // �κ��丮�� ���� ǥ�����̸� ���� ���ΰ�ħ
                Inventory.ForceRefreshNow();
            }
            else
            {
                // �κ��丮�� ������ ������ ������ ǥ�õ� �� ���ΰ�ħ�ϵ��� ����
                Inventory.RefreshInventory();
            }

            Debug.Log("�κ��丮 ���� �̺�Ʈ ó����");
        }
    }

    // ��� ���� �̺�Ʈ �ڵ鷯
    private void OnPlayerEquipmentChanged()
    {
        // ��� UI ������Ʈ (��� ���� UI�� �ִٸ�)
        // ���� UI ������Ʈ (��� ���� ���� ��ȭ �ݿ�)
        if (Status != null)
        {
            Status.UpdateCharacterInfo(playerCharacter);
        }
    }

    // ���� ���� �̺�Ʈ �ڵ鷯
    private void OnPlayerStatsChanged()
    {
        if (Status != null)
        {
            Status.UpdateCharacterInfo(playerCharacter);
        }

        if (MainMenu != null)
        {
            MainMenu.UpdateCharacterInfo(playerCharacter);
        }
    }

    // ��� UI ������Ʈ (�ܺο��� ȣ�� ����)
    public void UpdateAllUI()
    {
        if (playerCharacter == null)
            return;

        // ���� �޴� ������Ʈ
        if (MainMenu != null)
        {
            MainMenu.UpdateCharacterInfo(playerCharacter);
        }

        // ���� UI ������Ʈ
        if (Status != null)
        {
            Status.UpdateCharacterInfo(playerCharacter);
        }

        // �κ��丮 UI ������Ʈ
        if (Inventory != null)
        {
            Inventory.RefreshInventory();
        }
    }

    // ������Ʈ ���� �� �̺�Ʈ ���� ����
    private void OnDestroy()
    {
        if (playerCharacter != null)
        {
            playerCharacter.OnInventoryChanged.RemoveListener(OnPlayerInventoryChanged);
            playerCharacter.OnEquipmentChanged.RemoveListener(OnPlayerEquipmentChanged);
            playerCharacter.OnStatsChanged.RemoveListener(OnPlayerStatsChanged);
        }
    }
}