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

    }

}