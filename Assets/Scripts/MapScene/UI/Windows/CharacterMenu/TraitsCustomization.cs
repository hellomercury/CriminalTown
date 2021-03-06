﻿using UnityEngine;
using UnityEngine.UI;

namespace CriminalTown {

    public class TraitsCustomization : MonoBehaviour {
        public Text traitNameText;
        public Image image;
        public Text descriptionText;

        public void CustomizeTrait(Trait trait) {
            traitNameText.text = trait.name;
            descriptionText.text = trait.description;
            image.sprite = trait.Sprite;
        }
    }

}