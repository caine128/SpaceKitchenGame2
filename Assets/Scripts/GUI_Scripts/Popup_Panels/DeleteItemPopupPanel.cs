using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class DeleteItemPopupPanel : PopupPanel_Single_SNG<DeleteItemPopupPanel>, ISinglePanelInvokeButton, IQuickUnloadable
{
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private ContentDisplayIngredient_DeletePopup[] ingredientContentDisplays;

    public CounterObjectScript CounterObject => counterObject;
    [SerializeField] private CounterObjectScript counterObject;
    public InvokablePanelController PanelToInvoke => panelToInvoke;
    [SerializeField] private InvokablePanelController panelToInvoke;




    private const string DEFAULTPOPUPHEADER = "Dismantling";
    private const string DESCRIPTIONTEXT = "Dismantle";

    private TaskCompletionSource<bool> tcs;


    public sealed override void LoadPanel(PanelLoadData panelLoadData)
    {
        base.LoadPanel(panelLoadData);
        var productRecipe = ((ICraftable)bluePrint).GetProductRecipe();
        
        ingredientContentDisplays.PlaceContainers(productRecipe.recipeSpecs.requiredIngredients.Length, ingredientContentDisplays[0].RT.rect.height, isHorizontalPlacement: false);
        ingredientContentDisplays.LoadContainers(bluePrint, productRecipe.recipeSpecs.requiredIngredients.Length, hideAtInit: true);

        counterObject.OncounterModified += ModifyIngredientCurrentAmountModifier;
        counterObject.Initialize((IAmountable)bluePrint);
        
        descriptionText.text = NativeHelper.BuildString_Append(DESCRIPTIONTEXT, " ", bluePrint.GetName(), " ?");

        popupButtons[0].SetupButton(ButtonFunctionType.PopupPanel.Reject);
        popupButtons[1].SetupButton(ButtonFunctionType.PopupPanel.Confirm);
    }

    public sealed override void DisplayContainers()
    {
        base.DisplayContainers();
        ingredientContentDisplays.SortContainers(customInitialValues:null,
                                                 secondaryInterpolations:null,
                                                 amountToSort_IN: ((ICraftable)bluePrint).GetProductRecipe().recipeSpecs.requiredIngredients.Length, 
                                                 enumeratorIndex: 0,
                                                 parentPanel_IN: this,
                                                 lerpSpeedModifiers: null);        
    }

    public async void ConfirmDelete()
    {
        if ((bluePrint is IQualitative qualitativeItem && qualitativeItem.GetQuality() != Quality.Level.Normal) || (bluePrint is IEnhanceable enhanceableItem && enhanceableItem.isEnhanced == true))
        {
            var itemQuality = (bluePrint as IQualitative).GetQuality();

            tcs = new TaskCompletionSource<bool>();

            if (this.panelToInvoke.MainPanel is ConfirmationPopupPanel)
            {

                var panelLoadData = new PopupPanel_Confirmation_LoadData(
                    mainLoadInfo: bluePrint,
                    panelHeader: null,
                    tcs_IN: tcs,
                    bluePrintsToLoad: new List<(GameObject, int)>
                       { (bluePrint as GameObject, CounterObject.CurrentAmount)},
                    extraDescription_IN: NativeHelper.BuildString_Append("Are you sure do you want to delete ",
                        itemQuality != Quality.Level.Normal
                            ? MethodHelper.GiveRichTextString_Color(NativeHelper.GetQualityColor(itemQuality)) + MethodHelper.GiveRichTextString_Size(125)
                            : string.Empty,
                        bluePrint.GetName(),
                        itemQuality != Quality.Level.Normal
                            ? MethodHelper.GiveRichTextString_ClosingTagOf("color") + MethodHelper.GiveRichTextString_ClosingTagOf("size")
                            : string.Empty, " ?",
                        Environment.NewLine,
                        "This is a ",
                        itemQuality != Quality.Level.Normal
                            ? itemQuality.ToString()
                            : string.Empty,
                        (bluePrint as IEnhanceable)?.isEnhanced == true
                            ? " Enhanced"
                            : string.Empty, " item"));


                var invokablePanel_Confirmation = PanelManager.InvokablePanels[typeof(ConfirmationPopupPanel)];
                //Later to take this chec in the if statement or maybe even better without an if statement !!!!
                //Also CAN GET RID OF THE SUCKY CAST BELOW !!
                //Also can get rid of the invokablepanel Varibales Totally!
                PanelManager.ActivateAndLoad(
                    invokablePanel_IN: invokablePanel_Confirmation,  //panelToInvoke
                    panelLoadAction_IN: () => ((ConfirmationPopupPanel)invokablePanel_Confirmation.MainPanel).LoadPanel(panelLoadData));
                //() => ConfirmationPopupPanel.Instance.LoadPanel<GameItem>(bluePrint,
                //    new List<(GameItem, int)> { (bluePrint as GameItem, CounterObject.CurrentAmount) },
                //    tcs)); ;
            } // 

            await tcs.Task;
            ///Deactivate the confimation beucase it doesn't have self deactivation.
            if (PanelManager.SelectedPanels.Peek().MainPanel is ConfirmationPopupPanel confirmationPanel)
            {
                PanelManager.DeactivatePanel(PanelManager.SelectedPanels.Peek(), nextPanelLoadAction_IN: null, unloadAction: null);
            }
            if (tcs.Task.Result == false) return;
        }



        //for (int i = 0; i < ((ICraftable)bluePrint).GetProductRecipe().recipeSpecs.requiredIngredients.Length; i++)
        //{
        //    ResourcesManager.Instance.AddIngredient(ingredientContentDisplays[i].ContentType, ingredientContentDisplays[i].RecycledAmount, bypassMaxCap: true);
        //}

        foreach (var contentDisplay in ingredientContentDisplays)
        {
            if(contentDisplay.ContentType.HasValue) 
                ResourcesManager.Instance.AddIngredient((IngredientType.Type)contentDisplay.ContentType, contentDisplay.RecycledAmount, bypassMaxCap: true);
        }

        Inventory.Instance.RemoveFromInventory((GameObject)bluePrint, CounterObject.CurrentAmount);

        while (PanelManager.SelectedPanels.Peek().MainPanel != this)
        {
            await Task.Delay(50);
        }


        var (nextPanelLoadACtion, unloadAction) = GetNextItemAfterProcess(GameItemInfoPanel_Manager.Instance, (GameObject)bluePrint);
        PanelManager.DeactivatePanel(PanelManager.SelectedPanels.Peek(), nextPanelLoadAction_IN: nextPanelLoadACtion, unloadAction: unloadAction);


        var modalLoadData = new ModalPanel_DismantleItem_LoadData(
                                                                  craftableItem_IN: (ICraftable)bluePrint,
                                                                  dismantleAmount_IN:CounterObject.CurrentAmount,
                                                                  recycledResources_IN: ingredientContentDisplays.Where(x => x.ContentType.HasValue)
                                                                                                                 .ExtractEnumerableOfTuples(
                                                                          x => (AssetReferenceT<Sprite>)ResourcesManager.Instance.Resources_SO.ingredients[(int)x.ContentType].spriteRef,
                                                                          x => x.RecycledAmount),
                                                                  modalState_IN: Information_Modal_Panel.ModalState.ItemDismantle);

        var panelToInvoke = PanelManager.InvokablePanels[typeof(Information_Modal_Panel)];
        //var isPanelAlreadyActive = PanelManager.SelectedPanels.TryPeek(out InvokablePanelController activePanel) && activePanel.MainPanel == invokablePanel_Modal.MainPanel;

        //if (!isPanelAlreadyActive)
        //{
            PanelManager.ActivateAndLoad(invokablePanel_IN: panelToInvoke,
                                             panelLoadAction_IN: () => Information_Modal_Panel.Instance.LoadModalQueue(modalLoadData_IN: modalLoadData,
                                                                                                                       modalLoadDatas: Enumerable.Empty<ModalLoadData>()),
                                             alternativeLoadAction_IN: () =>
                                             {
                                                 var panel = (Information_Modal_Panel)panelToInvoke.MainPanel;
                                                 panel.ModalLoadDataQueue.Enqueue(modalLoadData);
                                             });
        //}
        /*else
        {
            var panel = ((Information_Modal_Panel)activePanel.MainPanel);
            panel.ModalLoadDataQueue.Enqueue(modalLoadData);
        }*/
    }

    private (Action nextPanelLoadACtion, Action unloadAction) GetNextItemAfterProcess<T_BlueprintType>(IBrowsablePanel<T_BlueprintType> browsablePanel,
                                                                                                       T_BlueprintType amountableItem)
        where T_BlueprintType : SortableBluePrint, IAmountable

        => amountableItem.GetAmount() switch
        {
            > 0 => (() => browsablePanel.BrowseInfo(browsablePanel.ListToIterate[browsablePanel.CurrentIndice]), null),
            <= 0 => browsablePanel.ListToIterate.Count switch
            {
                <= 0 => (null,
                                () =>
                                {
                                    PanelManager.BottomBarsController.PlaceBars();
                                    PanelManager.CraftWheelController.PlaceBars();
                                    PanelManager.ClearStackAndDeactivateElements();
                                }
                ),
                > 0 => (() => browsablePanel.BrowseInfo(browsablePanel.ListToIterate[0]), null),
            },
        };


    protected override string DefaultPopupHeader()
    {
        return NativeHelper.BuildString_Append(DEFAULTPOPUPHEADER + " " + bluePrint.GetName());
    }

    public void ModifyIngredientCurrentAmountModifier()
    {
        foreach (var ingredientContentDisplay in ingredientContentDisplays)
        {
            if (ingredientContentDisplay.gameObject.activeInHierarchy == true)
            {
                ingredientContentDisplay.ModifyOnModifierChange();
            }
        }
    }

    public void QuickUnload()
    {
        if(!PanelManager.SelectedPanels.Any(sp => sp.MainPanel == this))
        {
            foreach (var ingredientDisplay in ingredientContentDisplays)
            {
                ingredientDisplay.RemoveSubscriptions();
            }
            counterObject.OncounterModified -= ModifyIngredientCurrentAmountModifier;
        }   
    }

    public sealed override void UnloadAndDeallocate()
    {      
        foreach (var ingredientDisplay in ingredientContentDisplays)
        {
            ingredientDisplay.Unload();
        }

        base.UnloadAndDeallocate();
    }


}
