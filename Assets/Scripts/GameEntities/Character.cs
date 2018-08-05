using UnityEngine;

namespace CriminalTown {

    [System.Serializable]
    public struct CharacterStats {
        public static bool operator ==(CharacterStats left, CharacterStats right) {
            return left.Equals(right);
        }

        public static bool operator !=(CharacterStats left, CharacterStats right) {
            return !left.Equals(right);
        }

        public int Tiredness;
        public int Health;
        public int Expirience;
        public int Strength;
        public int Agility;
        public int Skill;
        public int Fear;
        public int Luck;
        public int Points;

        public bool Equals(CharacterStats other) {
            return Tiredness == other.Tiredness && Health == other.Health && Expirience == other.Expirience && Strength == other.Strength && Agility == other.Agility && Skill == other.Skill && Fear == other.Fear && Luck == other.Luck && Points == other.Points;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj))
                return false;
            return obj is CharacterStats && Equals((CharacterStats) obj);
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = Tiredness;
                hashCode = (hashCode * 397) ^ Health;
                hashCode = (hashCode * 397) ^ Expirience;
                hashCode = (hashCode * 397) ^ Strength;
                hashCode = (hashCode * 397) ^ Agility;
                hashCode = (hashCode * 397) ^ Skill;
                hashCode = (hashCode * 397) ^ Fear;
                hashCode = (hashCode * 397) ^ Luck;
                hashCode = (hashCode * 397) ^ Points;
                return hashCode;
            }
        }
    }

    [System.Serializable]
    public class Character {
        protected readonly int SpriteId;
        protected readonly int NameId;
        protected readonly int HistoryId;
        private readonly Sex m_sex;
        private CharacterStats m_characterStats;
        private int m_level;
        private CharacterStatus m_status;
        private RobberyType m_robType;
        private int m_statusValue;
        private int m_boostCoef;
        private int m_locNum;

        public virtual string Name {
            get {
                return CharactersOptions.GetCommonName(Sex, NameId);
            }
        }

        public virtual string History {
            get {
                return CharactersOptions.GetCommonHistory(Sex, HistoryId);
            }
        }

        public virtual Sprite Sprite {
            get {
                return CharactersOptions.Instance.GetCommonSprite(Sex, SpriteId);
            }
        }

        public CharacterStats Stats {
            get {
                return m_characterStats;
            }
            set {
                m_characterStats = value;
                OnStatsChangedEvent();
            }
        }

        public Sex Sex {
            get {
                return m_sex;
            }
        }

        public int Level {
            get {
                return m_level;
            }
        }

        public CharacterStatus Status {
            get {
                return m_status;
            }
        }

        public RobberyType RobberyType {
            get {
                return m_robType;
            }
        }

        public int LocationNum {
            get {
                return m_locNum;
            }
        }

        public int StatusValue {
            get {
                return m_statusValue;
            }
        }

        public int BoostCoefficient {
            get {
                return m_boostCoef;
            }
        }

        public int DaysLeft {
            get {
                if (Status == CharacterStatus.Hospital)
                    return Mathf.CeilToInt((CharactersOptions.MaxRecovery - StatusValue) / (float) (BoostCoefficient * CharactersOptions.RecoveryStep));
                else if (Status == CharacterStatus.Arrested)
                    return Mathf.CeilToInt(StatusValue / (float) Stats.Fear);
                else
                    return 0;
            }
        }

        public Character(CharacterStats characterStats, Sex sex, int level, int spriteId, int nameId, int historyId) {
            m_characterStats = characterStats;
            m_sex = sex;
            m_level = level;
            m_status = CharacterStatus.Normal;
            m_robType = RobberyType.None;
            m_statusValue = 0;
            m_boostCoef = -1;
            m_locNum = -1;
            SpriteId = spriteId;
            NameId = nameId;
            HistoryId = historyId;
        }

        public void AddExperience(int expToAdd) {
            if (m_characterStats.Expirience + expToAdd > CharactersOptions.GetExperienceMaxValue(m_level)) {
                while (m_characterStats.Expirience + expToAdd > CharactersOptions.GetExperienceMaxValue(m_level)) {
                    m_characterStats.Health = CharactersOptions.MaxHealth;
                    m_characterStats.Tiredness = 0;
                    expToAdd -= (CharactersOptions.GetExperienceMaxValue(m_level) - m_characterStats.Expirience);
                    m_characterStats.Expirience = 0;
                    m_characterStats.Points++;
                    m_level++;
                }
                OnLevelUpEvent();
            }
            OnStatsChangedEvent();
            m_characterStats.Expirience += expToAdd;
        }

        public void AddToHospital() {
            m_status = CharacterStatus.Hospital;
            m_statusValue = m_characterStats.Health;
            m_robType = RobberyType.None;
            m_boostCoef = 1;
            m_locNum = -1;
            OnStatsChangedEvent();
        }

        public void AddToPolice() {
            m_status = CharacterStatus.Arrested;
            m_statusValue = CharactersOptions.MaxOpposition - m_characterStats.Fear;
            m_robType = RobberyType.None;
            m_boostCoef = 0;
            m_locNum = -1;
            OnStatsChangedEvent();
        }

        public void AddToRobbery(RobberyType robberyType, int locationNum) {
            m_status = CharacterStatus.Robbery;
            m_statusValue = 0;
            m_robType = robberyType;
            m_boostCoef = -1;
            m_locNum = locationNum;
            OnStatsChangedEvent();
        }

        public void BoostRecovery() {
            m_boostCoef = CharactersOptions.BoostedCoef;
            OnStatsChangedEvent();
        }

        public void SetDefaultStatus() {
            m_status = CharacterStatus.Normal;
            m_statusValue = 0;
            m_robType = RobberyType.None;
            m_boostCoef = -1;
            m_locNum = -1;
            OnStatsChangedEvent();
        }

        public delegate void CharacterEvent();

        public event CharacterEvent OnStatsChangedEvent = delegate { };

        public event CharacterEvent OnLevelUpEvent = delegate { };
    }

}