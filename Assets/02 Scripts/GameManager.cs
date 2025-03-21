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

        if (_playerCharacter == null)
        {
            _playerCharacter = FindObjectOfType<Character>();

            if (_playerCharacter == null)
            {
                GameObject characterObj = new GameObject("Player");
                _playerCharacter = characterObj.AddComponent<Character>();
            }
        }
        SetData("코딩광전사", "정수누언", 78, "기본 캐릭터입니다.", 10, 100, 5, 0.05f);
    }

    public void SetData(string job, string name, int level, string description, int attackPower, int healthPoints, int defense, float criticalChance)
    {
        if (_playerCharacter == null)
        {
            GameObject characterObj = new GameObject("Player");
            _playerCharacter = characterObj.AddComponent<Character>();
        }

        _playerCharacter.SetCharacterData(job, name, level, description, attackPower, healthPoints, defense, criticalChance);

        UpdateUIWithCharacter(_playerCharacter);
    }

    private void UpdateUIWithCharacter(Character character)
    {
        if (UIManager.Instance != null)
        {
            if (UIManager.Instance.Status != null)
            {
                UIManager.Instance.Status.UpdateCharacterInfo(character);
            }

            if (UIManager.Instance.Inventory != null)
            {
                // 인벤토리 구현 예정
            }
        }
    }
}
