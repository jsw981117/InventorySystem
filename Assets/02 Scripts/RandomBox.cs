using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBox : MonoBehaviour
{
    [Header("랜덤박스 아이템 풀")]
    [SerializeField] private List<ItemData> possibleItems = new List<ItemData>(); // 획득 가능한 아이템 목록

    [Header("아이템 획득 확률")]
    [SerializeField] private List<float> itemWeights = new List<float>(); // 각 아이템의 획득 확률 (0~100)

    [Header("랜덤박스 설정")]
    [SerializeField] private int minItemCount = 1; // 최소 획득 아이템 개수
    [SerializeField] private int maxItemCount = 3; // 최대 획득 아이템 개수

    private void Start()
    {
        // 확률 리스트 크기 검사
        if (itemWeights.Count > 0 && itemWeights.Count != possibleItems.Count)
        {
            itemWeights.Clear(); // 불일치할 경우 확률 목록 초기화
        }
    }

    /// <summary>
    /// 랜덤박스 열기
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
    /// 랜덤으로 아이템 뽑기
    /// </summary>
    /// <returns></returns>
    private ItemData GetRandomItem()
    {
        // 확률 목록이 비어있으면 모든 아이템에 동일한 확률 적용
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