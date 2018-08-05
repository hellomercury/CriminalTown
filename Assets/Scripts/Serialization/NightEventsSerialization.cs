using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace CriminalTown {

    public class NightEventsSerialization : MonoBehaviour {

        private static TextAsset m_nightEventsCollectionDataXml;
        private static XmlDocument m_xmlDoc;

        public static Dictionary<RobberyType, NightEvent[]> GetNightEventsCollection() {
            Dictionary<RobberyType, NightEvent[]> eventsCollection = new Dictionary<RobberyType, NightEvent[]>();
            foreach (RobberyType robberyType in Enum.GetValues(typeof(RobberyType))) {
                if (robberyType != RobberyType.None) {
                    eventsCollection.Add(robberyType, GetEventsForRobberyType(robberyType));
                }
            }
            return eventsCollection;
        }

        public static NightEvent[] GetEventsForRobberyType(RobberyType robberyType) {
            if (m_xmlDoc == null)
                GetNightEventsCollectionData();
            XmlNode eventsOfNeededType = m_xmlDoc.SelectSingleNode("./eventsCollection/robbery[@type='" + robberyType.ToString("d").ToLower() + "']");

            int eventsCount = eventsOfNeededType.ChildNodes.Count;
            NightEvent[] nightEvents = new NightEvent[eventsCount];

            for (int eventNumber = 0; eventNumber < eventsCount; eventNumber++) {
                nightEvents[eventNumber] = new NightEvent();
                XmlNode eventRootNode = eventsOfNeededType.SelectSingleNode("./event[@id='" + eventNumber + "']");

                if (eventRootNode.SelectSingleNode("./node") != null) {
                    nightEvents[eventNumber].RootNode = ParseEventRandomTree(eventRootNode.SelectSingleNode("./node"), robberyType);
                }
                nightEvents[eventNumber].Success = ParseEventRandomTree(eventRootNode.SelectSingleNode("./success"), robberyType);
                nightEvents[eventNumber].Fail = ParseEventRandomTree(eventRootNode.SelectSingleNode("./fail"), robberyType);
            }
            return nightEvents;
        }

        /// <summary>Get night event tree with random nodes from "NightEventsCollectionData.xml"</summary>
        /// <param name="eventNode">A root node of event in "NightEventsCollectionData.xml"</param>
        public static NightEventNode ParseEventRandomTree(XmlNode eventNode, RobberyType robberyType) {
            foreach (XmlNode node in eventNode.ChildNodes) {
                Debug.Log(node.Name);
            }
            NightEventNode nightEvent = new NightEventNode();
            foreach (XmlNode info in eventNode) {
                nightEvent.TitleText = RobberiesOptions.GetRobberyData(robberyType, RobberyProperty.Name);
                Debug.Log(info.Name + " " + info.InnerText);
                switch (info.Name) {
                    case "text":
                        nightEvent.Description = info.InnerText;
                        break;
                    case "sprite":
                        foreach (XmlNode spriteInfo in info) {
                            if (spriteInfo.Name == "spriteType") {
                                switch (spriteInfo.InnerText) {
                                    case "people":
                                        nightEvent.SpriteType = SpriteType.People;
                                        break;
                                    case "characters":
                                        nightEvent.SpriteType = SpriteType.Characters;
                                        break;
                                    case "items":
                                        nightEvent.SpriteType = SpriteType.Items;
                                        break;
                                    case "places":
                                        nightEvent.SpriteType = SpriteType.Places;
                                        break;
                                    case "robberies":
                                        nightEvent.SpriteType = SpriteType.Robberies;
                                        break;
                                    default:
                                        Debug.LogError("No \"" + spriteInfo.InnerText + "\" sprite type exists! Check NightEventsCollectionData.xml or SpriteType enum");
                                        nightEvent.SpriteType = SpriteType.None;
                                        break;
                                }
                            }
                            if (spriteInfo.Name == "spriteId")
                                nightEvent.SpriteId = int.Parse(spriteInfo.InnerText);
                        }
                        break;
                    case "button":
                        if (nightEvent.Buttons == null)
                            nightEvent.Buttons = new List<NightEventButtonDetails>();
                        NightEventButtonDetails button = new NightEventButtonDetails();
                        foreach (XmlNode buttonInfo in info) {
                            if (buttonInfo.Name == "text")
                                button.ButtonText = buttonInfo.InnerText;
                            if (buttonInfo.Name == "effect")
                                button.Effect = int.Parse(buttonInfo.InnerText);
                            if (buttonInfo.Name == "policeEffect")
                                button.PoliceEffect = int.Parse(buttonInfo.InnerText);
                            if (buttonInfo.Name == "hospitalEffect")
                                button.HospitalEffect = int.Parse(buttonInfo.InnerText);
                            if (buttonInfo.Name == "healthAffect")
                                button.HealthAffect = int.Parse(buttonInfo.InnerText);
                            if (buttonInfo.Name == "policeKnowledge")
                                button.PoliceKnowledge = int.Parse(buttonInfo.InnerText);
                            if (buttonInfo.Name == "award") {
                                int itemId;
                                if (int.TryParse(buttonInfo.Attributes["itemId"].Value, out itemId)) {
                                    if (button.Awards == null)
                                        button.Awards = new Dictionary<int, int>();
                                    {
                                        if (!button.Awards.ContainsKey(itemId))
                                            button.Awards.Add(itemId, int.Parse(buttonInfo.InnerText));
                                        else
                                            button.Awards[itemId] += int.Parse(buttonInfo.InnerText);
                                    }
                                    ;
                                }
                            }
                            if (buttonInfo.Name == "money")
                                button.Money = int.Parse(buttonInfo.InnerText);
                        }
                        if (info.SelectSingleNode("./node") != null) //is button contains next nodes
                        {
                            Debug.Log("COUNT: " + info.SelectNodes("./node").Count);
                            button.NextEventNode = ParseEventRandomTree(info.SelectNodes("./node")[UnityEngine.Random.Range(0, info.SelectNodes("./node").Count)], robberyType); //Recursion
                        }
                        nightEvent.Buttons.Add(button);
                        break;
                }
                ;
            }
            return nightEvent;
        }

        public static void GetNightEventsCollectionData() {
            if (m_xmlDoc == null) {
                m_nightEventsCollectionDataXml = Resources.Load("NightEventsCollectionData") as TextAsset;
                if (m_nightEventsCollectionDataXml) {
                    m_xmlDoc = new XmlDocument();
                    m_xmlDoc.LoadXml(m_nightEventsCollectionDataXml.text);
                } else
                    Debug.LogError("Ошибка загрузки XML файла с данными о ночных событиях!");
            }
        }
    }

}