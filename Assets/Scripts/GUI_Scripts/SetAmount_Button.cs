using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SetAmount_Button : Button_Base
{
    private CounterObjectScript _counterObjectScript;
    [SerializeField] ButtonIteration.Type iterationType;

    public PressStates PressState => totalClickDuration switch
    {
        >= TimeTickSystem.LONGCLICKTHRESHOLD_1X and < TimeTickSystem.LONGCLICKTHRESHOLD_2X when partialClickDuration > TimeTickSystem.LONGCLICKFREQUENCY_1X => PressStates.Long_1X,
        >= TimeTickSystem.LONGCLICKTHRESHOLD_2X and < TimeTickSystem.LONGCLICKTHRESHOLD_3X when partialClickDuration > TimeTickSystem.LONGCLICKFREQUENCY_2X => PressStates.Long_2X,
        >= TimeTickSystem.LONGCLICKTHRESHOLD_3X when partialClickDuration > TimeTickSystem.LONGCLICKFREQUENCY_3X => PressStates.Long_3X,
        _ => isPointerDown ? PressStates.Normal : PressStates.None,
    };
      
    private bool isPointerDown = false;
    private float totalClickDuration = 0;
    private float partialClickDuration = 0;


    public enum PressStates
    {
        None,
        Normal,
        Long_1X,
        Long_2X,
        Long_3X,
    }

    private void Awake()
    {
        _counterObjectScript = GetComponentInParent<CounterObjectScript>(includeInactive: true);
    }

    private void Update()
    {
        if (isPointerDown && buttonImage_Adressable.enabled)
        {
            totalClickDuration += Time.deltaTime;
            partialClickDuration += Time.deltaTime;

            switch (PressState)
            {
                case PressStates.Long_1X:
                    partialClickDuration = 0;
                    _counterObjectScript.TryChangeAmount(iterationType);
                    Debug.Log("LongPress");
                    break;
                case PressStates.Long_2X:
                    partialClickDuration = 0;
                    _counterObjectScript.TryChangeAmount(iterationType);
                    Debug.Log("mediumSpeedThreshold");
                    break;
                case PressStates.Long_3X:
                    partialClickDuration = 0;
                    _counterObjectScript.TryChangeAmount(iterationType);
                    Debug.Log("fastSpeedThreshold");
                    break;
                default:
                    break;
            }
            /*if (totalClickDuration >= longClickThresHold && totalClickDuration < mediumSpeedThreshold && partialClickDuration > .2f)
            {
                partialClickDuration = 0;
                _counterObjectScript.TryChangeAmount(iterationType);
                Debug.Log("LongPress");
            }
            else if (totalClickDuration >= mediumSpeedThreshold && totalClickDuration < fastSpeedThreshold && partialClickDuration > .1f)
            {
                partialClickDuration = 0;
                _counterObjectScript.TryChangeAmount(iterationType);
                Debug.Log("mediumSpeedThreshold");
            }
            else if (totalClickDuration >= fastSpeedThreshold && partialClickDuration > .05f )
            {
                partialClickDuration = 0;
                _counterObjectScript.TryChangeAmount(iterationType);
                Debug.Log("fastSpeedThreshold");
            }*/
        }
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        totalClickDuration = 0;
        partialClickDuration = 0;
        isPointerDown = true;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if(PressState == PressStates.Normal)
        {
            Debug.LogWarning("now it is working onpointerup");
            _counterObjectScript.TryChangeAmount(iterationType);
        }

        /*if (isPointerDown && totalClickDuration < longClickThresHold || !isPointerDown)
        {
            Debug.LogWarning("now it is working onpointerup");
            _counterObjectScript.TryChangeAmount(iterationType);
        }*/

        isPointerDown = false;
        totalClickDuration = 0;
        partialClickDuration = 0;
    }

    public void TryChangeVisibility(bool isVisible)
    {
        if (buttonImage_Adressable.enabled != isVisible)
        {
            buttonImage_Adressable.enabled = isVisible;
        }
    }


}
