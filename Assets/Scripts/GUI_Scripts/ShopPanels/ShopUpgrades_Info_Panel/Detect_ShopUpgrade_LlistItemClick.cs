using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Detect_ShopUpgrade_LlistItemClick : DetectClickRequest<SortableBluePrint_ExtractedData<ShopUpgrade>>
{
    public sealed override void OnPointerUp(PointerEventData eventData)
    {
        if (isValidClick && (Vector2.Distance(initialClickPosition, eventData.position) <= PanelManager.MAXCLICKOFFSET))
        {
            if (initialSelection is Container<SortableBluePrint_ExtractedData<ShopUpgrade>> recipeContainer_Small)
            {
                recipeContainer_Small.Tintsize();
                var blueprint = recipeContainer_Small.bluePrint;

                Debug.Log(blueprint.BluePrint.GetName());
            }
        }
    }
}
