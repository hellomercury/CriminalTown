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
    public class QuestTransition {
        public int ShortDescription;
        public int NextId;
        public QuestAward Award;
    }

    [Serializable]
    public class Quest {
        public int Id;
        public string Name;
        public string Description;
    }

    [Serializable]
    public class ChoiceQuest : Quest {
        public QuestTransition[] Choices;
    }

    [Serializable]
    public class LinearQuest : Quest {
        public QuestTransition SuccessTransition;
        public QuestTransition FailTransition;
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

