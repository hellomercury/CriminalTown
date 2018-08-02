using UnityEngine;
using UnityEngine.UI;

namespace CriminalTown {

    public class MarketItemCustomization : MonoBehaviour {
        public int number;
        public Image itemImage;
        public Text itemName;
        public Text itemPrice;


        public void OnMarketItemClick() {
            WM1.buyWindow.SetBuyWindow(number);
        }
    }

}