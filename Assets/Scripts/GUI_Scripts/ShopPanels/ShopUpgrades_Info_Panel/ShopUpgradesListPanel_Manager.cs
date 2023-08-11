using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopUpgradesListPanel_Manager : ScrollablePanel<SortableBluePrint_ExtractedData<ShopUpgrade>>
{
    public override int IndiceIndex => _indiceIndex;
    private readonly int _indiceIndex = 6;
    public  float ContainerHeight { get => _containerHeight; protected set => _containerHeight = value; }
    private float _containerHeight;
    public sealed override float OffsetDistance { get => _offsetDistance; protected set => _offsetDistance = value; }
    private float _offsetDistance;

    protected sealed override void ConfigurePanel()
    {
        rt_Panel.offsetMin = new Vector2(0, rt_Panel.offsetMin.y / 2);
    }

    protected sealed override void PlaceAllContainers(List<Container<SortableBluePrint_ExtractedData<ShopUpgrade>>> recipeContainersListIN)
    {
        if (ContainerHeight == default)
            _containerHeight = recipeContainersListIN[0].rt.rect.height;

        if (_offsetDistance == default)
            _offsetDistance = (rt_Panel.rect.height - (_containerHeight * 3.5f)) / 4;

        var listOfContainersXPos = scrollingContainer.rect.width / 2;

        for (int i = 0; i < recipeContainersListIN.Count; i++)
        {
            recipeContainersListIN[i].rt.anchoredPosition = i == 0
                                                          ? new Vector2(listOfContainersXPos, -((_containerHeight / 2) + _offsetDistance))
                                                          : new Vector2(recipeContainersListIN[i - 1].rt.anchoredPosition.x, recipeContainersListIN[i - 1].rt.anchoredPosition.y - (_containerHeight + _offsetDistance));
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

    protected override List<SortableBluePrint_ExtractedData<ShopUpgrade>> CreateSortList()
    {
        var selectedWorkStation = ShopUpgradesInfoPanel_Manager.Instance.SelectedRecipe;

        switch (selectedWorkStation)
        {
            case WorkStationUpgrade workStationUpgrade:
                var associatedWoker = CharacterManager.Instance.WorkerList_SO.listOfWorkers.First(w => w.workStationPrerequisites[0].type == workStationUpgrade.GetWorkstationType());
                var debugList = ShopUpgradesManager.Instance.ShopUpgrades_SO.workstation_Upgrades.specsByLevel
                                    .Select(sbl => new SortableBluePrint_ExtractedData<ShopUpgrade>(
                                        data: (associatedWoker.characterName, associatedWoker.characterImageRef, sbl.level, sbl.maxWorkerLevelCap), bluePrint: selectedWorkStation)).ToList();
                return ShopUpgradesManager.Instance.ShopUpgrades_SO.workstation_Upgrades.specsByLevel
                                    .Select(sbl => new SortableBluePrint_ExtractedData<ShopUpgrade>(
                                        data:(associatedWoker.characterName, associatedWoker.characterImageRef, sbl.level, sbl.maxWorkerLevelCap),bluePrint:selectedWorkStation)).ToList();
            default:
                throw new System.NotImplementedException();
        }
    }

    protected sealed override float CalculateLastRecipeAnchorPoint(int amountToSort_IN)
    {
        return (amountToSort_IN * _containerHeight) + ((amountToSort_IN + 1) * OffsetDistance);
    }

    protected sealed override void ResizeScrolledRecipeContainer(float lastRecipeAnchorIN)
    {
        float difference = scrollingContainer.rect.height - lastRecipeAnchorIN;

        if (difference < 0 )
        {
            scrollingContainer.offsetMin += new Vector2(0, difference);
        }
        else if (difference > 0 && rt_Panel.rect.height != scrollingContainer.rect.height)
        {
            scrollingContainer.offsetMin = Vector2.zero;
        }
    }
}
