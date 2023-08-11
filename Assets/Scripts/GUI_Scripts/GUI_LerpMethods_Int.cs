using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class GUI_LerpMethods_Int : GUI_LerpMethods
{
    private TextMeshProUGUI textMeshPro;
    protected override float LerpDuration => TimeTickSystem.NUMERIC_LERPDURATION;
    //private float lerpDuration;   // this can go to a main area higher in hierarchy
    private bool cr_Running = false;
    //private IEnumerator runningCoroutine = null;
    private Queue<IEnumerator> queue = new Queue<IEnumerator>();


    public override void PanelConfig()
    {
        //lerpDuration = TimeTickSystem.NUMERIC_LERPDURATION;
        textMeshPro = GetComponent<TextMeshProUGUI>();
    }

    public void UpdatTextCall(int initialValue, int finalValue, float lerpSpeedModifier, Func<int,string> toScreenFormatter)
    {
        if (runningCoroutine == null && cr_Running == false)
        {
            runningCoroutine = UpdateText(initialValue, finalValue, lerpSpeedModifier, toScreenFormatter);
            StartCoroutine(runningCoroutine);
        }
        else
        {
            queue.Enqueue(UpdateText(initialValue, finalValue, lerpSpeedModifier, toScreenFormatter));
        }
    }

    IEnumerator UpdateText(int initialValue, int finalValue, float lerpSpeedModifier, Func<int,string> toScreenFormatter)
    {
        cr_Running = true;

        float elapsedTime = 0f;
        
        while (elapsedTime < LerpDuration * lerpSpeedModifier)
        {
            var retVal = Mathf.RoundToInt(Mathf.Lerp(initialValue, finalValue, elapsedTime / (LerpDuration * lerpSpeedModifier)));
            textMeshPro.text = toScreenFormatter != null 
                                ? toScreenFormatter(retVal) 
                                : retVal.ToString(); //.ToString();
            elapsedTime += Time.deltaTime;

            yield return null;
        }
        textMeshPro.text = toScreenFormatter != null
                                ? toScreenFormatter(finalValue)
                                : finalValue.ToString();
        cr_Running = false;
        Dequeue();
    }

    private void Dequeue()
    {
        runningCoroutine = null;
        if (queue.Count > 0)
        {
            runningCoroutine = queue.Dequeue();
            StartCoroutine(runningCoroutine);
        }
    }

}
