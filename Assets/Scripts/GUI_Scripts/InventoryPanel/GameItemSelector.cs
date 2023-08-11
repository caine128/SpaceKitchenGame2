
using UnityEngine.EventSystems;

public class GameItemSelector : SelectorButton<GameItemType.Type>
{
    private IScrollablePanel<GameItemType.Type, Sort.Type> panel;

    public sealed override void AssignPanel(object panel)
    {
        this.panel = (IScrollablePanel<GameItemType.Type, Sort.Type>)panel;
    }

    public sealed override void OnPointerDown(PointerEventData eventData)
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

    public sealed override void OnPointerUp(PointerEventData eventData)
    {
    }
}
