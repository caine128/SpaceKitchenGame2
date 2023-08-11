
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySortSelector : SelectorButton<Sort.Type>
{
    private IScrollablePanel<GameItemType.Type, Sort.Type> panel;
    public sealed override void AssignPanel(object panel)
    {
        this.panel = (IScrollablePanel<GameItemType.Type, Sort.Type>)panel;
    }

    public sealed override void OnPointerDown(PointerEventData eventData)
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

    public sealed override void OnPointerUp(PointerEventData eventData)
    {
    }
}
