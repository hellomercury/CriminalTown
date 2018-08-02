using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Collections.ObjectModel;

public enum RobberyType {
    None,
    DarkStreet,
    Stall,
    House,
    Shop,
    Band
}

public enum RobberyProperty {
    Description,
    Name,
    DescriptionFull,
    StrenghtInfluence,
    AgilityInfluence,
    SkillInfluence,
    LuckInfluence
}

[System.Serializable]
public class Robbery {
    private readonly RobberyType m_robberyType;
    private readonly int m_locationNum;

    private readonly int m_strength;
    private readonly int m_agility;
    private readonly int m_skill;
    private readonly int m_luck;

    public List<Character> Characters {
        get {
            List<Character> characters = new List<Character>();
            foreach (Character character in DataScript.ChData.PanelCharacters) {
                if (character.RobberyType == m_robberyType && character.LocationNum == m_locationNum) {
                    characters.Add(character);
                }
            }
            return characters;
        }
    }

    //Constructor
    public Robbery(RobberyType robberyType, int locationNum, int strength, int agility, int skill, int luck) {
        m_robberyType = robberyType;
        m_locationNum = locationNum;
        m_strength = strength;
        m_agility = agility;
        m_skill = skill;
        m_luck = luck;
        Items = new Dictionary<int, int>();
    }

    public bool IsRobberyEmpty() {
        return Characters.Count == 0;
    }

    public void AddCharacter(Character character) {
        character.AddToRobbery(m_robberyType, m_locationNum);
        OnAddToRobEvent(character);
    }

    public void RemoveCharacter(Character character) {
        character.SetDefaultStatus();
        OnRemoveFromRobEvent(character);
    }

    public RobberyType RobberyType {
        get {
            return m_robberyType;
        }
    }

    public int LocationNum {
        get {
            return m_locationNum;
        }
    }

    public int Strength {
        get {
            return m_strength;
        }
    }

    public int Agility {
        get {
            return m_agility;
        }
    }

    public int Skill {
        get {
            return m_skill;
        }
    }

    public int Luck {
        get {
            return m_luck;
        }
    }

    public Dictionary<int, int> Items { get; set; }

    public delegate void RobberyEvent(Character character);

    public event RobberyEvent OnAddToRobEvent = delegate { };

    public event RobberyEvent OnRemoveFromRobEvent = delegate { };
}

public class RobberiesOptions : MonoBehaviour {
    public Sprite[] RobberySprites = new Sprite[5];

    public static int TypesAmount;
    public const int DarkStreetsAmount = 3;
    public const int StallsAmount = 3;

    private static TextAsset m_robberiesCollectionDataXml;
    private static XmlDocument m_xmlDoc;

    private static List<Dictionary<RobberyProperty, string>> m_robberiesCollection = new List<Dictionary<RobberyProperty, string>>();

    public static string GetRobberyData(RobberyType robberyType, RobberyProperty robberyProperty) {
        return m_robberiesCollection[(int) robberyType][robberyProperty];
    }

    private static Dictionary<RobberyProperty, string> m_robberyDict;

    public static void GetNewRobberies() {
        if (DataScript.EData.RobberiesData != null)
            DataScript.EData.RobberiesData.Clear();

        DataScript.EData.RobberiesData = new Dictionary<RobberyType, Dictionary<int, Robbery>>() {
            {RobberyType.DarkStreet, new Dictionary<int, Robbery>()},
            {RobberyType.Stall, new Dictionary<int, Robbery>()}
        };

        Robbery robbery0 = GetRandomRobbery(RobberyType.DarkStreet, 0);
        DataScript.EData.RobberiesData[RobberyType.DarkStreet].Add(0, robbery0);

        Robbery robbery1 = GetRandomRobbery(RobberyType.Stall, 0);
        DataScript.EData.RobberiesData[RobberyType.Stall].Add(0, robbery1);

        Robbery robbery2 = GetRandomRobbery(RobberyType.DarkStreet, 1);
        DataScript.EData.RobberiesData[RobberyType.DarkStreet].Add(1, robbery2);

        Robbery robbery3 = GetRandomRobbery(RobberyType.DarkStreet, 2);
        DataScript.EData.RobberiesData[RobberyType.DarkStreet].Add(2, robbery3);

        Robbery robbery4 = GetRandomRobbery(RobberyType.Stall, 1);
        DataScript.EData.RobberiesData[RobberyType.Stall].Add(1, robbery4);

        Robbery robbery5 = GetRandomRobbery(RobberyType.Stall, 2);
        DataScript.EData.RobberiesData[RobberyType.Stall].Add(2, robbery5);
    }

