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
    public static Prop SelectedProp
    {
        get { return selectedProp_backingField; }
        set
        {
            selectedProp_backingField = value;

            if(value != null)
            {
                var invokablePanel = PanelManager.InvokablePanels[typeof(BuildOptionsPanel_Manager)];

                PanelManager.ActivateAndLoad(invokablePanel_IN: invokablePanel,
                                             panelLoadAction_IN: () => ((BuildOptionsPanel_Manager)invokablePanel.MainPanel).LoadPanel(value));
            }
            else
            {
                PanelManager.DeactivatePanel(invokablePanelIN: PanelManager.SelectedPanels.Peek(),
                                             nextPanelLoadAction_IN: null);
            }
       
        }
    }
    private static Prop selectedProp_backingField;
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

    public static void BeginBuildingNewProp(ShopUpgrade shopUpgrade)
    {
        if (SelectedProp != null)
            return;


        Prop newProp = UnityEngine.Object.Instantiate(shopUpgrade.GetPrefab).GetComponentInChildren<Prop>();

        newProp.Initialize(shopUpgrade,
                           subscribeToVerificationCallback: true,
                           builtState: Prop.BuiltState.NotFixed,
                           placementPos: BuildingGrid.Instance.FindMostCentralPlacementPosition(shopUpgrade.GetPropSize));
  
        SelectedProp = newProp;

        BuildingGrid.Instance.PlaceGridMarkers(SelectedProp);
    }

    public static void SelectProp(Prop currentProp, PointerEventData pointerEventData, Vector3 raycastToGroundPosition)
    {

        if (SelectedProp == null)
        {
            SelectedProp = currentProp;
            
            SelectedProp.BuiltSate = Prop.BuiltState.NotFixed;
            builtProps.ForEach(bp => bp.ActivateColliers(shouldActivate: false));

            SetInterpolationValue(raycastToGroundPosition);
            SubscribeToMoveProp(pointerEventData);

            BuildingGrid.Instance.ClearGrids(SelectedProp);
            BuildingGrid.Instance.PlaceGridMarkers(SelectedProp);

            SelectedProp.Nudge();
            SelectedProp.LiftUp(queueRequest: true);           
        }
        else if (SelectedProp == currentProp && SelectedProp.BuiltSate == Prop.BuiltState.NotFixed)
        {
            SelectedProp.ActivateColliers(shouldActivate: false);
            builtProps.ForEach(bp => bp.ActivateColliers(shouldActivate: false));

            SetInterpolationValue(raycastToGroundPosition);
            SubscribeToMoveProp(pointerEventData);

            SelectedProp.Nudge();
        }
        else
        {
            Debug.Log("there is already a selected prop");
        }
    }
    public static void ReleaseProp(Prop currentProp)
    {
        currentProp.BuiltSate = Prop.BuiltState.Fixing;

        if (SelectedProp == null || SelectedProp != currentProp)
        {
            Debug.Log($"there is no selectedprop or selectedprop is not this, reverting BuiltState to NotFixed");
            currentProp.BuiltSate = Prop.BuiltState.NotFixed;
            return;
        }
        

        builtProps.ForEach(bp => bp.ActivateColliers(shouldActivate: true));

        UnsubscribeToMoveProp();

        if (BuildingGrid.Instance.TrySetPropToGrids(currentProp))
        {
            SelectedProp.SubscribeToVerifivationCallback(false);
            SelectedProp.BuiltSate = Prop.BuiltState.Fixed;
            builtProps.Add(SelectedProp);
            SelectedProp.LiftDown(BuildingGrid.Instance.MarkerParent.position,
                                         QuaternionFromDirection(SelectedProp.PlacedDirection),
                                         () => BuildingGrid.Instance.RemoveGridMarkers(),
                                         () => SelectedProp = null);
        }
        else
        {
            currentProp.BuiltSate = Prop.BuiltState.NotFixed;
        }
    }

    private static void MoveProp()
    {
        if (SelectedProp == null)
            return;

        if (pointerEventData.pointerCurrentRaycast.gameObject?.layer != CollisionLayers.GROUND_LAYER)
            return;

        var interpolatedPointerPos = pointerEventData.pointerCurrentRaycast.worldPosition - interpolation;

        SelectedProp.transform.position = Vector3.Lerp(SelectedProp.transform.position,
                                                       new Vector3(interpolatedPointerPos.x, SelectedProp.transform.position.y, interpolatedPointerPos.z),//new Vector3(pointerEventData.pointerCurrentRaycast.worldPosition.x - interpolation.x, selectedProp.transform.position.y, pointerEventData.pointerCurrentRaycast.worldPosition.z - interpolation.z),
                                                       Time.deltaTime * 15);




        var hoveredGridWorldPosition = BuildingGrid.Instance.GridSystem.GetGrid(interpolatedPointerPos).GridWorldPosition; // pointerEventData.pointerCurrentRaycast.worldPosition - interpolation).GridWorldPosition;

        if (hoveredGridWorldPosition != BuildingGrid.Instance.MarkerParent.position
            && BuildingGrid.Instance.GridSystem.IsWithinEffectiveRange(pointerEventData.pointerCurrentRaycast.worldPosition - interpolation))
        {
            BuildingGrid.Instance.MoveGridMarkers(hoveredGridWorldPosition);
        }

        var selectedPropRotation = QuaternionFromDirection(SelectedProp.PlacedDirection);
        if (BuildingGrid.Instance.MarkerParent.rotation != selectedPropRotation)
        {
            BuildingGrid.Instance.RotateGridMarkers(selectedPropRotation);
        }
    }

    public static void RotateProp()
    {
        if (SelectedProp == null || SelectedProp.BuiltSate != Prop.BuiltState.NotFixed)
            return;

        SelectedProp.PlacedDirection = GetNextDirection(SelectedProp.PlacedDirection);
        BuildingGrid.Instance.RotateGridMarkers(QuaternionFromDirection(SelectedProp.PlacedDirection));
        SelectedProp.Rotate();
    }

    private static void SetInterpolationValue(Vector3 raycastToGroundPosition)
    {
        interpolation = new Vector3(raycastToGroundPosition.x - SelectedProp.transform.position.x,
                                    0f,
                                    raycastToGroundPosition.z - SelectedProp.transform.position.z);
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
