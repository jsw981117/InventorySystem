using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomBox : MonoBehaviour
{
    [SerializeField] private Button openBoxButton; // �����ڽ� ���� ��ư

    [Header("�����ڽ� ������ Ǯ")]
    [SerializeField] private List<ItemData> possibleItems = new List<ItemData>(); // ȹ�� ������ ������ ���

    [Header("������ ȹ�� Ȯ��")]
    [SerializeField] private List<float> itemWeights = new List<float>(); // �� �������� ȹ�� Ȯ�� (0~100)

    [Header("�����ڽ� ����")]
    [SerializeField] private int minItemCount = 1; // �ּ� ȹ�� ������ ����
    [SerializeField] private int maxItemCount = 3; // �ִ� ȹ�� ������ ����

    [Header("UI ���")]
    [SerializeField] private GameObject resultPanel; // ��� �г�
    [SerializeField] private Text resultText; // ��� �ؽ�Ʈ

    private void Start()
    {
        // ��ư �̺�Ʈ ���
        if (openBoxButton != null)
        {
            openBoxButton.onClick.AddListener(OpenRandomBox);
        }

        // ��� �г� �ʱ� ��Ȱ��ȭ
        if (resultPanel != null)
        {
            resultPanel.SetActive(false);
        }

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
            Debug.LogError("�����ڽ��� �������� ��ϵ��� �ʾҽ��ϴ�.");
            return;
        }

        // �÷��̾� ĳ���� Ȯ��
        Character playerCharacter = GameManager.Instance?.PlayerCharacter;
        if (playerCharacter == null)
        {
            Debug.LogError("�÷��̾� ĳ���͸� ã�� �� �����ϴ�.");
            return;
        }

        // �κ��丮 ���� Ȯ��
        if (playerCharacter.IsInventoryFull)
        {
            ShowResult("�κ��丮�� ���� á���ϴ�. ������ Ȯ���� �� �ٽ� �õ��ϼ���.");
            return;
        }

        // ȹ���� ������ ���� ����
        int itemCount = Random.Range(minItemCount, maxItemCount + 1);

        // ��� �޽���
        string resultMessage = "ȹ���� ������:\n";

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

                // ��� �޽����� �߰�
                resultMessage += $"- {selectedItem.ItemName} x{amount}\n";
            }
        }

        // ��� ǥ��
        ShowResult(resultMessage);
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

    // ��� �г� ǥ��
    private void ShowResult(string message)
    {
        if (resultPanel != null && resultText != null)
        {
            resultText.text = message;
            resultPanel.SetActive(true);
        }
        else
        {
            Debug.Log(message);
        }
    }

    // ��� �г� �ݱ�
    public void CloseResultPanel()
    {
        if (resultPanel != null)
        {
            resultPanel.SetActive(false);
        }
    }
}