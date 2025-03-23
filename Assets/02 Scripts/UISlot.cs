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
    private InventoryItem inventoryItem; // �κ��丮 ������ ����

    // ���� �ʱ�ȭ �޼���
    public void InitSlot(int index)
    {
        slotIndex = index;
        ClearSlot();
    }

    // ���� ���� �޼���
    public void ClearSlot()
    {
        inventoryItem = null;

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
    public void SetItem(InventoryItem invItem)
    {
        inventoryItem = invItem;
        UpdateUI();
    }

    // UI ������Ʈ �޼���
    public void UpdateUI()
    {
        if (inventoryItem != null && !inventoryItem.IsEmpty())
        {
            Item item = inventoryItem.Item;

            if (itemImage != null && item != null)
            {
                itemImage.gameObject.SetActive(true);
                itemImage.sprite = item.ItemSprite;
            }

            if (countText != null)
            {
                if (inventoryItem.Amount > 1)
                {
                    countText.gameObject.SetActive(true);
                    countText.text = inventoryItem.Amount.ToString();
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
        return inventoryItem == null || inventoryItem.IsEmpty();
    }

    // �κ��丮 ������ ��������
    public InventoryItem GetInventoryItem()
    {
        return inventoryItem;
    }

    // ������ ��������
    public Item GetItem()
    {
        return inventoryItem != null ? inventoryItem.Item : null;
    }

    // ������ ���� ��������
    public int GetItemAmount()
    {
        return inventoryItem != null ? inventoryItem.Amount : 0;
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
            Item item = GetItem();
            if (item != null)
            {
                Debug.Log($"���� {slotIndex}�� ������ ����: {item.ItemName}");

                // ������ ��� ����
                if (GameManager.Instance != null && GameManager.Instance.PlayerCharacter != null)
                {
                    // ������ ���
                    bool used = item.Use(GameManager.Instance.PlayerCharacter);

                    // �Ҹ�ǰ�̰� ��� ���������� ���� ����
                    if (used && item.Type == Item.ItemType.Consumable)
                    {
                        if (inventoryItem.RemoveAmount(1) && inventoryItem.Amount <= 0)
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
        }
        // ��Ŭ�� - ������ ���� ǥ��
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            Item item = GetItem();
            if (item != null)
            {
                Debug.Log($"���� {slotIndex}�� ������ ����: {item.ToString()}");

                // ������ ����â ǥ�� ���� (�ʿ�� ����)
                // UIManager.Instance.ShowItemInfo(item);
            }
        }
    }
}