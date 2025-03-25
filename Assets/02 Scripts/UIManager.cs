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

    // 프라이빗 필드
    [SerializeField] private UIMainMenu _mainMenu;
    [SerializeField] private UIStatus _status;
    [SerializeField] private UIInventory _inventory;

    // 참조 캐싱
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
        // 중복 인스턴스 방지
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject); // 씬 전환시에도 유지

        // 필요한 컴포넌트가 없으면 찾기
        if (_mainMenu == null)
            _mainMenu = FindObjectOfType<UIMainMenu>();

        if (_status == null)
            _status = FindObjectOfType<UIStatus>();

        if (_inventory == null)
            _inventory = FindObjectOfType<UIInventory>();
    }

    private void Start()
    {
        // GameManager로부터 캐릭터 참조 가져오기
        if (GameManager.Instance != null)
        {
            SetPlayerCharacter(GameManager.Instance.PlayerCharacter);
        }
    }

    // 플레이어 캐릭터 설정 및 이벤트 구독
    public void SetPlayerCharacter(Character character)
    {
        if (character == null)
            return;

        // 이전 캐릭터의 이벤트 구독 해제
        if (playerCharacter != null)
        {
            playerCharacter.OnInventoryChanged.RemoveListener(OnPlayerInventoryChanged);
            playerCharacter.OnEquipmentChanged.RemoveListener(OnPlayerEquipmentChanged);
            playerCharacter.OnStatsChanged.RemoveListener(OnPlayerStatsChanged);
        }

        // 새 캐릭터 설정
        playerCharacter = character;

        // 새 캐릭터의 이벤트 구독
        playerCharacter.OnInventoryChanged.AddListener(OnPlayerInventoryChanged);
        playerCharacter.OnEquipmentChanged.AddListener(OnPlayerEquipmentChanged);
        playerCharacter.OnStatsChanged.AddListener(OnPlayerStatsChanged);

        // 초기 UI 업데이트
        UpdateAllUI();
    }

    // 인벤토리 변경 이벤트 핸들러
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
                Inventory.RefreshInventory();
            }

            Debug.Log("인벤토리 변경 이벤트 처리됨");
        }
    }

    // 장비 변경 이벤트 핸들러
    private void OnPlayerEquipmentChanged()
    {
        // 장비 UI 업데이트 (장비 슬롯 UI가 있다면)
        // 스탯 UI 업데이트 (장비에 의한 스탯 변화 반영)
        if (Status != null)
        {
            Status.UpdateCharacterInfo(playerCharacter);
        }
    }

    // 스탯 변경 이벤트 핸들러
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

    // 모든 UI 업데이트 (외부에서 호출 가능)
    public void UpdateAllUI()
    {
        if (playerCharacter == null)
            return;

        // 메인 메뉴 업데이트
        if (MainMenu != null)
        {
            MainMenu.UpdateCharacterInfo(playerCharacter);
        }

        // 스탯 UI 업데이트
        if (Status != null)
        {
            Status.UpdateCharacterInfo(playerCharacter);
        }

        // 인벤토리 UI 업데이트
        if (Inventory != null)
        {
            Inventory.RefreshInventory();
        }
    }

    // 컴포넌트 제거 시 이벤트 구독 해제
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