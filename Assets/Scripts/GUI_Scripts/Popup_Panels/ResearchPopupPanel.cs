
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResearchPopupPanel : PopupPanel_Single_SNG<ResearchPopupPanel>
{
    [SerializeField] private RectTransform subTypeIconRect;
    [SerializeField] private Image subTypeIcon;

    [SerializeField] private RectTransform subHeaderTextRect;
    [SerializeField] private TextMeshProUGUI subHeaderText;

    [SerializeField] private TextMeshProUGUI ownedAmountText;
    public TextMeshProUGUI RequiredAmountText { get { return _requiredAmountText; } }
    [SerializeField] private TextMeshProUGUI _requiredAmountText;
    [SerializeField] private TextMeshProUGUI balanceAmountText;

    private string defaultPopupHeader = "Blueprint Research";


    int? ownedAmount = null;
    int? requiredAmount = null;

    public sealed override void LoadPanel(PanelLoadData panelLoadData)
    {
        base.LoadPanel(panelLoadData);
        subHeaderText.text = bluePrint.GetName();

        GUI_CentralPlacement.PlaceImageWithText(subHeaderText, subHeaderTextRect, subTypeIconRect, true);


        SetResearchScrollAmounts();
        popupButtons[0].SetupButton(ButtonFunctionType.PopupPanel.Research);

    }

    protected override string DefaultPopupHeader()
    {
        return defaultPopupHeader;
    }

    private void SetResearchScrollAmounts()
    {
        var productRecipe = bluePrint as ProductRecipe;

        ownedAmount = Inventory.Instance.CheckAmountInInventory_Name(SpecialItemsManager.Instance.Keys_Shards_Scrolls_SO.researchScrollInfo.name, GameItemType.Type.SpecialItem);
        requiredAmount = productRecipe.recipeSpecs.researchPointsRequired;

        ownedAmountText.text = ownedAmount.ToString();
        _requiredAmountText.text = requiredAmount.ToString();
        balanceAmountText.text = (ownedAmount - requiredAmount).ToString();
    }

    public void Research()
    {
        var productRecipe = bluePrint as ProductRecipe;
        var researchScroll = new ResearchScroll(SpecialItemType.Type.ResearchScroll);

        if (Inventory.Instance.RemoveFromInventory(researchScroll, (int)requiredAmount))
        {
            productRecipe.Research();

            ModalPanel_DisplayBonuses_ProductRecipeUnlockOrResearch modalLoadData = 
                 new(productRecipe: productRecipe, modalState_IN: Information_Modal_Panel.ModalState.ProductRecipe_UnlockedOrResearched);
            var panelToInvoke = PanelManager.InvokablePanels[typeof(Information_Modal_Panel)];

            PanelManager.ActivateAndLoad(invokablePanel_IN: panelToInvoke,
                                         preLoadAction_IN: () => PanelManager.RemoveCurrentPanelFromNavigationStackIf(removeConditions: ipc => ipc.MainPanel is ResearchPopupPanel),
                                         panelLoadAction_IN: () => Information_Modal_Panel.Instance.LoadModalQueue(modalLoadData_IN: modalLoadData,
                                                                                                                 modalLoadDatas: Enumerable.Empty<ModalLoadData>()),
                                         alternativeLoadAction_IN: () =>
                                         {
                                             var panel = (Information_Modal_Panel)panelToInvoke.MainPanel;
                                             panel.ModalLoadDataQueue.Enqueue(modalLoadData);
                                         });
                                         
                                         

            //PanelManager.DeactivatePanel(PanelManager.SelectedPanels.Peek(), nextPanelLoadAction_IN: null);
        }
        else
        {
            Debug.Log("there is not enough research Scrolls");
        }
    }


    public override void UnloadAndDeallocate()
    {
        base.UnloadAndDeallocate();

        ownedAmount = requiredAmount = null;
    }

}
