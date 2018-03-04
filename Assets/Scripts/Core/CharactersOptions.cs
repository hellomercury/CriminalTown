using System.Collections.Generic;
using UnityEngine;


public enum CharacterStatus { normal, robbery, hospital, arrested }
public enum CharacterProperty { name, history, spriteId, traitId }
public enum Sex { male, female }

[System.Serializable]
public class Character
{
    //abstract properties
    public virtual string Name { get { return null; } set { } }
    public virtual string History { get { return null; } set { } }
    public virtual Sprite Sprite { get { return null; } set { } }

    //base variables
    private int statusValue;
    private RobberyType robType;
    private int boostCoef;
    private int locNum;

    public Sex Sex { get; set; }
    public int Level { get; set; }
    public int Tiredness { get; set; }
    public int Health { get; set; }
    public int Exp { get; set; }
    public int Strength { get; set; }
    public int Agility { get; set; }
    public int Skill { get; set; }
    public int Fear { get; set; }
    public int Luck { get; set; }
    public int Points { get; set; }

    public CharacterStatus Status { set; get; }

    public RobberyType RobberyType
    {
        get { return robType; }
        set { robType = value; }
    }
    public int LocationNum
    {
        get { return locNum; }
        set { locNum = value; }
    }
    public int StatusValue
    {
        get { return statusValue; }
        set { statusValue = value; }
    }
    public int BoostCoefficient
    {
        get { return boostCoef; }
        set { boostCoef = value; }
    }
    public int DaysLeft
    {
        get
        {
            if (Status == CharacterStatus.hospital) return Mathf.CeilToInt((CharactersOptions.maxRecovery - StatusValue) / (float)(BoostCoefficient * CharactersOptions.recoveryStep));
            else if (Status == CharacterStatus.arrested) return Mathf.CeilToInt(StatusValue / (float)Fear);
            else return 0;
        }
    }

    public void SetStats(Character character)
    {
        Level = character.Level;
        Health = character.Health;
        Tiredness = character.Tiredness;
        Exp = character.Exp;
        Strength = character.Strength;
        Agility = character.Agility;
        Skill = character.Skill;
        Luck = character.Luck;
        Fear = character.Fear;
        Points = character.Points;

        Status = character.Status;
        statusValue = character.statusValue;
        robType = character.robType;
        locNum = character.locNum;
    }

    public Character GetStats()
    {
        return new Character()
        {
            Level = this.Level,
            Health = this.Health,
            Tiredness = this.Tiredness,
            Exp = this.Exp,
            Strength = this.Strength,
            Agility = this.Agility,
            Skill = this.Skill,
            Luck = this.Luck,
            Fear = this.Fear,
            Points = this.Points,

            Status = Status,
            statusValue = statusValue,
            robType = robType,
            locNum = locNum
        };
    }

    public void AddExperience(int expToAdd)
    {
        if (Exp + expToAdd > CharactersOptions.GetExperienceMaxValue(Level))
        {
            OnLevelUpEvent();
            while (Exp + expToAdd > CharactersOptions.GetExperienceMaxValue(Level))
            {
                Health = CharactersOptions.maxHealth;
                Tiredness = 0;
                expToAdd -= (CharactersOptions.GetExperienceMaxValue(Level) - Exp);
                Exp = 0;
                Level++;
                Points++;
            }
        }
        OnStatsChangedEvent();
        Exp += expToAdd;
    }

    public void AddToHospital()
    {

    }

    public void AddToPolice()
    {

    }

    public void SetDefaultStatus()
    {

    }

    public void CallOnStatsChangedEvent()
    {
        OnStatsChangedEvent();
    }

    public delegate void CharacterEvent();
    public event CharacterEvent OnStatsChangedEvent = delegate { };
    public event CharacterEvent OnLevelUpEvent = delegate { };
}

[System.Serializable]
public class SpecialCharacter : Character
{
    public int Authority { set; get; }
    public int Id { set; get; }
    public int SpriteId { set; get; }
    private List<int> traitIds;

    public override string Name
    {
        get { return CharactersOptions.GetSpecialName(Authority, Id); }
    }
    public override string History
    {
        get { return CharactersOptions.GetSpecialHistory(Authority, Id); }
    }
    public override Sprite Sprite
    {
        get { return CharactersOptions.GetSpecialSprite(SpriteId); }
    }
    public List<Trait> Traits
    {
        get
        {
            List<Trait> traits = new List<Trait>();
            for (int i = 0; i < traitIds.Count; i++)
            {
                traits.Add(TraitsOptions.GetTrait(traitIds[i]));
            }
            return traits;
        }
    }

