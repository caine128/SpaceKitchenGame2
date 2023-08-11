using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopUpgradeLlistItem_Scroller : ScrolledObjectsVisibility<SortableBluePrint_ExtractedData<ShopUpgrade>>
{
    ShopUpgradesListPanel_Manager panel;

    public sealed override void AssignPanelBelonged(ScrollablePanel panelBelonged_IN)
    {
        base.AssignPanelBelonged(panelBelonged_IN);
        panel = (ShopUpgradesListPanel_Manager)panelBelonged;
    }

    protected sealed override void SetControlPoints()
    {
        var rtPanel = panel.GetComponent<RectTransform>();
        worldControlPointMin = worldControlPointMin == default
                                ? transform.TransformPoint(rtPanel.rect.max.x, rtPanel.rect.max.y + panel.ContainerHeight / 2, 0).y
                                : worldControlPointMin;
        worldControlPointMax = worldControlPointMax == default
                                ? transform.TransformPoint(rtPanel.rect.min.x, rtPanel.rect.min.y - panel.ContainerHeight / 2, 0).y
                                : worldControlPointMax;
    }

    protected sealed override void CheckVisibility()
    {
        ///scrollrect is going upwards
        if (scrollRect.velocity.y > 0 && nextRecipeIndex < panel.RequestedBluePrints.Count)
        {
            if (panel.ContainersList[0].rt.position.y > worldControlPointMin)
            {
                UpdateContainerUpScroll();
            }
        }
        ///scrollrect is going downwards
        else if (scrollRect.velocity.y < 0 && previousRecipeIndex > -1)
        {
            if (panel.ContainersList[panel.IndiceIndex - 1].rt.position.y < worldControlPointMax)
            {
                UpdateContainerDownScroll();
            }
        }
    }

    private void UpdateContainerUpScroll()
    {
        var containerToMove = panel.ContainersList[0];
        var previousContainerPos = panel.ContainersList.LastOrDefault().rt.localPosition;
        panel.ContainersList.RemoveAt(0);
        panel.ContainersList.Insert(panel.IndiceIndex - 1, containerToMove);

        containerToMove.rt.localPosition = new Vector2(previousContainerPos.x, previousContainerPos.y - panel.OffsetDistance - panel.ContainerHeight);

        containerToMove.LoadContainer(panel.RequestedBluePrints[nextRecipeIndex]);
        SetRecipeIndexes(previousRecipeIndex + 1, nextRecipeIndex + 1);
    }
    private void UpdateContainerDownScroll()
    {
        var containerToMove = panel.ContainersList[panel.IndiceIndex - 1];
        var previousContainerPos = panel.ContainersList.FirstOrDefault().rt.localPosition;
        panel.ContainersList.RemoveAt(panel.IndiceIndex - 1);
        panel.ContainersList.Insert(0, containerToMove);

        containerToMove.rt.localPosition = new Vector2(previousContainerPos.x, previousContainerPos.y + panel.OffsetDistance + panel.ContainerHeight);

        containerToMove.LoadContainer(panel.RequestedBluePrints[previousRecipeIndex]);
        SetRecipeIndexes(previousRecipeIndex - 1, nextRecipeIndex - 1);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawCube(new Vector3(500, worldControlPointMin, 0), new Vector3(150, 150, 150));
        Gizmos.DrawCube(new Vector3(500, worldControlPointMax, 0), new Vector3(150, 150, 150));
    }
#endif
}
