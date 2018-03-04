using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum GUIMode { Day, Night }

public class NightButton : MonoBehaviour
{
    public static bool isNight;

    private List<NightRobberyData> robberies;

    public Image map;
    Color defaultMapColor;
    public GameObject nightAnimation;

    private float animationTime = 2;
    private float animationTimer = 0;

    private float eventTime = 4;

    private int currentEventNum;


    private bool CheckProblemsBeforeNight()
    {
        foreach (Character character in DataScript.chData.PanelCharacters)
        {
            if (character.Status == CharacterStatus.arrested)
                if (character.DaysLeft < 2)
                {
                    EventButtonDetails yesButton = new EventButtonDetails
                    {
                        buttonText = "Да",
                        action = StartNight
                    };
                    EventButtonDetails noButton = new EventButtonDetails
                    {
                        buttonText = "Нет",
                        action = WM1.modalPanel.ClosePanel
                    };
                    ModalPanelDetails details = new ModalPanelDetails
                    {
                        button0Details = yesButton,
                        button1Details = noButton,
                        imageSprite = character.Sprite,
                        text = "Этот персонаж скоро нас сдаст. Босс, ты уверен, что стоит оставить его в грязных руках копов?",
                        titletext = character.Name
                    };
                    WM1.modalPanel.CallModalPanel(details);
                    return false;
                }
        }
        return true;
    }

    public void OnNightButtonClick()
    {
        if (CheckProblemsBeforeNight()) StartNight();
    }

    private void StartNight()
    {
        isNight = true;

        SetActiveMapAndUpdate(false);

        StartCoroutine(NightAnimation());
        UpdateDataAfterDay();
        PrepareEvents();
    }

    IEnumerator NightAnimation()
    {
        nightAnimation.SetActive(true);

        animationTimer = 0;
        defaultMapColor = map.color;
        while (animationTimer < 1)
        {
            animationTimer += Time.deltaTime / animationTime;
            map.color = Color.Lerp(defaultMapColor, Color.black, animationTimer);
            yield return null;
        }

        nightAnimation.SetActive(false);
        yield return new WaitForSeconds(eventTime);

        StartCoroutine(NightEvents());
        yield break;
    }

    IEnumerator NightEvents()
    {
        while (GetEventNum(out currentEventNum))
        {
            Debug.Log("Call robbery event: " + currentEventNum);
            NightRobberyData rob = robberies[currentEventNum];

            if (rob.nightEvent.rootNode != null)
            {
                RM.rmInstance.AddNightEvent(rob.Robbery.RobberyType, rob.Robbery.LocationNum,
                    () => { WM1.nightEventWindow.ShowChoice(rob.nightEvent.rootNode); }, EventStatus.inProgress, eventTime);
                yield return new WaitForSeconds(eventTime);
                if (NightEventWindow.choice == -1)
                {
                    WM1.nightEventWindow.CloseWindow();
                    MakeChoice(Random.Range(0, rob.nightEvent.rootNode.buttons.Count));
                }
                RM.rmInstance.ResetNightEvent(rob.Robbery.RobberyType, rob.Robbery.LocationNum);
                ApplyChangesAfterChoice(currentEventNum);

                if (rob.nightEvent.rootNode.buttons[NightEventWindow.choice].nextEventNode != null)
                    rob.nightEvent.rootNode = rob.nightEvent.rootNode.buttons[NightEventWindow.choice].nextEventNode;
                else rob.nightEvent.rootNode = null;
            }
            else
            {
                switch (GetResult(currentEventNum))
                {
                    case false:
                        RM.rmInstance.AddNightEvent(rob.Robbery.RobberyType, rob.Robbery.LocationNum,
                            () => { WM1.nightEventWindow.ShowFail(rob.nightEvent.fail); }, EventStatus.fail, eventTime);

                        yield return new WaitForSeconds(eventTime);
                        if (NightEventWindow.choice == -1)
                        {
                            WM1.nightEventWindow.CloseWindow();
                            MakeChoice(0);
                        }
                        RM.rmInstance.ResetNightEvent(rob.Robbery.RobberyType, rob.Robbery.LocationNum);
                        rob.SetAsFailed();
                        break;
                    case true:
                        RM.rmInstance.AddNightEvent(rob.Robbery.RobberyType, rob.Robbery.LocationNum,
                            () => { WM1.nightEventWindow.ShowSuccess(rob.nightEvent.success, rob.Awards, rob.Money); }, EventStatus.success, eventTime);

                        yield return new WaitForSeconds(eventTime);
                        if (NightEventWindow.choice == -1)
                        {
                            MakeChoice(0);
                            WM1.nightEventWindow.CloseWindow();
                        }
                        RM.rmInstance.ResetNightEvent(rob.Robbery.RobberyType, rob.Robbery.LocationNum);
                        rob.SetAsSuccesfull();
                        break;
                };
                rob.nightEvent = null;
            }
        }

        StartCoroutine(DayAnimation());
        UpdateDataAfterNight();
        yield break;
    }

