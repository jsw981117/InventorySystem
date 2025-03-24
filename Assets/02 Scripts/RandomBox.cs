using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// �����ڽ� ����� �����ϱ� ���� �̺�Ʈ Ŭ����
[System.Serializable]
public class RandomBoxResultEvent : UnityEvent<string> { }

public class RandomBox : MonoBehaviour
{
    [Header("�����ڽ� ������ Ǯ")]
    [SerializeField] private List<ItemData> possibleItems = new List<ItemData>(); // ȹ�� ������ ������ ���

    [Header("������ ȹ�� Ȯ��")]
    [SerializeField] private List<float> itemWeights = new List<float>(); // �� �������� ȹ�� Ȯ�� (0~100)

    [Header("�����ڽ� ����")]
    [SerializeField] private int minItemCount = 1; // �ּ� ȹ�� ������ ����
    [SerializeField] private int maxItemCount = 3; // �ִ� ȹ�� ������ ����

    // ��� �̺�Ʈ - UI���� �� �̺�Ʈ�� �����Ͽ� ��� ǥ��
    [HideInInspector] public RandomBoxResultEvent OnRandomBoxResult = new RandomBoxResultEvent();

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
            OnRandomBoxResult.Invoke("�����ڽ��� �������� ��ϵ��� �ʾҽ��ϴ�.");
            return;
        }

        // �÷��̾� ĳ���� Ȯ��
        Character playerCharacter = GameManager.Instance?.PlayerCharacter;
        if (playerCharacter == null)
        {
            OnRandomBoxResult.Invoke("�÷��̾� ĳ���͸� ã�� �� �����ϴ�.");
            return;
        }

        // �κ��丮 ���� Ȯ��
        if (playerCharacter.IsInventoryFull)
        {
            OnRandomBoxResult.Invoke("�κ��丮�� ���� á���ϴ�. ������ Ȯ���� �� �ٽ� �õ��ϼ���.");
            return;
        }

        // ȹ���� ������ ���� ����
        int itemCount = Random.Range(minItemCount, maxItemCount + 1);

        // ��� �޽���
        string resultMessage = "ȹ���� ������:\n";

        // ȹ���� ������ ���
        List<ItemResult> acquiredItems = new List<ItemResult>();

        // ������ ������ŭ ������ ȹ��
        for (int i = 0; i < itemCount; i++)
        {
            // �κ��丮 ���� ��Ȯ��
            if (playerCharacter.IsInventoryFull)
            {
                resultMessage += "\n�κ��丮�� ���� á���ϴ�.";
                break;
            }

            // ������ ����
            ItemData selectedItem = GetRandomItem();

            if (selectedItem != null)
            {
                // ������ ���� ���� (��ø ������ �������� 1~3��, �Ұ����� �������� 1��)
                int amount = selectedItem.IsStackable ? Random.Range(1, 4) : 1;

                // ĳ���Ϳ� ������ �߰�
                playerCharacter.AddItem(selectedItem, amount);

                // ��� ��Ͽ� �߰�
                acquiredItems.Add(new ItemResult
                {
                    Item = selectedItem,
                    Amount = amount
                });

                // ��� �޽����� �߰�
                resultMessage += $"- {selectedItem.ItemName} x{amount}\n";
            }
        }

        // ��� �̺�Ʈ �߻�
        OnRandomBoxResult.Invoke(resultMessage);
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

// ������ ȹ�� ����� �����ϴ� Ŭ����
[System.Serializable]
public class ItemResult
{
    public ItemData Item;
    public int Amount;
}