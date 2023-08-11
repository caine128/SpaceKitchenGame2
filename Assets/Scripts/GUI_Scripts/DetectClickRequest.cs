using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class DetectClickRequest<T_BluePrint> : MonoBehaviour, IPointerDownHandler, IPointerUpHandler //, IPanelInvokeButton
    where T_BluePrint:SortableBluePrint_Base
{
    protected static readonly float validClickDuration = .25f;

    protected bool isValidClick = false;
    protected Vector2 initialClickPosition = default(Vector2);
    protected object initialSelection = null;
    protected IEnumerator co = null;



    public void OnPointerDown(PointerEventData eventData)
    {
        if (co != null)
        {
            StopCoroutine(co);
        }

        initialClickPosition = eventData.position;
        initialSelection = CheckObjectUnderScrollRect(eventData);
        if (initialSelection != null)
        {

            co = ValidateClick();
            StartCoroutine(co);
        }
    }

    public abstract void OnPointerUp(PointerEventData eventData);
    
    
    protected object CheckObjectUnderScrollRect(PointerEventData eventDataIN)
    {       
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataIN, results);
        

        foreach (var result in results)
        {
            var openInfoButton = result.gameObject.GetComponent<OpenInfoPanelButton<T_BluePrint>>();
            var blueprintContainer = result.gameObject.GetComponent<Container<T_BluePrint>>();

            if (openInfoButton != null)
            {
                return openInfoButton;

            }
            else if (blueprintContainer != null)
            {
                return blueprintContainer;
            }
        }

        return null;
    }
    /*
    protected object CheckObjectUnderScrollRect(PointerEventData eventDataIN)
    {
        RaycastHit[] results = new RaycastHit[1];
        int layermask = (1 << 6) | (1 << 7);
        Physics.RaycastNonAlloc(origin: eventDataIN.position, direction: Vector3.down,  results );

        Debug.Log(results[0].collider.gameObject.name);

        Debug.Log("Hit " + results[0].collider.gameObject.name);

        var openInfoPanelButton = results[0].collider.gameObject.GetComponent<OpenInfoPanelButton<T_BluePrint>>();
        var blueprintContainer = results[0].collider.gameObject.GetComponent<Container<T_BluePrint>>();



        if (openInfoPanelButton is not null)
        {
            return openInfoPanelButton;
        }
        else if (blueprintContainer is not null)
        {
            return blueprintContainer;
        }
        return null;
    }*/


    protected IEnumerator ValidateClick()
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
}
