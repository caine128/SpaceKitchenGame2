using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameItemInfoPanel_Manager : TabbedPanel<GameObject, Tab.GameItemInfoTabs>, IVariableTabDisplay //, IRefreshablePanel
{
    private static GameItemInfoPanel_Manager _instance;
    public static GameItemInfoPanel_Manager Instance { get { return _instance; } }
    private static readonly object _lock = new object();

    [SerializeField] private Image qualityBannerBG;
    [SerializeField] private PanelInvokeButton toRecipeButton;
    [SerializeField] private PanelInvokeButton deleteButton;
    [SerializeField] private PanelInvokeButton useEnhancementButton;
    public int TabAmountToDisplay { get; private set; }

    public override List<GameObject> ListToIterate
    {
        get
        {
            if (SelectedRecipe is Product)
            {
                Debug.Log("listto iterate is returned");
                return Inventory.InventoryIterationDict[GameItemType.Type.Product];
            }
            else if (SelectedRecipe is SpecialItem)
            {
                return Inventory.InventoryIterationDict[GameItemType.Type.SpecialItem];
            }
            else if (SelectedRecipe is ExtraComponent)
            {
                return Inventory.InventoryIterationDict[GameItemType.Type.ExtraComponents];
            }
            else if (SelectedRecipe is Enhancement)
            {
                return Inventory.InventoryIterationDict[GameItemType.Type.Enhancement];
            }
            else
            {
                Debug.LogWarning("the list shouldnt return null !");
                return null;
            }
        }
    }

    protected void Awake()
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
                    //DontDestroyOnLoad(this.gameObject);   /// This will be done later
                }
            }
        }
    }

    public override void InitialConfigBrowserButtons()
    {
        foreach (var browserButton in _browserButtons)
        {
            browserButton.ButtonConfig(Instance);
        }
    }


    protected override void LoadInfo(GameObject bluePrint_IN)  // make this private !!! and run it from the panelinvoke as much as you can !
    {
        TabAmountToDisplay = bluePrint_IN is Product ? 2 : 1;

        if (SelectedRecipe is null || !SelectedRecipe.Equals(bluePrint_IN)) //(selectedRecipe !=bluePrint_IN)
        {
            SelectedRecipe = bluePrint_IN;

            blueprintNameText.text = SelectedRecipe.GetName();
            blueprintTypeText.text = SelectedRecipe.ToString();

            var craftable = SelectedRecipe as ICraftable;
            Configure_ToRecipeButton(craftable);
            Configure_DeleteButton(craftable);
            Configure_UseEnhancementButton(bluePrint_IN as Enhancement);


            if (bluePrint_IN is Product productBluePrint)
            {
                if (bluePrintTypeLevelInfoText.enabled != true || blueprintRarityText.enabled != true || qualityBannerBG.enabled != true)
                {
                    bluePrintTypeLevelInfoText.enabled = blueprintRarityText.enabled = qualityBannerBG.enabled = true;
                }

                bluePrintTypeLevelInfoText.text = string.Format("Level {0} {1}", productBluePrint.GetLevel(), productBluePrint.GetProductRecipe().recipeSpecs.productType.ToString());
                blueprintRarityText.text = productBluePrint.GetQuality().ToString();

                //TabAmountToDisplay = 2;
            }

            else if (bluePrint_IN is ExtraComponent || bluePrint_IN is SpecialItem)
            {
                if (bluePrintTypeLevelInfoText.enabled != false || blueprintRarityText.enabled != false || qualityBannerBG.enabled != false)
                {
                    bluePrintTypeLevelInfoText.enabled = blueprintRarityText.enabled = qualityBannerBG.enabled = false;
                    bluePrintTypeLevelInfoText.text = blueprintRarityText.text = null;
                }
            }

            else if (bluePrint_IN is Enhancement enhancementBluePrint)
            {
                if (bluePrintTypeLevelInfoText.enabled != true || blueprintRarityText.enabled != true || qualityBannerBG.enabled != true)
                {
                    bluePrintTypeLevelInfoText.enabled = blueprintRarityText.enabled = qualityBannerBG.enabled = true;
                }

                bluePrintTypeLevelInfoText.text = string.Format("Level {0} {1}", enhancementBluePrint.GetLevel(), enhancementBluePrint.GetProductRecipe().recipeSpecs.productType.ToString());
                blueprintRarityText.text = enhancementBluePrint.GetQuality().ToString();
            }

            ArrangeTabButtons(TabAmountToDisplay);
        }


        var adressableImage = SelectedRecipe.GetAdressableImage();
        bigImageContainer_Adressable.LoadSprite(adressableImage);
        thumbnailImageContainer_Adressable.LoadSprite(adressableImage);

        for (int i = 0; i < TabAmountToDisplay; i++)
        {
            tabPanels[i].LoadInfo();
        }

        var browsablePanelInterface = ((IBrowsablePanel<GameObject>)this);
        
        _currentIndice = browsablePanelInterface.SetCurrentIndice(SelectedRecipe);
        browsablePanelInterface.SetVisibilityBrowserButtons();

        int lastSelectionToInvoke = lastSelectionIndex < TabAmountToDisplay ? lastSelectionIndex : 0;
        ExecuteEvents.Execute(tabSelectorButtons[lastSelectionToInvoke].gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerDownHandler);

    }

    private void Configure_ToRecipeButton(ICraftable craftableItem)
    {
        if (craftableItem == null)
        {
            if (toRecipeButton.gameObject.activeSelf != false) toRecipeButton.gameObject.SetActive(false);
            return;
        }
        else if (RecipeManager.Instance.GetRootRecipe(craftableItem) == null && toRecipeButton.gameObject.activeSelf != false)
        {
            toRecipeButton.gameObject.SetActive(false);
        }
        else if (RecipeManager.Instance.GetRootRecipe(craftableItem) != null && toRecipeButton.gameObject.activeSelf != true)
        {
            toRecipeButton.gameObject.SetActive(true);
        }
    }

    private void Configure_DeleteButton(ICraftable craftableItem)
    {
        if(craftableItem == null)
        {
            if (deleteButton.gameObject.activeSelf != false) deleteButton.gameObject.SetActive(false);
        }
        else
        {
            if (deleteButton.gameObject.activeSelf != true) deleteButton.gameObject.SetActive(true);
        }
    }

    private void Configure_UseEnhancementButton(Enhancement enhancement_IN)
    {
        if (enhancement_IN == null)
        {
            if (useEnhancementButton.gameObject.activeSelf != false) useEnhancementButton.gameObject.SetActive(false);
        }
        else
        {
            if (useEnhancementButton.gameObject.activeSelf != true) useEnhancementButton.gameObject.SetActive(true);
        }
    }

    public void ArrangeTabButtons(int tabAmountToDisplay_IN)
    {
        for (int i = 0; i < tabSelectorButtons.Length; i++)
        {

            if (tabAmountToDisplay_IN == tabSelectorButtons.Length)
            {
                if (tabSelectorButtons[i].gameObject.activeSelf == false)
                {
                    tabSelectorButtons[i].gameObject.SetActive(true);
                }

                IVariableTab variableTab = tabSelectorButtons[i] as IVariableTab;
                if (variableTab.originalSizeDelta_X != variableTab.RT.sizeDelta.x)
                {
                    variableTab.ResetButtonSizePosition();
                }
            }

            else
            {
                if (i < tabAmountToDisplay_IN)
                {
                    if (tabSelectorButtons[i].gameObject.activeSelf == false)
                    {
                        tabSelectorButtons[i].gameObject.SetActive(true);
                    }

                    IVariableTab variableTab = tabSelectorButtons[i] as IVariableTab;
                    if (variableTab.originalSizeDelta_X == variableTab.RT.sizeDelta.x)
                    {
                        variableTab.ArrangeButtonSizePosition(i, tabSelectorButtons.Length / tabAmountToDisplay_IN);
                    }
                }
                else
                {
                    if (tabSelectorButtons[i].gameObject.activeSelf == true)
                    {
                        tabSelectorButtons[i].gameObject.SetActive(false);
                    }
                    else
                    {
                        return;
                    }
                }
            }
        }
    }



    //public void RefreshPanel()
    //{
    //    for (int i = 0; i < TabAmountToDisplay; i++)
    //    {
    //        Debug.LogWarning("refreshing !! how ???");
    //        tabPanels[i].LoadInfo();
    //    }
    //}

    //public void OnDisable()
    //{

    //    //if(blueprintBigImage.sprite != null)
    //    //{
    //    //    blueprintBigImage.sprite = null;
    //    //    SpriteLoader.UnloadAdressable(selectedRecipe.GetAdressableImage());
    //    //}
    //}

    //public override void UnloadAndDeallocate()
    //{
    //    //Debug.LogWarning("gameitem info panel is deallocatin !!");
    //    //bigImageContainer.UnloadSprite();
    //    //thumbnailImageContainer.UnloadSprite();
    //}
}
