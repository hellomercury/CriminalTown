using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Hospital : MonoBehaviour, ICharactersContainer
{
    public GameObject hospitalObject;
    public Transform charactersLocation;
    public Transform parentButton;

    public GameObject characterPrefab;

    private Dictionary<Character, GameObject> charactersDict;

    private void OnEnable()
    {
        hospitalObject.transform.SetAsLastSibling();
        
    }

    public void UpdateHospital()
    {
        if (charactersDict != null)
            foreach (GameObject character in charactersDict.Values)
                if (character.gameObject) Destroy(character.gameObject);

        charactersDict = new Dictionary<Character, GameObject>();

        foreach (Character character in DataScript.chData.PanelCharacters)
        {
            if (character.Status == CharacterStatus.hospital)
            {
                charactersDict.Add(character, Instantiate(characterPrefab, charactersLocation));
                charactersDict[character].GetComponent<CharacterCustomization>().CustomizeCharacter(character);
            }
        }
    }

    public void CloseHospital()
    {
        hospitalObject.SetActive(false);
    }

    public void TryToAddCharacterToHospital(Character character)
    {
        ModalPanelDetails details;
        if (character.Health <= 90)
        {
            EventButtonDetails yesButton = new EventButtonDetails
            {
                buttonText = "Да",
                action = () => {character.AddToHospital(); }
            };
            EventButtonDetails noButton = new EventButtonDetails
            {
                buttonText = "Нет",
                action = WM1.modalPanel.ClosePanel
            };
            details = new ModalPanelDetails
            {
                button0Details = yesButton,
                button1Details = noButton,
                imageSprite = character.Sprite,
                text = "Отправить персонажа на принудительное лечение?",
                titletext = character.Name
            };
        }
        else
        {
            EventButtonDetails noButton = new EventButtonDetails
            {
                buttonText = "Ну ладно...",
                action = WM1.modalPanel.ClosePanel
            };
            details = new ModalPanelDetails
            {
                button1Details = noButton,
                imageSprite = character.Sprite,
                text = "Босс, я не пойду в больницу из-за этой царапины!",
                titletext = character.Name
            };
        }
        WM1.modalPanel.CallModalPanel(details);

    }

    public void OnRemoveReaction(Character character)
    {
        Destroy(charactersDict[character].gameObject);
        charactersDict.Remove(character);
    }

    public void OnAddReaction(Character character)
    {
        charactersDict.Add(character, Instantiate(characterPrefab, charactersLocation));
        charactersDict[character].GetComponent<CharacterCustomization>().CustomizeCharacter(character);
    }

}
