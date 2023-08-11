
using System;
using UnityEngine;
using UnityEngine.EventSystems;


public class TypeSelector : SelectorButton<ProductType.AllType>  
{
    private  IScrollablePanel<EquipmentType.Type, ProductType.AllType> panel;

    public sealed override void AssignPanel(object panel)
    {
        this.panel = (IScrollablePanel<EquipmentType.Type,ProductType.AllType>)panel;
    }

    public sealed override void OnPointerDown(PointerEventData eventData)
    {

        if(eventData.pointerEnter != this.gameObject)
        {
            panel.SortBySubType(type, this);
        }
        else
        {
            if (panel.CheckActiveSubType(type))
            {
                return;
            }
            else
            {
                panel.SortBySubType(type, this);
            }
        }
            
    }

    public sealed override void OnPointerUp(PointerEventData eventData)
    {
    }
}
