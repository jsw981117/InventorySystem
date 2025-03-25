using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIStatus : MonoBehaviour
{
    [SerializeField] private Button backBtn;

    // 스탯 UI 요소들
    [Header("Status UI")]
    [SerializeField] private TextMeshProUGUI attackText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI defenseText;
    [SerializeField] private TextMeshProUGUI criticalText;

    void Start()
    {
        gameObject.SetActive(false);
        backBtn.onClick.AddListener(OnClickBackBtn);
    }

    /// <summary>
    /// 뒤로가기 버튼 클릭
    /// </summary>
    void OnClickBackBtn()
    {
        gameObject.SetActive(false);
        UIManager.Instance.MainMenu.ReActiveButtons();
    }

    /// <summary>
    /// 스테이터스 UI 갱신
    /// </summary>
    public void UpdateStatus()               
    {
        gameObject.SetActive(true);

        if (GameManager.Instance != null && GameManager.Instance.PlayerCharacter != null)
        {
            UpdateCharacterInfo(GameManager.Instance.PlayerCharacter);
        }
    }

    /// <summary>
    /// 캐릭터 능력치 정보(공, 체, 방, 치명타) 업데이트
    /// </summary>
    /// <param name="character"></param>
    public void UpdateCharacterInfo(Character character)
    {
        if (character == null)
        {
            return;
        }

        attackText.text = $"{character.AttackPower}";
        healthText.text = $"{character.HealthPoints}";
        defenseText.text = $"{character.Defense}";
        criticalText.text = $"{character.CriticalChance * 100:F1}%";
    }
}
