using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level_Bars_Controller : HUDBarsController //MonoBehaviour ,IHUDBarsController
{
    //[SerializeField] RectTransform[] levelBars;
    //[SerializeField] RectTransform[] levelBarsExteriorAnchorPoints;
    //Vector2[] levelBarWidths;
    //[SerializeField] GUI_LerpMethods_Movement[] levelBarMovementLerpScripts;

    //private WaitForSeconds waitForSeconds = new WaitForSeconds(.05f);
    //private IEnumerator co;


    //public void PanelControllerConfig(float lerpDuration)
    //{
    //    GetBarWidthsArray();
    //    InitialPutBarsOut();
    //    PlaceBars(lerpDuration);
    //}

    //private void GetBarWidthsArray()
    //{
    //    levelBarWidths = new Vector2[levelBars.Length];
    //    for (int i = 0; i < levelBars.Length; i++)
    //    {
    //        levelBarWidths[i] = new Vector2(levelBars[i].sizeDelta.x, 0);
    //    }
    //}
    protected override void SetTargetPositions()
    {
        targetPositions = new Vector2[bars.Length];
        for (int i = 0; i < bars.Length; i++)
        {
            targetPositions[i] = new Vector2(bars[i].OriginalPos.x - ( MathF.Abs(bars[i].Rect.sizeDelta.x) + MathF.Abs(bars[i].Rect.offsetMin.x)), bars[i].OriginalPos.y);
        }
    }

    //protected sealed override void InitialPutBarsOut()
    //{
    //    //for (int i = 0; i < levelBars.Length; i++)
    //    //{
    //    //    levelBars[i].localPosition = levelBarsExteriorAnchorPoints[i].localPosition;
    //    //}
    //    for (int i = 0; i < bars.Length; i++)
    //    {
    //        bars[i].localPosition = targetPositions[i];
    //    }
    //}

    //public void DisplaceBars(float lerpDuration)
    //{
    //    if (co != null)
    //    {
    //        StopCoroutine(co);
    //    }

    //    co = (DisplaceBarsRoutine(lerpDuration));
    //    StartCoroutine(co);
    //}

    /*
    protected sealed override IEnumerator DisplaceBarsRoutine(Action followingAction)
    {
        //for (int i = levelBars.Length - 1; i > -1; i--)
        //{
        //    Vector2 diffference = levelBarsExteriorAnchorPoints[i].localPosition - levelBars[i].localPosition;

        //    levelBarMovementLerpScripts[i].InitialCallWithDiff(diffference, lerpDuration, moveDirection: 1);

        //    yield return waitForSeconds;
        //}
        for (int i = bars.Length - 1; i > -1; i--)
        {
            //Vector2 diffference = levelBarsExteriorAnchorPoints[i].localPosition - bars[i].localPosition;
            //barLerpScripts[i].InitialCallWithDiff(diffference, moveDirection: 1);

            bars[i].InitialCall(targetPositions[i]);

            //barLerpScripts[i].InitialCall();

            yield return TimeTickSystem.WaitForSeconds_HUDBarsMovement;
        }

        co = null;
    }*/

 
    //public void PlaceBars(float lerpDuration)
    //{
    //    if (co != null)
    //    {
    //        StopCoroutine(co);
    //    }

    //    co = (PlaceBarsRoutine(lerpDuration));
    //    StartCoroutine(co);
    //}

    //IEnumerator PlaceBarsRoutine(float lerpDuration)
    //{
    //    for (int i = 0; i < levelBars.Length; i++)
    //    {
    //        levelBarMovementLerpScripts[i].FinalCall(lerpDuration);

    //        yield return waitForSeconds;
    //    }

    //    co = null;
    //}

    //public override void ArrangeBarsInitial(float lerpDuration)
    //{
    //    throw new System.NotImplementedException();
    //}

    //public override void ArrangeBarsFinal(float lerpDuration)
    //{
    //    throw new System.NotImplementedException();
    //}
}
