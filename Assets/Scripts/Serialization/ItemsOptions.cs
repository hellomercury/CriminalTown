using System.Collections.Generic;
using UnityEngine;
using System.Xml;

//using ItemProperty = System.String;

public enum ItemProperty
{
    influence0 = 0,
    influence1 = 1,
    influence2 = 2,
    influence3 = 3,
    influence4 = 4,
    isSpecial,
    name,
    description,
    price
}

public struct ItemName
{
    public string name;
}

namespace CriminalTown {

    public class ItemsOptions : MonoBehaviour {
        private static ItemsOptions m_instance;

        public static ItemsOptions Instance {
            get {
                return m_instance;
            }
        }

        
        private static TextAsset m_itemsCollectionDataXml;
        private static XmlDocument m_xmlDoc;


        public static int totalAmount = 0;


        public Sprite[] itemsSprites = new Sprite[totalAmount];

        private static Dictionary<int, Dictionary<ItemProperty, string>> itemsCollection = new Dictionary<int, Dictionary<ItemProperty, string>>();
        private static Dictionary<ItemProperty, string> itemDict;


        public static Dictionary<ItemProperty, string> GetItemData(int num) {
            return itemsCollection[num];
        }

        public void Initialize() {
            m_instance = GetComponent<ItemsOptions>();
            GetItemsCollectionData();
        }

        public static void GetItemsCollectionData() {
            m_itemsCollectionDataXml = Resources.Load("ItemsCollectionData") as TextAsset;

            m_xmlDoc = new XmlDocument();
            if (m_itemsCollectionDataXml) {
                m_xmlDoc.LoadXml(m_itemsCollectionDataXml.text);
                XmlNode allItems = m_xmlDoc.SelectSingleNode("./items");
                foreach (XmlNode item in allItems) {
                    itemDict = new Dictionary<ItemProperty, string> {
                        {ItemProperty.isSpecial, item.Attributes["isSpecial"].Value}
                    };
                    XmlNodeList itemOptions = item.ChildNodes;
                    foreach (XmlNode iOption in itemOptions) {
                        if (iOption.Name == "name")
                            itemDict.Add(ItemProperty.name, iOption.InnerText);
                        if (iOption.Name == "price")
                            itemDict.Add(ItemProperty.price, iOption.InnerText);
                        if (iOption.Name == "influence")
                            itemDict.Add((ItemProperty) System.Enum.Parse(typeof(ItemProperty), iOption.Attributes["place"].Value), iOption.InnerText);
                    }
                    itemsCollection.Add(int.Parse(item.Attributes["number"].Value), itemDict);
                    totalAmount++;
                }
            } else {
                Debug.LogError("Ошибка загрузки XML файла с данными об игровых предметах!");
            }
        }
    }

}