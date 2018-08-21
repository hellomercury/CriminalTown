using UnityEditor;
using UnityEngine;

namespace CriminalTown.Serialization {

    public class CreateQuestsData {
        //[MenuItem("CriminalTown/Create/Create quests data")]
        public static QuestsScriptableObject Create() {
            QuestsScriptableObject data = ScriptableObject.CreateInstance<QuestsScriptableObject>();

            AssetDatabase.CreateAsset(data, "Assets/Scripts/Serialization/Quests/QuestsCollectionData.asset");
            AssetDatabase.SaveAssets();
            return data;
        }
    }

}