using System.Collections.Generic;

namespace CriminalTown {

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

}