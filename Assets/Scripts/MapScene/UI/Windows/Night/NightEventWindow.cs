﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CriminalTown {

    public class NightEventWindow : MonoBehaviour {

        #region References

        [SerializeField]
        public Button Button0;
        [SerializeField]
        public Button Button1;
        [SerializeField]
        public Text Button0Text;
        [SerializeField]
        public Text Button1Text;

        [SerializeField]
        public Image Background;

        [SerializeField]
        public Text TitleText;
        [SerializeField]
        public Text DescriptionText;
        [SerializeField]
        public Image Image;

        [SerializeField]
        public GameObject AwardsAndMoney;
        [SerializeField]
        public Text MoneyText;
        [SerializeField]
        public Image MoneyImage;
        [SerializeField]
        public Image[] AwardsImages = new Image[4];
        [SerializeField]
        public Text[] AwardsTexts = new Text[4];

        #endregion

        public static int Choice;

        private void ShowNode(NightEventNode eventNode) {
            Choice = -1;

            Button0.gameObject.SetActive(false);
            Button1.gameObject.SetActive(false);


            AwardsAndMoney.gameObject.SetActive(false);

            TitleText.text = eventNode.titleText;
            DescriptionText.text = eventNode.description;
            Image.sprite = NightEventsOptions.GetNightEventSprite(eventNode.spriteType, eventNode.spriteId, eventNode.charSpriteType);

            if (eventNode.buttons.Count > 0) {
                Button0.onClick.RemoveAllListeners();
                Button0.onClick.AddListener(() => { Night.Instance.MakeChoice(0); });
                Button0.onClick.AddListener(CloseWindow);
                Button0Text.text = eventNode.buttons[0].buttonText;
                Button0.gameObject.SetActive(true);
            }
            if (eventNode.buttons.Count > 1) {
                Button1.onClick.RemoveAllListeners();
                Button1.onClick.AddListener(() => { Night.Instance.MakeChoice(1); });
                Button1.onClick.AddListener(CloseWindow);
                Button1Text.text = eventNode.buttons[1].buttonText;
                Button1.gameObject.SetActive(true);
            }

            gameObject.transform.SetAsLastSibling();
            gameObject.SetActive(true);
        }

        public void ShowChoice(NightEventNode eventNode) {
            Background.color = new Color(216f, 216f, 216f);
            ShowNode(eventNode);
        }

        public void ShowSuccess(NightEventNode successNode, Dictionary<int, int> awards, int money) {
            Background.color = new Color(0, 255f, 0);
            ShowNode(successNode);

            int i = 0;
            foreach (int itemNum in awards.Keys) {
                if (awards[itemNum] > 0) {
                    AwardsImages[i].gameObject.SetActive(true);
                    AwardsImages[i].sprite = WM1.itemsOptions.itemsSprites[itemNum];
                    AwardsTexts[i].text = awards[itemNum].ToString();
                } else
                    AwardsImages[i].gameObject.SetActive(false);
                i++;
            }

            for (; i < AwardsImages.Length; i++) {
                AwardsImages[i].gameObject.SetActive(false);
            }

            MoneyText.text = money.ToString();
            AwardsAndMoney.SetActive(true);
        }

        public void ShowFail(NightEventNode failNode) {
            Background.color = new Color(255f, 0, 0);
            ShowNode(failNode);
        }

        public void CloseWindow() {
            gameObject.SetActive(false);
        }
    }

}