using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomBoxUI : MonoBehaviour
{
    [Header("UI ������Ʈ")]
    [SerializeField] private Button openBoxButton; // �����ڽ� ���� ��ư
    //[SerializeField] private GameObject resultPanel; // ��� �г�
    //[SerializeField] private Text resultText; // ��� �ؽ�Ʈ
    //[SerializeField] private Button closeResultButton; // �ݱ� ��ư

    //[Header("�ð� ȿ��")]
    //[SerializeField] private Image boxImage; // �����ڽ� �̹���
    //[SerializeField] private Animator boxAnimator; // �ִϸ��̼��� ���� Animator (���û���)
    //[SerializeField] private string openAnimationTrigger = "Open"; // �ִϸ��̼� Ʈ���� �̸�

    [Header("�����ڽ� ����")]
    [SerializeField] private RandomBox randomBox; // RandomBox ��ũ��Ʈ ����

    private void Start()
    {
        // �����ڽ� ��ũ��Ʈ �˻�
        if (randomBox == null)
        {
            randomBox = GetComponent<RandomBox>();

            if (randomBox == null)
            {
                Debug.LogError("RandomBox ������Ʈ�� ã�� �� �����ϴ�.");
                return;
            }
        }

        // ��ư �̺�Ʈ ����
        if (openBoxButton != null)
        {
            openBoxButton.onClick.AddListener(OnOpenBoxButtonClicked);
        }

        //if (closeResultButton != null)
        //{
        //    closeResultButton.onClick.AddListener(CloseResultPanel);
        //}

        //// ��� �̺�Ʈ ���
        //randomBox.OnRandomBoxResult.AddListener(ShowResult);

        //// ��� �г� �ʱ� ����
        //if (resultPanel != null)
        //{
        //    resultPanel.SetActive(false);
        //}
    }

    // �����ڽ� ���� ��ư Ŭ��
    private void OnOpenBoxButtonClicked()
    {
        //// �ִϸ��̼��� ������ ���
        //if (boxAnimator != null)
        //{
        //    boxAnimator.SetTrigger(openAnimationTrigger);
        //    // �ִϸ��̼� �� �ణ�� �����̸� �ΰ� ���� �����ڽ� ����
        //    StartCoroutine(OpenBoxAfterDelay(0.5f));
        //}
        //else
        //{
        //    // �ִϸ��̼��� ������ �ٷ� ����
        //}
        randomBox.OpenRandomBox();

    }

    // ������ �� �����ڽ� ����
    private IEnumerator OpenBoxAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        randomBox.OpenRandomBox();
    }

    //// ��� ǥ��
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

    //// ��� �г� �ݱ�
    //private void CloseResultPanel()
    //{
    //    if (resultPanel != null)
    //    {
    //        resultPanel.SetActive(false);
    //    }
    //}

    //// ������Ʈ ���� �� �̺�Ʈ ���� ����
    //private void OnDestroy()
    //{
    //    if (randomBox != null)
    //    {
    //        randomBox.OnRandomBoxResult.RemoveListener(ShowResult);
    //    }
    //}
}