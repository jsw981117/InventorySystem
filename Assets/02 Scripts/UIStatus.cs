using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIStatus : MonoBehaviour
{
    [SerializeField] private Button backBtn;

    // ���� UI ��ҵ�
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
    /// �ڷΰ��� ��ư Ŭ��
    /// </summary>
    void OnClickBackBtn()
    {
        gameObject.SetActive(false);
        UIManager.Instance.MainMenu.ReActiveButtons();
    }

    /// <summary>
    /// �������ͽ� UI ����
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
    /// ĳ���� �ɷ�ġ ����(��, ü, ��, ġ��Ÿ) ������Ʈ
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
