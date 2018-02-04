using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RobberyWindow : MonoBehaviour
{
    #region References
    public GameObject robberyWindowObject;
    public Transform charactersLocation;
    public Button characterPrefab;
    public GameObject itemPrefab;
    public Transform itemsLocation;
    public Transform parentButton;

    public Text descriptionText;
    public Text nameText;
    public Image robberyImage;

    public GameObject itemsPanel;
    public GameObject charactersPanel;
    #endregion

    private RobberyType robType;
    private int locationNum;
    public static Robbery robberyData;

    private List<GameObject> items = new List<GameObject>();

    private Dictionary<Character, Button> charactersDict;

    public void SetRobberyWindow(RobberyType robberyType, int locationNumber)
    {
        this.robType = robberyType;
        this.locationNum = locationNumber;
        robberyData = DataScript.eData.robberiesData[robberyType][locationNumber];

        UpdateCharacters();
        UpdateItems();

        robberyImage.sprite = WM1.robberiesOptions.robberySprites[(int)robType];
        descriptionText.text = RobberiesOptions.GetRobberyData(robType, RobberyProperty.description);
        nameText.text = RobberiesOptions.GetRobberyData(robType, RobberyProperty.name);

        robberyWindowObject.SetActive(true);
        robberyWindowObject.transform.SetAsLastSibling();
    }

    public void UpdateCharacters()
    {
        if (charactersDict != null)
            foreach (Button character in charactersDict.Values)
                Destroy(character.gameObject);

        charactersDict = new Dictionary<Character, Button>();

        foreach (Character character in robberyData.Characters)
        {
            charactersDict.Add(character, Instantiate(characterPrefab, charactersLocation));
            charactersDict[character].GetComponent<CharacterCustomization>().CustomizeCharacter(character);
        }
    }

    public void UpdateItems()
    {
        foreach (GameObject item in items) Destroy(item.gameObject);
        items.Clear();

        int j = 0;
        foreach (int number in robberyData.Items.Keys)
        {
            items.Add(Instantiate(itemPrefab, itemsLocation));
            items[j].GetComponent<ItemCustomization>().number = number;
            items[j].GetComponent<ItemCustomization>().itemImage.sprite = WM1.itemsOptions.itemsSprites[number];
            items[j].GetComponent<ItemCustomization>().itemCount.text = robberyData.Items[number].ToString();
            items[j].GetComponent<ItemCustomization>().itemName.text = ItemsOptions.GetItemData(number)[ItemProperty.name];
            items[j].SetActive(true);
            j++;
        }
    }

    public void TryToAddCharacterToRobbery(Character character, RobberyType robType, int locNum)
    {
        if (character.Health <= 10)
        {
            EventButtonDetails yesButton = new EventButtonDetails
            {
                buttonText = "Да мне плевать",
                action = () => { DataScript.eData.robberiesData[robType][locNum].AddCharacter(character); }
            };
            EventButtonDetails noButton = new EventButtonDetails
            {
                buttonText = "Ладно, отдыхай",
                action = WM1.modalPanel.ClosePanel
            };
            ModalPanelDetails details = new ModalPanelDetails
            {
                button0Details = yesButton,
                button1Details = noButton,
                imageSprite = character.Sprite,
                text = "Босс, может мне лучше в больницу?",
                titletext = character.Name
            };
            WM1.modalPanel.CallModalPanel(details);
        }
        else
        {
            DataScript.eData.robberiesData[robType][locNum].AddCharacter(character);
        }

    }

    public void TryToAddItemToRobbery(int itemNumber, RobberyType robberyType, int locationNum)
    {
        WM1.robberyItemsWindow.SetItemsWindow(itemNumber, isItemAdding: true, robberyType: robberyType, locationNum: locationNum);
    }

    public void AddItemToRobberyAndUpdate(int itemNumber, int itemCount, RobberyType robberyType, int locationNum)
    {
        DataScript.sData.itemsCount[itemNumber] -= itemCount;
        if (DataScript.eData.robberiesData[robberyType][locationNum].Items.ContainsKey(itemNumber))
        {
            DataScript.eData.robberiesData[robberyType][locationNum].Items[itemNumber] += itemCount;
        }
        else DataScript.eData.robberiesData[robberyType][locationNum].Items.Add(itemNumber, itemCount);
        //Debug.Log

        if (robberyWindowObject.activeInHierarchy) UpdateItems();
        WM1.itemsPanel.UpdateSingleItemWithAnimation(itemNumber);
    }

    public void TryToRemoveItemFromRobbery(int itemNumber, RobberyType robberyType, int locationNum)
    {
        WM1.robberyItemsWindow.SetItemsWindow(itemNumber, isItemAdding: false, robberyType: robberyType, locationNum: locationNum);
    }

    public void RemoveItemFromRobberyAndUpdate(int itemNumber, int itemCount, RobberyType robberyType, int locationNum)
    {
        DataScript.sData.itemsCount[itemNumber] += itemCount;
        if (DataScript.eData.robberiesData[robberyType][locationNum].Items[itemNumber] == itemCount)
        {
            DataScript.eData.robberiesData[robberyType][locationNum].Items.Remove(itemNumber);
        }
        else DataScript.eData.robberiesData[robberyType][locationNum].Items[itemNumber] -= itemCount;
        UpdateItems();
        WM1.itemsPanel.UpdateSingleItemWithAnimation(itemNumber);
    }

    public void RemoveAllItemsFromRoobbery(RobberyType robberyType, int locationNum)
    {
        if (DataScript.eData.robberiesData[robberyType][locationNum].Items != null)
        {
            foreach (var item in DataScript.eData.robberiesData[robberyType][locationNum].Items)
            {
                DataScript.sData.itemsCount[item.Key] += item.Value;
                Debug.Log(DataScript.sData.itemsCount[item.Key]);
            }
            DataScript.eData.robberiesData[robberyType][locationNum].Items.Clear();
        }
    }

    public void CloseRobberyWindow()
    {
        robberyWindowObject.SetActive(false);
    }
}
