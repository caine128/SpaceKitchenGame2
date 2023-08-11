using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickValidator<T_Type> 
    where T_Type : MonoBehaviour
{
    private static readonly double validClickDuration = 250;

    private Vector2 initialClickPosition = default;
    private T_Type initialSelection = null;
    private DateTime initialPointerStimulationTime = default;

    public bool IsValidClick(T_Type clickedObject, Vector2 clickPosition, DateTime pointerUpTime)
        => initialSelection != null
           && initialSelection == clickedObject
           && pointerUpTime.Subtract(initialPointerStimulationTime).TotalMilliseconds <= validClickDuration
           && Vector2.Distance(initialClickPosition, clickPosition) <= PanelManager.MAXCLICKOFFSET;  

    public void StartValidating(T_Type clickedObject, PointerEventData eventData, DateTime pointerDownTime)
    {
        initialPointerStimulationTime = pointerDownTime;
        initialClickPosition = eventData.position;
        initialSelection = clickedObject;
    }
}
