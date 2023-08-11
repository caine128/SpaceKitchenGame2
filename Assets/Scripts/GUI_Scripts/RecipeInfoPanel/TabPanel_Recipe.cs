using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class TabPanel_Recipe : TabPanel_Animated<Tab.RecipeInfoTabs>
{
    [SerializeField] private RectTransform rt; // NOTE USED ??? IF YES REMOVE !!
    private float iconWidth;

    [SerializeField] private TextMeshProUGUI recipeDescription;

    [SerializeField] private ContentDisplayIngredient[] ingredientContentDisplays;
    [SerializeField] private ContentDisplayAdditionalItems[] additionalItemsDisplay;
    [SerializeField] private ContentDisplayWorker[] workerContentDisplays;

    public override Tab.RecipeInfoTabs TabType { get { return _tabType; } }
    [SerializeField] private Tab.RecipeInfoTabs _tabType;

    private void Awake()
    {
        co = new IEnumerator[3];
        iconWidth = ingredientContentDisplays[0].GetComponent<RectTransform>().rect.width;      
    }
    private void Start()
    {
        GUI_CentralPlacement.DeactivateUnusedContainers(0, ingredientContentDisplays);
        GUI_CentralPlacement.DeactivateUnusedContainers(0, additionalItemsDisplay);
        GUI_CentralPlacement.DeactivateUnusedContainers(0, workerContentDisplays);
    }

    public override void LoadInfo()
    {
        recipeDescription.text = RecipeInfoPanel_Manager.Instance.SelectedRecipe.recipeSpecs.recipeDescription;
        ingredientContentDisplays.PlaceContainers(RecipeInfoPanel_Manager.Instance.SelectedRecipe.recipeSpecs.requiredIngredients.Length, iconWidth, isHorizontalPlacement: true);
        ingredientContentDisplays.LoadContainers(RecipeInfoPanel_Manager.Instance.SelectedRecipe, RecipeInfoPanel_Manager.Instance.SelectedRecipe.recipeSpecs.requiredIngredients.Length, hideAtInit: true);

        additionalItemsDisplay.PlaceContainers(RecipeInfoPanel_Manager.Instance.SelectedRecipe.recipeSpecs.requiredAdditionalItems.Length, iconWidth, isHorizontalPlacement: true);
        additionalItemsDisplay.LoadContainers(RecipeInfoPanel_Manager.Instance.SelectedRecipe, RecipeInfoPanel_Manager.Instance.SelectedRecipe.recipeSpecs.requiredAdditionalItems.Length, hideAtInit: true);

        workerContentDisplays.PlaceContainers(RecipeInfoPanel_Manager.Instance.SelectedRecipe.recipeSpecs.requiredworkers.Length, iconWidth, isHorizontalPlacement: true);
        workerContentDisplays.LoadContainers(RecipeInfoPanel_Manager.Instance.SelectedRecipe, RecipeInfoPanel_Manager.Instance.SelectedRecipe.recipeSpecs.requiredworkers.Length, hideAtInit:true);
        
    }
    public override void HideContainers()
    {
        
        ingredientContentDisplays.HideContainers();
        additionalItemsDisplay.HideContainers();
        workerContentDisplays.HideContainers();
    }

    public override void DisplayContainers()
    {
        ingredientContentDisplays.SortContainers(customInitialValues:null,
                                                 secondaryInterpolations:null,
                                                 amountToSort_IN: RecipeInfoPanel_Manager.Instance.SelectedRecipe.recipeSpecs.requiredIngredients.Length, 
                                                 enumeratorIndex: 0,
                                                 parentPanel_IN: this,
                                                 lerpSpeedModifiers: null);
        additionalItemsDisplay.SortContainers(customInitialValues: null,
                                              secondaryInterpolations: null,
                                              amountToSort_IN: RecipeInfoPanel_Manager.Instance.SelectedRecipe.recipeSpecs.requiredAdditionalItems.Length, 
                                              enumeratorIndex: 1, 
                                              parentPanel_IN: this,
                                              lerpSpeedModifiers: null);
        workerContentDisplays.SortContainers(customInitialValues: null,
                                             secondaryInterpolations: null,
                                             amountToSort_IN: RecipeInfoPanel_Manager.Instance.SelectedRecipe.recipeSpecs.requiredworkers.Length, 
                                             enumeratorIndex: 2,
                                             parentPanel_IN: this,
                                             lerpSpeedModifiers: null);
    }

    public override void UnloadInfo()
    {
        GUI_CentralPlacement.DeactivateUnusedContainers(0, ingredientContentDisplays);
        GUI_CentralPlacement.DeactivateUnusedContainers(0, additionalItemsDisplay);
        GUI_CentralPlacement.DeactivateUnusedContainers(0, workerContentDisplays);

        base.UnloadInfo();
    }


}
