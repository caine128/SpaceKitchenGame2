using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Events;
using UnityEngine;

using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class BuildingGrid : SingletonMonoBehaviourPersistent<BuildingGrid>, IPointerDownHandler //, IPointerMoveHandler 
{

    [SerializeField] private UnityEngine.GameObject testPropPrefab;
    [SerializeField] private UnityEngine.GameObject groundTile; // LaterToTakeFromScriptable with the texture of course

    [SerializeField] private UnityEngine.GameObject debugTextEachTile;

    public GridSystem GridSystem { get; private set; }
    public PropManager PropManager { get; private set; }

    private List<GridMarker> _gridMarkers = new(8);
    public Transform MarkerParent { get => markerParent; }
    [SerializeField] private Transform markerParent;

    public event Action<bool> OnValidate;

    public static float cellSize = 1f;

    protected override void Awake()
    {
        base.Awake();
        GridSystem = new(50,
                         50,
                         cellSize: cellSize,
                         shopSize: (20, 20),
                         tile_PF: groundTile.transform,
                         debugTextPrefab: debugTextEachTile.transform);
    }


    private void Update()     //TODO: later to remove FOR TEST PURPOSES
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            PropManager.RotateProp();
        }
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

        if(ValidateGridMarkers(out IEnumerable<Grid> freeGrids))
        {
            foreach (var freeGrid in freeGrids)
            {
                freeGrid.PlaceProp(prop);            
            }
            return true;
        }
        
        return false;

        /*foreach (var freeGrid in freeGrids)
        {
            freeGrid.PlaceProp(prop);
        }
        return true;*/
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

        //var isPositionValid = _gridMarkers.All(gm => ValidateGridMarker(gm, out _));
        //ColorizeGridMarkers(isPositionValid);
        //OnValidate?.Invoke(isPositionValid);
    }

    public void RotateGridMarkers(Quaternion rotation)
    {
        markerParent.rotation = rotation;

        ValidateGridMarkers(out _);
        //ColorizeGridMarkers(_gridMarkers.All(gm => ValidateGridMarker(gm, out _)));
    }

    /*private bool ValidateGridMarker(GridMarker gridMarker, out Grid grid)
    {
        grid = GridSystem.GetGrid(gridMarker.AnchorPosition);
        return grid.isBuildable && !grid.IsOccupied;
    }*/

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

    /* private void ValidateGridMarkers() 
     {
         areGridMarkersValid = _gridMarkers.Select(gm => GridSystem.GetGrid(gm.AnchorPosition))
                                                           .All(g => g.isBuildable && !g.IsOccupied);
     }*/

    /*private void ColorizeGridMarkers(bool isBuildable)
    {
        _gridMarkers.ForEach(gm => gm.ChangeColor(isBuildable ? Color.green : Color.red));

    }*/

    public void MoveGridMarkers(Vector3 position)
    {
        markerParent.position = position;

        ValidateGridMarkers(out _);
        //ColorizeGridMarkers(_gridMarkers.All(gm => ValidateGridMarker(gm, out _)));
    }

    public void RemoveGridMarkers()
    {
        foreach (var gridMarker in _gridMarkers)
        {
            GridMarkerSpawner.Pool.Release(gridMarker);
        }
        _gridMarkers.Clear();
    }

    // TODO : Remove this we will not need to instantiate props from here;
    public void OnPointerDown(PointerEventData eventData)
    {
        PropManager.BeginBuildingNewProp(testPropPrefab, GridSystem.GetGrid(eventData.pointerCurrentRaycast.worldPosition));
    }
}
