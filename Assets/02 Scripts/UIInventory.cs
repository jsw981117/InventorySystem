using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInventory : MonoBehaviour
{
    [SerializeField] private Button backBtn;
    [SerializeField] private UISlot slot;
    List<UISlot> slots = new List<UISlot>();


    void Start()
    {
        gameObject.SetActive(false);
        backBtn.onClick.AddListener(OnClickBackBtn);
    }

    void OnClickBackBtn()
    {
        gameObject.SetActive(false);
        UIManager.Instance.MainMenu.ReActiveButtons();
    }
}
