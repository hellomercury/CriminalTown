using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using UnityEngine;

namespace CriminalTown.Serialization {

    [Serializable]
    public class QuestsCollection {
        public int Test;
        public Dictionary<int, Quest> Quests;

        public QuestsCollection() {
            Quests = new Dictionary<int, Quest>() {
                {0, new Quest()}
            };
        }
    }

    public static class QuestsSerialization {

        [CanBeNull]
        public static QuestsCollection Deserialize() {
            string filePath = Application.dataPath + "/StreamingAssets/";
            if (!File.Exists(filePath)) {
                return new QuestsCollection();
            }
            string dataAsJson = File.ReadAllText(filePath);
            QuestsCollection questsCollection = JsonUtility.FromJson<QuestsCollection>(dataAsJson);
            return questsCollection == null ? new QuestsCollection() : questsCollection;
        }

        public static void Serialize([NotNull] QuestsCollection questsCollection) {
            string dataAsJson = JsonUtility.ToJson(questsCollection);

            string filePath = Application.dataPath + "/StreamingAssets/";
            File.WriteAllText(filePath, dataAsJson);
        }
    }

}