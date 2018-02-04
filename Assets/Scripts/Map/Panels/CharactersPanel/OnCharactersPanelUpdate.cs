using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnCharactersPanelUpdate : MonoBehaviour
{
    public Transform charactersLocation;
    public Button characterPrefab;

    private Dictionary<Character, Button> charactersDict;

    private void Start()
    {
        DataScript.chData.OnAddEvent += OnAddCharacterPanelReaction;
        DataScript.chData.OnRemoveEvent += OnKickCharacterPanelReaction;
    }

    public void UpdateCharactersPanel()
    {
        if (charactersDict != null)
            foreach (Button character in charactersDict.Values)
                if (character.gameObject) Destroy(character.gameObject);

        charactersDict = new Dictionary<Character, Button>();

        foreach (Character character in DataScript.chData.PanelCharacters)
        {
            charactersDict.Add(character, Instantiate(characterPrefab, charactersLocation));
            charactersDict[character].GetComponent<CharacterCustomization>().CustomizeCharacter(character);
        }
    }

    public void SetActive(bool value)
    {
        foreach (Button charObj in charactersDict.Values)
            charObj.transform.GetChild(0).GetComponent<Button>().interactable = value;
    }

    private void OnKickCharacterPanelReaction(Character character)
    {
        Destroy(charactersDict[character].gameObject);
    }

    private void OnAddCharacterPanelReaction(Character character)
    {
        charactersDict.Add(character, Instantiate(characterPrefab, charactersLocation));
        charactersDict[character].GetComponent<CharacterCustomization>().CustomizeCharacter(character);
    }
}
