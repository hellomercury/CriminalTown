using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnBanditCampOpen : MonoBehaviour
{
    #region References
    public Transform charactersLocation;
    public Button characterPrefab;

    public Image moneyImage;
    public Text priceText;
    public Button hireButton;
    #endregion

    private Dictionary<Character, Button> charactersDict;

    private Character highlightedChar;
    private int highlightedCharPrice;

    private void OnEnable()
    {
        transform.SetAsLastSibling();
    }

    public void UpdateBanditCamp()
    {
        moneyImage.gameObject.SetActive(false);
        priceText.gameObject.SetActive(false);
        hireButton.interactable = false;

        if (charactersDict != null)
            foreach (Button character in charactersDict.Values)
                if (character.gameObject) Destroy(character.gameObject);

        charactersDict = new Dictionary<Character, Button>();

        foreach (Character character in DataScript.ChData.CampCharacters)
        {
            charactersDict.Add(character, Instantiate(characterPrefab, charactersLocation));
            charactersDict[character].GetComponent<CampCharacterCustomization>().CustomizeCharacter(character);
        }
    }

    public void HighlightCharacter(Character character)
    {
        if (highlightedChar != null)
            charactersDict[highlightedChar].GetComponent<CampCharacterCustomization>().SetDefaultColor();

        moneyImage.gameObject.SetActive(true);
        priceText.gameObject.SetActive(true);

        highlightedCharPrice = character.GetType() == typeof(SpecialCharacter) ?
            CharactersOptions.GetSpPrice(character.Level) : CharactersOptions.GetComPrice(character.Level);

        priceText.text = highlightedCharPrice.ToString();

        if (DataScript.SData.Money >= highlightedCharPrice
            && DataScript.ChData.PanelCharacters.Count < CharactersOptions.PanelCellsMaxAmount)
        {
            hireButton.interactable = true;
        }
        else hireButton.interactable = false;

        highlightedChar = character;
    }

    public void HireCharacter()
    {
        Destroy(charactersDict[highlightedChar].gameObject);
        DataScript.SData.Money -= highlightedCharPrice;

        moneyImage.gameObject.SetActive(false);
        priceText.gameObject.SetActive(false);
        hireButton.interactable = false;

        DataScript.ChData.AddCharacter(highlightedChar);
        charactersDict.Remove(highlightedChar);

        highlightedChar = null;
    }
}
