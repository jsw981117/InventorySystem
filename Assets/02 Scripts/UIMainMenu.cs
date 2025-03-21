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
        statusBtn.gameObject.SetActive(false);
        invBtn.gameObject.SetActive(false);
    }

    void OnClickInvBtn()
    {

    }

    void ReActiveButtons()
    {
        statusBtn.gameObject.SetActive(true);
        invBtn.gameObject.SetActive(true);
    }
}
