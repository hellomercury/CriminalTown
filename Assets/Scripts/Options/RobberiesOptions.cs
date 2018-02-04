using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;
using System.Collections.ObjectModel;

public enum RobberyType { darkStreet, stall, house, shop, band }
public enum RobberyProperty
{
    description,
    name,
    descriptionFull,
    strenghtInfluence,
    agilityInfluence,
    skillInfluence,
    luckInfluence
}

[System.Serializable]
public class Robbery
{
    private RobberyType robberyType;
    private int locationNum;

    private int strength;
    private int agility;
    private int skill;
    private int luck;

    private List<Character> characters;

    //Constructor
    public Robbery(RobberyType robberyType, int locationNum, int strength, int agility, int skill, int luck)
    {
        this.robberyType = robberyType;
        this.locationNum = locationNum;
        //this.rank = rank;
        this.strength = strength;
        this.agility = agility;
        this.skill = skill;
        this.luck = luck;
        Items = new Dictionary<int, int>();
        characters = new List<Character>();
    }

    public bool IsRobberyEmpty()
    {
        return characters.Count == 0;
    }

    public void AddCharacter(Character character)
    {
        characters.Add(character);
        character.Status = CharacterStatus.robbery;
        character.RobberyType = robberyType;
        character.LocationNum = locationNum;
        DataScript.chData.OnRemoveEvent += RemoveCharacter; //For permanently deletion
        OnAddToRobEvent();
    }

    public void RemoveCharacter(Character character)
    {
        characters.Remove(character);
        character.Status = CharacterStatus.normal;
        character.RobberyType = 0;
        character.LocationNum = locationNum;
        DataScript.chData.OnRemoveEvent -= RemoveCharacter; //For permanently deletion
        OnRemoveFromRobEvent();
    }

    public RobberyType RobberyType { get { return robberyType; } }
    public int LocationNum { get { return locationNum; } }
    public int Strength { get { return strength; } }
    public int Agility { get { return agility; } }
    public int Skill { get { return skill; } }
    public int Luck { get { return luck; } }

    public Dictionary<int, int> Items { get; set; }
    public ReadOnlyCollection<Character> Characters { get { return characters.AsReadOnly(); } }

    public delegate void RobberyEvent();
    public event RobberyEvent OnAddToRobEvent = delegate { };
    public event RobberyEvent OnRemoveFromRobEvent = delegate { };
}

public class RobberiesOptions : MonoBehaviour
{
    public Sprite[] robberySprites = new Sprite[5];


    public static int typesAmount;
    public const int darkStreetsAmount = 3;
    public const int stallsAmount = 3;

    public static TextAsset RobberiesCollectionDataXml;
    public static XmlDocument xmlDoc;

    private static List<Dictionary<RobberyProperty, string>> robberiesCollection = new List<Dictionary<RobberyProperty, string>>();
    public static string GetRobberyData(RobberyType robberyType, RobberyProperty robberyProperty)
    {
        return robberiesCollection[(int)robberyType][robberyProperty];
    }

    private static Dictionary<RobberyProperty, string> robberyDict;

    public static void GetNewRobberies()
    {
        if (DataScript.eData.robberiesData != null) DataScript.eData.robberiesData.Clear();

        DataScript.eData.robberiesData = new Dictionary<RobberyType, Dictionary<int, Robbery>>()
        {
            { RobberyType.darkStreet, new Dictionary<int, Robbery>() },
            { RobberyType.stall, new Dictionary<int, Robbery>() }
        };

        Robbery robbery0 = GetRandomRobbery(RobberyType.darkStreet, 0);
        DataScript.eData.robberiesData[RobberyType.darkStreet].Add(0, robbery0);

        Robbery robbery1 = GetRandomRobbery(RobberyType.stall, 0);
        DataScript.eData.robberiesData[RobberyType.stall].Add(0, robbery1);

        Robbery robbery2 = GetRandomRobbery(RobberyType.darkStreet, 1);
        DataScript.eData.robberiesData[RobberyType.darkStreet].Add(1, robbery2);

        Robbery robbery3 = GetRandomRobbery(RobberyType.darkStreet, 2);
        DataScript.eData.robberiesData[RobberyType.darkStreet].Add(2, robbery3);

        Robbery robbery4 = GetRandomRobbery(RobberyType.stall, 1);
        DataScript.eData.robberiesData[RobberyType.stall].Add(1, robbery4);

        Robbery robbery5 = GetRandomRobbery(RobberyType.stall, 2);
        DataScript.eData.robberiesData[RobberyType.stall].Add(2, robbery5);

    }

    public static Robbery GetRandomRobbery(RobberyType robberyType, int locationNum)
    {
        int rndRang = 0;

        switch (robberyType)
        {
            case RobberyType.darkStreet:
                rndRang = 1;
                break;
            case RobberyType.stall:
                rndRang = Random.Range(1, 3);
                break;
            case RobberyType.house:
                rndRang = Random.Range(2, 4);
                break;
            case RobberyType.shop:
                rndRang = Random.Range(3, 5);
                break;
            case RobberyType.band:
                rndRang = Random.Range(4, 6);
                break;
        }
        int[] stats = GetRandomRobberyStats(rndRang);
        return new
            Robbery(robberyType: robberyType, locationNum: locationNum, strength: stats[0], agility: stats[1], skill: stats[2], luck: stats[3]);
    }

