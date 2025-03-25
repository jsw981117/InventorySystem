using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomBoxUI : MonoBehaviour
{
    [Header("UI 컴포넌트")]
    [SerializeField] private Button openBoxButton; // 랜덤박스 열기 버튼

    [Header("랜덤박스 설정")]
    [SerializeField] private RandomBox randomBox; // RandomBox 스크립트 참조

    private void Start()
    {
        // 랜덤박스 스크립트 검사
        if (randomBox == null)
        {
            randomBox = GetComponent<RandomBox>();

            if (randomBox == null)
            {
                return;
            }
        }

        // 버튼 이벤트 설정
        if (openBoxButton != null)
        {
            openBoxButton.onClick.AddListener(OnOpenBoxButtonClicked);
        }
    }

    // 랜덤박스 열기 버튼 클릭
    private void OnOpenBoxButtonClicked()
    {
        randomBox.OpenRandomBox();
    }
}