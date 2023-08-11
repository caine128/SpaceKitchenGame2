using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public abstract class ScrollablePanel : Panel_Base
{
    protected IEnumerator co_CurrentExtraLoad = null;
    public IEnumerator<Action> ExtraLoadActionsExecutionRoutine(params Action[] extraLoadActions_IN)
    {
        for (int i = 0; i < extraLoadActions_IN.Length; i++)
        {
            extraLoadActions_IN[i]?.Invoke();
            while (co_CurrentExtraLoad != null) yield return null;
        }
    }
}

public abstract class ScrollablePanel<T_BluePrint> : ScrollablePanel, IDeallocatable
    where T_BluePrint:SortableBluePrint_Base
{
    public abstract int IndiceIndex { get; }
    //private readonly int _indiceIndex = 11;
    public virtual float ContainerWidth { get; protected set; }
    public virtual float OffsetDistance { get; protected set; }

    public IEnumerator CO => _co;
    protected IEnumerator _co = null;


    [SerializeField] protected RectTransform rt_Panel;
    [SerializeField] protected UnityEngine.GameObject container_pf;
    [SerializeField] protected RectTransform scrollingContainer;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private AnimationCurve easeCurve;
    [SerializeField] protected ScrolledObjectsVisibility<T_BluePrint> scrolledObjectsVisibility;
    public virtual List<Container<T_BluePrint>> ContainersList { get { return _containersList; } }
    protected static List<Container<T_BluePrint>> _containersList = new List<Container<T_BluePrint>>();
    public List<T_BluePrint> RequestedBluePrints = new List<T_BluePrint>();
    protected virtual WaitForSeconds containerActivationDuration => TimeTickSystem.WaitForSeconds_ContainerActivation;

    protected virtual void Start()
    {
        scrolledObjectsVisibility.AssignPanelBelonged(this);
        ConfigurePanel();
        InitializeContainers();
    }

    protected virtual void ConfigurePanel()
    {
        float panelMaxHeight = rt_Panel.rect.width / 13f;
        rt_Panel.sizeDelta = new Vector2(0, panelMaxHeight);
    }

    protected virtual void InitializeContainers()
    {
        for (int i = 0; i < IndiceIndex; i++)
        {
            var newRecipeContainer = Instantiate(container_pf, scrollingContainer, false);
            var recipeContainer_Script = newRecipeContainer.GetComponent<Container<T_BluePrint>>();

            if (scrollingContainer.childCount == 1)
            {
                ContainerWidth = recipeContainer_Script.rt.rect.width;
            }

            ContainersList.Add(recipeContainer_Script);
            recipeContainer_Script.ScaleDirect(isVisible: false);
            OnPanelMoved += recipeContainer_Script.SetState_ImageRaycast;
            SetContainerState(newRecipeContainer, isActiveIN: false);

        }
        PlaceAllContainers(ContainersList);
    }

    protected virtual void PlaceAllContainers(List<Container<T_BluePrint>> recipeContainersListIN)
    {
        for (int i = 0; i < recipeContainersListIN.Count; i++)
        {
            PlaceContainer(ContainersList[i].rt, ContainerWidth, i);
        }
    }
    protected void PlaceContainer(RectTransform rt_In, float itemWidth, int order, bool isInversePlaced = false)
    {
         
        if (isInversePlaced)
        {
            rt_In.anchorMin = rt_In.anchorMax = new Vector2(1f, 0.5f);
            rt_In.anchoredPosition = rt_In.anchoredPosition = new Vector2((itemWidth / 2 * order) + (itemWidth / 2 * (order + 1)) + (OffsetDistance * (order + 1)), 0) * new Vector2(-1f, 1f);
        }
        else
        {
            rt_In.anchoredPosition = new Vector2((itemWidth / 2 * order) + (itemWidth / 2 * (order + 1)) + (OffsetDistance * (order + 1)), 0);
        }
    }

    protected void SetContainerState(UnityEngine.GameObject container, bool isActiveIN)
    {
        container.SetActive(isActiveIN);
    }

    protected abstract List<T_BluePrint> CreateSortList();

    protected void Arrange(List<T_BluePrint> requestedProductRecipesIN)
    {
        RequestedBluePrints = requestedProductRecipesIN;
        var requestedProductRecipes_Count = RequestedBluePrints.Count;
        for (int i = 0; i < IndiceIndex; i++)
        {
            if (i <= requestedProductRecipes_Count - 1)
            {
                if (ContainersList[i].gameObject.activeSelf == false)
                {
                    ContainersList[i].gameObject.SetActive(true);
                    ContainersList[i].LoadContainer(RequestedBluePrints[i]);
                }
                else
                {
                    ContainersList[i].LoadContainer(RequestedBluePrints[i]);
                    ContainersList[i].ScaleDirect(isVisible: false);
                }
            }
            else
            {
                if (ContainersList[i].gameObject.activeSelf == false)
                {
                    break;
                }
                else
                {
                    ContainersList[i].UnloadContainer();
                    ContainersList[i].ScaleDirect(isVisible: false);
                    ContainersList[i].gameObject.SetActive(false);
                }
            }
        }

        if (requestedProductRecipes_Count <= 0)
        {
            return;
        }

        NormalizeContainers();
        scrolledObjectsVisibility.Configure(RequestedBluePrints);
    }

    protected void ArrangeAndSort(List<T_BluePrint> requestedProductRecipesIN)
    {
        Arrange(requestedProductRecipesIN);
        SortRecipes(RequestedBluePrints.Count);
    }

    protected void SortRecipes(int amountToSortIN)
    {
        if (_co != null)
        {
            StopCoroutine(_co);
            _co = null;
        }
        if (co_CurrentExtraLoad != null)
        {
            StopCoroutine(co_CurrentExtraLoad);
            co_CurrentExtraLoad = null;
        }
        _co = SortRecipesRoutine(amountToSortIN);
        StartCoroutine(_co);
    }

    protected virtual IEnumerator SortRecipesRoutine(int amountToSortIN)
    {
        ResizeScrolledRecipeContainer(CalculateLastRecipeAnchorPoint(amountToSortIN));

        if (amountToSortIN > IndiceIndex)
        {
            amountToSortIN = IndiceIndex;
        }

        for (int i = 0; i < amountToSortIN; i++)
        {
            ContainersList[i].ScaleWithRoutine(isVisible: true);
            yield return containerActivationDuration; //TimeTickSystem.WaitForSeconds_ContainerActivation;
        }

        _co = null;
    }


    protected virtual float CalculateLastRecipeAnchorPoint(int amountToSort_IN)
    {
        return (ContainerWidth * amountToSort_IN) + (OffsetDistance * amountToSort_IN);       
    }


    protected virtual void ResizeScrolledRecipeContainer(float lastRecipeAnchorIN)    /// Resize the Scrolled Container According to The Scrollable Item Amount
    {
        float difference = rt_Panel.rect.width - lastRecipeAnchorIN;


        if (difference < 0)
        {
            scrollingContainer.offsetMax = new Vector2(difference * -1, 0);
        }
        else if (difference >= 0 && scrollingContainer.rect.width != rt_Panel.rect.width)
        {
            scrollingContainer.offsetMax = Vector2.zero;
        }
    }

    protected virtual void NormalizeContainers()
    {
        if (scrollRect.horizontalNormalizedPosition != 0 || scrollRect.verticalNormalizedPosition != 1)                    /// Put scrollbars normalized position in case its left on something between
        {
            scrollRect.horizontalNormalizedPosition = 0;
            scrollRect.verticalNormalizedPosition = 1;
            PlaceAllContainers(ContainersList);
        }
    }
    public void UnloadAndDeallocate()
    {
        for (int i = 0; i < ContainersList.Count; i++)
        {
            if (ContainersList[i].gameObject.activeInHierarchy == true) ContainersList[i].UnloadContainer();
        }
    }


    /// <summary>
    /// This part is to move the scrollbar TO THE selected item
    /// </summary>
    protected IEnumerator ScrollToPosition(float targetScrollPos, int? normalizedContainerIndex_IN)
    {
        if (_co != null)
        {
            yield return null;
        }
    
        var currentScrollPos = scrollRect.horizontalNormalizedPosition;
        float elapsedTime = 0f;
        float lerpDuration = .15f;  //Later to arrange it from the timer class 

        scrolledObjectsVisibility.ForceScrolled = true;

        while (elapsedTime < lerpDuration)
        {
            float easeFactor = elapsedTime / lerpDuration;
            easeFactor = easeCurve.Evaluate(easeFactor);
            scrollRect.horizontalNormalizedPosition = Mathf.Lerp(currentScrollPos, targetScrollPos, easeFactor);
            elapsedTime += Time.deltaTime;

            yield return null;
        }
        scrollRect.horizontalNormalizedPosition = targetScrollPos;

        scrolledObjectsVisibility.ForceScrolled = false;
        //if (scrolledObjectsVisibility is AscensionLadderPanel_Scroller ascensionLadderPanel_Scroller) ascensionLadderPanel_Scroller.ForceUpdateScrolledContainer();

        if (normalizedContainerIndex_IN != null)
        {
            MarkContainer((int)normalizedContainerIndex_IN);
        }

        co_CurrentExtraLoad = null;       
    }

    protected void JumpScrollToPosition(float differenceToScroll)
    {
        scrollRect.horizontalNormalizedPosition += differenceToScroll;
    }


    protected void MarkContainer(int normalizedContainerIndex_IN)
    {
        ContainersList[normalizedContainerIndex_IN].Tintsize();
    }
}





