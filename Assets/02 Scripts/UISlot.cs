using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISlot : MonoBehaviour
{
    [SerializeField] private Image itemImage; // ������ �̹���
    [SerializeField] private TextMeshProUGUI countText; // ������ ���� �ؽ�Ʈ

    private int slotIndex; // ���� �ε���
    private Item currentItem; // ���� ������ (Item Ŭ���� ���� �ʿ�)
    private int itemCount = 0; // ������ ����

    // ���� �ʱ�ȭ �޼���
    public void InitSlot(int index)
    {
        slotIndex = index;
        ClearSlot();
    }

    // ���� ���� �޼���
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

    // ������ ���� �޼���
    public void SetItem(Item item, int count = 1)
    {
        currentItem = item;
        itemCount = count;

        UpdateUI();
    }

    // UI ������Ʈ �޼���
    public void UpdateUI()
    {
        if (currentItem != null)
        {
            if (itemImage != null)
            {
                itemImage.gameObject.SetActive(true);
                itemImage.sprite = currentItem.ItemSprite; // Item Ŭ������ ItemSprite ������Ƽ�� �ִٰ� ����
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

    // ������ ����ִ��� Ȯ���ϴ� �޼���
    public bool IsEmpty()
    {
        return currentItem == null;
    }

    // ���� ������ ��������
    public Item GetItem()
    {
        return currentItem;
    }

    // ������ ���� ��������
    public int GetItemCount()
    {
        return itemCount;
    }

    // ���� �ε��� ��������
    public int GetSlotIndex()
    {
        return slotIndex;
    }

    // Ŭ�� �̺�Ʈ ó��
    public void OnPointerClick(PointerEventData eventData)
    {
        // ��Ŭ�� - ������ ���� �Ǵ� ���
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (currentItem != null)
            {
                Debug.Log($"���� {slotIndex}�� ������ ����: {currentItem.ItemName}"); // Item Ŭ������ ItemName ������Ƽ�� �ִٰ� ����

                // ������ ��� ���� (�ʿ�� ����)
                // currentItem.Use();
            }
        }
        // ��Ŭ�� - ������ ���� ǥ��
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (currentItem != null)
            {
                Debug.Log($"���� {slotIndex}�� ������ ����: {currentItem.ItemName}");

                // ������ ����â ǥ�� ���� (�ʿ�� ����)
                // UIManager.Instance.ShowItemInfo(currentItem);
            }
        }
    }
}
