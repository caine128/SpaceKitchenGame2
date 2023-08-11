using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Enhancement : GameObject , IEquatable<Enhancement>, IValuable , IRankable , IQualitative , ICraftable ,IStatable, IToolTipDisplayable
{
    private readonly ProductRecipe productRecipe;
    private readonly Quality.Level quality;
    private readonly EnhancementType.Type enhancementType; // serialized for debug purposes s!!

    public Enhancement(ProductRecipe productRecipe_IN, Quality.Level qualityLevel_IN, DateTime? dateLastCrafted_IN = null)
    {
        productRecipe = productRecipe_IN;
        DateLastCrafted = dateLastCrafted_IN ?? DateTime.Now;
        //quality = qualityLevel_IN ?? Quality.CalculateQualityLevel(productRecipe_IN.GetQualityModifers());
        quality = qualityLevel_IN;
        enhancementType = (EnhancementType.Type)(int)productRecipe.recipeSpecs.productType;
    }

    public ToolTipInfo GetToolTipText()
    {
        var retStr1 = NativeHelper.BuildString_Append("Quality",
                                                      Environment.NewLine,
                                                      "Lvl.",
                                                      Environment.NewLine,
                                                      "Value");

        var retStr2 = NativeHelper.BuildString_Append(GetQuality().ToString(),
                                                      Environment.NewLine,
                                                      GetLevel().ToString(),
                                                      Environment.NewLine,
                                                      ISpendable.ToScreenFormat(GetValue())); // ;

        return new ToolTipInfo(bodytextAsColumns: new string[2] {retStr1.ToString(),retStr2.ToString()},
                               header:GetName(),
                               footer: GetDescription());
    }

    public IEnumerable<Recipes_SO.MealStatBonus> GetStatBonuses()
    {
        return productRecipe.recipeSpecs.mealStatBonuses.Select(msb => new Recipes_SO.MealStatBonus()
        {
            statType = msb.statType,
            statBonus = Mathf.CeilToInt(msb.statBonus * Quality.StatModifierPerQuality(quality))
        });
    }

    public override string GetDescription()
    {
        return productRecipe.recipeSpecs.recipeDescription;
    }

    public override AssetReferenceAtlasedSprite GetAdressableImage()
    {
        return productRecipe.recipeSpecs.receipeImageRef;
    }

    public int GetLevel()
    {
        return productRecipe.recipeSpecs.productLevel;
    }

    public override string GetName()
    {
        return productRecipe.recipeSpecs.recipeName;
    }

    public ProductRecipe GetProductRecipe()
    {
        return productRecipe;
    }

    public Quality.Level GetQuality()
    {
        return quality;
    }

    public int GetValue()
    {
        return Mathf.CeilToInt(productRecipe.GetValue() * Quality.ValueModifierPerQuality(quality));
    }

    public EnhancementType.Type GetEnhancementType()
    {
        return enhancementType;
    }

    public bool Equals(Enhancement other)
    {
        if (other == null || GetType() != other.GetType())
        {
            return false;
        }
        else
        {
            return GetQuality() == other.GetQuality() &&
                    GetEnhancementType() == other.GetEnhancementType() &&
                    GetName() == other.GetName();
        }
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as Enhancement);
    }

    public override int GetHashCode()
    {
        int hashCode = (int)GetQuality() * (int)GetEnhancementType() * GetName().GetHashCode();
        return hashCode;
    }


}
