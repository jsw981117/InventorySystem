using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UISlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image itemImage; // ������ �̹���
    [SerializeField] private Text countText; // ������ ���� �ؽ�Ʈ

    private int slotIndex; // ���� �ε���
    private Item item; // ������ ����

    // ���� �ʱ�ȭ �޼���
    public void InitSlot(int index)
    {
        slotIndex = index;
        ClearSlot();
    }

    // ���� ���� �޼���
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

    // ������ ���� �޼���
    public void SetItem(Item newItem)
    {
        item = newItem;
        UpdateUI();
    }

    // UI ������Ʈ �޼���
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

    // ������ ����ִ��� Ȯ���ϴ� �޼���
    public bool IsEmpty()
    {
        return item == null || item.IsEmpty();
    }

    // ������ ��������
    public Item GetItem()
    {
        return item;
    }

    // ������ ���� ��������
    public int GetItemAmount()
    {
        return item != null ? item.Amount : 0;
    }

    // ���� �ε��� ��������
    public int GetSlotIndex()
    {
        return slotIndex;
    }

    // Ŭ�� �̺�Ʈ ó��
    public void OnPointerClick(PointerEventData eventData)
    {
        if (IsEmpty())
            return;

        // ��Ŭ�� - ������ ���� �Ǵ� ���
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log($"���� {slotIndex}�� ������ ����: {item.ItemName}");

            // ������ ��� ����
            if (GameManager.Instance != null && GameManager.Instance.PlayerCharacter != null)
            {
                // ������ ���
                bool used = item.Use(GameManager.Instance.PlayerCharacter);

                // �Ҹ�ǰ�̰� ��� ���������� ���� ����
                if (used && item.Type == ItemData.ItemType.Consumable)
                {
                    if (item.RemoveAmount(1) && item.Amount <= 0)
                    {
                        // ������ 0�� �Ǹ� ���� ����
                        ClearSlot();
                    }
                    else
                    {
                        // ���� ������Ʈ
                        UpdateUI();
                    }
                }
            }
        }
        // ��Ŭ�� - ������ ���� ǥ��
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log($"���� {slotIndex}�� ������ ����: {item.ToString()}");

            // ������ ����â ǥ�� ���� (�ʿ�� ����)
            // UIManager.Instance.ShowItemInfo(item);
        }
    }
}