using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomBoxUI : MonoBehaviour
{
    [SerializeField] private RandomBox randomBox; // �����ڽ� ��ũ��Ʈ ����
    [SerializeField] private Button openButton; // ���� ��ư
    //[SerializeField] private Button closeResultButton; // ��� �ݱ� ��ư

    //[SerializeField] private GameObject resultPanel; // ��� �г�
    //[SerializeField] private Text resultText; // ��� �ؽ�Ʈ

    void Start()
    {
        // ��ư �̺�Ʈ ����
        if (openButton != null && randomBox != null)
        {
            openButton.onClick.AddListener(randomBox.OpenRandomBox);
        }

        //if (closeResultButton != null && randomBox != null)
        //{
        //    closeResultButton.onClick.AddListener(randomBox.CloseResultPanel);
        //}

        //// ��� �г� �ʱ� ����
        //if (resultPanel != null)
        //{
        //    resultPanel.SetActive(false);
        //}
    }
}