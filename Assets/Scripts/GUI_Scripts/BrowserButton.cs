using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class BrowserButton<T_BluePrint> : Button_Base  //MonoBehaviour , IPointerDownHandler
    where T_BluePrint : class
{
    [SerializeField] protected ButtonIteration.Type iterationType;
    protected IBrowsablePanel<T_BluePrint> infoPanel;


    public void ButtonConfig(IBrowsablePanel<T_BluePrint> infoPanel_In)
    {
        infoPanel = infoPanel_In;
    }

    public sealed override void OnPointerDown(PointerEventData eventData)
    {
        switch (iterationType)
        {
            case ButtonIteration.Type.Previous:
                Browse(-1);
                break;
            case ButtonIteration.Type.Next:
                Browse(1);
                break;
        }
    }

    public sealed override void OnPointerUp(PointerEventData eventData)
    {
       
    }

    protected void Browse(int browseOrder)
    {
        var listToIterate = infoPanel.ListToIterate;
        if (infoPanel.CurrentIndice + browseOrder >= 0 && infoPanel.CurrentIndice + browseOrder < listToIterate.Count)
        {
            infoPanel.BrowseInfo(listToIterate[infoPanel.CurrentIndice + browseOrder]);
        }
    }
}
