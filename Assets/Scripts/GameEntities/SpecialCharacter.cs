using System.Collections.Generic;
using System.Collections.ObjectModel;
using JetBrains.Annotations;
using UnityEngine;

namespace CriminalTown {

    [System.Serializable]
    public class SpecialCharacter : Character {
        //todo: Implement nice way to store special character data

        private readonly List<int> m_traitIds;


        public override string Name {
            get {
                return CharactersOptions.GetSpecialName(0, NameId);
            }
        }

        public override string History {
            get {
                return CharactersOptions.GetSpecialHistory(0, HistoryId);
            }
        }

        public override Sprite Sprite {
            get {
                return CharactersOptions.Instance.GetSpecialSprite(SpriteId);
            }
        }

        public ReadOnlyCollection<Trait> Traits {
            get {
                List<Trait> traits = new List<Trait>();
                for (int i = 0; i < m_traitIds.Count; i++) {
                    traits.Add(TraitsOptions.GetTrait(m_traitIds[i]));
                }
                return traits.AsReadOnly();
            }
        }

        public SpecialCharacter(CharacterStats characterStats, Sex sex, int level, int spriteId, int nameId, int historyId, [CanBeNull] List<int> traitIds) : base(characterStats, sex, level, spriteId, nameId, historyId) {
            m_traitIds = traitIds ?? new List<int>();
        }

    }

}