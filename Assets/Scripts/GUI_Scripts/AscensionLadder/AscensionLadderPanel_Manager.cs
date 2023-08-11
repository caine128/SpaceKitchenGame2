using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class AscensionLadderPanel_Manager : ScrollablePanel<AscensionRewardState, ProductType.Type> ,IQuickUnloadable
{
    [SerializeField] private UnityEngine.GameObject container_Pf_Small;
    [SerializeField] private UnityEngine.GameObject container_Pf_Star;
    [SerializeField] private TextMeshProUGUI ascensionLadderPanelHeader;
    public List<LadderStar> LadderStarList => _ladderStarList;
    private static List<LadderStar> _ladderStarList;
    public float ContainerWidth_Small { get; protected set; }
    
    public float DistanceBetweenContainers => _distanceBetweenContainers;
    private float _distanceBetweenContainers;

    public override int IndiceIndex => _indiceIndex;
    private readonly int _indiceIndex = 14;

    [SerializeField] private RectTransform progressBar_BG;
    [SerializeField] private GUI_LerpMethods_ProgressSimple progressBar_FG;
    //[SerializeField] private GUI_LerpMethods_Scale ascensionMarker;
    [SerializeField] private AscensionMarker ascensionMarker;

    protected override WaitForSeconds containerActivationDuration => TimeTickSystem.WaitForSeconds_AscensionContainerActivation;
    private Dictionary<ProductType.Type, int> ascensionMarkerStates = new Dictionary<ProductType.Type, int>();

    public (int currentMarkerState, int maxMarkerState) MarkerStates { get; private set; } = (0, 0);

    protected sealed override void Start()
    {
        base.Start();
        activeSelection_MainType = ProductType.Type.None;

        
        var selectorButton = FindObjectOfType<AscensionLadderSelector_Button>();
        selectorButton.AssignPanel(this);
    }

    protected override void ConfigurePanel()
    {
        float panelMaxWidth = this.GetComponent<RectTransform>().rect.width;
        float panelMaxHeight = panelMaxWidth / 3f;
        rt_Panel.sizeDelta = new Vector2(panelMaxWidth, panelMaxHeight);
    }


    protected sealed override void InitializeContainers()
    {
        for (int i = 0; i < IndiceIndex; i++)
        {

            bool isBigContainer = i > 3 && i % 3 == 1;

            UnityEngine.GameObject newContainer = isBigContainer
                                                        ? Instantiate(container_pf, scrollingContainer, false)
                                                        : Instantiate(container_Pf_Small, scrollingContainer, false);
            LadderContainer recipeContainer_Script = isBigContainer
                                                        ? newContainer.GetComponent<LadderContainer_Big>()
                                                        : newContainer.GetComponent<LadderContainer_Small>();

            _containersList.Add(recipeContainer_Script);
            recipeContainer_Script.ScaleDirect(isVisible: false);
            recipeContainer_Script.initialOrder = i; // this is necessary to reorder the elements 
            OnPanelMoved += recipeContainer_Script.SetState_ImageRaycast;
            SetContainerState(container: newContainer, isActiveIN: false);

        }

        ContainerWidth_Small = _containersList[0].rt.rect.width;
        ContainerWidth = _containersList[4].rt.rect.width;
        OffsetDistance = ContainerWidth_Small /6;
        _distanceBetweenContainers = OffsetDistance + ContainerWidth / 2 + ContainerWidth_Small / 2;

        //Instantiate Star Containers in Necessary Amount and Populate _ladderStarList

        var bigContainerAmount = _containersList.Where(ct => ct is LadderContainer_Big).Count();
        var smallContainerAmount = _containersList.Count - bigContainerAmount;
        var requiredLadderStarsAmount = bigContainerAmount + smallContainerAmount / 2;

        _ladderStarList = new List<LadderStar>(requiredLadderStarsAmount);
        for (int i = 0; i < requiredLadderStarsAmount; i++)
        {
            var ladderStar = Instantiate(container_Pf_Star, scrollingContainer, false);
            var ladderStar_Script = ladderStar.GetComponent<LadderStar>();
            _ladderStarList.Add(ladderStar_Script);
        }


        PlaceAllContainers(_containersList);
    }

    protected sealed override void PlaceAllContainers(List<Container<AscensionRewardState>> recipeContainersListIN)
    {
        var basePoint_Y = progressBar_BG.anchoredPosition.y + progressBar_BG.rect.height;
        var starBottomPoint_Y = scrollingContainer.rect.height / 2 * -1;
        int amountOfStarsPlaced = 0;

        for (int i = 0; i < recipeContainersListIN.Count; i++)
        {
            var rt_In = recipeContainersListIN[i].rt;
    
            /*rt_In.anchoredPosition = i > 0
                                       ? recipeContainersListIN[i] is LadderContainer_Small
                                       && recipeContainersListIN[i - 1] is LadderContainer_Small previousSmallContainer
                                       && previousSmallContainer.rt.anchoredPosition.y == basePoint_Y
                                          ? new Vector2(previousSmallContainer.rt.anchoredPosition.x, basePoint_Y + previousSmallContainer.rt.rect.height + OffsetDistance)
                                          : new Vector2(recipeContainersListIN[i - 1].rt.anchoredPosition.x + recipeContainersListIN[i - 1].rt.rect.width + OffsetDistance, basePoint_Y)
                                       : new Vector2(progressBar_BG.anchoredPosition.x, basePoint_Y);*/

            rt_In.anchoredPosition = i > 0
                                       ? recipeContainersListIN[i] is LadderContainer_Small
                                       && recipeContainersListIN[i - 1] is LadderContainer_Small previousSmallContainer
                                       && previousSmallContainer.rt.anchoredPosition.y == basePoint_Y
                                          ? new Vector2(previousSmallContainer.rt.anchoredPosition.x, basePoint_Y + previousSmallContainer.rt.rect.height + OffsetDistance)
                                          : new Vector2(recipeContainersListIN[i - 1].rt.anchoredPosition.x + _distanceBetweenContainers, basePoint_Y)
                                       : new Vector2(progressBar_BG.anchoredPosition.x + _distanceBetweenContainers, basePoint_Y);



            //Place the Stars In The Meantime                        
            if (recipeContainersListIN[i].rt.anchoredPosition.y == basePoint_Y)
            {
                // Getting anchored pos x + rect width half to find the center ofthe upper container                           
                _ladderStarList[amountOfStarsPlaced].RT.anchoredPosition = new Vector2(rt_In.anchoredPosition.x , starBottomPoint_Y);    // FindStarContainerPos(recipeContainersListIN[i]); // new Vector2(rt_In.anchoredPosition.x + rt_In.rect.width/2 , starBottomPoint_Y);               
                amountOfStarsPlaced++;
                
            }
        }
    }

    //public Vector2 FindStarContainerPos(Container<AscensionRewardState> acensionRewardContainer)
    //{
    //    return new Vector2(acensionRewardContainer.rt.anchoredPosition.x + acensionRewardContainer.rt.rect.width / 2, StarBottomPoint_Y);
    //}

    public override void SortByEquipmentType(ProductType.Type mainType_IN, SelectorButton<ProductType.Type> buttonIN)
    {
        activeSelection_MainType = mainType_IN;

        var listToSort = CreateSortList();
        NormalizeContainers();
        ArrangeAndSort(listToSort);
    }

    protected override List<AscensionRewardState> CreateSortList()
    {
        var ascensionTreeRewardState = AscensionTreeManager.Instance.QueryAscensionRewardState(activeSelection_MainType);
        MarkerStates = (ascensionTreeRewardState.currentAscensionAmount,ascensionTreeRewardState.maxAscensionsAmount); // GetCurrentAndMaxAscesionAmounts();
        // Get the ascension title in the Meantime
        ascensionLadderPanelHeader.text = ascensionTreeRewardState.rewardsAndStates                                                       
                                                        .Where(rs => !string.IsNullOrEmpty(rs.reward.ascensionTitle) && !string.IsNullOrWhiteSpace(rs.reward.ascensionTitle) && rs.isUnlocked && rs.IsClaimed)
                                                        .Select(rs => rs.reward.ascensionTitle)
                                                        .DefaultIfEmpty($"{activeSelection_MainType} beginner")
                                                        .Last();
   
        return ascensionTreeRewardState.rewardsAndStates.ToList(); 
    }

    private void LoadStarContainers()
    {
        var amountOfStarsPlaced = 0;
        foreach (var reward in _containersList.Where(rc => rc.bluePrint.reward.isPremiumReward == false).Select(rc=>rc.bluePrint.reward))
        {
            _ladderStarList[amountOfStarsPlaced].LoadStarContainer(value_IN: reward.ascensionsNeeded,
                                                                   isBig: amountOfStarsPlaced % 2 == 0,
                                                                   currentAscensionState: GetExistingMarkerState());

            amountOfStarsPlaced++;
        }
    }

    protected sealed override float CalculateLastRecipeAnchorPoint(int amountToSort_IN)
    {        
       
        var ascensionContainersTotalWidth =  RequestedBluePrints.Aggregate(_distanceBetweenContainers, (totalWidth, nextBlueprint) => !string.IsNullOrEmpty(nextBlueprint.reward.ascensionTitle) ||
                                                                                                              !string.IsNullOrWhiteSpace(nextBlueprint.reward.ascensionTitle)
                                                                                                              ? totalWidth + ContainerWidth + OffsetDistance
                                                                                                              : nextBlueprint.reward.isPremiumReward
                                                                                                                   ? totalWidth + 0
                                                                                                                   : totalWidth + ContainerWidth_Small + OffsetDistance);
        // remove the half of the last container.
        var lastRequestedBlueprint = RequestedBluePrints.Last();
        ascensionContainersTotalWidth -= !string.IsNullOrEmpty(lastRequestedBlueprint.reward.ascensionTitle) || !string.IsNullOrWhiteSpace(lastRequestedBlueprint.reward.ascensionTitle)
                                            ? ContainerWidth / 2
                                            : ContainerWidth_Small / 2;

        // Resize Progress Bar 
        progressBar_BG.sizeDelta = progressBar_FG.RT.sizeDelta = new Vector2(ascensionContainersTotalWidth, progressBar_BG.sizeDelta.y);
           
        var progressBarOffsets = progressBar_BG.anchoredPosition.x * 2;
        
        // Final width Of scrolling container
        return ascensionContainersTotalWidth + progressBarOffsets;
    }

    protected override IEnumerator SortRecipesRoutine(int amountToSortIN)
    {
        LoadStarContainers();

        ResizeScrolledRecipeContainer(CalculateLastRecipeAnchorPoint(amountToSortIN));

        var existingMarkerState = GetExistingMarkerState();
        
        progressBar_FG.SetProgressBarTo(fillamount_IN: (float)existingMarkerState / (float)MarkerStates.maxMarkerState);
        ascensionMarker.DisableMarker();
        amountToSortIN = amountToSortIN > IndiceIndex ? IndiceIndex : amountToSortIN;

        for (int i = 0; i < amountToSortIN; i++)
        {
            _containersList[i].ScaleWithRoutine(isVisible: true);
            yield return containerActivationDuration; 
        }

        float totalDifference = (scrollingContainer.rect.width) - rt_Panel.rect.width;
        float panelMiddlePoint = rt_Panel.rect.width / 2;
        var currentMarkerPos = ((float)existingMarkerState / (float)MarkerStates.maxMarkerState * progressBar_FG.RT.rect.width) + progressBar_FG.RT.anchoredPosition.x;
        if(currentMarkerPos > panelMiddlePoint)
        {
            
            float screenForwardPos = currentMarkerPos - panelMiddlePoint;
            float targetHorNormPos = Mathf.Min(screenForwardPos / totalDifference, 1);
            co_CurrentExtraLoad = ScrollToPosition(targetScrollPos: targetHorNormPos,normalizedContainerIndex_IN:null);
            StartCoroutine(co_CurrentExtraLoad);
        }

        while (co_CurrentExtraLoad != null) yield return null;
        yield return TimeTickSystem.WaitForSeconds_QuarterSec;

        Debug.Log(existingMarkerState +  " existing markerstate ");
        Debug.Log(MarkerStates.currentMarkerState + " currentmarkerstate ");
        if (MarkerStates.currentMarkerState != existingMarkerState && MarkerStates.currentMarkerState <= MarkerStates.maxMarkerState)
        {
            float initialValue = (float)existingMarkerState / (float)MarkerStates.maxMarkerState;
            float finalValue = (float)MarkerStates.currentMarkerState / (float)MarkerStates.maxMarkerState;
            var lerpDuration = progressBar_FG.LerpDuration * (MarkerStates.currentMarkerState - existingMarkerState);
            float elapsedTime = 0;

          
            scrolledObjectsVisibility.ForceScrolled = true;
            float lerpedValDiffFromCenterPoint = 0;
            var amountNudgedStars = 0;

            while (elapsedTime < lerpDuration)
            {
                var lerpedValue = Mathf.Lerp(initialValue, finalValue, elapsedTime / lerpDuration);
                progressBar_FG.SetProgressBarTo(lerpedValue);

                lerpedValDiffFromCenterPoint = progressBar_FG.RT.TransformPoint(lerpedValue * progressBar_FG.RT.rect.width, progressBar_FG.RT.anchoredPosition.y, 0).x - progressBar_FG.RT.anchoredPosition.x - panelMiddlePoint;
                
                if (lerpedValDiffFromCenterPoint > 0) 
                {                
                    var targetHorNormPos = Mathf.Min(Mathf.InverseLerp(0,totalDifference,lerpedValDiffFromCenterPoint), 1);
                    JumpScrollToPosition(targetHorNormPos);                           
                }

                
                
                var nextStarContainer = _ladderStarList.First(ls => ls.Value > existingMarkerState + (amountNudgedStars * AscensionTreeManager.ascensionAmountInterval));
                var nextStarContainerPositionX = nextStarContainer.RT.anchoredPosition.x;
                if(lerpedValue* progressBar_FG.RT.rect.width + progressBar_FG.RT.anchoredPosition.x >= nextStarContainerPositionX)
                {
                    nextStarContainer.NudgeStarContainer();
                    amountNudgedStars++;
                }

                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            lerpedValDiffFromCenterPoint = progressBar_FG.RT.TransformPoint(finalValue * progressBar_FG.RT.rect.width, progressBar_FG.RT.anchoredPosition.y, 0).x - progressBar_FG.RT.anchoredPosition.x - panelMiddlePoint;
            JumpScrollToPosition(Mathf.Min(Mathf.InverseLerp(0, totalDifference, lerpedValDiffFromCenterPoint), 1));

            if(_ladderStarList.Any(ls => ls.Value == MarkerStates.currentMarkerState)) _ladderStarList.First(ls => ls.Value == MarkerStates.currentMarkerState).NudgeStarContainer();
            
            progressBar_FG.SetProgressBarTo(finalValue);
            scrolledObjectsVisibility.ForceScrolled = false;

            
            ascensionMarkerStates[activeSelection_MainType] = MarkerStates.currentMarkerState;
        }

        ascensionMarker.SetMarkerPosAndValue(progressBar_FG.RT.anchoredPosition.x + (((float)MarkerStates.currentMarkerState / (float)MarkerStates.maxMarkerState) * progressBar_FG.RT.rect.width), MarkerStates.currentMarkerState);

        if (!RequestedBluePrints.Any(rb => rb.reward.ascensionsNeeded == (ascensionMarkerStates.TryGetValue(activeSelection_MainType, out int state)?state:0) )) ascensionMarker.EnableMarker();
        _co = null;
    }

    public int GetExistingMarkerState()
    {
        return ascensionMarkerStates.TryGetValue(activeSelection_MainType, out int markerState)
                                                                ? markerState
                                                                : 0;
    }


    protected sealed override void NormalizeContainers()
    {       
        ContainersList.Sort((ca,cb) => ((LadderContainer)ca).initialOrder.CompareTo(((LadderContainer)cb).initialOrder)); // sorting the list by initial order 
        base.NormalizeContainers();
    }


    public void QuickUnload()
    {
        //var (newMarkerState, maxMarkerState) = GetCurrentAndMaxAscesionAmounts();

        ascensionMarkerStates[activeSelection_MainType] = MarkerStates.currentMarkerState;
        ascensionMarker.SetMarkerPosAndValue(progressBar_FG.RT.anchoredPosition.x + ((float)MarkerStates.currentMarkerState / (float)MarkerStates.maxMarkerState) * progressBar_FG.RT.rect.width, MarkerStates.currentMarkerState);
    }

    /*private (int newMarkerState, int maxMarkerState) GetCurrentAndMaxAscesionAmounts()
    {
        return AscensionTreeManager.AscensionTreeRewardsOfAllProducts.TryGetValue(activeSelection_MainType, out AscensionTreeRewardState ascensionTreeRewardState)
                                                                        ? (ascensionTreeRewardState.currentAscensionAmount, ascensionTreeRewardState.maxAscensionsAmount)
                                                                        : (0, 0);
    }*/
}
