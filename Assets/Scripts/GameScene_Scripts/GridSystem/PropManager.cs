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
                if(PanelManager.SelectedPanels.TryPeek(out var invokablePanelController) 
                    && invokablePanelController == PanelManager.InvokablePanels[typeof(BuildOptionsPanel_Manager)])
                {
                    // TODO : Buildoptionspanel buttons should be reassigned asccording to the new selection
                    Debug.Log("// TODO : Buildoptionspanel buttons should be reassigned asccording to the new selection");
                }
                else
                {
                    var invokablePanel = PanelManager.InvokablePanels[typeof(BuildOptionsPanel_Manager)];

                    PanelManager.ActivateAndLoad(invokablePanel_IN: invokablePanel,
                                                 panelLoadAction_IN: () => ((BuildOptionsPanel_Manager)invokablePanel.MainPanel).LoadPanel(value));
                }
            }

            /*if (selectedProp_backingField == null && value != null)
            {
                var invokablePanel = PanelManager.InvokablePanels[typeof(BuildOptionsPanel_Manager)];

                PanelManager.ActivateAndLoad(invokablePanel_IN: invokablePanel,
                                             panelLoadAction_IN: () => ((BuildOptionsPanel_Manager)invokablePanel.MainPanel).LoadPanel(value));
            }
            else if (selectedProp_backingField != null && value != null)
            {
                // TODO : Buildoptionspanel buttons should be reassigned asccording to the new selection 
            }*/
            /*else if(selectedProp_backingField.IsPurchased && value == null)
            {
                PanelManager.DeactivatePanel(PanelManager.SelectedPanels.Peek(), nextPanelLoadAction_IN: null, unloadAction:
                                () =>
                                {
                                    PanelManager.TopBarsController.ArrangeBarsFinal();
                                    PanelManager.BottomBarsController.PlaceBars();
                                    PanelManager.CraftWheelController.PlaceBars();
                                    PanelManager.ClearStackAndDeactivateElements();
                                });
            }
            else
            {
                PanelManager.DeactivatePanel(invokablePanelIN: PanelManager.SelectedPanels.Peek(),
                                             nextPanelLoadAction_IN: null);
            }    */       
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

    public static void BeginBuildingNewProp(ShopUpgrade shopUpgradeBlueprintToBeCloned)
    {
        if (SelectedProp != null)
            return;


        Prop newProp = UnityEngine.Object.Instantiate(shopUpgradeBlueprintToBeCloned.GetPrefab).GetComponentInChildren<Prop>();

        newProp.Initialize(shopUpgradeBlueprintToBeCloned,
                           subscribeToVerificationCallback: true,
                           builtState: Prop.BuiltState.NotFixed,
                           anchorGrid: BuildingGrid.Instance.FindMostCentralPlacementGrid(shopUpgradeBlueprintToBeCloned.GetPropSize),
                           onInitialized: p => BuildingGrid.Instance.PlaceGridMarkers(p));


        SelectedProp = newProp;
        //BuildingGrid.Instance.PlaceGridMarkers(SelectedProp);
        //BuildingGrid.Instance.SetPropToGrids(SelectedProp);
    }

    public static void SelectProp(Prop currentProp, PropMovementInterpolationData propMovementInterpolationData = null) //PointerEventData pointerEventData = null, Vector3? raycastToGroundPosition = null)
    {
        if(SelectedProp == null)
        {
            SelectedProp = currentProp;
            builtProps.ForEach(bp => bp.ActivateColliders(shouldActivate: false));

            if(propMovementInterpolationData != null)
            {
                SetInterpolationValue(propMovementInterpolationData.raycastToGroundPosition);
                SubscribeToMoveProp(propMovementInterpolationData.pointerEventData);//  pointerEventData);

                BuildingGrid.Instance.ClearGrids(SelectedProp);
                BuildingGrid.Instance.PlaceGridMarkers(SelectedProp);

                SelectedProp.Nudge();
                SelectedProp.LiftUp(queueRequest: true);
            }

        }
        /*if (SelectedProp == null && propMovementInterpolationData != null)
        {
            SelectedProp = currentProp;
            
            builtProps.ForEach(bp => bp.ActivateColliders(shouldActivate: false));

            SetInterpolationValue(propMovementInterpolationData.raycastToGroundPosition);
            SubscribeToMoveProp(propMovementInterpolationData.pointerEventData);//  pointerEventData);

            BuildingGrid.Instance.ClearGrids(SelectedProp);
            BuildingGrid.Instance.PlaceGridMarkers(SelectedProp);

            SelectedProp.Nudge();
            SelectedProp.LiftUp(queueRequest: true);           
        }
        else if(SelectedProp == null && propMovementInterpolationData == null)
        {

        }*/

        else if (SelectedProp == currentProp && SelectedProp.Built_State != Prop.BuiltState.Fixing)
        {
            Debug.LogWarning("REselecting existing prop");

            SelectedProp.SubscribeToVerifivationCallback(true);
            SelectedProp.ActivateColliders(shouldActivate: false);
            builtProps.ForEach(bp => bp.ActivateColliders(shouldActivate: false));

            SetInterpolationValue(propMovementInterpolationData.raycastToGroundPosition);
            SubscribeToMoveProp(propMovementInterpolationData.pointerEventData);

            //BuildingGrid.Instance.ClearGrids(SelectedProp);
            //BuildingGrid.Instance.RemoveGridMarkers();
            //BuildingGrid.Instance.PlaceGridMarkers(SelectedProp);

            SelectedProp.Nudge();
           
            if(SelectedProp.Built_State == Prop.BuiltState.Fixed) 
                SelectedProp.LiftUp(queueRequest: true);
                     
        }
        else
        {
            Debug.Log("there is already a selected prop");
        }
    }



    public static void ReleaseProp(Prop currentProp, params Action[] postReleaseActions)
    {
        if (SelectedProp == null || SelectedProp != currentProp)
        {
            return;
        }

        builtProps.ForEach(bp => bp.ActivateColliders(shouldActivate: true));
        UnsubscribeToMoveProp();

        if (SelectedProp.HasValidPosition)
        {
            //SelectedProp.BuiltSsate = Prop.BuiltState.Fixing;
            //BuildingGrid.Instance.SetPropToGrids(SelectedProp);
            SelectedProp.SubscribeToVerifivationCallback(false);

            SelectedProp.LiftDown(BuildingGrid.Instance.GridSystem.GetGrid(BuildingGrid.Instance.MarkerParent.position),
                                  QuaternionFromDirection(SelectedProp.PlacedDirection),
                                  finalCallbacks: postReleaseActions);//.Append(() => SelectedProp.BuiltSsate = Prop.BuiltState.Fixed).ToArray());
                                                                      //.Append(() => SelectedProp = null).ToArray()); 
        }
        /*if (BuildingGrid.Instance.TrySetPropToGrids(currentProp))
        {
            currentProp.BuiltSate = Prop.BuiltState.Fixing;
            SelectedProp.SubscribeToVerifivationCallback(false);
            //SelectedProp.BuiltSate = Prop.BuiltState.Fixed;
            //builtProps.Add(SelectedProp);
            postReleaseActions = postReleaseActions.Append(() => SelectedProp.BuiltSate = Prop.BuiltState.Fixed)
                                                   .Append(() => SelectedProp = null).ToArray();

            SelectedProp.LiftDown(BuildingGrid.Instance.MarkerParent.position,
                                  QuaternionFromDirection(SelectedProp.PlacedDirection),
                                  postReleaseActions);
                                  //() => BuildingGrid.Instance.RemoveGridMarkers(),                                        
                                  //() => SelectedProp.BuiltSate = Prop.BuiltState.Fixed,
                                  //() => SelectedProp = null);
        }*/
        /*else
        {
            currentProp.BuiltSate = Prop.BuiltState.NotFixed;
        }*/
    }

    public static void PurchaseProp(Prop currentProp)
    {
        if(SelectedProp == null || SelectedProp != currentProp || !SelectedProp.HasValidPosition)
        {
            Debug.Log("there is no selectedprop or selectedprop is not this, cannot purchase prop");
            return;
        }

        BuildingGrid.Instance.RegisterPropToGrids(SelectedProp);       
        
        //SelectedProp.IsPurchased = true;
        var clonedShopUpgrade = ShopData.Instance.CloneAndPurchaseShopUpgrade(currentProp.ShopUpgradeBluePrint);

        SelectedProp.RegisterClonedBluePrint(clonedShopUpgrade);
        builtProps.Add(currentProp);
        ReleaseProp(currentProp,
                            () => BuildingGrid.Instance.RemoveGridMarkers(),
                            () => SelectedProp = null);  // TODO : Nulling the selection closes the panel ?? but is it the thing ?
        /*switch (currentProp.Built_State)
        {
            case Prop.BuiltState.NotFixed:
                ReleaseProp(currentProp,
                            () => BuildingGrid.Instance.RemoveGridMarkers(),
                            () => SelectedProp = null);  // TODO : Nulling the selection closes the panel ?? but is it the thing ?
                break;

            case Prop.BuiltState.Fixing:
            case Prop.BuiltState.Fixed:
                BuildingGrid.Instance.RemoveGridMarkers();
                SelectedProp = null;                      // TODO : Nulling the selection closes the panel ?? but is it the thing ?
                break;
        }*/
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
            BuildingGrid.Instance.MoveGridMarkers(SelectedProp.IsCentrallypositionedToGrid 
                                                      ? new Vector3(hoveredGridWorldPosition.x+ (BuildingGrid.cellSize/2f), hoveredGridWorldPosition.y,hoveredGridWorldPosition.z + (BuildingGrid.cellSize / 2f)) 
                                                      :hoveredGridWorldPosition);
        }

        var selectedPropRotation = QuaternionFromDirection(SelectedProp.PlacedDirection);
        if (BuildingGrid.Instance.MarkerParent.rotation != selectedPropRotation)
        {
            BuildingGrid.Instance.RotateGridMarkers(selectedPropRotation);
        }
    }

    public static void RotateProp()
    {
        if (SelectedProp == null || SelectedProp.Built_State == Prop.BuiltState.Fixing) //SelectedProp.BuiltSsate != Prop.BuiltState.NotFixed)
            return;

        SelectedProp.PlacedDirection = GetNextDirection(SelectedProp.PlacedDirection);
        BuildingGrid.Instance.RotateGridMarkers(QuaternionFromDirection(SelectedProp.PlacedDirection));
        SelectedProp.Rotate();
    }

    public static void DestroyProp(Prop prop)
    {
        if (SelectedProp == null || prop != SelectedProp)
            return;

        UnityEngine.Object.Destroy(prop);
        BuildingGrid.Instance.RemoveGridMarkers();
        SelectedProp = null;
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
