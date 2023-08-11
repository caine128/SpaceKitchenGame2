using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public abstract class TabbedPanel : Panel_Base, ILoadablePanel, IDeallocatable, IQuickUnloadable, IRefreshablePanel
{
    //public abstract void LoadPanel(object mainLoadInfo, params object[] extraLoadInfo);
    public abstract void RefreshPanel();
    public abstract void QuickUnload();
    public abstract void UnloadAndDeallocate();

    //public abstract void LoadPanel<T_BlueprintType>(PanelLoadData<T_BlueprintType> panelLoadData)
    //where T_BlueprintType : SortableBluePrint;

    public abstract void LoadPanel(PanelLoadData panelLoadData);
}


public abstract class TabbedPanel<T_ItemType> : TabbedPanel, IBrowsablePanel<T_ItemType>
    where T_ItemType : SortableBluePrint
{
    public BrowserButton<T_ItemType>[] BrowserButtons { get { return _browserButtons; } }
    [SerializeField] protected BrowserButton<T_ItemType>[] _browserButtons;
    public int CurrentIndice { get { return _currentIndice; } }
    protected int _currentIndice = 0;
    public abstract List<T_ItemType> ListToIterate { get; }

    private void Start()
    {
        InitialConfigBrowserButtons();
    }
    /*
    public sealed override void LoadPanel(object mainLoadInfo, params object[] extraLoadInfo)
    {
        LoadInfo(mainLoadInfo as T_ItemType);
    }*/
    //public sealed override void LoadPanel<T_BlueprintType>(PanelLoadData<T_BlueprintType> panelLoadData)
    //{
    //    LoadInfo(((T_ItemType)panelLoadData.mainLoadInfo));
    //}

    public sealed override void LoadPanel(PanelLoadData panelLoadData)
    {
        LoadInfo(((T_ItemType)panelLoadData.mainLoadInfo));
    }

    public void BrowseInfo(T_ItemType blueprint_IN)
    {
        LoadInfo(blueprint_IN);
    }

    protected abstract void LoadInfo(T_ItemType bluePrint_IN);

    public abstract void InitialConfigBrowserButtons();

}



public abstract class TabbedPanel<T_ItemType, T_TabType> : TabbedPanel<T_ItemType>
    where T_ItemType : SortableBluePrint
    where T_TabType : System.Enum
{
    [SerializeField] protected Color originalButtonBGColor;        // LATER TO CHANGE TO DIFFERNET IMAGE WHICH IS HOLD IN THE BUTTON
    [SerializeField] protected Color selectedButtonBGColor;          // LATER TO CHANGE TO DIFFERNET IMAGE WHICH IS HOLD IN THE BUTTON
    [SerializeField] protected Color originalButtonFontColor;       // LATER TO CHANGE TO DIFFERNET IMAGE WHICH IS HOLD IN THE BUTTON
    [SerializeField] protected Color selectedButtonFontcolor;        // LATER TO CHANGE TO DIFFERNET IMAGE WHICH IS HOLD IN THE BUTTON


    //[SerializeField] protected Adressable_SpriteDisplay bigImageContainer;
    //[SerializeField] protected Adressable_SpriteDisplay thumbnailImageContainer;
    [SerializeField] protected AdressableImage bigImageContainer_Adressable;
    [SerializeField] protected AdressableImage thumbnailImageContainer_Adressable;
    [SerializeField] protected TextMeshProUGUI blueprintNameText;
    [SerializeField] protected TextMeshProUGUI bluePrintTypeLevelInfoText;
    [SerializeField] protected TextMeshProUGUI blueprintTypeText;                 // LATER TO MAKE THIS DESIGN CHANGE FOR RECIPE INFO PANEL AS WELL
    [SerializeField] protected TextMeshProUGUI blueprintRarityText;               // LATER TO MAKE THIS DESIGN CHANGE FOR RECIPE INFO PANEL AS WELL

    public T_ItemType SelectedRecipe        // can this be made static ???
    {
        get => _selectedRecipe;
        protected set => _selectedRecipe = value;
    }    
    private T_ItemType _selectedRecipe = null;
    public T_TabType activeTabType = (T_TabType)(object)0;
    protected int lastSelectionIndex = 0;
    [SerializeField] protected TabPanel<T_TabType>[] tabPanels;
    [SerializeField] protected TabSelectorButton<T_TabType>[] tabSelectorButtons;

    public virtual void PlaceTabPanel(T_TabType tabType_IN, TabSelectorButton<T_TabType> button_IN)
    {
        if (!tabType_IN.Equals(activeTabType))
        {
            activeTabType = tabType_IN;
            SetButtonColors(button_IN);

            for (int i = 0; i < tabPanels.Length; i++)
            {
                if (tabPanels[i].TabType.Equals(tabType_IN))
                {
                    lastSelectionIndex = i;
                    tabPanels[i].gameObject.SetActive(true);
                }
                else if (tabPanels[i].gameObject.activeSelf == true)
                {
                    if (tabPanels[i] is TabPanel_Animated<T_TabType> animatedPanel) animatedPanel.HideContainers();
                    tabPanels[i].gameObject.SetActive(false);
                }
            }
        }

        RefreshPanel();
    }

    protected void SetButtonColors(TabSelectorButton<T_TabType> tabSelectorButton_IN)
    {
        foreach (TabSelectorButton<T_TabType> tabSelectorButton in tabSelectorButtons)
        {
            if (tabSelectorButton == tabSelectorButton_IN)
            {
                tabSelectorButton.SetbuttonColor(selectedButtonBGColor, selectedButtonFontcolor);
            }
            else
            {
                tabSelectorButton.SetbuttonColor(originalButtonBGColor, originalButtonFontColor);
            }
        }
    }
    public override void RefreshPanel()
    {
        if (SelectedRecipe != null && tabPanels[lastSelectionIndex] is TabPanel_Animated<T_TabType> animatedPanel)
        {
            animatedPanel.DisplayContainers();
        }
    }

    public override void QuickUnload()
    {
        for (int i = 0; i < tabPanels.Length; i++)
        {
            if (tabPanels[i] is TabPanel_Animated<T_TabType> animatedPanel)
            {
                animatedPanel.HideContainers();
            }
        }
    }

    public override void UnloadAndDeallocate()
    {
        bigImageContainer_Adressable.UnloadSprite();
        thumbnailImageContainer_Adressable.UnloadSprite();
        SelectedRecipe = null;

        foreach (var tabPanel in tabPanels)
        {
            tabPanel.UnloadInfo();
        }
    }
}
