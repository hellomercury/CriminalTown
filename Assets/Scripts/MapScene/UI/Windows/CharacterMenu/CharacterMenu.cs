using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;

public class CharacterMenu : MonoBehaviour {

    #region References

    [FormerlySerializedAs("")]
    public GameObject characterMenuObject;
    public Scrollbar verticalScrollbar;
    public Image background;

    private Color defaultSpColor = Color.yellow;
    private Color defaultComColor = Color.white;
    private Color hospitalColor = Color.green;
    private Color arrestedColor = Color.cyan;
    private Color robberyColor = Color.gray;

    public Text levelText;
    public Text characterName;
    public Image portrait;

    public GameObject baseParameters;

    public Text health;
    public Text tiredness;
    public Text experience;

    public Slider healthSlider;
    public Slider tirednessSlider;
    public Slider experienceSlider;

    public GameObject hospitalParameters;

    public Text recovery;
    public Slider recoverySlider;
    public Text moneyRecText;
    public Image moneyRecImage;
    public Text recDaysLeft;
    public Button boostRecoveryButton;

    public GameObject arrestParameters;

    public Text opposition;
    public Slider oppositionSlider;
    public Text moneyArrText;
    public Text arrDaysLeft;
    public Button breakOutButton;

    public Text strengthText;
    public Text agilityText;
    public Text skillText;
    public Text luckText;
    public Text fearText;

    public Slider strengthSlider;
    public Slider agilitySlider;
    public Slider skillSlider;
    public Slider luckSlider;
    public Slider fearSlider;

    public Button strengthButton;
    public Button agilityButton;
    public Button skillButton;
    public Button luckButton;

    public Button undoButton;
    public Text levelUpText;
    public Text pointsCountText;

    public GameObject traitsParameters;
    public List<GameObject> traits;
    public GameObject traitPrefab;

    public Text historyText;

    public Button kickButton;

    #endregion

    private int charNumber;
    private int price;

    private Character m_character;
    private CharacterStats m_newCharacterStats;

    public void SetCharacterMenu([NotNull] Character characterReference) {
        m_character = characterReference;
        SetSlidersMaxValues();

        portrait.sprite = m_character.Sprite;
        characterName.text = m_character.Name;
        historyText.text = m_character.History;
        levelText.text = m_character.Level.ToString();

        m_newCharacterStats = m_character.Stats;
        UpdatePointsAndStats();
        SetTraits();
        CommonWindowSettings();

        DataScript.ChData.OnRemoveEvent += ch => OnKickCharacterWindowReaction();
        m_character.OnStatsChangedEvent += UpdatePointsAndStats;
    }

    private void SetSlidersMaxValues() {
        strengthSlider.maxValue = CharactersOptions.MaxStat;
        agilitySlider.maxValue = CharactersOptions.MaxStat;
        skillSlider.maxValue = CharactersOptions.MaxStat;
        luckSlider.maxValue = CharactersOptions.MaxStat;
        fearSlider.maxValue = CharactersOptions.MaxFear;

        experienceSlider.maxValue = CharactersOptions.GetExperienceMaxValue(m_character.Level);
        healthSlider.maxValue = CharactersOptions.MaxHealth;
        tirednessSlider.maxValue = CharactersOptions.MaxTiredness;
        recoverySlider.maxValue = CharactersOptions.MaxRecovery;
        oppositionSlider.maxValue = CharactersOptions.MaxOpposition;
    }

    private void CommonWindowSettings() {
        verticalScrollbar.value = 1;
        characterMenuObject.transform.SetAsLastSibling();
        characterMenuObject.SetActive(true);
    }

