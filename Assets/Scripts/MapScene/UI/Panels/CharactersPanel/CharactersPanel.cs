using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace CriminalTown {

    public class CharactersPanel : MonoBehaviour, ICharactersContainer {
        public Transform charactersLocation;
        public Button characterPrefab;

        private Dictionary<Character, Button> charactersDict;

        private void Start() {
            DataScript.ChData.OnAddEvent += OnAddReaction;
            DataScript.ChData.OnRemoveEvent += OnRemoveReaction;
            Night.Instance.OnNightEnded += UpdateCharactersPanel;
        }

        public void UpdateCharactersPanel() {
            if (charactersDict != null)
                foreach (Button character in charactersDict.Values)
                    if (character.gameObject)
                        Destroy(character.gameObject);

            charactersDict = new Dictionary<Character, Button>();

            foreach (Character character in DataScript.ChData.PanelCharacters) {
                charactersDict.Add(character, Instantiate(characterPrefab, charactersLocation));
                charactersDict[character].GetComponent<CharacterCustomization>().CustomizeCharacter(character);
            }
        }

        public void SetActive(bool value) {
            foreach (Button charObj in charactersDict.Values)
                charObj.transform.GetChild(0).GetComponent<Button>().interactable = value;
        }

        public void OnRemoveReaction(Character character) {
            Destroy(charactersDict[character].gameObject);
            charactersDict.Remove(character);
        }

        public void OnAddReaction([NotNull] Character character) {
            charactersDict.Add(character, Instantiate(characterPrefab, charactersLocation));
            charactersDict[character].GetComponent<CharacterCustomization>().CustomizeCharacter(character);
        }
    }

}