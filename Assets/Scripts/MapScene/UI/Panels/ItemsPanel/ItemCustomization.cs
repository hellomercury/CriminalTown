using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace CriminalTown {

    public class ItemCustomization : MonoBehaviour {
        private const int animationDuration = 5;
        private float timer = 0;

        public int number;
        public Image itemImage;
        public Text itemName;
        public Text itemCount;

        public IEnumerator ItemAnimation() {
            timer = 0;
            Color defaultColor = itemCount.color;
            while (timer < 1) {
                timer += Time.deltaTime / animationDuration;
                itemCount.color = Color.Lerp(Color.red, defaultColor, timer);
                yield return null;
            }
        }
    }

}