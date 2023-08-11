using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface ICraftable 
{
    ProductRecipe GetProductRecipe();
    //IEnumerable<Recipes_SO.MealStatBonus> GetStatBonuses();
    
    //Dictionary<Recipes_SO.MealStatType, List<(string bonusSource, int bonusAmount)>> StatIncreaseModifiers { get; }
    
    /*string GetTooltipTextForStatModifiers (Recipes_SO.MealStatType statType)
    {       
        if(StatIncreaseModifiers.TryGetValue(statType, out List<(string bonusSource, int bonusAmount)> statBonuses))
        {
            StringBuilder sb = new();
            var lastStatBonus = statBonuses.Last();

            foreach (var statBonuus in statBonuses)
            {
                sb.Append(statBonuus.bonusSource).Append(' ', 10).Append(statBonuus.bonusAmount);
                if (!statBonuus.Equals(lastStatBonus))
                    sb.AppendLine();
            }
            return sb.ToString();
        }
        return null;
    }*/

}
