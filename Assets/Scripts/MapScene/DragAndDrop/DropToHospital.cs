﻿using UnityEngine;
using UnityEngine.EventSystems;

namespace CriminalTown {

    public class DropToHospital : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
        bool isEntered;
        public GameObject hospital;

        public void OnPointerEnter(PointerEventData eventData) {
            isEntered = true;
        }

        public void OnPointerExit(PointerEventData eventData) {
            isEntered = false;
        }

        public void DropToHospitalButtonUpdate() {
            if (Input.GetMouseButtonUp(0)) {
                if (isEntered) {
                    if (Drag.ItemType == DraggeableItemType.Character) {
                        CharacterCustomization charCust;
                        Drop.DropObject(out charCust);

                        if (charCust != null)
                            if (charCust.Character.Status == CharacterStatus.Normal)
                                hospital.GetComponent<Hospital>().TryToAddCharacterToHospital(charCust.Character);
                    }
                    isEntered = false;
                }
            }
        }
    }

}