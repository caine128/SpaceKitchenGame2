using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotater 
{
    private IEnumerator rotateRoutine;
    private readonly Transform _rootTransform;
    private readonly MonoBehaviour monoBehaviour;

    public Rotater(Transform rootTransform, MonoBehaviour monoBehaviour)
    {
        this._rootTransform = rootTransform;
        this.monoBehaviour = monoBehaviour;
    }

    public void Begin(Quaternion quaternion)
    {
        if (rotateRoutine != null)
            monoBehaviour.StopCoroutine(rotateRoutine);

        rotateRoutine = _rootTransform.SingleTypeTransformRoutine(
                                       targetValue: quaternion.eulerAngles,
                                       lerpDuration: .4f,
                                       moveRoutineType: CRHelper.MoveRoutineType.Rotation,
                                       coordinateFlags: CRHelper.CoordinateFlags.X | CRHelper.CoordinateFlags.Y | CRHelper.CoordinateFlags.Z,
                                       easeCurveType : TimeTickSystem.EaseCurveType.Standard,
                                       followingActions: () => rotateRoutine = null);

        monoBehaviour.StartCoroutine(rotateRoutine);
    }

    public void End()
    {
        if(rotateRoutine != null)
        {
            monoBehaviour.StopCoroutine(rotateRoutine);
            rotateRoutine = null;
        }
    }

}
