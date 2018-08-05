using System;
using System.IO;
using System.Xml.Serialization;
using JetBrains.Annotations;
using UnityEngine;

namespace CriminalTown.Serialization {
    [Serializable, XmlRoot("robberiesCollection")]
    public class RobberiesCollection {
        [XmlArray("robberies")]
        [XmlArrayItem("robbery", typeof(RobberiesCollectionItem))]
        public RobberiesCollectionItem[] Robberies;

        [Serializable]
        public class RobberiesCollectionItem {
            [XmlAttribute("type", typeof(RobberyType))]
            public RobberyType Type;

            [XmlElement("name")]
            public string Name;
            [XmlElement("description")]
            public string Description;
            [XmlElement("descriptionFull")]
            public string DescriptionFull;
            [XmlElement("strenghtInfluence", typeof(float))]
            public float StrenghtInfluence;
            [XmlElement("agilityInfluence", typeof(float))]
            public float AgilityInfluence;
            [XmlElement("skillInfluence", typeof(float))]
            public float SkillInfluence;
            [XmlElement("luckInfluence", typeof(float))]
            public float LuckInfluence;
        }
    }

    public class RobberiesSerialization {
        [CanBeNull]
        public static RobberiesCollection GetRobberiesCollection() {
            TextAsset robberiesCollectionData = Resources.Load("RobberiesCollectionData") as TextAsset;
            if (robberiesCollectionData == null) {
                Debug.LogError("Can not open file RobberiesCollectionData.xml");
                return null;
            }
            XmlSerializer serializer = new XmlSerializer(typeof(RobberiesCollection));
            StringReader reader = new StringReader(robberiesCollectionData.text);
            RobberiesCollection rc = (RobberiesCollection) serializer.Deserialize(reader);
            reader.Close();
            return rc;
        }

    }

}