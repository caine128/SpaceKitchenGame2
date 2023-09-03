using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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

    public void Begin(Quaternion quaternion)//, Vector3? correctionOffset = null)
    {
        if (rotateRoutine != null)
        monoBehaviour.StopCoroutine(rotateRoutine);

        /*while (rotateRoutines.Count > 0)
        {
            var lastRoutine = rotateRoutines.Last();
            monoBehaviour.StopCoroutine(lastRoutine);
            rotateRoutines.Remove(lastRoutine);
        }*/

        rotateRoutine = _rootTransform.SingleTypeTransformRoutine(
                                       targetValue: quaternion.eulerAngles,
                                       lerpDuration: .4f,
                                       moveRoutineType: CRHelper.MoveRoutineType.Rotation,
                                       coordinateFlags: CRHelper.CoordinateFlags.X | CRHelper.CoordinateFlags.Y | CRHelper.CoordinateFlags.Z,
                                       easeCurveType : TimeTickSystem.EaseCurveType.Standard);


        monoBehaviour.StartCoroutine(rotateRoutine);

        //rotateRoutines.Add(thisRotateRoutine);


        /*if (correctionOffset.HasValue)
        {
            var thisFixPositioningRoutine = _rootTransform.SingleTypeTransformRoutine(
                               targetValue: _rootTransform.position + correctionOffset.Value,
                               lerpDuration: .4f,
                               moveRoutineType: CRHelper.MoveRoutineType.Position,
                               coordinateFlags: CRHelper.CoordinateFlags.X | CRHelper.CoordinateFlags.Z,
                               easeCurveType: TimeTickSystem.EaseCurveType.Standard);



            rotateRoutines.Add(thisFixPositioningRoutine);
        }*/

        //rotateRoutines.ForEach(rr=> monoBehaviour.StartCoroutine(rr));

    }

    public void End()
    {
        if(rotateRoutine != null)
        {
            monoBehaviour.StopCoroutine(rotateRoutine);
            rotateRoutine = null;
        }

        /*while (rotateRoutines.Count > 0)
        {
            var lastRoutine = rotateRoutines.Last();
            monoBehaviour.StopCoroutine(lastRoutine);
            rotateRoutines.Remove(lastRoutine);
        }*/
    }

}
