using System;
using UnityEngine;

namespace CriminalTown.Serialization {

    [Serializable]
    public class QuestsSerialization : MonoBehaviour {

        public static QuestsScriptableObject GetQuestsCollection {
            get {
                return Resources.Load<QuestsScriptableObject>("QuestsCollectionData");
            }
        }
        
        public static QuestsGraphScriptableObject GetQuestsGraphData {
            get {
                return Resources.Load<QuestsGraphScriptableObject>("QuestsGraphData");
            }
        }
    }
    
}