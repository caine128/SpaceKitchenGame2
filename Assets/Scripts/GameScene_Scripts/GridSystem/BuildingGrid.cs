using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.Events;
using UnityEngine;

using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class BuildingGrid : SingletonMonoBehaviourPersistent<BuildingGrid>//, IPointerDownHandler //, IPointerMoveHandler 
{

    [SerializeField] private UnityEngine.GameObject testPropPrefab;
    [SerializeField] private UnityEngine.GameObject groundTile; // LaterToTakeFromScriptable with the texture of course

    [SerializeField] private UnityEngine.GameObject debugTextEachTile;

    public Transform MarkerParent { get => markerParent; }
    [SerializeField] private Transform markerParent;
    private List<GridMarker> _gridMarkers = new(8);

    public GridSystem GridSystem { get; private set; }

    private List<Grid> ShopGrids;
    private HashSet<Grid> invalidPlacementGrids;
    public static float cellSize = 1f;

    public event Action<bool> OnValidate;

    protected override void Awake()
    {
        base.Awake();

        Initialize(); // TODO : Later to take on config // pay attention that execute event of default dict request comes before this script starts
    }

    private void Update()     //TODO: FOR TEST PURPOSES LATER TO DELEETE 
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            PropManager.RotateProp();
        }
    }

    private void Initialize(GridSystem gridSystem = null, List<Grid> shopGrids = null)
    {
        this.GridSystem = gridSystem ?? new(50, 50, cellSize: cellSize, shopSize: (10, 8), tile_PF: groundTile.transform, debugTextPrefab: debugTextEachTile.transform);
        this.ShopGrids = shopGrids ?? GridSystem.GetGrids(g => g.isBuildable)
                                                                    .OrderBy(g => CalculateDistanceFrom(fromGrid: GridSystem.CenterGrid, toGrid: g))
                                                                    .ToList();

        invalidPlacementGrids = new HashSet<Grid>(ShopGrids.Count);

        ///////////////////////////////// TODO : FOR TEST PURPOSES LATER TO DELEETE 
        int counter = 0; 
        foreach (var sg in ShopGrids)
        {
            var debugTextGO = UnityEngine.Object.Instantiate(debugTextEachTile);
            var tmComponent = debugTextGO.GetComponent<TextMeshPro>();
            tmComponent.text = $"{counter} {Environment.NewLine} {sg.GridPosition.x} , {sg.GridPosition.z}";
            debugTextGO.GetComponent<Transform>().position = new Vector3(sg.GridPosition.x + .5f, 0.2f, sg.GridPosition.z + .5f);
            counter++;
        }
        /////////////////////////////////
    }

    private int CalculateDistanceFrom(Grid fromGrid, Grid toGrid)
    {
        int distanceX = Mathf.Abs(toGrid.GridPosition.x - fromGrid.GridPosition.x);
        int distanceZ = Mathf.Abs(toGrid.GridPosition.z - fromGrid.GridPosition.z);

        return distanceX > distanceZ
                    ? (14 * distanceZ) + (10 * (distanceX - distanceZ))
                    : (14 * distanceX) + (10 * (distanceZ - distanceX));
    }

    public Vector3 FindMostCentralPlacementPosition((int x, int z) propSize)
    {
        invalidPlacementGrids.Clear();
        Vector3 posToReturn = GridSystem.CenterGrid.GridWorldPosition;

        for (int i = 0; i < ShopGrids.Count; i++)
        {

            if (!invalidPlacementGrids.Contains(ShopGrids[i]) && CheckGridPositionsOfPropForAnchor(ShopGrids[i].GridPosition, propSize))
            {
                posToReturn = ShopGrids[i].GridWorldPosition;
                break;
            }
        }

        ///////////////////////////////// TODO : FOR TEST PURPOSES LATER TO DELEETE 
        foreach (var sg in invalidPlacementGrids)
        {
            var debugTextGO = UnityEngine.Object.Instantiate(debugTextEachTile);
            var tmComponent = debugTextGO.GetComponent<TextMeshPro>();
            tmComponent.color = Color.red;
            tmComponent.text = "invalid";
            debugTextGO.GetComponent<Transform>().position = new Vector3(sg.GridPosition.x + .5f, 0.2f, sg.GridPosition.z + .5f);
        }
        /////////////////////////////////

        return posToReturn;
    }

    private bool CheckGridPositionsOfPropForAnchor(GridPosition fakeAnchor, (int x, int z) propSize)
    {
        for (int x = fakeAnchor.x; x < fakeAnchor.x + propSize.x; x++)
        {
            for (int z = fakeAnchor.z; z < fakeAnchor.z + propSize.z; z++)
            {
                var grid = GridSystem.GetGrid(new GridPosition(x, z));
                Debug.LogWarning("cehcking gridpos : " + x + " , " + z);
                if (!grid.isBuildable)
                {
                    return false;
                }

                else if (grid.IsOccupied)
                {
                    invalidPlacementGrids.Add(grid);
                    return false;
                }
            }
        }

        return true;
    }

    public bool TrySetPropToGrids(Prop prop)
    {
        /*List<Grid> freeGrids = new(prop.Size.x * prop.Size.z);

        foreach (var gridMarker in _gridMarkers)
        {
            if (ValidateGridMarker(gridMarker, out var grid))
            {
                freeGrids.Add(grid);
            }
            else
            {
                Debug.Log("cannot set prop at position(s)");
                return false;
            }
        }*/

        if (ValidateGridMarkers(out IEnumerable<Grid> freeGrids))
        {
            foreach (var freeGrid in freeGrids)
            {
                freeGrid.PlaceProp(prop);
            }
            return true;
        }

        return false;

    }

    public void ClearGrids(Prop prop)
    {
        ClearGrids(prop.GetCurrentGridPositions());
    }

    private void ClearGrids(IEnumerable<GridPosition> gridPositions)
    {
        foreach (var gridPosition in gridPositions)
        {
            GridSystem.GetGrid(gridPosition).RemoveProp();
        }
    }

    public void PlaceGridMarkers(Prop prop)
    {
        Debug.LogWarning("gridmarkers are being placed ");
        markerParent.SetPositionAndRotation(new Vector3(prop.transform.position.x,
                                            0f,
                                            prop.transform.position.z), PropManager.QuaternionFromDirection(prop.PlacedDirection));

        foreach (var gridPosition in prop.GetCurrentGridPositions())
        {
            var gridMarker = GridMarkerSpawner.Pool.Get();

            gridMarker.Place(GridSystem.FromGridPosToWorldPos(gridPosition));

            gridMarker.transform.SetParent(markerParent);
            _gridMarkers.Add(gridMarker);

        }

        ValidateGridMarkers(out _);

    }

    public void RotateGridMarkers(Quaternion rotation)
    {
        markerParent.rotation = rotation;

        ValidateGridMarkers(out _);
    }

    private bool ValidateGridMarkers(out IEnumerable<Grid> grids)
    {
        var gridsTemp = _gridMarkers.Select(gm => GridSystem.GetGrid(gm.AnchorPosition));

        if (gridsTemp.All(g => g.isBuildable && !g.IsOccupied))
        {
            grids = gridsTemp;
            OnValidate?.Invoke(true);
            return true;
        }
        else
        {
            grids = null;
            OnValidate?.Invoke(false);
            return false;
        }
    }

    public void MoveGridMarkers(Vector3 position)
    {
        markerParent.position = position;

        ValidateGridMarkers(out _);
    }

    public void RemoveGridMarkers()
    {
        foreach (var gridMarker in _gridMarkers)
        {
            GridMarkerSpawner.Pool.Release(gridMarker);
        }
        _gridMarkers.Clear();
    }

}
