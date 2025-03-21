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

    // 프라이빗 필드
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
        // 중복 인스턴스 방지
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject); // 씬 전환시에도 유지

        // 필요한 컴포넌트가 없으면 찾기
        if (_mainMenu == null)
            _mainMenu = FindObjectOfType<UIMainMenu>();

        if (_status == null)
            _status = FindObjectOfType<UIStatus>();

        if (_inventory == null)
            _inventory = FindObjectOfType<UIInventory>();
    }
}
