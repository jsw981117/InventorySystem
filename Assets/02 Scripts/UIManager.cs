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

    // �����̺� �ʵ�
    [SerializeField] private UIMainMenu _mainMenu;
    [SerializeField] private UIStatus _status;
    [SerializeField] private UIInventory _inventory;

    public UIMainMenu MainMenu
    {
        get { return _mainMenu; }
        private set { _mainMenu = value; }
    }

    public UIStatus Status
    {
        get { return _status; }
        private set { _status = value; }
    }

    public UIInventory Inventory
    {
        get { return _inventory; }
        private set { _inventory = value; }
    }

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

        // �ʿ��� ������Ʈ�� ������ ã��
        if (_mainMenu == null)
            _mainMenu = FindObjectOfType<UIMainMenu>();

        if (_status == null)
            _status = FindObjectOfType<UIStatus>();

        if (_inventory == null)
            _inventory = FindObjectOfType<UIInventory>();
    }
}
