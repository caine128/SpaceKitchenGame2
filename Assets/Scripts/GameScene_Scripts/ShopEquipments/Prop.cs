using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class Prop : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IVerificationCallbackReceiver
{
    public ShopUpgrade ShopUpgradeBluePrint {get;private set;}
    public bool IsPurchased { get; private set; } = false;

    public GridPosition AnchorGridPosition { get => BuildingGrid.Instance.GridSystem.FromWorldPosToGridPos(transform.position); }  
    public bool IsCentrallypositionedToGrid { get => isCentrallyPositionedToGrid; }
    private bool isCentrallyPositionedToGrid;
    private bool isTransformBalanced;

    public BuiltState Built_State { get; private set; } = BuiltState.NotFixed; 
    public bool HasValidPosition { get; private set; } = false;
    public (int x, int z) PropSize 
    {
        get    // TODO : if there is no problem of ShopUpgradeBluePrint is accessed before it is set , then remove if else
        {
            if (ShopUpgradeBluePrint != null)
                return ShopUpgradeBluePrint.GetPropSize;
            else    
            {
                Debug.LogError("ShopUpgradeBluePrint is accessed before it is set ");
                return default;
            }          
        }  
    }
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
        (int widthOffset, int heightOffset) = isTransformBalanced
                                                    ? (Mathf.FloorToInt((float)PropSize.x / 2f), Mathf.FloorToInt((float)PropSize.z / 2f))
                                                    : PropSize.x > PropSize.z
                                                            ? (Mathf.FloorToInt(((float)PropSize.x -1) / 2f), Mathf.FloorToInt((float)PropSize.z / 2f))
                                                            : (Mathf.FloorToInt((float)PropSize.x / 2f) , Mathf.FloorToInt(((float)PropSize.z -1) / 2f));

        switch (PlacedDirection)
        {
            case PropManager.Direction.Up:
                for (int x = AnchorGridPosition.x - widthOffset; x < AnchorGridPosition.x -widthOffset + PropSize.x; x++)
                {
                    for (int z = AnchorGridPosition.z - heightOffset; z < AnchorGridPosition.z - heightOffset + PropSize.z; z++)
                    {
                        yield return new(x: x, z: z);
                    }
                }
                
                /*for (int x = AnchorGridPosition.x; x < AnchorGridPosition.x + PropSize.x; x++)
                {
                    for (int z = AnchorGridPosition.z; z < AnchorGridPosition.z + PropSize.z; z++)
                    {
                        yield return new(x: x, z: z);
                    }
                }*/
                break;
            case PropManager.Direction.Left:
                for (int x = AnchorGridPosition.x - heightOffset; x < AnchorGridPosition.x - heightOffset + PropSize.z; x++)
                {
                    for (int z = AnchorGridPosition.z + widthOffset; z > AnchorGridPosition.z + widthOffset - PropSize.x; z--)
                    {
                        yield return new(x: x, z: z);
                    }
                }
                /*for (int x = AnchorGridPosition.x; x < AnchorGridPosition.x + PropSize.z; x++)
                {
                    for (int z = AnchorGridPosition.z - 1; z > AnchorGridPosition.z - PropSize.x - 1; z--)
                    {
                        yield return new(x: x, z: z);
                    }
                }*/
                break;
            case PropManager.Direction.Down:
                for (int x = AnchorGridPosition.x + widthOffset; x > AnchorGridPosition.x + widthOffset - PropSize.x; x--)
                {
                    for (int z = AnchorGridPosition.z - heightOffset; z < AnchorGridPosition.z - heightOffset + PropSize.z; z++)
                    {
                        yield return new(x: x, z: z);
                    }
                }
                /*for (int x = AnchorGridPosition.x - 1; x > AnchorGridPosition.x - PropSize.x - 1; x--)
                {
                    for (int z = AnchorGridPosition.z - 1; z > AnchorGridPosition.z - PropSize.z - 1; z--)
                    {
                        yield return new(x: x, z: z);
                    }
                }*/
                break;
            case PropManager.Direction.Right:
                for (int x = AnchorGridPosition.x + heightOffset; x > AnchorGridPosition.x + heightOffset - PropSize.z; x--)
                {
                    for (int z = AnchorGridPosition.z - widthOffset; z < AnchorGridPosition.z - widthOffset + PropSize.x; z++)
                    {
                        yield return new(x: x, z: z);
                    }
                }
                /*for (int x = AnchorGridPosition.x - 1; x > AnchorGridPosition.x - PropSize.z - 1; x--)
                {
                    for (int z = AnchorGridPosition.z; z < AnchorGridPosition.z + PropSize.x; z++)
                    {
                        yield return new(x: x, z: z);
                    }
                }*/
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

    public void RegisterClonedBluePrint(ShopUpgrade clonedAndPurchasedShopUpgrade)
    {
        if (IsPurchased)
        {
            Debug.LogError("Cannot change the purchase status after it's once set");
            return;
        }

        ShopUpgradeBluePrint = clonedAndPurchasedShopUpgrade;
        IsPurchased = true;
    }
    private void OnDestroy()
    {
        SubscribeToVerifivationCallback(false);               
        Destroy(_rootTransform.gameObject);
    }
    public void Initialize(ShopUpgrade shopUpgradeBulePrint, 
                           bool subscribeToVerificationCallback, 
                           BuiltState builtState, 
                           Grid anchorGrid, Action<Prop> onInitialized)
    {
        SubscribeToVerifivationCallback(subscribeToVerificationCallback);

        ActivateColliders(false);

        Built_State = builtState;
        ShopUpgradeBluePrint = shopUpgradeBulePrint;
      
        SetPositionRelativeToPropSize();

        _rootTransform.position = isCentrallyPositionedToGrid
                                 ? new(anchorGrid.GridWorldPosition.x + BuildingGrid.cellSize / 2f, 2.5f, anchorGrid.GridWorldPosition.z + BuildingGrid.cellSize / 2f)
                                 : new(anchorGrid.GridWorldPosition.x, 2.5f, anchorGrid.GridWorldPosition.z);

        onInitialized(this);
        ActivateColliders(true);


        if (HasValidPosition)
        {
            LiftDown(finalPlacementGrid: anchorGrid, Quaternion.identity);
        }
        else
        {
            Float();
        }
    }

    private void SetPositionRelativeToPropSize()
    {
        //bool isTransformBalanced = Mathf.Abs(PropSize.x - PropSize.z) % 2f == 0;
        isTransformBalanced =  Mathf.Abs(PropSize.x - PropSize.z) % 2f == 0;
        _actualTransform.position = isTransformBalanced
                                        ? Vector3.zero
                                        : new Vector3(x: PropSize.x > PropSize.z ? .5f : 0,
                                                       y: 0,
                                                       z: PropSize.x < PropSize.z ? -.5f : 0);

        isCentrallyPositionedToGrid = isTransformBalanced
                                        ? PropSize.x%2 == 0 
                                                ? false
                                                : true
                                        : Mathf.Min(PropSize.x, PropSize.z) % 2 == 0
                                                ? false
                                                :true;     
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (Physics.Raycast(ray: GameManager.Instance.CameraMain.ScreenPointToRay(Input.mousePosition), out RaycastHit hit,
                            maxDistance: Mathf.Infinity,
                            layerMask: CollisionLayers.GROUND_LAYER_MASK))
        {

            PropManager.SelectProp(this, new(pointerEventData: eventData, raycastToGroundPosition: hit.point));      //eventData, raycastToGroundPosition: hit.point); 
        }
    }

    public void ActivateColliders(bool shouldActivate)
    {
        foreach (var meshCollider in _meshColliders)
        {
            meshCollider.enabled = shouldActivate;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Built_State = HasValidPosition ? BuiltState.Fixing : Built_State;
        PropManager.ReleaseProp(this);

        foreach (var meshCollider in _meshColliders)
        {
            meshCollider.enabled = true;
        }
    }

    public void Rotate() => rotater.Begin(PropManager.QuaternionFromDirection(PlacedDirection));//, GetPositionCorrectionOffset());
  
    public void Nudge(float scaleMax = 1.1f, float duration = .08f, TimeTickSystem.EaseCurveType easeCurveType = TimeTickSystem.EaseCurveType.NudgeScale)
        => nudger.Begin(scaleMax, duration, easeCurveType);

    public void Float() => floater.Begin();
 
    public void LiftUp(bool queueRequest) 
    {
        Built_State = BuiltState.Fixing;
        lifter.LiftUp(queueRequest, 
                        () => Float(),
                        () => Built_State = BuiltState.NotFixed);
    } 

    public void LiftDown(Grid finalPlacementGrid, Quaternion finalRotation, params Action[] finalCallbacks)
                => lifter.LiftDown(finalPosition: isCentrallyPositionedToGrid 
                                                          ? new(finalPlacementGrid.GridWorldPosition.x + BuildingGrid.cellSize/2f, 0 , finalPlacementGrid.GridWorldPosition.z + BuildingGrid.cellSize / 2f)
                                                          : finalPlacementGrid.GridWorldPosition,
                                   finalRotation: finalRotation,
                                   initialCallback: () =>
                                   {
                                       rotater.End();
                                       nudger.End();
                                       floater.End();
                                   },
                                   finalCallbacks: finalCallbacks.Append(() => Built_State = BuiltState.Fixed).ToArray());


    public void SubscribeToVerifivationCallback(bool shouldSubscribe)
    {
        if (shouldSubscribe)
        {
            BuildingGrid.Instance.OnValidate += VerificationCallback;
            Debug.LogWarning("subscribed for: "  + ShopUpgradeBluePrint?.GetName());
        }        
        else
        {
            BuildingGrid.Instance.OnValidate -= VerificationCallback;
            Debug.LogWarning("unsubscribed for: " + ShopUpgradeBluePrint?.GetName());
        }
    }

    public void VerificationCallback(bool isVerified)
    {
        //Debug.Log("verification :" + isVerified + "for prop name : " + ShopUpgradeBluePrint.GetName());
        HasValidPosition = isVerified;
        _meshrenderer.material.color = isVerified ? _originalMeshColor : Color.red;
    }
}
