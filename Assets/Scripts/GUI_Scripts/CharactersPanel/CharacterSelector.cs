using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterSelector : SelectorButton<CharacterType.Type>
{
    private IScrollablePanel<CharacterType.Type, Sort.Type> panel;
    public override void AssignPanel(object panel)
    {
        this.panel = (IScrollablePanel<CharacterType.Type, Sort.Type>)panel;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (panel.CheckActiveMainType(type))
        {
            return;
        }
        else
        {
            panel.SortByEquipmentType(type, this);                       //change the methodname 
        }
    }

    public override void OnPointerUp(PointerEventData eventData)
    {        
    }
}
