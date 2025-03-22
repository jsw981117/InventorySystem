using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInventory : MonoBehaviour
{
    [SerializeField] private Button backBtn;
    [SerializeField] private UISlot slotPrefab; // UISlot 프리팹 참조
    private Transform slotsParent; // 슬롯들의 부모 Transform

    private List<UISlot> slots = new List<UISlot>(); // UISlot 리스트

    [SerializeField] private int slotCount = 20; // 기본 슬롯 개수 (필요에 따라 조정)

    void Start()
    {
        gameObject.SetActive(false);

        if (backBtn != null)
            backBtn.onClick.AddListener(OnClickBackBtn);

        InitInventoryUI();
    }

    // 인벤토리 UI 초기화 메서드
    private void InitInventoryUI()
    {
        // 이전에 생성된 슬롯이 있으면 정리
        ClearSlots();

        // 슬롯 부모가 없으면 현재 오브젝트를 부모로 사용
        if (slotsParent == null)
            slotsParent = transform;

        // 슬롯 프리팹 검사
        if (slotPrefab == null)
        {
            Debug.LogError("UISlot 프리팹이 할당되지 않았습니다!");
            return;
        }

        // 지정된 개수만큼 슬롯 생성
        for (int i = 0; i < slotCount; i++)
        {
            UISlot newSlot = Instantiate(slotPrefab, slotsParent);
            newSlot.name = $"Slot_{i}";

            // 슬롯 초기화 (슬롯에 초기화 메서드가 있다면 호출)
            newSlot.InitSlot(i);

            // 리스트에 추가
            slots.Add(newSlot);
        }

        Debug.Log($"인벤토리 초기화 완료: {slotCount}개의 슬롯 생성됨");
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
        UIManager.Instance.MainMenu.ReActiveButtons();
    }

    // 외부에서 접근 가능한 슬롯 리스트 프로퍼티
    public List<UISlot> Slots => slots;

    // 아이템 추가 메서드 (UISlot 클래스와 아이템 시스템이 구현되었을 때 사용)
    public bool AddItem(Item item)
    {
        // 비어있는 슬롯 찾기
        foreach (UISlot slot in slots)
        {
            if (slot.IsEmpty())
            {
                slot.SetItem(item);
                return true;
            }
        }

        Debug.Log("인벤토리가 가득 찼습니다!");
        return false;
    }

    public void AddItemToSlot()
    {

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
