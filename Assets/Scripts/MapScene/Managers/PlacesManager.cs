using UnityEngine;
using UnityEngine.UI;

namespace CriminalTown {

    public class PlacesManager : MonoBehaviour {
        private static PlacesManager m_instance;

        public static PlacesManager Instance {
            get {
                return m_instance;
            }
        }
        
        public GameObject hospitalObject;
        public GameObject policeStationObject;
        public GameObject blackMarketObject;
        public GameObject banditCampObject;
        public GameObject testPanelObject;

        public static Button hospital;
        public static Button policeStation;
        public static Button blackMarket;
        public static Button banditCamp;
        public static Button testPanel;

        public void Initialize() {
            m_instance = GetComponent<PlacesManager>();
            
            hospital = hospitalObject.GetComponentInChildren<Button>();
            policeStation = policeStationObject.GetComponent<Button>();
            blackMarket = blackMarketObject.GetComponent<Button>();
            banditCamp = banditCampObject.GetComponent<Button>();
            testPanel = testPanelObject.GetComponent<Button>();

            Night.Instance.OnNightBegan += () => SetActiveAllPlaces(false);
            Night.Instance.OnNightEnded += () => SetActiveAllPlaces(true);
        }

        public static void SetActiveAllPlaces(bool status) {
            hospital.interactable = status;
            policeStation.interactable = status;
            blackMarket.interactable = status;
            banditCamp.interactable = status;
            testPanel.interactable = status;
        }
    }

}