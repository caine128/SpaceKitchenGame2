using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Ingredient : SortableBluePrint, ICollectible
{
    public readonly IngredientType.Type IngredientType;  
    private int AmountOwned = 0;
    public int MaxCap { get; private set; } 

    public Ingredient(IngredientType.Type ingredientType_IN)//, int? maxCap_IN = null)
    {
        IngredientType = ingredientType_IN;
        //SetMaxCap(maxCap_IN ?? (ShopData.CheckPresenceOfUpgrade(ShopUpgradesManager.Instance.GetRelevantResourceCabinet(this), out ShopUpgrade shopUpgrade) == true ? ((ResourceCabinetUpgrade)shopUpgrade).GetStorageCap() : 0));
        //MaxCap = maxCap_IN ?? (ShopData.CheckPresenceOfUpgrade(ShopUpgradesManager.Instance.GetRelevantResourceCabinet(this), out ShopUpgrade shopUpgrade) == true ? ((ResourceCabinetUpgrade)shopUpgrade).GetStorageCap() : 0); //SetMaxCap();
        //Debug.Log(MaxCap.ToString() + ingredientType_IN + " amountOwned : " + AmountOwned.ToString());
    }

    public override AssetReferenceAtlasedSprite GetAdressableImage()
    {
        return ResourcesManager.Instance.Resources_SO.ingredients[(int)IngredientType].spriteRef;
    }

    public void SetMaxCap(int newCap_IN)
    {
        MaxCap = newCap_IN;
        ResourcesManager.Instance.EvaluateMaxCapReached(this);
        Debug.Log("ingredient type : " + IngredientType + "maxCap : " + MaxCap);
        //if(IngredientGeneratorsManager.IngredientGenerators.TryGetValue(IngredientType, out IngredientGenerator ingredientGenerator))
        //{
        //    ingredientGenerator.SetSubscriptionStatus(MaxCap > AmountOwned ? false : true, IngredientType);
        //}
    }

    public bool IsMaxCapReached()
    {
        return AmountOwned >= MaxCap;
    }

    public void SetAmount(int amount)
    {
        AmountOwned += amount;
    }

    public int GetAmount()
    {
        return AmountOwned;
    }

    public override string GetDescription() // to be implemented 
    {
        return ResourcesManager.Instance.Resources_SO.ingredients[(int)IngredientType].description;
    }

    public override string GetName() // to be implemented 
    {
        return ResourcesManager.Instance.Resources_SO.ingredients[(int)IngredientType].name;
    }

    private int CalculateRefillCost_Gem(int requiredAmount)
    => ((MaxCap - requiredAmount), (MaxCap - AmountOwned)) switch
    {
        ( < 0,  _) => 20 + Mathf.CeilToInt((requiredAmount - AmountOwned) / 5),
        ( >= 0,  > 0 and <= 10) => 10,
        ( >= 0, 0) => 0,
        ( >= 0,  > 10) => 10 + Mathf.CeilToInt((MaxCap - AmountOwned) / 5),
        _ => throw new System.NotImplementedException("IngredientType : " + IngredientType.ToString() + MaxCap.ToString() + " : " + AmountOwned.ToString()),
    };


    private int CalculateRefillCost_Gold()
    {
        //switch (IngredientType)
        //{
        //    case global::IngredientType.Type.Meat:
        //    case global::IngredientType.Type.Flour_Grain:
        //    case global::IngredientType.Type.Veggie:
        //    case global::IngredientType.Type.Seafood:
        //        return 10;
        //    case global::IngredientType.Type.Fruit:
        //    case global::IngredientType.Type.Spices:
        //    case global::IngredientType.Type.Dairy:
        //    case global::IngredientType.Type.Fats_Oils:
        //        return 20;
        //    case global::IngredientType.Type.None:
        //    default:
        //        Debug.Log("shouldnt Calculate None Type ?ngredient's Refill Cost");
        //        return 0;
        //}

        return IngredientType switch
        {
            global::IngredientType.Type.Meat or
            global::IngredientType.Type.Flour_Grain or
            global::IngredientType.Type.Veggie or
            global::IngredientType.Type.Seafood => 10,

            global::IngredientType.Type.Fruit or
            global::IngredientType.Type.Spices or
            global::IngredientType.Type.Dairy or
            global::IngredientType.Type.Fats_Oils => 20,
            global::IngredientType.Type.None => 0,
            _ => throw new System.NotImplementedException(),
        };
    }

    public int CalculateRefillCost(int requiredAmount, ISpendable spendable)
    {
        return spendable switch
        {
            Gold => CalculateRefillCost_Gold(),
            Gem => CalculateRefillCost_Gem(requiredAmount),
            _ => throw new System.NotImplementedException(),
        };
    }

}
