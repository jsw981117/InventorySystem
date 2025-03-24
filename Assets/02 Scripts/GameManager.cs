using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();

                if (_instance == null)
                {
                    GameObject obj = new GameObject("GameManager");
                    _instance = obj.AddComponent<GameManager>();
                }
            }
            return _instance;
        }
    }

    [SerializeField] private Character _playerCharacter;

    public Character PlayerCharacter
    {
        get { return _playerCharacter; }
        private set { _playerCharacter = value; }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        // �÷��̾� ĳ���� ����
        SetupPlayerCharacter();
    }

    private void Start()
    {
        // UIManager�� �÷��̾� ĳ���� ���� ����
        if (UIManager.Instance != null && _playerCharacter != null)
        {
            UIManager.Instance.SetPlayerCharacter(_playerCharacter);
        }
    }

    // �÷��̾� ĳ���� ����
    private void SetupPlayerCharacter()
    {
        if (_playerCharacter == null)
        {
            _playerCharacter = FindObjectOfType<Character>();

            if (_playerCharacter == null)
            {
                GameObject characterObj = new GameObject("Player");
                _playerCharacter = characterObj.AddComponent<Character>();
            }
        }

        // �⺻ ĳ���� ������ ����
        SetData("����", "�÷��̾�", 1, "�⺻ ĳ�����Դϴ�.", 10, 100, 5, 0.05f);
    }

    // ĳ���� �⺻ ������ ����
    public void SetData(string job, string name, int level, string description, int attackPower, int healthPoints, int defense, float criticalChance)
    {
        if (_playerCharacter == null)
        {
            GameObject characterObj = new GameObject("Player");
            _playerCharacter = characterObj.AddComponent<Character>();
        }

        // ĳ���� �⺻ ������ ����
        _playerCharacter.SetCharacterData(job, name, level, description, attackPower, healthPoints, defense, criticalChance);

        // UIManager�� ĳ���� ���� ���� (UI ������Ʈ�� Character�� �̺�Ʈ�� ���� ó����)
        if (UIManager.Instance != null)
        {
            UIManager.Instance.SetPlayerCharacter(_playerCharacter);
        }
    }
}