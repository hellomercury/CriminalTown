using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

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
        public int NextId = -1;
        public QuestAward Award;
    }

    [Serializable]
    public class Quest : ScriptableObject {
        public int Id;
        public string Name;
        public string Description;
        public Vector2 PositionInEditor;
    }

    [Serializable]
    public class ChoiceQuest : Quest {
        [NotNull]
        public QuestTransition[] Choices = new []{new QuestTransition(), new QuestTransition()};
    }

    [Serializable]
    public class LinearQuest : Quest {
        [NotNull]
        public QuestTransition SuccessTransition = new QuestTransition();
        [NotNull]
        public QuestTransition FailTransition = new QuestTransition();
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

