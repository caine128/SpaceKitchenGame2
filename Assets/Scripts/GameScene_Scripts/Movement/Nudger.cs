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

    public void Begin()
    {
        Debug.Log("nudger working");
        nudgeRoutine = _actualTransform.SingleTypeTransformRoutine(targetValue: new Vector3(1.1f, 1.1f, 1.1f),
                                                                   lerpDuration: .08f,
                                                                   moveRoutineType: CRHelper.MoveRoutineType.Scale,
                                                                   coordinateFlags: CRHelper.CoordinateFlags.X | CRHelper.CoordinateFlags.Y | CRHelper.CoordinateFlags.Z,
                                                                   easeCurveType: TimeTickSystem.EaseCurveType.NudgeScale,
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