    public SpecialCharacter(Character stats, int authority, int id, int spriteId, List<int> traitIds)
    {
        SetStats(stats);
        Authority = authority;
        Id = id;
        SpriteId = spriteId;
        this.traitIds = traitIds;
    }
}

[System.Serializable]
public class CommonCharacter : Character
{
    public int SpriteId { set; get; }
    public int NameId { set; get; }
    public int HistoryId { set; get; }

    public override string Name
    {
        get { return CharactersOptions.GetCommonName((Sex)Sex, NameId); }
    }
    public override string History
    {
        get { return CharactersOptions.GetCommonHistory((Sex)Sex, HistoryId); }
    }
    public override Sprite Sprite
    {
        get { return CharactersOptions.GetCommonSprite((Sex)Sex, SpriteId); }
    }
}

public partial class CharactersOptions : MonoBehaviour
{
    #region CharactersSettings - constants
    public const int campComCellsAmount = 10;
    public const int campSpCellsAmount = 10;
    public const int panelCellsMaxAmount = 20;

    public const int maxStat = 10;
    public const int maxAuthority = 10;
    public const int maxTiredness = 100;
    public const int maxHealth = 100;
    public const int maxFear = 100;

    //Hospital
    public const int maxRecovery = 100;
    public const int recoveryStep = 10;
    public const int boostedCoef = 2;

    //PoliceStation
    public const int maxOpposition = 100;

    //Random
    public const int maxRndStat = 8;
    public const int levelsRndDispersion = 3;

    //Leveling
    public const int expStepByLevel = 20;
    public const float pointsPerLevelCoefficient = 1.2f;

    //Sprites
    public const int comMaleSpritesAmount = 10;
    public const int comFemaleSpritesAmount = 10;
    public const int specialSpritesAmount = 10;
    #endregion

    public Sprite[] specialSprites = new Sprite[specialSpritesAmount];
    public Sprite[] comMaleSprites = new Sprite[comMaleSpritesAmount];
    public Sprite[] comFemaleSprites = new Sprite[comFemaleSpritesAmount];


    #region Accessing methods
    public static string GetCommonName(Sex sex, int id)
    {
        switch (sex)
        {
            case Sex.male:
                return commonMaleNames[id];
            case Sex.female:
                return commonFemaleNames[id];
            default:
                return null;
        }
    }
    public static string GetCommonHistory(Sex sex, int id)
    {
        switch (sex)
        {
            case Sex.male:
                return commonMaleHistories[id];
            case Sex.female:
                return commonFemaleHistories[id];
            default:
                return null;
        }
    }
    public static Sprite GetCommonSprite(Sex sex, int id)
    {
        switch (sex)
        {
            case Sex.male:
                return WM1.charactersOptions.comMaleSprites[id];
            case Sex.female:
                return WM1.charactersOptions.comFemaleSprites[id];
            default:
                return null;
        }
    }

    public static string GetSpecialName(int authority, int id)
    {
        return specialCharactersAuthList[authority][id][CharacterProperty.name];
    }
    public static string GetSpecialHistory(int authority, int id)
    {
        return specialCharactersAuthList[authority][id][CharacterProperty.history];
    }
    public static int GetSpecialSpriteId(int authority, int id)
    {
        return int.Parse(specialCharactersAuthList[authority][id][CharacterProperty.spriteId]);
    }
    public static Sprite GetSpecialSprite(int spriteId)
    {
        return WM1.charactersOptions.specialSprites[spriteId];
    }
    public static int GetSpecialTraitId(int authority, int id)
    {
        return int.Parse(specialCharactersAuthList[authority][id][CharacterProperty.traitId]);
    }
    #endregion

    public static int[] GetRandomStats(int level)
    {
        int summ = (int)(pointsPerLevelCoefficient * level);
        int minStat = (int)(1f * summ / 6f);
        int maxStat = (int)(4f * summ / 9f);
        int[] stats = new int[3];

        int[] seq = new int[3] { 0, 1, 2 };
        for (int i = 2; i > 0; i--)
        {
            int temp;
            int j = Random.Range(0, i);
            temp = seq[i];
            seq[i] = seq[j];
            seq[j] = temp;
        }

        stats[seq[0]] = Random.Range(minStat, maxStat + 1);

        if (summ - stats[seq[0]] > maxStat) stats[seq[1]] = Random.Range(minStat, maxStat + 1);
        else if (summ - stats[seq[0]] < 0) stats[seq[1]] = 0;
        else stats[seq[1]] = Random.Range(minStat, summ - stats[seq[0]] + 1);

        if (summ < stats[seq[0]] + stats[seq[1]]) stats[seq[2]] = 0;
        else if (summ - stats[seq[0]] - stats[seq[1]] > maxStat) stats[seq[2]] = maxStat;
        else stats[seq[2]] = summ - stats[seq[0]] - stats[seq[1]];

        return stats;
    }

