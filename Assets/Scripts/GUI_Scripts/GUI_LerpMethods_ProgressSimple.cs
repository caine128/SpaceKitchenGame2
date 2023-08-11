using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUI_LerpMethods_ProgressSimple : MonoBehaviour // GUI_LerpMethods
{
    private Image progressBar;
    public RectTransform RT { get; private set; }
    public float LerpDuration => TimeTickSystem.NUMERIC_LERPDURATION;

    /*public override void PanelConfig()
    {
        progressBar = GetComponent<Image>();
        RT = GetComponent<RectTransform>();
    }*/

    private void Awake()
    {
        progressBar = GetComponent<Image>();
        RT = GetComponent<RectTransform>();
    }

    public void SetProgressBarTo(float fillamount_IN)
    {
        progressBar.fillAmount = fillamount_IN;
    }



    /*public void UpdateBarAndScrollCall(float initialValue, float finalValue, float lerpSpeedModifier)
    {
        if (runningCoroutine is not null)
        {
            StopCoroutine(runningCoroutine);
        }

        runningCoroutine = UpdateBarAndScroll(initialValue, finalValue, lerpSpeedModifier);
        StartCoroutine(runningCoroutine);

    }

    IEnumerator UpdateBarAndScroll(float initialValue, float finalValue, float lerpSpeedModifier)
    {
        float elapsedTime = 0f;
        while (elapsedTime < LerpDuration * lerpSpeedModifier)
        {
            progressBar.fillAmount = Mathf.Lerp(initialValue, finalValue, elapsedTime / (LerpDuration * lerpSpeedModifier));          
            elapsedTime += Time.deltaTime;

            yield return null;
        }
        progressBar.fillAmount = finalValue;

        runningCoroutine = null;
    }

    public void ResetProgressBar(float fillAmount_IN)
    {
        if (runningCoroutine is not null)
        {
            StopCoroutine(runningCoroutine);
            runningCoroutine = null;
        }
        progressBar.fillAmount = fillAmount_IN;
    } */
}
