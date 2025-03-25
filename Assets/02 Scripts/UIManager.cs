using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    #region 싱글톤 구현
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

    #region UI 컴포넌트 참조
    [SerializeField] private UIMainMenu _mainMenu;
    [SerializeField] private UIStatus _status;
    [SerializeField] private UIInventory _inventory;

    // 캐릭터 참조
    private Character playerCharacter;

    // UI 컴포넌트 프로퍼티
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
        // GameManager로부터 캐릭터 참조 가져오기
        if (GameManager.Instance != null && GameManager.Instance.PlayerCharacter != null)
        {
            SetPlayerCharacter(GameManager.Instance.PlayerCharacter);
        }
    }

    /// <summary>
    /// 싱글톤 초기화
    /// </summary>
    private void InitializeSingleton()
    {
        // 중복 인스턴스 방지
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// UI 컴포넌트 찾기
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
    /// 플레이어 캐릭터 설정 및 이벤트 구독
    /// </summary>
    /// <param name="character">캐릭터 인스턴스</param>
    public void SetPlayerCharacter(Character character)
    {
        if (character == null)
            return;

        // 이전 캐릭터의 이벤트 구독 해제
        UnsubscribeFromCharacterEvents();

        // 새 캐릭터 설정
        playerCharacter = character;

        // 새 캐릭터의 이벤트 구독
        SubscribeToCharacterEvents();

        // 초기 UI 업데이트
        UpdateAllUI();
    }

    /// <summary>
    /// 캐릭터 이벤트 구독
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
    /// 캐릭터 이벤트 구독 해제
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
    /// 인벤토리 변경 이벤트 처리
    /// </summary>
    /// <param name="character">변경된 인벤토리의 캐릭터</param>
    private void OnPlayerInventoryChanged(Character character)
    {
        if (Inventory != null && character == playerCharacter)
        {
            if (Inventory.gameObject.activeSelf)
            {
                // 인벤토리가 현재 표시중이면 강제 새로고침
                Inventory.ForceRefreshNow();
            }
            else
            {
                // 인벤토리가 숨겨져 있으면 다음에 표시될 때 새로고침하도록 예약
                Inventory.RefreshInventory(false);
            }
        }
    }

    /// <summary>
    /// 장비 변경 이벤트 처리
    /// </summary>
    private void OnPlayerEquipmentChanged()
    {
        // 장비 변경은 스탯에 영향을 미치므로 스탯 UI 업데이트
        UpdateStatus();
    }

    /// <summary>
    /// 스탯 변경 이벤트 처리
    /// </summary>
    private void OnPlayerStatsChanged()
    {
        // 스탯 UI와 메인 메뉴(캐릭터 정보) 업데이트
        UpdateStatus();
        UpdateMainMenu();
    }

    /// <summary>
    /// 모든 UI 업데이트
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
    /// 메인 메뉴 UI 업데이트
    /// </summary>
    private void UpdateMainMenu()
    {
        if (MainMenu != null && playerCharacter != null)
        {
            MainMenu.UpdateCharacterInfo(playerCharacter);
        }
    }

    /// <summary>
    /// 스탯 UI 업데이트
    /// </summary>
    private void UpdateStatus()
    {
        if (Status != null && playerCharacter != null)
        {
            Status.UpdateCharacterInfo(playerCharacter);
        }
    }

    /// <summary>
    /// 인벤토리 UI 업데이트
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