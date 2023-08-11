using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Radial_CraftSlots_Scroller : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IConfigurablePanel
{

    [SerializeField] private Radial_CraftSlots_Controller radial_CraftSlots_Controller;
    [SerializeField] private Radial_CraftSlots_Crafter radial_CraftSlots_Crafter;
    [SerializeField] private Image craftButton_Image;
    private Image scrollerImage;
    private float validClickDuration = .25f;
    private bool isValidClick = false;
    public bool isValidTimeBetweenClicks = true;
    private IEnumerator co = null;

    private Vector3 initialclickPosition;
    private float initialClicktime;
    private bool isPointerDown = false;
    private bool previousPositionTaken = false;
    private bool isDragged = false;
    private Vector3 previousPointerPosition = new Vector3();
    private Vector3 pointerDelta;

    private bool isBarVisible = false;


    private void OnDisable()
    {
        CraftWheel_Controller.isBarVisible -= SetBarVisibility;
    }
    public void PanelConfig()
    {
        //craftButton_Image.alphaHitTestMinimumThreshold = 1;
        scrollerImage = GetComponent<Image>();
        CraftWheel_Controller.isBarVisible += SetBarVisibility;
    }

    private void SetBarVisibility(bool isBarVisibleIN)
    {
        if (isBarVisibleIN)
        {
            scrollerImage.raycastTarget = true;
        }
        else
        {
            scrollerImage.raycastTarget = false;
            radial_CraftSlots_Controller.SetRotation(SpinType.Type.BackSpin);
        }
        isBarVisible = isBarVisibleIN;
    }

    private void Update()
    {
        if (!isBarVisible)
        {
            return;
        }

        if (isPointerDown)
        {
            if (!previousPositionTaken && !isDragged)
            {
                previousPointerPosition = Input.mousePosition;
                previousPositionTaken = true;
                initialclickPosition = previousPointerPosition;
                initialClicktime = Time.time;
            }
            else
            {
                isDragged = true;
                pointerDelta = Input.mousePosition - previousPointerPosition;

                radial_CraftSlots_Controller.SetRotation(SpinType.Type.ControlledSpin, Mathf.Clamp(pointerDelta.y, -3.2f, 3.2f));

                previousPointerPosition = Input.mousePosition;
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isValidTimeBetweenClicks)
        {
            return;
        }
        isPointerDown = true;
        isValidTimeBetweenClicks = false;

        StartCoroutine(CalculateValidTimeBEtweenClicks());

        if (co != null)
        {
            StopCoroutine(co);
        }
        co = ValidateClick();
        StartCoroutine(co);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isValidClick && Vector2.Distance((Vector2)initialclickPosition, eventData.position) <= PanelManager.MAXCLICKOFFSET) 
        {
            isPointerDown = false;
            previousPositionTaken = false;
            isDragged = false;

            InteractWithButtons(eventData);
        }
        else if ((Vector2)initialclickPosition != eventData.position)
        {
            isPointerDown = false;
            previousPositionTaken = false;
            isDragged = false;

            MouseBUttonReleased();
            return;
        }
        else    //if (!isValidClick)
        {
            isPointerDown = false;
            previousPositionTaken = false;
            isDragged = false;

            MouseBUttonReleased();
            return;
        }
/*
        if ((Vector2)initialclickPosition != eventData.position)    //initialclickPosition.x != eventData.position.x || initialclickPosition.y != eventData.position.y)
        {
            isPointerDown = false;
            previousPositionTaken = false;
            isDragged = false;

            MouseBUttonReleased();
            return;
        }*/

        /*isPointerDown = false;
        previousPositionTaken = false;
        isDragged = false;

        InteractWithButtons(eventData);*/
    }

    private void MouseBUttonReleased()
    {

        Vector3 pointerPosDifference = Input.mousePosition - initialclickPosition;
        float timeDiffernce = Time.time - initialClicktime;
        float angularVelocity = pointerPosDifference.y / timeDiffernce;
        initialClicktime = 0f;
        pointerDelta = Input.mousePosition - previousPointerPosition;


        if (Mathf.Approximately(pointerDelta.y, 0f) && pointerPosDifference.y != 0)
        {

            radial_CraftSlots_Controller.SetRotation(SpinType.Type.BackSpin);
            return;
        }

        radial_CraftSlots_Controller.SetRotation(SpinType.Type.FreeSpin, Mathf.Clamp(angularVelocity, -300f, 300f));
    }

    IEnumerator ValidateClick()
    {
        float elapsedTime = 0f;

        while (elapsedTime < validClickDuration)
        {
            isValidClick = true;
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        isValidClick = false;
        co = null;
    }

    IEnumerator CalculateValidTimeBEtweenClicks()
    {
        float elapsedTime = 0f;

        while (elapsedTime < validClickDuration)
        {
            elapsedTime += Time.deltaTime;

            yield return null;
        }
        isValidTimeBetweenClicks = true;
    }

    private void InteractWithButtons(PointerEventData eventData)
    {
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        Single_CraftedItem single_CraftSlot = null;
        CraftPanel_Button craft_Button = null;

        foreach (var result in results)
        {
            single_CraftSlot = result.gameObject.GetComponent<Single_CraftedItem>();
            craft_Button = result.gameObject.GetComponent<CraftPanel_Button>();

            if (single_CraftSlot != null || craft_Button != null)
            {
                break;
            }
        }

        if (single_CraftSlot != null)
        {
            radial_CraftSlots_Crafter.ReclaimCrafted(single_CraftSlot);
            return;
        }

        else if (craft_Button != null)
        {
            ExecuteEvents.Execute(craft_Button.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerUpHandler);
            //craft_Button.InvokePanel();
            return;
        }

        MouseBUttonReleased();
    }

}
