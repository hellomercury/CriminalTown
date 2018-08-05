using UnityEngine;

namespace CriminalTown {

    public class OptionsManager : MonoBehaviour {
        [SerializeField]
        private CharactersOptions m_charactersOptions;
        [SerializeField]
        private ItemsOptions m_itemsOptions;
        [SerializeField]
        private RobberiesOptions m_robberiesOptions;
        [SerializeField]
        private NightEventsOptions m_nightEventsOprions;
        [SerializeField]
        private TraitsOptions m_traitsOptions;

        public void Initialize() {
            m_charactersOptions.Initialize(); 
            m_itemsOptions.Initialize(); 
            m_robberiesOptions.Initialize(); 
            m_nightEventsOprions.Initialize(); 
            m_traitsOptions.Initialize(); 
        }
    }

}