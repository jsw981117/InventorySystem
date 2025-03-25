using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBox : MonoBehaviour
{
    [Header("�����ڽ� ������ Ǯ")]
    [SerializeField] private List<ItemData> possibleItems = new List<ItemData>(); // ȹ�� ������ ������ ���

    [Header("������ ȹ�� Ȯ��")]
    [SerializeField] private List<float> itemWeights = new List<float>(); // �� �������� ȹ�� Ȯ�� (0~100)

    [Header("�����ڽ� ����")]
    [SerializeField] private int minItemCount = 1; // �ּ� ȹ�� ������ ����
    [SerializeField] private int maxItemCount = 3; // �ִ� ȹ�� ������ ����

    private void Start()
    {
        // Ȯ�� ����Ʈ ũ�� �˻�
        if (itemWeights.Count > 0 && itemWeights.Count != possibleItems.Count)
        {
            Debug.LogWarning("������ ��ϰ� Ȯ�� ����� ũ�Ⱑ ��ġ���� �ʽ��ϴ�. ��� �����ۿ� ������ Ȯ���� ����˴ϴ�.");
            itemWeights.Clear(); // ����ġ�� ��� Ȯ�� ��� �ʱ�ȭ
        }
    }

    // �����ڽ� ����
    public void OpenRandomBox()
    {
        if (possibleItems.Count == 0)
        {
            Debug.LogWarning("�����ڽ��� �������� ��ϵ��� �ʾҽ��ϴ�.");
            return;
        }

        // �÷��̾� ĳ���� Ȯ��
        Character playerCharacter = GameManager.Instance?.PlayerCharacter;
        if (playerCharacter == null)
        {
            Debug.LogError("�÷��̾� ĳ���͸� ã�� �� �����ϴ�.");
            return;
        }

        // ȹ���� ������ ���� ����
        int itemCount = Random.Range(minItemCount, maxItemCount + 1);
        bool anyItemAdded = false;

        // ������ ������ŭ ������ ȹ��
        for (int i = 0; i < itemCount; i++)
        {
            // ������ ����
            ItemData selectedItem = GetRandomItem();

            if (selectedItem != null)
            {
                // ������ ���� ���� (��ø ������ �������� 1~3��, �Ұ����� �������� 1��)
                int amount = selectedItem.IsStackable ? Random.Range(1, 4) : 1;

                // ĳ���Ϳ� ������ �߰�
                bool added = playerCharacter.AddItem(selectedItem, amount);

                if (added)
                {
                    Debug.Log($"ȹ��: {selectedItem.ItemName} x{amount}");
                    anyItemAdded = true;
                }
                else
                {
                    Debug.LogWarning($"�κ��丮�� ���� ���� {selectedItem.ItemName}��(��) ȹ���� �� �����ϴ�.");
                    break;
                }
            }
        }

        // �������� �߰��Ǿ��� �κ��丮 UI�� ���� ǥ�� ���̸� ���� ���ΰ�ħ
        if (anyItemAdded && UIManager.Instance != null && UIManager.Instance.Inventory != null)
        {
            if (UIManager.Instance.Inventory.gameObject.activeSelf)
            {
                // �κ��丮 UI�� �����ִ� ���¸� ������ ���ΰ�ħ
                UIManager.Instance.Inventory.ForceRefreshNow();
                Debug.Log("�κ��丮 UI ���� ���ΰ�ħ �Ϸ�");
            }
        }
    }

    // ����ġ�� ���� ���� ������ ����
    private ItemData GetRandomItem()
    {
        // Ȯ�� ����� ��������� ��� �����ۿ� ������ Ȯ�� ����
        if (itemWeights.Count == 0)
        {
            int randomIndex = Random.Range(0, possibleItems.Count);
            return possibleItems[randomIndex];
        }

        // ����ġ ���� ���� ����
        float totalWeight = 0f;

        // ��ü ����ġ �հ� ���
        for (int i = 0; i < itemWeights.Count; i++)
        {
            totalWeight += itemWeights[i];
        }

        // ���� �� ����
        float randomValue = Random.Range(0f, totalWeight);
        float currentWeight = 0f;

        // ����ġ�� ���� ������ ����
        for (int i = 0; i < possibleItems.Count; i++)
        {
            currentWeight += itemWeights[i];

            if (randomValue <= currentWeight)
            {
                return possibleItems[i];
            }
        }

        // �⺻ ��ȯ (ù ��° ������)
        return possibleItems[0];
    }
}