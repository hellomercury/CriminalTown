using UnityEngine;

namespace CriminalTown {

    public class WM0 : MonoBehaviour {
        public GameObject modalPanelObject;

        public static ModalPanel modalPanel;


        private void Awake() {
            modalPanel = modalPanelObject.GetComponent<ModalPanel>();

        }
    }

}