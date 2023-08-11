using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Floater
{
    private readonly List<IEnumerator> floatRoutines = new();
    private readonly Transform _rootTransform;
    private readonly Transform _actualTransform;
    private readonly MonoBehaviour monoBehaviour;


    public Floater(Transform rootTransform, Transform actualTransform, MonoBehaviour monoBehaviour)
    {
        this._rootTransform = rootTransform;
        this._actualTransform = actualTransform;
        this.monoBehaviour = monoBehaviour;
    }


    public void Begin()
    {
        var floatRoutineMmovement = _rootTransform.MoveRoutine1D(targetValue: new Vector3(_rootTransform.position.x, _rootTransform.position.y + 1f, _rootTransform.position.z),
                                                        lerpDuration: 1f,
                                                        moveRoutineType: CRHelper.MoveRoutineType.Position,
                                                        coordinateFlags: CRHelper.CoordinateFlags.Y,
                                                        easeCurveType: TimeTickSystem.EaseCurveType.NudgeScale,
                                                        routineRecursionType: CRHelper.RoutineRecursionType.Continous);


        var floatRoutineRotation = _actualTransform.MoveRoutine1D(targetValue: new Vector3(_actualTransform.eulerAngles.x + 10, _actualTransform.eulerAngles.y, _actualTransform.eulerAngles.z),
                                                    lerpDuration: 1f,
                                                    moveRoutineType: CRHelper.MoveRoutineType.Rotation,
                                                    coordinateFlags: CRHelper.CoordinateFlags.X,
                                                    easeCurveType: TimeTickSystem.EaseCurveType.SmoothContinous,
                                                    routineRecursionType: CRHelper.RoutineRecursionType.ContinousWithInversiton);



        floatRoutines.Add(floatRoutineMmovement);
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

        if(_actualTransform.eulerAngles.x != 0)
        {
            Debug.LogWarning("rectifiying the rotation ofthe subtransform");
            _actualTransform.rotation = Quaternion.Euler(0, _actualTransform.eulerAngles.y, _actualTransform.eulerAngles.z);
        }
    }
}
