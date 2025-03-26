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
            itemWeights.Clear(); // ����ġ�� ��� Ȯ�� ��� �ʱ�ȭ
        }
    }

    /// <summary>
    /// �����ڽ� ����
    /// </summary>
    public void OpenRandomBox()
    {
        if (possibleItems.Count == 0)
        {
            return;
        }

        Character playerCharacter = GameManager.Instance?.PlayerCharacter;
        if (playerCharacter == null)
        {
            return;
        }

        int itemCount = Random.Range(minItemCount, maxItemCount + 1);
        bool anyItemAdded = false;

        for (int i = 0; i < itemCount; i++)
        {
            ItemData selectedItem = GetRandomItem();

            if (selectedItem != null)
            {
                int amount = selectedItem.IsStackable ? Random.Range(1, 10) : 1;
                bool added = playerCharacter.AddItem(selectedItem, amount);

                if (added)
                {
                    anyItemAdded = true;
                }
                else
                {
                    break;
                }
            }
        }

        if (anyItemAdded && UIManager.Instance != null && UIManager.Instance.Inventory != null)
        {
            if (UIManager.Instance.Inventory.gameObject.activeSelf)
            {
                UIManager.Instance.Inventory.ForceRefreshNow();
            }
        }
    }

    /// <summary>
    /// �������� ������ �̱�
    /// </summary>
    /// <returns></returns>
    private ItemData GetRandomItem()
    {
        // Ȯ�� ����� ��������� ��� �����ۿ� ������ Ȯ�� ����
        if (itemWeights.Count == 0)
        {
            int randomIndex = Random.Range(0, possibleItems.Count);
            return possibleItems[randomIndex];
        }

        float totalWeight = 0f;
        for (int i = 0; i < itemWeights.Count; i++)
        {
            totalWeight += itemWeights[i];
        }
        float randomValue = Random.Range(0f, totalWeight);
        float currentWeight = 0f;

        for (int i = 0; i < possibleItems.Count; i++)
        {
            currentWeight += itemWeights[i];

            if (randomValue <= currentWeight)
            {
                return possibleItems[i];
            }
        }
        return possibleItems[0];
    }
}