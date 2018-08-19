using System.Collections.Generic;
using UnityEngine;

namespace CriminalTown {

    public class QuestsScriptableObject : ScriptableObject {
        [SerializeField]
        private List<HireCharacterQuest> m_hireCharacterQuests;
        [SerializeField]
        private List<KickCharacterQuest> m_kickCharacterQuests;
        [SerializeField]
        private List<StatusCharacterQuest> m_statusCharacterQuests;
        [SerializeField]
        private List<LevelUpCharacterQuest> m_levelUpCharacterQuests;
        [SerializeField]
        private List<StatsUpCharacterQuest> m_statsUpCharacterQuests;
        [SerializeField]
        private List<ItemQuest> m_itemQuests;
        [SerializeField]
        private List<RobberyQuest> m_robberyQuests;
        [SerializeField]
        private List<EducationQuest> m_educationQuests;
        [SerializeField]
        private List<ChoiceQuest> m_choiceQuests;

        public List<HireCharacterQuest> HireCharacterQuests {
            get {
                return m_hireCharacterQuests;
            }
        }

        public List<KickCharacterQuest> KickCharacterQuests {
            get {
                return m_kickCharacterQuests;
            }
        }

        public List<StatusCharacterQuest> StatusCharacterQuests {
            get {
                return m_statusCharacterQuests;
            }
        }

        public List<LevelUpCharacterQuest> LevelUpCharacterQuests {
            get {
                return m_levelUpCharacterQuests;
            }
        }

        public List<StatsUpCharacterQuest> StatsUpCharacterQuests {
            get {
                return m_statsUpCharacterQuests;
            }
        }

        public List<ItemQuest> ItemQuests {
            get {
                return m_itemQuests;
            }
        }

        public List<RobberyQuest> RobberyQuests {
            get {
                return m_robberyQuests;
            }
        }

        public List<EducationQuest> EducationQuests {
            get {
                return m_educationQuests;
            }
        }

        public List<ChoiceQuest> ChoiceQuests {
            get {
                return m_choiceQuests;
            }
        }
    }

}