public abstract class ScrollablePanel<T_BluePrint, T_MainSelector> : ScrollablePanel<T_BluePrint>
    where T_BluePrint:SortableBluePrint_Base
    where T_MainSelector : System.Enum
{
    public static T_MainSelector activeSelection_MainType;
    public bool CheckActiveMainType(T_MainSelector mainType_IN)
    {
        if (activeSelection_MainType.Equals(mainType_IN))
        {
            return true;
        }
        return false;
    }

    //protected override void InitializeContainers()
    //{
    //    for (int i = 0; i < IndiceIndex; i++)
    //    {
    //        var newRecipeContainer = Instantiate(container_pf, scrollingContainer, false);
    //        var recipeContainer_Script = newRecipeContainer.GetComponent<Container<T_BluePrint>>();

    //        if (scrollingContainer.childCount == 1)
    //        {
    //            ContainerWidth = recipeContainer_Script.rt.rect.width;
    //            Debug.Log("WORKING SHOULDNT BE TWICE");
    //        }

    //        ContainersList.Add(recipeContainer_Script);
    //        recipeContainer_Script.ScaleDirect(isVisible: false);
    //        OnPanelMoved += recipeContainer_Script.SetState_ImageRaycast;
    //        SetContainerState(newRecipeContainer, isActiveIN: false);

    //    }
    //    PlaceAllContainers(ContainersList);
    //}

    public abstract void SortByEquipmentType(T_MainSelector mainType_IN, SelectorButton<T_MainSelector> buttonIN);


}





