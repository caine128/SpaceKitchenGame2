using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ScrollableDisplayPopupPanel : MonoBehaviour //: PopupPanel_Multi_SNG<ContentDisplayFrame>
{
    public ScrollRect ScrollRect
    {
        get => _scrollRect;
    }
    private ScrollRect _scrollRect;
    public RectTransform ScrollingContainer
    {
        get => _scrollingContainer_Rt;
    }

    private IEnumerator[] _co = new IEnumerator[2] ;

    [SerializeField] private RectTransform _scrollingContainer_Rt;

    private Vector2 _originalSCrollingcontainerSize;

    private List<ContentDisplayPopup_Generic> contentDisplays = new List<ContentDisplayPopup_Generic>(4);
    [SerializeField] UnityEngine.GameObject contentDisplay_pf;

    private int displayedSubcontainerAmount;

    public bool IsAnimating { get; private set; }
    private GUI_LerpMethods_Scale _gUI_LerpMethods_Scale;

    private void Awake()
    {
        _scrollRect = GetComponentInChildren<ScrollRect>();
        _originalSCrollingcontainerSize = _scrollingContainer_Rt.rect.size;
        _gUI_LerpMethods_Scale = GetComponent<GUI_LerpMethods_Scale>();
    }
 
    public void Load(PanelLoadDatas loadArgs)
    {
        _gUI_LerpMethods_Scale.RescaleDirect(finalScale: Vector3.zero,
                                            finalValueOperations: null);

        var objectsToLoad = loadArgs.bluePrintsToLoad;
        displayedSubcontainerAmount = objectsToLoad.Count;
        
        if (objectsToLoad.Count == 0)
        {
            throw new ArgumentOutOfRangeException();
        }

        if (displayedSubcontainerAmount > contentDisplays.Count)
        {
            CreateContentDisplays(displayedSubcontainerAmount - contentDisplays.Count);
        }

        var (positions, requiredContainerHeight) = GUI_CentralPlacement.MatrixPlacementCalculation(containingPanel_RT: _scrollingContainer_Rt,
                                                                    container_RT: contentDisplays[0].RT,
                                                                    requiredAmount: objectsToLoad.Count);

        if(_scrollingContainer_Rt.rect.height != requiredContainerHeight)
        {
            _scrollingContainer_Rt.sizeDelta = _scrollingContainer_Rt.rect.height < requiredContainerHeight
                                            ? new Vector2(0, Mathf.Abs(requiredContainerHeight - _scrollingContainer_Rt.rect.height))
                                            : requiredContainerHeight < _originalSCrollingcontainerSize.y
                                                    ? Vector2.zero
                                                    : new Vector2(0, _scrollingContainer_Rt.sizeDelta.y - (_scrollingContainer_Rt.rect.height - requiredContainerHeight));
        }

        /*if (_scrollingContainer_Rt.rect.height < requiredContainerHeight)
        {           
            _scrollingContainer_Rt.sizeDelta = new Vector2(0, Mathf.Abs(requiredContainerHeight - _scrollingContainer_Rt.rect.height));
        }
        else
        {
            (_scrollingContainer_Rt.offsetMax, _scrollingContainer_Rt.offsetMin) = _originalScrollingContainerOffsets;
        }*/

        contentDisplays.PlaceContainersMatrix(positions);
        contentDisplays.LoadContainers(loadData_IN: objectsToLoad.ConvertEnumerable(otl => new ContentDisplayInfo_PopupGeneric(
                                                                                                    spriteRef_IN: otl.blueprintToLoad.GetAdressableImage(),
                                                                                                    contentTitle: string.Empty,
                                                                                                    contentValue: string.Empty,
                                                                                                    GetTooltipText: otl.blueprintToLoad is IToolTipDisplayable tooltipDisplayable
                                                                                                                                ? tooltipDisplayable.GetToolTipText
                                                                                                                                : null)),
                                        hideAtInit: true);
    }

    public void DisplayPanel()
    {
        IsAnimating = true;
        _scrollRect.verticalNormalizedPosition = 0;
        _gUI_LerpMethods_Scale.Rescale(customInitialValue: null,
                                       secondaryInterpolation: null,
                                       finalScale: Vector3.one,
                                       followingAction_IN: DisplayContainers);
    }

    private void CreateContentDisplays(int requiredAmount)
    {
        var newContentDisplaySize = _scrollingContainer_Rt.rect.width / 9;
        for (int i = 0; i < requiredAmount; i++)
        {
            var newContentDisplay_go = Instantiate(contentDisplay_pf, _scrollingContainer_Rt, false);           
            var newcontentDisplay = newContentDisplay_go.GetComponent<ContentDisplayPopup_Generic>();
            
            newcontentDisplay.RT.sizeDelta = new(newContentDisplaySize, newContentDisplaySize);
            newcontentDisplay.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new(newContentDisplaySize, newContentDisplaySize);

            newcontentDisplay.ScaleDirect(isVisible: false, finalValueOperations: null);
            newContentDisplay_go.SetActive(false);
            contentDisplays.Add(newcontentDisplay);
        }
    }

    private void DisplayContainers()
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
        yield return TimeTickSystem.WaitForSeconds_QuarterSec;
        for (int i = 0; i < displayedSubcontainerAmount; i++)
        {
            contentDisplays[i].AnimateWithRoutine(customInitialValue: null,
                                                  secondaryInterpolation: null,
                                                  isVisible: true,
                                                  lerpSpeedmodifier: 1,
                                                  followingAction_IN: i == displayedSubcontainerAmount-1 
                                                                        ? () => IsAnimating = false
                                                                        : null);
           
            yield return TimeTickSystem.WaitForSeconds_ContainerActivation;
        }
        _co[0] = null;      
    }

    public void Unload()
    {

        for (int i = 0; i < _co.Length; i++)
        {
            if (_co[i] is not null)
            {
                StopCoroutine(_co[i]);
                _co[i] = null;
            }
        }
        IsAnimating = IsAnimating == false ? IsAnimating : false;
        displayedSubcontainerAmount = 0;
        foreach (var contentDisplay in contentDisplays)
        {

            contentDisplay.Unload();
        }
    }
}
