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

        // 인벤토리에 있는 아이템 개수만큼 슬롯 생성
        Character playerCharacter = GameManager.Instance?.PlayerCharacter;
        if (playerCharacter != null)
        {
            // 아이템 목록을 가져와 슬롯으로 변환
            List<Item> characterItems = new List<Item>();

            // Inventory에서 아이템 가져오기
            foreach (var invItem in playerCharacter.Inventory)
            {
                characterItems.Add(invItem.Item);
            }

            int itemCount = characterItems.Count;

            // 최소한 빈 슬롯 하나는 표시
            if (itemCount == 0)
                itemCount = 1;

            CreateSlots(itemCount);

            // 아이템 정보로 슬롯 채우기
            for (int i = 0; i < characterItems.Count; i++)
            {
                if (i < slots.Count)
                {
                    slots[i].SetItem(characterItems[i]);
                }
            }

            Debug.Log($"인벤토리 UI 초기화 완료 - 아이템 {characterItems.Count}개");
        }
        else
        {
            // 캐릭터가 없으면 기본 슬롯 5개 생성
            CreateSlots(5);
            Debug.LogWarning("캐릭터를 찾을 수 없어 기본 슬롯 5개만 생성합니다.");
        }
    }

    // 지정된 개수만큼 슬롯 생성
    private void CreateSlots(int count)
    {
        for (int i = 0; i < count; i++)
        {
            UISlot newSlot = Instantiate(slotPrefab, slotsParent);
            newSlot.name = $"Slot_{i}";

            // 슬롯 초기화
            newSlot.InitSlot(i);

            // 리스트에 추가
            slots.Add(newSlot);
        }

        Debug.Log($"인벤토리 초기화 완료: {count}개의 슬롯 생성됨");
    }

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
        if (UIManager.Instance != null && UIManager.Instance.MainMenu != null)
        {
            UIManager.Instance.MainMenu.ReActiveButtons();
        }
    }

    // 외부에서 접근 가능한 슬롯 리스트 프로퍼티
    public List<UISlot> Slots => slots;

    // 아이템을 슬롯에 추가하는 메서드
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

        // 새 슬롯에 아이템 설정
        newSlot.SetItem(item);

        Debug.Log($"새 슬롯 추가: {item.ItemName}");
        return true;
    }

    // 인벤토리 UI 업데이트 메서드
    public void UpdateInventoryUI()
    {
        // 현재 표시된 모든 슬롯 업데이트
        foreach (UISlot slot in slots)
        {
            slot.UpdateUI();
        }
    }

    // 인벤토리 내용 변경 시 호출하여 UI 완전 새로고침
    public void RefreshInventory()
    {
        InitInventoryUI();
    }
}