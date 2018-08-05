using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace CriminalTown {

    public class CharacterCustomization : MonoBehaviour, ICharacterCard {
        [SerializeField]
        private Sprite m_hospitalIcon;
        [SerializeField]
        private Sprite m_normalIcon;
        [SerializeField]
        private Sprite m_policeIcon;
        [SerializeField]
        private Sprite m_deathIcon;
        [SerializeField]
        private Sprite m_prisonIcon;

        #region References

        [SerializeField]
        private Button m_characterObject;
        [SerializeField]
        private Text m_levelText;
        [SerializeField]
        private Image m_levelUpImage;

        [SerializeField]
        private Text m_characterName;
        [SerializeField]
        private Image m_portrait;

        [SerializeField]
        private GameObject m_baseStats;

        [SerializeField]
        private Slider m_health;
        [SerializeField]
        private Slider m_tiredness;
        [SerializeField]
        private Text m_strength;
        [SerializeField]
        private Text m_agility;
        [SerializeField]
        private Text m_skill;
        [SerializeField]
        private Text m_luck;
        [SerializeField]
        private Text m_fear;

        [SerializeField]
        private GameObject m_hospitalStats;

        [SerializeField]
        private Slider m_recovery;
        [SerializeField]
        private Text m_recDaysLeft;

        [SerializeField]
        private GameObject m_arrestStats;

        [SerializeField]
        private Slider m_opposition;
        [SerializeField]
        private Text m_arrDaysLeft;

        #endregion

        private Character m_character;

        public Character Character {
            get {
                return m_character;
            }
        }

        //private Color defaultSpColor = Color.yellow;
        //private Color defaultComColor = Color.white;
        //private Color hospitalColor = Color.green;
        //private Color arrestedColor = Color.cyan;
        //private Color robberyColor = Color.gray;

        private Animator m_animator;

        public Animator Animator {
            get {
                if (m_animator == null)
                    m_animator = m_characterObject.GetComponent<Animator>();
                return m_animator;
            }
            set {
                if (m_animator == null)
                    m_animator = m_characterObject.GetComponent<Animator>();
                m_animator = value;
            }
        }

        public void OnClick() {
            UIManager.characterMenu.SetCharacterMenu(Character);
        }

        public void OnDestroy() {
            Character.OnStatsChangedEvent -= OnStatsChangedReaction;
            Character.OnLevelUpEvent -= OnLevelUpReaction;
        }

        public void CustomizeCharacter([NotNull] Character character) {
            m_character = character;

            m_tiredness.maxValue = CharactersOptions.MaxTiredness;
            m_health.maxValue = CharactersOptions.MaxHealth;

            m_portrait.sprite = character.Sprite;
            m_characterName.text = character.Name;

            SetCharStats();
            character.OnStatsChangedEvent += OnStatsChangedReaction;
            character.OnLevelUpEvent += OnLevelUpReaction;
            gameObject.SetActive(true);
        }

        private void SetCharStats() {
            m_baseStats.SetActive(false);
            m_arrestStats.SetActive(false);
            m_hospitalStats.SetActive(false);

            switch (m_character.Status) {
                case CharacterStatus.Normal: {
                    m_characterObject.GetComponent<Image>().sprite = m_normalIcon;
                    Animator.SetTrigger("Normal");
                    m_baseStats.SetActive(true);
                    break;
                }
                case CharacterStatus.Robbery: {
                    m_characterObject.GetComponent<Image>().sprite = m_normalIcon;
                    Animator.SetTrigger("Normal");
                    m_baseStats.SetActive(true);
                    break;
                }
                case CharacterStatus.Hospital: {
                    m_characterObject.GetComponent<Image>().sprite = m_hospitalIcon;
                    Animator.SetTrigger("Hospital");
                    m_hospitalStats.SetActive(true);
                    m_recovery.value = Character.StatusValue;
                    m_recDaysLeft.text = "Осталось дней: " + Character.DaysLeft.ToString();
                    break;
                }
                case CharacterStatus.Arrested: {
                    m_characterObject.GetComponent<Image>().sprite = m_policeIcon;
                    if (Character.DaysLeft < 2)
                        Animator.SetTrigger("PoliceOneDayLeft");
                    else
                        Animator.SetTrigger("Police");
                    m_arrestStats.SetActive(true);
                    m_opposition.value = Character.StatusValue;
                    m_arrDaysLeft.text = "Осталось дней: " + Character.DaysLeft.ToString();
                    break;
                }
            }

            if (Character.Stats.Points > 0)
                m_levelUpImage.gameObject.SetActive(true);
            else
                m_levelUpImage.gameObject.SetActive(false);
            m_levelText.text = Character.Level.ToString();

            m_health.value = Character.Stats.Health;
            m_tiredness.value = Character.Stats.Tiredness;
            m_strength.text = Character.Stats.Strength.ToString();
            m_agility.text = Character.Stats.Agility.ToString();
            m_skill.text = Character.Stats.Skill.ToString();
            m_luck.text = Character.Stats.Luck.ToString();
            m_fear.text = Character.Stats.Fear.ToString();
        }

        public void OnLevelUpReaction() {
            StartCoroutine(LevelUpAnimation());
        }

        public void OnStatsChangedReaction() {
            SetCharStats();
        }

        private IEnumerator LevelUpAnimation() {
            float timer = 1;
            Vector3 localScale = m_portrait.transform.localScale;
            while (timer > 0) {
                timer -= Time.deltaTime;
                m_portrait.transform.localScale += new Vector3(0.1f, 0.1f);
                yield return 0;
            }
            m_portrait.transform.localScale = localScale;
        }
    }

}