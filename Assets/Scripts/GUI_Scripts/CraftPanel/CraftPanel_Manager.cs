using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CraftPanel_Manager : ScrollablePanel<ProductRecipe, EquipmentType.Type, ProductType.AllType>, IRefreshablePanel //, IDeallocatablePanel
{
    public static Floating_YPpoints floating_YPpoints;
    public static float SubContainerWidth;

    public override int IndiceIndex => _indiceIndex;
    private readonly int _indiceIndex = 11;
    public struct Floating_YPpoints
    {
        public float middlePoint;
        public float upperPoint;
        public float lowerPoint;
    }


    protected override void Start() // later to be taken to panel config
    {
        base.Start();

        activeSelection_MainType = EquipmentType.Type.None;// activeSelection.active_MainType = EquipmentType.Type.None;
        activeSelection_SubtType = ProductType.AllType.None;// activeSelection.active_SubtType = ProductType.AllType.None;

        ExecuteEvents.Execute(mainType_Selector_Buttons[7].gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerDownHandler);
    }

    protected override void InitializeContainers()
    {

        base.InitializeContainers();

        floating_YPpoints.upperPoint = ContainersList[0].rt.rect.height * (-30f / 274f);
        floating_YPpoints.lowerPoint = ContainersList[0].rt.rect.height * (-75f / 274f);
        floating_YPpoints.middlePoint = ContainersList[0].rt.rect.height * (-50f / 274f);

        var recipeContainer = (RecipeContainer)ContainersList[0];
        SubContainerWidth = recipeContainer.subContainerWidth;
    }



    public override void ScrollToSelection(ProductRecipe productRecipe_IN, bool markSelection)
    {
        var equalityComparer = new RecipeEqualityComparer();
        //float targetForwardPos = 0;
        int selectedContainerIndex = 0;
        for (int i = 0; i < RequestedBluePrints.Count; i++)
        {
            if (equalityComparer.Equals(RequestedBluePrints[i], productRecipe_IN))
            {
                selectedContainerIndex = i;
                //targetForwardPos = (CardWidth / 2 * i) + (CardWidth / 2 * (i + 1)) + (OffsetDistance * (i + 1));
                break;
            }
        }

      CalculateForwardPosAndScroll(selectedContainerIndex, markSelection);
    }

    protected override List<ProductRecipe> CreateSortList()//EquipmentType.Type mainSelector, ProductType.AllType subSelector)
    {
        var mainSelector = activeSelection_MainType;// var mainSelector = activeSelection.active_MainType;
        var subSelector = activeSelection_SubtType;// var subSelector = activeSelection.active_SubtType;
        if (subSelector.IsSortType())
        {
            var derived_SortType = subSelector.GetDerivedSortType();
            DefineSortType(derived_SortType, RecipeManager.RecipesAvailable_List);
            return RecipeManager.RecipesAvailable_List;
        }
        else
        {
            var derived_SubType = subSelector.GetDerivedType();
            return RecipeManager.Instance.GetRootList_W_LockedItems(mainSelector, derived_SubType).ToList();
        }
    }

    /*
    protected override List<ProductRecipe> CreateSortList(EquipmentType.Type mainSelector, ProductType.AllType subSelector)
    {
        if (subSelector.IsSortType())
        {
            var derived_SortType = subSelector.GetDerivedSortType();
            DefineSortType(derived_SortType, RecipeManager.RecipesAvailable_List);
            return RecipeManager.RecipesAvailable_List;
        }
        else
        {
            var derived_SubType = subSelector.GetDerivedType();
            return RecipeManager.Instance.GetRootList_W_LockedItems(mainSelector, derived_SubType);
        }
    }*/

    public void RefreshPanel()
    {
        var listToSort = CreateSortList();// activeSelection.active_MainType, activeSelection.active_SubtType);
        ArrangeAndSort(listToSort);
    }



}
