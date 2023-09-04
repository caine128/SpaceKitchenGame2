using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class Information_Modal_Panel : Panel_Base, IDeallocatable, IAanimatedPanelController_Cancellable, ITaskHandlerPanel, IVariableButtonPanel
{
    public InvokablePanelController InvokablePanelController { get; private set; }

    [SerializeField] private TextMeshProUGUI header;
    private RectTransform headerRT;
    private Vector2 headerOriginalPos;
    [SerializeField] private TextMeshProUGUI subHeader;
    [SerializeField] private TextMeshProUGUI secondarySubHeader;
    [SerializeField] private ContentDisplayModalPanel mainContentDisplay;
    [SerializeField] private ContentDisplayModalPanel[] subContentDisplays;
    [SerializeField] private ScrollableDisplayPopupPanel scrollableDisplayPanel;
    [SerializeField] private InformationModalPanelButton[] informationModalPanelButtons;

    private TextAnimate textReveal;
    private int displayedSubcontainerAmount;

    public bool IsAnimating
    {
        get
        {
            Debug.Log("getting is animating");
            return _isAnimating;
        }
        set
        {
            _isAnimating = value;
            Debug.Log("is animating value set to :" + value);
        }
    }

    private bool _isAnimating = false;

    public ConcurrentQueue<ModalLoadData> ModalLoadDataQueue
    {
        get => _modalLoadDataQueue;
    }
    private ConcurrentQueue<ModalLoadData> _modalLoadDataQueue = new ConcurrentQueue<ModalLoadData>();
    private ModalLoadData _currentModalLoadData;

    public ModalState State => _state;
    [SerializeField] private ModalState _state = ModalState.None;

    [Flags]
    public enum ModalState
    {
        None = 0,
        Enhancement = 1 << 0,
        Runestone_Success = 1 << 1,
        Elemental_Success = 1 << 2,
        Spirit_Success = 1 << 3,
        Failure = 1 << 4,
        ItemDismantle = 1 << 5,
        RecipeUpgrade = 1 << 6,
        AscensionUpgrade = 1 << 7,
        Char_Hired = 1 << 8,
        Char_Unlocked = 1 << 9,
        Char_Levelled = 1 << 10,
        ProductRecipe_UnlockedOrResearched = 1 << 11,
        WorkStationUpgrade = 1 << 12,
        GameItemRarityUpgrade = 1 << 13,
        RarityUpgradeAccepted = 1 << 14,

        //Should Put "Research Recipe"// BUt its not an upgrade its reaseach
    }

    private static string[] storedStrings = new string[]
        {
            "Successfull Enhancement !",
            "Unsuccesfull Enhancement =(",
            "Item Dismantled",
            "You have succesfully dismantled ",
            " And Received Following Ingredients",
            "Recipe Unlocked!",
            "Recipe Level Up!",
            "Recipe Ascended!",
            "Workstation Upgraded!"
        };

    public IEnumerator[] CO => _co;
    private IEnumerator[] _co;
    public GUI_LerpMethods PanelToAwait => _panelToAwait;
    [SerializeField] GUI_LerpMethods _panelToAwait; // IS THIS REALLY NECESSARY HERE JUST TO USE DISPLAYCONTAIERS ??

    private WaitWhile waitWhileScrollablePanelIsAnimating;

    public static Information_Modal_Panel Instance => _instance;

    public TaskCompletionSource<bool> TCS { get => _tcs; }

    public RectTransform[] PopupButtons_RT { get => _popupButtons_RT; }
    private RectTransform[] _popupButtons_RT;
    public Vector2[] PopupButtons_OriginalLocations { get => _popupButtons_OriginalLocations; }
    private Vector2[] _popupButtons_OriginalLocations;

    private TaskCompletionSource<bool> _tcs = null;

    private static Information_Modal_Panel _instance;
    private static readonly object _lock = new object();

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }

        else
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = this;
                }
            }
        }

        headerRT = header.GetComponent<RectTransform>();
        headerOriginalPos = headerRT.anchoredPosition;
        InvokablePanelController = GetComponent<InvokablePanelController>();
        textReveal = header.GetComponent<TextAnimate>();
        waitWhileScrollablePanelIsAnimating = new WaitWhile(() => scrollableDisplayPanel.IsAnimating == true);
        _co = new IEnumerator[5]; /// 0-is start routine / 1-is text routine / 2 - is the main image routine / 3 - subImages routine

    }

    public void ConfigureVariableButtons()
    {
        var arrayLength = informationModalPanelButtons.Length;
        _popupButtons_RT = new RectTransform[arrayLength];
        _popupButtons_OriginalLocations = new Vector2[arrayLength];

        for (int i = 0; i < arrayLength; i++)
        {
            _popupButtons_RT[i] = informationModalPanelButtons[i].GetComponent<RectTransform>();
            _popupButtons_OriginalLocations[i] = _popupButtons_RT[i].anchoredPosition;
        }
    }

    public void HandleTask(bool isTrue)
    {
        Debug.LogWarning(this.GetType() + "has its taskhandler working");
        if (_tcs != null && _tcs.TrySetResult(isTrue))
        {
            _tcs = null;
        }
    }

    public void LoadModalQueue(ModalLoadData modalLoadData_IN, IEnumerable<ModalLoadData> modalLoadDatas)
    {


        foreach (var loadData in modalLoadDatas)
        {
            _modalLoadDataQueue.Enqueue(loadData);
        }

        LoadModal(modalLoadData_IN);
    }


    private void LoadModal(ModalLoadData modalLoadData_IN)
    {
        HideContainers();

        _state = modalLoadData_IN.modalState;
        _currentModalLoadData = modalLoadData_IN;

        ArrangeModalLayoutFromModalState();

        switch (_state)
        {
            case var mainState when (mainState & ModalState.Enhancement) != 0:

                var enhancePanelLoadData = (ModalPanel_Enhancement_LoadData)_currentModalLoadData;


                switch (mainState)
                {
                    case var enhanceSubState when (enhanceSubState & ModalState.Failure) != 0:

                        header.text = NativeHelper.BuildString_Append(MethodHelper.GiveRichTextString_Color(Color.red), storedStrings[1]); ;
                        subHeader.text = GetFailedAttemptInfoString(enhancePanelLoadData.failedAttemptnfo.Value.failAmount, enhancePanelLoadData.failedAttemptnfo.Value.failedEnhanceableName, enhancePanelLoadData.failedAttemptnfo.Value.failedEnhancementName);

                        break;

                    case var enhanceSubState when
                    ((enhanceSubState & ModalState.Runestone_Success) != 0) ||
                    ((enhanceSubState & ModalState.Elemental_Success) != 0):
                        header.text = NativeHelper.BuildString_Append(MethodHelper.GiveRichTextString_Color(Color.green), storedStrings[0]);
                        subHeader.text = NativeHelper.BuildString_Append(GetSuccesfulAttemptInfoString(
                            enhancePanelLoadData.enhanceableInfo.succesAmount, enhancePanelLoadData.enhanceableInfo.enhanceable.GetName(), enhancePanelLoadData.enhancement.GetName()),
                            enhancePanelLoadData.failedAttemptnfo.HasValue ? GetFailedAttemptInfoString(enhancePanelLoadData.failedAttemptnfo.Value.failAmount) : String.Empty);

                        mainContentDisplay.Load(new ContentDisplayInfo_Modal(mainBluePrint_IN: enhancePanelLoadData.enhanceableInfo.enhanceable,
                                                                             spriteRef_IN: null,
                                                                             contentTextMain_IN: enhancePanelLoadData.enhanceableInfo.enhanceable.GetName(),
                                                                             contentTextSecondary_IN: (enhancePanelLoadData.enhanceableInfo.enhanceable as IQualitative).GetQuality().ToString(),
                                                                             dynamicShape_IN: ContentDisplayModalPanel.DynamicShape.Maincontainer));

                        displayedSubcontainerAmount = enhancePanelLoadData.enhancementBonuses.Count;
                        subContentDisplays.PlaceContainers(requiredAmount: displayedSubcontainerAmount,
                                                           containerWidth: subContentDisplays[0].OriginalSizeWideContainer.y,
                                                           isHorizontalPlacement: true);
                        subContentDisplays.LoadContainers(enhancePanelLoadData.enhancementBonuses,
                                                          hideAtInit: false,
                                                          dynamicShape_IN: ContentDisplayModalPanel.DynamicShape.Subcontainer_Square_Small,
                                                          isMaskActive: false,
                                                          bgColor: Color.grey);
                        break;

                    case var enhanceSubState when (enhanceSubState & ModalState.Spirit_Success) != 0:

                        header.text = NativeHelper.BuildString_Append(MethodHelper.GiveRichTextString_Color(Color.green), storedStrings[0]);
                        subHeader.text = NativeHelper.BuildString_Append(GetSuccesfulAttemptInfoString(
                            enhancePanelLoadData.enhanceableInfo.succesAmount, enhancePanelLoadData.enhanceableInfo.enhanceable.GetName(), enhancePanelLoadData.enhancement.GetName()),
                            enhancePanelLoadData.failedAttemptnfo.HasValue ? GetFailedAttemptInfoString(enhancePanelLoadData.failedAttemptnfo.Value.failAmount) : String.Empty);


                        mainContentDisplay.Load(new ContentDisplayInfo_Modal(mainBluePrint_IN: enhancePanelLoadData.enhanceableInfo.enhanceable, spriteRef_IN: null,
                            contentTextMain_IN: enhancePanelLoadData.enhanceableInfo.enhanceable.GetName(),
                            contentTextSecondary_IN: (enhancePanelLoadData.enhanceableInfo.enhanceable as IQualitative).GetQuality().ToString(),
                            dynamicShape_IN: ContentDisplayModalPanel.DynamicShape.Maincontainer));

                        displayedSubcontainerAmount = 1;
                        subContentDisplays.PlaceContainers(requiredAmount: displayedSubcontainerAmount,
                                                           containerWidth: subContentDisplays[0].OriginalSizeWideContainer.y,
                                                           isHorizontalPlacement: true);
                        subContentDisplays[0].Load(new ContentDisplayInfo_Modal(mainBluePrint_IN: enhancePanelLoadData.enhancement,
                                                                                spriteRef_IN: null,
                                                                                contentTextMain_IN: enhancePanelLoadData.enhancement.GetName(),
                                                                                contentTextSecondary_IN: enhancePanelLoadData.enhancement.GetQuality().ToString(),
                                                                                dynamicShape_IN: ContentDisplayModalPanel.DynamicShape.SubContainer_Wide,
                                                                                bgColor: Color.gray));
                        break;
                }
                break;

            case var mainstate when (mainstate & ModalState.ItemDismantle) != 0:

                var dismantleItemLoadData = (ModalPanel_DismantleItem_LoadData)_currentModalLoadData;
                var qualityLevelofDismantledItem = ((IQualitative)dismantleItemLoadData.craftableItem).GetQuality();

                header.text = NativeHelper.BuildString_Append(MethodHelper.GiveRichTextString_Color(Color.red), storedStrings[2]);
                subHeader.text = NativeHelper.BuildString_Append(storedStrings[3],
                                                                 MethodHelper.GiveRichTextString_Size(125),
                                                                 " ",
                                                                 qualityLevelofDismantledItem != Quality.Level.Normal
                                                                    ? NativeHelper.BuildString_Append(MethodHelper.GiveRichTextString_Color(NativeHelper.GetQualityColor(qualityLevelofDismantledItem)),
                                                                      dismantleItemLoadData.dismantleAmount.ToString(),
                                                                      " ",
                                                                      qualityLevelofDismantledItem.ToString(), " ")
                                                                    : dismantleItemLoadData.dismantleAmount.ToString(),
                                                                      " ",
                                                                 (dismantleItemLoadData.craftableItem).GetName(),
                                                                 MethodHelper.GiveRichTextString_ClosingTagOf("size"),
                                                                 qualityLevelofDismantledItem != Quality.Level.Normal
                                                                    ? MethodHelper.GiveRichTextString_ClosingTagOf("color")
                                                                    : String.Empty);
                secondarySubHeader.text = storedStrings[4];
                mainContentDisplay.Load(new ContentDisplayInfo_Modal(mainBluePrint_IN: dismantleItemLoadData.craftableItem,
                                                                     spriteRef_IN: null,
                                                                     contentTextMain_IN: String.Empty,
                                                                     contentTextSecondary_IN: String.Empty,
                                                                     dynamicShape_IN: ContentDisplayModalPanel.DynamicShape.Maincontainer));

                displayedSubcontainerAmount = dismantleItemLoadData.recycledResources.Count();
                subContentDisplays.PlaceContainers(requiredAmount: displayedSubcontainerAmount,
                                                   containerWidth: subContentDisplays[0].OriginalSizeWideContainer.y,
                                                   isHorizontalPlacement: true);
                subContentDisplays.LoadContainers(enhancementBonuses: dismantleItemLoadData.recycledResources.ConvertEnumerableOfTuples(converter1: x => x,
                                                                                                                                        converter2: x => $"+ {x}"),
                                                  hideAtInit: true,
                                                  dynamicShape_IN: ContentDisplayModalPanel.DynamicShape.Subcontainer_Square_Small,
                                                  isMaskActive: false,
                                                  bgColor: Color.cyan);
                break;


            case var mainstate when (mainstate & ModalState.RecipeUpgrade) != 0
                                 || (mainstate & ModalState.WorkStationUpgrade) != 0:

                var recipeUpgradeLoadData = (ModalPanel_DisplayBonuses_LoadData)_currentModalLoadData;

                header.text = ((mainstate & ModalState.RecipeUpgrade) != 0)
                                        ? NativeHelper.BuildString_Append(MethodHelper.GiveRichTextString_Color(Color.yellow), storedStrings[6])
                                        : NativeHelper.BuildString_Append(MethodHelper.GiveRichTextString_Color(Color.green), storedStrings[8]);

                subHeader.text = recipeUpgradeLoadData.subheaderString;
                mainContentDisplay.Load(new ContentDisplayInfo_Modal(mainBluePrint_IN: null,
                                                                     spriteRef_IN: recipeUpgradeLoadData.mainSprite,
                                                                     contentTextMain_IN: string.Empty,
                                                                     contentTextSecondary_IN: string.Empty,
                                                                     dynamicShape_IN: ContentDisplayModalPanel.DynamicShape.Maincontainer));

                displayedSubcontainerAmount = 1;
                subContentDisplays.PlaceContainers(requiredAmount: displayedSubcontainerAmount,
                                                   containerWidth: subContentDisplays[0].OriginalSizeWideContainer.y,
                                                   isHorizontalPlacement: true);
                subContentDisplays[0].Load(new ContentDisplayInfo_Modal(mainBluePrint_IN: null,
                                                                        spriteRef_IN: recipeUpgradeLoadData.secondarySprite,
                                                                        contentTextMain_IN: recipeUpgradeLoadData.bonusExplanationStringTuple.bonusExplanationString1,
                                                                        contentTextSecondary_IN: recipeUpgradeLoadData.bonusExplanationStringTuple.bonusExplanationString2,
                                                                        dynamicShape_IN: ContentDisplayModalPanel.DynamicShape.SubContainer_Wide,
                                                                        bgColor: Color.gray));

                break;

            case var mainstate when (mainstate & ModalState.AscensionUpgrade) != 0:
                var ascensionUpgradeLoadData = (ModalPanel_DisplayBonuses_Ascension_LoadData)_currentModalLoadData;

                header.text = NativeHelper.BuildString_Append(MethodHelper.GiveRichTextString_Color(Color.yellow), storedStrings[7]);
                subHeader.text = ascensionUpgradeLoadData.subheaderString;
                mainContentDisplay.Load(new ContentDisplayInfo_Modal(mainBluePrint_IN: null,
                                                                     spriteRef_IN: ascensionUpgradeLoadData.mainSprite,
                                                                     contentTextMain_IN: string.Empty,
                                                                     contentTextSecondary_IN: string.Empty,
                                                                     dynamicShape_IN: ContentDisplayModalPanel.DynamicShape.Maincontainer));
                displayedSubcontainerAmount = 4;
                subContentDisplays.PlaceContainers(requiredAmount: 3,
                                                   containerWidth: subContentDisplays[0].OriginalSizeWideContainer.y,
                                                   isHorizontalPlacement: true);
                subContentDisplays.LoadContainers(enhancementBonuses: FunctionalHelpers.CreateEnumerableOfTuple(iterationCount: displayedSubcontainerAmount,
                                                                                            factor1: iteration => iteration <= (int)ascensionUpgradeLoadData.ascensionLevel
                                                                                                        ? ImageManager.SelectSprite("StarIconYellow")
                                                                                                        : ImageManager.SelectSprite("StarIconRed"),
                                                                                            factor2: () => string.Empty),
                                                  hideAtInit: true,
                                                  dynamicShape_IN: ContentDisplayModalPanel.DynamicShape.Subcontainer_Square_Small,
                                                  isMaskActive: false,
                                                  bgColor: Color.gray);



                subContentDisplays[3].Load(new ContentDisplayInfo_Modal(mainBluePrint_IN: null,
                                                                        spriteRef_IN: ascensionUpgradeLoadData.secondarySprite,
                                                                        contentTextMain_IN: ascensionUpgradeLoadData.bonusExplanationStringTuple.bonusExplanationString1,
                                                                        contentTextSecondary_IN: ascensionUpgradeLoadData.bonusExplanationStringTuple.bonusExplanationString2,
                                                                        dynamicShape_IN: ContentDisplayModalPanel.DynamicShape.SubContainer_Wide,
                                                                        bgColor: new Color(1f, 1f, 1f, 0.21f)));

                break;

            case var mainstate when (mainstate & ModalState.Char_Unlocked) != 0:
                var characterLoadData_Unlock = (ModalPanel_DisplayBonuses_UnlockCharacter)_currentModalLoadData;

                header.text = $"{characterLoadData_Unlock.character.GetType()} Unlocked!";
                subHeader.text = $"{((Worker)characterLoadData_Unlock.character).workerspecs.workerType.ToString().Replace("_", " ")} is now available! ";
                secondarySubHeader.text = $"You can hire {characterLoadData_Unlock.character.GetName()} in the Characters Panel";
                mainContentDisplay.Load(new ContentDisplayInfo_Modal(mainBluePrint_IN: null,
                                                                     spriteRef_IN: characterLoadData_Unlock.character.GetAdressableImage(),
                                                                     contentTextMain_IN: string.Empty,
                                                                     contentTextSecondary_IN: string.Empty,
                                                                     dynamicShape_IN: ContentDisplayModalPanel.DynamicShape.Maincontainer));

                displayedSubcontainerAmount = 0;
                GUI_CentralPlacement.DeactivateUnusedContainers(0, subContentDisplays);

                break;

            case var mainState when (mainState & ModalState.Char_Hired) != 0:
                var characterLoadData_Hire = (ModalPanel_DisplayBonuses_HireCharacter)_currentModalLoadData;
                header.text = $"{characterLoadData_Hire.character.GetType()} Hired!";
                subHeader.text = $"{characterLoadData_Hire.character.GetName()} is now available!";
                secondarySubHeader.text = ((Worker)characterLoadData_Hire.character).workerspecs.unlockRecipes.Length > 0
                                                                    ? "You can cook new recipes!"
                                                                    : "You have the most powerful of all now";
                mainContentDisplay.Load(new ContentDisplayInfo_Modal(mainBluePrint_IN: null,
                                                                     spriteRef_IN: characterLoadData_Hire.character.GetAdressableImage(),
                                                                     contentTextMain_IN: string.Empty,
                                                                     contentTextSecondary_IN: string.Empty,
                                                                     dynamicShape_IN: ContentDisplayModalPanel.DynamicShape.Maincontainer));
                displayedSubcontainerAmount = characterLoadData_Hire.unlocks.Count();
                subContentDisplays.PlaceContainers(requiredAmount: displayedSubcontainerAmount,
                                                   containerWidth: subContentDisplays[0].OriginalSizeWideContainer.y * 2,
                                                   isHorizontalPlacement: true);
                subContentDisplays.LoadContainers(enhancementBonuses: characterLoadData_Hire.unlocks.ConvertEnumerable(converter: x => (x.receipeImageRef as AssetReferenceT<Sprite>, String.Empty)),
                                                  hideAtInit: true,
                                                  dynamicShape_IN: ContentDisplayModalPanel.DynamicShape.Subcontainer_Square_Big,
                                                  bgColor: Color.red);

                break;
            case var mainState when (mainState & ModalState.Char_Levelled) != 0:
                var characterLoadData_Level = (ModalPanel_DisplayBonuses_LevelUpCharacter)_currentModalLoadData;
                var selectedWorker = (Worker)characterLoadData_Level.character;
                header.text = "Worker Level Up!";
                subHeader.text = $"Congratulations! {selectedWorker.GetName()} reached Level {selectedWorker.GetLevel()}!";
                secondarySubHeader.text = characterLoadData_Level.unlocks.Any()
                                                        ? "you can NOW cook:"
                                                        : string.Empty;
                mainContentDisplay.Load(new ContentDisplayInfo_Modal(mainBluePrint_IN: null,
                                                                     spriteRef_IN: selectedWorker.GetAdressableImage(),
                                                                     contentTextMain_IN: string.Empty,
                                                                     contentTextSecondary_IN: string.Empty,
                                                                     dynamicShape_IN: ContentDisplayModalPanel.DynamicShape.Maincontainer));
                displayedSubcontainerAmount = 2; //characterLoadData_Level.unlocks.Count() + 2;

                subContentDisplays.PlaceContainers(requiredAmount: 2,
                                                   containerWidth: subContentDisplays[0].OriginalSizeWideContainer.y,
                                                   isHorizontalPlacement: false,
                                                   shiftDistance: -(mainContentDisplay.RT.anchoredPosition.y + mainContentDisplay.RTsToMove[2].rect.height / 2)); //mainContentDisplay.OriginalPosition.y);

                subContentDisplays[0].Load(new ContentDisplayInfo_Modal(mainBluePrint_IN: null,
                                                                        spriteRef_IN: selectedWorker.GetAdressableImage(),
                                                                        contentTextMain_IN: "Crafting Speed",
                                                                        contentTextSecondary_IN: $"{MethodHelper.GetValueStringPercent(selectedWorker.GetCraftTimeReduction(selectedWorker.GetLevel() - 1))} == {MethodHelper.GiveRichTextString_Color(Color.green)}{MethodHelper.GetValueStringPercent(selectedWorker.GetCraftTimeReduction())}{MethodHelper.GiveRichTextString_ClosingTagOf("color")}",
                                                                        dynamicShape_IN: ContentDisplayModalPanel.DynamicShape.SubContainer_Wide,
                                                                        bgColor: Color.gray));
                subContentDisplays[1].Load(new ContentDisplayInfo_Modal(mainBluePrint_IN: null,
                                                                        spriteRef_IN: selectedWorker.GetAdressableImage(),
                                                                        contentTextMain_IN: "XP To Next Level",
                                                                        contentTextSecondary_IN: selectedWorker.isAtMaxLevel
                                                                                                    ? $"{MethodHelper.GiveRichTextString_Color(Color.green)} Max Level {MethodHelper.GiveRichTextString_ClosingTagOf("color")}"
                                                                                                    : $"{selectedWorker.GetCurrentXP()} / {selectedWorker.GetXpToNextLevel()}",
                                                                        dynamicShape_IN: ContentDisplayModalPanel.DynamicShape.SubContainer_Wide,
                                                                        bgColor: Color.gray));

                if (characterLoadData_Level.unlocks.Any())
                {
                    var panelLoadDatas = new PanelLoadDatas(mainLoadInfo: selectedWorker,
                                                           panelHeader: "you can NOW cook:",
                                                           tcs_IN: null,
                                                           bluePrintsToLoad: characterLoadData_Level.unlocks.ConvertEnumerable(x => (x, ((IRankable)x).GetLevel())).ToList());
                    scrollableDisplayPanel.gameObject.SetActive(true);
                    scrollableDisplayPanel.Load(panelLoadDatas);
                }

                break;

            case var mainState when (mainState & ModalState.ProductRecipe_UnlockedOrResearched) != 0:
                var productRecipeLoadData = (ModalPanel_DisplayBonuses_ProductRecipeUnlockOrResearch)_currentModalLoadData;
                (string header, string subHeader) headers = (productRecipeLoadData.productRecipe.IsUnlocked(), productRecipeLoadData.productRecipe.IsResearched())
                                                             switch
                {
                    (true, false) => ("New Recipe Discovered!", "Research this recipe to start crafting it!"),
                    (true, true) => ("Recipe Researched!", "You can now start crafting it"),
                    _ => throw new NotImplementedException(),
                };

                header.text = headers.header;
                subHeader.text = headers.subHeader;
                secondarySubHeader.text = productRecipeLoadData.productRecipe.GetName();
                mainContentDisplay.Load(new ContentDisplayInfo_Modal(mainBluePrint_IN: null,
                                                                     spriteRef_IN: productRecipeLoadData.productRecipe.GetAdressableImage(),
                                                                     contentTextMain_IN: string.Empty,
                                                                     contentTextSecondary_IN: string.Empty,
                                                                     dynamicShape_IN: ContentDisplayModalPanel.DynamicShape.Maincontainer));

                displayedSubcontainerAmount = 0;
                GUI_CentralPlacement.DeactivateUnusedContainers(0, subContentDisplays);
                break;



            case var mainState when (mainState & ModalState.GameItemRarityUpgrade) != 0:
                var rarityUpgradeLoadData = (ModalPanel_GameItemRarityUpgrade)_currentModalLoadData;
                _tcs = rarityUpgradeLoadData.tcs;
                header.text = "Masterpiece!";
                subHeader.text = $"You have crafted an {((IQualitative)rarityUpgradeLoadData.gameItem).GetQuality()} item!";
                mainContentDisplay.Load(new ContentDisplayInfo_Modal(mainBluePrint_IN: null,
                                                                     spriteRef_IN: rarityUpgradeLoadData.gameItem.GetAdressableImage(),
                                                                     contentTextMain_IN: ((IQualitative)rarityUpgradeLoadData.gameItem).GetQuality().ToString(),
                                                                     contentTextSecondary_IN: rarityUpgradeLoadData.gameItem.GetName(),
                                                                     dynamicShape_IN: ContentDisplayModalPanel.DynamicShape.Maincontainer));

                var mealStatBonuses = ((IStatable)rarityUpgradeLoadData.gameItem).GetStatBonuses();
                displayedSubcontainerAmount = mealStatBonuses.Any()
                                                ? mealStatBonuses.Count() + 1
                                                : 1;

                if (displayedSubcontainerAmount > 0)
                {
                    subContentDisplays.PlaceContainers(requiredAmount: displayedSubcontainerAmount,
                                                       containerWidth: subContentDisplays[0].OriginalSizeWideContainer.x / 2,
                                                       isHorizontalPlacement: true);
                    subContentDisplays.LoadContainers(enhancementBonuses: mealStatBonuses.ExtractEnumerableOfTuples(extractor1: msb => msb.atlasedSpriteRef,
                                                                                                                    extractor2: msb => $"{msb.statType}{Environment.NewLine}{msb.statBonus}")
                                                                                         .AddToEnumerableOfTuples(factor1: () => ImageManager.SelectSprite("ValueIncrease"),
                                                                                                                  factor2: () => $"Value{Environment.NewLine}{ISpendable.ToScreenFormat(((IValuable)rarityUpgradeLoadData.gameItem).GetValue())}"),
                                                      hideAtInit: true,
                                                      dynamicShape_IN: ContentDisplayModalPanel.DynamicShape.SubContainer_Wide_Small,

                                                      bgColor: Color.grey);
                }


                Array.ForEach(informationModalPanelButtons, button => button.AssignModalLoadData(rarityUpgradeLoadData));
                var amountOfButtonsToDisplay = Quality.TryGetNextQualityLevel(((IQualitative)rarityUpgradeLoadData.gameItem).GetQuality(), out Quality.Level? nextQuality) == true
                                                                                    ? 2
                                                                                    : 1;
                ((IVariableButtonPanel)this).SetButtonLayout(amountOfButtonsToDisplay);
                switch (amountOfButtonsToDisplay)
                {
                    case 1:
                        informationModalPanelButtons[1].SetupButton(ButtonFunctionType.InformationModalPanel.Collect_Rarity);
                        break;
                    case 2:
                        informationModalPanelButtons[0].SetupButton(ButtonFunctionType.InformationModalPanel.Collect_Rarity);
                        informationModalPanelButtons[1].SetupButton(ButtonFunctionType.InformationModalPanel.Upgrade_Rarity);
                        break;
                    default:
                        throw new System.NotImplementedException();
                }
                break;

            default:
                Debug.LogError("another state  is loaded and its not covered");
                break;
        }
    }



    private void ArrangeModalLayoutFromModalState()
    {
        switch (_state)
        {
            case var resolvedstate when (resolvedstate & ModalState.RecipeUpgrade) != 0
                                     || (resolvedstate & ModalState.WorkStationUpgrade) != 0:
                if (headerRT.anchoredPosition != headerOriginalPos) headerRT.anchoredPosition = headerOriginalPos;
                if (secondarySubHeader.gameObject.activeInHierarchy != false) secondarySubHeader.gameObject.SetActive(false);
                if (informationModalPanelButtons.Any(button => button.gameObject.activeInHierarchy != false)) Array.ForEach(informationModalPanelButtons, button => button.gameObject.SetActive(false));

                PlaceSubcontentDisplays(new Vector2(
                                                    subContentDisplays[0].OriginalPosition.x,
                                                    subContentDisplays[0].OriginalPosition.y * 1.25f));
                break;

            case var resolvedState when (resolvedState & ModalState.Enhancement) != 0:

                if (headerRT.anchoredPosition != headerOriginalPos) headerRT.anchoredPosition = headerOriginalPos;
                if (secondarySubHeader.gameObject.activeInHierarchy != false) secondarySubHeader.gameObject.SetActive(false);
                if (informationModalPanelButtons.Any(button => button.gameObject.activeInHierarchy != false)) Array.ForEach(informationModalPanelButtons, button => button.gameObject.SetActive(false));

                switch (resolvedState)
                {
                    case var subResolvedState when (subResolvedState & ModalState.Runestone_Success) != 0
                    || (subResolvedState & ModalState.Elemental_Success) != 0:
                        PlaceSubcontentDisplays(new Vector2(
                                                    subContentDisplays[0].OriginalPosition.x,
                                                    subContentDisplays[0].OriginalPosition.y * 1.25f));
                        break;

                    case var subResolvedState when (subResolvedState & ModalState.Spirit_Success) != 0:
                        PlaceSubcontentDisplays(subContentDisplays[0].OriginalPosition);
                        break;
                }
                break;

            case var resolvedState when (resolvedState & ModalState.ItemDismantle) != 0
                                     || (resolvedState & ModalState.Char_Unlocked) != 0
                                     || (resolvedState & ModalState.Char_Hired) != 0
                                     || (resolvedState & ModalState.ProductRecipe_UnlockedOrResearched) != 0:
                if (headerRT.anchoredPosition == headerOriginalPos) headerRT.anchoredPosition = new Vector2(headerOriginalPos.x, headerOriginalPos.y / 8 * 9);
                if (secondarySubHeader.gameObject.activeInHierarchy != true) secondarySubHeader.gameObject.SetActive(true);
                if (informationModalPanelButtons.Any(button => button.gameObject.activeInHierarchy != false)) Array.ForEach(informationModalPanelButtons, button => button.gameObject.SetActive(false));

                PlaceSubcontentDisplays(new Vector2(
                                                    subContentDisplays[0].OriginalPosition.x,
                                                    subContentDisplays[0].OriginalPosition.y * 1.25f));

                break;

            case var resolvedSate when (resolvedSate & ModalState.Char_Levelled) != 0:
                if (headerRT.anchoredPosition == headerOriginalPos) headerRT.anchoredPosition = new Vector2(headerOriginalPos.x, headerOriginalPos.y / 8 * 9);
                if (!string.IsNullOrEmpty(secondarySubHeader.text) && secondarySubHeader.gameObject.activeInHierarchy != true) secondarySubHeader.gameObject.SetActive(true);
                if (informationModalPanelButtons.Any(button => button.gameObject.activeInHierarchy != false)) Array.ForEach(informationModalPanelButtons, button => button.gameObject.SetActive(false));

                PlaceSubcontentDisplays(new Vector2(
                                                    subContentDisplays[0].OriginalSizeWideContainer.x / 2,
                                                    mainContentDisplay.OriginalPosition.y));
                //subContentDisplays[0].OriginalPosition.x,
                //subContentDisplays[0].OriginalPosition.y * 1.25f));
                break;

            case var resolvedState when (resolvedState & ModalState.AscensionUpgrade) != 0:
                if (headerRT.anchoredPosition != headerOriginalPos) headerRT.anchoredPosition = headerOriginalPos;
                if (secondarySubHeader.gameObject.activeInHierarchy != false) secondarySubHeader.gameObject.SetActive(false);
                if (informationModalPanelButtons.Any(button => button.gameObject.activeInHierarchy != false)) Array.ForEach(informationModalPanelButtons, button => button.gameObject.SetActive(false));

                PlaceSubcontentDisplays(new Vector2(
                                                    subContentDisplays[0].OriginalPosition.x,
                                                    subContentDisplays[0].OriginalPosition.y * 1.25f));

                PlaceSubcontentDisplay(subContentDisplay: subContentDisplays[3],
                                       position_IN: new Vector2(
                                                                subContentDisplays[3].OriginalPosition.x,
                                                                subContentDisplays[3].OriginalPosition.y / 3.8f * 3));
                break;

            case var resolvedState when (resolvedState & ModalState.GameItemRarityUpgrade) != 0:
                if (headerRT.anchoredPosition != headerOriginalPos) headerRT.anchoredPosition = headerOriginalPos;
                if (secondarySubHeader.gameObject.activeInHierarchy != false) secondarySubHeader.gameObject.SetActive(false);

                Array.ForEach(informationModalPanelButtons, button => button.ScaleDirect(isVisible: false, finalValueOperations: null));

                PlaceSubcontentDisplays(new Vector2(
                                                    subContentDisplays[0].OriginalPosition.x,
                                                    subContentDisplays[0].OriginalPosition.y));

                break;
        }
    }

    private void PlaceSubcontentDisplays(Vector2 position_IN)
    {
        for (int i = 0; i < subContentDisplays.Length; i++)
        {
            PlaceSubcontentDisplay(subContentDisplays[i], position_IN);
        }
    }

    private void PlaceSubcontentDisplay(ContentDisplayModalPanel subContentDisplay, Vector2 position_IN)
    {
        if (!subContentDisplay.RT.anchoredPosition.Equals(position_IN))
        {

            if (subContentDisplay.RT.pivot != new Vector2(.5f, .5f))
                subContentDisplay.RT.pivot = new Vector2(.5f, .5f);

            subContentDisplay.RT.anchoredPosition = position_IN;
        }
    }


    private string GetFailedAttemptInfoString(int countOfAttempts, params string[] infoStrings)
    => _state switch
    {
        var mainstate when ((mainstate & ModalState.Enhancement) != 0 && (mainstate & ModalState.Failure) != 0) =>
            NativeHelper.BuildString_Append("Failed to enhance ",
                            MethodHelper.GiveRichTextString_Size(150), MethodHelper.GiveRichTextString_Color(Color.red),
                            countOfAttempts.ToString(), " ",
                            MethodHelper.GiveRichTextString_ClosingTagOf("size"), MethodHelper.GiveRichTextString_ClosingTagOf("color"),
                            infoStrings[0],
                            " with ",
                            infoStrings[1]),
        var mainstate when ((mainstate & ModalState.Enhancement)) != 0 =>
        NativeHelper.BuildString_Append(MethodHelper.GiveRichTextString_Color(Color.red), MethodHelper.GiveRichTextString_Size(sizePercenage: 80),
            Environment.NewLine, "you have ", countOfAttempts.ToString(), " failed enhancements as well =("),
        _ => throw new NotImplementedException(),
    };

    private string GetSuccesfulAttemptInfoString(int countOfAttempts, params string[] infoStrings)
        => _state switch
        {
            var mainstate when ((mainstate & ModalState.Enhancement)) != 0 =>
            NativeHelper.BuildString_Append(MethodHelper.GiveRichTextString_Color(Color.green), MethodHelper.GiveRichTextString_Size(sizePercenage: 150),
                countOfAttempts.ToString(), " ",
                MethodHelper.GiveRichTextString_ClosingTagOf("color"), MethodHelper.GiveRichTextString_ClosingTagOf("size"),
                infoStrings[0],
                countOfAttempts > 1 ? " are" : " is", " enhanced with ",
                infoStrings[1]),
            _ => throw new NotImplementedException(),
        };


    public void DisplayContainers()
    {
        if (_co[0] is not null)
        {
            StopCoroutine(_co[0]);
            _co[0] = null;
        }
        _co[0] = DisplayContainersRoutine();
        StartCoroutine(_co[0]);
    }

    private IEnumerator DisplayContainersRoutine()
    {
        textReveal.SetVisibility(isVisible: true);

        switch (_state)
        {
            case var mainState when (mainState & ModalState.Enhancement) != 0:  ///States of Enhancement
                switch (mainState)
                {
                    case var subState when (subState & ModalState.Failure) != 0:
                        textReveal.UpSize(followingAction_IN: () => IsAnimating = false);
                        yield return null;
                        break;

                    default:
                        textReveal.UpSize();
                        yield return textReveal.CO;

                        mainContentDisplay.AnimateWithRoutine(customInitialValue: null,
                                                              secondaryInterpolation: null,
                                                              isVisible: true,
                                                              lerpSpeedModifier: 1,
                                                              followingAction_IN: null);
                        yield return TimeTickSystem.WaitForSeconds_QuarterSec;
                        subContentDisplays.SortContainers(customInitialValues: null,
                                                          secondaryInterpolations: null,
                                                          amountToSort_IN: displayedSubcontainerAmount,
                                                          enumeratorIndex: 3,
                                                          parentPanel_IN: this,
                                                          customWaitInterval: TimeTickSystem.WaitForSeconds_EighthSec,
                                                          lerpSpeedModifiers: null,
                                                          followingAction: () => IsAnimating = false);
                        break;

                }
                break;

            case var mainstate when (mainstate & ModalState.ItemDismantle) != 0
            || (mainstate & ModalState.RecipeUpgrade) != 0
            || (mainstate & ModalState.Char_Unlocked) != 0
            || (mainstate & ModalState.Char_Hired) != 0
            || (mainstate & ModalState.ProductRecipe_UnlockedOrResearched) != 0
            || (mainstate & ModalState.WorkStationUpgrade) != 0:

                textReveal.UpSize();

                yield return textReveal.CO;

                mainContentDisplay.AnimateWithRoutine(customInitialValue: null,
                                                      secondaryInterpolation: null,
                                                      isVisible: true,
                                                      lerpSpeedModifier: 1,
                                                      followingAction_IN: displayedSubcontainerAmount > 0
                                                                                ? null
                                                                                : () => IsAnimating = false);
                yield return TimeTickSystem.WaitForSeconds_QuarterSec;

                subContentDisplays.SortContainers(customInitialValues: null,
                                                  secondaryInterpolations: null,
                                                  amountToSort_IN: displayedSubcontainerAmount,
                                                  enumeratorIndex: 3,
                                                  parentPanel_IN: this,
                                                  customWaitInterval: TimeTickSystem.WaitForSeconds_EighthSec,
                                                  lerpSpeedModifiers: null,
                                                  followingAction: () => IsAnimating = false);
                break;

            case var mainstate when (mainstate & ModalState.AscensionUpgrade) != 0:

                textReveal.UpSize();
                yield return textReveal.CO;

                mainContentDisplay.AnimateWithRoutine(customInitialValue: null,
                                                      secondaryInterpolation: null,
                                                      isVisible: true,
                                                      lerpSpeedModifier: 1,
                                                      followingAction_IN: null);
                yield return TimeTickSystem.WaitForSeconds_QuarterSec;

                var customInitialValues = new Vector3?[displayedSubcontainerAmount];
                var secondaryInterpolators = new (Action<float, RectTransform> interpolator, Action<RectTransform, bool> setValues)?[displayedSubcontainerAmount];
                var lerpSpeedModifiers = new float[displayedSubcontainerAmount];
                for (int i = 0; i < displayedSubcontainerAmount; i++)
                {
                    if (i == 0 || i == 1 || i == 2)
                    {
                        customInitialValues[i] = new Vector3(3, 3, 3);
                        secondaryInterpolators[i] = (FunctionalHelpers.quaternionRotation.Curry(360 * 2f),
                                                     FunctionalHelpers.setValuesofRotation.Curry(0f, 0f));
                        lerpSpeedModifiers[i] = 5f;
                    }
                    else
                    {
                        customInitialValues[i] = null;
                        lerpSpeedModifiers[i] = 1f;
                    }

                }

                subContentDisplays.SortContainers(customInitialValues: customInitialValues,
                                                  secondaryInterpolations: secondaryInterpolators,
                                                  amountToSort_IN: displayedSubcontainerAmount,
                                                  enumeratorIndex: 3,
                                                  parentPanel_IN: this,
                                                  customWaitInterval: TimeTickSystem.WaitForSeconds_EighthSec,
                                                  lerpSpeedModifiers: lerpSpeedModifiers,
                                                  followingAction: () => IsAnimating = false);



                break;

            case var mainstate when (mainstate & ModalState.Char_Levelled) != 0:
                textReveal.UpSize();
                yield return textReveal.CO;

                var containerTotalWidth = subContentDisplays[0].RT.rect.width;
                for (int i = 0; i < displayedSubcontainerAmount; i++)
                {
                    for (int j = 0; j < subContentDisplays[i].RTsToMove.Length; j++)
                    {
                        subContentDisplays[i].RTsToMove[j].anchoredPosition = new Vector2(subContentDisplays[i].RTsToMove[j].anchoredPosition.x - containerTotalWidth, subContentDisplays[i].RTsToMove[j].anchoredPosition.y);
                    }
                    subContentDisplays[i].ScaleDirect(isVisible: true, finalValueOperations: null);
                    subContentDisplays[i].gameObject.SetActive(true);
                }

                _co[4] = CRHelper.MoveRoutine(mainContentDisplay.RT,
                                              new Vector2(mainContentDisplay.RT.anchoredPosition.x - mainContentDisplay.RT.rect.width, mainContentDisplay.RT.anchoredPosition.y),
                                              lerpDuration: .1f,
                                              followingAction: () => _co[4] = null);

                mainContentDisplay.AnimateWithRoutine(customInitialValue: null,
                                                      secondaryInterpolation: null,
                                                      isVisible: true,
                                                      lerpSpeedModifier: 1,
                                                      followingAction_IN: () => StartCoroutine(_co[4]));

                yield return _co[4];

                for (int i = 0; i < displayedSubcontainerAmount; i++)
                {
                    _co[4] = CRHelper.MoveRoutine(rtInfos: subContentDisplays[i].RTsToMove.Select(rtm => (rtm, new Vector2(rtm.anchoredPosition.x + containerTotalWidth, rtm.anchoredPosition.y))),
                                         lerpDuration: .5f,
                                         followingAction: i == displayedSubcontainerAmount - 1
                                                            ? () =>
                                                            {
                                                                IsAnimating = false;
                                                                _co[4] = null;
                                                            }
                    : null);
                    StartCoroutine(_co[4]);    // NEED TO ENCAPSULATE THIS COROUTINE IN A VAR !!!

                    yield return TimeTickSystem.WaitForSeconds_EighthSec;
                }

                yield return _co[4];

                if (scrollableDisplayPanel.gameObject.activeInHierarchy)
                {
                    scrollableDisplayPanel.DisplayPanel();
                    yield return waitWhileScrollablePanelIsAnimating;
                }
                Debug.LogWarning("reached at the end of the routine ");

                break;

            case var mainState when (mainState & ModalState.GameItemRarityUpgrade) != 0:
                textReveal.UpSize();
                yield return textReveal.CO;

                mainContentDisplay.AnimateWithRoutine(customInitialValue: null,
                                                      secondaryInterpolation: null,
                                                      isVisible: true,
                                                      lerpSpeedModifier: 1,
                                                      followingAction_IN: displayedSubcontainerAmount > 0
                                                                                ? null
                                                                                : () => IsAnimating = false);
                yield return TimeTickSystem.WaitForSeconds_QuarterSec;

                subContentDisplays.SortContainers(customInitialValues: null,
                                                  secondaryInterpolations: null,
                                                  amountToSort_IN: displayedSubcontainerAmount,
                                                  enumeratorIndex: 3,
                                                  parentPanel_IN: this,
                                                  customWaitInterval: TimeTickSystem.WaitForSeconds_EighthSec,
                                                  lerpSpeedModifiers: null,
                                                  followingAction: () => Array.ForEach(informationModalPanelButtons, button => button.AnimateWithRoutine(customInitialValue: null,
                                                                                                secondaryInterpolation: null,
                                                                                                isVisible: true,
                                                                                                lerpSpeedModifier: 1,
                                                                                                followingAction_IN: informationModalPanelButtons.Length - 1 == Array.IndexOf(informationModalPanelButtons, button)
                                                                                                                    ? () => IsAnimating = false
                                                                                                                    : null)));
                break;

            default:
                Debug.LogError("this state should never occur");
                break;
        }
        _co[0] = null;
    }

    public void AcceptRarityUpgrade<T_Item>(T_Item gameItem, T_Item oldGameItem)
        where T_Item : GameObject, IStatable, IQualitative, IValuable
    {
        IsAnimating = true;
        _state = ModalState.RarityUpgradeAccepted;

        var buttonAmountsToDisplay = 1;
        Array.ForEach(informationModalPanelButtons, button => button.ScaleDirect(isVisible: false, finalValueOperations: null));
        ((IVariableButtonPanel)this).SetButtonLayout(buttonAmountToDisplay: buttonAmountsToDisplay);
        informationModalPanelButtons[1].SetupButton(ButtonFunctionType.InformationModalPanel.Collect_Rarity);

        subHeader.text = $"You have upgraded to a {gameItem.GetQuality()} item";

        var oldMealStatBonuses = oldGameItem.GetStatBonuses().Select(msb => (msb.statType, msb.statBonus)).ToArray();
        var newMealStatBonuses = gameItem.GetStatBonuses().Select(msb => (msb.statType, msb.statBonus)).ToArray();

        if (newMealStatBonuses.Length != oldMealStatBonuses.Length)
            throw new IndexOutOfRangeException();

        var tasks = new Task[newMealStatBonuses.Length + 1];

        for (int i = 0; i < newMealStatBonuses.Length; i++)
        {
            tasks[i] = subContentDisplays[i].ModifyText(identifier: newMealStatBonuses[i].statType.ToString(),
                                                        oldValue: oldMealStatBonuses[i].statBonus,
                                                        newValue: newMealStatBonuses[i].statBonus,
                                                        lerpSpeedModifier: 1);
            //subContentDisplays[i].ModifyText($"{newMealStatBonuses[i].statType}{Environment.NewLine}{newMealStatBonuses[i].statBonus}");
        }
        //subContentDisplays[newMealStatBonuses.Length].ModifyText($"Value{Environment.NewLine}{gameItem.GetValue()}");

        tasks[newMealStatBonuses.Length] = subContentDisplays[newMealStatBonuses.Length].ModifyText(identifier: "Value",
                                                                                                    oldValue: oldGameItem.GetValue(),
                                                                                                    newValue: gameItem.GetValue(),
                                                                                                    lerpSpeedModifier: 1);
        //subContentDisplays[1].ModifyText($"Value{Environment.NewLine}{gameItem.GetValue()}");

        mainContentDisplay.AnimateWithRoutine(customInitialValue: null,
                                              secondaryInterpolation: null,
                                              isVisible: false,
                                              lerpSpeedModifier: 1,
                                              followingAction_IN: null);

        mainContentDisplay.Load(new ContentDisplayInfo_Modal(mainBluePrint_IN: null,
                                                                     spriteRef_IN: gameItem.GetAdressableImage(),
                                                                     contentTextMain_IN: gameItem.GetQuality().ToString(),
                                                                     contentTextSecondary_IN: gameItem.GetName(),
                                                                     dynamicShape_IN: ContentDisplayModalPanel.DynamicShape.Maincontainer,
                                                                     bgColor: Color.green));

        Debug.LogWarning("isanimating : " + IsAnimating);
        mainContentDisplay.AnimateWithRoutine(customInitialValue: null,
                                              secondaryInterpolation: null,
                                              isVisible: true,
                                              lerpSpeedModifier: 1,
                                              followingAction_IN: () => Array.ForEach(informationModalPanelButtons, button => button.AnimateWithRoutine(customInitialValue: null,
                                                                                                                                                  secondaryInterpolation: null,
                                                                                                                                                  isVisible: true,
                                                                                                                                                  lerpSpeedModifier: 1,
                                                                                                                                                  followingAction_IN: informationModalPanelButtons.Length - 1 == Array.IndexOf(informationModalPanelButtons, button)
                                                                                                                                                        ? async () =>
                                                                                                                                                        {
                                                                                                                                                            await Task.WhenAll(tasks);
                                                                                                                                                            IsAnimating = false;
                                                                                                                                                        }
                                              : null))); ;
        Debug.LogWarning("isanimating : " + IsAnimating);
    }


    public void HideContainers()
    {
        textReveal.SetVisibility(isVisible: false);

        mainContentDisplay.ScaleDirect(isVisible: false, finalValueOperations: null);
        subContentDisplays.HideContainers();

        if (scrollableDisplayPanel.gameObject.activeInHierarchy != false) scrollableDisplayPanel.gameObject.SetActive(false);
    }

    public void FastForwardDisplayAnimation()
    {

        if (IsAnimating && ((_state & ModalState.Char_Levelled) == 0 || (_state & ModalState.RarityUpgradeAccepted) == 0))
        {
            textReveal.InstantFinaliseAndResolveVisibility(isVisible: true);


            mainContentDisplay.StopAnimateWithRoutine();
            mainContentDisplay.ScaleDirect(isVisible: true, finalValueOperations: null);

            for (int i = 0; i < displayedSubcontainerAmount; i++)
            {
                subContentDisplays[i].StopAnimateWithRoutine();

                if ((_state & ModalState.AscensionUpgrade) != 0 && i == 0 || i == 1 || i == 2)
                {
                    subContentDisplays[i].ScaleDirect(isVisible: true, finalValueOperations: (FunctionalHelpers.checkValueOFRotation.Curry(Vector3.zero),
                                                                                              FunctionalHelpers.setValueOfRotation.Curry(Vector3.zero)));
                }
                else
                {
                    subContentDisplays[i].ScaleDirect(isVisible: true, finalValueOperations: null);
                }
            }

            var activeinformationPanelButtons = informationModalPanelButtons.Where(button => button.gameObject.activeInHierarchy == true);
            foreach (var informationButton in activeinformationPanelButtons)
            {
                informationButton.StopAnimateWithRoutine();
                informationButton.ScaleDirect(isVisible: true, finalValueOperations: null);

            }
            IsAnimating = false;
        }
        Debug.LogWarning("isanimating is : " + IsAnimating);
    }

    public void UnloadAndDeallocate()
    {
        scrollableDisplayPanel.Unload();
        mainContentDisplay.Unload();
        ModalLoadDataQueue.Clear();
        foreach (var subContentDisplay in subContentDisplays)
        {
            subContentDisplay.Unload();
        }
        foreach (var informationModelPanelButton in informationModalPanelButtons)
        {
            informationModelPanelButton.UnloadButton();
        }
        _currentModalLoadData = null;
        _state = ModalState.None;
    }

}
