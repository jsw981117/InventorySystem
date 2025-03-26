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

    /// <summary>
    /// 플레이어 캐릭터 설정
    /// </summary>
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
        SetData("전사", "플레이어", 1, "기본 캐릭터입니다.", 99999,  10, 100, 5, 0.05f);
    }

    /// <summary>
    /// 플레이어 캐릭터 데이터 설정
    /// </summary>
    /// <param name="job"></param>
    /// <param name="name"></param>
    /// <param name="level"></param>
    /// <param name="description"></param>
    /// <param name="gold"></param>
    /// <param name="attackPower"></param>
    /// <param name="healthPoints"></param>
    /// <param name="defense"></param>
    /// <param name="criticalChance"></param>
    public void SetData(string job, string name, int level, string description, int gold,  int attackPower, int healthPoints, int defense, float criticalChance)
    {
        if (_playerCharacter == null)
        {
            GameObject characterObj = new GameObject("Player");
            _playerCharacter = characterObj.AddComponent<Character>();
        }

        _playerCharacter.SetCharacterData(job, name, level, description, gold, attackPower, healthPoints, defense, criticalChance);
        if (UIManager.Instance != null)
        {
            UIManager.Instance.SetPlayerCharacter(_playerCharacter);
        }
    }
}