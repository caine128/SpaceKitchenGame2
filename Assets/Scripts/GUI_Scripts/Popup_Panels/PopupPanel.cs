using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class PopupPanel : Panel_Base, IDeallocatable , ILoadablePanel
{
    [SerializeField] protected TextMeshProUGUI popupHeader;
    [SerializeField] protected PopupButton[] popupButtons;
    public SortableBluePrint bluePrint { get; protected set; }
    public PopupButton[] PopupButtons { get { return popupButtons; } }
    public virtual void ConfigureButtons()
    {
        foreach (var popupButton in popupButtons)
        {
            popupButton.AssignPanel(this);
        }
    }


    public virtual void LoadPanel(PanelLoadData panelLoadData)
    {
      
        Debug.Log("base panelLloader is working and panelloaddata maininfo is : " + panelLoadData.mainLoadInfo );
        switch ((panelLoadData, panelLoadData.mainLoadInfo, bluePrint is null))
        {
            ///PopupPanel_Enhancement_LoadData makes its own check since it has 2 types to check 
            ///Progress Panel Shouldn't check since same productrecipes can be in the craft queue in the same time, even though blueprint doenst 
            ///have to change the header x/x amount should be updated in each singleLoad
            case (PopupPanel_Enhancement_LoadData, _, _):
            case (ProgressPanelLoadData, _, _):
                bluePrint = panelLoadData.mainLoadInfo;
                popupHeader.text = panelLoadData.panelHeader ?? DefaultPopupHeader();
                break;

            /// If the blueprint is null load anyways 
            case (_,_, true):
                bluePrint = panelLoadData.mainLoadInfo;
                popupHeader.text = panelLoadData.panelHeader ?? DefaultPopupHeader();
                break;


            /// ProductRecipe doesn't have own EqualityCoparer. The case is written exclusively for this.
            case (_,ProductRecipe productRecipe, false):
                Debug.Log("comparing as productRecipe in LoadPopupBaseData");
                var comparer = new RecipeEqualityComparer();

                switch (comparer.Equals(bluePrint as ProductRecipe, productRecipe))
                {
                    case false:
                        bluePrint = panelLoadData.mainLoadInfo;
                        popupHeader.text = panelLoadData.panelHeader ?? DefaultPopupHeader();
                        break;
                    default:  /// this is also including the true condition where we should take no action
                        Debug.Log(productRecipe.GetType() + " is same as the existing :: " + bluePrint.GetType());
                        break;
                }
                break;

            /// SortableBlueprints which have their own EqualityComparer should not be compared because same products with different amounts 
            /// can match even though they dont have the same amounts (deleted item which is still referenced as blueprint (before unload) can match 
            /// with the new one.
            case (_,_, false):    

                bluePrint = panelLoadData.mainLoadInfo;
                popupHeader.text = panelLoadData.panelHeader ?? DefaultPopupHeader();
                break;

                /*switch (bluePrint.Equals(panelLoadData.mainLoadInfo))
                {
                    case false:
                        bluePrint = panelLoadData.mainLoadInfo;
                        popupHeader.text = panelLoadData.panelHeader ?? DefaultPopupHeader();
                        break;
                    default:  /// this is also including the true condition where we should take no action
                        Debug.Log(panelLoadData.mainLoadInfo.GetType() + " is same as the existing :: " + bluePrint.GetType());
                        break;
                }
                break;*/
        }
    }




    protected abstract string DefaultPopupHeader();
    public virtual void UnloadAndDeallocate()
    {
        foreach (var button in popupButtons)
        {
            button.UnloadButton();
        }
    }


}



public abstract class PopupPanelSingleton<T_PopupPanel> : PopupPanel
    where T_PopupPanel : PopupPanel
{
    private static T_PopupPanel _instance;
    public static T_PopupPanel Instance { get { return _instance; } }
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
                    _instance = this as T_PopupPanel;
                }
            }
        }
    }
}




