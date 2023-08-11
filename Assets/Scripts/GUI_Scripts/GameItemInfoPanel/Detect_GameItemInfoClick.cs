using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Detect_GameItemInfoClick : DetectClickRequest<GameObject>, IMultiPanelInvokeButton//ISinglePanelInvokeButton
{
    private static IReassignablePanel _reassignablePanel;

    public InvokablePanelController[] InvokablePanels { get { return _invokablePanels; } }
    [SerializeField] private InvokablePanelController[] _invokablePanels;


    public void Awake()                 // LATER TO TAKE UP
    {
        _reassignablePanel = (IReassignablePanel)InvokablePanels[1].MainPanel; // GameObject.Find("Inventory_Panel_Parent").GetComponent<IReassignablePanel>();

    }



    public override  void OnPointerUp(PointerEventData eventData)
    {
        if (isValidClick && (Vector2.Distance(initialClickPosition, eventData.position) <= PanelManager.MAXCLICKOFFSET))  //initialClickPosition == eventData.position
        {
            if (initialSelection is GameItemContainer gameItemContainerSelection)
            {
                gameItemContainerSelection.Tintsize();

                if (_reassignablePanel.panelAssignedState == IReassignablePanel.AssignedState.Inventory_FromProductToEnhance)
                {

                    var selectedEnhancement = gameItemContainerSelection.bluePrint as Enhancement;
                    var selectedEnhanceable = GameItemInfoPanel_Manager.Instance.SelectedRecipe as IEnhanceable;

                    if (_invokablePanels[2].MainPanel is EnhanceItemPopupPanel)
                    {
                        var enhancePanelLoadData = new PopupPanel_Enhancement_LoadData(
                            mainLoadInfo: (SortableBluePrint)selectedEnhanceable,
                            panelHeader: null,
                            enhancement_IN: selectedEnhancement, //gameItemContainerSelection.bluePrint as Enhancement, 
                            tcs_IN: null,
                            panelState_IN: IReassignablePanel.AssignedState.Inventory_FromProductToEnhance); //_reassignablePanel.panelAssignedState);
                            //isBlueprintEnhanced_IN: selectedEnhanceable.isEnhanced);

                        PanelManager.ActivateAndLoad(invokablePanel_IN: _invokablePanels[2], panelLoadAction_IN:
                            () => EnhanceItemPopupPanel.Instance.LoadPanel(enhancePanelLoadData));
                    }                                
                }

                /*  
                 *  else if (_reassignablePanel.panelAssignedState == IReassignablePanel.AssignedState.Inventory_ForReplacingEnhancemnet)
                {
                    //PopupPanel_Enhancement_LoadData panelLoadData;

                    //var enhancePopupPanel = (EnhanceItemPopupPanel)_invokablePanels[2].MainPanel;
                    var selectedEnhancement = gameItemContainerSelection.bluePrint as Enhancement;
                    var selectedEnhanceable = GameItemInfoPanel_Manager.Instance.selectedRecipe as IEnhanceable;

                    //panelLoadData = new (mainLoadInfo: (SortableBluePrint)selectedEnhanceable,
                    //    panelHeader: null, 
                    //    enhancement_IN: selectedEnhancement, //selectedEnhanceable.enhancementsDict_ro[selectedEnhancement.GetEnhancementType()], 
                    //    tcs_IN:null,
                    //    panelSate_IN: IReassignablePanel.AssignedState.Inventory_ForReplacingEnhancemnet);;

                    //enhancePopupPanel.LoadPanel(panelLoadData);

                    var isOverEnhanceConfirmed = await Enhance.IsOverEnhanceConfirmedAsync(
                        enhanceable_IN: selectedEnhanceable,
                        enhancement_IN: selectedEnhancement,
                        amountToEnhance_IN: 1); //enhancePopupPanel.IsOverEnhanceConfirmed();
                    
                    if (isOverEnhanceConfirmed)
                    {
                        var isEnhancementDestroyed = await enhancePopupPanel.ConfirmAndDestroyEnhancementAsync(selectedEnhancement);
                        if (isEnhancementDestroyed)
                        {
                            selectedEnhanceable = GameItemInfoPanel_Manager.Instance.selectedRecipe as IEnhanceable;

                            panelLoadData = new(mainLoadInfo: (SortableBluePrint)selectedEnhanceable,
                            panelHeader: null,
                            enhancement_IN: selectedEnhancement,
                            tcs_IN: null,
                            panelSate_IN: IReassignablePanel.AssignedState.Inventory_ForReplacingEnhancemnet);

                            enhancePopupPanel.LoadPanel(panelLoadData);
                            enhancePopupPanel.PerformEnhancementAndDisplayModal();
                        }
                    }
                    
                    //if (isEnhancementDestroyed)
                    //{
                    //    /// SelectedItem is updated therefore the variable containint (selectedenheanceable) should be updated as well///
                    //    selectedEnhanceable = GameItemInfoPanel_Manager.Instance.selectedRecipe as IEnhanceable;


                    //    panelLoadData = new(mainLoadInfo: (SortableBluePrint)selectedEnhanceable,
                    //    panelHeader: null,
                    //    enhancement_IN: selectedEnhancement,
                    //    tcs_IN:null,
                    //    panelSate_IN: IReassignablePanel.AssignedState.Inventory_ForReplacingEnhancemnet);

                    //    enhancePopupPanel.LoadPanel(panelLoadData);
                    //    enhancePopupPanel.PerformEnhancementAndDisplayModal();
                    //}
                }  
                
                 */


                else if (_reassignablePanel.panelAssignedState == IReassignablePanel.AssignedState.Inventory_FromEnhanceToProduct)
                {
                    var selectedEnhancement = GameItemInfoPanel_Manager.Instance.SelectedRecipe as Enhancement;
                    var selectedEnhanceable = gameItemContainerSelection.bluePrint as IEnhanceable;

                    if (_invokablePanels[2].MainPanel is EnhanceItemPopupPanel)
                    {
                        var enhancePanelLoadData = new PopupPanel_Enhancement_LoadData(
                            mainLoadInfo: (SortableBluePrint)selectedEnhanceable,
                            panelHeader: null,
                            enhancement_IN: selectedEnhancement,
                            tcs_IN:null,
                            panelState_IN: IReassignablePanel.AssignedState.Inventory_FromEnhanceToProduct);
                            //isBlueprintEnhanced_IN: selectedEnhanceable.isEnhanced);

                        PanelManager.ActivateAndLoad(invokablePanel_IN: _invokablePanels[2], panelLoadAction_IN:
                            () => EnhanceItemPopupPanel.Instance.LoadPanel(enhancePanelLoadData));
                    }                   
                }

                else
                {
                    var bluePrint = gameItemContainerSelection.bluePrint;

                    if (InvokablePanels[0].MainPanel is GameItemInfoPanel_Manager)
                    {
                        //var panelLoadData = new PanelLoadData<SortableBluePrint>(mainLoadInfo: bluePrint, panelHeader: null, tcs_IN: null, bluePrintsToLoad: null);//

                        var panelLoadData = new PanelLoadData(mainLoadInfo: bluePrint, panelHeader: null, tcs_IN: null);
                        PanelManager.ActivateAndLoad(invokablePanel_IN: _invokablePanels[0], panelLoadAction_IN:
                                                () => GameItemInfoPanel_Manager.Instance.LoadPanel(panelLoadData));

                    }
                    else
                    {
                        Debug.LogError("Wrong Panel Type Sent To Load");
                    }

                }
            }
        }

        initialSelection = null;
        initialClickPosition = default(Vector2);
    }




}
