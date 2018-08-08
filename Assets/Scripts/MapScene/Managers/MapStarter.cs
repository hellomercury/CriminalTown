using UnityEngine;

namespace CriminalTown {

    public class MapStarter : MonoBehaviour {
        [SerializeField]
        private UIManager m_uiManager;
        [SerializeField]
        private OptionsManager m_optionsManager;
        [SerializeField]
        private RobberiesManager m_robberiesManager;
        [SerializeField]
        private PlacesManager m_placesManager;

        private void Awake() {
            m_optionsManager.Initialize();

            AssignDefaultData();

            m_robberiesManager.Initialize();
            m_placesManager.Initialize();
            m_uiManager.Initialize();
        }

        public static void AssignDefaultData() {
            //sData
            DataScript.SData.ItemsCount = new int[ItemsOptions.totalAmount];
            for (int i = 0; i < ItemsOptions.totalAmount; i++)
                DataScript.SData.ItemsCount[i] = 5;
            DataScript.SData.Money = 1000000;

            //chData
            Character arrestedChar = CharactersOptions.GetRandomCharacter(5);
            arrestedChar.AddToPolice();
            DataScript.ChData.AddCharacter(arrestedChar);

            Character hospitalChar = CharactersOptions.GetRandomCharacter(6);
            hospitalChar.AddToHospital();
            DataScript.ChData.AddCharacter(hospitalChar);

            Character[] characters = new[] {
                CharactersOptions.GetRandomCharacter(6),
                CharactersOptions.GetRandomCharacter(5),
                CharactersOptions.GetRandomCharacter(4)
            };

            //chData.panelCharacters.Add(CharactersOptions.GetRandomCommonCharacter(8));
            //chData.panelCharacters.Add(CharactersOptions.GetRandomCommonCharacter(9));
            //chData.panelCharacters.Add(CharactersOptions.GetSpecialCharacter(9, 0));
            DataScript.ChData.AddCharacter(CharactersOptions.GetSpecialCharacter(9, 1));
            DataScript.ChData.AddCharacter(characters[0]);
            DataScript.ChData.AddCharacter(characters[1]);
            DataScript.ChData.AddCharacter(characters[2]);

            
            //eData
            DataScript.EData.PoliceKnowledge = 0;

            RobberiesOptions.GetNewRobberies();

            //pData
            DataScript.PData.IsItemAvailable = new bool[ItemsOptions.totalAmount];
            for (int i = 0; i < ItemsOptions.totalAmount; i++)
                DataScript.PData.IsItemAvailable[i] = true;
            DataScript.PData.Authority = 9;

            DataScript.SaveAll();
        }
    }

}