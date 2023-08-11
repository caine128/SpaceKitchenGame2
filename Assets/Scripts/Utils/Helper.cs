using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;

public static class Helper
{


    #region CRAFTPANEL ENUMS
    public static bool IsOfSameIndex(this ProductType.Type typeEnum, ProductType.AllType allTypeEnum)
    {
        if ((int)allTypeEnum == (int)typeEnum)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    /*
    public static bool IsSortType(this ProductType.AllType allTypeEnum)
    {
        var enumInex = (int)allTypeEnum;
        if (Enum.IsDefined(typeof(ProductType.Type), enumInex))
        {
            return false;
        }
        else
        {
            return true;
        }
    }*/

    public static bool IsSortType(this Enum allTypeEnum)
    {
        var enumInex = Convert.ToInt32(allTypeEnum);

        if (allTypeEnum.GetType() == typeof(ProductType.AllType))
        {
            if (Enum.IsDefined(typeof(ProductType.Type), enumInex))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        throw new NotImplementedException();
    }
    /*
    public static ProductType.Type GetDerivedType(this ProductType.AllType allTypeEnum)
    {
        var enumIndex = (int)allTypeEnum;
        return (ProductType.Type)enumIndex;
    }*/

    public static ProductType.Type GetDerivedType(this Enum allTypeEnum)
    {
        var enumIndex = Convert.ToInt32(allTypeEnum);
        if (allTypeEnum.GetType() == typeof(ProductType.AllType))
        {
            return (ProductType.Type)enumIndex;
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    /*
    public static Sort.Type GetDerivedSortType(this ProductType.AllType allTypeEnum)
    {
        var enumIndex = (int)allTypeEnum;
        return (Sort.Type)enumIndex;
    }*/
    public static Sort.Type GetDerivedSortType(this Enum allTypeEnum)
    {
        var enumIndex = Convert.ToInt32(allTypeEnum);
        return (Sort.Type)enumIndex;
    }
    /*
    public static Enum GetBelongedMainType(this Enum typeEnum, Type mainType_Enum = null)
    {
        Console.WriteLine(typeEnum +""+ mainType_Enum);
        var enumIndex = Convert.ToInt32(typeEnum);    //(int)typeEnum;

        if(mainType_Enum.GetType() == typeof(EquipmentType.Type))   
        {
            foreach (int mainEnum_Index in Enum.GetValues(typeof(EquipmentType.Type)))
            {
                if (enumIndex / 100 == mainEnum_Index / 100)
                {
                    return (EquipmentType.Type)mainEnum_Index;
                }
            }         
        }
        else if(mainType_Enum.GetType() == typeof(GameItemType.Type))
        {
            switch (Enum.GetValues(typeof(GameItemType.Type)))
            {

            }
            
            //return mainType_Enum;
            //if(mainType_Enum == gam)
            //if (typeEnum.GetType() == typeof(ProductType.AllType))
            //{
            //    return GameItemType.Type.Product;
            //}
            //else if (typeEnum.GetType() == typeof(ExtraComponentsType.AllType))
            //{
            //    return GameItemType.Type.ExtraComponents;
            //}
            //else if(typeEnum.GetType() == typeof(SpecialItemType.AllType))
            //{
            //    return GameItemType.Type.SpecialItem;
            //}
            //else
            //{
                
            //    throw new NotImplementedException();
            //}
        }

        throw new NotImplementedException();

        /*
        foreach (int mainEnum_Index in Enum.GetValues(typeof(EquipmentType.Type)))
        {
            if (enumIndex / 100 == mainEnum_Index / 100)
            {
                return (EquipmentType.Type)mainEnum_Index;
            }
        }
        throw new NotImplementedException();
  
    }   */




    public static List<Enum> GetAllSubTypes(this Enum allTypeEnum)
    {
        if (allTypeEnum.GetType() == typeof(EquipmentType.Type))  // LATER TO MAKE BELOW WITH GETENUM FUNCTION !!!
        {
            var enumIndex = Convert.ToInt32(allTypeEnum);  //  (int)(object)allTypeEnum;
            List<Enum> allSubTypes = new List<Enum>();

            foreach (int subEnum_Index in Enum.GetValues(typeof(ProductType.AllType)))
            {
                if (enumIndex / 100 == subEnum_Index / 100)
                {
                    allSubTypes.Add((ProductType.AllType)subEnum_Index);
                }
            }

            return allSubTypes;
        }
        else if (allTypeEnum.GetType() == typeof(GameItemType.Type))
        {
            return GetAllSubTypes_Sort((GameItemType.Type)allTypeEnum);
        }
        else if (allTypeEnum.GetType() == typeof(ShopUpgradeType.Type))
        {
            return GetEnum<ShopUpgradeType.AllType>(0);
        }
        else if(allTypeEnum.GetType() == typeof(CharacterType.Type))
        {
            return GetEnum<CharacterType.AllType>(0);
        }
        throw new NotImplementedException();         // LATER TO MAKE FOR ALL TYPES OF ENUMS 
    }


    public static List<Enum> GetAllSubTypes_Sort(GameItemType.Type allTypes)
    {
        switch (allTypes)
        {
            case GameItemType.Type.Product:
                return GetEnum<ProductType.AllType>(0);

            case GameItemType.Type.ExtraComponents:
                return GetEnum<ExtraComponentsType.AllType>(0);

            case GameItemType.Type.SpecialItem:
                return GetEnum<SpecialItemType.AllType>(0);

            case GameItemType.Type.Enhancement:
                return GetEnum<EnhancementType.AllType>(0);

        }

        throw new NotImplementedException();
    }

    public static List<Enum> GetEnum<T>(int index)
        where T : System.Enum
    {
        List<Enum> allSubTypes = new List<Enum>();
        foreach (int subEnum_Index in Enum.GetValues(typeof(T)))
        {
            if (index / 100 == subEnum_Index / 100)
            {
                allSubTypes.Add((T)(object)subEnum_Index);
            }

        }
        return allSubTypes;        // LATER TO MAKE FOR ALL TYPES OF ENUMS 
    }



    #endregion
}

public static class NativeHelper
{
    public static StringBuilder stringBuilder = new StringBuilder();

    public static Color White = Color.white;
    public static Color Blue = Color.blue;             // Later to take the static colors to the game manager !!!!
    public static Color Green = Color.green;             // Later to take the static colors to the game manager !!!!
    public static Color Red = Color.red;             // Later to take the static colors to the game manager !!!!
    public static Color Black = Color.black;             // Later to take the static colors to the game manager !!!!
    public static Color Magenta = Color.magenta;              // this colors are already Static so either make new colors or use Color class's static colros as Color.Red
    
    public static void Enqueue<T>(this ConcurrentQueue<T> @this, IEnumerable<T> enumerableToEnqueue)
    {
        foreach (var item in enumerableToEnqueue)
        {
            @this.Enqueue(item);
        }
    }
    
    public static int GetHighestInt(this List<int> list)
    {
        int temp = default(int);

        for (int i = 0; i < list.Count; i++)
        {
            for (int j = 0; j < list.Count; j++)
            {
                if (list[i] > list[j])
                {
                    temp = list[j];
                    list[j] = list[i];
                    list[i] = temp;
                }
            }
        }
        return list[0];
    }

    public static void SetAsModifiableSpec(this TextMeshProUGUI textField, string string_IN, bool isModified, bool isAmountEnough = true)
    {
        if (!isAmountEnough)
        {
            if (textField.color != Color.red) textField.color = Color.red;
        }
        else if (isModified == true && textField.color != Green)
        {
            textField.color = Green;
        }
        else if (isModified == false && textField.color != White)
        {
            textField.color = White;
        }
        textField.text = string_IN;
    }

    public static void SetAsModifiableSpec_Comparaison(this TextMeshProUGUI textField, int valueInitial, int valueToCompare)
    {
        if (valueInitial >= valueToCompare && textField.color != Green)
        {
            textField.color = Green;
        }
        else if (valueInitial < valueToCompare && textField.color != Red)
        {
            textField.color = Red;
        }
        textField.text = string.Concat(valueInitial.ToString(), "/", valueToCompare.ToString());
    }

    public static void SetTextColor_CapReached(this TextMeshProUGUI textField, bool isCapReached)
    {
        if (isCapReached && textField.color != Color.green)
        {
            textField.color = Color.green;
        }
        else if (!isCapReached && textField.color != Color.white)
        {
            textField.color = Color.white;
        }
    }


    public static Color GetQualityColor(this Quality.Level qualityLevel)
    {
        switch (qualityLevel)
        {
            case Quality.Level.Normal:
                return White;

            case Quality.Level.Superior:
                return Blue;

            case Quality.Level.Flawless:
                return Green;

            case Quality.Level.Epic:
                return Red;

            case Quality.Level.Legendary:
                return Black;

            case Quality.Level.Mythic:
                return Magenta;
            default:
                return White;
        }
    }

    public static Color GetSuccessRatioColor(int enhancementSuccessRatio_IN)
    => enhancementSuccessRatio_IN switch
    {
        100 => Color.green,
        >= 25 and < 100 => Color.yellow,
        >= 10 and < 25 => Color.magenta,
        _ => Color.red,
    };

    public static Dictionary<T_Type, List<T_Item>> InitEmptyDict<T_Type, T_Item>(this Dictionary<T_Type, List<T_Item>> emptyDict)
        where T_Type : System.Enum
        where T_Item : SortableBluePrint
    {
        Dictionary<T_Type, List<T_Item>> newDict = new Dictionary<T_Type, List<T_Item>>();

        foreach (int enumIndex in Enum.GetValues(typeof(T_Type)))
        {
            if (enumIndex == 0)
            {
                continue;
            }
            else
            {
                newDict.Add((T_Type)(object)enumIndex, new List<T_Item>());
            }
        }

        return newDict;
    }

    public static string GetAscensionTreeRewardValue(this AscensionTree_SO.AscensionTreeReward ascensionTreeReward)
        => ascensionTreeReward.ascensionTreeRewardType switch
        {
            AscensionTree_SO.AscensionTreeRewardType.GoldReward => $"+ {ISpendable.ToScreenFormat(ascensionTreeReward.goldRewards)}", //MethodHelper.GetValueStringPlus(ascensionTreeReward.goldRewards), //$"+ {ascensionTreeReward.goldRewards}",
            AscensionTree_SO.AscensionTreeRewardType.GemReward => $"+ {ISpendable.ToScreenFormat(ascensionTreeReward.goldRewards)}", //MethodHelper.GetValueStringPlus(ascensionTreeReward.gemRewards), //$"+ {ascensionTreeReward.gemRewards}",
            AscensionTree_SO.AscensionTreeRewardType.SurchargeValueIncreasemodifier =>MethodHelper.GetValueStringPercent(ascensionTreeReward.surchargeValueIncreaseModifier.surchargeValueIncreaseModifier), // $"% {}",
            AscensionTree_SO.AscensionTreeRewardType.WorkerXPIncreaseModifier => MethodHelper.GetValueStringPercent(ascensionTreeReward.workerXPIncreasemodifier), //  $"X {ascensionTreeReward.workerXPIncreasemodifier}",
            AscensionTree_SO.AscensionTreeRewardType.ReduceSurchargeEnergyModifier => MethodHelper.GetValueStringPercent(ascensionTreeReward.reduceSurchargeEnergyModifier), //  $"X {ascensionTreeReward.reduceSurchargeEnergyModifier}",
            AscensionTree_SO.AscensionTreeRewardType.MultiCraftChance => MethodHelper.GetValueStringCross(ascensionTreeReward.multicraftChanceModifier), // $"+ {ascensionTreeReward.multicraftChanceModifier}",
            AscensionTree_SO.AscensionTreeRewardType.QualityChanceIncrease => MethodHelper.GetValueStringCross(ascensionTreeReward.qualityChanceIncreaseModifier), // $"X {ascensionTreeReward.qualityChanceIncreaseModifier}",
            AscensionTree_SO.AscensionTreeRewardType.IngredientsReduction =>MethodHelper.GetValueStringPercent(ascensionTreeReward.ingredientsReduction.ingredientReduction[0].reductionAmountPercent),// $"- {ascensionTreeReward.ingredientsReduction.ingredientReduction[0].reductionAmountPercent}",
            AscensionTree_SO.AscensionTreeRewardType.CommanderBadge => MethodHelper.GetValueStringCross(ascensionTreeReward.commanderBadges.amount),
            _ => throw new NotImplementedException(),
        };

    private static string[] descriptions = new string[]
    {
        "Increases Worker XP!!",
        "Reduce Surcharge Energy!",
        "More chance to MultiCraft!",
        "More Quality Items!",
        "Economy Of Ingredients!",
        "Increased surcharge value for ",
    };

    public static string GetAscensionTreeRewardDescription(this AscensionTree_SO.AscensionTreeReward ascensionTreeReward)
         => ascensionTreeReward.ascensionTreeRewardType switch
         {
             AscensionTree_SO.AscensionTreeRewardType.WorkerXPIncreaseModifier => descriptions[0],
             AscensionTree_SO.AscensionTreeRewardType.ReduceSurchargeEnergyModifier => descriptions[1],
             AscensionTree_SO.AscensionTreeRewardType.MultiCraftChance => descriptions[2],
             AscensionTree_SO.AscensionTreeRewardType.QualityChanceIncrease => descriptions[3],
             AscensionTree_SO.AscensionTreeRewardType.IngredientsReduction => descriptions[4],
             AscensionTree_SO.AscensionTreeRewardType.SurchargeValueIncreasemodifier => ascensionTreeReward.surchargeValueIncreaseModifier.productTypes.Length switch
             {
                 1 => $"{descriptions[5]}{ascensionTreeReward.surchargeValueIncreaseModifier.productTypes[0]}",
                 > 1 => $"{descriptions[5]}all types",
                 _ => throw new NotImplementedException()
             },
             _ => throw new System.NotImplementedException(ascensionTreeReward.ascensionTreeRewardType.ToString()),
         };

    public static IEnumerable<AssetReferenceT<Sprite>> GetAscensionTreeRewardSpriteRefs(this AscensionTree_SO.AscensionTreeReward ascensionTreeReward)
    {
        var stringKey = ascensionTreeReward.ascensionTreeRewardType.ToString();
        switch (ascensionTreeReward.ascensionTreeRewardType)
        {
            case AscensionTree_SO.AscensionTreeRewardType.GoldReward:
            case AscensionTree_SO.AscensionTreeRewardType.SurchargeValueIncreasemodifier:
            case AscensionTree_SO.AscensionTreeRewardType.GemReward:
            case AscensionTree_SO.AscensionTreeRewardType.WorkerXPIncreaseModifier:
            case AscensionTree_SO.AscensionTreeRewardType.ReduceSurchargeEnergyModifier:
            case AscensionTree_SO.AscensionTreeRewardType.MultiCraftChance:
            case AscensionTree_SO.AscensionTreeRewardType.QualityChanceIncrease:
                yield return ImageManager.SelectSprite(stringKey);
                break;
            case AscensionTree_SO.AscensionTreeRewardType.IngredientsReduction:
                foreach (var ingredient in ascensionTreeReward.ingredientsReduction.ingredientReduction)
                {
                    var ingredientType = ingredient.ingredient;
                    yield return ResourcesManager.Instance.Resources_SO.ingredients[(int)ingredientType].spriteRef;
                }
                break;
            case AscensionTree_SO.AscensionTreeRewardType.CommanderBadge:
                yield return ImageManager.SelectSprite(stringKey); // this has to be specialised according to the commander type later when making commander class !! 
                break;
        }
    }


    public static void AddNewOrAugmentExisting<T_Type, T_Item>(this Dictionary<T_Type, List<T_Item>> existingDict, T_Type itemType, T_Item incomingItem, int amount = 1)
        where T_Type : System.Enum
        where T_Item : SortableBluePrint, ICollectible
    {
        foreach (T_Item existingItem in existingDict[itemType])
        {
            DebuggerForNonMono.Logger("checking existing item :" + "" + existingItem.GetName());
            DebuggerForNonMono.Logger("checking NEW item :" + "" + incomingItem.GetName());
            if (existingItem.IsSameGameItemWith(incomingItem))
            {
                DebuggerForNonMono.Logger("added to already existing item" + existingItem.GetName() + "" + incomingItem.GetName());
                existingItem.SetAmount(amount);

                return;
            }
            else
            {
                continue;
            }
        }
        DebuggerForNonMono.Logger("created new Ýtem" + "" + incomingItem.GetName());
        existingDict[itemType].Add(incomingItem);
        incomingItem.SetAmount(amount);
    }


    public static string BuildString_Append(params string[] strings_IN)
    {
        if (stringBuilder.Length > 0)
        {
            stringBuilder.Clear();
        }

        for (int i = 0; i < strings_IN.Length; i++)
        {
            stringBuilder.Append(strings_IN[i]);
        }

        return stringBuilder.ToString();
    }

    public static string BuildString_Insert(string mainSentence, string stringToInsert, int insertionPoint)
    {
        if (stringBuilder.Length > 0)
        {
            stringBuilder.Clear();
        }
        stringBuilder.Append(mainSentence);
        stringBuilder.Insert(insertionPoint, stringToInsert);

        return stringBuilder.ToString();
    }

    public static bool IsNullOrWhiteSpaceOrEmpty(this string @string) => string.IsNullOrEmpty(@string) || string.IsNullOrWhiteSpace(@string);
}

public static class MethodHelper
{
    public static bool IsSameGameItemWith<T_Item>(this T_Item existingGameItem_IN, T_Item incomingGameItem_IN)
        where T_Item : SortableBluePrint
    {
        if (incomingGameItem_IN is IQualitative qualitativeIncomingItem && existingGameItem_IN is IQualitative qualitativeExistingItem)
        {
            if (qualitativeExistingItem.GetQuality() == qualitativeIncomingItem.GetQuality() && existingGameItem_IN.GetName() == incomingGameItem_IN.GetName())
            {

                if (qualitativeExistingItem is IEnhanceable enheanceableExistingItem && qualitativeIncomingItem is IEnhanceable enheanceableIncomingItem)
                {
                    DebuggerForNonMono.Logger("Iterating as requested");
                    foreach (var existingEnhancementSlot in enheanceableExistingItem.enhancementsDict_ro)
                    {
                        DebuggerForNonMono.Logger(existingEnhancementSlot.ToString());
                        foreach (var incomingEnhancementSlot in enheanceableIncomingItem.enhancementsDict_ro)
                        {
                            DebuggerForNonMono.Logger(existingEnhancementSlot.ToString() + " " + incomingEnhancementSlot.ToString());
                            if (existingEnhancementSlot.Value != incomingEnhancementSlot.Value)
                            {
                                return false;
                            }
                        }
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (existingGameItem_IN.GetName() == incomingGameItem_IN.GetName())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }



    public static bool AreDictionariesEqual<T_Key, T_Value>(IDictionary<T_Key, T_Value> dict_X, IDictionary<T_Key, T_Value> dict_Y)
    where T_Key : System.Enum
    {
        if (dict_X == dict_Y)
        {
            return true;
        }
        if (dict_X == null || dict_Y == null)
        {
            return false;
        }

        bool result = false;
        result = dict_X.Count == dict_Y.Count;
        if (result)
        {
            foreach (KeyValuePair<T_Key, T_Value> pair in dict_X)
            {
                T_Value value_Y;
                if (!dict_Y.TryGetValue(pair.Key, out value_Y))
                {
                    result = false;
                    break;
                }
                else
                {
                    result = pair.Value.Equals(value_Y);
                    if (!result)
                    {
                        break;
                    }
                }
            }
        }
        return result;
    }

    public static void SortByDateDescending<T_BluePrint>(this List<T_BluePrint> datableBluePrints_IN)
        where T_BluePrint : SortableBluePrint
    {
        var immutableList = datableBluePrints_IN as IReadOnlyList<IDatable>;

        if (immutableList == null)
        {
            for (int i = 0; i < datableBluePrints_IN.Count; i++)
            {
                for (int j = 0; j < datableBluePrints_IN.Count; j++)
                {
                    if ((datableBluePrints_IN[i] as IDatable).GetLastCraftedDate() > (datableBluePrints_IN[j] as IDatable).GetLastCraftedDate())
                    {
                        var temp = datableBluePrints_IN[j];
                        datableBluePrints_IN[j] = datableBluePrints_IN[i];
                        datableBluePrints_IN[i] = temp;
                    }
                }
            }
        }

        else
        {
            for (int i = 0; i < datableBluePrints_IN.Count; i++)
            {
                for (int j = 0; j < datableBluePrints_IN.Count; j++)
                {
                    if (immutableList[i].GetLastCraftedDate() > immutableList[j].GetLastCraftedDate())
                    {
                        var temp = datableBluePrints_IN[j];
                        datableBluePrints_IN[j] = datableBluePrints_IN[i];
                        datableBluePrints_IN[i] = temp;
                    }
                }
            }
        }
    }

    public static void SortByValue<T_BluePrint>(this List<T_BluePrint> valuabeItemslist_IN)
        where T_BluePrint : SortableBluePrint
    {
        var immutableList = valuabeItemslist_IN as IReadOnlyList<IValuable>;

        if (immutableList == null)
        {
            for (int i = 0; i < valuabeItemslist_IN.Count; i++)
            {
                for (int j = 0; j < valuabeItemslist_IN.Count; j++)
                {
                    if ((valuabeItemslist_IN[i] as IValuable).GetValue() > (valuabeItemslist_IN[j] as IValuable).GetValue())
                    {
                        var temp = valuabeItemslist_IN[j];
                        valuabeItemslist_IN[j] = valuabeItemslist_IN[i];
                        valuabeItemslist_IN[i] = temp;
                    }
                }
            }
        }

        else
        {
            for (int i = 0; i < valuabeItemslist_IN.Count; i++)
            {
                for (int j = 0; j < valuabeItemslist_IN.Count; j++)
                {
                    if (immutableList[i].GetValue() > immutableList[j].GetValue())
                    {
                        var temp = valuabeItemslist_IN[j];
                        valuabeItemslist_IN[j] = valuabeItemslist_IN[i];
                        valuabeItemslist_IN[i] = temp;
                    }
                }
            }
        }

    }

    public static void SortByLevel<T_BluePrint>(this List<T_BluePrint> rankableItemsList_IN)
        where T_BluePrint : SortableBluePrint
    {
        var immutableList = rankableItemsList_IN as IReadOnlyList<IRankable>;

        if (immutableList == null)
        {
            for (int i = 0; i < rankableItemsList_IN.Count; i++)
            {
                for (int j = 0; j < rankableItemsList_IN.Count; j++)
                {
                    if ((rankableItemsList_IN[i] as IRankable).GetLevel() > (rankableItemsList_IN[j] as IRankable).GetLevel())
                    {
                        var temp = rankableItemsList_IN[j];
                        rankableItemsList_IN[j] = rankableItemsList_IN[i];
                        rankableItemsList_IN[i] = temp;
                    }
                }
            }
        }

        else
        {
            for (int i = 0; i < rankableItemsList_IN.Count; i++)
            {
                for (int j = 0; j < rankableItemsList_IN.Count; j++)
                {
                    if (immutableList[i].GetLevel() > immutableList[j].GetLevel())
                    {
                        var temp = rankableItemsList_IN[j];
                        rankableItemsList_IN[j] = rankableItemsList_IN[i];
                        rankableItemsList_IN[i] = temp;
                    }
                }
            }
        }
    }


    public static void SortByQuantitiy<T_BluePrint>(this List<T_BluePrint> amountableItemsList_IN)
    where T_BluePrint : SortableBluePrint
    {
        var immutableList = amountableItemsList_IN as IReadOnlyList<IAmountable>;

        if (immutableList == null)
        {
            for (int i = 0; i < amountableItemsList_IN.Count; i++)
            {
                for (int j = 0; j < amountableItemsList_IN.Count; j++)
                {
                    if ((amountableItemsList_IN[i] as IAmountable).GetAmount() > (amountableItemsList_IN[j] as IAmountable).GetAmount())
                    {
                        var temp = amountableItemsList_IN[j];
                        amountableItemsList_IN[j] = amountableItemsList_IN[i];
                        amountableItemsList_IN[i] = temp;
                    }
                }
            }
        }

        else
        {
            for (int i = 0; i < amountableItemsList_IN.Count; i++)
            {
                for (int j = 0; j < amountableItemsList_IN.Count; j++)
                {
                    if (immutableList[i].GetAmount() > immutableList[j].GetAmount())
                    {
                        var temp = amountableItemsList_IN[j];
                        amountableItemsList_IN[j] = amountableItemsList_IN[i];
                        amountableItemsList_IN[i] = temp;
                    }
                }
            }
        }

    }

    public static void SortByQuality<T_BluePrint>(this List<T_BluePrint> qualititiveItemsList)
        where T_BluePrint : SortableBluePrint
    {
        var immutableList = qualititiveItemsList as IReadOnlyList<IQualitative>;
        if (immutableList == null)
        {
            for (int i = 0; i < qualititiveItemsList.Count; i++)
            {
                for (int j = 0; j < qualititiveItemsList.Count; j++)
                {
                    if ((qualititiveItemsList[i] as IQualitative).GetQuality() > (qualititiveItemsList[j] as IQualitative).GetQuality())
                    {
                        var temp = qualititiveItemsList[j];
                        qualititiveItemsList[j] = qualititiveItemsList[i];
                        qualititiveItemsList[i] = temp;
                    }
                }
            }
        }

        else
        {
            for (int i = 0; i < qualititiveItemsList.Count; i++)
            {
                for (int j = 0; j < qualititiveItemsList.Count; j++)
                {
                    if (immutableList[i].GetQuality() > immutableList[j].GetQuality())
                    {
                        var temp = qualititiveItemsList[j];
                        qualititiveItemsList[j] = qualititiveItemsList[i];
                        qualititiveItemsList[i] = temp;
                    }
                }
            }
        }
    }

    public static void SortByName<T_BluePrint>(this List<T_BluePrint> namedItemsList_IN)
        where T_BluePrint : SortableBluePrint
    {
        namedItemsList_IN.Sort((x, y) => x.GetName().CompareTo(y.GetName()));
    }


    public static string GiveRichTextString_Color(Color color_IN)
    {
        return NativeHelper.BuildString_Append("<color=#", ColorUtility.ToHtmlStringRGBA(color_IN), ">");
    }

    public static string GiveRichTextString_Size(int sizePercenage)
    {
        return NativeHelper.BuildString_Append("<size=", sizePercenage.ToString(), "%>");
    }

    public static string GiveRichTextString_ClosingTagOf(string requiredTagType)
    {

        switch (requiredTagType.ToLower())
        {
            case "color": return NativeHelper.BuildString_Append("</", "color", ">");
            case "size": return NativeHelper.BuildString_Append("</", "size", ">");
            default:
                throw new Exception();
        }

    }

    public static string GetNameOfTheCraftingUpgradeType(this Recipes_SO.CraftUpgradeType craftingUpgrade_IN)
        => GetNameOfUpgradeType(craftingUpgrade_IN.ToString());
    public static string GetNameOfTheAscensionUpgradeType(this Recipes_SO.AscensionUpgradeType ascensionUpgrade_IN)
    => GetNameOfUpgradeType(ascensionUpgrade_IN.ToString());

    public static string AppendCraftingUpgradeBonusToSTring(this Recipes_SO.CraftUpgradeType craftingUpgrade_IN, float value_IN)
        => AppendUpgradeBonusToString(craftingUpgrade_IN.ToString(), value_IN);


    public static string AppendAscensionUpgradeBonusToSTring(this Recipes_SO.AscensionUpgradeType ascensionUpgrade_IN, float value_IN)
        => AppendUpgradeBonusToString(ascensionUpgrade_IN.ToString(), value_IN);


    private static string GetNameOfUpgradeType(string upgradeName_IN)
        => upgradeName_IN switch
        {
            nameof(Recipes_SO.CraftUpgradeType.CraftTimeReduction)
            or nameof(Recipes_SO.AscensionUpgradeType.CraftTimeReduction) => "Cook Faster",

            nameof(Recipes_SO.CraftUpgradeType.IngredientReduction)
            or nameof(Recipes_SO.AscensionUpgradeType.IngredientReduction) => "Ingredient Economy",

            nameof(Recipes_SO.CraftUpgradeType.ExtraComponentReduction)
            or nameof(Recipes_SO.AscensionUpgradeType.ExtraComponentReduction) => "Component Economy",

            nameof(Recipes_SO.CraftUpgradeType.ValueIncrease) => "Value Increase",

            nameof(Recipes_SO.CraftUpgradeType.QualityChanceIncrease)
            or nameof(Recipes_SO.AscensionUpgradeType.QualityChanceIncrease) => "Quality Chance",

            nameof(Recipes_SO.CraftUpgradeType.UnlockRecipe) => "Unlock Recipe",

            nameof(Recipes_SO.AscensionUpgradeType.MultiCraftChance) => "Multicraft Chance",

            nameof(Recipes_SO.AscensionUpgradeType.RequiredProductReduction) => "Product Economy",

            _ => throw new NotImplementedException(),
        };


    private static string AppendUpgradeBonusToString(string upgradeTypeName_IN, float value_IN)
        => upgradeTypeName_IN switch
        {
            nameof(Recipes_SO.CraftUpgradeType.CraftTimeReduction)
            or nameof(Recipes_SO.AscensionUpgradeType.CraftTimeReduction) => GetValueStringPercent(value_IN),

            nameof(Recipes_SO.CraftUpgradeType.IngredientReduction)
            or nameof(Recipes_SO.AscensionUpgradeType.IngredientReduction) => GetValueStringMinus(value_IN),

            nameof(Recipes_SO.CraftUpgradeType.ExtraComponentReduction)
            or nameof(Recipes_SO.AscensionUpgradeType.ExtraComponentReduction) => GetValueStringMinus(value_IN),

            nameof(Recipes_SO.CraftUpgradeType.ValueIncrease) => GetValueStringCross(value_IN),

            nameof(Recipes_SO.CraftUpgradeType.QualityChanceIncrease)
            or nameof(Recipes_SO.AscensionUpgradeType.QualityChanceIncrease) => GetValueStringCross(value_IN),

            nameof(Recipes_SO.CraftUpgradeType.UnlockRecipe) => string.Empty,

            nameof(Recipes_SO.AscensionUpgradeType.MultiCraftChance) => GetValueStringCross(value_IN),

            nameof(Recipes_SO.AscensionUpgradeType.RequiredProductReduction) => GetValueStringMinus(value_IN),

            _ => throw new NotImplementedException(),
        };


    public static string GetValueStringPercent(float value_IN, bool inverseValue = true) => inverseValue ? $"{Mathf.CeilToInt(Mathf.Abs(1 - value_IN) * 100)}%" : $"{Mathf.CeilToInt(value_IN * 100)}%";
    public static string GetValueStringCross(float value_IN) => $"x {value_IN}";
    public static string GetValueStringMinus(float value_IN) => $"- {value_IN}";
    /*public static string GetValueStringPlus(float value_IN) =>
       value_IN switch
       {
           < -999 and >= -999999 or > 999 and <= 999999 => $"+ {value_IN.ToString("0,.#K", CultureInfo.InvariantCulture)}",
           < -999999 or > 999999 => $"+ {value_IN.ToString("0,,.##M", CultureInfo.InvariantCulture)}",
           _ => $"+ {value_IN}"
       };*/

    public static string GetNameOfWorkerType(WorkerType.Type workerType)
        => workerType switch
        {
            WorkerType.Type.Krixath_The_Rotisseur => "The Rotisseur",
            WorkerType.Type.Trilqeox_The_Entremetier => "Entremetier",
            WorkerType.Type.Qindrek_The_Poissonier => "Poissonier",
            WorkerType.Type.Chophu_The_Patissier => "Patisser",
            WorkerType.Type.Xaden_The_Fast_Fooder => "Fast Fooder",
            WorkerType.Type.Trugmil_The_Legumier => "Legumier",
            WorkerType.Type.Ekol_The_Potager => "Potager",
            WorkerType.Type.Master_Chef => "Master Chef",
            WorkerType.Type.None => throw new System.NotImplementedException(),
            _ => throw new System.NotImplementedException()
        };
    public static string GetNameOfShopUpgradeType(ShopUpgradeType.Type shopUpgrdeType)
        => shopUpgrdeType switch
        {
            ShopUpgradeType.Type.SalesCabinetUpgrade => "Sales Cabinet",
            ShopUpgradeType.Type.ResourceCabinetUpgrade => "Resource Cabinet",
            ShopUpgradeType.Type.WorkstationUpgrades => "Worksation",
            _ => throw new NotImplementedException(),
        };

    public static string GetNameOfWorkStationType(WorkstationType.Type workStationType)
        => workStationType switch
        {
            WorkstationType.Type.Solar_Grill_Station => "Solar Grill",
            WorkstationType.Type.HandCranked_Pasta_Machine_Station => "HC Pasta Machine",
            WorkstationType.Type.Laser_Precision_Carving_Knife_Station => "LP Carving Knife",
            WorkstationType.Type.Convection_Oven_Station => "Convection Oven",
            WorkstationType.Type.Griddle_Oven_Sixburner_Station => "Griddle Oven 6-Burner",
            WorkstationType.Type.Plasmatic_Deepfry_Station => "Plasmatic Deepfry",
            WorkstationType.Type.Higgs_Pot_Station => "Higgs Pot",
            WorkstationType.Type.MasterChef_Station => "Master Chef's Station",
            _ => throw new NotImplementedException(),
        };
}
public static class FunctionalHelpers
{
    public static IEnumerable<(T_OutPut1, T_OutPut2)> ExtractEnumerableOfTuples<T_OutPut1, T_OutPut2, T>(this IEnumerable<T> @this,
                                                                                               Func<T, T_OutPut1> extractor1,
                                                                                               Func<T, T_OutPut2> extractor2)
    {
        (T_OutPut1, T_OutPut2) extractedTupleToReturn;
        foreach (T element in @this)
        {
            extractedTupleToReturn.Item1 = extractor1(element);
            extractedTupleToReturn.Item2 = extractor2(element);

            yield return extractedTupleToReturn;
        }
    }

    public static IEnumerable<(T_OutPut1, T_OutPut2)> ConvertEnumerableOfTuples<T_Input1, T_Input2, T_OutPut1, T_OutPut2>(this IEnumerable<(T_Input1, T_Input2)> @this,
                                                                                                                           Func<T_Input1, T_OutPut1> converter1,
                                                                                                                           Func<T_Input2, T_OutPut2> converter2)
    {
        (T_OutPut1, T_OutPut2) convertedTupleToReturn;
        foreach (var tuple in @this)
        {
            convertedTupleToReturn.Item1 = converter1(tuple.Item1);
            convertedTupleToReturn.Item2 = converter2(tuple.Item2);

            yield return convertedTupleToReturn;
        }
    }
    public static IEnumerable<(T_OutPut1, T_OutPut2)> CreateEnumerableOfTuple<T_OutPut1, T_OutPut2>(int iterationCount,
                                                                                                        Func<int, T_OutPut1> factor1,
                                                                                                        Func<T_OutPut2> factor2)
    {
        (T_OutPut1, T_OutPut2) convertedTupleToReturn;
        for (int i = 0; i < iterationCount; i++)
        {
            convertedTupleToReturn.Item1 = factor1(i);
            convertedTupleToReturn.Item2 = factor2();

            yield return convertedTupleToReturn;
        }
    }



    public static IEnumerable<T_Output> ConvertEnumerable<T_Input, T_Output>(this IEnumerable<T_Input> @this,
                                                                             Func<T_Input, T_Output> converter)
    {
        foreach (var item in @this)
        {
            yield return converter(item);
        }
    }

    public static IEnumerable<T_Input> CreateEnumerableFrom<T_Input>(this T_Input @this, int iterationCount)
    {
        for (int i = 0; i < iterationCount; i++)
        {
            yield return @this;
        }
    }
    public static T_Output ConditionalyCreateFrom<T_Input, T_Output>(this T_Input @this,
                                                bool condition,
                                                Func<T_Input, T_Output> ifTrueFunc,
                                                Func<T_Input, T_Output> ifFalseFunc)
    {
        return condition
            ? ifTrueFunc(@this)
            : ifFalseFunc(@this);

    }

    //public static void ConditionalForEAch<T_Input1, T_Input2>(this IEnumerable<T_Input> sequence, 
    //                                                      Func<T_Input,bool> predicate,
    //                                                      Action<T_Input2> ifTrueFunc,
    //                                                      Action<T_Input2> ifFalseFunc)
    //{
    //    foreach (var item in sequence)
    //    {
    //        if (predicate(item)) ifTrueFunc();
    //        else ifFalseFunc();
    //    }
    //}
    public static IEnumerable<(T_Input1, T_Input2)> CreateEnumerableFromSequences<T_Input1, T_Input2>(IEnumerable<T_Input1> sequence1,
                                                                                                      IEnumerable<T_Input2> sequence2)
    {
        int iterationNo = 0;
        foreach (var seq1Item in sequence1)
        {
            yield return (seq1Item, sequence2.ElementAt(iterationNo));
            iterationNo++;
        }

    }

    public static IEnumerable<(T_Input1, T_Input2, T_Input3)> CreateEnumerableFromSequences<T_Input1, T_Input2, T_Input3>(IEnumerable<T_Input1> sequence1,
                                                                                                                          IEnumerable<T_Input2> sequence2,
                                                                                                                          IEnumerable<T_Input3> sequence3)
    {
        int iterationNo = 0;
        foreach (var seq1Item in sequence1)
        {
            yield return (seq1Item, sequence2.ElementAt(iterationNo), sequence3.ElementAt(iterationNo));
            iterationNo++;
        }
    }

    public static IEnumerable<(T_Input1, T_Input2)> AddToEnumerableOfTuples<T_Input1,T_Input2> (this IEnumerable<(T_Input1, T_Input2)> @this,
                                                                                        Func<T_Input1> factor1,
                                                                                        Func<T_Input2> factor2)
    {
        foreach (var item in @this)
        {
            yield return item;
        }
        yield return (factor1(), factor2());
    }

    public static IEnumerable<T_Input> AddToEnumerable<T_Input>(this IEnumerable<T_Input> @this,
                                                                                 Func<T_Input> factor,
                                                                                 bool condition = true)
    {
        foreach (var item in @this)
        {
            yield return item;
        }

        if(condition) 
            yield return factor();
    }

    [Flags]
    public enum PositionFlags
    {
        Normal=0,
        First=1,
        Last=2,
    }
    public static IEnumerable<(T_Input, PositionFlags flag)> WithPositions<T_Input>(this IEnumerable<T_Input> @this)
    {
        using(var enumerator = @this.GetEnumerator())
        {
            if (!enumerator.MoveNext())
            {
                yield break;
            }

            T_Input value = enumerator.Current;
            PositionFlags flag = PositionFlags.First;

            while (enumerator.MoveNext())
            {
                yield return (value, flag);

                value = enumerator.Current;
                flag = PositionFlags.Normal;
            }

            flag |= PositionFlags.Last;
            yield return (value, flag);
        }
    }




    public static Action<T1, T2> Curry<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> @function, T3 arg3, T4 arg4)
        => (arg1, arg2) => @function(arg1, arg2, arg3, arg4);


    public static Action<T1, T2> Curry<T1, T2, T3>(this Action<T1, T2, T3> @function, T3 arg3)
        => (arg1, arg2) => @function(arg1, arg2, arg3);

    public static Action<T1> Curry<T1, T2>(this Action<T1, T2> @function, T2 arg2)
        => arg1 => @function(arg1, arg2);

    public static Action Curry<T1>(this Action<T1> @function, T1 arg1)
        => () => @function(arg1);

    public static Func<T1, T3> Curry<T1, T2, T3>(this Func<T1, T2, T3> @function, T2 arg2)
        where T3 : struct
        => arg1 => @function(arg1, arg2);





    public static Action<float, RectTransform, float> quaternionRotation = (interpolationFactor, rT, totalDelta) =>
    {
        var currentAngle = (totalDelta * interpolationFactor);
        rT.rotation = Quaternion.Euler(0, 0, currentAngle);
    };

    public static Action<RectTransform, bool, float, float> setValuesofRotation = (rt, isInitial, ang1, ang2) =>
    {
        rt.rotation = isInitial
                ? Quaternion.Euler(0, 0, ang1)
                : Quaternion.Euler(0, 0, ang2);
    };

    public static Action<RectTransform, Vector3> setValueOfRotation = (rt, rot)
        => rt.rotation = Quaternion.Euler(rot);

    public static Func<RectTransform, Vector3, bool> checkValueOFRotation = (rt, initialValue)
        => rt.rotation == Quaternion.Euler(initialValue);

}


public static class CRHelper
{
    public static IEnumerator MoveRoutine(RectTransform rt, Vector2 targetPos, float lerpDuration ,float lerpSpeedModifier = 1, Action followingAction = null)
    {
        float elapsedTime = 0f;
        Vector2 currentPos = rt.anchoredPosition;
        while (elapsedTime < lerpDuration * lerpSpeedModifier)
        {
            float easeFactor = elapsedTime / (lerpDuration * lerpSpeedModifier);
            easeFactor = TimeTickSystem.Instance.easeCurve.Evaluate(easeFactor);

            rt.anchoredPosition = Vector2.LerpUnclamped(currentPos, targetPos, easeFactor);
            elapsedTime += Time.deltaTime;

            yield return null;
        }
        rt.anchoredPosition = targetPos;
        followingAction?.Invoke();
    }

    public static IEnumerator MoveRoutine(IEnumerable<(RectTransform rts, Vector2 targetpos)> rtInfos, float lerpDuration, float lerpSpeedModifier=1, Action followingAction = null)
    {
        float elapsedTime = 0f;
        (RectTransform rt, Vector2 currentPos, Vector2 targetPos)[] itemsToMove = rtInfos.Select(rti => (rti.rts, rti.rts.anchoredPosition, rti.targetpos)).ToArray();
        while (elapsedTime < lerpDuration * lerpSpeedModifier)
        {
            float easeFactor = elapsedTime / (lerpDuration * lerpSpeedModifier);
            easeFactor = TimeTickSystem.Instance.easeCurve.Evaluate(easeFactor);

            for (int i = 0; i < itemsToMove.Length; i++)
            {
                itemsToMove[i].rt.anchoredPosition = Vector2.LerpUnclamped(itemsToMove[i].currentPos, itemsToMove[i].targetPos, easeFactor);
                elapsedTime += Time.deltaTime;
            }

            //rt.anchoredPosition = Vector2.LerpUnclamped(currentPos, targetPos, easeFactor);
            //elapsedTime += Time.deltaTime;

            yield return null;
        }

        for (int i = 0; i < itemsToMove.Length; i++)
        {
            itemsToMove[i].rt.anchoredPosition = itemsToMove[i].targetPos;
        }
        followingAction?.Invoke();
    }


    public enum MoveRoutineType
    {
        Position =0,
        Scale = 1,
        Rotation = 2,
    }

    [Flags]
    public enum CoordinateFlags
    {
        None = 0,
        X = 1 << 0,
        Y = 1 << 1,
        Z = 1 << 2,
    }
    public enum RoutineRecursionType
    {
        None,
        RevertsToOriginal,
        Continous,
        ContinousWithInversiton,
    }

    public static IEnumerator MoveRoutine3D(this Transform transform,
                                            Vector3 targetPos,
                                            float lerpDuration,      
                                            TimeTickSystem.EaseCurveType easeCurveType = TimeTickSystem.EaseCurveType.Standard,
                                            Quaternion? targetRotationQ = null,
                                            float lerpSpeedModifier = 1,
                                            params Action[] followingActions)
    {
        float elapsedTime = 0f;
        Vector3 currentPos = transform.position;
        Quaternion? currentRotationQ = targetRotationQ.HasValue ? transform.rotation : null;
        var easeCurve = TimeTickSystem.Instance.GetCurve(easeCurveType);

        while (elapsedTime < lerpDuration * lerpSpeedModifier)
        {
            float easeFactor = elapsedTime / (lerpDuration * lerpSpeedModifier);
            easeFactor = easeCurve.Evaluate(easeFactor);

            transform.position = Vector3.LerpUnclamped(currentPos, targetPos, easeFactor);
            if(currentRotationQ.HasValue)
            {
                transform.rotation = Quaternion.Lerp(currentRotationQ.Value, targetRotationQ.Value, easeFactor);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPos;

        foreach (var followingAction in followingActions)
        {
            followingAction?.Invoke();
        }
    }


    public static IEnumerator MoveRoutine1D(this Transform transform,
                                            Vector3 targetValue,
                                            float lerpDuration,
                                            MoveRoutineType moveRoutineType,
                                            CoordinateFlags coordinateFlags,
                                            TimeTickSystem.EaseCurveType easeCurveType = TimeTickSystem.EaseCurveType.Standard,
                                            float lerpSpeedModifier = 1,
                                            RoutineRecursionType routineRecursionType = RoutineRecursionType.None,
                                            params Action[] followingActions)
    {
        float elapsedTime = 0f;
        Vector3 originalValue = moveRoutineType == MoveRoutineType.Position 
                                                    ? transform.position 
                                                    : moveRoutineType == MoveRoutineType.Scale
                                                         ? transform.localScale
                                                         :transform.eulerAngles;
        
        var easeCurve = TimeTickSystem.Instance.GetCurve(easeCurveType);


        var iterationNo = 1;
        int inversionModifier = 1;

        while (routineRecursionType == RoutineRecursionType.Continous 
            || routineRecursionType == RoutineRecursionType.ContinousWithInversiton 
            || elapsedTime < lerpDuration * lerpSpeedModifier)
        {
            float easeFactor = routineRecursionType == RoutineRecursionType.Continous || routineRecursionType == RoutineRecursionType.ContinousWithInversiton
                                    ? Mathf.PingPong(elapsedTime / (lerpDuration * lerpSpeedModifier) , 1f)
                                    : routineRecursionType == RoutineRecursionType.RevertsToOriginal
                                        ? Mathf.PingPong(elapsedTime / (lerpDuration * lerpSpeedModifier) * 2  , 1f) 
                                        : elapsedTime / (lerpDuration * lerpSpeedModifier);

            easeFactor = easeCurve.Evaluate(easeFactor) * inversionModifier;

            switch (moveRoutineType)
            {
                case MoveRoutineType.Position:
                    transform.position = Vector3.LerpUnclamped(new Vector3(x: coordinateFlags.HasFlag(CoordinateFlags.X) ? originalValue.x : transform.position.x,
                                                                           y: coordinateFlags.HasFlag(CoordinateFlags.Y) ? originalValue.y : transform.position.y,
                                                                           z: coordinateFlags.HasFlag(CoordinateFlags.Z) ? originalValue.z : transform.position.z),

                                                               new Vector3(x: coordinateFlags.HasFlag(CoordinateFlags.X) ? targetValue.x : transform.position.x,
                                                                           y: coordinateFlags.HasFlag(CoordinateFlags.Y) ? targetValue.y : transform.position.y,
                                                                           z: coordinateFlags.HasFlag(CoordinateFlags.Z) ? targetValue.z : transform.position.z),
                                                               easeFactor);

                    break;
                case MoveRoutineType.Scale:
                    transform.localScale = Vector3.LerpUnclamped(new Vector3(x: coordinateFlags.HasFlag(CoordinateFlags.X) ? originalValue.x : transform.localScale.x,
                                                                             y: coordinateFlags.HasFlag(CoordinateFlags.Y) ? originalValue.y : transform.localScale.y,
                                                                             z: coordinateFlags.HasFlag(CoordinateFlags.Z) ? originalValue.z : transform.localScale.z),

                                                                 new Vector3(x: coordinateFlags.HasFlag(CoordinateFlags.X) ? targetValue.x : transform.localScale.x,
                                                                             y: coordinateFlags.HasFlag(CoordinateFlags.Y) ? targetValue.y : transform.localScale.y,
                                                                             z: coordinateFlags.HasFlag(CoordinateFlags.Z) ? targetValue.z : transform.localScale.z),
                                                                 easeFactor);
                    break;
                case MoveRoutineType.Rotation:
                    transform.rotation = Quaternion.LerpUnclamped(Quaternion.Euler(originalValue), Quaternion.Euler(targetValue) , easeFactor);
                    break;
            }
                
            elapsedTime += Time.deltaTime;

            if(routineRecursionType == RoutineRecursionType.ContinousWithInversiton 
                && elapsedTime >= lerpDuration * lerpSpeedModifier * iterationNo )
            {
                inversionModifier = iterationNo % 2 == 0 ? inversionModifier * -1 : inversionModifier;

                iterationNo++;                
            }

            yield return null;
        }


        switch (moveRoutineType)
        {
            case MoveRoutineType.Position when routineRecursionType != RoutineRecursionType.RevertsToOriginal:
                transform.position = new Vector3(x: coordinateFlags.HasFlag(CoordinateFlags.X) ? targetValue.x : transform.position.x,
                                                 y: coordinateFlags.HasFlag(CoordinateFlags.Y) ? targetValue.y : transform.position.y,
                                                 z: coordinateFlags.HasFlag(CoordinateFlags.Z) ? targetValue.z : transform.position.z);
                break;
            case MoveRoutineType.Position when routineRecursionType == RoutineRecursionType.RevertsToOriginal:
                transform.position = originalValue;

                break;
            case MoveRoutineType.Scale when routineRecursionType != RoutineRecursionType.RevertsToOriginal:
                transform.localScale = new Vector3(x: coordinateFlags.HasFlag(CoordinateFlags.X) ? targetValue.x : transform.localScale.x,
                                                   y: coordinateFlags.HasFlag(CoordinateFlags.Y) ? targetValue.y : transform.localScale.y,
                                                   z: coordinateFlags.HasFlag(CoordinateFlags.Z) ? targetValue.z : transform.localScale.z);
                break;
            case MoveRoutineType.Scale when routineRecursionType == RoutineRecursionType.RevertsToOriginal:
                transform.localScale = originalValue;
                break;

            case MoveRoutineType.Rotation:
                Debug.LogError("his feature shouldn't be necessary yet ");
                break;
        }

        //transform.position = new Vector3(transform.position.x, targetValue, transform.position.z); //targetPos;

        foreach (var followingAction in followingActions)
        {
            followingAction?.Invoke();
        }
    }

    public static IEnumerator RotateRoutine(Transform transform, Quaternion targetRotationQ, float lerpDuration, float lerpSpeedModifier = 1, params Action[] followingActions)
    {
        float elapsedTime = 0f;
        var currentRotationQ = transform.rotation;

        while (elapsedTime < lerpDuration * lerpSpeedModifier)
        {
            float easeFactor = elapsedTime / (lerpDuration * lerpSpeedModifier);
            easeFactor = TimeTickSystem.Instance.easeCurve.Evaluate(easeFactor);

            transform.rotation = Quaternion.Lerp(currentRotationQ, targetRotationQ, easeFactor);// Vector3.Lerp(currentRotation, targetRotation, easeFactor);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.rotation = targetRotationQ; //new Vector3(transform.position.x, targetPos, transform.position.z); //targetPos;

        foreach (var followingAction in followingActions)
        {
            followingAction?.Invoke();
        }
    }


}

public static class AsyncHelper 
{
    public static async Task<bool> WaitForEndOfFrameAsync()
    {
        var currenFrame = Time.renderedFrameCount;
        while(currenFrame >= Time.renderedFrameCount)
        {
            await Task.Yield();
        }
        return true;
    }

    public static async Task ResizeText(TMP_Text text, float sizeMultiplier, float lerpSpeedModifier, bool shouldRevertTooriginalSize)
    {
        float originalSize = text.fontSize;
        await ChangeSizeOfText(text, originalSize*sizeMultiplier, lerpSpeedModifier);

        if (shouldRevertTooriginalSize)
            await ChangeSizeOfText(text, originalSize, lerpSpeedModifier);
      
    }

    private static async Task ChangeSizeOfText(TMP_Text text, float finalSize, float lerpSpeedModifier)
    {
        float elapsedTime = 0;
        var initialSize = text.fontSize;  
        
        while (elapsedTime < 0.05f * lerpSpeedModifier)
        {
            text.fontSize = Mathf.Lerp(initialSize, finalSize, elapsedTime / (0.05f * lerpSpeedModifier));
            elapsedTime += Time.deltaTime;

            await WaitForEndOfFrameAsync();
        }
        text.fontSize = finalSize;
    }
}