    public static Robbery GetRandomRobbery(RobberyType robberyType, int locationNum) {
        int rndRang = 0;

        switch (robberyType) {
            case RobberyType.DarkStreet:
                rndRang = 1;
                break;
            case RobberyType.Stall:
                rndRang = Random.Range(1, 3);
                break;
            case RobberyType.House:
                rndRang = Random.Range(2, 4);
                break;
            case RobberyType.Shop:
                rndRang = Random.Range(3, 5);
                break;
            case RobberyType.Band:
                rndRang = Random.Range(4, 6);
                break;
        }
        int[] stats = GetRandomRobberyStats(rndRang);
        return new
            Robbery(robberyType: robberyType, locationNum: locationNum, strength: stats[0], agility: stats[1], skill: stats[2], luck: stats[3]);
    }

    private static int[] GetRandomRobberyStats(int rank) {
        int[] stats = new int[4] {0, 0, 0, 0};
        for (int i = 0; i < rank; i++) {
            int[] randStats = CharactersOptions.GetRandomStats(CharactersOptions.GetRandomCharLevelAtCurrentMoment());
            stats[0] += randStats[0];
            stats[1] += randStats[1];
            stats[2] += randStats[2];
            stats[3] += Random.Range(0, 5);
        }
        return stats;
    }

    public static int GetRobberyMoneyRewardAtTheCurrentMoment(RobberyType robberyType) {
        return 1000;
    }

    public static Dictionary<int, int> GetRobberyAwardsAtTheCurrentMoment(RobberyType robberyType) {
        return new Dictionary<int, int> {{Random.Range(0, 3), Random.Range(0, 3)}};
    }

    public static float CalculatePreliminaryChance(Robbery robbery) {
        float chance;
        List<Trait> chanceTraits;

        int rStrength = robbery.Strength;
        int rAgility = robbery.Agility;
        int rSkill = robbery.Skill;
        int rLuck = robbery.Luck;

        float banditsStats = CalculateBanditsStats(robbery, out chanceTraits);
        float robberyStats = rStrength + rAgility + rSkill + rLuck;
        float equipmentStats = CalcucateEquipmentStats(robbery);

        Debug.Log(robbery.RobberyType + " " + robbery.LocationNum);
        Debug.Log("banditsStats: " + banditsStats);
        Debug.Log("robberyStats: " + robberyStats);
        Debug.Log("equipmentStats: " + equipmentStats);

        chance = (banditsStats + equipmentStats) / (banditsStats + equipmentStats + robberyStats);

        Debug.Log(chance);

        foreach (Trait chanceTrait in chanceTraits) {
            switch (chanceTrait.stat) {
                case Stat.chance:
                    chance *= chanceTrait.value;
                    break;
                default:
                    break;
            }
        }

        chance = Random.Range(0, 100);
        return chance;
    }

