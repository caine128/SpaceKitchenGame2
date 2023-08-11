using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Floater
{
    private readonly List<IEnumerator> floatRoutines = new();
    private readonly Transform transform;
    private readonly MonoBehaviour monoBehaviour;


    public Floater(Transform transform, MonoBehaviour monoBehaviour)
    {
        this.transform = transform;
        this.monoBehaviour = monoBehaviour;
    }


    public void Begin()
    {
        /*var floatRoutineMmovement = transform.MoveRoutine1D(targetValue: new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z),
                                                        lerpDuration: 1f,
                                                        moveRoutineType: CRHelper.MoveRoutineType.Position,
                                                        coordinateFlags: CRHelper.CoordinateFlags.Y,
                                                        easeCurveType: TimeTickSystem.EaseCurveType.NudgeScale,
                                                        routineRecursionType: CRHelper.RoutineRecursionType.Continous);*/


        var floatRoutineRotation = transform.MoveRoutine1D(targetValue: new Vector3(transform.eulerAngles.x + 10, transform.eulerAngles.y, transform.eulerAngles.z),
                                                    lerpDuration: 1f,
                                                    moveRoutineType: CRHelper.MoveRoutineType.Rotation,
                                                    coordinateFlags: CRHelper.CoordinateFlags.X,
                                                    easeCurveType: TimeTickSystem.EaseCurveType.SmoothContinous,
                                                    routineRecursionType: CRHelper.RoutineRecursionType.ContinousWithInversiton);



        //floatRoutines.Add(floatRoutineMmovement);
        floatRoutines.Add(floatRoutineRotation);

        floatRoutines.ForEach(fr => monoBehaviour.StartCoroutine(fr));
    }

    public void End()
    {
        while (floatRoutines.Count > 0)
        {
            var lastItem = floatRoutines.Last();
            monoBehaviour.StopCoroutine(lastItem);
            floatRoutines.Remove(lastItem);
        }

        if(transform.eulerAngles.x != 0)
        {
            Debug.LogWarning("rectifiying the rotation ofthe subtransform");
            transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, transform.eulerAngles.z);
        }
    }
}
