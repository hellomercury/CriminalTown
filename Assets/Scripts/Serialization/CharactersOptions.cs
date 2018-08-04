using System.Collections.Generic;
using UnityEngine;

namespace CriminalTown {




    public enum CharacterStatus {
        Normal,
        Robbery,
        Hospital,
        Arrested
    }

    public enum CharacterProperty {
        Name,
        History,
        SpriteId,
        TraitId
    }

    public enum Sex {
        Male,
        Female
    }

    public partial class CharactersOptions {

        #region CharactersSettings - constants

        public const int CampComCellsAmount = 10;
        public const int CampSpCellsAmount = 10;
        public const int PanelCellsMaxAmount = 20;

        public const int MaxStat = 10;
        public const int MaxAuthority = 10;
        public const int MaxTiredness = 100;
        public const int MaxHealth = 100;
        public const int MaxFear = 100;

        //Hospital
        public const int MaxRecovery = 100;
        public const int RecoveryStep = 10;
        public const int BoostedCoef = 2;

        //PoliceStation
        public const int MaxOpposition = 100;

        //Random
        public const int MaxRndStat = 8;
        public const int LevelsRndDispersion = 3;

        //Leveling
        public const int ExpStepByLevel = 20;
        public const float PointsPerLevelCoefficient = 1.2f;

        //Sprites
        public const int ComMaleSpritesAmount = 10;
        public const int ComFemaleSpritesAmount = 10;
        public const int SpecialSpritesAmount = 10;

        #endregion

        public Sprite[] SpecialSprites = new Sprite[SpecialSpritesAmount];
        public Sprite[] ComMaleSprites = new Sprite[ComMaleSpritesAmount];
        public Sprite[] ComFemaleSprites = new Sprite[ComFemaleSpritesAmount];

        #region Accessing methods

        public static string GetCommonName(Sex sex, int id) {
            switch (sex) {
                case Sex.Male:
                    return commonMaleNames[id];
                case Sex.Female:
                    return commonFemaleNames[id];
                default:
                    return null;
            }
        }

        public static string GetCommonHistory(Sex sex, int id) {
            switch (sex) {
                case Sex.Male:
                    return commonMaleHistories[id];
                case Sex.Female:
                    return commonFemaleHistories[id];
                default:
                    return null;
            }
        }

        public static Sprite GetCommonSprite(Sex sex, int id) {
            switch (sex) {
                case Sex.Male:
                    return WM1.charactersOptions.ComMaleSprites[id];
                case Sex.Female:
                    return WM1.charactersOptions.ComFemaleSprites[id];
                default:
                    return null;
            }
        }

        public static string GetSpecialName(int authority, int id) {
            return specialCharactersAuthList[authority][id][CharacterProperty.Name];
        }

        public static string GetSpecialHistory(int authority, int id) {
            return specialCharactersAuthList[authority][id][CharacterProperty.History];
        }

        public static int GetSpecialSpriteId(int authority, int id) {
            return int.Parse(specialCharactersAuthList[authority][id][CharacterProperty.SpriteId]);
        }

        public static Sprite GetSpecialSprite(int spriteId) {
            return WM1.charactersOptions.SpecialSprites[spriteId];
        }

        public static int GetSpecialTraitId(int authority, int id) {
            return int.Parse(specialCharactersAuthList[authority][id][CharacterProperty.TraitId]);
        }

        #endregion

