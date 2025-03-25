using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomBoxUI : MonoBehaviour
{
    [Header("UI ������Ʈ")]
    [SerializeField] private Button openBoxButton; // �����ڽ� ���� ��ư

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
                return;
            }
        }

        // ��ư �̺�Ʈ ����
        if (openBoxButton != null)
        {
            openBoxButton.onClick.AddListener(OnOpenBoxButtonClicked);
        }
    }

    // �����ڽ� ���� ��ư Ŭ��
    private void OnOpenBoxButtonClicked()
    {
        randomBox.OpenRandomBox();
    }
}