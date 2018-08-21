using System;
using System.Collections.Generic;

namespace CriminalTown {

    [Serializable]
    public class QuestAward {
        public int Money;
        public Dictionary<int, int> Items;
        public Character[] Characters;
    }

    [Serializable]
    public class Quest {
        public int Id;
        public string Name;
        public string Description;
    }

    [Serializable]
    public class ChoiceQuest : Quest {
        public Choice[] Choices;

        [Serializable]
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
        public int StatId;
    }

    [Serializable]
    public class ItemQuest : LinearQuest {
        public int ItemId;
        public int Count;
    }

    [Serializable]
    public class RobberyQuest : LinearQuest {
        public RobberyType RobberyType;
        public int Count;
    }

    [Serializable]
    public class EducationQuest : LinearQuest {
        
    }

}

