using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeEqualityComparer : IEqualityComparer<ProductRecipe>
{
    public bool Equals(ProductRecipe productRecipe_x, ProductRecipe productRecipe_y)
    {
        if(productRecipe_x == null && productRecipe_y == null)
        {
            return true;
        }
        else if(productRecipe_x == null || productRecipe_y == null)
        {
            return false;
        }
        else if(productRecipe_x.GetName().Equals(productRecipe_y.GetName(), System.StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public int GetHashCode(ProductRecipe productRecipe_IN)
    {
        return productRecipe_IN.GetName().GetHashCode();
    }
}
