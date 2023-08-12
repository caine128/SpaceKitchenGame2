using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class PropManager
{
    private static Prop selectedProp;
    private static Vector3 interpolation;
    private static PointerEventData pointerEventData;

    private static readonly List<Prop> builtProps = new();

    

    public enum Direction
    {
        Up = 0,
        Left = 1,
        Down = 2,
        Right = 3,
    }

    private static Direction GetNextDirection(Direction curentDirection)
        => curentDirection switch
        {
            Direction.Up => Direction.Left,
            Direction.Left => Direction.Down,
            Direction.Down => Direction.Right,
            Direction.Right => Direction.Up,
            _ => throw new NotImplementedException(),
        };

    public static Quaternion QuaternionFromDirection(Direction direction)
        => direction switch
        {
            Direction.Up => Quaternion.Euler(Vector3.zero),
            Direction.Left => Quaternion.Euler(new Vector3(0f,90,0f)),
            Direction.Down => Quaternion.Euler(new Vector3(0f, 180, 0f)),
            Direction.Right => Quaternion.Euler(new Vector3(0f, 270, 0f)),
            _ => throw new NotImplementedException(),
        };

    public static void BeginBuildingNewProp(UnityEngine.GameObject propPrefab, Grid grid)
    {
        if (selectedProp != null)
            return;

        Prop newProp = UnityEngine.Object.Instantiate(propPrefab, new Vector3(grid.GridWorldPosition.x, 3f, grid.GridWorldPosition.z), Quaternion.identity).GetComponentInChildren<Prop>();
        newProp.SubscribeToVerifivationCallback(true);
        newProp.BuiltSate = Prop.BuiltState.NotFixed;
        newProp.Float();
        
        selectedProp = newProp;

        BuildingGrid.Instance.PlaceGridMarkers(selectedProp);
    }

    public static void SelectProp(Prop currentProp, PointerEventData pointerEventData, Vector3 raycastToGroundPosition)
    {

        if (selectedProp == null)
        {
            selectedProp = currentProp;
            
            selectedProp.BuiltSate = Prop.BuiltState.NotFixed;
            builtProps.ForEach(bp => bp.ActivateColliers(shouldActivate: false));

            SetInterpolationValue(raycastToGroundPosition);
            SubscribeToMoveProp(pointerEventData);

            BuildingGrid.Instance.ClearGrids(selectedProp);
            BuildingGrid.Instance.PlaceGridMarkers(selectedProp);

            selectedProp.Nudge();
            selectedProp.LiftUp(queueRequest: true);           
        }
        else if (selectedProp == currentProp && selectedProp.BuiltSate == Prop.BuiltState.NotFixed)
        {
            selectedProp.ActivateColliers(shouldActivate: false);
            builtProps.ForEach(bp => bp.ActivateColliers(shouldActivate: false));

            SetInterpolationValue(raycastToGroundPosition);
            SubscribeToMoveProp(pointerEventData);

            selectedProp.Nudge();
        }
        else
        {
            Debug.Log("there is already a selected prop");
        }
    }
    public static void ReleaseProp(Prop currentProp)
    {
        currentProp.BuiltSate = Prop.BuiltState.Fixing;

        if (selectedProp == null || selectedProp != currentProp)
        {
            Debug.Log($"there is no selectedprop or selectedprop is not this, reverting BuiltState to NotFixed");
            currentProp.BuiltSate = Prop.BuiltState.NotFixed;
            return;
        }
        

        builtProps.ForEach(bp => bp.ActivateColliers(shouldActivate: true));

        UnsubscribeToMoveProp();

        if (BuildingGrid.Instance.TrySetPropToGrids(currentProp))
        {
            selectedProp.SubscribeToVerifivationCallback(false);
            selectedProp.BuiltSate = Prop.BuiltState.Fixed;
            builtProps.Add(selectedProp);
            selectedProp.LiftDown(BuildingGrid.Instance.MarkerParent.position,
                                         QuaternionFromDirection(selectedProp.PlacedDirection),
                                         () => BuildingGrid.Instance.RemoveGridMarkers(),
                                         () => selectedProp = null);
        }
        else
        {
            currentProp.BuiltSate = Prop.BuiltState.NotFixed;
        }
    }

    private static void MoveProp()
    {
        if (selectedProp == null)
            return;

        if (pointerEventData.pointerCurrentRaycast.gameObject?.layer != CollisionLayers.GROUND_LAYER)
            return;

        var interpolatedPointerPos = pointerEventData.pointerCurrentRaycast.worldPosition - interpolation;

        selectedProp.transform.position = Vector3.Lerp(selectedProp.transform.position,
                                                       new Vector3(interpolatedPointerPos.x, selectedProp.transform.position.y, interpolatedPointerPos.z),//new Vector3(pointerEventData.pointerCurrentRaycast.worldPosition.x - interpolation.x, selectedProp.transform.position.y, pointerEventData.pointerCurrentRaycast.worldPosition.z - interpolation.z),
                                                       Time.deltaTime * 15);




        var hoveredGridWorldPosition = BuildingGrid.Instance.GridSystem.GetGrid(interpolatedPointerPos).GridWorldPosition; // pointerEventData.pointerCurrentRaycast.worldPosition - interpolation).GridWorldPosition;

        if (hoveredGridWorldPosition != BuildingGrid.Instance.MarkerParent.position
            && BuildingGrid.Instance.GridSystem.IsWithinEffectiveRange(pointerEventData.pointerCurrentRaycast.worldPosition - interpolation))
        {
            BuildingGrid.Instance.MoveGridMarkers(hoveredGridWorldPosition);
        }

        var selectedPropRotation = QuaternionFromDirection(selectedProp.PlacedDirection);
        if (BuildingGrid.Instance.MarkerParent.rotation != selectedPropRotation)
        {
            BuildingGrid.Instance.RotateGridMarkers(selectedPropRotation);
        }
    }

    public static void RotateProp()
    {
        if (selectedProp == null || selectedProp.BuiltSate != Prop.BuiltState.NotFixed)
            return;

        selectedProp.PlacedDirection = GetNextDirection(selectedProp.PlacedDirection);
        BuildingGrid.Instance.RotateGridMarkers(QuaternionFromDirection(selectedProp.PlacedDirection));
        selectedProp.Rotate();
    }

    private static void SetInterpolationValue(Vector3 raycastToGroundPosition)
    {
        interpolation = new Vector3(raycastToGroundPosition.x - selectedProp.transform.position.x,
                                    0f,
                                    raycastToGroundPosition.z - selectedProp.transform.position.z);
    }

    private static void SubscribeToMoveProp(PointerEventData pointerEventData_IN)
    {
        pointerEventData = pointerEventData_IN;
        TimeTickSystem.OnLateUpdate += MoveProp;
    }

    private static void UnsubscribeToMoveProp()
    {
        pointerEventData = null;
        TimeTickSystem.OnLateUpdate -= MoveProp;
    }

}
