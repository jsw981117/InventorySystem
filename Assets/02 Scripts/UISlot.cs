using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UISlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image itemImage; // 아이템 이미지
    [SerializeField] private TextMeshProUGUI countText; // 아이템 개수 텍스트

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
            Debug.Log("슬롯 비었음");
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

        // 좌클릭 - 아이템 장착/해제
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log($"슬롯 {slotIndex}의 아이템 선택: {item.ItemName}");

            // 장비 아이템인 경우에만 장착 가능
            if (GameManager.Instance != null && GameManager.Instance.PlayerCharacter != null &&
                (item.Type == ItemData.ItemType.Weapon ||
                 item.Type == ItemData.ItemType.Armor ||
                 item.Type == ItemData.ItemType.Accessory))
            {
                // 아이템 장착
                item.Use(GameManager.Instance.PlayerCharacter);
            }
        }
        // 우클릭 - 아이템 정보 표시
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log($"슬롯 {slotIndex}의 아이템 정보: {item.ToString()}");
        }
    }
}