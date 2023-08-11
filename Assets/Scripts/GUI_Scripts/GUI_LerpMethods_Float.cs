using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUI_LerpMethods_Float : GUI_LerpMethods
{
    private Image progressBar;
    protected override float LerpDuration => TimeTickSystem.NUMERIC_LERPDURATION;
    //private float lerpDuration;   // this can go to a main area higher in hierarchy
    private bool cr_Running = false;
    //private IEnumerator runningCoroutine = null;
    private Queue<IEnumerator> queue = new Queue<IEnumerator>();


    public override void PanelConfig()
    {
        //lerpDuration = TimeTickSystem.NUMERIC_LERPDURATION;
        progressBar = GetComponent<Image>();
    }

    public void UpdateBarCall(float initialValue, float finalValue, float lerpSpeedModifier, bool queueRequest)
    {
        if (queueRequest && (runningCoroutine != null || cr_Running != false))
        {
            queue.Enqueue(UpdateBar(initialValue, finalValue, lerpSpeedModifier));
            //if (runningCoroutine == null && cr_Running == false)
            //{
            //    runningCoroutine = UpdateBar(initialValue, finalValue, lerpSpeedModifier);
            //    StartCoroutine(runningCoroutine);

            //}
            //else
            //{
            //    queue.Enqueue(UpdateBar(initialValue, finalValue, lerpSpeedModifier));
            //}
        }
        else
        {
            runningCoroutine = UpdateBar(initialValue, finalValue, lerpSpeedModifier);
            StartCoroutine(runningCoroutine);
        }
    }

    IEnumerator UpdateBar(float initialValue, float finalValue, float lerpSpeedModifier)
    {
        cr_Running = true;

        float elapsedTime = 0f;

        while (elapsedTime < LerpDuration * lerpSpeedModifier)
        {     
            progressBar.fillAmount = Mathf.Lerp (initialValue, finalValue, elapsedTime / (LerpDuration * lerpSpeedModifier));
            elapsedTime += Time.deltaTime;

            yield return null;
        }
        progressBar.fillAmount = finalValue;

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

    public void ClearQueue()
    {
        StopAllCoroutines();
        runningCoroutine = null;
        cr_Running = false;
        queue.Clear();
    }
}
