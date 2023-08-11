using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameItemInfoPanelTabSelectorButton : TabSelectorButton<Tab.GameItemInfoTabs> , IVariableTab
{
    public RectTransform RT { get { return _rt; } }
    [SerializeField] private RectTransform _rt;
    public float originalPosition_X { get; private set; }
    public float originalSizeDelta_X { get; private set; }

    private void Awake()
    {
        GetOriginalSizePosition(); // Later to Take up MAYBE 
    }

    public void GetOriginalSizePosition()
    {
        originalPosition_X = _rt.anchoredPosition.x;
        originalSizeDelta_X = _rt.sizeDelta.x;
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        //if (GameItemInfoPanel_Manager.Instance.activeTabType.Equals(_tabType))
        //{
        //    return;
        //}
        //else
        //{
            Debug.Log("LOADING THE TAB AGAIN");
            GameItemInfoPanel_Manager.Instance.PlaceTabPanel(_tabType, this);
        //}  
    }

    public void ArrangeButtonSizePosition(int buttonIndex_IN, float resizeRatio_IN)
    {
        Debug.Log(resizeRatio_IN);
        _rt.sizeDelta = new Vector2(_rt.sizeDelta.x * resizeRatio_IN, _rt.sizeDelta.y);
        float newPos_X = buttonIndex_IN * _rt.sizeDelta.x;
        _rt.anchoredPosition = new Vector2(newPos_X, _rt.anchoredPosition.y);
    }

    public void ResetButtonSizePosition()
    {
        _rt.anchoredPosition = new Vector2(originalPosition_X, _rt.anchoredPosition.y);
        _rt.sizeDelta = new Vector2(originalSizeDelta_X, _rt.sizeDelta.y);
    }


}
