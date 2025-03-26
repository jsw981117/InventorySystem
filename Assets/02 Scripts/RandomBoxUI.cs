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
        if (randomBox == null)
        {
            randomBox = GetComponent<RandomBox>();

            if (randomBox == null)
            {
                return;
            }
        }

        if (openBoxButton != null)
        {
            openBoxButton.onClick.AddListener(OnOpenBoxButtonClicked);
        }
    }

    /// <summary>
    /// 랜덤박스 버튼 클릭
    /// </summary>
    private void OnOpenBoxButtonClicked()
    {
        randomBox.OpenRandomBox();
    }
}