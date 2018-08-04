using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CriminalTown {

    public class NightEventArgs : EventArgs {
        private Character m_character;

        public Character Character {
            get {
                return m_character;
            }
        }
    }

    public class Night : MonoBehaviour {
        private static Night m_instance;

        private bool m_isNight;
        private List<NightRobberyData> m_robberies;
        private int m_currentEventNum;
        //todo Разобраться со временем каждого ивента
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

        public delegate void NightEvent(); //NightEventArgs nightEventArgs);

        public NightEvent OnNightBegan;
        public NightEvent OnNightEnded;

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
                            action = WM1.modalPanel.ClosePanel
                        };
                        ModalPanelDetails details = new ModalPanelDetails {
                            button0Details = yesButton,
                            button1Details = noButton,
                            imageSprite = character.Sprite,
                            text = "Этот персонаж скоро нас сдаст. Босс, ты уверен, что стоит оставить его в грязных руках копов?",
                            titletext = character.Name
                        };
                        WM1.modalPanel.CallModalPanel(details);
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

                if (rob.nightEvent.RootNode != null) {
                    RobberyManager.RmInstance.AddNightEvent(rob.Robbery.RobberyType, rob.Robbery.LocationNum,
                        () => { WM1.nightEventWindow.ShowChoice(rob.nightEvent.RootNode); }, EventStatus.InProgress, eventTime);
                    yield return new WaitForSeconds(eventTime);
                    if (NightEventWindow.Choice == -1) {
                        WM1.nightEventWindow.CloseWindow();
                        MakeChoice(Random.Range(0, rob.nightEvent.RootNode.Buttons.Count));
                    }
                    RobberyManager.RmInstance.ResetNightEvent(rob.Robbery.RobberyType, rob.Robbery.LocationNum);
                    ApplyChangesAfterChoice(m_currentEventNum);

                    if (rob.nightEvent.RootNode.Buttons[NightEventWindow.Choice].NextEventNode != null)
                        rob.nightEvent.RootNode = rob.nightEvent.RootNode.Buttons[NightEventWindow.Choice].NextEventNode;
                    else
                        rob.nightEvent.RootNode = null;
                } else {
                    switch (GetResult(m_currentEventNum)) {
                        case false:
                            RobberyManager.RmInstance.AddNightEvent(rob.Robbery.RobberyType, rob.Robbery.LocationNum,
                                () => { WM1.nightEventWindow.ShowFail(rob.nightEvent.Fail); }, EventStatus.Fail, eventTime);

                            yield return new WaitForSeconds(eventTime);
                            if (NightEventWindow.Choice == -1) {
                                WM1.nightEventWindow.CloseWindow();
                                MakeChoice(0);
                            }
                            RobberyManager.RmInstance.ResetNightEvent(rob.Robbery.RobberyType, rob.Robbery.LocationNum);
                            rob.SetAsFailed();
                            break;
                        case true:
                            RobberyManager.RmInstance.AddNightEvent(rob.Robbery.RobberyType, rob.Robbery.LocationNum,
                                () => { WM1.nightEventWindow.ShowSuccess(rob.nightEvent.Success, rob.Awards, rob.Money); }, EventStatus.Success, eventTime);

                            yield return new WaitForSeconds(eventTime);
                            if (NightEventWindow.Choice == -1) {
                                MakeChoice(0);
                                WM1.nightEventWindow.CloseWindow();
                            }
                            RobberyManager.RmInstance.ResetNightEvent(rob.Robbery.RobberyType, rob.Robbery.LocationNum);
                            rob.SetAsSuccesfull();
                            break;
                    }
                    rob.nightEvent = null;
                }
            }

            UpdateDataAfterNight();
            FinishNight();
        }

        private void FinishNight() {
            WM1.nightResumeWindow.SetActive(true);
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
            RobberyManager.RmInstance.ResetNightEvent(m_robberies[m_currentEventNum].Robbery.RobberyType,
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
                WM1.robberyWindow.RemoveAllItemsFromRoobbery(nightRobDat.Robbery.RobberyType, nightRobDat.Robbery.LocationNum);
            }
            m_robberies.Clear();
            NightEventsOptions.ClearUsedEvents();
            RobberiesOptions.GetNewRobberies();
        }
    }

}