using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CriminalTown {

    [Serializable]
    public class QuestAward {
        public int Money;
        public Dictionary<int, int> Items;
        public Character[] Characters;
    }

    [Serializable]
    public class Quest {
        public string Name;
        public string Description;
    }

    [Serializable]
    public class ChoiceQuest : Quest {
        public Choice[] Choices;
        public class Choice {
            public int ShortDescription;
            public int NextId;
            public QuestAward Award;
        }
    }

    [Serializable]
    public class LinearQuest : Quest {
        public int SuccessNextId;
        public int FailNextId;
        public QuestAward Award; 
    }
    
    [Serializable]
    public class CharacterQuest : LinearQuest {
        public Character Character;
    }

    [Serializable]
    public class HireCharacterQuest : CharacterQuest {

    }

    [Serializable]
    public class KickCharacterQuest : CharacterQuest {

    }
    
    [Serializable]
    public class StatusCharacterQuest : CharacterQuest {
        public CharacterStatus Status;
    }
    
    [Serializable]
    public class LevelUpCharacterQuest : CharacterQuest {
        
    }

    [Serializable]
    public class StatsUpCharacterQuest : CharacterQuest {
        public Stat Stat;
    }

    [Serializable]
    public class ItemQuest : LinearQuest {
        public int ItemID;
        public int Count;
    }

    [Serializable]
    public class RobberyQuest : LinearQuest {
        public RobberyType RobberyType;
        public int Count;
    }
    
}

