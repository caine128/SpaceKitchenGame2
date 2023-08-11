using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissingRequirementsPopupPanel : PopupPanel_Single_SNG<MissingRequirementsPopupPanel>,
    IBrowsablePanel<SortableBluePrint>, ITaskHandlerPanel, IQuickUnloadable, ISinglePanelInvokeButton, IVariableButtonPanel, IDeferredDisabledPanel
{
    [SerializeField] private TextMeshProUGUI explanation_TextField;
    [SerializeField] private TextMeshProUGUI numeric_TextField;
    [SerializeField] private GUI_LerpMethods_Float progressBar_LerpMethod;
    [SerializeField] private Image progressBar_BG;
    [SerializeField] private Image progressBar_FG;

    public BrowserButton<SortableBluePrint>[] BrowserButtons { get { return _browserButtons; } }
    [SerializeField] private BrowserButton<SortableBluePrint>[] _browserButtons;

    public TaskCompletionSource<bool> TCS { get { return tcs; } }
    private TaskCompletionSource<bool> tcs = null;

    public int CurrentIndice { get { return _currentIndice; } }
    private int _currentIndice = 0;
    public List<SortableBluePrint> ListToIterate { get; private set; } = new List<SortableBluePrint>();    // Can be given default Size 
    public List<int> RequiredAmounts { get; private set; } = new List<int>();

    public InvokablePanelController PanelToInvoke { get { return panelToInvoke; } }

    public RectTransform[] PopupButtons_RT { get { return popupButtonsRT; } }
    [SerializeField] private RectTransform[] popupButtonsRT; // serialized for debug puprposes!!
    public Vector2[] PopupButtons_OriginalLocations { get { return popupButtonsOriginalLocations; } }

    public bool MarkedForDeferredDisabing
    {
        get => _markedForDeferredDisabling;
        private set => _markedForDeferredDisabling = value;
    }
    private bool _markedForDeferredDisabling = false;

    private Vector2[] popupButtonsOriginalLocations;

    [SerializeField] private InvokablePanelController panelToInvoke;

    private IBrowsablePanel<SortableBluePrint> interface_browsablePanel;
    private IVariableButtonPanel interface_variableButtonPanel;

    public static string PopupHeader { get => _PopupHeader; }
    private static string _PopupHeader;
    private static readonly string[] MISSINGTYPES = { "Missing ", "Ingredient : ", "Additional Item : ", "Product : " };


    /*public sealed override void ConfigureButtons()
    {
        var arrayLength = popupButtons.Length;
        popupButtonsRT = new RectTransform[arrayLength];
        popupButtonsOriginalLocations = new Vector2[arrayLength];

        for (int i = 0; i < arrayLength; i++)
        {
            popupButtonsRT[i] = popupButtons[i].GetComponent<RectTransform>();
            popupButtonsOriginalLocations[i] = popupButtonsRT[i].anchoredPosition;
            popupButtons[i].AssignPanel(this);
        }
    }*/

    public void ConfigureVariableButtons()
    {
        var arrayLength = popupButtons.Length;
        popupButtonsRT = new RectTransform[arrayLength];
        popupButtonsOriginalLocations = new Vector2[arrayLength];

        for (int i = 0; i < arrayLength; i++)
        {
            popupButtonsRT[i] = popupButtons[i].GetComponent<RectTransform>();
            popupButtonsOriginalLocations[i] = popupButtonsRT[i].anchoredPosition;
        }
    }

    protected sealed override void Start()
    {
        base.Start();
        InitialConfigBrowserButtons();
        interface_browsablePanel = this;
        interface_variableButtonPanel = this;
    }

    //public void InvokePanel(int invokablePanelIndex_IN, Action panelLoadAction_IN = null)
    //{
    //    OnInvokeButtonPressed?.Invoke(_invokablePanels[invokablePanelIndex_IN], panelLoadAction_IN);
    //}

    public void InitialConfigBrowserButtons()
    {
        foreach (var browserButton in _browserButtons)
        {
            browserButton.ButtonConfig(Instance);
        }
    }

    public sealed override void LoadPanel(PanelLoadData panelLoadData)
    {
        if (panelLoadData is PanelLoadDatas popupPanel_MissingRequirements_LoadData)
        {
            tcs = popupPanel_MissingRequirements_LoadData.tcs;

            if (ListToIterate.Count != 0 || RequiredAmounts.Count != 0)
            {
                ListToIterate.Clear(); RequiredAmounts.Clear();
            }

            foreach (var (blueprintToLoad, amountToLoad) in popupPanel_MissingRequirements_LoadData.bluePrintsToLoad)
            {
                ListToIterate.Add(blueprintToLoad);
                RequiredAmounts.Add(amountToLoad);
            }

            _currentIndice = 0;
            _PopupHeader = string.IsNullOrEmpty(panelLoadData.panelHeader) ? string.Empty : panelLoadData.panelHeader;
            LoadSingleItem(ListToIterate[_currentIndice]);
        }
        _markedForDeferredDisabling = false;

    }

    private void UpdateTextField()
    {
        numeric_TextField.text = NativeHelper.BuildString_Append(((Ingredient)bluePrint).GetAmount().ToString(), " / ", RequiredAmounts[_currentIndice].ToString());
        popupButtons[0].ModifyButtonValue(((Ingredient)bluePrint).CalculateRefillCost(RequiredAmounts[_currentIndice], new Gold()));
        popupButtons[1].ModifyButtonValue(((Ingredient)bluePrint).CalculateRefillCost(RequiredAmounts[_currentIndice], new Gem()));
        
        if (((Ingredient)bluePrint).GetAmount() >= RequiredAmounts[_currentIndice]) // reload the panel when amount is enough
        {
            RearrangeOnRequirementEnough();
        }
        else if (ShopData.CheckPresenceOfUpgrade(ShopUpgradesManager.Instance.GetRelevantResourceCabinet((Ingredient)bluePrint), out _)
            && ResourceCabinetUpgrade.GetOverallStorageCap(((Ingredient)bluePrint).IngredientType) <= ((Ingredient)bluePrint).GetAmount()) // ((ResourceCabinetUpgrade)shopUpgrade).GetOverallStorageCap() <= ((Ingredient)bluePrint).GetAmount())         // disable bar image component when cap is reached 
        {
            if (progressBar_BG.enabled != false || progressBar_FG.enabled != false) progressBar_BG.enabled = progressBar_FG.enabled = false;
        }     
    }
    public void UpdateProgressBar(float valueInitial, float valueFinal)
    {
        progressBar_LerpMethod.UpdateBarCall(valueInitial, valueFinal, lerpSpeedModifier: 1, queueRequest: false);
    }
    public void UpdateMissingWorker()
    {
        var currentWorker = (Worker)bluePrint;
        switch ((currentWorker.GetLevel() >= RequiredAmounts[_currentIndice], currentWorker.isHired))
        {
            case (true, true):
                RearrangeOnRequirementEnough();
                break;
            case (false, true):
                LoadSingleItem(ListToIterate[_currentIndice]);
                break;
            default:
                break;
        }
    }
    private void RearrangeOnRequirementEnough()
    {

        ListToIterate.RemoveAt(_currentIndice);
        RequiredAmounts.RemoveAt(_currentIndice);
        Debug.Log("inside the rearange call : list containts " + ListToIterate.Count + " elements " + PanelManager.SelectedPanels.Peek().MainPanel.name);
        if (ListToIterate.Count == 0)
        {
            if (PanelManager.SelectedPanels.Peek().MainPanel is MissingRequirementsPopupPanel missingRequirementsPopupPanel)
            {
                PanelManager.DeactivatePanel(PanelManager.SelectedPanels.Peek(), nextPanelLoadAction_IN: null);
            }
            else
            {
                Debug.Log("ismarked for diabling");
                _markedForDeferredDisabling = true;
            }
        }
        //PanelManager.DeactivatePanel(PanelManager.SelectedPanels.Peek(), nextPanelLoadAction_IN: null);

        else if (_currentIndice >= ListToIterate.Count)
        {
            LoadSingleItem(ListToIterate[_currentIndice - 1]);
        }
        else
        {
            LoadSingleItem(ListToIterate[_currentIndice]);
        }
    }
    public void BrowseInfo(SortableBluePrint blueprint_IN)
    {
        LoadSingleItem(blueprint_IN);
        //DisplayContainers();
    }

    private void LoadSingleItem(SortableBluePrint blueprint_IN)
    {
        _currentIndice = interface_browsablePanel.SetCurrentIndice(blueprint_IN);
        interface_browsablePanel.SetVisibilityBrowserButtons();


        if (bluePrint is Ingredient previousBlueprint)   // unsubscribe from the delegate in case previous bluePrint was a ingredient.
        {
            ResourcesManager.ingredientEventMapping[previousBlueprint.IngredientType][0] -= UpdateTextField;
            progressBar_LerpMethod.ClearQueue();
        }
        if (bluePrint is Worker previousWorker)
        {
            CharacterManager.WorkerEventsDict[previousWorker.workerspecs.workerType] -= UpdateMissingWorker;
        }


        base.LoadPanel(new PanelLoadData(mainLoadInfo: ListToIterate[_currentIndice], panelHeader: DefaultPopupHeader(), tcs_IN: null));

        if (bluePrint is Ingredient missingIngredient)
        {
            if (missingIngredient.GetAmount() >= RequiredAmounts[_currentIndice])  // Early Abort / Rearrange
            {
                RearrangeOnRequirementEnough();
                return;
            }
            else
            {
                SetProgressBarActiveStatus(true);

                if (IngredientGeneratorsManager.IngredientGenerators[missingIngredient.IngredientType].IngredientGeneratorLevel < 1)
                {

                    if (progressBar_BG.enabled != false || progressBar_FG.enabled != false) progressBar_BG.enabled = progressBar_FG.enabled = false;
                    interface_variableButtonPanel.SetButtonLayout(buttonAmountToDisplay: 1);
                    popupButtons[1].SetupButton(ButtonFunctionType.PopupPanel.MissingRequirements_ActivateIngredientGenerator);
                }

               /* else if (!ShopData.CheckPresenceOfUpgrade(ShopUpgradesManager.Instance.GetRelevantResourceCabinet(missingIngredient), out _)) //                  ShopUpgradesManager.Instance.ShopUpgrades_SO.resourceCabinet_Upgrades.baseInfo[(int)missingIngredient.IngredientType].name))                
                {

                    if (progressBar_BG.enabled != false || progressBar_FG.enabled != false) progressBar_BG.enabled = progressBar_FG.enabled = false;
                    interface_variableButtonPanel.SetButtonLayout(buttonAmountToDisplay: 1);
                    popupButtons[1].SetupButton(ButtonFunctionType.PopupPanel.MissingRequirements_BuyShopUpgrade);
                }*/

                else if (ShopData.CheckPresenceOfUpgrade(ShopUpgradesManager.Instance.GetRelevantResourceCabinet(missingIngredient), out _) 
                    && ResourceCabinetUpgrade.GetOverallStorageCap(missingIngredient.IngredientType) < RequiredAmounts[_currentIndice]) //((ResourceCabinetUpgrade)resourceCabinet).GetOverallStorageCap() < RequiredAmounts[_currentIndice])
                {
                    switch (((Ingredient)bluePrint).GetAmount())
                    {
                        case var amount when amount < ResourceCabinetUpgrade.GetOverallStorageCap(missingIngredient.IngredientType):  //((ResourceCabinetUpgrade)resourceCabinet).GetOverallStorageCap():
                            if (progressBar_BG.enabled != true || progressBar_FG.enabled != true) progressBar_BG.enabled = progressBar_FG.enabled = true;
                            break;
                        case var amount when amount >= ResourceCabinetUpgrade.GetOverallStorageCap(missingIngredient.IngredientType): //((ResourceCabinetUpgrade)resourceCabinet).GetOverallStorageCap():
                            if (progressBar_BG.enabled != false || progressBar_FG.enabled != false) progressBar_BG.enabled = progressBar_FG.enabled = false;
                            break;
                    }
                    interface_variableButtonPanel.SetButtonLayout(buttonAmountToDisplay: 1);
                    ResourcesManager.ingredientEventMapping[missingIngredient.IngredientType][0] += UpdateTextField;
                    popupButtons[1].SetupButton(ButtonFunctionType.PopupPanel.MissingRequirements_UpgradeResourceCabinet);
                }

                else
                {
                    if (progressBar_BG.enabled != true || progressBar_FG.enabled != true) progressBar_BG.enabled = progressBar_FG.enabled = true;
                    interface_variableButtonPanel.SetButtonLayout(buttonAmountToDisplay: 2);
                    ResourcesManager.ingredientEventMapping[missingIngredient.IngredientType][0] += UpdateTextField;
                    popupButtons[0].SetupButton(ButtonFunctionType.PopupPanel.MissingRequirements_RefillByToken);
                    popupButtons[1].SetupButton(ButtonFunctionType.PopupPanel.MissingRequirements_RefillByGems);
                }

                explanation_TextField.text = NativeHelper.BuildString_Append(MISSINGTYPES[0], MISSINGTYPES[1], Environment.NewLine, missingIngredient.GetName());
                numeric_TextField.text = NativeHelper.BuildString_Append(missingIngredient.GetAmount().ToString(), " / ", RequiredAmounts[_currentIndice].ToString());
            }


        }

        else if (bluePrint is ExtraComponent missingExtracomponent)
        {

            interface_variableButtonPanel.SetButtonLayout(buttonAmountToDisplay: 2);
            SetProgressBarActiveStatus(isActive: false);

            popupButtons[0].SetupButton(ButtonFunctionType.PopupPanel.MissingRequirements_BuyFromMarket);
            popupButtons[1].SetupButton(ButtonFunctionType.PopupPanel.MissingRequirements_GoToQuestArea);
            explanation_TextField.text = NativeHelper.BuildString_Append(MISSINGTYPES[0], MISSINGTYPES[2], Environment.NewLine, missingExtracomponent.GetName());
            numeric_TextField.text = NativeHelper.BuildString_Append(missingExtracomponent.GetAmount().ToString(), " / ", RequiredAmounts[_currentIndice].ToString());
        }

        else if (bluePrint is Product missingProduct)
        {
            interface_variableButtonPanel.SetButtonLayout(buttonAmountToDisplay: 2);

            SetProgressBarActiveStatus(isActive: false);

            popupButtons[0].SetupButton(ButtonFunctionType.PopupPanel.MissingRequirements_BuyFromMarket);
            popupButtons[1].SetupButton(ButtonFunctionType.PopupPanel.MissingRequirements_CraftProduct);

            explanation_TextField.text = NativeHelper.BuildString_Append(MISSINGTYPES[0], MISSINGTYPES[3], Environment.NewLine, missingProduct.GetName());
            numeric_TextField.text = NativeHelper.BuildString_Append(Inventory.Instance.CheckAmountInInventory_ByNameDict(missingProduct.GetName(), out _).ToString(), " / ", RequiredAmounts[_currentIndice].ToString());
        }

        else if (bluePrint is Worker missingWorker)
        {
            //interface_variableButtonPanel.SetButtonLayout(buttonAmountToDisplay: 1);
            SetProgressBarActiveStatus(isActive: false);
            numeric_TextField.text = string.Empty;

            CharacterManager.WorkerEventsDict[missingWorker.workerspecs.workerType] += UpdateMissingWorker;

            switch (missingWorker)
            {
                case var mw when mw.GetLevel() < 1:
                    interface_variableButtonPanel.SetButtonLayout(buttonAmountToDisplay: 1);
                    popupButtons[1].SetupButton(ButtonFunctionType.PopupPanel.MissingRequirements_None);
                    explanation_TextField.text = $"{missingWorker.GetName()} is not unlocked yet, keep leveling up!";
                    break;
                case var mw when !mw.isHired:
                    interface_variableButtonPanel.SetButtonLayout(buttonAmountToDisplay: 2);
                    popupButtons[1].SetupButton(ButtonFunctionType.PopupPanel.MissingRequirements_None);
                    popupButtons[0].SetupButton(ButtonFunctionType.PopupPanel.MissingRequirements_WorkerNotHired);
                    explanation_TextField.text = $"Hire {missingWorker.GetName()}";
                    break;
                case var mw when mw.GetLevel() < RequiredAmounts[_currentIndice]:
                    interface_variableButtonPanel.SetButtonLayout(buttonAmountToDisplay: 1);
                    popupButtons[1].SetupButton(ButtonFunctionType.PopupPanel.MissingRequirements_None);
                    explanation_TextField.text = $"Requires Level {RequiredAmounts[_currentIndice]} {missingWorker.GetName()}";
                    break;
                default:
                    Debug.LogError("this case should not come MissingWorkers");
                    break;
            }
        }

        else if (bluePrint is ShopUpgrade missingShopUpgrade)
        {
            SetProgressBarActiveStatus(isActive: false);
            numeric_TextField.text = string.Empty;


            switch (missingShopUpgrade)
            {
                case var msu when msu is WorkStationUpgrade workStationUpgrade:

                    switch (workStationUpgrade)
                    {
                        case var wsu when wsu.GetLevel() < 1: // This means the workstation does not exist, not purchased yet
                            interface_variableButtonPanel.SetButtonLayout(buttonAmountToDisplay: 2);
                            popupButtons[0].SetupButton(ButtonFunctionType.PopupPanel.MissingRequirements_BuyShopUpgrade);
                            popupButtons[1].SetupButton(ButtonFunctionType.PopupPanel.MissingRequirements_None);

                            explanation_TextField.text = $"{missingShopUpgrade.GetName()} is not purchased yet, purchase it now!";
                            break;
                        case var wsu when wsu.GetLevel() < RequiredAmounts[_currentIndice]:
                            interface_variableButtonPanel.SetButtonLayout(buttonAmountToDisplay: 2);
                            popupButtons[0].SetupButton(ButtonFunctionType.PopupPanel.MissingRequirements_UpgradeShopUpgrade);
                            popupButtons[1].SetupButton(ButtonFunctionType.PopupPanel.MissingRequirements_None);

                            explanation_TextField.text = $"You need Lvl.{RequiredAmounts[_currentIndice]} {missingShopUpgrade.GetName()}, Level it up now!";
                            break;
                        default:
                            Debug.LogError("this case should not come : MissingWorkstations");
                            break;
                    }
                    break;

                case var msu when msu is ResourceCabinetUpgrade resourceCabinetUpgrade:

                    switch (resourceCabinetUpgrade)
                    {
                        case var rcu when rcu.GetLevel() < 1: // This means the resourceCabinet does not exist, not purchased yet

                            if (progressBar_BG.enabled != false || progressBar_FG.enabled != false) progressBar_BG.enabled = progressBar_FG.enabled = false;
                            interface_variableButtonPanel.SetButtonLayout(buttonAmountToDisplay: 2);
                            popupButtons[0].SetupButton(ButtonFunctionType.PopupPanel.MissingRequirements_BuyShopUpgrade);
                            popupButtons[1].SetupButton(ButtonFunctionType.PopupPanel.MissingRequirements_None);

                            explanation_TextField.text = $"{resourceCabinetUpgrade.GetName()} is not purchased yet, purchase it now!";
                            break;
                    }
                    break;              
            }

        }

        DisplayContainers();
    }

    private void SetProgressBarActiveStatus(bool isActive)
    {
        if (progressBar_BG.gameObject.activeInHierarchy != isActive || progressBar_FG.gameObject.activeInHierarchy != isActive)
        {
            progressBar_BG.gameObject.SetActive(isActive);
            progressBar_FG.gameObject.SetActive(isActive);
        }
    }


    public void HandleTask(bool isTrue)
    {
        if (tcs != null && tcs.TrySetResult(isTrue))
        {
            tcs = null;
        }
    }

    public void UpgradeResourceContainer()
    {
        var resourceCabinet = ShopUpgradesManager.Instance.GetRelevantResourceCabinet(bluePrint as Ingredient);

        // later to remove debug order above and rearrange deactivation of panel by opening the panel of the cabinet to upgrade the cabinet.
        Debug.Log("upgrade panel should be opened here of the type : " + resourceCabinet.GetName());
        PanelManager.DeactivatePanel(PanelManager.SelectedPanels.Peek(), nextPanelLoadAction_IN: null);
    }

    /// <summary>
    /// Button Click Action 
    /// </summary>
    public void TryOpenCraftPanel()
    {
        var recipeToCraft = ((ICraftable)bluePrint).GetProductRecipe();
        if (RecipeManager.RecipesAvailableLookUp_Dict.ContainsKey(recipeToCraft))
        {
            if (PanelManager.SelectedPanels.Last().MainPanel is CraftPanel_Manager craftPanel_Manager)
            {

                PanelManager.DeactivatePanel(PanelManager.SelectedPanels.Peek(),
                    nextPanelLoadAction_IN: () => craftPanel_Manager.StimulateSelectionOf(recipeToCraft),
                    extraLoadActions_IN: () => craftPanel_Manager.ScrollToSelection(recipeToCraft, markSelection: true));
            }
            else
            {
                Debug.LogError("the disactivated panel is not the correct panel");
            }
        }
        else
        {
            Debug.Log("you do not have the necessary recipe yet");
        }
    }

    /// <summary>
    /// Button Click Action 
    /// </summary>
    /// 
    public void OpenShopPanel()
    {

        var shopUpgradeToStimulate = ShopUpgradesManager.Instance.GetRelevantResourceCabinet(bluePrint);

        if (panelToInvoke.MainPanel is ShopUpgradesPanel_Manager shopupgradesPanel_Manager)
        {
            PanelManager.ActivateAndLoad(invokablePanel_IN: panelToInvoke,
                panelLoadAction_IN: () => shopupgradesPanel_Manager.StimulateSelectionOf(shopUpgradeToStimulate),
                extraLoadActions_IN: () => shopupgradesPanel_Manager.ScrollToSelection(shopUpgradeToStimulate, markSelection: true));
        }
    }


    /// <summary>
    /// Button Click Action 
    /// </summary>
    /// 
    public void RefillIngredient(ISpendable spendable)
    {
        if (RequiredAmounts.Count > _currentIndice) /// To prevent the do the below operation in case the list has been already cleared by timer method that fills ingredients.
        {
            var ingredient = (Ingredient)bluePrint;
            //var requiredGold = ingredient.CalculateRefillCost(RequiredAmounts[CurrentIndice], spendable);
            if (StatsData.IsSpendableAmountEnough(spendable.Amount, spendable))
            {
                StatsData.SetSpendableValue(spendable, -spendable.Amount);
                ResourcesManager.Instance.AddIngredient(ingredient.IngredientType, ingredient.MaxCap - ingredient.GetAmount(), bypassMaxCap: false);
            }
            else
            {
                Debug.Log("you font have enough spendable of type :" + spendable.GetType().ToString());
            }
        }
    }
    protected override string DefaultPopupHeader()
    {
        return NativeHelper.BuildString_Append(_PopupHeader + " " + (_currentIndice + 1).ToString() + " / " + ListToIterate.Count.ToString());
    }

    public void QuickUnload()
    {
        if (bluePrint is Ingredient ingredient) ResourcesManager.ingredientEventMapping[ingredient.IngredientType][0] -= UpdateTextField;
        _markedForDeferredDisabling = false;

        if (bluePrint is Worker worker && !PanelManager.SelectedPanels.Contains(PanelManager.InvokablePanels[this.GetType()]))
        {
            CharacterManager.WorkerEventsDict[worker.workerspecs.workerType] -= UpdateMissingWorker;
        }
    }

    public override void UnloadAndDeallocate()
    {
        bluePrint = null;

        ListToIterate.Clear();
        RequiredAmounts.Clear();

        base.UnloadAndDeallocate();
    }
}
