using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopUpgradesInfoPanel_Manager : TabbedPanel<ShopUpgrade, Tab.ShopUpgradeInfoTabs>
{

    private static ShopUpgradesInfoPanel_Manager _instance;
    public static ShopUpgradesInfoPanel_Manager Instance { get { return _instance; } }
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
    }


    public override List<ShopUpgrade> ListToIterate
    {
        get
        {
            /*Debug.Log($"listto iterate is returned in shopupgrades as {SelectedRecipe.GetName()}");
            Debug.Log(SelectedRecipe.shopUpgradeType);

            foreach (var shopupgrade in ShopData.ShopUpgradesIteration_Dict[SelectedRecipe.shopUpgradeType])
            {
                Debug.Log(shopupgrade.GetName());
            }*/
            return ShopData.ShopUpgradesIteration_Dict[SelectedRecipe.shopUpgradeType];
        }
    }

    public sealed override void InitialConfigBrowserButtons()
    {
        foreach (var browserButton in _browserButtons)
        {
            browserButton.ButtonConfig(Instance);
        }
    }

    public override void RefreshPanel()
    {
        if (SelectedRecipe != null && tabPanels[lastSelectionIndex] is TabPanel_Animated<Tab.ShopUpgradeInfoTabs> animatedPanel)
        {
            animatedPanel.LoadInfo();
            animatedPanel.DisplayContainers();
        }
    }

    protected override void LoadInfo(ShopUpgrade bluePrint_IN)
    {

        if(SelectedRecipe is null || SelectedRecipe != bluePrint_IN)
        {
            SelectedRecipe = bluePrint_IN;
            blueprintNameText.text = SelectedRecipe.GetName();
        }

        bluePrintTypeLevelInfoText.text = $"Level {SelectedRecipe.GetLevel()}";
        blueprintTypeText.text = $"{MethodHelper.GetNameOfShopUpgradeType(SelectedRecipe.shopUpgradeType)}";
        bigImageContainer_Adressable.LoadSprite(SelectedRecipe.GetAdressableImage());
        thumbnailImageContainer_Adressable.LoadSprite(SelectedRecipe.GetAdressableImage());


        foreach (var tabPanel in tabPanels)
        {
            tabPanel.LoadInfo();
        }

        var browsablePanelInterface = ((IBrowsablePanel<ShopUpgrade>)this);
        _currentIndice = browsablePanelInterface.SetCurrentIndice(SelectedRecipe);
        browsablePanelInterface.SetVisibilityBrowserButtons();

        ExecuteEvents.Execute(tabSelectorButtons[lastSelectionIndex].gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerDownHandler);
    }
}
