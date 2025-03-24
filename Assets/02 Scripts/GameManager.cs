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

        // 플레이어 캐릭터 설정
        SetupPlayerCharacter();
    }

    private void Start()
    {
        // UIManager에 플레이어 캐릭터 참조 전달
        if (UIManager.Instance != null && _playerCharacter != null)
        {
            UIManager.Instance.SetPlayerCharacter(_playerCharacter);
        }
    }

    // 플레이어 캐릭터 설정
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

        // 기본 캐릭터 데이터 설정
        SetData("전사", "플레이어", 1, "기본 캐릭터입니다.", 10, 100, 5, 0.05f);
    }

    // 캐릭터 기본 데이터 설정
    public void SetData(string job, string name, int level, string description, int attackPower, int healthPoints, int defense, float criticalChance)
    {
        if (_playerCharacter == null)
        {
            GameObject characterObj = new GameObject("Player");
            _playerCharacter = characterObj.AddComponent<Character>();
        }

        // 캐릭터 기본 데이터 설정
        _playerCharacter.SetCharacterData(job, name, level, description, attackPower, healthPoints, defense, criticalChance);

        // UIManager에 캐릭터 참조 전달 (UI 업데이트는 Character의 이벤트를 통해 처리됨)
        if (UIManager.Instance != null)
        {
            UIManager.Instance.SetPlayerCharacter(_playerCharacter);
        }
    }
}