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

    private List<UISlot> slots = new List<UISlot>(); // UISlot 리스트
    private Vector2 lastScrollPosition = Vector2.zero; // 마지막 스크롤 위치 저장

    void Start()
    {
        gameObject.SetActive(false);

        if (backBtn != null)
            backBtn.onClick.AddListener(OnClickBackBtn);

        // 필요한 컴포넌트 찾기
        InitializeComponents();
    }

    /// <summary>
    /// 필요한 UI 컴포넌트들을 초기화
    /// </summary>
    private void InitializeComponents()
    {
        // ScrollView 구성요소 찾기
        if (scrollView == null)
            scrollView = GetComponentInChildren<ScrollRect>();

        // Content 찾기
        if (slotsParent == null && scrollView != null)
            slotsParent = scrollView.content;

        // 슬롯 부모가 없으면 현재 오브젝트를 부모로 사용
        if (slotsParent == null)
            slotsParent = transform;
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
        // 플레이어 캐릭터 참조 확인
        Character playerCharacter = GameManager.Instance?.PlayerCharacter;
        if (playerCharacter == null)
            return;

        // 스크롤 위치 저장
        if (preserveScrollPosition && scrollView != null)
            lastScrollPosition = scrollView.normalizedPosition;

        // 필요한 슬롯 개수 계산
        var inventory = playerCharacter.Inventory;
        int requiredSlots = Mathf.Max(1, inventory.Count); // 최소 1개 슬롯 필요

        // 슬롯 개수 조정
        AdjustSlotCount(requiredSlots);

        // 모든 슬롯 초기화
        ClearAllSlots();

        // 아이템 정보로 슬롯 채우기
        PopulateSlots(inventory);

        // 스크롤 위치 복원
        if (preserveScrollPosition && scrollView != null)
            scrollView.normalizedPosition = lastScrollPosition;

        UpdateSlotCountText();
    }

    /// <summary>
    /// 필요한 슬롯 개수로 조정
    /// </summary>
    /// <param name="requiredSlots">필요한 슬롯 개수</param>
    private void AdjustSlotCount(int requiredSlots)
    {
        // 슬롯 프리팹 확인
        if (slotPrefab == null)
            return;

        // 슬롯이 부족하면 추가
        if (slots.Count < requiredSlots)
        {
            int slotsToAdd = requiredSlots - slots.Count;
            CreateSlots(slotsToAdd);
        }
    }

    /// <summary>
    /// 지정된 개수만큼 슬롯 생성
    /// </summary>
    /// <param name="count">생성할 슬롯 개수</param>
    private void CreateSlots(int count)
    {
        for (int i = 0; i < count; i++)
        {
            UISlot newSlot = Instantiate(slotPrefab, slotsParent);
            newSlot.name = $"Slot_{slots.Count}";
            newSlot.InitSlot(slots.Count);
            slots.Add(newSlot);
        }
    }

    /// <summary>
    /// 모든 슬롯을 비움
    /// </summary>
    private void ClearAllSlots()
    {
        foreach (UISlot slot in slots)
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
        for (int i = 0; i < inventory.Count; i++)
        {
            if (i < slots.Count)
            {
                slots[i].SetItem(inventory[i]);
            }
        }
    }

    /// <summary>
    /// 모든 슬롯 제거 및 초기화
    /// </summary>
    public void ClearSlots()
    {
        foreach (UISlot slot in slots)
        {
            if (slot != null)
                Destroy(slot.gameObject);
        }

        slots.Clear();
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
            currentSlotCount.text = slots.Count.ToString();
        }
    }

    /// <summary>
    /// 인벤토리 슬롯 리스트 반환
    /// </summary>
    public List<UISlot> Slots => slots;

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
        foreach (UISlot slot in slots)
        {
            if (slot.IsEmpty())
            {
                slot.SetItem(item);
                return true;
            }
        }

        // 빈 슬롯이 없으면 새 슬롯 추가
        UISlot newSlot = Instantiate(slotPrefab, slotsParent);
        newSlot.name = $"Slot_{slots.Count}";
        newSlot.InitSlot(slots.Count);
        slots.Add(newSlot);
        newSlot.SetItem(item);

        UpdateSlotCountText();
        return true;
    }

    /// <summary>
    /// 현재 표시된 모든 슬롯 UI 업데이트
    /// </summary>
    public void UpdateInventoryUI()
    {
        foreach (UISlot slot in slots)
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