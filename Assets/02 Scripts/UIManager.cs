using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    #region �̱��� ����
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
    #endregion

    #region UI ������Ʈ ����
    [SerializeField] private UIMainMenu _mainMenu;
    [SerializeField] private UIStatus _status;
    [SerializeField] private UIInventory _inventory;

    // ĳ���� ����
    private Character playerCharacter;

    // UI ������Ʈ ������Ƽ
    public UIMainMenu MainMenu => _mainMenu;
    public UIStatus Status => _status;
    public UIInventory Inventory => _inventory;
    #endregion

    private void Awake()
    {
        InitializeSingleton();
        FindUIComponents();
    }

    private void Start()
    {
        // GameManager�κ��� ĳ���� ���� ��������
        if (GameManager.Instance != null && GameManager.Instance.PlayerCharacter != null)
        {
            SetPlayerCharacter(GameManager.Instance.PlayerCharacter);
        }
    }

    /// <summary>
    /// �̱��� �ʱ�ȭ
    /// </summary>
    private void InitializeSingleton()
    {
        // �ߺ� �ν��Ͻ� ����
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// UI ������Ʈ ã��
    /// </summary>
    private void FindUIComponents()
    {
        if (_mainMenu == null)
            _mainMenu = FindObjectOfType<UIMainMenu>();

        if (_status == null)
            _status = FindObjectOfType<UIStatus>();

        if (_inventory == null)
            _inventory = FindObjectOfType<UIInventory>();
    }

    /// <summary>
    /// �÷��̾� ĳ���� ���� �� �̺�Ʈ ����
    /// </summary>
    /// <param name="character">ĳ���� �ν��Ͻ�</param>
    public void SetPlayerCharacter(Character character)
    {
        if (character == null)
            return;

        // ���� ĳ������ �̺�Ʈ ���� ����
        UnsubscribeFromCharacterEvents();

        // �� ĳ���� ����
        playerCharacter = character;

        // �� ĳ������ �̺�Ʈ ����
        SubscribeToCharacterEvents();

        // �ʱ� UI ������Ʈ
        UpdateAllUI();
    }

    /// <summary>
    /// ĳ���� �̺�Ʈ ����
    /// </summary>
    private void SubscribeToCharacterEvents()
    {
        if (playerCharacter == null)
            return;

        playerCharacter.OnInventoryChanged.AddListener(OnPlayerInventoryChanged);
        playerCharacter.OnEquipmentChanged.AddListener(OnPlayerEquipmentChanged);
        playerCharacter.OnStatsChanged.AddListener(OnPlayerStatsChanged);
    }

    /// <summary>
    /// ĳ���� �̺�Ʈ ���� ����
    /// </summary>
    private void UnsubscribeFromCharacterEvents()
    {
        if (playerCharacter == null)
            return;

        playerCharacter.OnInventoryChanged.RemoveListener(OnPlayerInventoryChanged);
        playerCharacter.OnEquipmentChanged.RemoveListener(OnPlayerEquipmentChanged);
        playerCharacter.OnStatsChanged.RemoveListener(OnPlayerStatsChanged);
    }

    /// <summary>
    /// �κ��丮 ���� �̺�Ʈ ó��
    /// </summary>
    /// <param name="character">����� �κ��丮�� ĳ����</param>
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
                Inventory.RefreshInventory(false);
            }
        }
    }

    /// <summary>
    /// ��� ���� �̺�Ʈ ó��
    /// </summary>
    private void OnPlayerEquipmentChanged()
    {
        // ��� ������ ���ȿ� ������ ��ġ�Ƿ� ���� UI ������Ʈ
        UpdateStatus();
    }

    /// <summary>
    /// ���� ���� �̺�Ʈ ó��
    /// </summary>
    private void OnPlayerStatsChanged()
    {
        // ���� UI�� ���� �޴�(ĳ���� ����) ������Ʈ
        UpdateStatus();
        UpdateMainMenu();
    }

    /// <summary>
    /// ��� UI ������Ʈ
    /// </summary>
    public void UpdateAllUI()
    {
        if (playerCharacter == null)
            return;

        UpdateMainMenu();
        UpdateStatus();
        UpdateInventory();
    }

    /// <summary>
    /// ���� �޴� UI ������Ʈ
    /// </summary>
    private void UpdateMainMenu()
    {
        if (MainMenu != null && playerCharacter != null)
        {
            MainMenu.UpdateCharacterInfo(playerCharacter);
        }
    }

    /// <summary>
    /// ���� UI ������Ʈ
    /// </summary>
    private void UpdateStatus()
    {
        if (Status != null && playerCharacter != null)
        {
            Status.UpdateCharacterInfo(playerCharacter);
        }
    }

    /// <summary>
    /// �κ��丮 UI ������Ʈ
    /// </summary>
    private void UpdateInventory()
    {
        if (Inventory != null)
        {
            Inventory.RefreshInventory(false);
        }
    }

    private void OnDestroy()
    {
        UnsubscribeFromCharacterEvents();
    }
}