using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIStatus : MonoBehaviour
{
    [SerializeField] private Button backBtn;

    // Ω∫≈» UI ø‰º“µÈ
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

    void OnClickBackBtn()
    {
        gameObject.SetActive(false);
        UIManager.Instance.MainMenu.ReActiveButtons();
    }

    public void UpdateStatus()               
    {
        gameObject.SetActive(true);

        if (GameManager.Instance != null && GameManager.Instance.PlayerCharacter != null)
        {
            UpdateCharacterInfo(GameManager.Instance.PlayerCharacter);
        }
    }

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
