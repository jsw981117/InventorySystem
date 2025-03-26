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

    /// <summary>
    /// ���� �ʱ�ȭ
    /// </summary>
    /// <param name="index"></param>
    public void InitSlot(int index)
    {
        slotIndex = index;
        ClearSlot();
    }

    /// <summary>
    /// ���� �ʱ�ȭ
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
    /// ���Կ� ������ ����
    /// </summary>
    /// <param name="newItem"></param>
    public void SetItem(Item newItem)
    {
        item = newItem;
        UpdateUI();
    }

    /// <summary>
    /// ���� UI ����
    /// </summary>
    public void UpdateUI()
    {
        if (item != null && !item.IsEmpty())
        {
            // ������ �̹��� ����
            if (itemImage != null)
            {
                itemImage.gameObject.SetActive(true);
                itemImage.sprite = item.ItemSprite;
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
                }
                else if (item.Amount > 1)
                {
                    // ���� ���� �������̰� ������ 2 �̻��� ��� ���� ǥ��
                    countBox.gameObject.SetActive(true);
                    countText.text = item.Amount.ToString();
                }
                else
                {
                    // ���� ���� �ƴϰų� ������ 1�̸� countBox ��Ȱ��ȭ
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
    /// ������ ����ִ��� Ȯ��
    /// </summary>
    /// <returns></returns>
    public bool IsEmpty()
    {
        return item == null || item.IsEmpty();
    }



    /// <summary>
    /// ���콺 Ŭ�� �̺�Ʈ ó��
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
            // ������ ���� ǥ��
            // ����� �ִ´ٸ� ���⿡...
        }
    }
}