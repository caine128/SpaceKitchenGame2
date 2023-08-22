using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

public class Lifter 
{
    private readonly ConcurrentQueue<IEnumerator> moveRoutines = new();
    private readonly Transform _rootTransform;
    private readonly MonoBehaviour monoBehaviour;


    public Lifter(Transform rootTransform, MonoBehaviour monoBehaviour)
    {
        this._rootTransform = rootTransform; ;
        this.monoBehaviour= monoBehaviour; ;
    }

    public void LiftUp(bool queueRequest, params Action[] followingActions)
    {
        if (!queueRequest)
            while(moveRoutines.TryDequeue(out IEnumerator moveRoutine))
                monoBehaviour.StopCoroutine(moveRoutine);

        var thisMoveRoutine = _rootTransform.SingleTypeTransformRoutine(
                                             targetValue: new Vector3(_rootTransform.position.x, 2.5f, _rootTransform.position.z),
                                             lerpDuration: .25f,
                                             moveRoutineType: CRHelper.MoveRoutineType.Position,
                                             coordinateFlags: CRHelper.CoordinateFlags.Y,
                                             easeCurveType: TimeTickSystem.EaseCurveType.PropUp,
                                             followingActions: 
                                             () =>
                                             {
                                                 moveRoutines.TryDequeue(out var runningRoutine);
                                            
                                                 if (moveRoutines.TryPeek(out var nextRoutine))
                                                        monoBehaviour.StartCoroutine(nextRoutine);
                                                 else
                                                 {
                                                     foreach (var followingAction in followingActions)
                                                     {
                                                         followingAction?.Invoke();
                                                     }
                                                 }
                                             });

        moveRoutines.Enqueue(thisMoveRoutine);

        if (moveRoutines.TryPeek(out IEnumerator nextMoveRoutine) && nextMoveRoutine == thisMoveRoutine)
            monoBehaviour.StartCoroutine(nextMoveRoutine);

    }


    public void LiftDown(Vector3 finalPosition, Quaternion finalRotation, Action initialCallback , params Action[] finalCallbacks)
    {
        initialCallback?.Invoke();

        while (moveRoutines.TryDequeue(out IEnumerator moveRoutine))
            monoBehaviour.StopCoroutine(moveRoutine);

        var thisMoveRoutine = _rootTransform.MoveRoutine3D(targetPos: finalPosition,
                                                            lerpDuration: .25f,
                                                            easeCurveType: TimeTickSystem.EaseCurveType.PropDown,
                                                            targetRotationQ: finalRotation,
                                                            followingActions: () =>
                                                            {
                                                                moveRoutines.TryDequeue(out var runningRoutine);

                                                                if (moveRoutines.TryPeek(out var nextRoutine))
                                                                     monoBehaviour.StartCoroutine(nextRoutine);
                                                                else
                                                                {
                                                                    foreach (var finalCallback in finalCallbacks)
                                                                        finalCallback?.Invoke();
                                                                }
                                                            });

        moveRoutines.Enqueue(thisMoveRoutine);

        if (moveRoutines.TryPeek(out IEnumerator nextMoveRoutine) && nextMoveRoutine == thisMoveRoutine)
            monoBehaviour.StartCoroutine(nextMoveRoutine);
    }
}
