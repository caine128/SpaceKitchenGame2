using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;

public class SmallRecipes_Scroller : ScrolledObjectsVisibility<ProductRecipe>
{
    RecipesAvailablePanel_Manager panel;


    public sealed override void AssignPanelBelonged(ScrollablePanel panelBelonged_IN)
    {
        base.AssignPanelBelonged(panelBelonged_IN);
        panel = (RecipesAvailablePanel_Manager)panelBelonged;
    }

    protected sealed override void SetControlPoints()
    {
        var rtPanel = panel.GetComponent<RectTransform>();
        worldControlPointMin = worldControlPointMin == default
                                ? transform.TransformPoint(rtPanel.rect.max.x, rtPanel.rect.max.y + panel.ContainerWidth / 2, 0).y 
                                : worldControlPointMin;
        worldControlPointMax = worldControlPointMax == default 
                                ?transform.TransformPoint(rtPanel.rect.min.x, rtPanel.rect.min.y - panel.ContainerWidth / 2, 0).y 
                                :worldControlPointMax;
    }

    protected sealed override void CheckVisibility()
    {
        ///scrollrect is going upwards
        if (scrollRect.velocity.y > 0 && nextRecipeIndex < panel.RequestedBluePrints.Count) 
        {
            if (panel.ContainersList[0].rt.position.y > worldControlPointMin)
            {
                Debug.LogWarning("reached upwards, nextrecipeindex is : " + nextRecipeIndex);
                UpdateContainerUpScroll();
            }
        }
        ///scrollrect is going downwards
        else if (scrollRect.velocity.y < 0 && previousRecipeIndex > -1)
        {
            if (panel.ContainersList[panel.IndiceIndex - 1].rt.position.y < worldControlPointMax)
            {
                Debug.LogWarning("reached downwards, previousrecipeindex is :" + previousRecipeIndex);
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

        containerToMove.rt.localPosition = (nextRecipeIndex + 1) % 4 == 1
                                            ? new Vector2(previousContainerPos.x - (3 * (panel.ContainerWidth + panel.OffsetDistance)),
                                                          previousContainerPos.y - panel.OffsetDistance - panel.ContainerWidth)
                                            : new Vector2(previousContainerPos.x + panel.OffsetDistance + panel.ContainerWidth,
                                                          previousContainerPos.y);

        containerToMove.LoadContainer(panel.RequestedBluePrints[nextRecipeIndex]);
        SetRecipeIndexes(previousRecipeIndex + 1, nextRecipeIndex + 1);
    }
    private void UpdateContainerDownScroll()
    {
        var containerToMove = panel.ContainersList[panel.IndiceIndex - 1];
        var previousContainerPos = panel.ContainersList.FirstOrDefault().rt.localPosition;
        panel.ContainersList.RemoveAt(panel.IndiceIndex - 1);
        panel.ContainersList.Insert(0, containerToMove);

        containerToMove.rt.localPosition = (previousRecipeIndex + 1) % 4 == 0
                                            ? new Vector2(previousContainerPos.x + (3 * (panel.ContainerWidth + panel.OffsetDistance)),
                                                         previousContainerPos.y + panel.OffsetDistance + panel.ContainerWidth)
                                            : new Vector2(previousContainerPos.x - panel.OffsetDistance - panel.ContainerWidth,
                                                         previousContainerPos.y);

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
