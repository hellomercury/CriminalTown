using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace CriminalTown {

    public class RobberyCustomization : MonoBehaviour {

        [SerializeField]
        private Image m_nightEvent;
        [SerializeField]
        private Button m_nightEventButton;
        [SerializeField]
        private Slider m_timerSlider;
        [SerializeField]
        private Transform m_counter;
        [SerializeField]
        private GameObject[] m_charCounterIcons;

        private Robbery m_robbery;
        private RobberyType m_robberyType;
        private int m_number;
        private bool m_isAvailable;

        public Vector2 LocalPosition {
            get {
                return transform.localPosition;
            }
        }
        
        public RobberyType RobberyType {
            get {
                return m_robberyType;
            }
        }

        public int Number {
            get {
                return m_number;
            }
        }

        public bool IsAvailable {
            get {
                return m_isAvailable;
            }
        }

        public void ActivateRobbery(bool value) {
            m_isAvailable = value;
        }

        [UsedImplicitly]
        public void OnClick() {
            UIManager.robberyWindow.SetRobberyWindow(m_robberyType, m_number);
        }

        public void CustomizeRobbery(int num, RobberyType robType) {
            m_number = num;
            m_robberyType = robType;
            m_robbery = DataScript.EData.RobberiesData[robType][m_number];
            SetCounter();
        }

        private void SetCounter() {
            int charsCount = m_robbery.Characters.Count;
            for (int i = 0; i < charsCount && i < m_charCounterIcons.Length; i++) {
                m_charCounterIcons[i].SetActive(i < charsCount);
            }
        }

        public void AddNightEvent(UnityAction windowSettings, EventStatus eventStatus, float eventTime) {
            m_nightEventButton.onClick.AddListener(windowSettings);
            switch (eventStatus) {
                case EventStatus.Success:
                    m_nightEvent.color = Color.green;
                    break;
                case EventStatus.Fail:
                    m_nightEvent.color = Color.red;
                    break;
                case EventStatus.InProgress:
                    m_nightEvent.color = Color.yellow;
                    break;
            }

            StartCoroutine(Timer(eventTime));
            m_nightEvent.gameObject.SetActive(true);
        }

        private IEnumerator Timer(float eventTime) {
            float timer = eventTime;
            m_timerSlider.maxValue = eventTime;

            while (timer > 0) {
                timer -= Time.deltaTime;
                m_timerSlider.value = timer;
                yield return null;
            }
        }

        public void ResetNightEvent() {
            m_nightEvent.gameObject.SetActive(false);
            m_nightEventButton.onClick.RemoveAllListeners();
        }
    }

}