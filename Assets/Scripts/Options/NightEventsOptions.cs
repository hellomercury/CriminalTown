using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CriminalTown {

    public enum SpriteType {
        None = -1,
        Characters,
        Items,
        Robberies,
        People,
        Places
    }

    public enum CharacterSpriteType {
        ComMale,
        ComFemale,
        Special
    }

    public enum EventStatus {
        Success,
        Fail,
        InProgress
    }

    public class NightEventButtonDetails {
        public string ButtonText;
        public NightEventNode NextEventNode;
        public int Effect;
        public int PoliceEffect;
        public int HospitalEffect;
        public int HealthAffect;
        public int PoliceKnowledge;
        public Dictionary<int, int> Awards;
        public int Money;
    }

    public class NightEventNode {
        public string TitleText;
        public string Description;
        public SpriteType SpriteType;
        public CharacterSpriteType CharSpriteType;
        public int SpriteId;
        public List<NightEventButtonDetails> Buttons;
    }

    public class NightEvent {
        public NightEventNode RootNode;
        public NightEventNode Success;
        public NightEventNode Fail;
    }

    public class NightRobberyData {
        private EventStatus m_eventStatus;
        private readonly Robbery m_robbery;

        public NightEvent nightEvent;

        private int m_money;
        private readonly Dictionary<int, int> m_awards;

        private float m_chance;
        private float m_hospitalChance;
        private float m_policeChance;
        private int m_healthAffect;
        private int m_policeKnowledge;

        public EventStatus Status {
            get {
                return m_eventStatus;
            }
        }

        public Robbery Robbery {
            get {
                return m_robbery;
            }
        }

        public int Money {
            get {
                return m_money;
            }
        }

        public Dictionary<int, int> Awards {
            get {
                return m_awards;
            }
        }

        public float Chance {
            get {
                return m_chance;
            }
        }

        //Constructor
        public NightRobberyData(Robbery robbery) {
            this.m_robbery = robbery;
            nightEvent = NightEventsOptions.GetRandomEvent(robbery.RobberyType);
            m_eventStatus = EventStatus.InProgress;
            m_chance = RobberiesOptions.CalculatePreliminaryChance(robbery);
            m_policeChance = Random.Range(0, 51);
            m_hospitalChance = Random.Range(0, 51);
            m_money = RobberiesOptions.GetRobberyMoneyRewardAtTheCurrentMoment(robbery.RobberyType);
            m_awards = RobberiesOptions.GetRobberyAwardsAtTheCurrentMoment(robbery.RobberyType);
            m_policeKnowledge = 1;
        }

        public void ApplyChoice(NightEventButtonDetails buttonDetails) {
            m_chance += buttonDetails.Effect;
            m_hospitalChance += buttonDetails.HospitalEffect;
            m_policeChance += buttonDetails.PoliceEffect;
            m_policeKnowledge += buttonDetails.PoliceKnowledge;
            m_money += buttonDetails.Money;
            m_healthAffect += buttonDetails.HealthAffect;

            if (buttonDetails.Awards != null)
                foreach (int bKey in buttonDetails.Awards.Keys) {
                    if (m_awards.ContainsKey(bKey))
                        m_awards[bKey] += buttonDetails.Awards[bKey];
                    else
                        m_awards.Add(bKey, buttonDetails.Awards[bKey]);
                }
        }

        public void SetAsSuccesfull() {
            m_eventStatus = EventStatus.Success;
        }

        public void SetAsFailed() {
            m_eventStatus = EventStatus.Fail;
        }

        public void SetAsInProgress() {
            m_eventStatus = EventStatus.InProgress;
        }
    }

    public class NightEventsOptions : MonoBehaviour {
        private static NightEventsOptions m_instance;

        public static NightEventsOptions Instance {
            get {
                return m_instance;
            }
        }
        
        [SerializeField]
        private Sprite[] m_placesSprites = new Sprite[5];
        [SerializeField]
        private Sprite[] m_peopleSprites = new Sprite[10];

        private static Dictionary<RobberyType, List<int>> m_usedEvents;
        private static Dictionary<RobberyType, NightEvent[]> m_nightEventsCollection;

        public void Initialize() {
            m_instance = GetComponent<NightEventsOptions>();
            m_nightEventsCollection = NightEventsSerialization.GetNightEventsCollection();
            m_usedEvents = new Dictionary<RobberyType, List<int>>();
        }
        
        public static NightEvent GetRandomEvent(RobberyType robberyType) {
            int eventsCount = m_nightEventsCollection[robberyType].Length;
            int randomNumber = Random.Range(0, eventsCount);
            
            //Avoid repetitions while it is possible
            if (m_usedEvents.ContainsKey(robberyType)) {
                if (m_usedEvents[robberyType].Count < eventsCount) {
                    while (m_usedEvents[robberyType].Contains(eventsCount)) {
                        randomNumber++;
                        if (randomNumber >= eventsCount) {
                            randomNumber = 0;
                        }
                    }
                }
                m_usedEvents[robberyType].Add(randomNumber);
            } else {
                m_usedEvents.Add(robberyType, new List<int> {randomNumber});
            }
            
            return m_nightEventsCollection[robberyType][randomNumber];
        }
        
        public static Sprite GetNightEventSprite(SpriteType spriteType, int spriteId, CharacterSpriteType charSpriteType = 0) {
            switch (spriteType) {
                case SpriteType.Characters:
                    switch (charSpriteType) {
                        case CharacterSpriteType.ComMale:
                            return CharactersOptions.Instance.GetCommonSprite(Sex.Male, spriteId);
                        case CharacterSpriteType.ComFemale:
                            return CharactersOptions.Instance.GetCommonSprite(Sex.Female, spriteId);
                        case CharacterSpriteType.Special:
                            return CharactersOptions.Instance.GetSpecialSprite(spriteId);
                        default:
                            return null;
                    }
                case SpriteType.Items:
                    return ItemsOptions.Instance.itemsSprites[spriteId];
                case SpriteType.Robberies:
                    return RobberiesOptions.Instance.RobberySprites[spriteId];
                case SpriteType.People:
                    return m_instance.m_peopleSprites[spriteId];
                case SpriteType.Places:
                    return m_instance.m_placesSprites[spriteId];
                default:
                    return null;
            }
        }

        public int GetCharacterSpriteId(RobberyType robberyType, int locationNum) {
            return 0;
        }

        public static void ClearUsedEvents() {
            m_usedEvents.Clear();
        }
    }

}