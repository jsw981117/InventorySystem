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
            Debug.LogWarning("아이템 목록과 확률 목록의 크기가 일치하지 않습니다. 모든 아이템에 동일한 확률이 적용됩니다.");
            itemWeights.Clear(); // 불일치할 경우 확률 목록 초기화
        }
    }

    // 랜덤박스 열기
    public void OpenRandomBox()
    {
        if (possibleItems.Count == 0)
        {
            Debug.LogWarning("랜덤박스에 아이템이 등록되지 않았습니다.");
            return;
        }

        // 플레이어 캐릭터 확인
        Character playerCharacter = GameManager.Instance?.PlayerCharacter;
        if (playerCharacter == null)
        {
            Debug.LogError("플레이어 캐릭터를 찾을 수 없습니다.");
            return;
        }

        // 획득할 아이템 개수 결정
        int itemCount = Random.Range(minItemCount, maxItemCount + 1);
        bool anyItemAdded = false;

        // 지정된 개수만큼 아이템 획득
        for (int i = 0; i < itemCount; i++)
        {
            // 아이템 선택
            ItemData selectedItem = GetRandomItem();

            if (selectedItem != null)
            {
                // 아이템 수량 결정 (중첩 가능한 아이템은 1~3개, 불가능한 아이템은 1개)
                int amount = selectedItem.IsStackable ? Random.Range(1, 4) : 1;

                // 캐릭터에 아이템 추가
                bool added = playerCharacter.AddItem(selectedItem, amount);

                if (added)
                {
                    Debug.Log($"획득: {selectedItem.ItemName} x{amount}");
                    anyItemAdded = true;
                }
                else
                {
                    Debug.LogWarning($"인벤토리가 가득 차서 {selectedItem.ItemName}을(를) 획득할 수 없습니다.");
                    break;
                }
            }
        }

        // 아이템이 추가되었고 인벤토리 UI가 현재 표시 중이면 강제 새로고침
        if (anyItemAdded && UIManager.Instance != null && UIManager.Instance.Inventory != null)
        {
            if (UIManager.Instance.Inventory.gameObject.activeSelf)
            {
                // 인벤토리 UI가 열려있는 상태면 강제로 새로고침
                UIManager.Instance.Inventory.ForceRefreshNow();
                Debug.Log("인벤토리 UI 강제 새로고침 완료");
            }
        }
    }

    // 가중치에 따라 랜덤 아이템 선택
    private ItemData GetRandomItem()
    {
        // 확률 목록이 비어있으면 모든 아이템에 동일한 확률 적용
        if (itemWeights.Count == 0)
        {
            int randomIndex = Random.Range(0, possibleItems.Count);
            return possibleItems[randomIndex];
        }

        // 가중치 적용 랜덤 선택
        float totalWeight = 0f;

        // 전체 가중치 합계 계산
        for (int i = 0; i < itemWeights.Count; i++)
        {
            totalWeight += itemWeights[i];
        }

        // 랜덤 값 생성
        float randomValue = Random.Range(0f, totalWeight);
        float currentWeight = 0f;

        // 가중치에 따라 아이템 선택
        for (int i = 0; i < possibleItems.Count; i++)
        {
            currentWeight += itemWeights[i];

            if (randomValue <= currentWeight)
            {
                return possibleItems[i];
            }
        }

        // 기본 반환 (첫 번째 아이템)
        return possibleItems[0];
    }
}