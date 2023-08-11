using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RecipesAvailablePanel_Manager : ScrollablePanel<ProductRecipe>
{
    public override int IndiceIndex => _indiceIndex;
    private readonly int _indiceIndex = 12;

    public sealed override float ContainerWidth { get => _containerWidth; protected set => _containerWidth = value; }
    private float _containerWidth;
    public sealed override float OffsetDistance { get => _offsetDistance; protected set => _offsetDistance = value; }
    private float _offsetDistance;

    public sealed override List<Container<ProductRecipe>> ContainersList => _containersList_s;
    private static List<Container<ProductRecipe>> _containersList_s = new List<Container<ProductRecipe>>();


    protected override void ConfigurePanel()
    {
        rt_Panel.offsetMin = new Vector2(0, rt_Panel.offsetMin.y / 2);
    }

    protected sealed override void PlaceAllContainers(List<Container<ProductRecipe>> recipeContainersListIN)
    {
        if(_offsetDistance == default) _offsetDistance = (rt_Panel.rect.width - (ContainerWidth * 4)) / 5f;

        Vector2 lastAnchorPoint = new(- _containerWidth / 2,
                                        _containerWidth / 2);

        for (int i = 0; i < recipeContainersListIN.Count; i++)
        {

            var rt_IN = recipeContainersListIN[i].rt;
            var rowNo = Mathf.FloorToInt(i / 4);
           
            rt_IN.anchoredPosition = i % 4 == 0
                                    ? new Vector2(- _containerWidth / 2 + _containerWidth + _offsetDistance,
                                                   (_containerWidth / 2) - ((rowNo + 1) * (_containerWidth + _offsetDistance)))
                                    : new Vector2(lastAnchorPoint.x + _containerWidth + _offsetDistance,
                                                  lastAnchorPoint.y);
            lastAnchorPoint = rt_IN.anchoredPosition;
        }
    }

    public void LoadContainers()
    {
        var listToSort = CreateSortList();    
        Arrange(listToSort);
    }

    public void DisplayContainers()
    {
        SortRecipes(RequestedBluePrints.Count);
    }

    protected sealed override List<ProductRecipe> CreateSortList()
    {
        var activeWorker = (Worker)CharactersInfoPanel_Manager.Instance.SelectedRecipe;
        return activeWorker.GetWorkerRecipes().ToList();
    }

    protected sealed override float CalculateLastRecipeAnchorPoint(int amountToSort_IN)
    {
        var remainingContainersAmount = amountToSort_IN % 4;
        var totalNumRows = remainingContainersAmount != 0
                            ? (amountToSort_IN / 4) + 1
                            : amountToSort_IN / 4;

        return (totalNumRows * (ContainerWidth + OffsetDistance))+ OffsetDistance ;
    }

    protected sealed override void ResizeScrolledRecipeContainer(float lastRecipeAnchorIN)
    {
        //float difference = rt_Panel.rect.height - lastRecipeAnchorIN;
        float difference = scrollingContainer.rect.height - lastRecipeAnchorIN;

        if (difference < 0)
        {
            scrollingContainer.offsetMin += new Vector2(0, difference);
        }
        else if(difference > 0 && rt_Panel.rect.height != scrollingContainer.rect.height)
        {
            scrollingContainer.offsetMin = Vector2.zero;
        }

    }
}
