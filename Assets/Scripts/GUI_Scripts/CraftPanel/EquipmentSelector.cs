using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentSelector : SelectorButton<EquipmentType.Type>
{

    private IScrollablePanel<EquipmentType.Type, ProductType.AllType> panel;

    public sealed override void AssignPanel(object panel)
    {
        this.panel = (IScrollablePanel<EquipmentType.Type, ProductType.AllType>)panel;
    }


    public sealed override void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.pointerEnter != this.gameObject)
        {
            panel.SortByEquipmentType(type, this);
        }
        else
        {
            if (panel.CheckActiveMainType(type))
            {
                return;
            }
            else
            {

                panel.SortByEquipmentType(type, this);
            }
        }

    }

    public sealed override void OnPointerUp(PointerEventData eventData)
    {
       
    }
}
