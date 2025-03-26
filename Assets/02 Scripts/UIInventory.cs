using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIInventory : MonoBehaviour
{
    [SerializeField] private Button backBtn;
    [SerializeField] private UISlot slotPrefab; // UISlot 프리팹 참조
    [SerializeField] private Transform slotsParent; // 슬롯들의 부모 Transform (ScrollView의 Content)
    [SerializeField] private ScrollRect scrollView; // 스크롤뷰 참조
    [SerializeField] private TextMeshProUGUI currentSlotCount;

    [Header("오브젝트 풀링 설정")]
    [SerializeField] private int initialPoolSize = 30; // 초기 풀 크기

    private List<UISlot> pooledSlots = new List<UISlot>(); // 전체 슬롯 풀
    private List<UISlot> activeSlots = new List<UISlot>(); // 현재 활성화된 슬롯들
    private Vector2 lastScrollPosition = Vector2.zero; // 마지막 스크롤 위치 저장

    void Start()
    {
        gameObject.SetActive(false);

        if (backBtn != null)
            backBtn.onClick.AddListener(OnClickBackBtn);

        // 필요한 컴포넌트 찾기
        InitializeComponents();

        // 슬롯 풀 초기화
        InitializeSlotPool();
    }

    /// <summary>
    /// 필요한 UI 컴포넌트들을 초기화
    /// </summary>
    private void InitializeComponents()
    {
        if (scrollView == null)
            scrollView = GetComponentInChildren<ScrollRect>();

        if (slotsParent == null && scrollView != null)
            slotsParent = scrollView.content;

        if (slotsParent == null)
            slotsParent = transform;
    }

    /// <summary>
    /// 슬롯 오브젝트 풀 초기화
    /// </summary>
    private void InitializeSlotPool()
    {
        if (slotPrefab == null)
            return;

        // 초기 슬롯 풀 생성
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateSlotInPool(i);
        }
    }

    /// <summary>
    /// 풀에 새 슬롯 생성
    /// </summary>
    private UISlot CreateSlotInPool(int index)
    {
        UISlot newSlot = Instantiate(slotPrefab, slotsParent);
        newSlot.name = $"Slot_{index}";
        newSlot.InitSlot(index);
        newSlot.gameObject.SetActive(false);
        pooledSlots.Add(newSlot);
        return newSlot;
    }

    /// <summary>
    /// 인벤토리 UI 초기화 및 표시
    /// </summary>
    public void ShowInventory()
    {
        // 인벤토리 UI 표시
        gameObject.SetActive(true);

        // 인벤토리 내용 갱신
        RefreshInventory(false);
    }

    /// <summary>
    /// 인벤토리 UI를 갱신
    /// </summary>
    /// <param name="preserveScrollPosition">스크롤 위치 보존 여부</param>
    public void RefreshInventory(bool preserveScrollPosition = false)
    {
        Character playerCharacter = GameManager.Instance?.PlayerCharacter;
        if (playerCharacter == null)
            return;

        if (preserveScrollPosition && scrollView != null)
            lastScrollPosition = scrollView.normalizedPosition;

        var inventory = playerCharacter.Inventory;
        int requiredSlots = Mathf.Max(0, inventory.Count);

        // 필요한 만큼의 슬롯을 활성화
        AdjustActiveSlotCount(requiredSlots);

        // 모든 활성 슬롯 초기화
        ClearAllActiveSlots();

        // 슬롯에 아이템 정보 채우기
        PopulateSlots(inventory);

        if (preserveScrollPosition && scrollView != null)
            scrollView.normalizedPosition = lastScrollPosition;

        UpdateSlotCountText();
    }

    /// <summary>
    /// 필요한 슬롯 개수를 조정 (활성화/비활성화)
    /// </summary>
    /// <param name="requiredSlots">필요한 슬롯 개수</param>
    private void AdjustActiveSlotCount(int requiredSlots)
    {
        // 활성 슬롯 수 조정
        if (activeSlots.Count < requiredSlots)
        {
            // 더 필요한 경우 슬롯 활성화
            int slotsToActivate = requiredSlots - activeSlots.Count;
            ActivateMoreSlots(slotsToActivate);
        }
        else if (activeSlots.Count > requiredSlots)
        {
            // 여분의 슬롯 비활성화
            int slotsToDeactivate = activeSlots.Count - requiredSlots;
            DeactivateExcessSlots(slotsToDeactivate);
        }
    }

    /// <summary>
    /// 추가 슬롯 활성화
    /// </summary>
    /// <param name="count">활성화할 슬롯 수</param>
    private void ActivateMoreSlots(int count)
    {
        for (int i = 0; i < count; i++)
        {
            UISlot slot = GetInactiveSlot();
            if (slot != null)
            {
                // 비활성 슬롯 활성화
                slot.gameObject.SetActive(true);
                activeSlots.Add(slot);
            }
        }
    }

    /// <summary>
    /// 비활성 상태인 슬롯을 찾아 반환
    /// </summary>
    /// <returns>비활성 슬롯 (없으면 새로 생성)</returns>
    private UISlot GetInactiveSlot()
    {
        // 비활성 슬롯 찾기
        foreach (UISlot slot in pooledSlots)
        {
            if (!slot.gameObject.activeSelf && !activeSlots.Contains(slot))
            {
                return slot;
            }
        }

        // 못 찾으면 새로 생성
        return CreateSlotInPool(pooledSlots.Count);
    }

    /// <summary>
    /// 여분의 슬롯 비활성화
    /// </summary>
    /// <param name="count">비활성화할 슬롯 수</param>
    private void DeactivateExcessSlots(int count)
    {
        if (count <= 0 || activeSlots.Count == 0)
            return;

        // 비활성화할 슬롯 수 조정
        count = Mathf.Min(count, activeSlots.Count);

        // 뒤에서부터 비활성화 (최근에 추가된 슬롯부터)
        for (int i = 0; i < count; i++)
        {
            int lastIndex = activeSlots.Count - 1;
            UISlot slot = activeSlots[lastIndex];

            // 슬롯 비활성화 및 초기화
            slot.ClearSlot();
            slot.gameObject.SetActive(false);

            // 활성 슬롯 리스트에서 제거
            activeSlots.RemoveAt(lastIndex);
        }
    }

    /// <summary>
    /// 모든 활성 슬롯을 비움
    /// </summary>
    private void ClearAllActiveSlots()
    {
        foreach (UISlot slot in activeSlots)
        {
            slot.ClearSlot();
        }
    }

    /// <summary>
    /// 슬롯들을 아이템 정보로 채움
    /// </summary>
    /// <param name="inventory">인벤토리 아이템 목록</param>
    private void PopulateSlots(IReadOnlyList<Item> inventory)
    {
        for (int i = 0; i < inventory.Count && i < activeSlots.Count; i++)
        {
            activeSlots[i].SetItem(inventory[i]);
        }
    }

    /// <summary>
    /// 모든 슬롯 풀로 반환 및 초기화
    /// </summary>
    public void ClearSlots()
    {
        foreach (UISlot slot in activeSlots)
        {
            if (slot != null)
            {
                slot.ClearSlot();
                slot.gameObject.SetActive(false);
            }
        }

        activeSlots.Clear();
        UpdateSlotCountText();
    }

    /// <summary>
    /// 뒤로가기 버튼 클릭 처리
    /// </summary>
    private void OnClickBackBtn()
    {
        gameObject.SetActive(false);
        if (UIManager.Instance != null && UIManager.Instance.MainMenu != null)
        {
            UIManager.Instance.MainMenu.ReActiveButtons();
        }
    }

    /// <summary>
    /// 슬롯 개수 텍스트 업데이트
    /// </summary>
    private void UpdateSlotCountText()
    {
        if (currentSlotCount != null)
        {
            currentSlotCount.text = activeSlots.Count.ToString();
        }
    }

    /// <summary>
    /// 활성화된 인벤토리 슬롯 리스트 반환
    /// </summary>
    public List<UISlot> Slots => activeSlots;

    /// <summary>
    /// 아이템을 빈 슬롯에 추가
    /// </summary>
    /// <param name="item">추가할 아이템</param>
    /// <returns>추가 성공 여부</returns>
    public bool AddItemToSlot(Item item)
    {
        if (item == null || item.IsEmpty())
            return false;

        // 빈 슬롯 찾기
        foreach (UISlot existingSlot in activeSlots)
        {
            if (existingSlot.IsEmpty())
            {
                existingSlot.SetItem(item);
                return true;
            }
        }

        // 빈 슬롯이 없으면 새 슬롯 활성화
        UISlot newSlot = GetInactiveSlot();
        newSlot.gameObject.SetActive(true);
        activeSlots.Add(newSlot);
        newSlot.SetItem(item);

        UpdateSlotCountText();
        return true;
    }

    /// <summary>
    /// 현재 표시된 모든 슬롯 UI 업데이트
    /// </summary>
    public void UpdateInventoryUI()
    {
        foreach (UISlot slot in activeSlots)
        {
            slot.UpdateUI();
        }
    }

    /// <summary>
    /// 인벤토리 UI 강제 갱신
    /// </summary>
    public void ForceRefreshNow()
    {
        RefreshInventory(true);
    }
}