    public static int GetRandomCharLevelAtCurrentMoment()
    {
        int summ = 0;
        foreach (Character character in DataScript.chData.PanelCharacters) summ += character.Level;
        int average = summ == 0 ? 0 : summ / DataScript.chData.PanelCharacters.Count;
        return average < levelsRndDispersion ? Random.Range(0, average + levelsRndDispersion) :
            Random.Range(average - levelsRndDispersion, average + levelsRndDispersion);
    }

    public static int GetExperienceMaxValue(int level)
    {
        return 100 + level * expStepByLevel;
    }
    //Доделать 
    public static CommonCharacter GetRandomCommonCharacter(int level)
    {
        CommonCharacter randomCharacter = new CommonCharacter();
        int sex = Random.Range(0, 2);
        if (sex == 0)
        {
            randomCharacter.HistoryId = Random.Range(0, commonMaleHistories.Count);
            randomCharacter.NameId = Random.Range(0, commonMaleNames.Count);
            randomCharacter.SpriteId = Random.Range(0, comMaleSpritesAmount);
        }
        if (sex == 1)
        {
            randomCharacter.HistoryId = Random.Range(0, commonFemaleHistories.Count);
            randomCharacter.NameId = Random.Range(0, commonFemaleNames.Count);
            randomCharacter.SpriteId = Random.Range(0, comFemaleSpritesAmount);
        }

        randomCharacter.Level = level;

        int[] rndStats = GetRandomStats(randomCharacter.Level);

        randomCharacter.Status = CharacterStatus.normal;
        randomCharacter.StatusValue = 0;
        randomCharacter.LocationNum = 1;

        randomCharacter.Sex = (Sex)sex;
        randomCharacter.Strength = rndStats[0];
        randomCharacter.Agility = rndStats[1];
        randomCharacter.Skill = rndStats[2];
        randomCharacter.Luck = Random.Range(0, 5);
        randomCharacter.Fear = Random.Range(25, 51);

        randomCharacter.Tiredness = Random.Range(0, maxTiredness / 2);
        randomCharacter.Exp = 0;
        randomCharacter.Points = 0;
        randomCharacter.Health = Random.Range(maxHealth * 1 / 2, maxHealth + 1);

        return randomCharacter;
    }

    public static SpecialCharacter GetSpecialCharacter(int authorityLevel, int charNum)
    {
        //Special character must be stronger than common
        int rndLevel = GetRandomCharLevelAtCurrentMoment() + levelsRndDispersion;
        int[] rndStats = GetRandomStats(rndLevel);
        //int rndId = Random.Range(0, specialCharactersAuthList[authorityLevel].Count);

        return new SpecialCharacter(
            stats:
            new Character()
            {
                Status = CharacterStatus.normal,
                StatusValue = 0,
                LocationNum = 1,
                BoostCoefficient = 1,
                RobberyType = 0,

                Level = rndLevel,

                Strength = rndStats[0],
                Agility = rndStats[1],
                Skill = rndStats[2],
                Luck = Random.Range(5, 7),
                Fear = Random.Range(0, 26),
                Health = Random.Range(maxHealth * 3 / 4, maxHealth + 1),
                Tiredness = Random.Range(0, maxTiredness / 4),
                Exp = 0,
                Points = 0,
            },
            authority: authorityLevel,
            id: charNum,
            spriteId: GetSpecialSpriteId(authorityLevel, charNum),
            traitIds: new List<int> { GetSpecialTraitId(authorityLevel, charNum) });
    }

    public static int GetComPrice(int level)
    {
        return 100 * level;
    }

    public static int GetSpPrice(int level)
    {
        return 1000 * level;
    }

    public static int GetBoostRecoveryPrice(int level)
    {
        return 20 * level;
    }

    public static int GetBreakOutPrice(int level)
    {
        return 50 * level;
    }

    public static void FillCampCells()
    {
        while (DataScript.chData.CampCharacters.Count < campComCellsAmount)
        {
            CommonCharacter randomComCharacter = GetRandomCommonCharacter(GetRandomCharLevelAtCurrentMoment());
            DataScript.chData.CampCharacters.Add(randomComCharacter);
        }
        while (DataScript.chData.CampCharacters.Count < campSpCellsAmount)
        {
            SpecialCharacter randomSpCharacter = GetSpecialCharacter(DataScript.pData.authority, 0);
            DataScript.chData.CampCharacters.Add(randomSpCharacter);
        }
        //DataScript.SaveCharactersData();
    }
}
