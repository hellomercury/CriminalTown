using UnityEngine;

namespace CriminalTown {

    public class DragAndDropUpdate : MonoBehaviour {
        public GameObject hospitalButton;
        //public GameObject hospitalWindow;

        public RobberiesManager robUpdate;

        private void Update() {
            if (!Night.Instance.IsNight) {
                //Update drop
                if (Drag.IsObjectDragging) {
                    if (UIManager.hospital.gameObject.activeInHierarchy)
                        UIManager.hospital.GetComponent<DropToHospitalWindow>().DropToHospitalWindowUpdate();
                    hospitalButton.GetComponent<DropToHospital>().DropToHospitalButtonUpdate();

                    if (UIManager.robberyWindow.gameObject.activeInHierarchy)
                        UIManager.robberyWindow.GetComponent<DropToRobberyWindow>().DropToRobberyWindowUpdate();

                    foreach (RobberyType robberyType in robUpdate.Robberies.Keys) {
                        for (int i = 0; i < robUpdate.Robberies[robberyType].Length; i++) {
                            RobberyCustomization robbery = robUpdate.Robberies[robberyType][i];
                            if (robbery.IsAvailable) {
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