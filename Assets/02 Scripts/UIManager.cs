using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;

    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<UIManager>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject("UIManager");
                    _instance = obj.AddComponent<UIManager>();
                }
            }
            return _instance;
        }
    }

    [SerializeField] UIMainMenu mainMenu;
    [SerializeField] UIStatus status;
    [SerializeField] UIInventory inventory;

    private void Awake()
    {
        // �ߺ� �ν��Ͻ� ����
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject); // �� ��ȯ�ÿ��� ����
    }
}
