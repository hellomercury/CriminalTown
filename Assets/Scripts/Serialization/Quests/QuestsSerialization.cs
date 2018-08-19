using System;
using UnityEngine;

namespace CriminalTown.Serialization {

    [Serializable]
    public class QuestsSerialization : MonoBehaviour {
        private static QuestsSerialization m_instance;

        public static QuestsSerialization Instance {
            get {
                if (m_instance == null) {
                    m_instance = FindObjectOfType<QuestsSerialization>();
                }
                return m_instance;
            }
        }
        
        [SerializeField]
        private QuestsScriptableObject m_questsData;

        public QuestsScriptableObject QuestsData {
            get {
                return m_questsData;
            }
        }

        public static QuestsScriptableObject GetQuestsCollection {
            get {
                return Instance.QuestsData;
            }
        }
        
    }
    
}