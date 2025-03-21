using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMainMenu : MonoBehaviour
{
    [SerializeField] private Button statusBtn;
    [SerializeField] private Button invBtn;

    void Start()
    {
        statusBtn.onClick.AddListener(OnClickStatusBtn);
        invBtn.onClick.AddListener(OnClickInvBtn);
    }

    void OnClickStatusBtn()
    {
        DeactiveButtons();
        // 스테이터스 창 활성화
    }

    void OnClickInvBtn()
    {
        DeactiveButtons();
        // 인벤 창 활성화
    }
    void DeactiveButtons()
    {
        statusBtn.gameObject.SetActive(false);
        invBtn.gameObject.SetActive(false);
    }

    void ReActiveButtons()
    {
        statusBtn.gameObject.SetActive(true);
        invBtn.gameObject.SetActive(true);
    }
}
