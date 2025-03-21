using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMainMenu : MonoBehaviour
{
    [SerializeField] private Button statusBtn;
    [SerializeField] private Button invBtn;

    [Header("Character Info UI")]
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private TextMeshProUGUI jobText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    void Start()
    {
        statusBtn.onClick.AddListener(OnClickStatusBtn);
        invBtn.onClick.AddListener(OnClickInvBtn);

        if (GameManager.Instance != null && GameManager.Instance.PlayerCharacter != null)
        {
            UpdateCharacterInfo(GameManager.Instance.PlayerCharacter);
        }
    }

    void OnClickStatusBtn()
    {
        DeactiveButtons();
        // 스테이터스 창 활성화
        UIManager.Instance.Status.UpdateStatus();
    }

    void OnClickInvBtn()
    {
        DeactiveButtons();
        // 인벤 창 활성화
        UIManager.Instance.Inventory.gameObject.SetActive(true);
    }
    void DeactiveButtons()
    {
        statusBtn.gameObject.SetActive(false);
        invBtn.gameObject.SetActive(false);
    }

    public void ReActiveButtons()
    {
        statusBtn.gameObject.SetActive(true);
        invBtn.gameObject.SetActive(true);
    }

    public void UpdateCharacterInfo(Character character)
    {
        if (character == null)
        {
            Debug.LogError("Character is null");
            return;
        }
        characterNameText.text = $"{character.CharacterName}";
        jobText.text = $"{character.CharacterJob}";
        levelText.text = $"{character.Level}";
        descriptionText.text = character.Description;
    }
}
