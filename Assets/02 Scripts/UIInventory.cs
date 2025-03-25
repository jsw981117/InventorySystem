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

        // �κ��丮�� �ִ� ������ ������ŭ ���� ����
        Character playerCharacter = GameManager.Instance?.PlayerCharacter;
        if (playerCharacter != null)
        {
            // �κ��丮 ������ ��� ��������
            var inventoryItems = playerCharacter.Inventory;
            int itemCount = inventoryItems.Count;

            // �ּ��� �� ���� �ϳ��� ǥ��
            if (itemCount == 0)
                itemCount = 1;

            CreateSlots(itemCount);

            // ������ ������ ���� ä��� - ���Ⱑ �߿��� �κ�
            for (int i = 0; i < inventoryItems.Count; i++)
            {
                if (i < slots.Count)
                {
                    var inventoryItem = inventoryItems[i];
                    Item item = inventoryItem.Item;

                    // �߿�: ���⼭ �������� ������ ��������� ����
                    if (item.IsStackable)
                    {
                        // �����ۿ� �κ��丮�������� ������ ���� ����
                        item.SetAmount(inventoryItem.Amount);
                    }

                    // ���Կ� ������ ����
                    slots[i].SetItem(item);
                }
            }

            Debug.Log($"�κ��丮 UI �ʱ�ȭ �Ϸ� - ������ {inventoryItems.Count}��");
        }
        else
        {
            // ĳ���Ͱ� ������ �⺻ ���� 5�� ����
            CreateSlots(5);
            Debug.LogWarning("ĳ���͸� ã�� �� ���� �⺻ ���� 5���� �����մϴ�.");
        }
        UpdateSlotCountText();
    }

    // ������ ������ŭ ���� ����
    private void CreateSlots(int count)
    {
        for (int i = 0; i < count; i++)
        {
            UISlot newSlot = Instantiate(slotPrefab, slotsParent);
            newSlot.name = $"Slot_{i}";

            // ���� �ʱ�ȭ
            newSlot.InitSlot(i);

            // ����Ʈ�� �߰�
            slots.Add(newSlot);
        }
        UpdateSlotCountText();
        Debug.Log($"�κ��丮 �ʱ�ȭ �Ϸ�: {count}���� ���� ������");
    }

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
        UpdateSlotCountText();
    }

    void OnClickBackBtn()
    {
        gameObject.SetActive(false);
        if (UIManager.Instance != null && UIManager.Instance.MainMenu != null)
        {
            UIManager.Instance.MainMenu.ReActiveButtons();
        }
    }

    // �ܺο��� ���� ������ ���� ����Ʈ ������Ƽ
    public List<UISlot> Slots => slots;

    // �������� ���Կ� �߰��ϴ� �޼���
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

        // �� ���Կ� ������ ����
        newSlot.SetItem(item);

        Debug.Log($"�� ���� �߰�: {item.ItemName}");
        return true;
    }

    // �κ��丮 UI ������Ʈ �޼���
    public void UpdateInventoryUI()
    {
        // ���� ǥ�õ� ��� ���� ������Ʈ
        foreach (UISlot slot in slots)
        {
            slot.UpdateUI();
        }

        Debug.Log("�κ��丮 UI ���� ������Ʈ �Ϸ�");
    }

    // �κ��丮 ���� ���� �� ȣ���Ͽ� UI ���� ���ΰ�ħ
    public void RefreshInventory()
    {
        // �÷��̾� ĳ���� ���� Ȯ��
        Character playerCharacter = GameManager.Instance?.PlayerCharacter;
        if (playerCharacter == null)
            return;

        // ������ ����� ������
        List<Item> characterItems = new List<Item>();
        foreach (var invItem in playerCharacter.Inventory)
        {
            characterItems.Add(invItem.Item);
        }

        // �ʿ��� ���� �� ���
        int requiredSlots = Mathf.Max(1, characterItems.Count); // �ּ� 1�� ���� �ʿ�

        // ���� ���� ���� �ʿ��� ���� �� ��
        if (slots.Count < requiredSlots)
        {
            // ������ �����ϸ� �߰�
            int slotsToAdd = requiredSlots - slots.Count;
            for (int i = 0; i < slotsToAdd; i++)
            {
                UISlot newSlot = Instantiate(slotPrefab, slotsParent);
                newSlot.name = $"Slot_{slots.Count}";
                newSlot.InitSlot(slots.Count);
                slots.Add(newSlot);
            }
        }
        else if (slots.Count > requiredSlots)
        {
            // ������ �ʹ� ������ ���� (������)
            // ���⼭�� ������ �����Ͽ� UI �������� ������
        }

        // ��� ���� �ʱ�ȭ
        foreach (UISlot slot in slots)
        {
            slot.ClearSlot();
        }

        // ������ ������ ���� ä���
        for (int i = 0; i < characterItems.Count; i++)
        {
            if (i < slots.Count)
            {
                slots[i].SetItem(characterItems[i]);
            }
        }

        Debug.Log($"�κ��丮 UI ������Ʈ �Ϸ� - ������ {characterItems.Count}��");
    }

    public void ForceRefreshNow()
    {
        // �÷��̾� ĳ���� ���� Ȯ��
        Character playerCharacter = GameManager.Instance?.PlayerCharacter;
        if (playerCharacter == null)
            return;

        // ���� ���� ���� ���� (��ũ�� ��ġ ���� �����ϱ� ����)
        Vector2 scrollPosition = Vector2.zero;
        if (scrollView != null)
        {
            scrollPosition = scrollView.normalizedPosition;
        }

        // ���� �ʱ�ȭ (���� ���� ����)
        foreach (UISlot slot in slots)
        {
            if (slot != null)
            {
                slot.ClearSlot();
            }
        }

        // �κ��丮 ���������� ���� ä���
        var inventoryItems = playerCharacter.Inventory;
        int neededSlots = Mathf.Max(inventoryItems.Count, slots.Count);

        // ������ �����ϸ� �߰�
        if (slots.Count < neededSlots)
        {
            int slotsToAdd = neededSlots - slots.Count;
            for (int i = 0; i < slotsToAdd; i++)
            {
                UISlot newSlot = Instantiate(slotPrefab, slotsParent);
                newSlot.name = $"Slot_{slots.Count}";
                newSlot.InitSlot(slots.Count);
                slots.Add(newSlot);
            }
        }

        // ������ ������ ���� ä���
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            if (i < slots.Count)
            {
                var inventoryItem = inventoryItems[i];
                Item item = inventoryItem.Item;

                // ������ ���� ����ȭ
                if (item.IsStackable)
                {
                    item.SetAmount(inventoryItem.Amount);
                }

                // ���Կ� ������ ����
                slots[i].SetItem(item);
            }
        }

        // ��ũ�� ��ġ ����
        if (scrollView != null)
        {
            scrollView.normalizedPosition = scrollPosition;
        }

        Debug.Log($"�κ��丮 UI ���� ���ΰ�ħ �Ϸ� - ������ {inventoryItems.Count}��, ���� {slots.Count}��");
        UpdateSlotCountText();
    }

    private void UpdateSlotCountText()
    {
        if (currentSlotCount != null)
        {
            currentSlotCount.text = slots.Count.ToString();
        }
    }
}