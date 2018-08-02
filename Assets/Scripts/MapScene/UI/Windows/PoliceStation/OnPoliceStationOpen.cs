using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CriminalTown {

    public class OnPoliceStationOpen : MonoBehaviour, ICharactersContainer {
        public Transform charactersLocation;
        public GameObject characterPrefab;
        public Slider policeKnowledge;
        public Text policeKnowledgeValueText;

        private Dictionary<Character, GameObject> charactersDict;

        private void OnEnable() {
            transform.SetAsLastSibling();
            policeKnowledge.value = DataScript.EData.PoliceKnowledge;
            policeKnowledgeValueText.text = DataScript.EData.PoliceKnowledge + " / 100";
        }

        public void UpdatePoliceStationCharacters() {
            if (charactersDict != null)
                foreach (GameObject character in charactersDict.Values)
                    if (character.gameObject)
                        Destroy(character.gameObject);

            charactersDict = new Dictionary<Character, GameObject>();

            foreach (Character character in DataScript.ChData.PanelCharacters) {
                if (character.Status == CharacterStatus.Arrested) {
                    charactersDict.Add(character, Instantiate(characterPrefab, charactersLocation));
                    charactersDict[character].GetComponent<CharacterCustomization>().CustomizeCharacter(character);
                }
            }
        }

        public void OnRemoveReaction(Character character) {
            Destroy(charactersDict[character].gameObject);
            charactersDict.Remove(character);
        }

        public void OnAddReaction(Character character) {
            charactersDict.Add(character, Instantiate(characterPrefab, charactersLocation));
            charactersDict[character].GetComponent<CharacterCustomization>().CustomizeCharacter(character);
        }

    }

}
