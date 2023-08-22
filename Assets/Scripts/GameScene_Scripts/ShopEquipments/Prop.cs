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
    public (int x, int z) PropSize { get; private set; } 
    public PropManager.Direction PlacedDirection { get; set; } = PropManager.Direction.Up;

    private Lifter lifter;
    private Floater floater;
    private Rotater rotater;
    private Nudger nudger;


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
                for (int x = AnchorGridPosition.x; x < AnchorGridPosition.x + PropSize.x; x++)
                {
                    for (int z = AnchorGridPosition.z; z < AnchorGridPosition.z + PropSize.z; z++)
                    {
                        yield return new(x: x, z: z);
                    }
                }
                break;
            case PropManager.Direction.Left:
                for (int x = AnchorGridPosition.x; x < AnchorGridPosition.x + PropSize.z; x++)
                {
                    for (int z = AnchorGridPosition.z - 1; z > AnchorGridPosition.z - PropSize.x - 1; z--)
                    {
                        yield return new(x: x, z: z);
                    }
                }
                break;
            case PropManager.Direction.Down:
                for (int x = AnchorGridPosition.x - 1; x > AnchorGridPosition.x - PropSize.x - 1; x--)
                {
                    for (int z = AnchorGridPosition.z - 1; z > AnchorGridPosition.z - PropSize.z - 1; z--)
                    {
                        yield return new(x: x, z: z);
                    }
                }
                break;
            case PropManager.Direction.Right:
                for (int x = AnchorGridPosition.x - 1; x > AnchorGridPosition.x - PropSize.z - 1; x--)
                {
                    for (int z = AnchorGridPosition.z; z < AnchorGridPosition.z + PropSize.x; z++)
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

        lifter = new(_rootTransform, this);
        floater = new(_rootTransform, _actualTransform, this);
        rotater = new(_rootTransform, this);
        nudger = new(_actualTransform, this);
    }

    public void Initialize(ShopUpgrade shopUpgradeBulePrint, bool subscribeToVerificationCallback, BuiltState builtState, Vector3 placementPos)//, Grid anchorGrid)
    {
        SubscribeToVerifivationCallback(subscribeToVerificationCallback);
        BuiltSate = builtState;
        ShopUpgradeBluePrint = shopUpgradeBulePrint;
        PropSize = shopUpgradeBulePrint.GetPropSize;
        _rootTransform.position = placementPos;

        Float();
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

    public void Rotate() => rotater.Begin(PropManager.QuaternionFromDirection(PlacedDirection));
    /*{
        if (rotateRoutine != null)
            StopCoroutine(rotateRoutine);

        rotateRoutine = CRHelper.RotateRoutine(transform: _rootTransform,
                                                  targetRotationQ: PropManager.QuaternionFromDirection(PlacedDirection),
                                                  lerpDuration: .4f,
                                                  lerpSpeedModifier: 1f,
                                                  followingActions: () => rotateRoutine = null );

        StartCoroutine(rotateRoutine);
    }*/
    public void Nudge() => nudger.Begin();
    /*{
       
        
        nudgeRoutine = _actualTransform.SingleTypeTransformRoutine(targetValue: new Vector3(1.1f, 1.1f, 1.1f),
                                                                lerpDuration: .08f,
                                                                moveRoutineType: CRHelper.MoveRoutineType.Scale,
                                                                coordinateFlags: CRHelper.CoordinateFlags.X | CRHelper.CoordinateFlags.Y | CRHelper.CoordinateFlags.Z,
                                                                easeCurveType: TimeTickSystem.EaseCurveType.NudgeScale,
                                                                routineRecursionType : CRHelper.RoutineRecursionType.RevertsToOriginal,
                                                                followingActions: () => nudgeRoutine = null);

        StartCoroutine(nudgeRoutine);
        

    }*/
    public void Float() => floater.Begin();
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

    public void LiftUp(bool queueRequest) => lifter.LiftUp(queueRequest, () => Float());
    /*{
 
        if (!queueRequest)
            while (moveRoutines.TryDequeue(out IEnumerator moveRoutine))
                StopCoroutine(moveRoutine);

        var thisMmoveRoutine = _rootTransform.SingleTypeTransformRoutine(targetValue: new Vector3(_rootTransform.position.x, 3f, _rootTransform.position.z),
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
    }*/

    public void LiftDown(Vector3 finalPosition, Quaternion finalRotation, params Action[] finalCallbacks)
        => lifter.LiftDown(finalPosition: finalPosition,
                          finalRotation: finalRotation,
                          initialCallback: () =>
                          {
                              rotater.End();
                              nudger.End();
                              floater.End();
                          },
                          finalCallbacks: finalCallbacks);
    /*{
       
        rotater.End();
        nudger.End();
        floater.End();

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

                                                                   //_actualTransform.localScale = Vector3.one;
                                                                }
                                                            });

        moveRoutines.Enqueue(thisMoveRoutine);
        
        if (moveRoutines.TryPeek(out IEnumerator nextMoveroutine) && nextMoveroutine == thisMoveRoutine)
            StartCoroutine(nextMoveroutine);
    }*/

    public void SubscribeToVerifivationCallback(bool shouldSubscribe)
    {
        if (shouldSubscribe)
            BuildingGrid.Instance.OnValidate += VerificationCallback;
        else
        {
            BuildingGrid.Instance.OnValidate -= VerificationCallback;
        }
    }

    public void VerificationCallback(bool isVerified)
    {
        _meshrenderer.material.color = isVerified ? _originalMeshColor : Color.red;
    }
}
