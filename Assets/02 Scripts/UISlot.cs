using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UISlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image itemImage; // 아이템 이미지
    [SerializeField] private Text countText; // 아이템 개수 텍스트

    private int slotIndex; // 슬롯 인덱스
    private Item item; // 아이템 참조

    // 슬롯 초기화 메서드
    public void InitSlot(int index)
    {
        slotIndex = index;
        ClearSlot();
    }

    // 슬롯 비우기 메서드
    public void ClearSlot()
    {
        item = null;

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
    public void SetItem(Item newItem)
    {
        item = newItem;
        UpdateUI();
    }

    // UI 업데이트 메서드
    public void UpdateUI()
    {
        if (item != null && !item.IsEmpty())
        {
            if (itemImage != null)
            {
                itemImage.gameObject.SetActive(true);
                itemImage.sprite = item.ItemSprite;
            }

            if (countText != null)
            {
                if (item.Amount > 1)
                {
                    countText.gameObject.SetActive(true);
                    countText.text = item.Amount.ToString();
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
        return item == null || item.IsEmpty();
    }

    // 아이템 가져오기
    public Item GetItem()
    {
        return item;
    }

    // 아이템 개수 가져오기
    public int GetItemAmount()
    {
        return item != null ? item.Amount : 0;
    }

    // 슬롯 인덱스 가져오기
    public int GetSlotIndex()
    {
        return slotIndex;
    }

    // 클릭 이벤트 처리
    public void OnPointerClick(PointerEventData eventData)
    {
        if (IsEmpty())
            return;

        // 좌클릭 - 아이템 선택 또는 사용
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log($"슬롯 {slotIndex}의 아이템 선택: {item.ItemName}");

            // 아이템 사용 로직
            if (GameManager.Instance != null && GameManager.Instance.PlayerCharacter != null)
            {
                // 아이템 사용
                bool used = item.Use(GameManager.Instance.PlayerCharacter);

                // 소모품이고 사용 성공했으면 개수 감소
                if (used && item.Type == ItemData.ItemType.Consumable)
                {
                    if (item.RemoveAmount(1) && item.Amount <= 0)
                    {
                        // 수량이 0이 되면 슬롯 비우기
                        ClearSlot();
                    }
                    else
                    {
                        // 수량 업데이트
                        UpdateUI();
                    }
                }
            }
        }
        // 우클릭 - 아이템 정보 표시
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log($"슬롯 {slotIndex}의 아이템 정보: {item.ToString()}");

            // 아이템 정보창 표시 로직 (필요시 구현)
            // UIManager.Instance.ShowItemInfo(item);
        }
    }
}