    public void UpdatePointsAndStats() {
        baseParameters.SetActive(false);
        hospitalParameters.SetActive(false);
        arrestParameters.SetActive(false);

        healthSlider.value = m_newCharacterStats.Health;
        tirednessSlider.value = m_newCharacterStats.Tiredness;
        experienceSlider.value = m_newCharacterStats.Expirience;

        health.text = m_newCharacterStats.Health + " / " + healthSlider.maxValue.ToString();
        tiredness.text = m_newCharacterStats.Tiredness + " / " + tirednessSlider.maxValue.ToString();
        experience.text = m_newCharacterStats.Expirience + " / " + experienceSlider.maxValue.ToString();

        strengthButton.interactable = m_character.Status == CharacterStatus.Normal;
        agilityButton.interactable = m_character.Status == CharacterStatus.Normal;
        skillButton.interactable = m_character.Status == CharacterStatus.Normal;
        luckButton.interactable = m_character.Status == CharacterStatus.Normal;
        undoButton.interactable = m_character.Status == CharacterStatus.Normal;

        switch (m_character.Status) {
            case CharacterStatus.Normal: {
                background.color = m_character.GetType() == typeof(SpecialCharacter) ? defaultSpColor : defaultComColor;
                baseParameters.SetActive(true);
                break;
            }
            case CharacterStatus.Robbery: {
                background.color = robberyColor;
                baseParameters.SetActive(true);
                break;
            }
            case CharacterStatus.Hospital: {
                background.color = hospitalColor;
                price = CharactersOptions.GetBoostRecoveryPrice(m_character.Level);
                hospitalParameters.SetActive(true);
                recoverySlider.value = m_character.StatusValue;
                recovery.text = m_character.StatusValue.ToString() + " / " + recoverySlider.maxValue.ToString();

                recDaysLeft.text = m_character.DaysLeft.ToString();
                moneyRecText.text = price.ToString();

                boostRecoveryButton.gameObject.SetActive(m_character.BoostCoefficient == 1 && price < DataScript.SData.Money);
                moneyRecImage.gameObject.SetActive(m_character.BoostCoefficient == 1);
                moneyRecText.gameObject.SetActive(m_character.BoostCoefficient == 1);
                break;
            }
            case CharacterStatus.Arrested: {
                background.color = arrestedColor;
                price = CharactersOptions.GetBreakOutPrice(m_character.Level);
                arrestParameters.SetActive(true);
                oppositionSlider.value = m_character.StatusValue;
                opposition.text = m_character.StatusValue.ToString() + " / " + oppositionSlider.maxValue.ToString();

                arrDaysLeft.text = m_character.DaysLeft.ToString();
                moneyArrText.text = CharactersOptions.GetBreakOutPrice(m_character.Level).ToString();
                breakOutButton.gameObject.SetActive(price < DataScript.SData.Money);
                break;
            }
        }

        strengthText.text = m_newCharacterStats.Strength.ToString();
        agilityText.text = m_newCharacterStats.Agility.ToString();
        skillText.text = m_newCharacterStats.Skill.ToString();
        luckText.text = m_newCharacterStats.Luck.ToString();
        fearText.text = m_newCharacterStats.Fear.ToString();

        strengthSlider.value = m_newCharacterStats.Strength;
        agilitySlider.value = m_newCharacterStats.Agility;
        skillSlider.value = m_newCharacterStats.Skill;
        luckSlider.value = m_newCharacterStats.Luck;
        fearSlider.value = m_newCharacterStats.Fear;

        undoButton.gameObject.SetActive(m_newCharacterStats.Points > 0);

        strengthButton.gameObject.SetActive(m_newCharacterStats.Points > 0 && strengthSlider.value < strengthSlider.maxValue);
        agilityButton.gameObject.SetActive(m_newCharacterStats.Points > 0 && agilitySlider.value < agilitySlider.maxValue);
        skillButton.gameObject.SetActive(m_newCharacterStats.Points > 0 && skillSlider.value < skillSlider.maxValue);
        luckButton.gameObject.SetActive(m_newCharacterStats.Points > 0 && luckSlider.value < luckSlider.maxValue);

        levelUpText.gameObject.SetActive(m_newCharacterStats.Points > 0);
        pointsCountText.gameObject.SetActive(m_newCharacterStats.Points > 0);
        if (m_newCharacterStats.Points > 0)
            pointsCountText.text = m_newCharacterStats.Points.ToString();
    }

    public void OnSkillButtonClick(int skillNum) {
        switch (skillNum) {
            case 0: {
                m_newCharacterStats.Strength++;
                break;
            }
            case 1: {
                m_newCharacterStats.Agility++;
                break;
            }
            case 2: {
                m_newCharacterStats.Skill++;
                break;
            }
            case 3: {
                m_newCharacterStats.Luck++;
                break;
            }
        }
        m_newCharacterStats.Points--;
        UpdatePointsAndStats();
    }

    public void OnUndoButtonClick() {
        m_newCharacterStats = m_character.Stats;
        UpdatePointsAndStats();
    }

    public void SetTraits() {
        foreach (GameObject trait in traits)
            Destroy(trait.gameObject);

        if (m_character.GetType() == typeof(SpecialCharacter)) {
            SpecialCharacter spChar = (SpecialCharacter) m_character;
            traits = new List<GameObject>();
            for (int i = 0; i < spChar.Traits.Count; i++) {
                traits.Add(Instantiate(traitPrefab, traitsParameters.transform));
                traits[i].GetComponent<TraitsCustomization>().CustomizeTrait(spChar.Traits[i]);
            }
            traitsParameters.SetActive(true);
        } else {
            traitsParameters.SetActive(false);
        }
    }

    public void CloseAndSave() {
        characterMenuObject.SetActive(false);
        m_character.Stats = m_newCharacterStats;
        DataScript.ChData.OnRemoveEvent -= (ch) => OnKickCharacterWindowReaction();
        m_character.OnStatsChangedEvent -= UpdatePointsAndStats;
    }

    public void BoostRecovery() {
        m_character.BoostRecovery();
        DataScript.SData.Money -= price;
    }

    public void BreakOut() {
        m_character.SetDefaultStatus();
        DataScript.SData.Money -= price;
    }

    public void OnKickButtonClick() {
        EventButtonDetails yesButton = new EventButtonDetails {
            buttonText = "Да",
            action = () => DataScript.ChData.RemoveCharacter(m_character)
        };
        EventButtonDetails noButton = new EventButtonDetails {
            buttonText = "Нет",
            action = WM1.modalPanel.ClosePanel
        };
        ModalPanelDetails details = new ModalPanelDetails {
            button0Details = yesButton,
            button1Details = noButton,
            imageSprite = portrait.sprite,
            text = m_character.Status == CharacterStatus.Arrested
                ? "Вы уверены что хотите оставить данного персонажа в грязных руках копов? Это скажется на осведомлённости полиции о вашей банде"
                : "Вы уверены, что хотите выгнать данного персонажа?",
            titletext = characterName.text
        };
        WM1.modalPanel.CallModalPanel(details);
    }

    public void OnKickCharacterWindowReaction() {
        //Animation???
        StartCoroutine(AnimationExample());
    }

    private IEnumerator AnimationExample() {
        yield return new WaitForSeconds(1);
        background.color = Color.black;
        yield return new WaitForSeconds(1);
        background.color = Color.white;
        yield return new WaitForSeconds(1);
        characterMenuObject.SetActive(false);
    }
}