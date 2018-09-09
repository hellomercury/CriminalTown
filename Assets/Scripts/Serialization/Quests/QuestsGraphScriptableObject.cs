using System.Collections.Generic;
using System.Collections.ObjectModel;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace CriminalTown.Serialization {

    public class QuestsGraphScriptableObject : ScriptableObject {
        [SerializeField]
        private int m_lastId;
        [SerializeField]
        private List<Quest> m_quests;
        
        private static QuestsGraphScriptableObject m_questsGraphData;

        public static QuestsGraphScriptableObject Instance {
            get {
                if (m_questsGraphData == null) {
                    m_questsGraphData = Resources.Load<QuestsGraphScriptableObject>("QuestsGraphData");
                }
                return m_questsGraphData;
            }
        }
        
        public ReadOnlyCollection<Quest> Quests {
            get {
                if (m_quests == null) {
                    m_quests = new List<Quest>();
                }
                return m_quests.AsReadOnly();
            }
        }

        public void AddQuest(Quest quest) {
            if (m_quests == null) {
                m_quests = new List<Quest>();
            }
            AssetDatabase.CreateAsset(quest, "Assets/Scripts/Serialization/Quests/quest" + quest.Id + ".asset");
            m_quests.Add(quest);
            EditorUtility.SetDirty(this);
        }

        public void RemoveQuest(Quest quest) {
            
        }

        [CanBeNull]
        public Quest GetQuestById(int questId) {
            if (m_quests == null) {
                return null;
            }
            foreach (Quest quest in m_quests) {
                if (quest.Id == questId) {
                    return quest;
                }
            }
            return null;
        }

        /// <summary>
        /// Makes ids begin from zero and aligns them in natural number series.
        /// 0, 1, 2, 3 ....
        /// Why? I don't know.
        /// </summary>
        public void AlignIds() {
        }

        public static int GenerateId() {
            Instance.m_lastId++;
            return Instance.m_lastId;
        }

        private bool IsIdExists() {
            return false;
        }
    }

}