        public static int[] GetRandomStats(int level) {
            int summ = (int) (PointsPerLevelCoefficient * level);
            int minStat = (int) (1f * summ / 6f);
            int maxStat = (int) (4f * summ / 9f);
            int[] stats = new int[3];

            int[] seq = new int[3] {0, 1, 2};
            for (int i = 2; i > 0; i--) {
                int j = Random.Range(0, i);
                int temp = seq[i];
                seq[i] = seq[j];
                seq[j] = temp;
            }

            stats[seq[0]] = Random.Range(minStat, maxStat + 1);

            if (summ - stats[seq[0]] > maxStat)
                stats[seq[1]] = Random.Range(minStat, maxStat + 1);
            else if (summ - stats[seq[0]] < 0)
                stats[seq[1]] = 0;
            else
                stats[seq[1]] = Random.Range(minStat, summ - stats[seq[0]] + 1);

            if (summ < stats[seq[0]] + stats[seq[1]])
                stats[seq[2]] = 0;
            else if (summ - stats[seq[0]] - stats[seq[1]] > maxStat)
                stats[seq[2]] = maxStat;
            else
                stats[seq[2]] = summ - stats[seq[0]] - stats[seq[1]];

            return stats;
        }

        public static int GetRandomCharLevelAtCurrentMoment() {
            int summ = 0;
            foreach (Character character in DataScript.ChData.PanelCharacters)
                summ += character.Level;
            int average = summ == 0 ? 0 : summ / DataScript.ChData.PanelCharacters.Count;
            return average < LevelsRndDispersion ? Random.Range(0, average + LevelsRndDispersion) : Random.Range(average - LevelsRndDispersion, average + LevelsRndDispersion);
        }

        public static int GetExperienceMaxValue(int level) {
            return 100 + level * ExpStepByLevel;
        }

        //Доделать 
        public static Character GetRandomCharacter(int level) {
            Sex sex = (Sex) Random.Range(0, 2);
            int historyId;
            int nameId;
            int spriteId;
            if (sex == Sex.Male) {
                historyId = Random.Range(0, commonMaleHistories.Count);
                nameId = Random.Range(0, commonMaleNames.Count);
                spriteId = Random.Range(0, ComMaleSpritesAmount);
            } else {
                historyId = Random.Range(0, commonFemaleHistories.Count);
                nameId = Random.Range(0, commonFemaleNames.Count);
                spriteId = Random.Range(0, ComFemaleSpritesAmount);
            }

            int[] rndStats = GetRandomStats(level);
            CharacterStats characterStats = new CharacterStats {
                Strength = rndStats[0],
                Agility = rndStats[1],
                Skill = rndStats[2],
                Luck = Random.Range(0, 5),
                Fear = Random.Range(25, 51),
                Tiredness = Random.Range(0, MaxTiredness / 2),
                Health = Random.Range(MaxHealth * 1 / 2, MaxHealth + 1)
            };

            return new Character(characterStats, sex, level, spriteId, nameId, historyId);
        }

        public static SpecialCharacter GetSpecialCharacter(int authorityLevel, int charNum) {
            //Special character must be stronger than common
            int level = GetRandomCharLevelAtCurrentMoment() + LevelsRndDispersion;
            int[] rndStats = GetRandomStats(level);
            //high: need sex value!
            Sex sex = (Sex) Random.Range(0, 2);
            //high: need names, history and sprites database!!!
            //high: and traits

            int historyId = 0;
            int nameId = 0;
            int spriteId = GetSpecialSpriteId(authorityLevel, charNum);

            CharacterStats characterStats = new CharacterStats {
                Strength = rndStats[0],
                Agility = rndStats[1],
                Skill = rndStats[2],
                Luck = Random.Range(5, 7),
                Fear = Random.Range(25, 51),
                Tiredness = Random.Range(0, MaxTiredness / 4),
                Health = Random.Range(MaxHealth * 3 / 4, MaxHealth + 1)
            };

            return new SpecialCharacter(characterStats, sex, level, spriteId,
                nameId, historyId, new List<int> {GetSpecialTraitId(authorityLevel, charNum)});
        }

        public static int GetComPrice(int level) {
            return 100 * level;
        }

        public static int GetSpPrice(int level) {
            return 1000 * level;
        }

        public static int GetBoostRecoveryPrice(int level) {
            return 20 * level;
        }

        public static int GetBreakOutPrice(int level) {
            return 50 * level;
        }

    }

}