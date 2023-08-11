using System.Collections.Generic;

public interface IStatable 
{
    IEnumerable<Recipes_SO.MealStatBonus> GetStatBonuses();
}
