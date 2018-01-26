using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropToRobbery : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    bool isEntered;

    public void OnPointerEnter(PointerEventData eventData)
    {
        isEntered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isEntered = false;
    }

    public void DropToRobberyUpdate()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (isEntered)
            {
                RobberyCustomization robCust = gameObject.GetComponent<RobberyCustomization>();
                int locNum = robCust.number;
                RobberyType robType = robCust.robberyType;

                if (Drag.ItemType == DraggeableItemType.Character)
                {
                    CharacterCustomization charCust;
                    Drop.DropObject(out charCust);

                    if (charCust != null)
                        if (charCust.status == CharacterStatus.normal)
                            WM1.robberyWindow.TryToAddCharacterToRobbery(charCust.character, robType, locNum);

                    isEntered = false;
                }
                if (Drag.ItemType == DraggeableItemType.Item)
                {
                    ItemCustomization iCust;
                    Drop.DropObject(out iCust);
                    if (iCust != null) WM1.robberyWindow.TryToAddItemToRobbery(iCust.number, robType, locNum);
                }
                isEntered = false;
            }
        }
    }
}