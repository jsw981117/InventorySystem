using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInventory : MonoBehaviour
{
    [SerializeField] private Button backBtn;
    [SerializeField] private UISlot slotPrefab; // UISlot ������ ����
    [SerializeField] private Transform slotsParent; // ���Ե��� �θ� Transform (ScrollView�� Content)
    [SerializeField] private ScrollRect scrollView; // ��ũ�Ѻ� ����

    private List<UISlot> slots = new List<UISlot>(); // UISlot ����Ʈ

    [SerializeField] private int slotCount = 20; // �⺻ ���� ���� (�ʿ信 ���� ����)

    void Start()
    {
        gameObject.SetActive(false);

        if (backBtn != null)
            backBtn.onClick.AddListener(OnClickBackBtn);

        // ScrollView ������� ã��
        if (scrollView == null)
            scrollView = GetComponentInChildren<ScrollRect>();

        // Content ã��
        if (slotsParent == null && scrollView != null)
            slotsParent = scrollView.content;

        InitInventoryUI();
    }

    // �κ��丮 UI �ʱ�ȭ �޼���
    private void InitInventoryUI()
    {
        // ������ ������ ������ ������ ����
        ClearSlots();

        // ���� �θ� ������ ���� ������Ʈ�� �θ�� ���
        if (slotsParent == null)
        {
            Debug.LogWarning("���� �θ�(Content)�� �������� �ʾҽ��ϴ�. ���� ������Ʈ�� �θ�� ����մϴ�.");
            slotsParent = transform;
        }

        // ���� ������ �˻�
        if (slotPrefab == null)
        {
            Debug.LogError("UISlot �������� �Ҵ���� �ʾҽ��ϴ�!");
            return;
        }

        // ������ ������ŭ ���� ���� - ScrollView�� Content �Ʒ��� ����
        for (int i = 0; i < slotCount; i++)
        {
            UISlot newSlot = Instantiate(slotPrefab, slotsParent);
            newSlot.name = $"Slot_{i}";

            // ���� �ʱ�ȭ (���Կ� �ʱ�ȭ �޼��尡 �ִٸ� ȣ��)
            newSlot.InitSlot(i);

            // ����Ʈ�� �߰�
            slots.Add(newSlot);
        }

        // Content ũ�� ���� (�ʿ��)

        Debug.Log($"�κ��丮 �ʱ�ȭ �Ϸ�: {slotCount}���� ���� ������");
    }

    //// Content ũ�� ���� �޼���
    //private void AdjustContentSize()
    //{
    //    if (slotsParent != null)
    //    {
    //        // Content�� RectTransform �ִ��� Ȯ��
    //        RectTransform contentRect = slotsParent.GetComponent<RectTransform>();
    //        if (contentRect != null)
    //        {
    //            // GridLayoutGroup�� ������ �� ������ Ȱ��
    //            GridLayoutGroup gridLayout = slotsParent.GetComponent<GridLayoutGroup>();
    //            if (gridLayout != null)
    //            {
    //                // �� �ٿ� �� ���� ���� ���
    //                float contentWidth = contentRect.rect.width;
    //                float cellWidth = gridLayout.cellSize.x + gridLayout.spacing.x;
    //                int slotsPerRow = Mathf.Max(1, Mathf.FloorToInt(contentWidth / cellWidth));

    //                // �ʿ��� �� �� ���
    //                int rowsNeeded = Mathf.CeilToInt((float)slotCount / slotsPerRow);

    //                // Content ���� ��� �� ����
    //                float cellHeight = gridLayout.cellSize.y + gridLayout.spacing.y;
    //                float contentHeight = rowsNeeded * cellHeight + gridLayout.padding.top + gridLayout.padding.bottom;

    //                // �ּ� ���� (��ũ�Ѻ� ����)���� ���� �ʰ� ����
    //                RectTransform scrollRectTransform = scrollView?.GetComponent<RectTransform>();
    //                float minHeight = scrollRectTransform != null ? scrollRectTransform.rect.height : 100f;
    //                contentHeight = Mathf.Max(contentHeight, minHeight);

    //                // Content ���� ����
    //                contentRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, contentHeight);
    //            }
    //        }
    //    }
    //}

    // ���� ���� �޼���
    public void ClearSlots()
    {
        // �̹� ������ ���Ե� ����
        foreach (UISlot slot in slots)
        {
            if (slot != null)
                Destroy(slot.gameObject);
        }

        // ����Ʈ �ʱ�ȭ
        slots.Clear();
    }

    void OnClickBackBtn()
    {
        gameObject.SetActive(false);
        UIManager.Instance.MainMenu.ReActiveButtons();
    }

    // �ܺο��� ���� ������ ���� ����Ʈ ������Ƽ
    public List<UISlot> Slots => slots;

    // �������� ���Կ� �߰��ϴ� �޼���
    public bool AddItemToSlot(Item item)
    {
        if (item == null) return false;

        // �� ���� ã��
        foreach (UISlot slot in slots)
        {
            if (slot.IsEmpty())
            {
                slot.SetItem(item);
                return true;
            }
        }

        Debug.Log("��� ������ ���� á���ϴ�!");
        return false;
    }

    // �κ��丮 UI ������Ʈ �޼���
    public void UpdateInventoryUI()
    {
        foreach (UISlot slot in slots)
        {
            slot.UpdateUI();
        }
    }
}
