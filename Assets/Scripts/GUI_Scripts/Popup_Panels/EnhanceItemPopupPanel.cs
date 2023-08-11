using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class EnhanceItemPopupPanel : PopupPanel_Single_SNG<EnhanceItemPopupPanel>, IVariableButtonPanel, ISinglePanelInvokeButton //, ITaskHandlerPanel
{
    private static readonly string[] STATICNAMES = { "Value", "Enhance", "With" };

    private List<ContentDisplayInfo_PopupGeneric> _statBonusesInfo = new List<ContentDisplayInfo_PopupGeneric>(4); 


    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private ContentDisplayPopup_Generic[] contentDisplayEnhancePopups;
    [SerializeField] private RectTransform contentDisplayContainingRect;
    public CounterObjectScript CounterObject => counterObject;
    [SerializeField] private CounterObjectScript counterObject;

    public Enhancement Enhancement => enhancement;
    private Enhancement enhancement;

    public RectTransform[] PopupButtons_RT => popupButtonsRT;
    private RectTransform[] popupButtonsRT;
    public Vector2[] PopupButtons_OriginalLocations => popupButtonsOriginalLocations;

    public InvokablePanelController PanelToInvoke => _panelToInvoke;

    [SerializeField] InvokablePanelController _panelToInvoke;

    private Vector2[] popupButtonsOriginalLocations;

    private IVariableButtonPanel interface_variableButtonPanel;

    public IReassignablePanel.AssignedState? InventoryPanelStateToRevert => _inventorPanelStateToRevert;
    private IReassignablePanel.AssignedState? _inventorPanelStateToRevert;

    protected sealed override void Start()
    {
        base.Start();
        interface_variableButtonPanel = this;
    }

   /* public sealed override void ConfigureButtons()
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

    protected override string DefaultPopupHeader()
    {
        return enhancement.GetEnhancementType() switch
        {
            EnhancementType.Type.Runestone_Enhancement => "Runestone Enhancement",
            EnhancementType.Type.Elemental_Enhancement => "Elemental Enhancement",
            EnhancementType.Type.Spirit_Enhancement => "Spirit Enhancement",
            _ => throw new NotImplementedException(),
        };
    }

    public sealed override void LoadPanel(PanelLoadData panelLoadData)
    {
        var enhancePanelData = (PopupPanel_Enhancement_LoadData)panelLoadData;

        switch (bluePrint is null, enhancement is null, enhancePanelData.mainLoadInfo.Equals(bluePrint), enhancePanelData.enhancement.Equals(enhancement))
        {

            case (true, _, _, _):
            case (_, true, _, _):
            case (_, _, false, _):
            case (_, _, _, false):

                enhancement = enhancePanelData.enhancement;
                base.LoadPanel(enhancePanelData);

                var canBeEnhancedWith = ((IEnhanceable)bluePrint).CanEnhanceWith(enhancement.GetEnhancementType());

                descriptionText.text = canBeEnhancedWith
                    ? NativeHelper.BuildString_Append(STATICNAMES[1], " ", bluePrint.GetName(), " ", STATICNAMES[2], " ", enhancement.GetName())
                    : NativeHelper.BuildString_Append("Do you want to replace ", enhancement.GetName(), " on ", bluePrint.GetName(), " with another enhancement? ");

                break;

            default:
                break;
        }

        var maxAmount = Mathf.Min(((IAmountable)bluePrint).GetAmount(), enhancement.GetAmount());
        _inventorPanelStateToRevert = enhancePanelData.panelState;

        ArrangePanelAndButtonsSetup(_inventorPanelStateToRevert, ((IEnhanceable)bluePrint).CanEnhanceWith(enhancement.GetEnhancementType()), maxAmount);
    }

    public void UpdateBluePrint(SortableBluePrint newBlueprint)
    {
        bluePrint = newBlueprint;
    }

    public sealed override void DisplayContainers()
    {
        base.DisplayContainers();
        contentDisplayEnhancePopups.SortContainers(customInitialValues:null,
                                                   secondaryInterpolations: null,
                                                   amountToSort_IN: _statBonusesInfo.Count, 
                                                   enumeratorIndex: 0,
                                                   parentPanel_IN: this,
                                                   lerpSpeedModifiers: null);
    }

    private void ArrangePanelAndButtonsSetup(IReassignablePanel.AssignedState? panelState, bool canEnhanceWith, int maxAmount)
    {
        switch (panelState, canEnhanceWith, maxAmount)
        {

            case (null, _, _):                                                                   /// The State where it comes directly from EnhanceTabPanel, means it's just enhancement info
                interface_variableButtonPanel.SetButtonLayout(buttonAmountToDisplay: 2);
                UpdateStatBonusesLists(existingEnhancement:null);

                popupButtons[0].SetupButton(ButtonFunctionType.PopupPanel.EnhancementPanel_DestroyEnhancement);
                popupButtons[1].SetupButton(ButtonFunctionType.PopupPanel.EnhancemenPanel_ReplaceEnhancement);
                //if (counterObject.gameObject.activeInHierarchy != false) counterObject.gameObject.SetActive(false);
                counterObject.Initialize(maxAmount_IN: 1);
                break;
            case (IReassignablePanel.AssignedState.Inventory_FromProductToEnhance, false, _):   /// State where it comes from inventory panel selection but its a replace
                UpdateStatBonusesLists(existingEnhancement: ((IEnhanceable)bluePrint).enhancementsDict_ro[enhancement.GetEnhancementType()]);

                interface_variableButtonPanel.SetButtonLayout(buttonAmountToDisplay: 2);
                popupButtons[0].SetupButton(ButtonFunctionType.PopupPanel.Reject);
                popupButtons[1].SetupButton(ButtonFunctionType.PopupPanel.Confirm);
                //if (counterObject.gameObject.activeInHierarchy != false) counterObject.gameObject.SetActive(false);
                counterObject.Initialize(maxAmount_IN: 1);
                break;
            case (IReassignablePanel.AssignedState.Inventory_FromProductToEnhance, true, > 1):
                UpdateStatBonusesLists(existingEnhancement: null);

                interface_variableButtonPanel.SetButtonLayout(buttonAmountToDisplay: 2);
                popupButtons[0].SetupButton(ButtonFunctionType.PopupPanel.EnhancementPanel_ChangeSelection);
                popupButtons[1].SetupButton(ButtonFunctionType.PopupPanel.EnhancementPanel_Enhance);
                //if (counterObject.gameObject.activeInHierarchy != true) counterObject.gameObject.SetActive(true);
                counterObject.Initialize(maxAmount);
                break;
            case (IReassignablePanel.AssignedState.Inventory_FromProductToEnhance, true, <= 1):
                UpdateStatBonusesLists(existingEnhancement: null);

                interface_variableButtonPanel.SetButtonLayout(buttonAmountToDisplay: 2);
                popupButtons[0].SetupButton(ButtonFunctionType.PopupPanel.EnhancementPanel_ChangeSelection);
                popupButtons[1].SetupButton(ButtonFunctionType.PopupPanel.EnhancementPanel_Enhance);
                //if (counterObject.gameObject.activeInHierarchy != false) counterObject.gameObject.SetActive(false);
                counterObject.Initialize(maxAmount_IN: 1);
                break;
            case (IReassignablePanel.AssignedState.Inventory_FromEnhanceToProduct, _, > 1):
                UpdateStatBonusesLists(existingEnhancement: null);

                interface_variableButtonPanel.SetButtonLayout(buttonAmountToDisplay: 2);
                popupButtons[0].SetupButton(ButtonFunctionType.PopupPanel.EnhancementPanel_ChangeSelection);
                popupButtons[1].SetupButton(ButtonFunctionType.PopupPanel.EnhancementPanel_Enhance);
                //if (counterObject.gameObject.activeInHierarchy != true) counterObject.gameObject.SetActive(true);
                counterObject.Initialize(maxAmount);
                break;
            case (IReassignablePanel.AssignedState.Inventory_FromEnhanceToProduct, _, <= 1):
                UpdateStatBonusesLists(existingEnhancement: null);

                interface_variableButtonPanel.SetButtonLayout(buttonAmountToDisplay: 2);
                popupButtons[0].SetupButton(ButtonFunctionType.PopupPanel.EnhancementPanel_ChangeSelection);
                popupButtons[1].SetupButton(ButtonFunctionType.PopupPanel.EnhancementPanel_Enhance);
                //if (counterObject.gameObject.activeInHierarchy != false) counterObject.gameObject.SetActive(false);
                counterObject.Initialize(maxAmount_IN: 1);
                break;
            case (_, _, _):
                Debug.Log("This state should be unreachable");
                break;
        }

        contentDisplayEnhancePopups.PlaceContainersMatrix(_statBonusesInfo.Count, contentDisplayContainingRect);
        contentDisplayEnhancePopups.LoadContainers(_statBonusesInfo, hideAtInit: true);
    }

    /* private (AssetReferenceAtlasedSprite sprite, string statType, string statBonus_Existing, string statBonus_New)[] CompareEnhancementBonuseStats(Enhancement enhancementToReplace_IN)
     {
         //List<(AssetReferenceAtlasedSprite sprite, string statType, string statBonus, Enhancement enhancement)>_statBonusesLongList
         var enhanceble = (IEnhanceable)bluePrint;
         var arrayToReturn = new (AssetReferenceAtlasedSprite sprite, string statType, string statBonus_Existing, string statBonus_New)[_statBonusesInfo.Count];
         for (int i = 0; i < _statBonusesInfo.Count; i++)
         {
             var bonusInfo = _statBonusesInfo[i];
             if (i == 0)
             {
                 arrayToReturn[i] = (bonusInfo.spriteRef_IN, bonusInfo.contentTitle_IN, bonusInfo.contentValue_IN, string.Format("+ {0}", Enhance.GetMaxValueIncrease(enhanceble, enhancementToReplace_IN)));
             }
             else if (Enum.TryParse(bonusInfo.contentTitle_IN, ignoreCase: true, out Recipes_SO.MealStatType result))
             {
                 var newEnhacementStatBonuses = enhancementToReplace_IN.GetProductRecipe().recipeSpecs.mealStatBonuses;
                 foreach (var stat in newEnhacementStatBonuses)
                 {
                     if (result == stat.statType) arrayToReturn[i] = (bonusInfo.spriteRef_IN, bonusInfo.contentTitle_IN, bonusInfo.contentValue_IN, result.ToString());
                     break;
                 }
             }
             else if (i == _statBonusesInfo.Count - 1)
             {
                 arrayToReturn[i] = (bonusInfo.spriteRef_IN, bonusInfo.contentTitle_IN, bonusInfo.contentValue_IN, enhancementToReplace_IN.GetQuality().ToString());
             }
         }
         return arrayToReturn;
     }*/

    /*private List<ContentDisplayInfo_PopupGeneric> CompareEnhancementBonuseStats(Enhancement enhancementToReplace_IN)
    {
        //List<(AssetReferenceAtlasedSprite sprite, string statType, string statBonus, Enhancement enhancement)>_statBonusesLongList
        var enhanceble = (IEnhanceable)bluePrint;
        //var arrayToReturn = new (AssetReferenceAtlasedSprite sprite, string statType, string statBonus_Existing, string statBonus_New)[_statBonusesInfo.Count];
        var listToReturn = new List<ContentDisplayInfo_PopupGeneric>(_statBonusesInfo.Count);

        for (int i = 0; i < _statBonusesInfo.Count; i++)
        {
            var bonusInfo = _statBonusesInfo[i];
            if (i == 0)
            {
                listToReturn.Add(new ContentDisplayInfo_PopupGeneric(
                    spriteRef_IN: bonusInfo.spriteRef_IN,
                    contentTitle: bonusInfo.contentTitle_IN,
                    contentValue: bonusInfo.contentValue_IN + " > " + string.Format("+ {0}", Enhance.GetMaxValueIncrease(enhanceble, enhancementToReplace_IN)),
                    clickableObjectInfo_IN: null));

                //(bonusInfo.spriteRef_IN, bonusInfo.contentTitle_IN, bonusInfo.contentValue_IN, string.Format("+ {0}", Enhance.GetMaxValueIncrease(enhanceble, enhancementToReplace_IN)));
            }
            else if (Enum.TryParse(bonusInfo.contentTitle_IN, ignoreCase: true, out Recipes_SO.MealStatType result))
            {
                var newEnhacementStatBonuses = enhancementToReplace_IN.GetProductRecipe().recipeSpecs.mealStatBonuses;
                foreach (var stat in newEnhacementStatBonuses)
                {
                    if (result == stat.statType) listToReturn.Add(new ContentDisplayInfo_PopupGeneric(
                                                                      spriteRef_IN: bonusInfo.spriteRef_IN,
                                                                      contentTitle: bonusInfo.contentTitle_IN,
                                                                      contentValue: bonusInfo.contentValue_IN + " > " + result.ToString(),
                                                                      clickableObjectInfo_IN: null));

                    //arrayToReturn[i] = (bonusInfo.spriteRef_IN, bonusInfo.contentTitle_IN, bonusInfo.contentValue_IN, result.ToString());
                    break;
                }
            }
            else if (i == _statBonusesInfo.Count - 1)
            {
                listToReturn.Add(new ContentDisplayInfo_PopupGeneric(
                                  spriteRef_IN: bonusInfo.spriteRef_IN,
                                  contentTitle: bonusInfo.contentTitle_IN,
                                  contentValue: bonusInfo.contentValue_IN + " > " + enhancementToReplace_IN.GetQuality().ToString(),
                                  clickableObjectInfo_IN: null));

                // arrayToReturn[i] = (bonusInfo.spriteRef_IN, bonusInfo.contentTitle_IN, bonusInfo.contentValue_IN, enhancementToReplace_IN.GetQuality().ToString());
            }
        }
        return listToReturn;
    }*/
   /* private void UpdateStatBonusesListWith(Enhancement enhancementToReplace)
    {
        var enhanceble = (IEnhanceable)bluePrint;

        for (int i = 0; i < _statBonusesInfo.Count; i++)
        {
            var bonusInfo = _statBonusesInfo[i];

            if (i == 0)                                 /// This means the value bonus of the enhancement is being iterated over
            {
                var newMaxValueIncrease = Enhance.GetMaxValueIncrease(enhanceble, enhancementToReplace);

                _statBonusesInfo[i] = new ContentDisplayInfo_PopupGeneric(
                spriteRef_IN: bonusInfo.spriteRef_IN,
                contentTitle: bonusInfo.contentTitle_IN,
                contentValue: NativeHelper.BuildString_Append(bonusInfo.contentValue_IN, " > ", newMaxValueIncrease.ToString()),
                clickableObjectInfo_IN: bonusInfo.clickableInfoObject_IN);
            }
            else if (i == _statBonusesInfo.Count - 1)   /// This means the last bonus of the enhancement is being iterated over which is not a mealstatbonus
            {
                _statBonusesInfo[i] = new ContentDisplayInfo_PopupGeneric(
                spriteRef_IN: bonusInfo.spriteRef_IN,
                contentTitle: bonusInfo.contentTitle_IN,
                contentValue: NativeHelper.BuildString_Append(bonusInfo.contentValue_IN, " > ", enhancementToReplace.GetQuality().ToString()),
                clickableObjectInfo_IN: bonusInfo.clickableInfoObject_IN);
            }
            else                                        /// This means the bonus is a MEalStatBonus Type
            {
                var existingBonusType = Enum.TryParse(bonusInfo.contentTitle_IN, ignoreCase: true, out Recipes_SO.MealStatType result);
                var newBonusesArray = enhancementToReplace.GetProductRecipe().recipeSpecs.mealStatBonuses;

                foreach (var newBonus in newBonusesArray)
                {
                    if (newBonus.statType == result)
                    {
                        _statBonusesInfo[i] = new ContentDisplayInfo_PopupGeneric(
                        spriteRef_IN: bonusInfo.spriteRef_IN,
                        contentTitle: bonusInfo.contentTitle_IN,
                        contentValue: NativeHelper.BuildString_Append(bonusInfo.contentValue_IN, " > ", newBonus.statBonus.ToString()),
                        clickableObjectInfo_IN: bonusInfo.clickableInfoObject_IN);
                    }
                }
            }
        }
    }*/

    private void UpdateStatBonusesLists(Enhancement existingEnhancement)
    {
        var enhanceble = (IEnhanceable)bluePrint;
        if (_statBonusesInfo.Count != 0) _statBonusesInfo.Clear();       

        var bonusSprite = ImageManager.SelectSprite("ValueIncrease");
        var bonusString = existingEnhancement is not null
            ? NativeHelper.BuildString_Append("+ ",Enhance.GetMaxValueIncrease(enhanceble, existingEnhancement).ToString(), " > ","+ ", Enhance.GetMaxValueIncrease(enhanceble, enhancement).ToString())
            : NativeHelper.BuildString_Append("+ ",Enhance.GetMaxValueIncrease(enhanceble, enhancement).ToString());

            _statBonusesInfo.Add(new ContentDisplayInfo_PopupGeneric(
                                spriteRef_IN: bonusSprite,
                                contentTitle: STATICNAMES[0],
                                contentValue: bonusString,
                                GetTooltipText: null));

        foreach (var enhancementStatBonus in Enhance.GetRelevantEnhancementBonuses(enhanceable_IN: enhanceble, enhancement_IN: enhancement))
        {
            bonusSprite = enhancementStatBonus.atlasedSpriteRef;
            bonusString = existingEnhancement is not null
             ? NativeHelper.BuildString_Append("+ ", Enhance.GetStatBonusByStatType(existingEnhancement, enhancementStatBonus.statType).statBonus.ToString(), " > ", "+ ", enhancementStatBonus.statBonus.ToString())
             : NativeHelper.BuildString_Append("+ ", enhancementStatBonus.statBonus.ToString());
            
            _statBonusesInfo.Add(new ContentDisplayInfo_PopupGeneric(
                               spriteRef_IN: bonusSprite,
                               contentTitle: enhancementStatBonus.statType.ToString(),
                               contentValue: bonusString,
                               GetTooltipText: null));
        }

        bonusSprite = enhancement.GetAdressableImage();
        bonusString = existingEnhancement is not null
            ? NativeHelper.BuildString_Append(existingEnhancement.GetQuality().ToString() ," > ", enhancement.GetQuality().ToString())
            : enhancement.GetQuality().ToString();

        _statBonusesInfo.Add(new ContentDisplayInfo_PopupGeneric(
                           spriteRef_IN: bonusSprite,
                           contentTitle: enhancement.GetName(),
                           contentValue: bonusString,
                           GetTooltipText: null));
    }



    /* private void FindIntersectingStatBonuses()
     {
         var enhanceble = (IEnhanceable)bluePrint;
         //var statBonuses = new List<(AssetReferenceAtlasedSprite sprite, string statType, string statBonus, Enhancement enhancement)>(4);
         if (_statBonusesInfo.Count != 0) _statBonusesInfo.Clear();
         if (_statBonusesShortList.Count != 0) _statBonusesShortList.Clear();

         //statBonusesShortList = new List<(AssetReferenceAtlasedSprite spriteRef, string enhancmentBonus)>(6);

         var itemStats = enhanceble.GetProductRecipe().recipeSpecs.mealStatBonuses;
         var enhancementStats = enhancement.GetProductRecipe().recipeSpecs.mealStatBonuses;


         var spriteToAdd_A = ImageManager.SelectSprite("ValueIncrease");
         var statBonusString_A = string.Format("+ {0}", Enhance.GetMaxValueIncrease(enhanceble, enhancement));

         _statBonusesInfo.Add(new ContentDisplayInfo_PopupGeneric(
             spriteRef_IN: spriteToAdd_A,
             contentTitle: STATICNAMES[0],
             contentValue: statBonusString_A,
             clickableObjectInfo_IN: null));

         //_statBonusesInfo.Add((spriteToAdd_A, STATICNAMES[0], statBonusString_A, null));

         var newInfoNode = new ContentDisplayInfo_PopupGeneric(spriteRef_IN: _statBonusesLongList[i].sprite,
                         contentTitle: _statBonusesLongList[i].statType, contentValue: _statBonusesLongList[i].statBonus,
                         clickableObjectInfo_IN: _statBonusesLongList[i].enhancement);

         contentDisplayLoadInfo.Add(newInfoNode);

         //statBonuses.Add((spriteToAdd_A, STATICNAMES[0], statBonusString_A, null));
         _statBonusesShortList.Add((spriteToAdd_A, statBonusString_A));
         //statBonusesShortList.Add((spriteToAdd_A, statBonusString_A));

         if (enhancementStats is not null && itemStats is not null)
         {
             foreach (var itemStat in itemStats)
             {
                 foreach (var enhancementStat in enhancementStats)
                 {
                     var statTypeName = enhancementStat.statType.ToString();
                     if (itemStat.statType.Equals(enhancementStat.statType))
                     {
                         var spriteToAdd_B = ImageManager.SelectSprite(statTypeName);
                         var statBonusString_B = string.Format("+ {0}", enhancementStat.statBonus.ToString());

                         _statBonusesInfo.Add(new ContentDisplayInfo_PopupGeneric(
                            spriteRef_IN: spriteToAdd_B,
                            contentTitle: statTypeName,
                            contentValue: statBonusString_B,
                            clickableObjectInfo_IN: null));

                         //_statBonusesLongList.Add((spriteToAdd_B, statTypeName, statBonusString_B, null));
                         //statBonuses.Add((spriteToAdd_B, statTypeName, statBonusString_B, null));
                         _statBonusesShortList.Add((spriteToAdd_B, statBonusString_B));
                         //statBonusesShortList.Add((spriteToAdd_B, statBonusString_B));
                     }
                 }
             }
         }

         var spriteToAdd_C = enhancement.GetAdressableImage();
         var statBonusString_C = enhancement.GetQuality().ToString();

         _statBonusesInfo.Add(new ContentDisplayInfo_PopupGeneric(
                            spriteRef_IN: spriteToAdd_C,
                            contentTitle: enhancement.GetName(),
                            contentValue: statBonusString_C,
                            clickableObjectInfo_IN: enhancement));

         //_statBonusesLongList.Add((spriteToAdd_C, enhancement.GetName(), statBonusString_C, enhancement));
         //statBonuses.Add((spriteToAdd_C, enhancement.GetName(), statBonusString_C, enhancement));
         _statBonusesShortList.Add((spriteToAdd_C, statBonusString_C));
         //statBonusesShortList.Add((spriteToAdd_C, statBonusString_C));

         //return statBonuses;
     }*/

    /*  public async Task<bool> ConfirmAndDestroyEnhancementAsync(Enhancement enhancementToReplace_IN)
      {
          /// enhancementToReplace is used when destroy action will follow a replace threfore we need different info to be displayed as confirmation///
          var enhanceable = bluePrint as IEnhanceable;
          var extraContentDisplaysList = enhancementToReplace_IN is not null ? CompareEnhancementBonuseStats(enhancementToReplace_IN) : null;

          if (enhanceable?.CanEnhanceWith(enhancement.GetEnhancementType()) == false)
          {
              var enhancementQuality = enhancement.GetQuality();

              var tcs = new TaskCompletionSource<bool>();

              if (_panelToInvoke.MainPanel is ConfirmationPopupPanel)
              {
                  var panelLoadData = new PopupPanel_Confirmation_LoadData(
                      mainLoadInfo: bluePrint,
                      panelHeader: null,
                      tcs_IN: tcs,
                      bluePrintsToLoad: enhancementToReplace_IN is not null
                          ? new List<(GameItem enhancement, int amountToLoad)> { (enhancement, 1), (enhancementToReplace_IN, 1) }
                          : new List<(GameItem enhancement, int amountToLoad)> { (enhancement, 1) },
                      extraDescription_IN: NativeHelper.BuildString_Append(
                      enhancementToReplace_IN is not null
                          ? "Are you sure to change "
                          : "Are you sure to destroy",
                      enhancementQuality != Quality.Level.Normal
                          ? MethodHelper.GiveRichTextString_Color(NativeHelper.GetQualityColor(enhancementQuality)) + MethodHelper.GiveRichTextString_Size(125)
                          : String.Empty,
                      enhancementQuality.ToString(), " ", enhancement.GetName(),
                      enhancementToReplace_IN is not null
                          ? NativeHelper.BuildString_Append(
                              MethodHelper.GiveRichTextString_ClosingTagOf("color"), MethodHelper.GiveRichTextString_ClosingTagOf("size"), " with ",
                                  enhancementToReplace_IN.GetQuality() != Quality.Level.Normal
                                  ? MethodHelper.GiveRichTextString_Color(NativeHelper.GetQualityColor(enhancementToReplace_IN.GetQuality())) + MethodHelper.GiveRichTextString_Size(125)
                                  : String.Empty,
                              enhancementToReplace_IN.GetQuality().ToString(), " ", enhancementToReplace_IN.GetName())
                          //Environment.NewLine,
                          //enhancementsBonusCompareArray[0].statType, "will change to ", enhancementsBonusCompareArray[0].statBonus_Existing, " ==> ", enhancementsBonusCompareArray[0].statBonus_New,
                          //Environment.NewLine,
                          //enhancementsBonusCompareArray[1].statType, "will change to ", enhancementsBonusCompareArray[1].statBonus_Existing, " ==> ", enhancementsBonusCompareArray[1].statBonus_New)
                          //enhancement.GetProductRecipe().recipeSpecs.mealStatBonuses[0].statBonus.ToString(), " ==> ", enhancementToReplace_IN.GetProductRecipe().recipeSpecs.mealStatBonuses[0].statBonus.ToString())
                          : string.Empty),
                      extraContentDisplaysData_IN: extraContentDisplaysList);

                  var invokablePanel = PanelManager.InvokablePanels[typeof(ConfirmationPopupPanel)];
                  //Later to take this chec in the if statement or maybe even better without an if statement !!!!
                  //Also CAN GET RID OF THE SUCKY CAST BELOW !!
                  //Also can get rid of the invokablepanel Varibales Totally!
                  PanelManager.ActivateAndLoad(
                      invokablePanel_IN: invokablePanel, //_panelToInvoke, 
                      panelLoadAction_IN: () => ((ConfirmationPopupPanel)invokablePanel.MainPanel).LoadPanel(panelLoadData));  //ConfirmationPopupPanel.Instance.LoadPanel(panelLoadData));


              }

              await tcs.Task;
              if (tcs.Task.Result == false) return false;


              enhanceable = enhanceable.DestroyEnhancement(enhancement);
              bluePrint = Inventory.InventoryLookupDict_ByItem[(Product)enhanceable];   // this is to set the blueprint correctly, after deletinghe enhancement
          }
          return true;
      }

     /* public async Task<bool> IsOverEnhanceConfirmed()
      {
          var enhanceable = bluePrint as IEnhanceable;

          Debug.LogError("qualiy of enhancement is : " + enhancement.GetQuality().ToString() + " quality of enheanceable is : " + enhanceable.GetQuality().ToString());

          if ((int)enhancement.GetQuality() > (int)enhanceable.GetQuality())
          {
              var enhanceableQuality = enhanceable.GetQuality();
              var enhancementQuality = enhancement.GetQuality();

              var tcs = new TaskCompletionSource<bool>();

              if (_panelToInvoke.MainPanel is ConfirmationPopupPanel)
              {

                  var panelLoadData = new PopupPanel_Confirmation_LoadData(
                      mainLoadInfo: enhancement,
                      panelHeader: null,
                      tcs_IN: tcs,
                      bluePrintsToLoad: new List<(GameItem blueprintToLoad, int amountToLoad)> { (enhancement, counterObject.CurrentAmount) },
                      extraDescription_IN: NativeHelper.BuildString_Append(
                          enhancementQuality != Quality.Level.Normal ? MethodHelper.GiveRichTextString_Color(NativeHelper.GetQualityColor(enhancementQuality)) + MethodHelper.GiveRichTextString_Size(125) : String.Empty,
                          enhancementQuality.ToString(), " ", enhancement.GetName(),
                          enhancementQuality != Quality.Level.Normal ? MethodHelper.GiveRichTextString_ClosingTagOf("color") + MethodHelper.GiveRichTextString_ClosingTagOf("size") : String.Empty,
                          " is above the quality of ",
                          enhanceableQuality != Quality.Level.Normal ? MethodHelper.GiveRichTextString_Color(NativeHelper.GetQualityColor(enhanceableQuality)) + MethodHelper.GiveRichTextString_Size(125) : String.Empty,
                          enhanceableQuality.ToString(), " ", bluePrint.GetName(),
                          enhanceableQuality != Quality.Level.Normal ? MethodHelper.GiveRichTextString_ClosingTagOf("color") + MethodHelper.GiveRichTextString_ClosingTagOf("size") : String.Empty,
                          Environment.NewLine,
                          "Are you sure do you want to spend your enhancement?"),
                      extraContentDisplaysData_IN: null);

                  var invokablePanel = PanelManager.InvokablePanels[typeof(ConfirmationPopupPanel)];
                  //Later to take this chec in the if statement or maybe even better without an if statement !!!!
                  //Also CAN GET RID OF THE SUCKY CAST BELOW !!
                  //Also can get rid of the invokablepanel Varibales Totally!
                  PanelManager.ActivateAndLoad(
                      invokablePanel_IN: invokablePanel, // _panelToInvoke, 
                      panelLoadAction_IN: () => ((ConfirmationPopupPanel)invokablePanel.MainPanel).LoadPanel(panelLoadData));  //ConfirmationPopupPanel.Instance.LoadPanel(panelLoadData));
              }

              await tcs.Task;
              if (tcs.Task.Result == false) return false;
          }
          return true;
      }*/

    public void PerformEnhancementAndDisplayModal()
    {

        int listCapacityEnhancedProducts = counterObject.CurrentAmount;
        List<IEnhanceable> enhancedProducts = new List<IEnhanceable>(listCapacityEnhancedProducts);

        var enhanceableCandidate = bluePrint; // TP prevent enhancing same product on multiple enhancement beucase "blueprint" is getting updated when first enhancement is made.

        for (int i = 0; i < listCapacityEnhancedProducts; i++)
        {
            
            IEnhanceable enhancedProduct = ((IEnhanceable)enhanceableCandidate).TryEnhance(enhancement, out bool isEnhancementSuccessful);
            if (isEnhancementSuccessful) enhancedProducts.Add(enhancedProduct);
        }
        int successfulEnhancementCount = enhancedProducts.Count;

        var modalLoadData = new ModalPanel_Enhancement_LoadData(
            enhanceableInfo_IN: successfulEnhancementCount > 0 ? (enhancedProducts[0], successfulEnhancementCount) : (null, successfulEnhancementCount),
            enhancement_IN: successfulEnhancementCount > 0 ? enhancement : null,
            failedAttemptnfo_IN: listCapacityEnhancedProducts - successfulEnhancementCount > 0 ? (bluePrint.GetName(), enhancement.GetName(), listCapacityEnhancedProducts - successfulEnhancementCount) : null,
            enhancementBonuses_IN: ConvertStatBonusesList().ToList(),
            modalState_IN: (listCapacityEnhancedProducts - successfulEnhancementCount, enhancement.GetEnhancementType()) switch
            {
                (int x, _) when x == listCapacityEnhancedProducts => Information_Modal_Panel.ModalState.Enhancement | Information_Modal_Panel.ModalState.Failure, // one fail state is enough, total failure                             
                (_, EnhancementType.Type.Runestone_Enhancement) => Information_Modal_Panel.ModalState.Enhancement | Information_Modal_Panel.ModalState.Runestone_Success,
                (_, EnhancementType.Type.Elemental_Enhancement) => Information_Modal_Panel.ModalState.Enhancement | Information_Modal_Panel.ModalState.Elemental_Success,
                (_, EnhancementType.Type.Spirit_Enhancement) => Information_Modal_Panel.ModalState.Enhancement | Information_Modal_Panel.ModalState.Spirit_Success,
                _ => throw new NotImplementedException(),
            });;;
        
        PanelManager.DeactivatePanel(PanelManager.SelectedPanels.Peek(),
            unloadAction: () =>
            {
                PanelManager.ClearStackAndDeactivateElements();
                PanelManager.SelectedPanels.Push(Information_Modal_Panel.Instance.InvokablePanelController);
                //Information_Modal_Panel.Instance.IsAnimating = true;
            },
            nextPanelLoadAction_IN: () => Information_Modal_Panel.Instance.LoadModalQueue(modalLoadData,Enumerable.Empty<ModalLoadData>()));
    }

    private IEnumerable<(AssetReferenceT<Sprite> spriteRef, string enhancmentBonus)> ConvertStatBonusesList ()
    {
        foreach (var statBonus in _statBonusesInfo)
        {
            yield return (statBonus.spriteRef_IN, statBonus.contentValue_IN);
        }
    }






    //public void HandleTask(bool isTrue)
    //{
    //    if (tcs != null && tcs.TrySetResult(isTrue))
    //    {
    //        tcs = null;
    //    }
    //}


    /// <summary>
    /// TODO :  Needto make the unloading as well !! And there should clear the secondReadonlylist which I made with out 
    /// </summary>

    public sealed override void UnloadAndDeallocate()
    {
        bluePrint = null;
        enhancement = null;

        for (int i = 0; i < contentDisplayEnhancePopups.Length; i++)
        {
            contentDisplayEnhancePopups[i].Unload();
        }

        base.UnloadAndDeallocate();
    }


}