public abstract class PopupPanel_Single_SNG<T_PopupPanel> : PopupPanelSingleton<T_PopupPanel>, IAnimatedPanelController_ManualHide//, ILoadablePanel
    where T_PopupPanel : PopupPanel
{
    [SerializeField] protected ContentDisplayFrame contentDisplay;

    private IEnumerator[] co = null; // this coroutine is not necessay anymore !!!
    public IEnumerator[] CO
    {
        get { return co; }
    }

    [SerializeField] private GUI_LerpMethods panelToAwait;
    public GUI_LerpMethods PanelToAwait
    {
        get { return panelToAwait; }
    }

    protected virtual void Start()
    {
        co = new IEnumerator[1];

    }

    public override void LoadPanel(PanelLoadData panelLoadData)
    {
        base.LoadPanel(panelLoadData);

        contentDisplay.Load(new ContentDisplayInfo_ConentDisplayFrame(bluePrint));        //(bluePrint);
        contentDisplay.ScaleDirect(isVisible: false, finalValueOperations:null);
    }

    public virtual void DisplayContainers()
    {
        contentDisplay.AnimateWithRoutine(customInitialValue:null,
                                          secondaryInterpolation:null,
                                          isVisible: true,
                                          lerpSpeedModifier: 1,
                                          followingAction_IN:null);
    }

    public void HideContainers()
    {
        contentDisplay.ScaleDirect(isVisible: false, finalValueOperations: null);
    }


    public override void UnloadAndDeallocate()
    {
        if (co[0] != null)
        {
            StopCoroutine(co[0]);
            co[0] = null;
        }

        contentDisplay.Unload();
        base.UnloadAndDeallocate();
    }



}






public abstract class PopupPanel_Multi_SNG< T_ContentDisplayType> : PopupPanel, IAnimatedPanelController_ManualHide 
    where T_ContentDisplayType : ContentDisplayFrame
{
    [SerializeField] private RectTransform panel_BG;

    public RectTransform Panel_BG { get => panel_BG; } // This is made until the Martix method is setLater to make emableloadand display to work with matrix 

    [SerializeField] private UnityEngine.GameObject contentDisplay_pf;
    protected List<T_ContentDisplayType> contentDisplays = new List<T_ContentDisplayType>(6);
    protected Vector2 contentDisplaySpawnPosition;
    private int amountOfNecessaryContainers;

    protected IEnumerator[] co = null;
    public IEnumerator[] CO
    {
        get { return co; }
    }

    [SerializeField] private GUI_LerpMethods panelToAwait;
    public GUI_LerpMethods PanelToAwait
    {
        get { return panelToAwait; }
    }

    protected virtual void Start()
    {
        //co = new IEnumerator[1];

        var rt_header_Height = popupHeader.GetComponent<RectTransform>().rect.height;
        contentDisplaySpawnPosition = new(0f, ((panel_BG.rect.size.y/2) - rt_header_Height) / 2);

    }

    protected virtual void EnableAndLoadMainContentDisplays<T_BlueprintType> (List<(T_BlueprintType bluePrintToLoad, int amountToLoad)> lisToLoad_IN)
        where T_BlueprintType : SortableBluePrint
    {
        amountOfNecessaryContainers = lisToLoad_IN.Count;
        if (amountOfNecessaryContainers > contentDisplays.Count)
        {
            CreateContentDisplays(amountOfNecessaryContainers - contentDisplays.Count);
        }

        contentDisplays.PlaceContainers(amountOfNecessaryContainers, contentDisplays[0].RT.sizeDelta.x, isHorizontalPlacement: true);
        contentDisplays.LoadContainers(bluePrints_IN :lisToLoad_IN, hideAtInit: true);
        //contentDisplays.SortContainers(amountOfNecessaryContainers, enumeratorIndex: 0, this);
    }

    public void DisplayContainers()
    {
        //throw new System.NotImplementedException();
        //contentDisplay.ScaleWithRoutine(isVisible: true);
        contentDisplays.SortContainers(customInitialValues:null,
                                       secondaryInterpolations: null,
                                       amountToSort_IN: amountOfNecessaryContainers, 
                                       enumeratorIndex: 1,
                                       parentPanel_IN: this,
                                       lerpSpeedModifiers: null);
    }

    public void HideContainers()
    {
        foreach (var contentDisplay in contentDisplays)
        {
            contentDisplay.ScaleDirect(isVisible: false, finalValueOperations: null);
        }    
    }

    private void CreateContentDisplays(int requiredAmount)
    {
        for (int i = 0; i < requiredAmount; i++)
        {
            var newContentDisplay_go = Instantiate(contentDisplay_pf, panel_BG, false);
            var newContentDisplay = newContentDisplay_go.GetComponent<T_ContentDisplayType>();
            newContentDisplay.RT.anchoredPosition = contentDisplaySpawnPosition;
            newContentDisplay.ScaleDirect(isVisible: false, finalValueOperations:null);
            newContentDisplay_go.SetActive(false);
            contentDisplays.Add(newContentDisplay);
        }
    }


    public sealed override void UnloadAndDeallocate()
    {
        base.UnloadAndDeallocate();
        GUI_CentralPlacement.DeactivateUnusedContainers(0, contentDisplays);
    }
}
