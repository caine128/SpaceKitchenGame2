using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public interface IStatUpgradable : IStatable
{
    Dictionary<Recipes_SO.MealStatType, List<(string bonusSource, int bonusAmount)>> StatIncreaseModifiers { get; }

    ToolTipInfo GetTooltipTextForStatModifiers(Recipes_SO.MealStatType statType, string header = null, string footer = null)
        => new(bodytextAsColumns: GetTooltipTextForStatModifiers(statType), header: header, footer: footer);

    bool IsStatModified(Recipes_SO.MealStatType statType);

    private string[] GetTooltipTextForStatModifiers(Recipes_SO.MealStatType statType)
    {
        Lazy<StringBuilder> sb1 = new();
        Lazy<StringBuilder> sb2 = new();

        if (StatIncreaseModifiers.TryGetValue(statType, out List<(string bonusSource, int bonusAmount)> statBonuses))
        {
            foreach (var (statBonus, flags) in statBonuses.WithPositions())
            {
                sb1.Value.Append(statBonus.bonusSource);
                sb2.Value.Append("+ ").Append(statBonus.bonusAmount);

                if ((flags & FunctionalHelpers.PositionFlags.Last) != FunctionalHelpers.PositionFlags.Last)
                {
                    sb1.Value.AppendLine();
                    sb2.Value.AppendLine();
                }
            }
        }

        if(this is IQualitative qualitative && qualitative.GetQuality() != Quality.Level.Normal)
        {
            var quality = qualitative.GetQuality();
            var areEnhancementStatsAppended = sb1.IsValueCreated && sb2.IsValueCreated;
            if(areEnhancementStatsAppended) sb1.Value.AppendLine();
            if(areEnhancementStatsAppended) sb2.Value.AppendLine();
            sb1.Value.Append($"{quality} Quality Level");
            sb2.Value.Append($"x {Quality.StatModifierPerQuality(quality)}");
        }

        if(this is ICraftable craftable 
            && craftable.GetProductRecipe().recipeSpecs.mealStatBonuses.Where(msb=> msb.statType.Equals(statType)).Any())
        {
            sb1.Value.AppendLine();
            sb2.Value.AppendLine();
            sb1.Value.Append($"Base {statType}");
            sb2.Value.Append(craftable.GetProductRecipe().recipeSpecs.mealStatBonuses.Where(msb => msb.statType.Equals(statType)).First().statBonus);
        }

        return sb1.IsValueCreated || sb2.IsValueCreated
                    ? new string[] { sb1.ToString(), sb2.ToString() }
                    : null;
    }
}
