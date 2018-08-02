using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CampCharacterCustomization : MonoBehaviour
{
    #region References
    public Text LevelText;
    public Image Portrait;
    public Text CharacterName;
    public Text Strength;
    public Text Agility;
    public Text Skill;
    public Text Luck;
    public Text Fear;
    public Text Description;

    public GameObject Trait;
    public Text TraitDescriptionText;
    public Text TraitNameText;
    public Image TraitImage;
    #endregion

    private Character m_character;
    public Button CharacterButton;

    private Color m_defaultSpColor = Color.yellow;
    private Color m_defaultComColor = Color.white;

    public void OnClick()
    {
        ColorBlock tempColorBlock = CharacterButton.colors;
        Color highlightColor = tempColorBlock.normalColor;
        highlightColor.a = 0.5f;
        tempColorBlock.normalColor = highlightColor;
        tempColorBlock.highlightedColor = highlightColor;
        CharacterButton.colors = tempColorBlock;
        WM1.banditCamp.HighlightCharacter(m_character);
    }

    public void SetDefaultColor()
    {
        Color tempColor = m_character.GetType() == typeof(SpecialCharacter) ?
            m_defaultSpColor : m_defaultComColor;
        ColorBlock tempColorBlock = CharacterButton.colors;
        tempColorBlock.normalColor = tempColor;
        tempColorBlock.highlightedColor = tempColor;
        CharacterButton.colors = tempColorBlock;
    }

    public void CustomizeCharacter(Character character)
    {
        this.m_character = character;
        Portrait.sprite = character.Sprite;
        CharacterName.text = character.Name;
        Description.text = character.History;

        if (character.GetType() == typeof(SpecialCharacter))
        {
            CustomizeTraits();
        } else {
            Trait.SetActive(false);
        }

        SetCharStats();
        SetDefaultColor();
    }

    private void CustomizeTraits()
    {
        SpecialCharacter spChar = (SpecialCharacter)m_character;

        Trait newTrait = spChar.Traits[0];
        TraitDescriptionText.text = newTrait.description;
        TraitNameText.text = newTrait.name;
        TraitImage.sprite = spChar.Traits[0].Sprite;
        Trait.SetActive(true);
    }

    private void SetCharStats()
    {
        LevelText.text = m_character.Level + " Ур.";
        Strength.text = m_character.Stats.Strength.ToString();
        Agility.text = m_character.Stats.Agility.ToString();
        Skill.text = m_character.Stats.Skill.ToString();
        Luck.text = m_character.Stats.Luck.ToString();
        Fear.text = m_character.Stats.Fear.ToString();
    }
}