    IEnumerator DayAnimation()
    {
        nightAnimation.SetActive(true);

        animationTimer = 0;
        while (animationTimer < 1)
        {
            animationTimer += Time.deltaTime / animationTime;
            map.color = Color.Lerp(Color.black, defaultMapColor, animationTimer);
            yield return null;
        }

        nightAnimation.SetActive(false);

        yield return new WaitForSeconds(eventTime);


        FinishNight();
        yield break;
    }

    private void FinishNight()
    {
        WM1.nightResumeWindow.SetActive(true);
        SetActiveMapAndUpdate(true);

        isNight = false;
    }

    private void PrepareEvents()
    {
        robberies = new List<NightRobberyData>();

        foreach (Dictionary<int, Robbery> robberyType in DataScript.eData.robberiesData.Values)
            foreach (Robbery robbery in robberyType.Values)
                if (!robbery.IsRobberyEmpty()) robberies.Add(new NightRobberyData(robbery));
    }

    private void UpdateDataAfterDay()
    {
        foreach (Character character in DataScript.chData.PanelCharacters)
        {
            character.LiveOneDay();
        }
    }

    private void UpdateDataAfterNight()
    {
        foreach (NightRobberyData nightRobDat in robberies)
        {
            DataScript.eData.policeKnowledge++;
            DataScript.sData.money += nightRobDat.Money;
            foreach (int itemNum in nightRobDat.Awards.Keys)
                DataScript.sData.itemsCount[itemNum] += nightRobDat.Awards[itemNum];

            foreach (Character character in nightRobDat.Robbery.Characters)
            {
                if (character.Health <= 0)
                {
                    character.AddToHospital();
                }
                character.SetDefaultStatus();
            }
            WM1.robberyWindow.RemoveAllItemsFromRoobbery(nightRobDat.Robbery.RobberyType, nightRobDat.Robbery.LocationNum);
        }
        robberies.Clear();
        NightEventsOptions.ClearUsedEvents();
        RobberiesOptions.GetNewRobberies();
    }

    private void SetActiveMapAndUpdate(bool status)
    {
        if (status == false)
        {
            WM1.wm1Instance.CloseAllDayWindows();
            RM.rmInstance.DeactivateAllRobberies();
        }
        else if (status == true)
        {
            RM.rmInstance.UpdateRobberies();
            WM1.charactersPanel.UpdateCharactersPanel();
            WM1.itemsPanel.UpdateItemsPanel();
        }

        WM1.SetActivePanels(status);
        PM.SetActiveAllPlaces(status);
    }

    private bool GetEventNum(out int eventNum)
    {
        if (robberies.Count == 0)
        {
            eventNum = -1;
            return false;
        }

        int rndEventNum = Random.Range(0, robberies.Count);

        for (int i = 0; robberies[rndEventNum].nightEvent == null; i++)
        {
            if (i > robberies.Count)
            {
                Debug.Log("No more events");
                eventNum = -1;
                return false;
            }
            rndEventNum++;
            if (rndEventNum >= robberies.Count) rndEventNum = 0;
        }

        eventNum = rndEventNum;
        return true;
    }

    public void MakeChoice(int choiceNum)
    {
        NightEventWindow.choice = choiceNum;
        RM.rmInstance.ResetNightEvent(robberies[currentEventNum].Robbery.RobberyType,
            robberies[currentEventNum].Robbery.LocationNum);
    }

    public bool GetResult(int eventNum)
    {
        return (Random.Range(0f, 1f) < robberies[eventNum].Chance);
    }

    public void ApplyChangesAfterChoice(int eventNum)
    {
        NightRobberyData rob = robberies[eventNum];
        NightEventButtonDetails bd = rob.nightEvent.rootNode.buttons[NightEventWindow.choice];
        rob.ApplyChoice(bd);
    }
}
