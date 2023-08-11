using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterSortSelector : SelectorButton<Sort.Type>
{
    private IScrollablePanel<CharacterType.Type, Sort.Type> panel;
    public override void AssignPanel(object panel)
    {
        this.panel = (IScrollablePanel<CharacterType.Type, Sort.Type>)panel;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.pointerEnter != this.gameObject)
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

    public override void OnPointerUp(PointerEventData eventData)
    {
    }
}
