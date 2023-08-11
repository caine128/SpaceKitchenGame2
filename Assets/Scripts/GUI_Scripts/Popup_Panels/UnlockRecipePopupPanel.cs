using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnlockRecipePopupPanel : PopupPanel_Single_SNG<UnlockRecipePopupPanel> ,IVariableButtonPanel
{
    [SerializeField] private TextMeshProUGUI descriptionText;

    private const string DEFAULTPOPUPHEADER = "Unlock Recipe";
    private const string UNLOCKBY_CHESTBYSTRING = "This recipe can be found in the ";
    private const string UNLOCKBY_WORKERSTRING = "You must hire  to unlock this recipe";

    [SerializeField] private PanelFunction? activePanelFunction = null; // serialized for debugpurposes
    public int? activeAmount { get; private set; } = null;
    public WorkerType.Type? activeWorkerType { get; private set; } = null;
    public Chest activeChest { get; private set; } = null;

    public RectTransform[] PopupButtons_RT { get { return popupButtonsRT; } }
    private RectTransform[] popupButtonsRT;
    public Vector2[] PopupButtons_OriginalLocations { get { return popupButtonsOriginalLocations; } }
    private Vector2[] popupButtonsOriginalLocations;

    private IVariableButtonPanel interface_variableButtonPanel;

    private enum PanelFunction
    {
        SearchInChest,
        WorkerRequired,
    }


    public override void ConfigureButtons()
    {
        base.ConfigureButtons();
        interface_variableButtonPanel = this;
    }

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

    public sealed override void LoadPanel(PanelLoadData panelLoadData)
    {
        base.LoadPanel(panelLoadData);

        var recipe = bluePrint as ProductRecipe;
        if (recipe.recipeSpecs.unlockPrerequisite[0].unlockPrerequisiteType == Recipes_SO.UnlockPrerequisiteType.RequiredWorker)
        {
            var requiredWorker = recipe.recipeSpecs.unlockPrerequisite[0].requiredworkers.requiredWorker[0].requiredWorker;
            descriptionText.text = NativeHelper.BuildString_Insert(UNLOCKBY_WORKERSTRING, requiredWorker.ToString(), 14);

            if (activePanelFunction != PanelFunction.WorkerRequired)
            {
                activePanelFunction = PanelFunction.WorkerRequired;
                interface_variableButtonPanel.SetButtonLayout(buttonAmountToDisplay: 1);
                //popupButtonsRT[0].gameObject.SetActive(false);
                //popupButtonsRT[1].anchoredPosition = new Vector2(0, popupButtonsRT[1].anchoredPosition.y);
            }

            activeWorkerType = requiredWorker;
            popupButtons[1].SetupButton(ButtonFunctionType.PopupPanel.UnlockRecipe_Worker);
        }
        else if (recipe.recipeSpecs.unlockPrerequisite[0].unlockPrerequisiteType == Recipes_SO.UnlockPrerequisiteType.ChestLoot)
        {
            var chest = new Chest(SpecialItemType.Type.Chest, recipe.recipeSpecs.unlockPrerequisite[0].chest);
            activeAmount = Inventory.Instance.CheckAmountInInventory(chest);
            activeChest = activeAmount > 0 ? chest : null;

            descriptionText.text = NativeHelper.BuildString_Append(UNLOCKBY_CHESTBYSTRING, chest.GetName());

            if (activePanelFunction != PanelFunction.SearchInChest)
            {
                activePanelFunction = PanelFunction.SearchInChest;
                interface_variableButtonPanel.SetButtonLayout(buttonAmountToDisplay: 2);

            }


            popupButtons[0].SetupButton(ButtonFunctionType.PopupPanel.UnlockRecipe_None);
            popupButtons[1].SetupButton(activeAmount > 0 ? ButtonFunctionType.PopupPanel.UnlockRecipe_Chest : ButtonFunctionType.PopupPanel.UnlockReipe_Buy);
        }
    }

   /* public sealed override void LoadPanel<T_ListBlueprintType>(PanelLoadData<T_ListBlueprintType> panelLoadData)
    {
        base.LoadPanel(panelLoadData);

        var recipe = bluePrint as ProductRecipe;
        if (recipe.recipeSpecs.unlockPrerequisite[0].unlockPrerequisiteType == Recipes_SO.UnlockPrerequisiteType.RequiredWorker)
        {
            var requiredWorker = recipe.recipeSpecs.unlockPrerequisite[0].requiredworkers.requiredWorker[0].requiredWorker;
            descriptionText.text = NativeHelper.BuildString_Insert(UNLOCKBY_WORKERSTRING, requiredWorker.ToString(), 14);

            if (activePanelFunction != PanelFunction.WorkerRequired)
            {
                activePanelFunction = PanelFunction.WorkerRequired;
                interface_variableButtonPanel.SetButtonLayout(buttonAmountToDisplay: 1);
                //popupButtonsRT[0].gameObject.SetActive(false);
                //popupButtonsRT[1].anchoredPosition = new Vector2(0, popupButtonsRT[1].anchoredPosition.y);
            }

            activeWorkerType = requiredWorker;
            popupButtons[1].SetupButton(ButtonFunctionType.PopupPanel.UnlockRecipe_Worker);
        }
        else if (recipe.recipeSpecs.unlockPrerequisite[0].unlockPrerequisiteType == Recipes_SO.UnlockPrerequisiteType.ChestLoot)
        {
            var chest = new Chest(SpecialItemType.Type.Chest, recipe.recipeSpecs.unlockPrerequisite[0].chest);
            activeAmount = Inventory.Instance.CheckAmountInInventory(chest);
            activeChest = activeAmount > 0 ? chest : null;

            descriptionText.text = NativeHelper.BuildString_Append(UNLOCKBY_CHESTBYSTRING, chest.GetName());

            if (activePanelFunction != PanelFunction.SearchInChest)
            {
                activePanelFunction = PanelFunction.SearchInChest;
                interface_variableButtonPanel.SetButtonLayout(buttonAmountToDisplay: 2);

            }


            popupButtons[0].SetupButton(ButtonFunctionType.PopupPanel.UnlockRecipe_None);
            popupButtons[1].SetupButton(activeAmount > 0 ? ButtonFunctionType.PopupPanel.UnlockRecipe_Chest : ButtonFunctionType.PopupPanel.UnlockReipe_Buy);
        }
    }*/

    /*
    protected sealed override void LoadPopupInfo(SortableBluePrint bluePrint_IN, string popupHeader_IN = null)
    {
        base.LoadPopupInfo(bluePrint_IN, popupHeader_IN);
       
        var recipe = bluePrint as ProductRecipe;
        if (recipe.recipeSpecs.unlockPrerequisite[0].unlockPrerequisiteType == Recipes_SO.UnlockPrerequisiteType.RequiredWorker)
        {
            var requiredWorker = recipe.recipeSpecs.unlockPrerequisite[0].requiredworkers.requiredWorker[0].requiredWorker;
            descriptionText.text = NativeHelper.BuildString_Insert(UNLOCKBY_WORKERSTRING, requiredWorker.ToString(), 14);

            if (activePanelFunction != PanelFunction.WorkerRequired)
            {
                activePanelFunction = PanelFunction.WorkerRequired;
                interface_variableButtonPanel.SetButtonLayout(buttonAmountToDisplay: 1);
                //popupButtonsRT[0].gameObject.SetActive(false);
                //popupButtonsRT[1].anchoredPosition = new Vector2(0, popupButtonsRT[1].anchoredPosition.y);
            }

            activeWorkerType = requiredWorker;
            popupButtons[1].SetupButton(ButtonFunctionType.PopupPanel.UnlockRecipe_Worker);
        }
        else if (recipe.recipeSpecs.unlockPrerequisite[0].unlockPrerequisiteType == Recipes_SO.UnlockPrerequisiteType.ChestLoot)
        {
            var chest = new Chest(SpecialItemType.Type.Chest, recipe.recipeSpecs.unlockPrerequisite[0].chest);
            activeAmount = Inventory.Instance.CheckAmountInInventory(chest);
            activeChest = activeAmount > 0 ? chest : null;

            descriptionText.text = NativeHelper.BuildString_Append(UNLOCKBY_CHESTBYSTRING, chest.GetName());

            if (activePanelFunction != PanelFunction.SearchInChest)
            {
                activePanelFunction = PanelFunction.SearchInChest;
                interface_variableButtonPanel.SetButtonLayout(buttonAmountToDisplay: 2);
                //popupButtonsRT[0].gameObject.SetActive(true);
                //for (int i = 0; i < popupButtonsRT.Length; i++)
                //{
                //    popupButtonsRT[i].anchoredPosition = popupButtonsOriginalLocations[i];
                //}
            }


            popupButtons[0].SetupButton(ButtonFunctionType.PopupPanel.UnlockRecipe_None);
            popupButtons[1].SetupButton(activeAmount > 0 ? ButtonFunctionType.PopupPanel.UnlockRecipe_Chest : ButtonFunctionType.PopupPanel.UnlockReipe_Buy);
        }
    }
    */
    protected override string DefaultPopupHeader()
    {
        return DEFAULTPOPUPHEADER;
    }

    public void Unlock() // try to do it wich actions instead of switches !!!
    {
        switch (activePanelFunction)
        {
            case PanelFunction.SearchInChest:
                if (activeAmount > 0)
                {
                    Debug.Log(string.Format("Chest opening scene will be opened by {0}", activeChest.GetName()));
                }
                else
                {
                    Debug.Log(string.Format("shop will be openend"));
                }
                break;
            case PanelFunction.WorkerRequired:

                var requiredWorker = CharacterManager.CharactersAvailable_Dict[CharacterType.Type.Worker]
                                                    .FirstOrDefault(ch => ((Worker)ch).workerspecs.workerType == activeWorkerType);
                if(requiredWorker is not null)
                {
                    var panelLoadData = new PanelLoadData(mainLoadInfo: requiredWorker, panelHeader: null, tcs_IN: null);
                    PanelManager.ActivateAndLoad(invokablePanel_IN: PanelManager.InvokablePanels[typeof(HireCharacter_Panel)],
                                                 preLoadAction_IN: () => PanelManager.RemoveCurrentPanelFromNavigationStackIf(removeConditions: ipc => ipc.MainPanel is UnlockRecipePopupPanel),
                                                 panelLoadAction_IN: () => HireCharacter_Panel.Instance.LoadPanel(panelLoadData));
                }

                break;
        }
    }


    public override void UnloadAndDeallocate()
    {
        base.UnloadAndDeallocate();

        activePanelFunction = null;
        activeAmount = null;
        activeWorkerType = null;
        activeChest = null;
    }

}
