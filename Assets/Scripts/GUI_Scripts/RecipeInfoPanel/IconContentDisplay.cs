using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IconContentDisplay : MonoBehaviour
{
    [SerializeField] public RectTransform rt;
    public Image IconImage { get{ return _iconImage; }}
    [SerializeField] private Image _iconImage;
    public TextMeshProUGUI IconAmounttext { get{ return _iconAmountText; }}
    [SerializeField] private TextMeshProUGUI _iconAmountText;
    [SerializeField] private DisplayContainer.Type contentdisplayType;

    [SerializeField] private TextMeshProUGUI _contentDescription;
    [SerializeField] private Image _additionalInfoImage;
    [SerializeField] public TextMeshProUGUI _additionalInfoText;
    [SerializeField] public Image filledImageBG;

 

    public void Load(ProductRecipe productRecipe, int indexNo) // LATER TO LOAD Sprite iconSprite, 
    {
        switch (contentdisplayType)
        {
            case DisplayContainer.Type.None:
                break;
            case DisplayContainer.Type.IngredientsDisplay:
                var isAmountEnough = productRecipe.GetRequiredIngredients(indexNo, out _, out _) <= ResourcesManager.CheckAmountOfIngredient(productRecipe.recipeSpecs.requiredIngredients[indexNo].ingredient, out _);                             //ResourcesManager.ingredientsDict[productRecipe.recipeSpecs.requiredIngredients[indexNo].ingredient].GetAmount();
                _iconAmountText.SetAsModifiableSpec(productRecipe.GetRequiredIngredients(indexNo, out _, out _ ).ToString(), productRecipe.GetRequiredIngredients_Modification(indexNo), isAmountEnough);
                break;
            case DisplayContainer.Type.AdditionalItemsDisplay:
                _iconAmountText.SetAsModifiableSpec(productRecipe.GetRequiredAdditionalItems(indexNo, out _, out _).ToString(), productRecipe.GetRequiredAdditionalItems_Modification(indexNo));
                break;              
            case DisplayContainer.Type.WorkerDisplay:
                 _iconAmountText.text = productRecipe.recipeSpecs.requiredworkers[indexNo].requiredWorkerLevel.ToString();           
                break;

            case DisplayContainer.Type.CraftUpgradesDisplay:

                switch (productRecipe.recipeSpecs.craftingUpgrades[indexNo].craftUpgradeType)
                {
                    case Recipes_SO.CraftUpgradeType.CraftTimeReduction:
                        _iconAmountText.text = productRecipe.recipeSpecs.craftingUpgrades[indexNo].craftTimeReductionModifier.ToString();
                        _contentDescription.text = "Cook Faster";
                        break;
                    case Recipes_SO.CraftUpgradeType.IngredientReduction:
                        _iconAmountText.text = productRecipe.recipeSpecs.craftingUpgrades[indexNo].ingredientReduction.reductionAmount.ToString();
                        _contentDescription.text = "Ingredient Economy";
                        break;
                    case Recipes_SO.CraftUpgradeType.ExtraComponentReduction:
                        _iconAmountText.text = productRecipe.recipeSpecs.craftingUpgrades[indexNo].extraComponentReduction.reductionAmount.ToString();
                        _contentDescription.text = "Component Economy";
                        break;
                    case Recipes_SO.CraftUpgradeType.ValueIncrease:
                        _iconAmountText.text = productRecipe.recipeSpecs.craftingUpgrades[indexNo].valueIncreaseModifier.ToString();
                        _contentDescription.text = "Value Increase";
                        break;
                    case Recipes_SO.CraftUpgradeType.QualityChanceIncrease:
                        _iconAmountText.text = productRecipe.recipeSpecs.craftingUpgrades[indexNo].qualityChanceIncreaseModifier.ToString();
                        _contentDescription.text = "Quality Chance";
                        break;
                    case Recipes_SO.CraftUpgradeType.UnlockRecipe:
                        _iconAmountText.text = "";
                        _contentDescription.text = "Unlock Recipe";
                        break;
                }
                break;

            case DisplayContainer.Type.AscensionDisplay:

                switch (productRecipe.recipeSpecs.ascensionUpgrades[indexNo].ascensionUpgradeType)
                {
                    case Recipes_SO.AscensionUpgradeType.CraftTimeReduction:
                        _iconAmountText.text = productRecipe.recipeSpecs.ascensionUpgrades[indexNo].craftTimeReductionModifier.ToString();
                        _contentDescription.text = "Cook Faster";
                        break;
                    case Recipes_SO.AscensionUpgradeType.IngredientReduction:
                        _iconAmountText.text = productRecipe.recipeSpecs.ascensionUpgrades[indexNo].ingredientReduction.reductionAmount.ToString();
                        _contentDescription.text = "Ingredient Economy";
                        break;
                    case Recipes_SO.AscensionUpgradeType.ExtraComponentReduction:
                        _iconAmountText.text = productRecipe.recipeSpecs.ascensionUpgrades[indexNo].extraComponentReduction.reductionAmount.ToString();
                        _contentDescription.text = "Component Economy";
                        break;
                    case Recipes_SO.AscensionUpgradeType.QualityChanceIncrease:
                        _iconAmountText.text = productRecipe.recipeSpecs.ascensionUpgrades[indexNo].qualityChanceIncreaseModifier.ToString();
                        _contentDescription.text = "Quality Chance";
                        break;
                    case Recipes_SO.AscensionUpgradeType.MultiCraftChance:
                        _iconAmountText.text = productRecipe.recipeSpecs.ascensionUpgrades[indexNo].multicraftChanceModifier.ToString();
                        _contentDescription.text = "Multicraft Chance";
                        break;
                    case Recipes_SO.AscensionUpgradeType.RequiredProductReduction:
                        _iconAmountText.text = productRecipe.recipeSpecs.ascensionUpgrades[indexNo].requiredProductReduction.reductionAmount.ToString();
                        _contentDescription.text = "Component Economy";
                        break;
                }
                break;

        }
    }


}
