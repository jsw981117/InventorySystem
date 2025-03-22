using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISlot : MonoBehaviour
{
    [SerializeField] private Image itemImage; // 아이템 이미지
    [SerializeField] private TextMeshProUGUI countText; // 아이템 개수 텍스트

    private int slotIndex; // 슬롯 인덱스
    private Item currentItem; // 현재 아이템 (Item 클래스 구현 필요)
    private int itemCount = 0; // 아이템 개수

    // 슬롯 초기화 메서드
    public void InitSlot(int index)
    {
        slotIndex = index;
        ClearSlot();
    }

    // 슬롯 비우기 메서드
    public void ClearSlot()
    {
        currentItem = null;
        itemCount = 0;

        if (itemImage != null)
        {
            itemImage.gameObject.SetActive(false);
            itemImage.sprite = null;
        }

        if (countText != null)
        {
            countText.gameObject.SetActive(false);
            countText.text = "";
        }
    }

    // 아이템 설정 메서드
    public void SetItem(Item item, int count = 1)
    {
        currentItem = item;
        itemCount = count;

        UpdateUI();
    }

    // UI 업데이트 메서드
    public void UpdateUI()
    {
        if (currentItem != null)
        {
            if (itemImage != null)
            {
                itemImage.gameObject.SetActive(true);
                itemImage.sprite = currentItem.ItemSprite; // Item 클래스에 ItemSprite 프로퍼티가 있다고 가정
            }

            if (countText != null)
            {
                if (itemCount > 1)
                {
                    countText.gameObject.SetActive(true);
                    countText.text = itemCount.ToString();
                }
                else
                {
                    countText.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            ClearSlot();
        }
    }

    // 슬롯이 비어있는지 확인하는 메서드
    public bool IsEmpty()
    {
        return currentItem == null;
    }

    // 현재 아이템 가져오기
    public Item GetItem()
    {
        return currentItem;
    }

    // 아이템 개수 가져오기
    public int GetItemCount()
    {
        return itemCount;
    }

    // 슬롯 인덱스 가져오기
    public int GetSlotIndex()
    {
        return slotIndex;
    }

    // 클릭 이벤트 처리
    public void OnPointerClick(PointerEventData eventData)
    {
        // 좌클릭 - 아이템 선택 또는 사용
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (currentItem != null)
            {
                Debug.Log($"슬롯 {slotIndex}의 아이템 선택: {currentItem.ItemName}"); // Item 클래스에 ItemName 프로퍼티가 있다고 가정

                // 아이템 사용 로직 (필요시 구현)
                // currentItem.Use();
            }
        }
        // 우클릭 - 아이템 정보 표시
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (currentItem != null)
            {
                Debug.Log($"슬롯 {slotIndex}의 아이템 정보: {currentItem.ItemName}");

                // 아이템 정보창 표시 로직 (필요시 구현)
                // UIManager.Instance.ShowItemInfo(currentItem);
            }
        }
    }
}
