using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class AscensionLadderPanel_Scroller : ScrolledObjectsVisibility<AscensionRewardState>
{
    float basePoint_Y;
    AscensionLadderPanel_Manager panel;




    public sealed override void AssignPanelBelonged(ScrollablePanel panelBelonged_IN)
    {
        base.AssignPanelBelonged(panelBelonged_IN);
        panel = (AscensionLadderPanel_Manager)panelBelonged;
    }
    protected override void SetControlPoints()
    {
        //var panel = (AscensionLadderPanel_Manager)panelBelonged;

        worldControlPointMin = 0 - panel.ContainerWidth;
        worldControlPointMax = Screen.width + panel.ContainerWidth;
        basePoint_Y = panel.ContainersList[0].rt.anchoredPosition.y;
    }

    protected sealed override void SetRecipeIndexes(int? previousRecipeIndex_IN = null, int? nextRecipeIndex_IN = null)
    {
        previousRecipeIndex = previousRecipeIndex_IN ?? 1;
        nextRecipeIndex = nextRecipeIndex_IN ?? panel.IndiceIndex;
    }

    protected sealed override void CheckVisibility()
    {

        if (scrollRect.velocity.x > 0 && previousRecipeIndex > 1)
        {
            if (panel.ContainersList[panel.IndiceIndex - 1].rt.position.x >= worldControlPointMax)
            {
                UpdateContainersRightScroll();
            }
        }
        else if ((scrollRect.velocity.x < 0 || _forceScrolled) && nextRecipeIndex < panel.RequestedBluePrints.Count)
        {
            if (panel.ContainersList[2].rt.position.x <= worldControlPointMin)
            {
                UpdateContainersLeftScroll();
            }
        }
    }

    protected override void ForceUpdateScrolledContainerForwardScroll()
    {
        while (panel.ContainersList[2].rt.position.x <= worldControlPointMin && nextRecipeIndex < panel.RequestedBluePrints.Count)
        {
            UpdateContainersLeftScroll();
        }
    }

    /*public void ForceUpdateScrolledContainer()
    {
        while (panel.ContainersList[2].rt.position.x <= worldControlPointMin && nextRecipeIndex < panel.RequestedBluePrints.Count)
        {
            Container<AscensionRewardState> containerToMove = panel.ContainersList[2];
            var previousContainer = panel.ContainersList.LastOrDefault();
            panel.ContainersList.RemoveAt(2);
            panel.ContainersList.Insert(panel.IndiceIndex - 1, containerToMove);

            containerToMove.rt.localPosition = GetXPosAccordingToPreviousContainer(containerToMove, previousContainer, insertAtEnd: true);

            containerToMove.LoadContainer(panel.RequestedBluePrints[nextRecipeIndex]);

            // Star Containers Deplacement and Loading
            if (!containerToMove.bluePrint.reward.isPremiumReward)
            {
                var amountOfStarContainers = panel.LadderStarList.Count;
                var starContainerToMove = panel.LadderStarList[1];
                // var previousStarContainer = panel.LadderStarList.LastOrDefault();
                panel.LadderStarList.RemoveAt(1);
                panel.LadderStarList.Insert(amountOfStarContainers - 1, starContainerToMove);


                starContainerToMove.RT.anchoredPosition = new Vector2(containerToMove.rt.anchoredPosition.x, starContainerToMove.RT.anchoredPosition.y);
                starContainerToMove.LoadStarContainer(value_IN: containerToMove.bluePrint.reward.ascensionsNeeded,
                                                      isBig: panel.LadderStarList[panel.LadderStarList.Count - 2].InitialSize == panel.LadderStarList[panel.LadderStarList.Count - 2].RT.sizeDelta ? true : false,
                                                       currentAscensionState: panel.MarkerStates.currentMarkerState);
            }

            SetRecipeIndexes(previousRecipeIndex + 1, nextRecipeIndex + 1);
        }
    }*/

    protected override void UpdateContainersRightScroll()
    {
        Container<AscensionRewardState> containerToMove = panel.ContainersList[panel.IndiceIndex - 1];
        var previousContainer = panel.ContainersList[2];
        panel.ContainersList.RemoveAt(panel.IndiceIndex - 1);
        panel.ContainersList.Insert(2, containerToMove);

        containerToMove.rt.localPosition = (GetXPosAccordingToPreviousContainer(containerToMove, previousContainer, insertAtEnd: false));

        containerToMove.LoadContainer(panel.RequestedBluePrints[previousRecipeIndex]);

        // Star Containers Deplacement and Loading
        if (!containerToMove.bluePrint.reward.isPremiumReward)
        {
            var amountOfStarContainers = panel.LadderStarList.Count;
            var starContainerToMove = panel.LadderStarList[amountOfStarContainers - 1];
            var previousStarContainer = panel.LadderStarList[1];
            panel.LadderStarList.RemoveAt(amountOfStarContainers - 1);
            panel.LadderStarList.Insert(1, starContainerToMove);

            starContainerToMove.RT.anchoredPosition = new Vector2(containerToMove.rt.anchoredPosition.x, starContainerToMove.RT.anchoredPosition.y);
            starContainerToMove.LoadStarContainer(value_IN: containerToMove.bluePrint.reward.ascensionsNeeded,
                                                  isBig: panel.LadderStarList[2].InitialSize == panel.LadderStarList[2].RT.sizeDelta ? true : false,
                                                  currentAscensionState: panel.GetExistingMarkerState());
        }

        SetRecipeIndexes(previousRecipeIndex - 1, nextRecipeIndex - 1);
    }

    protected override void UpdateContainersLeftScroll()
    {
        Container<AscensionRewardState> containerToMove = panel.ContainersList[2];
        var previousContainer = panel.ContainersList.LastOrDefault();
        panel.ContainersList.RemoveAt(2);
        panel.ContainersList.Insert(panel.IndiceIndex - 1, containerToMove);

        containerToMove.rt.localPosition = GetXPosAccordingToPreviousContainer(containerToMove, previousContainer, insertAtEnd: true);

        containerToMove.LoadContainer(panel.RequestedBluePrints[nextRecipeIndex]);

        // Star Containers Deplacement and Loading
        if (!containerToMove.bluePrint.reward.isPremiumReward)
        {
            var amountOfStarContainers = panel.LadderStarList.Count;
            var starContainerToMove = panel.LadderStarList[1];
            // var previousStarContainer = panel.LadderStarList.LastOrDefault();
            panel.LadderStarList.RemoveAt(1);
            panel.LadderStarList.Insert(amountOfStarContainers - 1, starContainerToMove);


            starContainerToMove.RT.anchoredPosition = new Vector2(containerToMove.rt.anchoredPosition.x, starContainerToMove.RT.anchoredPosition.y);
            starContainerToMove.LoadStarContainer(value_IN: containerToMove.bluePrint.reward.ascensionsNeeded,
                                                  isBig: panel.LadderStarList[panel.LadderStarList.Count - 2].InitialSize == panel.LadderStarList[panel.LadderStarList.Count - 2].RT.sizeDelta ? true : false,
                                                   currentAscensionState: panel.GetExistingMarkerState());
        }

        SetRecipeIndexes(previousRecipeIndex + 1, nextRecipeIndex + 1);
    }

    private Vector2 GetXPosAccordingToPreviousContainer(Container<AscensionRewardState> containerToMove_IN, Container<AscensionRewardState> previousContainer_IN, bool insertAtEnd)
        => (insertAtEnd, containerToMove_IN.rt.anchoredPosition.y != basePoint_Y && containerToMove_IN is LadderContainer_Small, containerToMove_IN.rt.anchoredPosition.y != basePoint_Y || containerToMove_IN is LadderContainer_Big) switch
        {
            (true, true, _) => new Vector2(previousContainer_IN.rt.localPosition.x, containerToMove_IN.rt.localPosition.y),
            //(true,false,_) => new Vector2(previousContainer_IN.rt.localPosition.x + previousContainer_IN.rt.rect.width + panel.OffsetDistance, containerToMove_IN.rt.localPosition.y),
            (true, false, _) => new Vector2(previousContainer_IN.rt.localPosition.x + panel.DistanceBetweenContainers, containerToMove_IN.rt.localPosition.y),
            //(false,_,true) => new Vector2(previousContainer_IN.rt.localPosition.x - containerToMove_IN.rt.rect.width - panel.OffsetDistance, containerToMove_IN.rt.localPosition.y),
            (false, _, true) => new Vector2(previousContainer_IN.rt.localPosition.x - panel.DistanceBetweenContainers, containerToMove_IN.rt.localPosition.y),
            (false, _, false) => new Vector2(previousContainer_IN.rt.localPosition.x, containerToMove_IN.rt.localPosition.y),
        };



    /*=> previousContainer_IN switch
    {
        LadderContainer_Small => (containerToMove_IN.rt.anchoredPosition.y == basePoint_Y, insertAtEnd) switch
        {
            (true, true) => new Vector2(previousContainer_IN.rt.localPosition.x + ((AscensionLadderPanel_Manager)panelBelonged).ContainerWidth_Small + panelBelonged.OffsetDistance, basePoint_Y),
            (false, true) => new Vector2(previousContainer_IN.rt.localPosition.x, containerToMove_IN.rt.anchoredPosition.y),
            (true, false) => new Vector2(previousContainer_IN.rt.localPosition.x - containerToMove_IN.rt.rect.width - panelBelonged.OffsetDistance, basePoint_Y),
            (false, false) => new Vector2(previousContainer_IN.rt.localPosition.x - ((AscensionLadderPanel_Manager)panelBelonged).ContainerWidth_Small - panelBelonged.OffsetDistance, containerToMove_IN.rt.anchoredPosition.y)

        },
        LadderContainer_Big => insertAtEnd switch
        {
            true => new Vector2(previousContainer_IN.rt.localPosition.x + panelBelonged.ContainerWidth + panelBelonged.OffsetDistance, basePoint_Y),
            false => new Vector2(previousContainer_IN.rt.localPosition.x - panelBelonged.ContainerWidth - panelBelonged.OffsetDistance, basePoint_Y),
        },
        _ => throw new NotImplementedException(),
    };*/
}
