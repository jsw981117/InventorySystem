using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UISlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image itemImage; // 아이템 이미지
    [SerializeField] private Image countBox; // 수량 표시 배경 이미지
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

        if (countBox != null)
        {
            countBox.gameObject.SetActive(false);
        }

        if (countText != null)
        {
            countText.text = "";
        }

        Debug.Log("슬롯 비움 완료");
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
            // 아이템 이미지 설정
            if (itemImage != null)
            {
                itemImage.gameObject.SetActive(true);
                itemImage.sprite = item.ItemSprite;
                Debug.Log($"아이템 이미지 설정: {item.ItemName}");
            }

            // 수량 또는 장착 표시 설정
            if (countBox != null && countText != null)
            {
                // 현재 이 아이템이 장착 중인지 확인
                bool isEquipped = false;
                if (GameManager.Instance != null && GameManager.Instance.PlayerCharacter != null)
                {
                    isEquipped = GameManager.Instance.PlayerCharacter.IsItemEquipped(item);
                }

                if (item.IsEquippable() && isEquipped)
                {
                    // 장착 중인 아이템인 경우만 "E" 표시
                    countBox.gameObject.SetActive(true);
                    countText.text = "E";
                    Debug.Log($"장비 아이템 E 표시: {item.ItemName} (장착 중)");
                }
                else if (item.Amount > 1)
                {
                    // 스택 가능 아이템이고 수량이 2 이상인 경우 수량 표시
                    countBox.gameObject.SetActive(true);
                    countText.text = item.Amount.ToString();
                    Debug.Log($"수량 표시: {item.ItemName} x{item.Amount}");
                }
                else
                {
                    // 장착 중이 아니거나 수량이 1이면 countBox 비활성화
                    countBox.gameObject.SetActive(false);
                    Debug.Log($"수량 1 또는 장착 중이 아닌 장비 - 표시 안함: {item.ItemName}");
                }
            }
            else
            {
                Debug.LogWarning("countBox 또는 countText가 null입니다");
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
                item.IsEquippable())
            {
                // 이미 장착 중인지 확인
                bool isEquipped = GameManager.Instance.PlayerCharacter.IsItemEquipped(item);

                if (isEquipped)
                {
                    // 이미 장착 중이면 해제
                    GameManager.Instance.PlayerCharacter.UnEquip(item.Type);
                    Debug.Log($"아이템 해제: {item.ItemName}");
                }
                else
                {
                    // 장착되지 않았다면 장착
                    item.Use(GameManager.Instance.PlayerCharacter);
                    Debug.Log($"아이템 장착: {item.ItemName}");
                }
            }
        }
        // 우클릭 - 아이템 정보 표시
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log($"슬롯 {slotIndex}의 아이템 정보: {item.ToString()}");
        }
    }
}