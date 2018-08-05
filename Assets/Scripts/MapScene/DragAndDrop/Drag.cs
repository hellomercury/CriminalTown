using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace CriminalTown {

    public enum DraggableObjectsLocations {
        charactersPanel,
        itemsPanel,
        robbery
    }

    public enum DraggeableItemType {
        Item,
        Character
    }

    public class Drag : MonoBehaviour //, IPointerExitHandler
    {
        public static GameObject ItemBeingDragged { get; set; }

        public static bool IsObjectDragging { get; set; }

        public static DraggeableItemType ItemType { get; set; }

        public static DraggableObjectsLocations Location { get; set; }


        private Transform dragParent;

        public const float holdTime = 0.5f;
        public const float maxHoldingAreaRadius = 10;

        private Transform startParent;
        private ScrollRect scrollRect;

        private float timer;

        private static bool isHolding;
        private bool isDragging;
        //private bool isPointerOverGameObject;


        private CanvasGroup canvasGroup;
        private Vector3 startPos;

        public Transform StartParent {
            get {
                return startParent;
            }
        }

        public Vector3 StartPos {
            get {
                return startPos;
            }
        }

        private bool isHorizontalScrollActive;
        private bool isVerticalScrollActive;

        public virtual void Awake() {
            canvasGroup = GetComponent<CanvasGroup>();
            scrollRect = transform.parent.parent.parent.parent.GetComponent<ScrollRect>();
            dragParent = FindObjectOfType<Canvas>().transform;
        }

        private void GetCurrentLocation() {
            if (gameObject.transform.parent.parent == UIManager.charactersPanel.charactersLocation)
                Location = DraggableObjectsLocations.charactersPanel;
            else if (gameObject.transform.parent.parent == UIManager.itemsPanel.itemsLocation)
                Location = DraggableObjectsLocations.itemsPanel;
            else if (gameObject.transform.parent.parent == UIManager.robberyWindow.itemsLocation)
                Location = DraggableObjectsLocations.robbery;
            else if (gameObject.transform.parent.parent == UIManager.robberyWindow.charactersLocation)
                Location = DraggableObjectsLocations.robbery;
        }

        private void GetCurrentItemType() {
            switch (gameObject.tag) {
                case "DraggableCharacter":
                    ItemType = DraggeableItemType.Character;
                    break;
                case "DraggableItem":
                    ItemType = DraggeableItemType.Item;
                    break;
            }
        }

        public virtual void Start() {
            timer = holdTime;
        }

        public virtual void DragUpdate() {
            //DisplayManager.Instance().DisplayMessage(Input.touchCount.ToString());
            if (Input.GetMouseButtonDown(0)) {
                if (EventSystem.current.currentSelectedGameObject == gameObject) {
                    //isPointerOverGameObject = true;
                    isHolding = true;
                    StartCoroutine(Holding());
                }
            }

            if ((Input.GetMouseButtonUp(0) && EventSystem.current.currentSelectedGameObject == gameObject)
                || Input.touchCount > 1) {
                isHolding = false;
                if (isVerticalScrollActive)
                    scrollRect.GetComponent<ScrollRect>().vertical = true;
                if (isHorizontalScrollActive)
                    scrollRect.GetComponent<ScrollRect>().horizontal = true;

                if (isDragging) {
                    ItemBeingDragged = null;
                    IsObjectDragging = false;
                    if (transform.parent == dragParent) {
                        canvasGroup.blocksRaycasts = true;
                        gameObject.GetComponent<Animator>().SetTrigger("Dropped");
                        transform.SetParent(startParent);
                        transform.localPosition = startPos;
                    }
                    isDragging = false;
                    timer = holdTime;
                }
            }

            if (Input.GetMouseButton(0)) {
                if (EventSystem.current.currentSelectedGameObject == gameObject) {
                    if (isDragging) {
                        transform.position = Input.mousePosition;
                    }
                }
                //else
                //{
                //    if (!isPointerOverGameObject)
                //    {
                //        isHolding = false;
                //    }
                //}
            }
        }

        //public void OnPointerExit(PointerEventData eventData)
        //{
        //    isPointerOverGameObject = false;
        //}

        public virtual IEnumerator Holding() {
            Vector2 posBeforeHolding = Input.mousePosition;
            while (timer > 0) {
                if (Math.Abs(Input.mousePosition.magnitude - posBeforeHolding.magnitude) > maxHoldingAreaRadius) {
                    isHolding = false;
                }

                if (!isHolding) {
                    timer = holdTime;
                    yield break;
                }

                timer -= Time.deltaTime;
                yield return null;
            }

            gameObject.GetComponent<Animator>().SetTrigger("Dragged");

            if (scrollRect.vertical == true) {
                scrollRect.vertical = false;
                isVerticalScrollActive = true;
            } else
                isVerticalScrollActive = false;

            if (scrollRect.horizontal == true) {
                scrollRect.horizontal = false;
                isHorizontalScrollActive = true;
            } else
                isHorizontalScrollActive = false;

            GetCurrentLocation();
            GetCurrentItemType();

            IsObjectDragging = true;
            ItemBeingDragged = gameObject;
            startPos = transform.localPosition;
            startParent = transform.parent;
            isDragging = true;
            canvasGroup.blocksRaycasts = false;
            transform.SetParent(dragParent);

            Debug.Log(Location + " " + ItemType);
        }

        public void Reset() {
            isHolding = false;
            isDragging = false;
            //isPointerOverGameObject = false;
        }
    }

}