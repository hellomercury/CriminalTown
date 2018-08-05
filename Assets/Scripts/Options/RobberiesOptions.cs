using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using CriminalTown.Serialization;

namespace CriminalTown {

    public enum RobberyType {
        None = -1,
        [XmlEnum("0")]
        DarkStreet = 0,
        [XmlEnum("1")]
        Stall = 1,
        [XmlEnum("2")]
        House = 2,
        [XmlEnum("3")]
        Shop = 3,
        [XmlEnum("4")]
        Band = 4,
    }

    public class RobberiesOptions : MonoBehaviour {
        public Sprite[] RobberySprites = new Sprite[5];

        private static RobberiesCollection m_robberiesCollection;

        public static void InitializeRobberiesCollection() {
            m_robberiesCollection = RobberiesSerialization.GetRobberiesCollection();
        }

        public static string GetRobberyName(RobberyType robberyType) {
            return m_robberiesCollection.Robberies[(int) robberyType].Name;
        }

        public static string GetRobberyDescription(RobberyType robberyType) {
            return m_robberiesCollection.Robberies[(int) robberyType].Description;
        }

        public static string GetRobberyFullDescription(RobberyType robberyType) {
            return m_robberiesCollection.Robberies[(int) robberyType].DescriptionFull;
        }

        public static void GetNewRobberies() {
            if (DataScript.EData.RobberiesData != null)
                DataScript.EData.RobberiesData.Clear();

            DataScript.EData.RobberiesData = new Dictionary<RobberyType, Dictionary<int, Robbery>>() {
                {RobberyType.DarkStreet, new Dictionary<int, Robbery>()},
                {RobberyType.Stall, new Dictionary<int, Robbery>()}
            };

            Robbery robbery0 = GetRandomRobbery(RobberyType.DarkStreet, 0);
            DataScript.EData.RobberiesData[RobberyType.DarkStreet].Add(0, robbery0);

//            Robbery robbery1 = GetRandomRobbery(RobberyType.Stall, 0);
//            DataScript.EData.RobberiesData[RobberyType.Stall].Add(0, robbery1);

            Robbery robbery2 = GetRandomRobbery(RobberyType.DarkStreet, 1);
            DataScript.EData.RobberiesData[RobberyType.DarkStreet].Add(1, robbery2);

            Robbery robbery3 = GetRandomRobbery(RobberyType.DarkStreet, 2);
            DataScript.EData.RobberiesData[RobberyType.DarkStreet].Add(2, robbery3);

//            Robbery robbery4 = GetRandomRobbery(RobberyType.Stall, 1);
//            DataScript.EData.RobberiesData[RobberyType.Stall].Add(1, robbery4);
//
//            Robbery robbery5 = GetRandomRobbery(RobberyType.Stall, 2);
//            DataScript.EData.RobberiesData[RobberyType.Stall].Add(2, robbery5);
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

            float chance = (banditsStats + equipmentStats) / (banditsStats + equipmentStats + robberyStats);

            Debug.Log(chance);

            foreach (Trait chanceTrait in chanceTraits) {
                switch (chanceTrait.stat) {
                    case Stat.chance:
                        chance *= chanceTrait.value;
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
                }
            }

            return
                (cStrength * m_robberiesCollection.Robberies[(int) rType].StrenghtInfluence +
                    cLuck * m_robberiesCollection.Robberies[(int) rType].LuckInfluence +
                    cAgility * m_robberiesCollection.Robberies[(int) rType].AgilityInfluence +
                    cSkill * m_robberiesCollection.Robberies[(int) rType].SkillInfluence)
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

    }

}