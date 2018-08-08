using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace CriminalTown {

    public class RobberiesManager : MonoBehaviour {
        private static RobberiesManager m_instance;

        public static RobberiesManager Instance {
            get {
                return m_instance;
            }
        }

        [SerializeField]
        private RobberyCustomization[] m_darkStreets;
        [SerializeField]
        private RobberyCustomization[] m_stalls;
        private Dictionary<RobberyType, RobberyCustomization[]> m_robberies;

        public Dictionary<RobberyType, RobberyCustomization[]> Robberies {
            get {
                return m_robberies;
            }
        }

        public void Initialize() {
            m_robberies = new Dictionary<RobberyType, RobberyCustomization[]>() {
                {RobberyType.DarkStreet, m_darkStreets},
//                {RobberyType.Stall, m_stalls}
            };
            m_instance = gameObject.GetComponent<RobberiesManager>();
            CustomizeRobberies();
            UpdateRobberies();

            Night.Instance.OnNightBegan += DeactivateAllRobberies;
            Night.Instance.OnNightEnded += UpdateRobberies;
        }

        public void UpdateRobberies() {
            DeactivateAllRobberies();

            if (DataScript.EData == null) {
                return;
            }
            if (DataScript.EData.RobberiesData == null) {
                return;
            }
            foreach (RobberyType robType in DataScript.EData.RobberiesData.Keys)
                foreach (int locNum in DataScript.EData.RobberiesData[robType].Keys)
                    ActivateRobbery(robType, locNum);
        }

        public void DeactivateAllRobberies() {
            foreach (RobberyType robberyType in m_robberies.Keys) {
                for (int locNum = 0; locNum < m_robberies[robberyType].Length; locNum++) {
                    m_robberies[robberyType][locNum].ActivateRobbery(false);
                }
            }
        }

        private void CustomizeRobberies() {
            foreach (RobberyType robberyType in m_robberies.Keys) {
                for (int locNum = 0; locNum < m_robberies[robberyType].Length; locNum++) {
                    m_robberies[robberyType][locNum].CustomizeRobbery(locNum, RobberyType.DarkStreet);
                }
            }
        }

        public void ActivateRobbery(RobberyType robType, int locationNum) {
            m_robberies[robType][locationNum].ActivateRobbery(true);
        }

        public void AddNightEvent(RobberyType robType, int locationNum, UnityAction windowSettings, EventStatus eventStatus, float eventTime) {
            m_robberies[robType][locationNum].AddNightEvent(windowSettings, eventStatus, eventTime);
        }

        public void ResetNightEvent(RobberyType robType, int locationNum) {
            m_robberies[robType][locationNum].ResetNightEvent();
        }
    }

}