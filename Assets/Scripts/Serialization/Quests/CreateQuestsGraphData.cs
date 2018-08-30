using UnityEditor;
using UnityEngine;

namespace CriminalTown.Serialization {

    public class CreateQuestsGraphData : MonoBehaviour {
        [MenuItem("CriminalTown/Create/Create quests graph data")]
        public static QuestsScriptableObject Create() {
            QuestsScriptableObject data = ScriptableObject.CreateInstance<QuestsScriptableObject>();

            AssetDatabase.CreateAsset(data, "Assets/Scripts/Serialization/Quests/QuestsGraphData.asset");
            AssetDatabase.SaveAssets();
            return data;
        }
    }

}

