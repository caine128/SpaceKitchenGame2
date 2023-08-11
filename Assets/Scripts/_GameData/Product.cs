using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

[Serializable]
public class Product : GameObject, IEnhanceable, IEquatable<Product>, IToolTipDisplayable   // IRankable, IQualitative, IValuable
{
    private readonly ProductRecipe productRecipe;
    private readonly Quality.Level quality;
    public ReadOnlyDictionary<EnhancementType.Type, Enhancement> enhancementsDict_ro { get; }
    public bool isEnhanced { get; }
    //private int totalAdditiveValueModifier;
    //private readonly Dictionary<Recipes_SO.MealStatType, int> totalAdditiveStatModifiers;
    public Dictionary<Recipes_SO.MealStatType, List<(string bonusSource, int bonusAmount)>> StatIncreaseModifiers { get => _statIncreaseModifiers; }
    private readonly Dictionary<Recipes_SO.MealStatType, List<(string bonusSource, int bonusAmount)>> _statIncreaseModifiers;
    public bool IsValueModified => productRecipe.IsValueModified || _valueIncreaseModifiersFromEnhancements.Any() || !quality.Equals(Quality.Level.Normal);
    public IEnumerable<(string bonusName, string bonusAmount)> ValueIncreaseModifierStrings  => this._valueIncreaseModifiersFromEnhancements
                                                                                        .Select(vim => (vim.bonusName, $"+ {vim.bonusAmount}"))
                                                                                        .AddToEnumerable(factor : ()=> ($"{quality} Quality Level", $"x {Quality.ValueModifierPerQuality(quality)}"), condition: !Enum.Equals(quality,Quality.Level.Normal))
                                                                                        .Concat(productRecipe.ValueIncreaseModifierStrings);
    private readonly List<(string bonusName, float bonusAmount)> _valueIncreaseModifiersFromEnhancements;

    public Product(ProductRecipe productRecipe_IN, Quality.Level qualityLevel_IN, ReadOnlyDictionary<EnhancementType.Type, Enhancement> enhancementsDict_IN = null, DateTime? dateLastCrafted_IN = null)
    {
        productRecipe = productRecipe_IN;
        enhancementsDict_ro = enhancementsDict_IN ?? new ReadOnlyDictionary<EnhancementType.Type, Enhancement>(new Dictionary<EnhancementType.Type, Enhancement>());
        isEnhanced = IsEnhanced();
        quality = qualityLevel_IN;
        DateLastCrafted = dateLastCrafted_IN ?? DateTime.Now;
        (_valueIncreaseModifiersFromEnhancements, _statIncreaseModifiers) = GetEnhancementModifiers();
    }

    public Product(Product product_IN, Enhancement modified_Enhancement, bool isEnhancementAdded)
    {
        productRecipe = product_IN.productRecipe;
        quality = product_IN.quality;
        DateLastCrafted = DateTime.Now;

        var mutableDict = new Dictionary<EnhancementType.Type, Enhancement>(product_IN.enhancementsDict_ro);
        if (isEnhancementAdded)
        {
            /// Using the dictionnary indexer becuase if the enhancement is not null we would like to OVERRIDE IT !!!
            mutableDict[modified_Enhancement.GetEnhancementType()] = modified_Enhancement;
        }
        else
        {
            mutableDict.Remove(modified_Enhancement.GetEnhancementType());
        }

        enhancementsDict_ro = new ReadOnlyDictionary<EnhancementType.Type, Enhancement>(mutableDict);

        isEnhanced = IsEnhanced();
        (_valueIncreaseModifiersFromEnhancements, _statIncreaseModifiers) = GetEnhancementModifiers();
    }

