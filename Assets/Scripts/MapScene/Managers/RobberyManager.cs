using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace CriminalTown {

    public class RobberyManager : MonoBehaviour {
        private static RobberyManager m_rmInstance;

        public static RobberyManager RmInstance {
            get {
                return m_rmInstance;
            }
        }

        [SerializeField]
        private GameObject[] m_darkStreets;
        [SerializeField]
        private GameObject[] m_stalls;
        private Dictionary<RobberyType, GameObject[]> m_robberiesObjects;

        public Dictionary<RobberyType, GameObject[]> RobberiesObjects {
            get {
                return m_robberiesObjects;
            }
        }
        
        private void Awake() {
            m_robberiesObjects = new Dictionary<RobberyType, GameObject[]>() {
                {RobberyType.DarkStreet, m_darkStreets},
//                {RobberyType.Stall, m_stalls}
            };
            m_rmInstance = gameObject.GetComponent<RobberyManager>();
            CustomizeRobberies();
            UpdateRobberies();

            Night.Instance.OnNightBegan += DeactivateAllRobberies;
            Night.Instance.OnNightEnded += UpdateRobberies;
        }

        public void UpdateRobberies() {
            DeactivateAllRobberies();

            foreach (RobberyType robType in DataScript.EData.RobberiesData.Keys)
                foreach (int locNum in DataScript.EData.RobberiesData[robType].Keys)
                    ActivateRobbery(robType, locNum);
        }

        public void DeactivateAllRobberies() {
            foreach (RobberyType robberyType in m_robberiesObjects.Keys) {
                for (int i = 0; i < m_robberiesObjects[robberyType].Length; i++) {
                    GameObject robbery = m_robberiesObjects[robberyType][i];
                    robbery.GetComponentInChildren<Button>().interactable = false;
                    robbery.GetComponent<RobberyCustomization>().ActivateRobbery(false);
                }
            }
        }

        private void CustomizeRobberies() {
            foreach (RobberyType robberyType in m_robberiesObjects.Keys) {
                for (int i = 0; i < m_robberiesObjects[robberyType].Length; i++) {
                    GameObject robbery = m_robberiesObjects[robberyType][i];
                    robbery.GetComponent<RobberyCustomization>().CustomizeRobbery(i, RobberyType.DarkStreet);
                }
            }
        }

        public void ActivateRobbery(RobberyType robType, int locationNum) {
            m_robberiesObjects[robType][locationNum].GetComponentInChildren<Button>().interactable = true;
            m_robberiesObjects[robType][locationNum].GetComponent<RobberyCustomization>().ActivateRobbery(true);
        }

        public void AddNightEvent(RobberyType robType, int locationNum, UnityAction windowSettings, EventStatus eventStatus, float eventTime) {
            m_robberiesObjects[robType][locationNum].GetComponentInChildren<RobberyCustomization>().AddNightEvent(windowSettings, eventStatus, eventTime);
        }

        public void ResetNightEvent(RobberyType robType, int locationNum) {
            m_robberiesObjects[robType][locationNum].GetComponentInChildren<RobberyCustomization>().ResetNightEvent();
        }
    }

}