    private static int[] GetRandomRobberyStats(int rank)
    {
        int[] stats = new int[4] { 0, 0, 0, 0 };
        for (int i = 0; i < rank; i++)
        {
            int[] randStats = CharactersOptions.GetRandomStats(CharactersOptions.GetRandomCharLevelAtCurrentMoment());
            stats[0] += randStats[0];
            stats[1] += randStats[1];
            stats[2] += randStats[2];
            stats[3] += Random.Range(0, 5);
        }
        return stats;
    }

    public static int GetRobberyMoneyRewardAtTheCurrentMoment(RobberyType robberyType)
    {
        return 1000;
    }

    public static Dictionary<int, int> GetRobberyAwardsAtTheCurrentMoment(RobberyType robberyType)
    {
        return new Dictionary<int, int> { { Random.Range(0, 3), Random.Range(0, 3) } };
    }

    public static float CalculatePreliminaryChance(Robbery robbery)
    {
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

        foreach (Trait chanceTrait in chanceTraits)
        {
            switch (chanceTrait.stat)
            {
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

    private static float CalculateBanditsStats(Robbery robbery, out List<Trait> chanceTraits)
    {
        int count = 0;
        RobberyType rType = robbery.RobberyType;

        chanceTraits = new List<Trait>();
        List<Trait> groupTraits = new List<Trait>();

        float cStrength = 0;
        float cAgility = 0;
        float cSkill = 0;
        float cLuck = 0;
        float cFear = 0;

        foreach (Character character in robbery.Characters)
        {
            count++;
            float coefStr = 1, coefAg = 1, coefSk = 1, coefL = 1, coefF = 1;

            if (character.GetType() == typeof(SpecialCharacter))
            {
                SpecialCharacter spChar = (SpecialCharacter)character;
                List<Trait> traits = spChar.Traits;
                foreach (Trait trait in traits)
                {
                    //При редактировании трейтов ДОБАВИТЬ ИХ СЮДА!!!!!
                    switch (trait.traitType)
                    {
                        case TraitType.single:
                            switch (trait.stat)
                            {
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

            cStrength += (character.Strength * coefStr);
            cAgility += (character.Agility * coefAg);
            cSkill += (character.Skill * coefSk);
            cLuck += (character.Luck * coefL);
            cFear += (character.Fear * coefF);
        }

        foreach (Trait groupTrait in groupTraits)
        {
            switch (groupTrait.stat)
            {
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
            (cStrength * float.Parse(GetRobberyData(rType, RobberyProperty.strenghtInfluence)) +
            cLuck * float.Parse(GetRobberyData(rType, RobberyProperty.luckInfluence)) +
            cAgility * float.Parse(GetRobberyData(rType, RobberyProperty.agilityInfluence)) +
            cSkill * float.Parse(GetRobberyData(rType, RobberyProperty.skillInfluence)))
            *
            (1 - cFear / (110 * count));
    }

    private static float CalcucateEquipmentStats(Robbery robbery)
    {
        Dictionary<int, int> items = robbery.Items;

        float itemsStats = 0;

        foreach (int itemNum in items.Keys)
        {
            switch (robbery.RobberyType)
            {

                case RobberyType.darkStreet:
                    itemsStats += items[itemNum] * float.Parse(ItemsOptions.GetItemData(itemNum)[ItemProperty.influence0]);
                    break;
                case RobberyType.stall:
                    itemsStats += items[itemNum] * float.Parse(ItemsOptions.GetItemData(itemNum)[ItemProperty.influence1]);
                    break;
                case RobberyType.house:
                    itemsStats += items[itemNum] * float.Parse(ItemsOptions.GetItemData(itemNum)[ItemProperty.influence2]);
                    break;
                case RobberyType.shop:
                    itemsStats += items[itemNum] * float.Parse(ItemsOptions.GetItemData(itemNum)[ItemProperty.influence3]);
                    break;
                case RobberyType.band:
                    itemsStats += items[itemNum] * float.Parse(ItemsOptions.GetItemData(itemNum)[ItemProperty.influence4]);
                    break;
            }
        }

        return itemsStats;
    }

    public static void GetRobberiesCollectionData()
    {
        RobberiesCollectionDataXml = Resources.Load("RobberiesCollectionData") as TextAsset;

        xmlDoc = new XmlDocument();
        if (RobberiesCollectionDataXml)
        {
            xmlDoc.LoadXml(RobberiesCollectionDataXml.text);
            XmlNode allRobberies = xmlDoc.SelectSingleNode("./robberies");
            foreach (XmlNode robbery in allRobberies)
            {
                robberyDict = new Dictionary<RobberyProperty, string>();
                XmlNodeList robOptions = robbery.ChildNodes;
                foreach (XmlNode rOption in robOptions)
                {
                    if (rOption.Name == "name") robberyDict.Add(RobberyProperty.name, rOption.InnerText);
                    if (rOption.Name == "description") robberyDict.Add(RobberyProperty.description, rOption.InnerText);
                    if (rOption.Name == "descriptionFull") robberyDict.Add(RobberyProperty.descriptionFull, rOption.InnerText);
                    if (rOption.Name == "strenghtInfluence") robberyDict.Add(RobberyProperty.strenghtInfluence, rOption.InnerText);
                    if (rOption.Name == "agilityInfluence") robberyDict.Add(RobberyProperty.agilityInfluence, rOption.InnerText);
                    if (rOption.Name == "skillInfluence") robberyDict.Add(RobberyProperty.skillInfluence, rOption.InnerText);
                    if (rOption.Name == "luckInfluence") robberyDict.Add(RobberyProperty.luckInfluence, rOption.InnerText);
                }
                robberiesCollection.Add(robberyDict);
                typesAmount++;
            }
        }
        else
        {
            Debug.LogError("Ошибка загрузки XML файла с данными об ограблениях!");
        }
    }
}
