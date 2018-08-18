using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace CriminalTown.Serialization {

    [Serializable]
    public class QuestsSerialization : MonoBehaviour {
        public static QuestsSerialization Instance {
            get {
                return FindObjectOfType<QuestsSerialization>();
            }
        }
        
        [SerializeField]
        private List<Quest> m_quests;

        public List<Quest> Quests {
            get {
                return m_quests;
            }
        }
        
    }
    
}