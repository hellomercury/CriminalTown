﻿using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace CriminalTown {

    public enum TraitType {
        single,
        group,
        chance
    }

    public enum Stat {
        strenght,
        luck,
        fear,
        skill,
        agility,
        health,
        tiredness,
        chance,
        hospitalChance,
        policeChance,
        policeKnowledge
    }

    public class Trait {
        public int id;
        public string name;
        public string description;
        public TraitType traitType;
        public Stat stat;
        public float value;

        public Sprite Sprite {
            get {
                return TraitsOptions.GetTraitSprite(id);
            }
        }
    }

    public class TraitsOptions : MonoBehaviour {
        private static TraitsOptions m_instance;

        public static TraitsOptions Instance {
            get {
                return m_instance;
            }
        }
        
        public Sprite[] traitsSprites = new Sprite[15];

        private static Dictionary<int, Trait> traitsCollection;

        private static TextAsset TraitsCollectionDataXml;
        private static XmlDocument xmlDoc;

        public void Initialize() {
            m_instance = GetComponent<TraitsOptions>();
            GetTraitsCollectionData();
        }
        
        public static Trait GetTrait(int traitId) {
            return traitsCollection[traitId];
        }

        public static Sprite GetTraitSprite(int traitId) {
            return m_instance.traitsSprites[traitId];
        }

        public static void GetTraitsCollectionData() {
            TraitsCollectionDataXml = Resources.Load("TraitsCollectionData") as TextAsset;
            xmlDoc = new XmlDocument();
            if (TraitsCollectionDataXml) {
                traitsCollection = new Dictionary<int, Trait>();
                xmlDoc.LoadXml(TraitsCollectionDataXml.text);
                XmlNode allTraits = xmlDoc.SelectSingleNode("./traitsCollection");
                foreach (XmlNode trait in allTraits) {
                    Trait newTrait = new Trait() {id = int.Parse(trait.Attributes["id"].Value)};
                    foreach (XmlNode info in trait) {
                        if (info.Name == "name")
                            newTrait.name = info.InnerText;
                        if (info.Name == "description")
                            newTrait.description = info.InnerText;
                        if (info.Name == "traitType")
                            newTrait.traitType = (TraitType) System.Enum.Parse(typeof(TraitType), info.InnerText);
                        if (info.Name == "stat")
                            newTrait.stat = (Stat) System.Enum.Parse(typeof(Stat), info.InnerText);
                        if (info.Name == "value")
                            newTrait.value = float.Parse(info.InnerText);
                    }
                    traitsCollection.Add(int.Parse(trait.Attributes["id"].Value), newTrait);
                }
            } else {
                Debug.LogError("Ошибка загрузки XML файла с данными о трейтах!");
            }
        }
    }

}