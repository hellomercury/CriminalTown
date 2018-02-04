using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropToRobberyWindow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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

    //void Update()
    public void DropToRobberyWindowUpdate()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (isEntered)
            {
                int locNum = RobberyWindow.robberyData.LocationNum;
                RobberyType robType = RobberyWindow.robberyData.RobberyType;
                if (Drag.ItemType == DraggeableItemType.Character)
                {
                    CharacterCustomization charCust;
                    Drop.DropObject(out charCust);

                    if (charCust != null)
                        if (charCust.status == CharacterStatus.normal)
                            WM1.robberyWindow.TryToAddCharacterToRobbery(charCust.character, robType, locNum);
                }
                if (Drag.ItemType == DraggeableItemType.Item)
                {
                    ItemCustomization iCust;
                    Drop.DropObject(out iCust);
                    if (iCust != null)
                        if (Drag.Location == DraggableObjectsLocations.itemsPanel)
                            WM1.robberyWindow.TryToAddItemToRobbery(iCust.number, robType, locNum);
                }
                isEntered = false;
            }
            else //if pointer exited
            {
                if (Drag.ItemType == DraggeableItemType.Character)
                {
                    CharacterCustomization charCust;
                    Drop.DropObject(out charCust);

                    if (charCust.status == CharacterStatus.robbery)
                        if (Drag.Location == DraggableObjectsLocations.robbery)
                            RobberyWindow.robberyData.RemoveCharacter(charCust.character);
                }
                if (Drag.ItemType == DraggeableItemType.Item)
                {
                    ItemCustomization iCust;
                    Drop.DropObject(out iCust);

                    if (Drag.Location == DraggableObjectsLocations.robbery)
                        WM1.robberyWindow.TryToRemoveItemFromRobbery(iCust.number, RobberyWindow.robberyData.RobberyType, RobberyWindow.robberyData.LocationNum);
                }
                isEntered = false;
            }
        }
    }
}