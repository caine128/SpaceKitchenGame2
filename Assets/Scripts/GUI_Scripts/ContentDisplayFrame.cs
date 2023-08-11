using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

public class ContentDisplayFrame : ContentDisplay_WithText, IPointerDownHandler, IPointerUpHandler, IPlacableRt, ILoadable<ContentDisplayInfo_ConentDisplayFrame>
{
    public RectTransform RT { get { return _rt; } }
    [SerializeField] private RectTransform _rt;

    Lazy<ToolTipInfo> _toolTipInfoShaped = null;
    //private string _toolTipInfoShaped;

    public override void Load(ContentDisplayInfo_ContentDisplay_WithText info)
    {
        Load((ContentDisplayInfo_ConentDisplayFrame)(ContentDisplayInfo)info);
    }

    public void Load(ContentDisplayInfo_ConentDisplayFrame info)
    {
        switch (info.indexNo_IN.HasValue)
        {
            case true:
                indexNO = info.indexNo_IN.Value;
                if (contentInfo.gameObject.activeInHierarchy != true) contentInfo.gameObject.SetActive(true);

                contentInfo.text = indexNO.ToString();
                break;

            case false:
            default:
                if (contentInfo.gameObject.activeInHierarchy != false) contentInfo.gameObject.SetActive(false);         
                break;
        }

        _toolTipInfoShaped = info.bluePrint_IN is IToolTipDisplayable tooltipDisplayabe
                                                    ? new Lazy<ToolTipInfo>(tooltipDisplayabe.GetToolTipText)
                                                    : null;
        SelectAdressableSpritesToLoad(info.bluePrint_IN.GetAdressableImage());       
    }



    /*public override void Load(SortableBluePrint bluePrint_IN, int amount_IN)
    {
        if (contentInfo.gameObject.activeInHierarchy != true) contentInfo.gameObject.SetActive(true);

        contentInfo.text = amount_IN.ToString();

        SelectAdressableSpritesToLoad(bluePrint_IN.GetAdressableImage());
        contentObject = bluePrint_IN;
    }

    public void Load(SortableBluePrint bluePrint_IN)
    {
        if (contentInfo.gameObject.activeInHierarchy != false) contentInfo.gameObject.SetActive(false);

        SelectAdressableSpritesToLoad(bluePrint_IN.GetAdressableImage());
        contentObject = bluePrint_IN;
    }*/



    public void OnPointerDown(PointerEventData eventData)
    {
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        var toolTipText = _toolTipInfoShaped?.Value ?? null;
        if (toolTipText is not null)
        {
            var panelToload = PanelManager.InvokablePanels[typeof(Tooltip_Panel_Manager)];

            PanelManager.ActivateAndLoad(invokablePanel_IN: panelToload,
                                         //preLoadAction_IN: () => (panelToload.MainPanel as Tooltip_Panel_Manager).SetDynamicBGSize(toolTipText),
                                         panelLoadAction_IN: () => (panelToload.MainPanel as Tooltip_Panel_Manager).SetDynamicBGSize(toolTipText, _rt));
        }
        else
        {
            Debug.LogWarning("There is no tooltip to display");
        }

        /*if(contentObject is SortableBluePrint bluePrint)
        {
            Debug.Log(bluePrint.GetName() + " " + bluePrint.GetDescription() + " of quality " + ((IQualitative)bluePrint).GetQuality());
        }*/
    }

    /*private string ShapeToolTipText(ContentDisplayInfo_ConentDisplayFrame rawToolTipInfo)
    {
        return NativeHelper.BuildString_Append(rawToolTipInfo.bluePrint_IN.GetName(),
                                        rawToolTipInfo.bluePrint_IN is IQualitative qualitative ? Environment.NewLine + qualitative.GetQuality().ToString() : string.Empty,
                                        rawToolTipInfo.indexNo_IN is not null ? Environment.NewLine + rawToolTipInfo.indexNo_IN : string.Empty);
    }*/


    public override void Unload()
    {
        base.Unload();
        _toolTipInfoShaped = null;
        UnloadAdressableSprite();
    }


}
