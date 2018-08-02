using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CriminalTown {

    public class BuyWindow : MonoBehaviour {
        public GameObject itemOptions;
        public Image itemImage;
        public Text description;
        public Text itemName;
        public Text priceText;
        public Text countText;
        public Slider countSlider;
        public Button buyButton;
        public GameObject buyWindowObject;


        private int price;
        private int money;
        private int number;


        public void SetBuyWindow(int numberOfItem) {
            Dictionary<ItemProperty, string> itemDict = ItemsOptions.GetItemData(numberOfItem);

            price = int.Parse(itemDict[ItemProperty.price]);
            number = numberOfItem;
            money = DataScript.SData.Money;



            buyWindowObject.SetActive(true);
            buyWindowObject.transform.SetAsLastSibling();

            itemImage.sprite = itemOptions.GetComponent<ItemsOptions>().itemsSprites[number];
            itemName.text = itemDict[ItemProperty.name];
            if (price > money) {
                countSlider.interactable = false;
                buyButton.interactable = false;
            } else {
                countSlider.interactable = true;
                buyButton.interactable = true;
                countSlider.maxValue = money / price <= 10 ? money / price : 10;
            }
            countSlider.value = 0;
            priceText.text = "0";
            countText.text = "0";
        }

        public void BuyButtonClick() {
            DataScript.SData.Money -= (int) countSlider.value * price;
            DataScript.SData.ItemsCount[number] += (int) countSlider.value;
            DataScript.SaveSourcesData();
            buyWindowObject.SetActive(false);
            WM1.itemsPanel.UpdateSingleItemWithAnimation(number);
        }

        public void OnSliderValueChanged() {
            countText.text = countSlider.value.ToString();
            priceText.text = (countSlider.value * price).ToString();
        }
    }

}