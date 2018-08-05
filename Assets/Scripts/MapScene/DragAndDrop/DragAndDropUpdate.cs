using UnityEngine;

namespace CriminalTown {

    public class DragAndDropUpdate : MonoBehaviour {
        public GameObject hospitalButton;
        //public GameObject hospitalWindow;
        public GameObject robberies;


        public RobberyManager robUpdate;

        private void Update() {
            if (!Night.Instance.IsNight) {
                //Update drop
                if (Drag.IsObjectDragging) {
                    if (WM1.hospital.gameObject.activeInHierarchy)
                        WM1.hospital.GetComponent<DropToHospitalWindow>().DropToHospitalWindowUpdate();
                    hospitalButton.GetComponent<DropToHospital>().DropToHospitalButtonUpdate();

                    if (WM1.robberyWindow.gameObject.activeInHierarchy)
                        WM1.robberyWindow.GetComponent<DropToRobberyWindow>().DropToRobberyWindowUpdate();

                    foreach (RobberyType robberyType in robUpdate.RobberiesObjects.Keys) {
                        for (int i = 0; i < robUpdate.RobberiesObjects[robberyType].Length; i++) {
                            GameObject robbery = robUpdate.RobberiesObjects[robberyType][i];
                            if (robbery.GetComponent<RobberyCustomization>().IsAvailable) {
                                robbery.GetComponent<DropToRobbery>().DropToRobberyUpdate();
                            }
                        }
                    }
                }

                //Then update drag
                foreach (GameObject item in GameObject.FindGameObjectsWithTag("DraggableItem"))
                    item.GetComponent<Drag>().DragUpdate();
                foreach (GameObject character in GameObject.FindGameObjectsWithTag("DraggableCharacter"))
                    character.GetComponent<Drag>().DragUpdate();
            }
        }
    }

}