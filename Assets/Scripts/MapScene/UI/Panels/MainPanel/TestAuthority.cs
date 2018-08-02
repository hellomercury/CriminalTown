using UnityEngine;
using UnityEngine.UI;

namespace CriminalTown {

    public class TestAuthority : MonoBehaviour {
        public InputField authorityInputField;

        // Use this for initialization
        void Start() {
            authorityInputField.text = DataScript.PData.Authority.ToString();
        }

        // Update is called once per frame
        void Update() {

        }

        public void OnEndEditAuthority() {
            int.TryParse(authorityInputField.text, out DataScript.PData.Authority);
            DataScript.SaveProgressData();
        }
    }

}