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

    /// <summary>
    /// 슬롯 초기화
    /// </summary>
    /// <param name="index"></param>
    public void InitSlot(int index)
    {
        slotIndex = index;
        ClearSlot();
    }

    /// <summary>
    /// 슬롯 초기화
    /// </summary>
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

    }

    /// <summary>
    /// 슬롯에 아이템 설정
    /// </summary>
    /// <param name="newItem"></param>
    public void SetItem(Item newItem)
    {
        item = newItem;
        UpdateUI();
    }

    /// <summary>
    /// 슬롯 UI 갱신
    /// </summary>
    public void UpdateUI()
    {
        if (item != null && !item.IsEmpty())
        {
            // 아이템 이미지 설정
            if (itemImage != null)
            {
                itemImage.gameObject.SetActive(true);
                itemImage.sprite = item.ItemSprite;
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
                }
                else if (item.Amount > 1)
                {
                    // 스택 가능 아이템이고 수량이 2 이상인 경우 수량 표시
                    countBox.gameObject.SetActive(true);
                    countText.text = item.Amount.ToString();
                }
                else
                {
                    // 장착 중이 아니거나 수량이 1이면 countBox 비활성화
                    countBox.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            ClearSlot();
        }
    }

    /// <summary>
    /// 슬롯이 비어있는지 확인
    /// </summary>
    /// <returns></returns>
    public bool IsEmpty()
    {
        return item == null || item.IsEmpty();
    }



    /// <summary>
    /// 마우스 클릭 이벤트 처리
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (IsEmpty())
            return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {

            if (GameManager.Instance != null && GameManager.Instance.PlayerCharacter != null &&
                item.IsEquippable())
            {
                bool isEquipped = GameManager.Instance.PlayerCharacter.IsItemEquipped(item);

                if (isEquipped)
                {
                    GameManager.Instance.PlayerCharacter.UnEquip(item.Type);
                }
                else
                {
                    item.Use(GameManager.Instance.PlayerCharacter);
                }
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            // 아이템 정보 표시
            // 기능을 넣는다면 여기에...
        }
    }
}