using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class Detect_ShopPurchaseOrInfoclick : DetectClickRequest<ShopUpgrade>, ISinglePanelInvokeButton
{
    public InvokablePanelController PanelToInvoke { get { return panelToInvoke; } }
    [SerializeField] protected InvokablePanelController panelToInvoke;

    public event Action<InvokablePanelController, Action> OnInvokeButtonPressed;

    public void InvokePanel(Action panelLoadAction = null)
    {
        OnInvokeButtonPressed?.Invoke(panelToInvoke, panelLoadAction);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        // equality changed to distance checking in order to gentlize the click area
        if (isValidClick && (Vector2.Distance(initialClickPosition, eventData.position) <= PanelManager.MAXCLICKOFFSET)) // LATER TO ADD THE INFO BUTTON CONDITION AS WELL JUST COPY FROM CRAFTREQUEST CONDITIONS
        {
            if (initialSelection is ShopUpgradeContainer shopUpgradeContainer)
            {
                shopUpgradeContainer.Tintsize();

                switch (shopUpgradeContainer.bluePrint)
                {
                    case WorkStationUpgrade workStationUpgradeBluePrint when ShopData.CheckPresenceOfUpgrade(workStationUpgradeBluePrint, out var existingWorkStations): // ShopUpgrade existingShopUpgrade):

                        var existingWorkStation = existingWorkStations.First(); //(WorkStationUpgrade)existingShopUpgrade;
                        if (existingWorkStation.IsReadyToReclaim)
                        {
                            existingWorkStation.LevelUp();
                        }
                        else if(existingWorkStation.RemainingDuration > 0)
                        {
                            var panelToLoad = PanelManager.InvokablePanels[typeof(ProgressPopupPanel)];
                            /*var activeWorkStationUpgrades = ShopData.ShopUpgradesIteration_Dict[ShopUpgradeType.Type.WorkstationUpgrades]
                                                            .Select(su => su as WorkStationUpgrade)
                                                            .Where(wu => wu.IsReadyToReclaim == true || wu.RemainingDuration > 0);
                            var clickedObjectIndex = activeWorkStationUpgrades.Select((awu, i) => (awu, i))
                                                                              .Where(awui => awui.awu.Equals(existingShopUpgrade))
                                                                              .Select(awui => awui.i)
                                                                              .DefaultIfEmpty(0).FirstOrDefault();*/
                            
                            var (ongoingUpgrades, clickedObjectIndex) = ShopData.GetOngoingUpgradesWithClickedIndex(existingWorkStation);
                            ProgressPanelLoadData panelLoadData = new(mainLoadInfo: null, panelHeader: "Current Upgrades", tcs_IN: null,
                                                                      rushableItemsData: ongoingUpgrades, clickedObjectIndex: clickedObjectIndex);

                            PanelManager.ActivateAndLoad(invokablePanel_IN: panelToLoad, panelLoadAction_IN: () => ProgressPopupPanel.Instance.LoadPanel(panelLoadData));
                        }
                        else
                        {
                            var panelLoadData = new PanelLoadData(mainLoadInfo: existingWorkStation, panelHeader: existingWorkStation.GetName(), tcs_IN: null);
                            PanelManager.ActivateAndLoad(invokablePanel_IN: PanelManager.InvokablePanels[typeof(ShopUpgradesInfoPanel_Manager)],
                                                         panelLoadAction_IN: () => ShopUpgradesInfoPanel_Manager.Instance.LoadPanel(panelLoadData));
                        }

                        break;

                    default:
                        if (ShopData.Instance.TryPurchaseShopUpgrade(shopUpgradeContainer.bluePrint, new Gold()))
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
                        break;

                }
            }
        }
    }  
}
