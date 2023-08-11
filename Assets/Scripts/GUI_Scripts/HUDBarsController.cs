using System;
using System.Collections;
using UnityEngine;


public abstract class HUDBarsController : MonoBehaviour
{

    [SerializeField] protected GUI_LerpMethods_Movement[] bars;
    protected Vector2[] targetPositions;
    protected IEnumerator co;


    public virtual void PanelControllerConfig()
    {
        SetTargetPositions();
        InitialPutBarsOut();
        PlaceBars();
    }

    protected abstract void SetTargetPositions();

    protected void InitialPutBarsOut()
    {
        for (int i = 0; i < bars.Length; i++)
        {
            bars[i].Rect.anchoredPosition = targetPositions[i]; // barWidths[i] * moveDirection;
        }
    }

    public virtual void DisplaceBars( Action followingAction_IN = null)
    {
        if (co != null)
        {
            StopCoroutine(co);
        }

        co = (DisplaceBarsRoutine(followingAction_IN));
        StartCoroutine(co);
    }

    protected virtual IEnumerator DisplaceBarsRoutine(Action followingAction_IN)
    {
        for (int i = bars.Length - 1; i > -1; i--)
        {
            bars[i].InitialCall(targetPositions[i], followingAction: followingAction_IN);
            
            yield return TimeTickSystem.WaitForSeconds_HUDBarsMovement;
        }

        //followingAction_IN?.Invoke();
        co = null;
    }

    public void PlaceBars(Action followingAction_IN = null)
    {
        if (co != null)
        {
            StopCoroutine(co);
        }

        co = PlaceBarsRoutine(followingAction_IN);
        StartCoroutine(co);
    }

    protected virtual IEnumerator PlaceBarsRoutine(Action followingAction_IN)
    {
        for (int i = 0; i < bars.Length; i++)
        {
            bars[i].FinalCall(followingAction:followingAction_IN);

            yield return TimeTickSystem.WaitForSeconds_HUDBarsMovement;
        }

        co = null;
    }


}
