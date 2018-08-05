using UnityEngine;
using UnityEngine.UI;

namespace CriminalTown {

    public class MarketItemCustomization : MonoBehaviour {
        public int number;
        public Image itemImage;
        public Text itemName;
        public Text itemPrice;


        public void OnMarketItemClick() {
            UIManager.buyWindow.SetBuyWindow(number);
        }
    }

}