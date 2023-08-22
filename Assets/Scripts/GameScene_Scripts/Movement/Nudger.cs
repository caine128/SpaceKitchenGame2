using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nudger 
{
    private IEnumerator nudgeRoutine;
    private readonly Transform _actualTransform;
    private readonly MonoBehaviour monoBehaviour;

    public Nudger(Transform actualTransform, MonoBehaviour monoBehaviour)
    {
        this._actualTransform = actualTransform;
        this.monoBehaviour = monoBehaviour;
    }

    public void Begin(float scaleMax, float duration, TimeTickSystem.EaseCurveType easeCurveType)
    {
        if (nudgeRoutine != null)
            return;

        Debug.Log("nudger working");
        nudgeRoutine = _actualTransform.SingleTypeTransformRoutine(targetValue: new Vector3(scaleMax, scaleMax, scaleMax),
                                                                   lerpDuration: duration,
                                                                   moveRoutineType: CRHelper.MoveRoutineType.Scale,
                                                                   coordinateFlags: CRHelper.CoordinateFlags.X | CRHelper.CoordinateFlags.Y | CRHelper.CoordinateFlags.Z,
                                                                   easeCurveType: easeCurveType,
                                                                   routineRecursionType: CRHelper.RoutineRecursionType.RevertsToOriginal,
                                                                   followingActions: () => nudgeRoutine = null);

        monoBehaviour.StartCoroutine(nudgeRoutine);
    }

    public void End()
    {
        if(nudgeRoutine != null)
        {
            monoBehaviour.StopCoroutine(nudgeRoutine);
            nudgeRoutine = null;
        }

        _actualTransform.localScale = Vector3.one; 
    }
}



