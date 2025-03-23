using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInventory : MonoBehaviour
{
    [SerializeField] private Button backBtn;
    [SerializeField] private UISlot slotPrefab; // UISlot 프리팹 참조
    [SerializeField] private Transform slotsParent; // 슬롯들의 부모 Transform (ScrollView의 Content)
    [SerializeField] private ScrollRect scrollView; // 스크롤뷰 참조

    private List<UISlot> slots = new List<UISlot>(); // UISlot 리스트

    [SerializeField] private int slotCount = 20; // 기본 슬롯 개수 (필요에 따라 조정)

    void Start()
    {
        gameObject.SetActive(false);

        if (backBtn != null)
            backBtn.onClick.AddListener(OnClickBackBtn);

        // ScrollView 구성요소 찾기
        if (scrollView == null)
            scrollView = GetComponentInChildren<ScrollRect>();

        // Content 찾기
        if (slotsParent == null && scrollView != null)
            slotsParent = scrollView.content;

        InitInventoryUI();
    }

    // 인벤토리 UI 초기화 메서드
    private void InitInventoryUI()
    {
        // 이전에 생성된 슬롯이 있으면 정리
        ClearSlots();

        // 슬롯 부모가 없으면 현재 오브젝트를 부모로 사용
        if (slotsParent == null)
        {
            Debug.LogWarning("슬롯 부모(Content)가 설정되지 않았습니다. 현재 오브젝트를 부모로 사용합니다.");
            slotsParent = transform;
        }

        // 슬롯 프리팹 검사
        if (slotPrefab == null)
        {
            Debug.LogError("UISlot 프리팹이 할당되지 않았습니다!");
            return;
        }

        // 지정된 개수만큼 슬롯 생성 - ScrollView의 Content 아래에 생성
        for (int i = 0; i < slotCount; i++)
        {
            UISlot newSlot = Instantiate(slotPrefab, slotsParent);
            newSlot.name = $"Slot_{i}";

            // 슬롯 초기화 (슬롯에 초기화 메서드가 있다면 호출)
            newSlot.InitSlot(i);

            // 리스트에 추가
            slots.Add(newSlot);
        }

        // Content 크기 조정 (필요시)

        Debug.Log($"인벤토리 초기화 완료: {slotCount}개의 슬롯 생성됨");
    }

    //// Content 크기 조정 메서드
    //private void AdjustContentSize()
    //{
    //    if (slotsParent != null)
    //    {
    //        // Content에 RectTransform 있는지 확인
    //        RectTransform contentRect = slotsParent.GetComponent<RectTransform>();
    //        if (contentRect != null)
    //        {
    //            // GridLayoutGroup이 있으면 그 설정을 활용
    //            GridLayoutGroup gridLayout = slotsParent.GetComponent<GridLayoutGroup>();
    //            if (gridLayout != null)
    //            {
    //                // 한 줄에 들어갈 슬롯 개수 계산
    //                float contentWidth = contentRect.rect.width;
    //                float cellWidth = gridLayout.cellSize.x + gridLayout.spacing.x;
    //                int slotsPerRow = Mathf.Max(1, Mathf.FloorToInt(contentWidth / cellWidth));

    //                // 필요한 행 수 계산
    //                int rowsNeeded = Mathf.CeilToInt((float)slotCount / slotsPerRow);

    //                // Content 높이 계산 및 설정
    //                float cellHeight = gridLayout.cellSize.y + gridLayout.spacing.y;
    //                float contentHeight = rowsNeeded * cellHeight + gridLayout.padding.top + gridLayout.padding.bottom;

    //                // 최소 높이 (스크롤뷰 높이)보다 작지 않게 설정
    //                RectTransform scrollRectTransform = scrollView?.GetComponent<RectTransform>();
    //                float minHeight = scrollRectTransform != null ? scrollRectTransform.rect.height : 100f;
    //                contentHeight = Mathf.Max(contentHeight, minHeight);

    //                // Content 높이 설정
    //                contentRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, contentHeight);
    //            }
    //        }
    //    }
    //}

    // 슬롯 정리 메서드
    public void ClearSlots()
    {
        // 이미 생성된 슬롯들 제거
        foreach (UISlot slot in slots)
        {
            if (slot != null)
                Destroy(slot.gameObject);
        }

        // 리스트 초기화
        slots.Clear();
    }

    void OnClickBackBtn()
    {
        gameObject.SetActive(false);
        UIManager.Instance.MainMenu.ReActiveButtons();
    }

    // 외부에서 접근 가능한 슬롯 리스트 프로퍼티
    public List<UISlot> Slots => slots;

    // 아이템을 슬롯에 추가하는 메서드
    public bool AddItemToSlot(Item item)
    {
        if (item == null) return false;

        // 빈 슬롯 찾기
        foreach (UISlot slot in slots)
        {
            if (slot.IsEmpty())
            {
                slot.SetItem(item);
                return true;
            }
        }

        Debug.Log("모든 슬롯이 가득 찼습니다!");
        return false;
    }

    // 인벤토리 UI 업데이트 메서드
    public void UpdateInventoryUI()
    {
        foreach (UISlot slot in slots)
        {
            slot.UpdateUI();
        }
    }
}
