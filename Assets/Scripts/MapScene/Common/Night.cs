using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace CriminalTown {

    public class NightEventArgs : EventArgs {
        private NightRobberyData m_robberyData;

        public NightRobberyData RobberyData {
            get {
                return m_robberyData;
            }
        }

        public NightEventArgs(NightRobberyData robberyData) {
            m_robberyData = robberyData;
        }
    }

    public class Night : MonoBehaviour {
        private static Night m_instance;

        private bool m_isNight;
        private List<NightRobberyData> m_robberies;
        private int m_currentEventNum;
        //todo Разобраться со временем каждого эвента
        private float eventTime = 4;

        public static Night Instance {
            get {
                if (m_instance == null) {
                    m_instance = FindObjectOfType<Night>();
                }
                return m_instance;
            }
        }

        public bool IsNight {
            get {
                return m_isNight;
            }
        }

        private void Awake() {
            m_instance = this;
        }

        public delegate void NightScriptEvent();

        public NightScriptEvent OnNightBegan;
        public NightScriptEvent OnNightEnded;

        public void TryToStartNight() {
            foreach (Character character in DataScript.ChData.PanelCharacters) {
                if (character.Status == CharacterStatus.Arrested)
                    if (character.DaysLeft < 2) {
                        EventButtonDetails yesButton = new EventButtonDetails {
                            buttonText = "Да",
                            action = StartNight
                        };
                        EventButtonDetails noButton = new EventButtonDetails {
                            buttonText = "Нет",
                            action = UIManager.modalPanel.ClosePanel
                        };
                        ModalPanelDetails details = new ModalPanelDetails {
                            button0Details = yesButton,
                            button1Details = noButton,
                            imageSprite = character.Sprite,
                            text = "Этот персонаж скоро нас сдаст. Босс, ты уверен, что стоит оставить его в грязных руках копов?",
                            titletext = character.Name
                        };
                        UIManager.modalPanel.CallModalPanel(details);
                        return;
                    }
            }
            StartNight();
        }

        private void StartNight() {
            m_isNight = true;
            OnNightBegan();
            Debug.Log("Night began");
            UpdateDataAfterDay();
            PrepareEvents();
            StartCoroutine(NightEvents());
        }

        private IEnumerator NightEvents() {
            while (GetEventNum(out m_currentEventNum)) {
                Debug.Log("Call robbery event: " + m_currentEventNum);
                NightRobberyData rob = m_robberies[m_currentEventNum];
                
                //todo: implement event for that
                RobberyType rt = rob.Robbery.RobberyType;
                int ln = rob.Robbery.LocationNum;
                MapController.Instance.MoveToPosition(RobberiesManager.Instance.Robberies[rt][ln].LocalPosition);

                if (rob.nightEvent.RootNode != null) {
                    AssignNightEventDataToWindow(rob, eventTime);
                    yield return new WaitForSeconds(eventTime);
                    if (NightEventWindow.Choice == -1) {
                        MakeChoice(Random.Range(0, rob.nightEvent.RootNode.Buttons.Count));
                    }
                    ApplyChangesAfterChoice(m_currentEventNum);

                    if (rob.nightEvent.RootNode.Buttons[NightEventWindow.Choice].NextEventNode != null)
                        rob.nightEvent.RootNode = rob.nightEvent.RootNode.Buttons[NightEventWindow.Choice].NextEventNode;
                    else
                        rob.nightEvent.RootNode = null;
                } else {
                    if (GetResult(m_currentEventNum) == false) {
                        rob.SetAsFailed();
                    } else {
                        rob.SetAsSuccesfull();
                    }
                    AssignNightEventDataToWindow(rob, eventTime);
                    yield return new WaitForSeconds(eventTime);
                    if (NightEventWindow.Choice == -1) {
                        MakeChoice(0);
                    }
                    RobberiesManager.Instance.ResetNightEvent(rob.Robbery.RobberyType, rob.Robbery.LocationNum);
                    rob.nightEvent = null;
                }
                UIManager.nightEventWindow.CloseWindow();            
            }
            FinishNight();
        }

        private void AssignNightEventDataToWindow(NightRobberyData rData, float time) {
            UnityAction windowSetUpMethod;
            RobberyType rt = rData.Robbery.RobberyType;
            int ln = rData.Robbery.LocationNum;
            Vector2 windowPostion = RobberiesManager.Instance.Robberies[rt][ln].transform.localPosition;
            switch (rData.Status) {
                case EventStatus.Success:
                    windowSetUpMethod = () => UIManager.nightEventWindow.ShowSuccess(rData.nightEvent.Success,
                        rData.Awards, rData.Money, windowPostion);
                    RobberiesManager.Instance.AddNightEvent(rData.Robbery.RobberyType,
                        rData.Robbery.LocationNum, windowSetUpMethod, EventStatus.Success, time);
                    break;
                case EventStatus.Fail:
                    windowSetUpMethod = () => UIManager.nightEventWindow.ShowFail(rData.nightEvent.Fail, windowPostion);
                    RobberiesManager.Instance.AddNightEvent(rData.Robbery.RobberyType,
                        rData.Robbery.LocationNum, windowSetUpMethod, EventStatus.Fail, time);
                    break;
                case EventStatus.InProgress:
                    windowSetUpMethod = () => UIManager.nightEventWindow.ShowChoice(rData.nightEvent.RootNode, windowPostion);
                    RobberiesManager.Instance.AddNightEvent(rData.Robbery.RobberyType,
                        rData.Robbery.LocationNum, windowSetUpMethod, EventStatus.InProgress, time);
                    break;
            }
        }

        private void FinishNight() {
            UpdateDataAfterNight();
            UIManager.nightResumeWindow.SetActive(true);
            m_isNight = false;
            OnNightEnded();
        }

        private bool GetEventNum(out int eventNum) {
            Debug.Log("robberies count: " + m_robberies.Count);
            if (m_robberies.Count == 0) {
                eventNum = -1;
                return false;
            }

            int rndEventNum = Random.Range(0, m_robberies.Count);

            for (int i = 0; m_robberies[rndEventNum].nightEvent == null; i++) {
                if (i > m_robberies.Count) {
                    Debug.Log("No more events");
                    eventNum = -1;
                    return false;
                }
                rndEventNum++;
                if (rndEventNum >= m_robberies.Count)
                    rndEventNum = 0;
            }

            eventNum = rndEventNum;
            return true;
        }

        private bool GetResult(int eventNum) {
            return (Random.Range(0f, 1f) < m_robberies[eventNum].Chance);
        }

        public void MakeChoice(int choiceNum) {
            NightEventWindow.Choice = choiceNum;
            RobberiesManager.Instance.ResetNightEvent(m_robberies[m_currentEventNum].Robbery.RobberyType,
                m_robberies[m_currentEventNum].Robbery.LocationNum);
        }

        private void ApplyChangesAfterChoice(int eventNum) {
            NightRobberyData rob = m_robberies[eventNum];
            NightEventButtonDetails bd = rob.nightEvent.RootNode.Buttons[NightEventWindow.Choice];
            rob.ApplyChoice(bd);
        }

        private void PrepareEvents() {
            m_robberies = new List<NightRobberyData>();

            foreach (Dictionary<int, Robbery> robberyType in DataScript.EData.RobberiesData.Values)
                foreach (Robbery robbery in robberyType.Values)
                    if (!robbery.IsRobberyEmpty())
                        m_robberies.Add(new NightRobberyData(robbery));
        }

        private void UpdateDataAfterDay() {
            foreach (Character character in DataScript.ChData.PanelCharacters) {
                //high: Implement!!!
                //character.LiveOneDay();
            }
        }

        private void UpdateDataAfterNight() {
            foreach (NightRobberyData nightRobDat in m_robberies) {
                DataScript.EData.PoliceKnowledge++;
                DataScript.SData.Money += nightRobDat.Money;
                foreach (int itemNum in nightRobDat.Awards.Keys)
                    DataScript.SData.ItemsCount[itemNum] += nightRobDat.Awards[itemNum];

                foreach (Character character in nightRobDat.Robbery.Characters) {
                    if (character.Stats.Health <= 0) {
                        character.AddToHospital();
                    }
                    character.SetDefaultStatus();
                }
                UIManager.robberyWindow.RemoveAllItemsFromRoobbery(nightRobDat.Robbery.RobberyType, nightRobDat.Robbery.LocationNum);
            }
            m_robberies.Clear();
            NightEventsOptions.ClearUsedEvents();
            //todo: delete
            RobberiesOptions.GetNewRobberies();
        }
    }

}