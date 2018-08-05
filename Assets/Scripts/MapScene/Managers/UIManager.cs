using CriminalTown;
using UnityEngine;
using UnityEngine.UI;


namespace CriminalTown {

    /// <summary>
    /// This class contains references to all windows and panels in game
    /// and several methods to work with them
    /// </summary>
    public class UIManager : MonoBehaviour {

        #region Windows

        public GameObject modalPanelObject;
        public GameObject robberyWindowObject;
        public GameObject characterMenuObject;
        public GameObject robberyItemsWindowObject;
        public GameObject policeStationObject;
        public GameObject hospitalObject;
        public GameObject blackMarketObject;
        public GameObject buyWindowObject;
        public GameObject banditCampObject;

        public static ModalPanel modalPanel;
        public static RobberyWindow robberyWindow;
        public static CharacterMenu characterMenu;
        public static RobberyItemsWindow robberyItemsWindow;
        public static OnPoliceStationOpen policeStation;
        public static Hospital hospital;
        public static OnMarketOpen blackMarket;
        public static BuyWindow buyWindow;
        public static OnBanditCampOpen banditCamp;

        #region Night

        public GameObject nightEventWindowObject;
        public GameObject nightResumeWindowObject;

        public static NightEventWindow nightEventWindow;
        public static GameObject nightResumeWindow;

        #endregion

        #endregion

        #region Panels

        public GameObject charactersPanelObject;
        public GameObject itemsPanelObject;
        public GameObject nightButtonObject;


        public static CharactersPanel charactersPanel;
        public static ItemsPanel itemsPanel;
        public static NightButton nightButton;

        #endregion

        public void Initialize() {
            modalPanel = modalPanelObject.GetComponent<ModalPanel>();
            robberyWindow = robberyWindowObject.GetComponent<RobberyWindow>();
            characterMenu = characterMenuObject.GetComponent<CharacterMenu>();
            robberyItemsWindow = robberyItemsWindowObject.GetComponent<RobberyItemsWindow>();
            policeStation = policeStationObject.GetComponent<OnPoliceStationOpen>();
            hospital = hospitalObject.GetComponent<Hospital>();
            blackMarket = blackMarketObject.GetComponent<OnMarketOpen>();
            buyWindow = buyWindowObject.GetComponent<BuyWindow>();
            banditCamp = banditCampObject.GetComponent<OnBanditCampOpen>();

            charactersPanel = charactersPanelObject.GetComponent<CharactersPanel>();
            itemsPanel = itemsPanelObject.GetComponent<ItemsPanel>();
            nightButton = nightButtonObject.GetComponent<NightButton>();

            nightEventWindow = nightEventWindowObject.GetComponent<NightEventWindow>();
            nightResumeWindow = nightResumeWindowObject;

            Night.Instance.OnNightBegan += CloseAllDayWindows;
            Night.Instance.OnNightBegan += () => SetActivePanels(false);
            Night.Instance.OnNightEnded += () => SetActivePanels(true);
        }

        private void Start() {
            charactersPanel.UpdateCharactersPanel();
            banditCamp.UpdateBanditCamp();
            hospital.UpdateHospital();
            policeStation.UpdatePoliceStationCharacters();
        }

        private void CloseAllDayWindows() {
            modalPanelObject.SetActive(false);
            robberyWindowObject.SetActive(false);
            characterMenuObject.SetActive(false);
            robberyItemsWindowObject.SetActive(false);
            policeStationObject.SetActive(false);
            hospitalObject.SetActive(false);
            blackMarketObject.SetActive(false);
            buyWindowObject.SetActive(false);
            banditCampObject.SetActive(false);
            nightResumeWindowObject.SetActive(false);
        }

        private static void SetActivePanels(bool status) {
            Debug.Log("SetActivePanels: " + status);
            charactersPanel.SetActive(status);
            //SetActiveItemsPanel(false);

            foreach (GameObject item in itemsPanel.Items)
                item.transform.GetChild(0).GetComponent<Button>().interactable = status;
            nightButton.gameObject.GetComponent<Button>().interactable = status;
        }
    }

}