    private static float CalculateBanditsStats(Robbery robbery, out List<Trait> chanceTraits) {
        int count = 0;
        RobberyType rType = robbery.RobberyType;

        chanceTraits = new List<Trait>();
        List<Trait> groupTraits = new List<Trait>();

        float cStrength = 0;
        float cAgility = 0;
        float cSkill = 0;
        float cLuck = 0;
        float cFear = 0;

        foreach (Character character in robbery.Characters) {
            count++;
            float coefStr = 1, coefAg = 1, coefSk = 1, coefL = 1, coefF = 1;

            if (character.GetType() == typeof(SpecialCharacter)) {
                SpecialCharacter spChar = (SpecialCharacter) character;
                foreach (Trait trait in spChar.Traits) {
                    //При редактировании трейтов ДОБАВИТЬ ИХ СЮДА!!!!!
                    switch (trait.traitType) {
                        case TraitType.single:
                            switch (trait.stat) {
                                case Stat.strenght:
                                    coefStr = trait.value;
                                    break;
                                case Stat.luck:
                                    coefL = trait.value;
                                    break;
                                case Stat.fear:
                                    coefF = trait.value;
                                    break;
                                case Stat.skill:
                                    coefSk = trait.value;
                                    break;
                                case Stat.agility:
                                    coefAg = trait.value;
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case TraitType.group:
                            groupTraits.Add(trait);
                            break;
                        case TraitType.chance:
                            chanceTraits.Add(trait);
                            break;
                    }
                }
            }

            cStrength += (character.Stats.Strength * coefStr);
            cAgility += (character.Stats.Agility * coefAg);
            cSkill += (character.Stats.Skill * coefSk);
            cLuck += (character.Stats.Luck * coefL);
            cFear += (character.Stats.Fear * coefF);
        }

        foreach (Trait groupTrait in groupTraits) {
            switch (groupTrait.stat) {
                case Stat.strenght:
                    cStrength *= groupTrait.value;
                    break;
                case Stat.luck:
                    cLuck *= groupTrait.value;
                    break;
                case Stat.fear:
                    cFear *= groupTrait.value;
                    break;
                case Stat.skill:
                    cSkill *= groupTrait.value;
                    break;
                case Stat.agility:
                    cAgility *= groupTrait.value;
                    break;
                default:
                    break;
            }
        }

        return
            (cStrength * float.Parse(GetRobberyData(rType, RobberyProperty.StrenghtInfluence)) +
                cLuck * float.Parse(GetRobberyData(rType, RobberyProperty.LuckInfluence)) +
                cAgility * float.Parse(GetRobberyData(rType, RobberyProperty.AgilityInfluence)) +
                cSkill * float.Parse(GetRobberyData(rType, RobberyProperty.SkillInfluence)))
            *
            (1 - cFear / (110 * count));
    }

    private static float CalcucateEquipmentStats(Robbery robbery) {
        Dictionary<int, int> items = robbery.Items;

        float itemsStats = 0;

        foreach (int itemNum in items.Keys) {
            switch (robbery.RobberyType) {
                case RobberyType.DarkStreet:
                    itemsStats += items[itemNum] * float.Parse(ItemsOptions.GetItemData(itemNum)[ItemProperty.influence0]);
                    break;
                case RobberyType.Stall:
                    itemsStats += items[itemNum] * float.Parse(ItemsOptions.GetItemData(itemNum)[ItemProperty.influence1]);
                    break;
                case RobberyType.House:
                    itemsStats += items[itemNum] * float.Parse(ItemsOptions.GetItemData(itemNum)[ItemProperty.influence2]);
                    break;
                case RobberyType.Shop:
                    itemsStats += items[itemNum] * float.Parse(ItemsOptions.GetItemData(itemNum)[ItemProperty.influence3]);
                    break;
                case RobberyType.Band:
                    itemsStats += items[itemNum] * float.Parse(ItemsOptions.GetItemData(itemNum)[ItemProperty.influence4]);
                    break;
            }
        }

        return itemsStats;
    }

    public static void GetRobberiesCollectionData() {
        m_robberiesCollectionDataXml = Resources.Load("RobberiesCollectionData") as TextAsset;

        m_xmlDoc = new XmlDocument();
        if (m_robberiesCollectionDataXml) {
            m_xmlDoc.LoadXml(m_robberiesCollectionDataXml.text);
            XmlNode allRobberies = m_xmlDoc.SelectSingleNode("./robberies");
            foreach (XmlNode robbery in allRobberies) {
                m_robberyDict = new Dictionary<RobberyProperty, string>();
                XmlNodeList robOptions = robbery.ChildNodes;
                foreach (XmlNode rOption in robOptions) {
                    if (rOption.Name == "name")
                        m_robberyDict.Add(RobberyProperty.Name, rOption.InnerText);
                    if (rOption.Name == "description")
                        m_robberyDict.Add(RobberyProperty.Description, rOption.InnerText);
                    if (rOption.Name == "descriptionFull")
                        m_robberyDict.Add(RobberyProperty.DescriptionFull, rOption.InnerText);
                    if (rOption.Name == "strenghtInfluence")
                        m_robberyDict.Add(RobberyProperty.StrenghtInfluence, rOption.InnerText);
                    if (rOption.Name == "agilityInfluence")
                        m_robberyDict.Add(RobberyProperty.AgilityInfluence, rOption.InnerText);
                    if (rOption.Name == "skillInfluence")
                        m_robberyDict.Add(RobberyProperty.SkillInfluence, rOption.InnerText);
                    if (rOption.Name == "luckInfluence")
                        m_robberyDict.Add(RobberyProperty.LuckInfluence, rOption.InnerText);
                }
                m_robberiesCollection.Add(m_robberyDict);
                TypesAmount++;
            }
        } else {
            Debug.LogError("Ошибка загрузки XML файла с данными об ограблениях!");
        }
    }
}