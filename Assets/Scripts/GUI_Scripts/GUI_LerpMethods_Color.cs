using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class GUI_LerpMethods_Color : GUI_LerpMethods
{
    private Image image;
    //public bool IsOriginalColor => image.color == _originalColor;
    
    private Color _originalColor = new Color(0f, 0f, 0f, 0f);
    //private IEnumerator runningCoroutine;


    public override void PanelConfig()
    {
        image = this.GetComponent<Image>();
        _originalColor = image.color;
    }

    public void ColorLerpInitialCall(Color lerpedColor, float lerpSpeedModifier = 1, Action adressableAction = null)
    {
        if (image.raycastTarget != true) image.raycastTarget = true;
        if (runningCoroutine != null)
        {
            StopCoroutine(runningCoroutine);
        }
        
        runningCoroutine = ColorLerpInitial(lerpedColor, lerpSpeedModifier, adressableAction);
        StartCoroutine(runningCoroutine);
    }

    private IEnumerator ColorLerpInitial(Color lerpedColor, float lerpSpeedModifier, Action adressableAction)
    {
        float elapsedTime = 0f;
        Color currentColor = image.color;

        while (elapsedTime < LerpDuration * lerpSpeedModifier)
        {
            image.color = Color.Lerp(currentColor, 
                                     adressableAction is not null 
                                            ? Color.black
                                            : lerpedColor, 
                                     elapsedTime / (LerpDuration * lerpSpeedModifier));
            elapsedTime += Time.deltaTime;

            yield return null;
        }



        if(adressableAction is not null)
        {
            image.color = currentColor = Color.black;
            elapsedTime = 0f;
            adressableAction?.Invoke();

            yield return TimeTickSystem.WaitForSeconds_EighthSec;

            while (elapsedTime < LerpDuration * lerpSpeedModifier)
            {
                image.color = Color.Lerp(currentColor,
                                         lerpedColor,
                                         elapsedTime / (LerpDuration * lerpSpeedModifier));
                elapsedTime += Time.deltaTime;

                yield return null;
            }

        }

        image.color = lerpedColor;

        runningCoroutine = null;
    }

    public void ColorLerpFinalCall(bool disableObject=false, float lerpSpeedModifier = 1, Action adressableAction = null)
    {
        if (image.raycastTarget != false) image.raycastTarget = false;
        if (runningCoroutine != null)
        {
            StopCoroutine(runningCoroutine);
        }

        runningCoroutine = ColorLerpFinal(disableObject,lerpSpeedModifier, adressableAction);
        StartCoroutine(runningCoroutine);
    }


    private IEnumerator ColorLerpFinal(bool disableObject, float lerpSpeedModifier, Action adressableAction)
    {
        float elapsedTime = 0f;
        Color currentColor = image.color;

        while (elapsedTime < LerpDuration * lerpSpeedModifier)
        {
            image.color = Color.Lerp(currentColor,
                                     adressableAction is not null
                                            ? Color.black
                                            : _originalColor,
                                     elapsedTime / (LerpDuration * lerpSpeedModifier)); 
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        if (adressableAction is not null)
        {
            image.color = currentColor = Color.black;
            elapsedTime = 0f;
            adressableAction?.Invoke();

            yield return TimeTickSystem.WaitForSeconds_EighthSec;

            while (elapsedTime < LerpDuration * lerpSpeedModifier)
            {
                image.color = Color.Lerp(currentColor,
                                         _originalColor,
                                         elapsedTime / (LerpDuration * lerpSpeedModifier));
                elapsedTime += Time.deltaTime;

                yield return null;
            }

        }


        image.color = _originalColor;
        if (disableObject)
        {
            this.gameObject.SetActive(false);
        }

        runningCoroutine = null;
    }
}
