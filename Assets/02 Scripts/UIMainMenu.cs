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
    [SerializeField] private TextMeshProUGUI goldText;

    void Start()
    {
        statusBtn.onClick.AddListener(OnClickStatusBtn);
        invBtn.onClick.AddListener(OnClickInvBtn);

        if (GameManager.Instance != null && GameManager.Instance.PlayerCharacter != null)
        {
            UpdateCharacterInfo(GameManager.Instance.PlayerCharacter);
        }

        ReActiveButtons();
    }

    /// <summary>
    /// 스테이터스 버튼 클릭
    /// </summary>
    void OnClickStatusBtn()
    {
        DeactiveButtons();
        // 스테이터스 창 활성화
        UIManager.Instance.Status.UpdateStatus();
    }

    /// <summary>
    /// 인벤 버튼 클릭
    /// </summary>
    void OnClickInvBtn()
    {
        DeactiveButtons();
        // 인벤 창 활성화
        UIManager.Instance.Inventory.gameObject.SetActive(true);
    }

    /// <summary>
    /// 버튼들 비활성화(스테이터스, 인벤 팝업 뜨면)
    /// </summary>
    void DeactiveButtons()
    {
        statusBtn.gameObject.SetActive(false);
        invBtn.gameObject.SetActive(false);
    }

    /// <summary>
    /// 버튼들 활성화
    /// </summary>
    public void ReActiveButtons()
    {
        statusBtn.gameObject.SetActive(true);
        invBtn.gameObject.SetActive(true);
    }

    /// <summary>
    /// 캐릭터 정보 업데이트
    /// </summary>
    /// <param name="character"></param>
    public void UpdateCharacterInfo(Character character)
    {
        if (character == null)
        {
            return;
        }
        characterNameText.text = $"{character.CharacterName}";
        jobText.text = $"{character.CharacterJob}";
        levelText.text = $"{character.Level}";
        descriptionText.text = character.Description;
        goldText.text = $"{character.Gold}";
    }
}
