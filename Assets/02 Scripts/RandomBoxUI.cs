using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomBoxUI : MonoBehaviour
{
    [SerializeField] private RandomBox randomBox; // 랜덤박스 스크립트 참조
    [SerializeField] private Button openButton; // 열기 버튼
    //[SerializeField] private Button closeResultButton; // 결과 닫기 버튼

    //[SerializeField] private GameObject resultPanel; // 결과 패널
    //[SerializeField] private Text resultText; // 결과 텍스트

    void Start()
    {
        // 버튼 이벤트 설정
        if (openButton != null && randomBox != null)
        {
            openButton.onClick.AddListener(randomBox.OpenRandomBox);
        }

        //if (closeResultButton != null && randomBox != null)
        //{
        //    closeResultButton.onClick.AddListener(randomBox.CloseResultPanel);
        //}

        //// 결과 패널 초기 상태
        //if (resultPanel != null)
        //{
        //    resultPanel.SetActive(false);
        //}
    }
}