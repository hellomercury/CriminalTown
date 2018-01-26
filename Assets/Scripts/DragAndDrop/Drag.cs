using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//public enum DraggableObjectsLocations { charactersPanel, itemsPanel, robbery}
public enum DraggeableItemType { Item, Character }

public class Drag : MonoBehaviour
{
    public static GameObject ItemBeingDragged { get; set; }
    public static bool IsObjectDragging { get; set; }
    public static DraggeableItemType ItemType { get; set; }

    public Transform customerScrollRect;
    public Transform dragParent;

    public float holdTime;
    public float maxScrollVelocityInDrag;

    //private DraggableObjectsLocations location;
    private Transform startParent;
    private ScrollRect scrollRect;

    private float timer;

    private static bool isHolding;
    private bool isDragging;
    private bool isPointerOverGameObject;


    private CanvasGroup canvasGroup;
    private Vector3 startPos;

    public Transform StartParent { get { return startParent; } }
    public Vector3 StartPos { get { return startPos; } }

    private bool isHorizontalScrollActive;
    private bool isVerticalScrollActive;

    public virtual void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        customerScrollRect = transform.parent.parent.parent.parent;
        dragParent = FindObjectOfType<Canvas>().transform;
        switch (gameObject.tag)
        {
            case "DraggableCharacter": ItemType = DraggeableItemType.Character; break;
            case "Draggabltem": ItemType = DraggeableItemType.Item; break;
        }
    }

    public virtual void Start()
    {
        timer = holdTime;
    }

    public virtual void DragUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.currentSelectedGameObject == gameObject)
            {
                scrollRect = customerScrollRect.GetComponent<ScrollRect>();
                isPointerOverGameObject = true;
                isHolding = true;
                StartCoroutine(Holding());
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (EventSystem.current.currentSelectedGameObject == gameObject)
            {
                isHolding = false;
                if (isVerticalScrollActive) customerScrollRect.GetComponent<ScrollRect>().vertical = true;
                if (isHorizontalScrollActive) customerScrollRect.GetComponent<ScrollRect>().horizontal = true;

                if (isDragging)
                {
                    ItemBeingDragged = null;
                    IsObjectDragging = false;
                    if (transform.parent == dragParent)
                    {
                        canvasGroup.blocksRaycasts = true;
                        gameObject.GetComponent<Animator>().SetTrigger("Dropped");
                        transform.SetParent(startParent);
                        transform.localPosition = startPos;
                    }
                    isDragging = false;
                    timer = holdTime;
                }
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (EventSystem.current.currentSelectedGameObject == gameObject)
            //if (gameObject.GetComponent<Button>().)
            {
                if (isDragging)
                {
                    transform.position = Input.mousePosition;
                }
                else
                {
                    if (!isPointerOverGameObject)
                    {
                        isHolding = false;
                    }
                }
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerOverGameObject = false;
    }

    public virtual IEnumerator Holding()
    {
        Vector2 posBeforeHolding = transform.position;
        while (timer > 0)
        {
            if (transform.position.magnitude - posBeforeHolding.magnitude > 10)
            {
                isHolding = false;
            }

            if (!isHolding)
            {
                timer = holdTime;
                yield break;
            }

            timer -= Time.deltaTime;
            yield return null;
        }

        gameObject.GetComponent<Animator>().SetTrigger("Dragged");

        if (customerScrollRect.GetComponent<ScrollRect>().vertical == true)
        {
            customerScrollRect.GetComponent<ScrollRect>().vertical = false;
            isVerticalScrollActive = true;
        }
        else isVerticalScrollActive = false;

        if (customerScrollRect.GetComponent<ScrollRect>().horizontal == true)
        {
            customerScrollRect.GetComponent<ScrollRect>().horizontal = false;
            isHorizontalScrollActive = true;
        }
        else isHorizontalScrollActive = false;


        IsObjectDragging = true;
        ItemBeingDragged = gameObject;
        startPos = transform.localPosition;
        startParent = transform.parent;
        isDragging = true;
        canvasGroup.blocksRaycasts = false;
        transform.SetParent(dragParent);
    }

    public void Reset()
    {
        isHolding = false;
        isDragging = false;
        isPointerOverGameObject = false;
    }
}