public abstract class ScrollablePanel<T_BluePrint, T_MainSelector, T_SubSelector> : ScrollablePanel<T_BluePrint, T_MainSelector>, IScrollablePanel<T_MainSelector, T_SubSelector> 
    where T_BluePrint : SortableBluePrint
    where T_MainSelector : System.Enum
    where T_SubSelector : System.Enum
{
    //public readonly int IndiceIndex = 11;
    //public  float CardWidth { get; protected set; }
    //public  float OffsetDistance { get; protected set; }
    protected static float Selector_Button_Width = default(float);
    [SerializeField] protected Color OriginalButtonColor;                                                               // Later To Change With Icon
    [SerializeField] protected Color SelectedButtonColor;                                                               // Later To Change With Icon


    //protected IEnumerator co = null;

    //[SerializeField] protected RectTransform rt_Panel;
    //[SerializeField] protected GameObject container_pf;
    [SerializeField] protected UnityEngine.GameObject main_SelectorButton_pf;
    [SerializeField] protected UnityEngine.GameObject sub_SelectorButton_pf;
    //[SerializeField] protected RectTransform scrollingContainer;
    //[SerializeField] protected ScrollRect scrollRect;
    //[SerializeField] protected ScrolledObjectsVisibility<T_BluePrint, T_MainSelector, T_SubSelector> scrolledObjectsVisibility;
    
    protected SelectorButton<T_MainSelector>[] mainType_Selector_Buttons;                                 
    protected SelectorButton<T_SubSelector>[] subType_SelectorButtons;                                 
    
    //public List<Container<T_BluePrint>> ContainersList { get { return _containersList; } } 
    //protected static List<Container<T_BluePrint>> _containersList = new List<Container<T_BluePrint>>();
   
    //public  List<T_BluePrint> RequestedBluePrints = new List<T_BluePrint>();
    public static Dictionary<T_MainSelector, T_SubSelector> lastOrDefaultSelection;
    
    //public static T_MainSelector activeSelection_MainType;
    public static T_SubSelector activeSelection_SubtType;

    //public static (T_MainSelector active_MainType, T_SubSelector active_SubtType) activeSelection;
    //[SerializeField] protected AnimationCurve easeCurve;




    protected override void Start()    // later to be taken to panel config
    {
        Initialize_MainSelectionButtons();  // the order of initialization might change important is to have first and one OFFSetDistance !!!! 
        Initialize_SubSelectionButtons();

        base.Start();     
        
        LoadSortSelectionDict();
    }



    //protected override void ConfigurePanel(IDictionary lastOrDefaultSortSelectionIN = null)
    //{
    //    //float panelMaxHeight = rt_Panel.rect.width / 13f;
    //    //rt_Panel.sizeDelta = new Vector2(0, panelMaxHeight);
    //    base.ConfigurePanel();

    //    lastOrDefaultSelection = lastOrDefaultSortSelectionIN as Dictionary<T_MainSelector, T_SubSelector> ?? PopulateSortSelectionDictionary();
    //}

    protected void LoadSortSelectionDict(Dictionary<T_MainSelector, T_SubSelector> lastOrDefaultSortSelectionIN = null)
    {
        lastOrDefaultSelection = lastOrDefaultSortSelectionIN ?? PopulateSortSelectionDictionary();
    }

    protected Dictionary<T_MainSelector, T_SubSelector> PopulateSortSelectionDictionary()
    {
        Dictionary<T_MainSelector, T_SubSelector> newDict = new Dictionary<T_MainSelector, T_SubSelector>();

        T_MainSelector[] enumValues = (T_MainSelector[])Enum.GetValues(typeof(T_MainSelector));

        for (int i = 0; i < enumValues.Length; i++)
        {
            newDict[enumValues[i]] = (T_SubSelector)enumValues[i].GetAllSubTypes()[0];
        }
        return newDict;

    }

    protected void UpdateSortDictionary(T_MainSelector selectedMainType, T_SubSelector selectedSubType)
    {
        lastOrDefaultSelection[selectedMainType] = selectedSubType;
    }

    protected void Initialize_MainSelectionButtons()
    {
        
        T_MainSelector[] enumValues = (T_MainSelector[])Enum.GetValues(typeof(T_MainSelector));
        mainType_Selector_Buttons = new SelectorButton<T_MainSelector>[enumValues.Length];

        int buttonOrder = 0;
        for (int i = enumValues.Length - 1; i > -1; i--)
        {
            var main_SelectorButton = Instantiate(main_SelectorButton_pf, rt_Panel, worldPositionStays: false);
            //main_SelectorButton.AddComponent<EquipmentSelector>();
            var main_SelectorButton_Script = main_SelectorButton.GetComponent<SelectorButton<T_MainSelector>> ();

            //main_SelectorButton_Script.SetSelectorType(true);
            main_SelectorButton_Script.AssignPanel(this);
            main_SelectorButton_Script.AssignCriteria( enumValues[i]);

            mainType_Selector_Buttons[i] = main_SelectorButton_Script;

            if (Selector_Button_Width == default(float))
            {
                Selector_Button_Width = main_SelectorButton_Script.rt.sizeDelta.x;
                OffsetDistance = Selector_Button_Width / 6f;
            }

            PlaceContainer(main_SelectorButton_Script.rt, Selector_Button_Width, buttonOrder, isInversePlaced: true);
            buttonOrder++;
        }
    }

    protected virtual void Initialize_SubSelectionButtons()
    {
        List<int> subTypecounts = new List<int>();
        foreach (T_MainSelector equipmentType in Enum.GetValues(typeof(T_MainSelector)))
        {
            subTypecounts.Add(equipmentType.GetAllSubTypes().Count);
        }
        var maxNumberOfButtons = subTypecounts.GetHighestInt();


        subType_SelectorButtons = new SelectorButton<T_SubSelector>[maxNumberOfButtons];

        for (int i = 0; i < maxNumberOfButtons; i++)
        {

            var subSelectorButton = Instantiate(sub_SelectorButton_pf, rt_Panel, worldPositionStays: false);
            //subSelectorButton.AddComponent<TypeSelector>();
            var subSelectorButton_Script = subSelectorButton.GetComponent<SelectorButton<T_SubSelector>>();

            subSelectorButton_Script.AssignPanel(this);
            subType_SelectorButtons[i] = subSelectorButton_Script;

            PlaceContainer(subSelectorButton_Script.rt, Selector_Button_Width, i);
            SetContainerState(subSelectorButton, isActiveIN: false);
        }
    }

    //protected override void InitializeContainers() 
    //{

    //    for (int i = 0; i < IndiceIndex; i++)
    //    {
    //        var newRecipeContainer = Instantiate(container_pf, scrollingContainer, false);
    //        var recipeContainer_Script = newRecipeContainer.GetComponent<Container<T_BluePrint>>();

    //        if (scrollingContainer.childCount == 1)
    //        {
    //            ContainerWidth = recipeContainer_Script.rt.rect.width;
    //            Debug.Log("WORKING SHOULDNT BE TWICE");
    //        }

    //        _containersList.Add(recipeContainer_Script);
    //        recipeContainer_Script.ScaleDirect(isVisible: false);
    //        OnPanelMoved += recipeContainer_Script.SetState_ImageRaycast;
    //        SetContainerState(newRecipeContainer, isActiveIN: false);

    //    }

    //    PlaceAllContainers(_containersList);
    //}
    //protected void PlaceRecipeContainers(List<Container<T_BluePrint>> recipeContainersListIN) 
    //{
    //    for (int i = 0; i < recipeContainersListIN.Count; i++)
    //    {
    //        PlaceContainer(_containersList[i].rt, CardWidth, i);
    //    }
    //}


    //protected void PlaceContainers(RectTransform rt_In, float itemWidth, int order, bool isInversePlaced = false)
    //{
    //    if (isInversePlaced)
    //    {
    //        rt_In.anchorMin = rt_In.anchorMax = new Vector2(1f, 0.5f);
    //        rt_In.anchoredPosition = rt_In.anchoredPosition = new Vector2((itemWidth / 2 * order) + (itemWidth / 2 * (order + 1)) + (OffsetDistance * (order + 1)), 0) * new Vector2(-1f, 1f);
    //    }
    //    else
    //    {
    //        rt_In.anchoredPosition = new Vector2((itemWidth / 2 * order) + (itemWidth / 2 * (order + 1)) + (OffsetDistance * (order + 1)), 0);
    //    }
    //}


    //protected void SetContainerState(GameObject container, bool isActiveIN) 
    //{
    //    container.SetActive(isActiveIN);
    //}



    //public sealed override bool CheckActiveMainType(T_MainSelector mainType_IN)
    //{
    //    //if (activeSelection.active_MainType.Equals(mainType_IN))
    //    if (activeSelection_MainType.Equals(mainType_IN))
    //    {
    //        return true;
    //    }
    //    return false;
    //}

    public bool CheckActiveSubType(T_SubSelector subType_IN)
    {
        //if (activeSelection.active_SubtType.Equals(subType_IN))
        if (activeSelection_SubtType.Equals(subType_IN))
        {
            return true;
        }
        return false;
    }

    


    public sealed override void SortByEquipmentType(T_MainSelector mainType_IN, SelectorButton<T_MainSelector> buttonIN) // THE NAME SHOULD BE SORTBYMAIN TYPPE !!!
    {
        activeSelection_MainType = mainType_IN; //activeSelection.active_MainType = mainType_IN;
        SetButtonColors(buttonIN);

        var allSubTypes = mainType_IN.GetAllSubTypes();
        int allsubTypes_Count = allSubTypes.Count;

        for (int i = 0; i < subType_SelectorButtons.Length; i++)
        {
            if (i <= allsubTypes_Count - 1)
            {
                if (subType_SelectorButtons[i].gameObject.activeSelf == false)
                {
                    subType_SelectorButtons[i].gameObject.SetActive(true);
                    subType_SelectorButtons[i].AssignCriteria((T_SubSelector)allSubTypes[i]);
                }
                else
                {
                    subType_SelectorButtons[i].AssignCriteria((T_SubSelector)allSubTypes[i]);
                }
            }
            else
            {
                if (subType_SelectorButtons[i].gameObject.activeSelf == false)
                {
                    break;
                }
                else
                {
                    subType_SelectorButtons[i].gameObject.SetActive(false);
                }
            }

            if (subType_SelectorButtons[i].type.Equals(lastOrDefaultSelection[mainType_IN]))
            {
                ExecuteEvents.Execute(subType_SelectorButtons[i].gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerDownHandler);
            }
        }
    }

    public void SortBySubType(T_SubSelector subType_IN, SelectorButton<T_SubSelector> buttonIN)
    {
        activeSelection_SubtType = subType_IN;// activeSelection.active_SubtType = subType_IN;
        UpdateSortDictionary(activeSelection_MainType, subType_IN);// UpdateSortDictionary(activeSelection.active_MainType, subType_IN);

        SetButtonColors(buttonIN);

        var listToSort = CreateSortList();
        ArrangeAndSort (listToSort); 

    }

    //protected abstract List<T_BluePrint> CreateSortList(T_MainSelector mainSelector, T_SubSelector subSelector);
    //protected abstract List<T_BluePrint> CreateSortList();

    //protected void ArrangeAndSort(List<T_BluePrint> requestedProductRecipesIN)
    //{
    //    RequestedBluePrints = requestedProductRecipesIN;
    //    var requestedProductRecipes_Count = RequestedBluePrints.Count;
    //    for (int i = 0; i < IndiceIndex; i++)
    //    {
    //        if (i <= requestedProductRecipes_Count - 1)
    //        {
    //            if (_containersList[i].gameObject.activeSelf == false)
    //            {
    //                _containersList[i].gameObject.SetActive(true);
    //                _containersList[i].LoadContainer(RequestedBluePrints[i]); 
    //            }
    //            else
    //            {
    //                _containersList[i].LoadContainer(RequestedBluePrints[i]);
    //                _containersList[i].ScaleDirect(isVisible: false);
    //            }
    //        }
    //        else
    //        {
    //            if (_containersList[i].gameObject.activeSelf == false)
    //            {
    //                break;
    //            }
    //            else
    //            {
    //                _containersList[i].UnloadContainer();
    //                _containersList[i].ScaleDirect(isVisible: false);
    //                _containersList[i].gameObject.SetActive(false);
    //            }
    //        }
    //    }

    //    if (requestedProductRecipes_Count <= 0)
    //    {
    //        return;
    //    }

    //    NormalizeContainers();
    //    scrolledObjectsVisibility.Configure(RequestedBluePrints);
    //    SortRecipes(requestedProductRecipes_Count);
    //}

    protected void DefineSortType(Sort.Type sortTypeIn, List<T_BluePrint> listToSort) 
    {
        switch (sortTypeIn)
        {
            case Sort.Type.DateMostRecent:
                listToSort.SortByDateDescending();    // LATER TO IMPLEMENT !!              
                //SortByValue(listToSort);
                break;
            case Sort.Type.Value:
                listToSort.SortByValue();
                //SortByValue(listToSort);
                break;
            case Sort.Type.Quantity:
                listToSort.SortByQuantitiy();
                //SortByQuantity(listToSort);
                break;
            case Sort.Type.Type:
                listToSort.SortByName();
                //SortByType(listToSort);
                break;
            case Sort.Type.Level:
                listToSort.SortByLevel();
                //SortByLevel(listToSort);
                break;
            case Sort.Type.Quality:
                listToSort.SortByQuality();
                //SortByValue(listToSort);
                break;
            case Sort.Type.None:
                break;
        }
    }

    //protected void SortRecipes(int amountToSortIN)
    //{
    //    if (co != null)
    //    {
    //        StopCoroutine(co);
    //        co = null;
    //    }
    //    if(co_CurrentExtraLoad != null)
    //    {
    //        StopCoroutine(co_CurrentExtraLoad);
    //        co_CurrentExtraLoad = null;
    //    }
    //    co = SortRecipesRoutine(amountToSortIN);
    //    StartCoroutine(co);
    //}

    //protected IEnumerator SortRecipesRoutine(int amountToSortIN)
    //{
    //    float lastRecipeAnchor = (CardWidth * amountToSortIN) + (OffsetDistance * amountToSortIN); // this can be done by directly the CARD INDEX LATER!!
    //    ResizeScrolledRecipeContainer(lastRecipeAnchor);


    //    if (amountToSortIN > IndiceIndex)
    //    {
    //        amountToSortIN = IndiceIndex;
    //    }

    //    for (int i = 0; i < amountToSortIN; i++)
    //    {
    //        _containersList[i].ScaleWithRoutine(isVisible: true);
    //        yield return TimeTickSystem.WaitForSeconds_ContainerActivation;
    //    }

    //    co = null;
    //}

    protected void SetButtonColors<T>(T button)
    {  
        if (typeof(T) == typeof(SelectorButton<T_SubSelector>) || typeof(T).IsSubclassOf(typeof(SelectorButton<T_SubSelector>)))
        {
            foreach (var typeSelector in subType_SelectorButtons)
            {
                if (typeSelector == button as SelectorButton<T_SubSelector>)
                {
                    typeSelector.SetButtonColor(SelectedButtonColor);
                }
                else
                {
                    typeSelector.SetButtonColor(OriginalButtonColor);
                }
            }
        }
        else if (typeof(T) == typeof(SelectorButton<T_MainSelector>) || typeof(T).IsSubclassOf(typeof(SelectorButton<T_MainSelector>)))
        {
            foreach (var equipmentSelector in mainType_Selector_Buttons)
            {
                if (equipmentSelector == button as SelectorButton<T_MainSelector>)
                {
                    equipmentSelector.SetButtonColor(SelectedButtonColor);
                }
                else
                {
                    equipmentSelector.SetButtonColor(OriginalButtonColor);
                }
            }
        }
    }

 
    /// <summary>
    /// This part is to move the scrollbar TO THE selected item
    /// </summary>  
    public void StimulateSelectionOf(T_BluePrint displayedBluePrint_IN)   // THIS CAN GO IN THE TRIGGER FUNCTION as "MAKETABSELECTION"
    {
        (T_MainSelector mainType, T_SubSelector subType) displayedBpTypes = displayedBluePrint_IN switch
        {
            ProductRecipe productRecipe => ((T_MainSelector)(object)productRecipe.recipeSpecs.requiredEquipment, (T_SubSelector)(object)productRecipe.recipeSpecs.productType),
            ShopUpgrade shopUpgrade => ((T_MainSelector)(object)shopUpgrade.shopUpgradeType, (T_SubSelector)(object)Sort.Type.Value),
            Product product => ((T_MainSelector)(object)GameItemType.Type.Product, lastOrDefaultSelection[(T_MainSelector)(object)GameItemType.Type.Product]),
            Worker worker => ((T_MainSelector)(object)CharacterType.Type.Worker, (T_SubSelector)(object)Sort.Type.DateMostRecent),
            _ => throw new NotImplementedException(),
        };;

        UpdateSortDictionary(displayedBpTypes.mainType, displayedBpTypes.subType);
        foreach (var button in mainType_Selector_Buttons)
        {
            if (button.type.Equals(displayedBpTypes.mainType)) ExecuteEvents.Execute(button.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerDownHandler);
        }
    }

    /// <summary>
    /// This part is to move the scrollbar TO THE selected item
    /// Equality selection logic is given in the most derived 
    /// </summary>
    public abstract void ScrollToSelection(T_BluePrint displayBuluPrint_In, bool markSelection);
    
    /// <summary>
    /// The blueprint locator is on most derived class since equality comparers differ, forward pos according to the blueprint pos is always calculated below
    /// </summary>
    protected  void CalculateForwardPosAndScroll (int selectedContainerIndex, bool markSelection)
    {
        float targetForwardPos = (ContainerWidth / 2 * selectedContainerIndex) + (ContainerWidth / 2 * (selectedContainerIndex + 1)) + (OffsetDistance * (selectedContainerIndex + 1));
        int normalizedContainerIndex = Mathf.FloorToInt(selectedContainerIndex / IndiceIndex) + (selectedContainerIndex % IndiceIndex);


        if (targetForwardPos > rt_Panel.rect.width / 2)
        {
            float difference = (scrollingContainer.rect.width) - rt_Panel.rect.width;
            float screenForwardPos = targetForwardPos - rt_Panel.rect.width / 2;
            float targetHorNormPos = Mathf.Min(screenForwardPos / difference , 1);

            co_CurrentExtraLoad = ScrollToPosition(targetHorNormPos, markSelection == true ? normalizedContainerIndex : null);
            StartCoroutine(co_CurrentExtraLoad);
        }
        else
        {
            //co_CurrentExtraLoad = MarkContainer(normalizedContainerIndex);
            MarkContainer(normalizedContainerIndex);
        }

        //StartCoroutine(co_CurrentExtraLoad); 
    }

    /// <summary>
    /// This part is to move the scrollbar TO THE selected item
    /// </summary>
   /* protected IEnumerator ScrollToPosition(float targetScrollPos,int? normalizedContainerIndex_IN)
    {
        if (_co != null)
        {
            yield return null;
        }

        var currentScrollPos = scrollRect.horizontalNormalizedPosition;
        float elapsedTime = 0f;
        float lerpDuration = .15f;  //Later to arrange it from the timer class 

        while (elapsedTime < lerpDuration)
        {
            float easeFactor = elapsedTime / lerpDuration;
            easeFactor = easeCurve.Evaluate(easeFactor);
            scrollRect.horizontalNormalizedPosition = Mathf.Lerp(currentScrollPos, targetScrollPos, easeFactor);
            elapsedTime += Time.deltaTime;

            yield return null;
        }
        scrollRect.horizontalNormalizedPosition = targetScrollPos;

        if (normalizedContainerIndex_IN != null)
        {
            MarkContainer((int)normalizedContainerIndex_IN);
        }

        co_CurrentExtraLoad = null;
    }

    private void MarkContainer(int normalizedContainerIndex_IN) 
    {
        _containersList[normalizedContainerIndex_IN].Tintsize();
    }*/


    //protected  void NormalizeContainers()
    //{
    //    if (scrollRect.horizontalNormalizedPosition != 0)                    /// Put scrollbars normalized position in case its left on something between
    //    {
    //        scrollRect.horizontalNormalizedPosition = 0;
    //        PlaceAllContainers(_containersList); 
    //    }
    //}
    //protected void ResizeScrolledRecipeContainer(float lastRecipeAnchorIN)    /// Resize the Scrolled Container According to The Scrollable Item Amount
    //{
    //    float difference = rt_Panel.rect.width - lastRecipeAnchorIN; 


    //    if (difference < 0)
    //    {
    //        Debug.Log("Scrollbar Increased" + difference);
    //        scrollingContainer.offsetMax = new Vector2(difference * -1, 0);
    //    }
    //    else if (difference >= 0 && scrollingContainer.rect.width != rt_Panel.rect.width)
    //    {
    //        Debug.Log("scrollbar Decreased");
    //        scrollingContainer.offsetMax = Vector2.zero;
    //    }
    //}

    //public void UnloadAndDeallocate()
    //{
    //    for (int i = 0; i < _containersList.Count; i++)
    //    {
    //        if (_containersList[i].gameObject.activeInHierarchy == true) _containersList[i].UnloadContainer();
    //    }
    //}
}
