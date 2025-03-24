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
            int itemCount = playerCharacter.InventorySize;

            // �ּ��� �� ���� �ϳ��� ǥ��
            if (itemCount == 0)
                itemCount = 1;

            CreateSlots(itemCount);

            // ������ ������ ���� ä���
            for (int i = 0; i < playerCharacter.Inventory.Count; i++)
            {
                if (i < slots.Count)
                {
                    slots[i].SetItem(playerCharacter.Inventory[i].Item);
                }
            }
        }
        else
        {
            // ĳ���Ͱ� ������ �⺻ ���� 5�� ����
            CreateSlots(5);
        }
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
    }

    void OnClickBackBtn()
    {
        gameObject.SetActive(false);
        UIManager.Instance.MainMenu.ReActiveButtons();
    }

    // �ܺο��� ���� ������ ���� ����Ʈ ������Ƽ
    public List<UISlot> Slots => slots;

    // �������� ���Կ� �߰��ϴ� �޼��� - ����ִ� ���Կ� ������ �߰�
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
    }

    // �κ��丮 ���� ���� �� ȣ���Ͽ� UI ���� ���ΰ�ħ
    public void RefreshInventory()
    {
        InitInventoryUI();
    }
}