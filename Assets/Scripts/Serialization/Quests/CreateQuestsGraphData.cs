using UnityEditor;
using UnityEngine;

namespace CriminalTown.Serialization {

    public class CreateQuestsGraphData : MonoBehaviour {
        [MenuItem("CriminalTown/Create/Create quests graph data")]
        public static QuestsGraphScriptableObject Create() {
            QuestsGraphScriptableObject data = ScriptableObject.CreateInstance<QuestsGraphScriptableObject>();

            AssetDatabase.CreateAsset(data, "Assets/Scripts/Serialization/Quests/QuestsGraphData.asset");
            AssetDatabase.SaveAssets();
            return data;
        }
    }

}