    public bool IsStatModified(Recipes_SO.MealStatType statType)
     => GetQuality() != Quality.Level.Normal || (_statIncreaseModifiers.TryGetValue(statType, out var enhancementBonusesList) && enhancementBonusesList.Count > 0);

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
                                                      ISpendable.ToScreenFormat(GetValue()));//.ToString());

        return new ToolTipInfo (bodytextAsColumns: new string[] {retStr1.ToString(), retStr2.ToString()},
                                header: GetName(),
                                footer: GetDescription());
    }
    /*public string GetToolTipTextForModifiers_Value()
    {
        StringBuilder sb = new();
        var lastModifierItem = ValueIncreaseModifiers.Last();

        foreach (var modifier in ValueIncreaseModifiers)
        {
            sb.Append(modifier.bonusName).Append(' ', 10).Append(modifier.bonusAmount);

            if (!modifier.Equals(lastModifierItem))
                sb.AppendLine();
        }
        return sb.ToString();
    }*/

    public Quality.Level GetQuality()
    {
        return quality;
    }

    private (List<(string bonusName, float bonusAmount)> additiveValueModifier, Dictionary<Recipes_SO.MealStatType, List<(string bonusSource, int bonusAmount)>> additiveStatModifiers) GetEnhancementModifiers()
    {
        List<(string bonusName, float bonusAmount)> additiveValueModifier_retval = new(enhancementsDict_ro.Count);
        Dictionary<Recipes_SO.MealStatType, List<(string bonusSource, int bonusAmount)>> statIncreaseModifiers_retVal = new(capacity: productRecipe.recipeSpecs.mealStatBonuses.Length);

        foreach (var enhancement in enhancementsDict_ro.Where(ed => ed.Value != null).Select(ed=> ed.Value))
        {
            additiveValueModifier_retval.Add((bonusName: $"{enhancement.GetName()} bonus", bonusAmount: Enhance.GetMaxValueIncrease(this, enhancement)));

                foreach (var relevantStatBonus in Enhance.GetRelevantEnhancementBonuses(this, enhancement))
                {
                if (!statIncreaseModifiers_retVal.ContainsKey(relevantStatBonus.statType))
                {
                    statIncreaseModifiers_retVal.Add(relevantStatBonus.statType, new());
                }
                statIncreaseModifiers_retVal[relevantStatBonus.statType].Add((enhancement.GetName(), relevantStatBonus.statBonus));
                }             
        }
        return (additiveValueModifier_retval, statIncreaseModifiers_retVal);
    }

    public int GetLevel()
    {
        return productRecipe.GetLevel();
    }

    public override string GetName()
    {
        return productRecipe.GetName();
    }

    public int GetValue()
    {
        /*foreach (var (bonusName, bonusAmount) in ValueIncreaseModifiers)
        {
            Debug.LogWarning(string.Format("{0,-10}, {1}", bonusName, bonusAmount));
        }*/
        //return (Mathf.CeilToInt(productRecipe.GetValue() * Quality.ValueModifierPerQuality(quality))) + totalAdditiveValueModifier;
        return Mathf.CeilToInt((productRecipe.GetValue() * Quality.ValueModifierPerQuality(quality)) + _valueIncreaseModifiersFromEnhancements.Sum(vmi => vmi.bonusAmount));   //totalAdditiveValueModifier;
    }

    public override AssetReferenceAtlasedSprite GetAdressableImage()
    {
        return productRecipe.GetAdressableImage();
    }
    public IEnumerable<Recipes_SO.MealStatBonus> GetStatBonuses()
    {
        return productRecipe.recipeSpecs.mealStatBonuses.Select(msb => new Recipes_SO.MealStatBonus()
                                                                       {
                                                                           statType = msb.statType,
                                                                           statBonus = Mathf.CeilToInt(msb.statBonus * Quality.StatModifierPerQuality(quality)) 
                                                                                + (_statIncreaseModifiers.TryGetValue(msb.statType, out List<(string bonusSource, int bonusAmount)> bonuses)
                                                                                            ? bonuses.Sum(b=> b.bonusAmount)
                                                                                            : 0) 
                                                                                        
        });;
    }

    public ProductRecipe GetProductRecipe()
    {
        return productRecipe;
    }

    public override string GetDescription()
    {
        return productRecipe.recipeSpecs.recipeDescription;
    }

    public bool IsEnhanced()
    {
        foreach (var KVpair in enhancementsDict_ro)
        {
            if (KVpair.Value != null)
            {
                return true;
            }
        }
        return false;
    }

    public bool CanEnhanceWith(EnhancementType.Type enhancementType_IN)
    {
        if (enhancementsDict_ro.ContainsKey(enhancementType_IN))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public Product TryEnhance(Enhancement enhancement_IN, out bool isEnhancementSuccessful)
    {
        Product productToReturn = null;
        isEnhancementSuccessful = false;

        if (Enhance.EnhanceAttempt(this, enhancement_IN) == true) //&& Inventory.Instance.RemoveFromInventory(this, amount: 1)) 
        {
            isEnhancementSuccessful = true;
            Inventory.Instance.RemoveFromInventory(this, amount: 1);

            productToReturn = Inventory.Instance.CreateNew_EnhancedProduct(
                            this,
                            enhancement: enhancement_IN.GetQuality() == this.quality
                                        ? enhancement_IN
                                        : new Enhancement(productRecipe_IN: enhancement_IN.GetProductRecipe(),
                                                          qualityLevel_IN: this.quality,
                                                          dateLastCrafted_IN: enhancement_IN.DateLastCrafted),
                            isEnhancementAdded: true);
            
            /// in case the selecteditem should be changed (i.e. returning to iteminfo FROM ebhancement)
           /* if (GameItemInfoPanel_Manager.Instance.selectedRecipe is not null && Inventory.InventoryLookupDict_ByItem.TryGetValue(newProduct, out GameItem enhanceable))
            {
                /// To be able to catch the item in the inventory and not the new item which will not be used if itemtype alrady exists, then we just raise amount in inventory
                GameItemInfoPanel_Manager.Instance.BrowseInfo(enhanceable);
            }*/
        }
        return productToReturn;
    }

    public Product DestroyEnhancement(Enhancement enhancement_IN)
    {
        Product productToreturn = null;
        if (this.enhancementsDict_ro[enhancement_IN.GetEnhancementType()] is not null && Inventory.Instance.RemoveFromInventory(this, amount: 1))
        {
            productToreturn = Inventory.Instance.CreateNew_EnhancedProduct(this, enhancement_IN, isEnhancementAdded: false);

            /*/// in case the selecteditem should be changed (i.e. returning to iteminfo FROM ebhancement)
            if (GameItemInfoPanel_Manager.Instance.selectedRecipe is not null && Inventory.InventoryLookupDict_ByItem.TryGetValue(newProduct, out GameItem enhanceable))
            {
                /// To be able to catch the item in the inventory and not the new item which will not be used if itemtype alrady exists, then we just raise amount in inventory
                GameItemInfoPanel_Manager.Instance.BrowseInfo(enhanceable);
                
            }*/

        }
        return productToreturn;

    }

    public bool Equals(Product other)
    {
        if (other == null || GetType() != other.GetType())
        {
            return false;
        }
        else
        {
            return GetQuality() == other.GetQuality() &&
                    GetName() == other.GetName() &&
                    MethodHelper.AreDictionariesEqual(this.enhancementsDict_ro, other.enhancementsDict_ro);
        }
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as Product);
    }

    public override int GetHashCode()
    {
        int hashCode = 0;
        foreach (var pair in enhancementsDict_ro)
        {
            hashCode += (int)pair.Value.GetQuality();
            hashCode += (int)pair.Value.GetEnhancementType();
            hashCode += pair.Value.GetName().GetHashCode();
        }
        hashCode += (int)GetQuality() * GetName().GetHashCode() * 2;
        return hashCode;
    }


}
