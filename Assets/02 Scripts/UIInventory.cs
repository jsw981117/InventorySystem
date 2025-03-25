using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIInventory : MonoBehaviour
{
    [SerializeField] private Button backBtn;
    [SerializeField] private UISlot slotPrefab; // UISlot ������ ����
    [SerializeField] private Transform slotsParent; // ���Ե��� �θ� Transform (ScrollView�� Content)
    [SerializeField] private ScrollRect scrollView; // ��ũ�Ѻ� ����
    [SerializeField] private TextMeshProUGUI currentSlotCount;

    private List<UISlot> slots = new List<UISlot>(); // UISlot ����Ʈ
    private Vector2 lastScrollPosition = Vector2.zero; // ������ ��ũ�� ��ġ ����

    void Start()
    {
        gameObject.SetActive(false);

        if (backBtn != null)
            backBtn.onClick.AddListener(OnClickBackBtn);

        // �ʿ��� ������Ʈ ã��
        InitializeComponents();
    }

    /// <summary>
    /// �ʿ��� UI ������Ʈ���� �ʱ�ȭ
    /// </summary>
    private void InitializeComponents()
    {
        // ScrollView ������� ã��
        if (scrollView == null)
            scrollView = GetComponentInChildren<ScrollRect>();

        // Content ã��
        if (slotsParent == null && scrollView != null)
            slotsParent = scrollView.content;

        // ���� �θ� ������ ���� ������Ʈ�� �θ�� ���
        if (slotsParent == null)
            slotsParent = transform;
    }

    /// <summary>
    /// �κ��丮 UI �ʱ�ȭ �� ǥ��
    /// </summary>
    public void ShowInventory()
    {
        // �κ��丮 UI ǥ��
        gameObject.SetActive(true);

        // �κ��丮 ���� ����
        RefreshInventory(false);
    }

    /// <summary>
    /// �κ��丮 UI�� ����
    /// </summary>
    /// <param name="preserveScrollPosition">��ũ�� ��ġ ���� ����</param>
    public void RefreshInventory(bool preserveScrollPosition = false)
    {
        // �÷��̾� ĳ���� ���� Ȯ��
        Character playerCharacter = GameManager.Instance?.PlayerCharacter;
        if (playerCharacter == null)
            return;

        // ��ũ�� ��ġ ����
        if (preserveScrollPosition && scrollView != null)
            lastScrollPosition = scrollView.normalizedPosition;

        // �ʿ��� ���� ���� ���
        var inventory = playerCharacter.Inventory;
        int requiredSlots = Mathf.Max(1, inventory.Count); // �ּ� 1�� ���� �ʿ�

        // ���� ���� ����
        AdjustSlotCount(requiredSlots);

        // ��� ���� �ʱ�ȭ
        ClearAllSlots();

        // ������ ������ ���� ä���
        PopulateSlots(inventory);

        // ��ũ�� ��ġ ����
        if (preserveScrollPosition && scrollView != null)
            scrollView.normalizedPosition = lastScrollPosition;

        UpdateSlotCountText();
    }

    /// <summary>
    /// �ʿ��� ���� ������ ����
    /// </summary>
    /// <param name="requiredSlots">�ʿ��� ���� ����</param>
    private void AdjustSlotCount(int requiredSlots)
    {
        // ���� ������ Ȯ��
        if (slotPrefab == null)
            return;

        // ������ �����ϸ� �߰�
        if (slots.Count < requiredSlots)
        {
            int slotsToAdd = requiredSlots - slots.Count;
            CreateSlots(slotsToAdd);
        }
    }

    /// <summary>
    /// ������ ������ŭ ���� ����
    /// </summary>
    /// <param name="count">������ ���� ����</param>
    private void CreateSlots(int count)
    {
        for (int i = 0; i < count; i++)
        {
            UISlot newSlot = Instantiate(slotPrefab, slotsParent);
            newSlot.name = $"Slot_{slots.Count}";
            newSlot.InitSlot(slots.Count);
            slots.Add(newSlot);
        }
    }

    /// <summary>
    /// ��� ������ ���
    /// </summary>
    private void ClearAllSlots()
    {
        foreach (UISlot slot in slots)
        {
            slot.ClearSlot();
        }
    }

    /// <summary>
    /// ���Ե��� ������ ������ ä��
    /// </summary>
    /// <param name="inventory">�κ��丮 ������ ���</param>
    private void PopulateSlots(IReadOnlyList<Item> inventory)
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (i < slots.Count)
            {
                slots[i].SetItem(inventory[i]);
            }
        }
    }

    /// <summary>
    /// ��� ���� ���� �� �ʱ�ȭ
    /// </summary>
    public void ClearSlots()
    {
        foreach (UISlot slot in slots)
        {
            if (slot != null)
                Destroy(slot.gameObject);
        }

        slots.Clear();
        UpdateSlotCountText();
    }

    /// <summary>
    /// �ڷΰ��� ��ư Ŭ�� ó��
    /// </summary>
    private void OnClickBackBtn()
    {
        gameObject.SetActive(false);
        if (UIManager.Instance != null && UIManager.Instance.MainMenu != null)
        {
            UIManager.Instance.MainMenu.ReActiveButtons();
        }
    }

    /// <summary>
    /// ���� ���� �ؽ�Ʈ ������Ʈ
    /// </summary>
    private void UpdateSlotCountText()
    {
        if (currentSlotCount != null)
        {
            currentSlotCount.text = slots.Count.ToString();
        }
    }

    /// <summary>
    /// �κ��丮 ���� ����Ʈ ��ȯ
    /// </summary>
    public List<UISlot> Slots => slots;

    /// <summary>
    /// �������� �� ���Կ� �߰�
    /// </summary>
    /// <param name="item">�߰��� ������</param>
    /// <returns>�߰� ���� ����</returns>
    public bool AddItemToSlot(Item item)
    {
        if (item == null || item.IsEmpty())
            return false;

        // �� ���� ã��
        foreach (UISlot slot in slots)
        {
            if (slot.IsEmpty())
            {
                slot.SetItem(item);
                return true;
            }
        }

        // �� ������ ������ �� ���� �߰�
        UISlot newSlot = Instantiate(slotPrefab, slotsParent);
        newSlot.name = $"Slot_{slots.Count}";
        newSlot.InitSlot(slots.Count);
        slots.Add(newSlot);
        newSlot.SetItem(item);

        UpdateSlotCountText();
        return true;
    }

    /// <summary>
    /// ���� ǥ�õ� ��� ���� UI ������Ʈ
    /// </summary>
    public void UpdateInventoryUI()
    {
        foreach (UISlot slot in slots)
        {
            slot.UpdateUI();
        }
    }

    /// <summary>
    /// �κ��丮 UI ���� ����
    /// </summary>
    public void ForceRefreshNow()
    {
        RefreshInventory(true);
    }
}