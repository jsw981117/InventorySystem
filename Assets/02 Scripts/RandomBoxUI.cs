using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomBoxUI : MonoBehaviour
{
    [Header("UI 컴포넌트")]
    [SerializeField] private Button openBoxButton; // 랜덤박스 열기 버튼
    //[SerializeField] private GameObject resultPanel; // 결과 패널
    //[SerializeField] private Text resultText; // 결과 텍스트
    //[SerializeField] private Button closeResultButton; // 닫기 버튼

    //[Header("시각 효과")]
    //[SerializeField] private Image boxImage; // 랜덤박스 이미지
    //[SerializeField] private Animator boxAnimator; // 애니메이션을 위한 Animator (선택사항)
    //[SerializeField] private string openAnimationTrigger = "Open"; // 애니메이션 트리거 이름

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
                Debug.LogError("RandomBox 컴포넌트를 찾을 수 없습니다.");
                return;
            }
        }

        // 버튼 이벤트 설정
        if (openBoxButton != null)
        {
            openBoxButton.onClick.AddListener(OnOpenBoxButtonClicked);
        }

        //if (closeResultButton != null)
        //{
        //    closeResultButton.onClick.AddListener(CloseResultPanel);
        //}

        //// 결과 이벤트 등록
        //randomBox.OnRandomBoxResult.AddListener(ShowResult);

        //// 결과 패널 초기 상태
        //if (resultPanel != null)
        //{
        //    resultPanel.SetActive(false);
        //}
    }

    // 랜덤박스 열기 버튼 클릭
    private void OnOpenBoxButtonClicked()
    {
        //// 애니메이션이 있으면 재생
        //if (boxAnimator != null)
        //{
        //    boxAnimator.SetTrigger(openAnimationTrigger);
        //    // 애니메이션 후 약간의 딜레이를 두고 실제 랜덤박스 열기
        //    StartCoroutine(OpenBoxAfterDelay(0.5f));
        //}
        //else
        //{
        //    // 애니메이션이 없으면 바로 실행
        //}
        randomBox.OpenRandomBox();

    }

    // 딜레이 후 랜덤박스 열기
    private IEnumerator OpenBoxAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        randomBox.OpenRandomBox();
    }

    //// 결과 표시
    //private void ShowResult(string message)
    //{
    //    if (resultPanel != null && resultText != null)
    //    {
    //        resultText.text = message;
    //        resultPanel.SetActive(true);
    //    }
    //    else
    //    {
    //        Debug.Log(message);
    //    }
    //}

    //// 결과 패널 닫기
    //private void CloseResultPanel()
    //{
    //    if (resultPanel != null)
    //    {
    //        resultPanel.SetActive(false);
    //    }
    //}

    //// 컴포넌트 제거 시 이벤트 구독 해제
    //private void OnDestroy()
    //{
    //    if (randomBox != null)
    //    {
    //        randomBox.OnRandomBoxResult.RemoveListener(ShowResult);
    //    }
    //}
}