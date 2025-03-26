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

    [Header("������Ʈ Ǯ�� ����")]
    [SerializeField] private int initialPoolSize = 30; // �ʱ� Ǯ ũ��

    private List<UISlot> pooledSlots = new List<UISlot>(); // ��ü ���� Ǯ
    private List<UISlot> activeSlots = new List<UISlot>(); // ���� Ȱ��ȭ�� ���Ե�
    private Vector2 lastScrollPosition = Vector2.zero; // ������ ��ũ�� ��ġ ����

    void Start()
    {
        gameObject.SetActive(false);

        if (backBtn != null)
            backBtn.onClick.AddListener(OnClickBackBtn);

        // �ʿ��� ������Ʈ ã��
        InitializeComponents();

        // ���� Ǯ �ʱ�ȭ
        InitializeSlotPool();
    }

    /// <summary>
    /// �ʿ��� UI ������Ʈ���� �ʱ�ȭ
    /// </summary>
    private void InitializeComponents()
    {
        if (scrollView == null)
            scrollView = GetComponentInChildren<ScrollRect>();

        if (slotsParent == null && scrollView != null)
            slotsParent = scrollView.content;

        if (slotsParent == null)
            slotsParent = transform;
    }

    /// <summary>
    /// ���� ������Ʈ Ǯ �ʱ�ȭ
    /// </summary>
    private void InitializeSlotPool()
    {
        if (slotPrefab == null)
            return;

        // �ʱ� ���� Ǯ ����
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateSlotInPool(i);
        }
    }

    /// <summary>
    /// Ǯ�� �� ���� ����
    /// </summary>
    private UISlot CreateSlotInPool(int index)
    {
        UISlot newSlot = Instantiate(slotPrefab, slotsParent);
        newSlot.name = $"Slot_{index}";
        newSlot.InitSlot(index);
        newSlot.gameObject.SetActive(false);
        pooledSlots.Add(newSlot);
        return newSlot;
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
        Character playerCharacter = GameManager.Instance?.PlayerCharacter;
        if (playerCharacter == null)
            return;

        if (preserveScrollPosition && scrollView != null)
            lastScrollPosition = scrollView.normalizedPosition;

        var inventory = playerCharacter.Inventory;
        int requiredSlots = Mathf.Max(0, inventory.Count);

        // �ʿ��� ��ŭ�� ������ Ȱ��ȭ
        AdjustActiveSlotCount(requiredSlots);

        // ��� Ȱ�� ���� �ʱ�ȭ
        ClearAllActiveSlots();

        // ���Կ� ������ ���� ä���
        PopulateSlots(inventory);

        if (preserveScrollPosition && scrollView != null)
            scrollView.normalizedPosition = lastScrollPosition;

        UpdateSlotCountText();
    }

    /// <summary>
    /// �ʿ��� ���� ������ ���� (Ȱ��ȭ/��Ȱ��ȭ)
    /// </summary>
    /// <param name="requiredSlots">�ʿ��� ���� ����</param>
    private void AdjustActiveSlotCount(int requiredSlots)
    {
        // Ȱ�� ���� �� ����
        if (activeSlots.Count < requiredSlots)
        {
            // �� �ʿ��� ��� ���� Ȱ��ȭ
            int slotsToActivate = requiredSlots - activeSlots.Count;
            ActivateMoreSlots(slotsToActivate);
        }
        else if (activeSlots.Count > requiredSlots)
        {
            // ������ ���� ��Ȱ��ȭ
            int slotsToDeactivate = activeSlots.Count - requiredSlots;
            DeactivateExcessSlots(slotsToDeactivate);
        }
    }

    /// <summary>
    /// �߰� ���� Ȱ��ȭ
    /// </summary>
    /// <param name="count">Ȱ��ȭ�� ���� ��</param>
    private void ActivateMoreSlots(int count)
    {
        for (int i = 0; i < count; i++)
        {
            UISlot slot = GetInactiveSlot();
            if (slot != null)
            {
                // ��Ȱ�� ���� Ȱ��ȭ
                slot.gameObject.SetActive(true);
                activeSlots.Add(slot);
            }
        }
    }

    /// <summary>
    /// ��Ȱ�� ������ ������ ã�� ��ȯ
    /// </summary>
    /// <returns>��Ȱ�� ���� (������ ���� ����)</returns>
    private UISlot GetInactiveSlot()
    {
        // ��Ȱ�� ���� ã��
        foreach (UISlot slot in pooledSlots)
        {
            if (!slot.gameObject.activeSelf && !activeSlots.Contains(slot))
            {
                return slot;
            }
        }

        // �� ã���� ���� ����
        return CreateSlotInPool(pooledSlots.Count);
    }

    /// <summary>
    /// ������ ���� ��Ȱ��ȭ
    /// </summary>
    /// <param name="count">��Ȱ��ȭ�� ���� ��</param>
    private void DeactivateExcessSlots(int count)
    {
        if (count <= 0 || activeSlots.Count == 0)
            return;

        // ��Ȱ��ȭ�� ���� �� ����
        count = Mathf.Min(count, activeSlots.Count);

        // �ڿ������� ��Ȱ��ȭ (�ֱٿ� �߰��� ���Ժ���)
        for (int i = 0; i < count; i++)
        {
            int lastIndex = activeSlots.Count - 1;
            UISlot slot = activeSlots[lastIndex];

            // ���� ��Ȱ��ȭ �� �ʱ�ȭ
            slot.ClearSlot();
            slot.gameObject.SetActive(false);

            // Ȱ�� ���� ����Ʈ���� ����
            activeSlots.RemoveAt(lastIndex);
        }
    }

    /// <summary>
    /// ��� Ȱ�� ������ ���
    /// </summary>
    private void ClearAllActiveSlots()
    {
        foreach (UISlot slot in activeSlots)
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
        for (int i = 0; i < inventory.Count && i < activeSlots.Count; i++)
        {
            activeSlots[i].SetItem(inventory[i]);
        }
    }

    /// <summary>
    /// ��� ���� Ǯ�� ��ȯ �� �ʱ�ȭ
    /// </summary>
    public void ClearSlots()
    {
        foreach (UISlot slot in activeSlots)
        {
            if (slot != null)
            {
                slot.ClearSlot();
                slot.gameObject.SetActive(false);
            }
        }

        activeSlots.Clear();
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
            currentSlotCount.text = activeSlots.Count.ToString();
        }
    }

    /// <summary>
    /// Ȱ��ȭ�� �κ��丮 ���� ����Ʈ ��ȯ
    /// </summary>
    public List<UISlot> Slots => activeSlots;

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
        foreach (UISlot existingSlot in activeSlots)
        {
            if (existingSlot.IsEmpty())
            {
                existingSlot.SetItem(item);
                return true;
            }
        }

        // �� ������ ������ �� ���� Ȱ��ȭ
        UISlot newSlot = GetInactiveSlot();
        newSlot.gameObject.SetActive(true);
        activeSlots.Add(newSlot);
        newSlot.SetItem(item);

        UpdateSlotCountText();
        return true;
    }

    /// <summary>
    /// ���� ǥ�õ� ��� ���� UI ������Ʈ
    /// </summary>
    public void UpdateInventoryUI()
    {
        foreach (UISlot slot in activeSlots)
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