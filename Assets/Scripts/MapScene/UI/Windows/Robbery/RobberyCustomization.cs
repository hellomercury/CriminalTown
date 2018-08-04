using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


namespace CriminalTown {

    public class RobberyCustomization : MonoBehaviour {

        #region References

        public Image nightEvent;
        public Button nightEventButton;
        public Slider timerSlider;
        public Transform counter;
        public GameObject charCounterIconPrefab;

        #endregion

        private List<GameObject> charCounterIcons = new List<GameObject>();

        private Robbery robbery;
        public RobberyType robberyType;
        public int number;
        public bool isAvailable;


        public void CounterMinus() {
            Destroy(charCounterIcons[0].gameObject);
            charCounterIcons.RemoveAt(0);
        }

        public void CounterPlus() {
            charCounterIcons.Add(Instantiate(charCounterIconPrefab, counter));
        }

        public void OnClick() {
            WM1.robberyWindow.SetRobberyWindow(robberyType, number);
        }

        public void CustomizeRobbery(int num, RobberyType robType) {
            number = num;
            robberyType = robType;
            robbery = DataScript.EData.RobberiesData[robType][number];
            SetCounter();
        }

        private void SetCounter() {
            if (robbery.IsRobberyEmpty() == false) {
                for (int i = 0; i < robbery.Characters.Count; i++)
                    charCounterIcons.Add(Instantiate(charCounterIconPrefab, counter));
            }
        }

        public void AddNightEvent(UnityAction windowSettings, EventStatus eventStatus, float eventTime) {
            nightEventButton.onClick.AddListener(windowSettings);
            switch (eventStatus) {
                case EventStatus.Success:
                    nightEvent.color = Color.green;
                    break;
                case EventStatus.Fail:
                    nightEvent.color = Color.red;
                    break;
                case EventStatus.InProgress:
                    nightEvent.color = Color.yellow;
                    break;
            }

            StartCoroutine(Timer(eventTime));
            nightEvent.gameObject.SetActive(true);
        }

        private IEnumerator Timer(float eventTime) {
            float timer = eventTime;
            timerSlider.maxValue = eventTime;

            while (timer > 0) {
                timer -= Time.deltaTime;
                timerSlider.value = timer;
                yield return null;
            }
        }

        public void ResetNightButton() {
            nightEvent.gameObject.SetActive(false);
            nightEventButton.onClick.RemoveAllListeners();
        }
    }

}