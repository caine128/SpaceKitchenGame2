using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class Prop : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IVerificationCallbackReceiver
{
    public ShopUpgrade ShopUpgradeBluePrint { get; private set; }

    public GridPosition AnchorGridPosition { get => BuildingGrid.Instance.GridSystem.FromWorldPosToGridPos(transform.position); }

    public BuiltState BuiltSate { get; set; } = BuiltState.NotFixed;
    public (int x, int z) Size = (4, 2);
    public PropManager.Direction PlacedDirection { get; set; } = PropManager.Direction.Up;

    private readonly ConcurrentQueue<IEnumerator> moveRoutines = new();
    private IEnumerator rotateRoutine;
    private IEnumerator nudgeRoutine;
    //private readonly List<IEnumerator> floatRoutines = new();
    private Floater floater;


    private MeshCollider[] _meshColliders;
    private MeshRenderer _meshrenderer;
    private Color _originalMeshColor;

    public new Transform transform { get => _rootTransform; }
    [SerializeField] private Transform _rootTransform;
    private Transform _actualTransform;
    public enum BuiltState
    {
        NotFixed = 0,
        Fixed = 1,
        Fixing = 2,
    }

    public IEnumerable<GridPosition> GetCurrentGridPositions()
    {
        switch (PlacedDirection)
        {
            case PropManager.Direction.Up:
                for (int x = AnchorGridPosition.x; x < AnchorGridPosition.x + Size.x; x++)
                {
                    for (int z = AnchorGridPosition.z; z < AnchorGridPosition.z + Size.z; z++)
                    {
                        yield return new(x: x, z: z);
                    }
                }
                break;
            case PropManager.Direction.Left:
                for (int x = AnchorGridPosition.x; x < AnchorGridPosition.x + Size.z; x++)
                {
                    for (int z = AnchorGridPosition.z - 1; z > AnchorGridPosition.z - Size.x - 1; z--)
                    {
                        yield return new(x: x, z: z);
                    }
                }
                break;
            case PropManager.Direction.Down:
                for (int x = AnchorGridPosition.x - 1; x > AnchorGridPosition.x - Size.x - 1; x--)
                {
                    for (int z = AnchorGridPosition.z - 1; z > AnchorGridPosition.z - Size.z - 1; z--)
                    {
                        yield return new(x: x, z: z);
                    }
                }
                break;
            case PropManager.Direction.Right:
                for (int x = AnchorGridPosition.x - 1; x > AnchorGridPosition.x - Size.z - 1; x--)
                {
                    for (int z = AnchorGridPosition.z; z < AnchorGridPosition.z + Size.x; z++)
                    {
                        yield return new(x: x, z: z);
                    }
                }
                break;
        }

    }

    private void Awake()
    {
        _meshColliders = GetComponents<MeshCollider>();
        _meshrenderer = GetComponentInChildren<MeshRenderer>();
        _originalMeshColor = _meshrenderer.material.color;
        _actualTransform = base.transform;

        floater = new(_actualTransform, this); 
    }




    public void OnPointerDown(PointerEventData eventData)
    {
        if (Physics.Raycast(
            ray: GameManager.Instance.CameraMain.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, maxDistance: Mathf.Infinity, layerMask: CollisionLayers.GROUND_LAYER_MASK))
        {
            PropManager.SelectProp(this, eventData, raycastToGroundPosition: hit.point);
        }
    }

    public void ActivateColliers(bool shouldActivate)
    {
        foreach (var meshCollider in _meshColliders)
        {
            meshCollider.enabled = shouldActivate;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        PropManager.ReleaseProp(this);

        foreach (var meshCollider in _meshColliders)
        {
            meshCollider.enabled = true;
        }
    }

    public void AnimatePropRotate()
    {
        if (rotateRoutine != null)
            StopCoroutine(rotateRoutine);

        rotateRoutine = CRHelper.RotateRoutine(transform: _rootTransform,
                                                  targetRotationQ: PropManager.QuaternionFromDirection(PlacedDirection),
                                                  lerpDuration: .4f,
                                                  lerpSpeedModifier: 1f,
                                                  followingActions: () => rotateRoutine = null );

        StartCoroutine(rotateRoutine);
    }


    public void AnimatePropUp(bool queueRequest)
    {
        /*if (!queueRequest && MoveRoutines.TryDequeue(out IEnumerator moveRoutine))
        {
            StopCoroutine(moveRoutine);
        }*/
        if (!queueRequest)
            while (moveRoutines.TryDequeue(out IEnumerator moveRoutine))
                StopCoroutine(moveRoutine);

        var thisMmoveRoutine = _rootTransform.MoveRoutine1D(targetValue: new Vector3(_rootTransform.position.x, 3f, _rootTransform.position.z),
                                                            lerpDuration: .25f,
                                                            moveRoutineType: CRHelper.MoveRoutineType.Position,
                                                            coordinateFlags: CRHelper.CoordinateFlags.Y,
                                                            easeCurveType: TimeTickSystem.EaseCurveType.PropUp,
                                                            followingActions: () =>
                                                            {
                                                                moveRoutines.TryDequeue(out var runningRoutine);
                                                                
                                                                if (moveRoutines.TryPeek(out var nextRoutine))
                                                                    StartCoroutine(nextRoutine);
                                                                else
                                                                {
                                                                    Float();
                                                                }
                                                            });
        moveRoutines.Enqueue(thisMmoveRoutine);
        if (moveRoutines.TryPeek(out IEnumerator nextMoveroutine) && nextMoveroutine == thisMmoveRoutine)
            StartCoroutine(nextMoveroutine);
    }

    public void AnimatePropDown(Vector3 finalPosition, Quaternion finalRotation, params Action[] finalCallbacks)
    {
        /*if (!queueRequest && MoveRoutines.TryDequeue(out IEnumerator moveRoutine))
        {
            StopCoroutine(moveRoutine);
        }*/
        
        if (rotateRoutine != null)
        {
            StopCoroutine(rotateRoutine);
            rotateRoutine = null;
        }

        if(nudgeRoutine != null)
        {
            StopCoroutine(nudgeRoutine);
            nudgeRoutine = null;
        }

        floater.End();

        /*while(floatRoutines.Count > 0)
        {
            var lastItem = floatRoutines.Last();
            StopCoroutine(lastItem);
            floatRoutines.Remove(lastItem);
        }*/



        while (moveRoutines.TryDequeue(out IEnumerator moveRoutine))
            StopCoroutine(moveRoutine);


        var thisMoveRoutine = _rootTransform.MoveRoutine3D( targetPos: finalPosition,
                                                            lerpDuration: .25f,
                                                            easeCurveType: TimeTickSystem.EaseCurveType.PropDown,
                                                            targetRotationQ: finalRotation,
                                                            followingActions: () =>
                                                            {
                                                                moveRoutines.TryDequeue(out var runningRoutine);
                                                                
                                                                if (moveRoutines.TryPeek(out var nextRoutine))
                                                                    StartCoroutine(nextRoutine);
                                                                else
                                                                {
                                                                    foreach (var finalCallback in finalCallbacks)
                                                                        finalCallback?.Invoke();

                                                                   _actualTransform.localScale = Vector3.one;
                                                                }
                                                            });

        moveRoutines.Enqueue(thisMoveRoutine);
        
        /*MoveRoutines.Enqueue(CRHelper.MoveRoutine1D(transform: this.transform,
                                                    targetPos: 0.05f,
                                                    lerpDuration: .5f,
                                                    coorinateflags: CRHelper.CoodrinateFlags.Y,
                                                    lerpSpeedModifier: 1,
                                                    () =>
                                                    {
                                                        MoveRoutines.TryDequeue(out var runningRoutine);
                                                        if (MoveRoutines.TryDequeue(out var nextRoutine))
                                                            StartCoroutine(nextRoutine);
                                                        //Debug.Log("count of routines : " + MoveRoutines.Count);
                                                    }));*/
        if (moveRoutines.TryPeek(out IEnumerator nextMoveroutine) && nextMoveroutine == thisMoveRoutine)
            StartCoroutine(nextMoveroutine);
    }

    public void AnimatePropNudge()
    {
        /*var thisMoveRoutine1 = _rootTransform.MoveRoutine1D(targetValue: new Vector3(_rootTransform.position.x, 2.7f,_rootTransform.position.z),
                                                            lerpDuration: .2f,
                                                            moveRoutineType: CRHelper.MoveRoutineType.Position,
                                                            coordinateFlags: CRHelper.CoordinateFlags.Y,                                                           
                                                            followingActions: () =>
                                                            {
                                                                MoveRoutines.TryDequeue(out var runningRoutine);
                                                                if (MoveRoutines.TryDequeue(out var nextRoutine))
                                                                    StartCoroutine(nextRoutine);
                                                            });*/
        
        nudgeRoutine = _actualTransform.MoveRoutine1D(targetValue: new Vector3(1.1f, 1.1f, 1.1f),
                                                                lerpDuration: .08f,
                                                                moveRoutineType: CRHelper.MoveRoutineType.Scale,
                                                                coordinateFlags: CRHelper.CoordinateFlags.X | CRHelper.CoordinateFlags.Y | CRHelper.CoordinateFlags.Z,
                                                                easeCurveType: TimeTickSystem.EaseCurveType.NudgeScale,
                                                                routineRecursionType : CRHelper.RoutineRecursionType.RevertsToOriginal,
                                                                followingActions: () => nudgeRoutine = null);

        StartCoroutine(nudgeRoutine);
        /*var thisNudgeRoutine1 = _actualTransform.MoveRoutine1D(targetValue: new Vector3(1.1f, 1.1f, 1.1f),
                                                            lerpDuration: .08f,
                                                            moveRoutineType: CRHelper.MoveRoutineType.Scale,
                                                            coordinateFlags: CRHelper.CoordinateFlags.X |CRHelper.CoordinateFlags.Y | CRHelper.CoordinateFlags.Z,
                                                            easeCurveType: TimeTickSystem.EaseCurveType.PropUp,
                                                            followingActions: () =>
                                                            {
                                                                _nudgeRoutines.TryDequeue(out var runningRoutine);
                                                                
                                                                if (_nudgeRoutines.TryPeek(out var nextRoutine))
                                                                    StartCoroutine(nextRoutine);
                                                            });

        

        var thisNudgeRoutine2 = _actualTransform.MoveRoutine1D(targetValue: _rootTransform.localScale,
                                                            lerpDuration: .08f,
                                                            moveRoutineType: CRHelper.MoveRoutineType.Scale,
                                                            coordinateFlags: CRHelper.CoordinateFlags.X | CRHelper.CoordinateFlags.Y | CRHelper.CoordinateFlags.Z,
                                                            easeCurveType: TimeTickSystem.EaseCurveType.PropUp,
                                                            followingActions: () =>
                                                            {
                                                                _nudgeRoutines.TryDequeue(out var runningRoutine);
                                                                
                                                                if (_nudgeRoutines.TryPeek(out var nextRoutine))
                                                                    StartCoroutine(nextRoutine);
                                                            });

        _nudgeRoutines.Enqueue(thisNudgeRoutine1);
        _nudgeRoutines.Enqueue(thisNudgeRoutine2);

        _nudgeRoutines.Enqueue(exampleNudgeoutine);

        if (_nudgeRoutines.TryPeek(out IEnumerator nextNudgeRoutine) && nextNudgeRoutine == exampleNudgeoutine) ;// thisNudgeRoutine1)
            StartCoroutine(nextNudgeRoutine);*/

    }

    public void Subscribe(bool shouldSubscribe)
    {
        if (shouldSubscribe)
            BuildingGrid.Instance.OnValidate += VerificationCallback;
        else
        {
            BuildingGrid.Instance.OnValidate -= VerificationCallback;
        }
    }

    private void Float() => floater.Begin();
    /*{

        var floatRoutineMmovement = _rootTransform.MoveRoutine1D(targetValue: new Vector3(_rootTransform.position.x, _rootTransform.position.y + 1f, _rootTransform.position.z),
                                                        lerpDuration: 1f,
                                                        moveRoutineType: CRHelper.MoveRoutineType.Position,
                                                        coordinateFlags: CRHelper.CoordinateFlags.Y,
                                                        easeCurveType: TimeTickSystem.EaseCurveType.NudgeScale,
                                                        routineRecursionType : CRHelper.RoutineRecursionType.Continous);


        var floatRoutineRotation = _rootTransform.MoveRoutine1D(targetValue: new Vector3(_rootTransform.eulerAngles.x +5, _rootTransform.eulerAngles.y, _rootTransform.eulerAngles.z),
                                                    lerpDuration: 1f,
                                                    moveRoutineType: CRHelper.MoveRoutineType.Rotation,
                                                    coordinateFlags: CRHelper.CoordinateFlags.X,
                                                    easeCurveType: TimeTickSystem.EaseCurveType.SmoothContinous,
                                                    routineRecursionType: CRHelper.RoutineRecursionType.Continous);



        floatRoutines.Add(floatRoutineMmovement);
        floatRoutines.Add(floatRoutineRotation);

        floatRoutines.ForEach(fr => StartCoroutine(fr));


        //var pingpongedPosDifference = -.1f + Mathf.PingPong(Time.time * .1f, .2f);
        //_rootTransform.position = new Vector3(_rootTransform.position.x, 3f - pingpongedPosDifference, _rootTransform.position.z);

        //var pinpongedAngle = -1f + Mathf.PingPong(Time.time * 2, 2f);
        //_actualTransform.rotation = Quaternion.Euler(pinpongedAngle, _actualTransform.eulerAngles.y, _actualTransform.eulerAngles.z);
    }*/


    public void VerificationCallback(bool isVerified)
    {
        _meshrenderer.material.color = isVerified ? _originalMeshColor : Color.red;
    }
}
