using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class PanelManager : MonoBehaviour
{
    public const float MAXCLICKOFFSET = 3f;
    private static Canvas backgroundPanelCanvas;
    private static BackgroudPanel backgroundPanel;

    public static Color BackgroundPanel_LerpColor_NoImage
    {
        get => _backgroundPanel_LerpColor_NoImage;
    }
    private static Color _backgroundPanel_LerpColor_NoImage = new(0f, 0f, 0f, 0.8f);

    public static Color BackgroundPanel_LerpColor_WithImage
    {
        get => _backgroundPanel_LerpColor_WithImage;
    }
    private static Color _backgroundPanel_LerpColor_WithImage = new(0.3679245f, 0.3280082f, 0.3280082f, 1f);

    public static Top_Bars_Controller TopBarsController => _topBarsController;
    private static Top_Bars_Controller _topBarsController;

    public static Bottom_Bars_Controller BottomBarsController => _bottomBarsController;
    private static Bottom_Bars_Controller _bottomBarsController;

    public static Level_Bars_Controller LevelBarsController => _levelBarsController;
    private static Level_Bars_Controller _levelBarsController;

    public static CraftWheel_Controller CraftWheelController => _craftWheelController;
    private static CraftWheel_Controller _craftWheelController;

    public static Stack<InvokablePanelController> SelectedPanels
    {
        get { return selectedPanels; }
    }
    private static Stack<InvokablePanelController> selectedPanels = new();

    public static Dictionary<Type, InvokablePanelController> InvokablePanels => _invokablePanels;
    private static Dictionary<Type, InvokablePanelController> _invokablePanels = new();

    private static ConcurrentStack<PanelLoadData> missingRequirementsPanelPreviousLoadDatas = new();


    void Awake()
    {
        backgroundPanelCanvas = UnityEngine.GameObject.Find("Canvas_BackgroundPanel").GetComponent<Canvas>();
        backgroundPanel = UnityEngine.GameObject.Find("BackgroundPanel").GetComponent<BackgroudPanel>();

        _levelBarsController = UnityEngine.GameObject.Find("LevelBars_Parent").GetComponent<Level_Bars_Controller>();
        _topBarsController = UnityEngine.GameObject.Find("Top_Bars_Parent").GetComponent<Top_Bars_Controller>();
        _bottomBarsController = UnityEngine.GameObject.Find("Navigation_Bars_Parent").GetComponent<Bottom_Bars_Controller>();
        _craftWheelController = UnityEngine.GameObject.Find("Craftwheel_Bar_Parent").GetComponent<CraftWheel_Controller>();
    }

    void OnEnable()
    {
        FindObjectOfType<SceneController>().OnSceneLoaded += SceneConfig;
    }

    void OnDisable()
    {
        var scenecontroller = FindObjectOfType<SceneController>();
        if (scenecontroller)
        {
            scenecontroller.OnSceneLoaded -= SceneConfig;
        }
    }

    private void SceneConfig(object sender, SceneController.OnSceneLoadedEventArgs e) // Latertobeupdated when UI receives data from the player
    {

        foreach (GUI_LerpMethods lerpScript in FindObjectsOfType<GUI_LerpMethods>())
        {
            lerpScript.PanelConfig();
        }
        foreach (HUDBarsController HUDBarsController in FindObjectsOfType<HUDBarsController>())
        {
            HUDBarsController.PanelControllerConfig();
        }
        foreach (PopupPanel popupPanel in FindObjectsOfType<PopupPanel>())
        {
            popupPanel.ConfigureButtons();
        }
        foreach (IVariableButtonPanel variableButtonPanel in FindObjectsOfType<MonoBehaviour>().OfType<IVariableButtonPanel>())
        {
            variableButtonPanel.ConfigureVariableButtons();
        }
        foreach (InvokablePanelController invokablePanel in FindObjectsOfType<InvokablePanelController>())
        {
            invokablePanel.PanelSetup();
        }

        PopulateInvokablePanlesDict();
        backgroundPanel.gameObject.SetActive(false);
    }

    private static void PopulateInvokablePanlesDict()
    {
        FindPanelAndAdd<ConfirmationPopupPanel>();
        FindPanelAndAdd<InventoryPanel_Manager>();
        FindPanelAndAdd<Information_Modal_Panel>();
        FindPanelAndAdd<AscensionLadderPanel_Manager>();
        FindPanelAndAdd<CharactersInfoPanel_Manager>();
        FindPanelAndAdd<CharactersPanel_Manager>();
        FindPanelAndAdd<RecipeInfoPanel_Manager>();
        FindPanelAndAdd<UnlockRecipePopupPanel>();
        FindPanelAndAdd<ResearchPopupPanel>();
        FindPanelAndAdd<MissingRequirementsPopupPanel>();
        FindPanelAndAdd<HireCharacter_Panel>();
        FindPanelAndAdd<ShopUpgradesInfoPanel_Manager>();
        FindPanelAndAdd<Tooltip_Panel_Manager>();
        FindPanelAndAdd<ProgressPopupPanel>();
        FindPanelAndAdd<BuildOptionsPanel_Manager>();

        void FindPanelAndAdd<T_PanelType>()
            where T_PanelType : Panel_Base
        {
            var panelType = typeof(T_PanelType);
            var invokablePanelController = global::UnityEngine.GameObject.FindObjectsOfType<T_PanelType>();

            if (invokablePanelController.Length > 1) Debug.LogError($"There is more than 1 instance of {panelType}");

            _invokablePanels.Add(panelType, invokablePanelController[0].GetComponent<InvokablePanelController>());
        }
    }


    private static void SetBGPanelSortingOrder()
    {
        (bool isChangeNeeded, int? modifiedSortingOrder) backgroundStateCheck = selectedPanels.TryPeek(out InvokablePanelController controller) switch
        {
            true => (controller.MainPanel, backgroundPanelCanvas.sortingOrder) switch
            {
                (Tooltip_Panel_Manager, _) => (false, null),
                (PopupPanel, 10) => (false, null),
                (PopupPanel, not 10) => (true, 10),
                (not PopupPanel, 0) => (false, null),
                (not PopupPanel, not 0) => (true, 0),
            },
            false => (false, null),
        };

        if (backgroundStateCheck.isChangeNeeded) backgroundPanelCanvas.sortingOrder = (int)backgroundStateCheck.modifiedSortingOrder;
    }


    public static void ActivateAndLoad(InvokablePanelController invokablePanel_IN, Action panelLoadAction_IN, Action preLoadAction_IN = null, Action alternativeLoadAction_IN = null, params Action[] extraLoadActions_IN)
    {
        if (selectedPanels.TryPeek(out InvokablePanelController panel) && panel.MainPanel == invokablePanel_IN.MainPanel)
        {
            if (alternativeLoadAction_IN is null)
                Debug.LogError("There is no fallback Load Action on Panel Repetition");

            switch (invokablePanel_IN.MainPanel)
            {
                case Information_Modal_Panel:
                    alternativeLoadAction_IN?.Invoke();
                    break;
            }

            return;
        }


        var previousMissingRequirementsPanel = selectedPanels.FirstOrDefault(panel => panel.MainPanel is MissingRequirementsPopupPanel)?.MainPanel as MissingRequirementsPopupPanel;

        if (invokablePanel_IN.MainPanel is MissingRequirementsPopupPanel
            && previousMissingRequirementsPanel != null
            && selectedPanels.Where(sp => sp.MainPanel is MissingRequirementsPopupPanel).Count() == (missingRequirementsPanelPreviousLoadDatas.Count + 1))
        {
            var previousPanelLoadData = new PanelLoadDatas(mainLoadInfo: null,
                                                              panelHeader: MissingRequirementsPopupPanel.PopupHeader,
                                                              tcs_IN: previousMissingRequirementsPanel.TCS,
                                                              bluePrintsToLoad: FunctionalHelpers.CreateEnumerableFromSequences(
                                                                                                           sequence1: previousMissingRequirementsPanel.ListToIterate,
                                                                                                           sequence2: previousMissingRequirementsPanel.RequiredAmounts).ToList());
            missingRequirementsPanelPreviousLoadDatas.Push(previousPanelLoadData);
        }


        var isFirstPanelOfItsType = !selectedPanels.Contains(invokablePanel_IN);

        if (invokablePanel_IN.MainPanel is not Tooltip_Panel_Manager && selectedPanels.TryPeek(out InvokablePanelController activePanel))
        {
            MoveOutDeselectedPanel(activePanel, unloadAction: null);
        }

        preLoadAction_IN?.Invoke();

        selectedPanels.Push(invokablePanel_IN);
        backgroundPanel.UpdateBackgroundPanel();

        MoveInSelectedPanel(selectedPanels.Peek(), panelLoadAction_IN, extraLoadActions_IN);

        SetBGPanelSortingOrder();
        //var bgPanelStateChangeCheck = SetBGPanelSortingOrder();
        //if (bgPanelStateChangeCheck.isChangeNeeded) backgroundPanelCanvas.sortingOrder = (int)bgPanelStateChangeCheck.modifiedSortingOrder;

        switch (invokablePanel_IN.MainPanel)
        {
            case MenuPanel_Manager when isFirstPanelOfItsType:
                _bottomBarsController.DisplaceBars();
                _craftWheelController.DisplaceBars();
                _topBarsController.DisplaceBars();
                _levelBarsController.DisplaceBars();
                break;
            case CraftPanel_Manager when isFirstPanelOfItsType:
                _bottomBarsController.DisplaceBars();
                _craftWheelController.DisplaceBars();
                _topBarsController.ArrangeBarsInitial();
                break;
            case RecipeInfoPanel_Manager when isFirstPanelOfItsType && SelectedPanels.Where(panel => panel.MainPanel is CraftPanel_Manager).Count() < 1:
                _topBarsController.ArrangeBarsInitial();
                break;
            case InventoryPanel_Manager when isFirstPanelOfItsType:
                _bottomBarsController.DisplaceBars();
                _craftWheelController.DisplaceBars();
                _topBarsController.ArrangeBarsInitial();
                break;
            case ShopPanel_Manager when isFirstPanelOfItsType:
                _topBarsController.ArrangeBarsInitial();
                _craftWheelController.DisplaceBars();
                _bottomBarsController.ArrangeBarsInitial();
                break;
            case CharactersPanel_Manager when isFirstPanelOfItsType:
                _bottomBarsController.DisplaceBars();
                _craftWheelController.DisplaceBars();
                _topBarsController.ArrangeBarsInitial();
                break;
            case HireCharacter_Panel when isFirstPanelOfItsType && selectedPanels.All(panel => panel.MainPanel is not CharactersPanel_Manager):
                _bottomBarsController.DisplaceBars();
                _craftWheelController.DisplaceBars();
                _topBarsController.ArrangeBarsInitial();
                break;
            case ShopUpgradesPanel_Manager when selectedPanels.Where(panel => panel.MainPanel is ScrollablePanel).Count() > 1:
                _bottomBarsController.DisplaceBars();
                _craftWheelController.DisplaceBars();
                _topBarsController.ArrangeBarsInitial();
                break;
            case ShopUpgradesPanel_Manager when isFirstPanelOfItsType:
                _bottomBarsController.ArrangeBarsFinal();
                EnableAndUpdateBackgroundPanel();
                break;
            case GameItemInfoPanel_Manager when isFirstPanelOfItsType:
                break;
            case EnhanceItemPopupPanel when isFirstPanelOfItsType:
            case ConfirmationPopupPanel when isFirstPanelOfItsType:
                break;
            //do not placeThebars
            case PopupPanel when isFirstPanelOfItsType:
                _bottomBarsController.PlaceBars();
                _craftWheelController.PlaceBars();
                break;
            case Information_Modal_Panel when isFirstPanelOfItsType:
                _bottomBarsController.DisplaceBars();
                _craftWheelController.DisplaceBars();
                break;
            case AscensionLadderPanel_Manager when isFirstPanelOfItsType:
                _bottomBarsController.DisplaceBars();
                _craftWheelController.DisplaceBars();
                break;
            default:
                Debug.LogWarning("This panel is not in the cases of the switch statement, panel name is : " + invokablePanel_IN.MainPanel.name);
                break;
            case null:
                throw new ArgumentException();

        }

    }


    public static void DeactivatePanel(InvokablePanelController invokablePanelIN, Action nextPanelLoadAction_IN, Action unloadAction = null, params Action[] extraLoadActions_IN)
    {
        if (invokablePanelIN != selectedPanels.Peek())
        {
            Debug.LogError("the incoming invokable is not matching to the selected panles Top pamel, panel name is : " + invokablePanelIN.MainPanel.name);
        }


        MoveOutDeselectedPanel(selectedPanels.Pop(), unloadAction);

        SetBGPanelSortingOrder();
        //var bgPanelStateChangeCheck = SetBGPanelSortingOrder();
        //if (bgPanelStateChangeCheck.isChangeNeeded) backgroundPanelCanvas.sortingOrder = (int)bgPanelStateChangeCheck.modifiedSortingOrder;

        var isLastPanelOfType = !selectedPanels.Contains(invokablePanelIN);

        switch (invokablePanelIN.MainPanel)
        {
            case MenuPanel_Manager when isLastPanelOfType:
                _topBarsController.PlaceBars();
                _levelBarsController.PlaceBars();
                _bottomBarsController.PlaceBars();
                _craftWheelController.PlaceBars();
                break;

            case CraftPanel_Manager when isLastPanelOfType:
                _topBarsController.ArrangeBarsFinal();
                _bottomBarsController.PlaceBars();
                _craftWheelController.PlaceBars();
                break;

            case RecipeInfoPanel_Manager when selectedPanels.TryPeek(out InvokablePanelController panel) && panel.MainPanel is GameItemInfoPanel_Manager:
                _topBarsController.ArrangeBarsInitial();
                break;

            case InventoryPanel_Manager when isLastPanelOfType:
                _topBarsController.ArrangeBarsFinal();
                _bottomBarsController.PlaceBars();
                _craftWheelController.PlaceBars();
                break;

            case CharactersPanel_Manager when isLastPanelOfType:
                _topBarsController.ArrangeBarsFinal();
                _bottomBarsController.PlaceBars();
                _craftWheelController.PlaceBars();
                break;

            case HireCharacter_Panel when isLastPanelOfType && selectedPanels.All(panel => panel.MainPanel is not CharactersPanel_Manager):
                _topBarsController.ArrangeBarsFinal();
                _bottomBarsController.PlaceBars();
                _craftWheelController.PlaceBars();
                break;
            case ShopPanel_Manager when isLastPanelOfType:
                _topBarsController.ArrangeBarsFinal();
                _bottomBarsController.ArrangeBarsFinal();
                _bottomBarsController.PlaceBars();
                _craftWheelController.PlaceBars();
                break;

            case ShopUpgradesPanel_Manager when selectedPanels.TryPeek(out InvokablePanelController invokablePanelController) == true && invokablePanelController.MainPanel is ShopPanel_Manager:
                _bottomBarsController.ArrangeBarsInitial(); // HAVE TO REMOVE THIS AND THIS KIND OF CASES AS WELL !!!! very confusing
                break;

            case DeleteItemPopupPanel when selectedPanels.Count == 0:
                _topBarsController.ArrangeBarsFinal();
                break;

            case ConfirmationPopupPanel when selectedPanels.Count == 0: // is this case even hitting ? ?//
                _topBarsController.ArrangeBarsFinal();
                break;
            case ProgressPopupPanel when selectedPanels.Count == 0:
                _topBarsController.ArrangeBarsFinal();
                _bottomBarsController.PlaceBars();
                _craftWheelController.PlaceBars();
                break;
            case Information_Modal_Panel modal when selectedPanels.Count == 0:
                _topBarsController.ArrangeBarsFinal();
                _bottomBarsController.PlaceBars();
                _craftWheelController.PlaceBars();
                break;
            case AscensionLadderPanel_Manager when selectedPanels.Count == 0:
                _topBarsController.ArrangeBarsFinal();
                _bottomBarsController.PlaceBars();
                _craftWheelController.PlaceBars();
                break;
            case PopupPanel when isLastPanelOfType && selectedPanels.Count != 0:
                _bottomBarsController.DisplaceBars();
                _craftWheelController.DisplaceBars();
                break;

            default:
                Debug.LogWarning("This panel is not in the cases of the switch statement, panel name is : " + invokablePanelIN.MainPanel.name);
                break;
            case null:
                throw new ArgumentException();
        }


        if (selectedPanels.TryPeek(out InvokablePanelController activePanel))
        {
            if (activePanel.MainPanel is IDeferredDisabledPanel deferredDisabledPanel && deferredDisabledPanel.MarkedForDeferredDisabing)
            {
                Debug.Log("inside the call and " + deferredDisabledPanel.MarkedForDeferredDisabing);
                DeactivatePanel(activePanel, nextPanelLoadAction_IN: null);
                return;
            }

            else if (invokablePanelIN.MainPanel is not Tooltip_Panel_Manager)
            {
                Debug.Log("activePanel.MainPanel " + activePanel.MainPanel + " " + missingRequirementsPanelPreviousLoadDatas.TryPeek(out _).ToString());
                var modifiedLoadAction = activePanel.MainPanel is MissingRequirementsPopupPanel
                                         && missingRequirementsPanelPreviousLoadDatas.Count == selectedPanels.Where(p => p.MainPanel is MissingRequirementsPopupPanel).Count()
                                         && missingRequirementsPanelPreviousLoadDatas.TryPop(out PanelLoadData result)
                                                    ? () => MissingRequirementsPopupPanel.Instance.LoadPanel(result)
                                                    : nextPanelLoadAction_IN;

                MoveInSelectedPanel(activePanel, loadAction: modifiedLoadAction, extraLoadActions: extraLoadActions_IN);
            }
        }


        backgroundPanel.UpdateBackgroundPanel();


    }

    #region NavigationStack Intervention Methods

    public static void ClearStackAndDeactivateElements()
    {
        while (selectedPanels.TryPeek(out _))
        {
            RemoveCurrentPanelFromNavigationStack();
        }

        SetBGPanelSortingOrder();
    }



    public static void RemoveCurrentPanelFromNavigationStackIf(params Predicate<InvokablePanelController>[] removeConditions)
    {
        if (selectedPanels.TryPeek(out InvokablePanelController panel) && removeConditions.All(rc => rc(panel)))
        {
            RemoveCurrentPanelFromNavigationStack();
            SetBGPanelSortingOrder();
        }
    }

    private static async void RemoveCurrentPanelFromNavigationStack()
    {
        if (selectedPanels.Peek().MainPanel is MissingRequirementsPopupPanel && selectedPanels.Where(p => p.MainPanel is MissingRequirementsPopupPanel).Count() == missingRequirementsPanelPreviousLoadDatas.Count)
        {
            var testBool = missingRequirementsPanelPreviousLoadDatas.TryPop(out _);
            if (!testBool) Debug.LogError("cannot pop where it should");
        }

        var removedPanel = selectedPanels.Pop();
        await removedPanel.DisplacePanels(isInterpolated: false, unloadAction: null);
    }

    public static void RemoveFromNavigationStack_Until(Type removeUntilType)
    {
        while (SelectedPanels.Count > 0)
        {
            if (selectedPanels.Peek().MainPanel.GetType() == removeUntilType)
            {
                break;
            }
            else
            {
                RemoveCurrentPanelFromNavigationStack();
            }
        }

        SetBGPanelSortingOrder();
    }
    #endregion


    #region PanelInOut Movement Methods

    private static void MoveInSelectedPanel(InvokablePanelController selectedPanelIN, Action loadAction, params Action[] extraLoadActions)
    {
        selectedPanelIN.gameObject.SetActive(true);
        selectedPanelIN.PlacePanels(loadAction, extraLoadActions);
    }

    private static async void MoveOutDeselectedPanel(InvokablePanelController selectedPanelIN, Action unloadAction)
    {
        await selectedPanelIN.DisplacePanels(isInterpolated: true, unloadAction: unloadAction);
    }

    #endregion

    #region BackgroundPanel Enable And Disable Methods

    private static void EnableAndUpdateBackgroundPanel()
    {
        if (selectedPanels.TryPeek(out InvokablePanelController invokableController) &&
            invokableController.MainPanel is not ShopPanel_Manager &&
            (backgroundPanel.gameObject.activeInHierarchy != true || backgroundPanel.GUI_LerpMethods_Color.RunningCoroutine is not null))
        {
            backgroundPanel.gameObject.SetActive(true);
        }
    }

    #endregion
}