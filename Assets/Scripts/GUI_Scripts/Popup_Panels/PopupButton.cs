using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Recipes_SO;

public class PopupButton : MultiPurposeButton<ButtonFunctionType.PopupPanel>
{
    [SerializeField] protected TextMeshProUGUI clickRequirementText;

    private PopupPanel parentPanel;
    private int buttonValue;
    public bool StimulateWhenPanelExit { get; private set; } = false;

    private void Awake()
    {
        buttonNames = new string[]
        {
            "View Worker",
            "Search In",
            "Buy From The Shop!",
            "Reject" ,
            "Accept",
            "Refill Ny Token",
            "Refill Ny Gem",
            "Find In : Quest",
            "Craft",
            "Activate",
            "Buy",
            "Upgrade",
            "Destroy Enhancement",
            "Replace Enhancement",
            "Change",
            "Enhance !",

        };
    }

    public void ModifyButtonValue(int newValue)
    {
        buttonValue = newValue;
        clickRequirementText.text = newValue.ToString();
        Debug.Log(newValue);
    }

    public override void SetupButton(ButtonFunctionType.PopupPanel buttonFunction_IN)
    {
        switch (buttonFunction_IN)
        {
            case ButtonFunctionType.PopupPanel.None:
                break;
            case ButtonFunctionType.PopupPanel.Research:
                StimulateWhenPanelExit = false;

                //var researchPopupPanel = parentPanel as ResearchPopupPanel;
                clickRequirementText.text = ResearchPopupPanel.Instance.RequiredAmountText.text;
                buttonFunctionDelegate = gUI_TintScale.TintSize;
                buttonFunctionDelegate += ((ResearchPopupPanel)parentPanel).Research;
                break;
            case ButtonFunctionType.PopupPanel.UnlockRecipe_None:
                StimulateWhenPanelExit = false;

                clickRequirementText.text = NativeHelper.BuildString_Append("X ", UnlockRecipePopupPanel.Instance.activeAmount.ToString());
                buttonFunctionDelegate = DoNothing;
                break;
            case ButtonFunctionType.PopupPanel.UnlockRecipe_Worker:
                StimulateWhenPanelExit = false;

                if (buttonInnerImage_Adressable.color != Color.white) buttonInnerImage_Adressable.color = Color.white;
                //if (imageContainer_Button.color != Color.green) buttonImage.color = Color.green;
                if (clickRequirementText.enabled != true) clickRequirementText.enabled = true;
                buttonInnerImage_Adressable.LoadSprite(ImageManager.SelectSprite(UnlockRecipePopupPanel.Instance.activeWorkerType.Value.ToString()));
                buttonName.text = buttonNames[0];
                clickRequirementText.text = UnlockRecipePopupPanel.Instance.activeWorkerType.ToString();
                buttonFunctionDelegate = gUI_TintScale.TintSize;
                buttonFunctionDelegate += ((UnlockRecipePopupPanel)parentPanel).Unlock;
                break;
            case ButtonFunctionType.PopupPanel.UnlockRecipe_Chest:
                StimulateWhenPanelExit = false;

                if (buttonInnerImage_Adressable.color != Color.blue) buttonInnerImage_Adressable.color = Color.blue;
                //if (buttonImage.color != Color.blue) buttonImage.color = Color.blue;
                if (clickRequirementText.enabled != true) clickRequirementText.enabled = true;
                buttonName.text = buttonNames[1];
                clickRequirementText.text = UnlockRecipePopupPanel.Instance.activeChest.GetName();
                buttonFunctionDelegate = gUI_TintScale.TintSize;
                buttonFunctionDelegate += ((UnlockRecipePopupPanel)parentPanel).Unlock;
                break;
            case ButtonFunctionType.PopupPanel.UnlockReipe_Buy:
                StimulateWhenPanelExit = false;

                if (buttonInnerImage_Adressable.color != Color.yellow) buttonInnerImage_Adressable.color = Color.yellow;
                //if (buttonImage.color != Color.yellow) buttonImage.color = Color.yellow;
                if (clickRequirementText.enabled != false) clickRequirementText.enabled = false;
                buttonName.text = buttonNames[2];
                buttonFunctionDelegate = gUI_TintScale.TintSize;
                buttonFunctionDelegate += ((UnlockRecipePopupPanel)parentPanel).Unlock;
                break;


            #region Confirmation Panel /// <== These buttons do not act as panelExitbuttonsSince they previous panel waits for the confirmations boolean value ==>

            case ButtonFunctionType.PopupPanel.Confirm when parentPanel is ConfirmationPopupPanel:
                StimulateWhenPanelExit = false;

                if (buttonInnerImage_Adressable.enabled != true) buttonInnerImage_Adressable.enabled = true;
                buttonInnerImage_Adressable.LoadSprite(ImageManager.SelectSprite("StarIconYellow"));
                buttonName.text = buttonNames[4];
                if (clickRequirementText.enabled != false) clickRequirementText.enabled = false; // Later to make Twow Stage Confirmation !! 
                buttonFunctionDelegate = gUI_TintScale.TintSize;
                buttonFunctionDelegate += ((ConfirmationPopupPanel)parentPanel).Confirm;
                break;
        
            case ButtonFunctionType.PopupPanel.Reject when parentPanel is ConfirmationPopupPanel:
                StimulateWhenPanelExit = false;

                if (buttonInnerImage_Adressable.enabled != true) buttonInnerImage_Adressable.enabled = true;
                buttonInnerImage_Adressable.LoadSprite(ImageManager.SelectSprite("StarIconRed"));
                buttonName.text = buttonNames[3];
                if (clickRequirementText.enabled != false) clickRequirementText.enabled = false;
                buttonFunctionDelegate = gUI_TintScale.TintSize;
                buttonFunctionDelegate += ((ConfirmationPopupPanel)parentPanel).Reject;
                break;
            #endregion

            #region DeleteItempopupPanel // ACCEPT or REJECT ITEM DELETION // Buttons

            case ButtonFunctionType.PopupPanel.Confirm when parentPanel is DeleteItemPopupPanel:
                StimulateWhenPanelExit = false;

                if (buttonInnerImage_Adressable.enabled != true) buttonInnerImage_Adressable.enabled = true;
                buttonInnerImage_Adressable.LoadSprite(ImageManager.SelectSprite("StarIconYellow"));
                buttonName.text = buttonNames[4];
                if (clickRequirementText.enabled != false) clickRequirementText.enabled = false; // Later to make Twow Stage Confirmation !! 
                buttonFunctionDelegate = gUI_TintScale.TintSize;
                buttonFunctionDelegate += ((DeleteItemPopupPanel)parentPanel).ConfirmDelete;
                break;
            case ButtonFunctionType.PopupPanel.Reject when parentPanel is DeleteItemPopupPanel:
                StimulateWhenPanelExit = true;

                if (buttonInnerImage_Adressable.enabled != true) buttonInnerImage_Adressable.enabled = true;
                buttonInnerImage_Adressable.LoadSprite(ImageManager.SelectSprite("StarIconRed"));
                buttonName.text = buttonNames[3];
                if (clickRequirementText.enabled != false) clickRequirementText.enabled = false;
                buttonFunctionDelegate = () =>
                {
                    gUI_TintScale.TintSize();
                    PanelManager.DeactivatePanel(PanelManager.SelectedPanels.Peek(), nextPanelLoadAction_IN: null);
                };
                //gUI_TintScale.TintSize;
                //buttonFunctionDelegate +=  //((DeleteItemPopupPanel)parentPanel).RejectDelete;
                break;
            #endregion

            #region EnhancePanel // ACCEPT or REJECT REPLACE ENHANCEMENT // Buttons

            case ButtonFunctionType.PopupPanel.Confirm when parentPanel is EnhanceItemPopupPanel enhanceItemPopupPanel:
                StimulateWhenPanelExit = false;

                if (buttonInnerImage_Adressable.enabled != true) buttonInnerImage_Adressable.enabled = true;
                buttonInnerImage_Adressable.LoadSprite(ImageManager.SelectSprite("StarIconYellow"));
                buttonName.text = buttonNames[4];
                if (clickRequirementText.enabled != false) clickRequirementText.enabled = false; // Later to make Twow Stage Confirmation !! 

                buttonFunctionDelegate = async () =>
                {
                    var enhanceable = (IEnhanceable)enhanceItemPopupPanel.bluePrint;
                    var enhancement = enhanceItemPopupPanel.Enhancement;
                    gUI_TintScale.TintSize();
                    var isOverEnhanceConfirmed = await Enhance.IsOverEnhanceConfirmedAsync(
                            enhanceable_IN: enhanceable,
                            enhancement_IN: enhancement,
                            amountToEnhance_IN: enhanceItemPopupPanel.CounterObject.CurrentAmount);

                    if (isOverEnhanceConfirmed)
                    {
                        //enhanceable = enhanceable.DestroyEnhancement(enhancement); /// reset the enhanceable to sync it with gameiteminfo selection after enhance                        
                        enhanceItemPopupPanel.PerformEnhancementAndDisplayModal();
                    }

                    else PanelManager.DeactivatePanel(PanelManager.SelectedPanels.Peek(), nextPanelLoadAction_IN: null,
                       unloadAction: () => PanelManager.RemoveFromNavigationStack_Until(typeof(GameItemInfoPanel_Manager)));
                };

                //buttonFunctionDelegate = gUI_TintScale.TintSize;
                //buttonFunctionDelegate += ((EnhanceItemPopupPanel)parentPanel).PerformEnhancementAndDisplayModal;
                break;
            case ButtonFunctionType.PopupPanel.Reject when parentPanel is EnhanceItemPopupPanel enhanceItemPopupPanel:
                StimulateWhenPanelExit = true;

                if (buttonInnerImage_Adressable.enabled != true) buttonInnerImage_Adressable.enabled = true;
                buttonInnerImage_Adressable.LoadSprite(ImageManager.SelectSprite("StarIconRed"));
                buttonName.text = buttonNames[3];
                if (clickRequirementText.enabled != false) clickRequirementText.enabled = false;

                buttonFunctionDelegate = () =>
                {                   
                    var invokablePanel = PanelManager.InvokablePanels[typeof(InventoryPanel_Manager)];
                    var inventoryPanelManager = (InventoryPanel_Manager)invokablePanel.MainPanel;
                    var enhancementPopupPanel = (EnhanceItemPopupPanel)parentPanel;
                    var selectedItemEnhancementType = enhancementPopupPanel.Enhancement.GetEnhancementType();

                    gUI_TintScale.TintSize();
                    PanelManager.DeactivatePanel(
                        PanelManager.SelectedPanels.Peek(),
                        nextPanelLoadAction_IN: null,
                        unloadAction: () =>
                        {
                            inventoryPanelManager.ReassignPanelLayout(
                                      mainTypeSelection_IN: GameItemType.Type.Enhancement,
                                      subtypeSelection_IN: selectedItemEnhancementType,
                                      assignedState_IN: IReassignablePanel.AssignedState.Inventory_FromProductToEnhance);
                            enhancementPopupPanel.LoadPanel(new PopupPanel_Enhancement_LoadData(
                                mainLoadInfo: enhanceItemPopupPanel.bluePrint,
                                panelHeader: null,
                                enhancement_IN: ((IEnhanceable)enhanceItemPopupPanel.bluePrint).enhancementsDict_ro[selectedItemEnhancementType], ///enhanceItemPopupPanel.Enhancement,
                                tcs_IN: null,
                                panelState_IN: null)); /// Reverse the state of the enhancementPanel back to the Destroy and Replace State manually ///

                });  // () => PanelManager.RemoveFromNavigationStack_Until(typeof(TabPanel_GameItemEnhancements)));
                                
                };
               


                



                //buttonFunctionDelegate = gUI_TintScale.TintSize;
                //buttonFunctionDelegate += ((DeleteItemPopupPanel)parentPanel).RejectDelete;
                break;
            #endregion

            #region MissingRequirements Panel Buttons

            case ButtonFunctionType.PopupPanel.MissingRequirements_RefillByToken:
                StimulateWhenPanelExit = false;

                if (buttonInnerImage_Adressable.enabled != true) buttonInnerImage_Adressable.enabled = true;
                buttonInnerImage_Adressable.LoadSprite(ImageManager.SelectSprite("TokenIcon"));
                buttonName.text = buttonNames[5];

                var mrPanelA = ((MissingRequirementsPopupPanel)parentPanel);
                if (clickRequirementText.enabled != true) clickRequirementText.enabled = true;
                buttonValue = ((Ingredient)mrPanelA.bluePrint).CalculateRefillCost(mrPanelA.RequiredAmounts[mrPanelA.CurrentIndice], new Gold());
                clickRequirementText.text = buttonValue.ToString();
                
                buttonFunctionDelegate = gUI_TintScale.TintSize;
                buttonFunctionDelegate += () => mrPanelA.RefillIngredient(new Gold(buttonValue));
                break;

            case ButtonFunctionType.PopupPanel.MissingRequirements_RefillByGems:
                StimulateWhenPanelExit = false;

                if (buttonInnerImage_Adressable.enabled != true) buttonInnerImage_Adressable.enabled = true;
                buttonInnerImage_Adressable.LoadSprite(ImageManager.SelectSprite("GemIcon"));
                buttonName.text = buttonNames[6];

                var mrPanelB = ((MissingRequirementsPopupPanel)parentPanel);
                if (clickRequirementText.enabled != true) clickRequirementText.enabled = true;
                buttonValue = ((Ingredient)mrPanelB.bluePrint).CalculateRefillCost(mrPanelB.RequiredAmounts[mrPanelB.CurrentIndice], new Gem());
                clickRequirementText.text = buttonValue.ToString();

                buttonFunctionDelegate = gUI_TintScale.TintSize;
                buttonFunctionDelegate += () => mrPanelB.RefillIngredient(new Gem(buttonValue));
                break;

            case ButtonFunctionType.PopupPanel.MissingRequirements_BuyFromMarket:
                StimulateWhenPanelExit = false;

                if (buttonInnerImage_Adressable.enabled != false) buttonInnerImage_Adressable.enabled = false;
                buttonName.text = buttonNames[2];
                if (clickRequirementText.enabled != false) clickRequirementText.enabled = false;

                buttonFunctionDelegate = gUI_TintScale.TintSize;
                buttonFunctionDelegate += DoNothing;
                break;

            case ButtonFunctionType.PopupPanel.MissingRequirements_GoToQuestArea:
                StimulateWhenPanelExit = false;

                if (buttonInnerImage_Adressable.enabled != false) buttonInnerImage_Adressable.enabled = false;
                buttonName.text = buttonNames[7];
                if (clickRequirementText.enabled != false) clickRequirementText.enabled = false;

                buttonFunctionDelegate = gUI_TintScale.TintSize;
                buttonFunctionDelegate += DoNothing;
                break;

            case ButtonFunctionType.PopupPanel.MissingRequirements_CraftProduct:
                StimulateWhenPanelExit = false;

                if (buttonInnerImage_Adressable.enabled != false) buttonInnerImage_Adressable.enabled = false;
                buttonName.text = buttonNames[8];
                if (clickRequirementText.enabled != false) clickRequirementText.enabled = false;


                buttonFunctionDelegate = gUI_TintScale.TintSize;
                buttonFunctionDelegate += ((MissingRequirementsPopupPanel)parentPanel).TryOpenCraftPanel;  //delegate { InvokePanel(invokablePanelIndex_IN: 0); };               //() => InvokePanel(invokablePanelIndex_IN: 0);         
                break;

            case ButtonFunctionType.PopupPanel.MissingRequirements_ActivateIngredientGenerator:
                StimulateWhenPanelExit = false;

                if (buttonInnerImage_Adressable.enabled != false) buttonInnerImage_Adressable.enabled = false;
                buttonName.text = buttonNames[9];
                if (clickRequirementText.enabled != false) clickRequirementText.enabled = false;

                var castedParentPanel = (MissingRequirementsPopupPanel)parentPanel;
                var selectedIngredient = parentPanel.bluePrint as Ingredient;
                buttonFunctionDelegate = gUI_TintScale.TintSize;
                buttonFunctionDelegate += () => IngredientGeneratorsManager.Instance.ActivateGenerator(selectedIngredient.IngredientType);       // Activate the Relevant Ingredient Generator   
                buttonFunctionDelegate += () => castedParentPanel.BrowseInfo(castedParentPanel.ListToIterate[castedParentPanel.CurrentIndice]);  // Reload the popup Panel
                break;

            case ButtonFunctionType.PopupPanel.MissingRequirements_BuyShopUpgrade:
                StimulateWhenPanelExit = false;

                if (buttonInnerImage_Adressable.enabled != false) buttonInnerImage_Adressable.enabled = false;
                buttonName.text = buttonNames[10];
                if (clickRequirementText.enabled != false) clickRequirementText.enabled = false;

                buttonFunctionDelegate = gUI_TintScale.TintSize;
                buttonFunctionDelegate += ((MissingRequirementsPopupPanel)parentPanel).OpenShopPanel;

                break;
            case ButtonFunctionType.PopupPanel.MissingRequirements_UpgradeShopUpgrade:
                StimulateWhenPanelExit = false;

                if (buttonInnerImage_Adressable.enabled != false) buttonInnerImage_Adressable.enabled = false;
                buttonName.text = buttonNames[11];
                if (clickRequirementText.enabled != false) clickRequirementText.enabled = false;

                buttonFunctionDelegate = gUI_TintScale.TintSize;
                buttonFunctionDelegate += ((MissingRequirementsPopupPanel)parentPanel).OpenShopPanel;

                break;

            case ButtonFunctionType.PopupPanel.MissingRequirements_UpgradeResourceCabinet:
                StimulateWhenPanelExit = false;

                if (buttonInnerImage_Adressable.enabled != false) buttonInnerImage_Adressable.enabled = false;
                buttonName.text = buttonNames[11];
                if (clickRequirementText.enabled != false) clickRequirementText.enabled = false;

                buttonFunctionDelegate = gUI_TintScale.TintSize;
                buttonFunctionDelegate += ((MissingRequirementsPopupPanel)parentPanel).UpgradeResourceContainer;
                break;

            case ButtonFunctionType.PopupPanel.MissingRequirements_None:
                StimulateWhenPanelExit = false;
                buttonName.text = "Okay";
                if (clickRequirementText.enabled != false) clickRequirementText.enabled = false;

                buttonFunctionDelegate = gUI_TintScale.TintSize;
                buttonFunctionDelegate += () => PanelManager.DeactivatePanel(PanelManager.SelectedPanels.Peek(), nextPanelLoadAction_IN: null);
                break;
            case ButtonFunctionType.PopupPanel.MissingRequirements_WorkerNotHired:
                StimulateWhenPanelExit = false;
                buttonName.text = "Hire";
                if (clickRequirementText.enabled != false) clickRequirementText.enabled = false;

                buttonFunctionDelegate = gUI_TintScale.TintSize;
                buttonFunctionDelegate += () =>
                {             
                    var selectedWorker = ((MissingRequirementsPopupPanel)parentPanel).bluePrint as Worker;
                    var panelLoadData = new PanelLoadData(mainLoadInfo: selectedWorker, panelHeader: null, tcs_IN: null);

                    PanelManager.ActivateAndLoad(invokablePanel_IN: PanelManager.InvokablePanels[typeof(HireCharacter_Panel)],
                                                 //preLoadAction_IN: ()=> PanelManager.RemoveCurrentPanelFromNavigationStackIf(removeConditions: ipc => ipc.MainPanel is MissingRequirementsPopupPanel msp && !msp.UpdateAndMoveToNextWorker()),
                                                 panelLoadAction_IN: () => HireCharacter_Panel.Instance.LoadPanel(panelLoadData));
                };
                break;

            #endregion

            #region EnhancePanel // Destroy or Replace Selection For Occupied Enhancement Slos // Buttons

            case ButtonFunctionType.PopupPanel.EnhancementPanel_DestroyEnhancement:
                StimulateWhenPanelExit = false;

                if (buttonInnerImage_Adressable.enabled != false) buttonInnerImage_Adressable.enabled = false;
                buttonName.text = buttonNames[12];
                if (buttonImage_Adressable.color != Color.red) buttonImage_Adressable.color = Color.red;

                buttonFunctionDelegate = async () =>
                {
                    var enhanceItemPopupPanel = (EnhanceItemPopupPanel)parentPanel;
                    var selectedEnhanceable = (IEnhanceable)enhanceItemPopupPanel.bluePrint;
                    var selectedEnhancement = enhanceItemPopupPanel.Enhancement;

                    gUI_TintScale.TintSize();
                    var isDestroyconfirmed = await Enhance.IsDestroyEnhancementConfirmed(
                            enhanceable_IN: selectedEnhanceable,
                            enhancement_IN: selectedEnhancement);
                    if (isDestroyconfirmed)
                    {
                        selectedEnhanceable.DestroyEnhancement(selectedEnhancement);  
                        PanelManager.DeactivatePanel(PanelManager.SelectedPanels.Peek(), nextPanelLoadAction_IN:null,
                            unloadAction: () => PanelManager.RemoveFromNavigationStack_Until(typeof(GameItemInfoPanel_Manager)));
                    }
                    else
                    {
                        PanelManager.DeactivatePanel(PanelManager.SelectedPanels.Peek(), unloadAction: null, nextPanelLoadAction_IN: null);
                    }

                };

                break;

            case ButtonFunctionType.PopupPanel.EnhancemenPanel_ReplaceEnhancement:
                StimulateWhenPanelExit = false;

                if (buttonInnerImage_Adressable.enabled != false) buttonInnerImage_Adressable.enabled = false;
                buttonName.text = buttonNames[13];
                if (buttonImage_Adressable.color != Color.blue) buttonImage_Adressable.color = Color.blue;

                buttonFunctionDelegate = () =>
                {
                    var invokablePanel = PanelManager.InvokablePanels[typeof(InventoryPanel_Manager)];
                    var inventoryPanelManager = (InventoryPanel_Manager)invokablePanel.MainPanel;
                    
                    gUI_TintScale.TintSize();
                    inventoryPanelManager.ReassignPanelLayout(
                                            mainTypeSelection_IN: GameItemType.Type.Enhancement, 
                                            subtypeSelection_IN:((EnhanceItemPopupPanel)parentPanel).Enhancement.GetEnhancementType(),
                                            assignedState_IN: IReassignablePanel.AssignedState.Inventory_FromProductToEnhance);
                    PanelManager.ActivateAndLoad(
                                            invokablePanel_IN: invokablePanel,
                                            panelLoadAction_IN: null);
                };

                break;
            #endregion

            #region EnhancePanel // Enhance or Change Selection For EMpty Enhance SLots // Buttons

            case ButtonFunctionType.PopupPanel.EnhancementPanel_ChangeSelection:
                StimulateWhenPanelExit = true;

                if (buttonImage_Adressable.enabled != false) buttonInnerImage_Adressable.enabled = false;
                buttonName.text = buttonNames[14];
                if (clickRequirementText.enabled != false) clickRequirementText.enabled = false;
                if (buttonImage_Adressable.color != Color.white) buttonImage_Adressable.color = Color.white;

                buttonFunctionDelegate = () =>
                {
                    gUI_TintScale.TintSize();
                    PanelManager.DeactivatePanel(PanelManager.SelectedPanels.Peek(), 
                        nextPanelLoadAction_IN: null,
                        unloadAction: PanelManager.SelectedPanels.ElementAt(PanelManager.SelectedPanels.Count - 1).MainPanel is IReassignablePanel reassignable
                            ? () => reassignable.ReassignPanelLayout(GameItemType.Type.Enhancement, 
                                                                    ((EnhanceItemPopupPanel)parentPanel).Enhancement.GetEnhancementType(),
                                                                    (IReassignablePanel.AssignedState)((EnhanceItemPopupPanel)parentPanel).InventoryPanelStateToRevert)
                            : null);
                };
                break;

            /*case ButtonFunctionType.PopupPanel.EnhancementPanel_ChangeSelection_FromEnhancement:
                StimulateWhenPanelExit = true;

                if (buttonImage.ImageContainer.enabled != false) buttonInnerImage.ImageContainer.enabled = false;
                buttonName.text = buttonNames[14];
                if (clickRequirementText.enabled != false) clickRequirementText.enabled = false;
                if (buttonImage.ImageContainer.color != Color.white) buttonImage.ImageContainer.color = Color.white;

                buttonFunctionDelegate = () =>
                {
                    gUI_TintScale.TintSize();
                    PanelManager.DeactivatePanel(PanelManager.SelectedPanels.Peek(),
                        nextPanelLoadAction_IN: null,
                        unloadAction: PanelManager.SelectedPanels.ElementAt(PanelManager.SelectedPanels.Count - 1).MainPanel is IReassignablePanel reassignable
                            ? () => reassignable.ReassignPanelLayout(GameItemType.Type.Product, ((EnhanceItemPopupPanel)parentPanel).Enhancement.GetEnhancementType(), IReassignablePanel.AssignedState.Inventory_FromEnhanceToProduct)
                            : null);
                };

                break;*/

            case ButtonFunctionType.PopupPanel.EnhancementPanel_Enhance:
                StimulateWhenPanelExit = false;

                var EIpanel = (EnhanceItemPopupPanel)parentPanel;
                var enhanceable = (IEnhanceable)EIpanel.bluePrint;
                var enhanceSuccessRatio = Enhance.EnhanceSuccessRatio(enhanceable, EIpanel.Enhancement);
                var buttonColor = NativeHelper.GetSuccessRatioColor(enhanceSuccessRatio);

                if (buttonImage_Adressable.enabled != false) buttonInnerImage_Adressable.enabled = false;
                buttonName.text = buttonNames[15];
                if (clickRequirementText.enabled != true) clickRequirementText.enabled = true;
                clickRequirementText.text = NativeHelper.BuildString_Append(Enhance.EnhanceSuccessRatio(enhanceable, EIpanel.Enhancement).ToString(), "% Chance");

                if (buttonImage_Adressable.color != buttonColor) buttonImage_Adressable.color = buttonColor;
                buttonFunctionDelegate = async () =>
                {
                    gUI_TintScale.TintSize();

                    var confirmOverEnhance = await Enhance.IsOverEnhanceConfirmedAsync(enhanceable_IN: EIpanel.bluePrint as IEnhanceable,
                                                                                       enhancement_IN: EIpanel.Enhancement,
                                                                                       amountToEnhance_IN: EIpanel.CounterObject.CurrentAmount); //EIpanel.IsOverEnhanceConfirmed();
                        
                    if (confirmOverEnhance)
                    {
                        ((EnhanceItemPopupPanel)parentPanel).PerformEnhancementAndDisplayModal();
                    }
                    else
                    {
                        PanelManager.DeactivatePanel(PanelManager.SelectedPanels.Peek(),
                            unloadAction: () =>
                            {
                                PanelManager.RemoveFromNavigationStack_Until(typeof(InventoryPanel_Manager));
                                var currentPanel = (InventoryPanel_Manager)PanelManager.SelectedPanels.Peek().MainPanel;
                                currentPanel.ReassignPanelLayout(GameItemType.Type.Enhancement, 
                                                                 EIpanel.Enhancement.GetEnhancementType(),
                                                                 (IReassignablePanel.AssignedState)((EnhanceItemPopupPanel)parentPanel).InventoryPanelStateToRevert );
                            }, 
                            nextPanelLoadAction_IN: null);
                    }
                };

                break;
            #endregion

            #region Progress Panel
            case ButtonFunctionType.PopupPanel.ProgressPanel_RushByEnergy:
                StimulateWhenPanelExit = false;

                var progressPanel = (ProgressPopupPanel)parentPanel;
                var rushableWithEnergy = (IRushableWithEnergy)progressPanel.ListToIterate[progressPanel.CurrentIndice];


                buttonValue = rushableWithEnergy.GetCurrentRushCostEnergy;
                buttonInnerImage_Adressable.LoadSprite(ImageManager.SelectSprite("EnergyIcon"));
                if (buttonImage_Adressable.color != Color.yellow) buttonImage_Adressable.color = Color.yellow;

                buttonFunctionDelegate = () =>
                {
                    gUI_TintScale.TintSize();
                    if (StatsData.Instance.TrySetEnergy(-buttonValue))
                    {
                        rushableWithEnergy.Rush();
                    }
                    else
                    {
                        Debug.Log("you dont have eough Energy to do that");
                    }
                };
                break;

            case ButtonFunctionType.PopupPanel.ProgressPanel_RushByGem:
                StimulateWhenPanelExit = false;

                var PGPanel = (ProgressPopupPanel)parentPanel;
                var rushablewithGem = PGPanel.ListToIterate[PGPanel.CurrentIndice];

                buttonValue = rushablewithGem.GetCurrentRushCostGem;
                buttonInnerImage_Adressable.LoadSprite(ImageManager.SelectSprite("GemIcon"));
                if (buttonImage_Adressable.color != Color.blue) buttonImage_Adressable.color = Color.blue;

                buttonFunctionDelegate = () =>
                {
                    gUI_TintScale.TintSize();
                    if (StatsData.IsSpendableAmountEnough(buttonValue, new Gem()))
                    {
                        StatsData.SetSpendableValue(new Gem(), -buttonValue);
                        rushablewithGem.Rush();
                    }
                    else
                    {
                        Debug.Log("you dont have eough Gems to do that");
                    }
                };
                break;
            case ButtonFunctionType.PopupPanel.ProgressPanel_BackToShop:
                StimulateWhenPanelExit = false;

                var panel = (ProgressPopupPanel)parentPanel;
                var selectedItem = panel.ListToIterate[panel.CurrentIndice];

                buttonValue = 0;
                clickRequirementText.text = "Collect!";
                buttonInnerImage_Adressable.LoadSprite(ImageManager.SelectSprite("StarIconYellow"));
                if (buttonImage_Adressable.color != Color.green) buttonImage_Adressable.color = Color.green;

                buttonFunctionDelegate = () =>
                {
                    gUI_TintScale.TintSize();

                    /*switch (selectedItem)
                    {
                        case Single_CraftedItem single_CraftedItem:
                            Radial_CraftSlots_Crafter.Instance.ReclaimCrafted(single_CraftedItem);
                            break;
                        default:
                            DoNothing();
                            break;
                    }*/
                    PanelManager.DeactivatePanel(PanelManager.SelectedPanels.Peek(),                                                 
                                                 unloadAction: selectedItem switch
                                                 {
                                                     Single_CraftedItem single_CraftedItem => () => Radial_CraftSlots_Crafter.Instance.ReclaimCrafted(single_CraftedItem),
                                                     WorkStationUpgrade workStationUpgrade => () => workStationUpgrade.LevelUp(),
                                                     _ => () => Debug.LogWarning("typeofselecteditem " + selectedItem.GetType()),
                                                 },
                                                 nextPanelLoadAction_IN: null);
                };
                break;
                #endregion
        }
    }

    public override void UnloadButton()
    {
        base.UnloadButton();
        clickRequirementText.text = null;
        //parentPanel = null;
    }

    public void AssignPanel(PopupPanel parentPanel_IN)
    {
        parentPanel = parentPanel_IN;
    }


}

