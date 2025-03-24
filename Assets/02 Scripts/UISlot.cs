using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UISlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image itemImage; // ������ �̹���
    [SerializeField] private Image countBox; // ���� ǥ�� ��� �̹���
    [SerializeField] private TextMeshProUGUI countText; // ������ ���� �ؽ�Ʈ

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

        if (countBox != null)
        {
            countBox.gameObject.SetActive(false);
        }

        if (countText != null)
        {
            countText.text = "";
        }

        Debug.Log("���� ��� �Ϸ�");
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
            // ������ �̹��� ����
            if (itemImage != null)
            {
                itemImage.gameObject.SetActive(true);
                itemImage.sprite = item.ItemSprite;
                Debug.Log($"������ �̹��� ����: {item.ItemName}");
            }

            // ���� �Ǵ� ���� ǥ�� ����
            if (countBox != null && countText != null)
            {
                // ���� �� �������� ���� ������ Ȯ��
                bool isEquipped = false;
                if (GameManager.Instance != null && GameManager.Instance.PlayerCharacter != null)
                {
                    isEquipped = GameManager.Instance.PlayerCharacter.IsItemEquipped(item);
                }

                if (item.IsEquippable() && isEquipped)
                {
                    // ���� ���� �������� ��츸 "E" ǥ��
                    countBox.gameObject.SetActive(true);
                    countText.text = "E";
                    Debug.Log($"��� ������ E ǥ��: {item.ItemName} (���� ��)");
                }
                else if (item.Amount > 1)
                {
                    // ���� ���� �������̰� ������ 2 �̻��� ��� ���� ǥ��
                    countBox.gameObject.SetActive(true);
                    countText.text = item.Amount.ToString();
                    Debug.Log($"���� ǥ��: {item.ItemName} x{item.Amount}");
                }
                else
                {
                    // ���� ���� �ƴϰų� ������ 1�̸� countBox ��Ȱ��ȭ
                    countBox.gameObject.SetActive(false);
                    Debug.Log($"���� 1 �Ǵ� ���� ���� �ƴ� ��� - ǥ�� ����: {item.ItemName}");
                }
            }
            else
            {
                Debug.LogWarning("countBox �Ǵ� countText�� null�Դϴ�");
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

        // ��Ŭ�� - ������ ����/����
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log($"���� {slotIndex}�� ������ ����: {item.ItemName}");

            // ��� �������� ��쿡�� ���� ����
            if (GameManager.Instance != null && GameManager.Instance.PlayerCharacter != null &&
                item.IsEquippable())
            {
                // �̹� ���� ������ Ȯ��
                bool isEquipped = GameManager.Instance.PlayerCharacter.IsItemEquipped(item);

                if (isEquipped)
                {
                    // �̹� ���� ���̸� ����
                    GameManager.Instance.PlayerCharacter.UnEquip(item.Type);
                    Debug.Log($"������ ����: {item.ItemName}");
                }
                else
                {
                    // �������� �ʾҴٸ� ����
                    item.Use(GameManager.Instance.PlayerCharacter);
                    Debug.Log($"������ ����: {item.ItemName}");
                }
            }
        }
        // ��Ŭ�� - ������ ���� ǥ��
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log($"���� {slotIndex}�� ������ ����: {item.ToString()}");
        }
    }
}