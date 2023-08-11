using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUpgradeEqualityComparer_ByName : IEqualityComparer<ShopUpgrade>
{
    public bool Equals(ShopUpgrade shopUpgrade_x, ShopUpgrade shopUpgrade_y)
    {
        if (shopUpgrade_x == null && shopUpgrade_y == null)
        {
            return true;
        }
        else if (shopUpgrade_x == null || shopUpgrade_y == null)
        {
            return false;
        }
        else if (shopUpgrade_x.GetName().Equals(shopUpgrade_y.GetName(), System.StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public int GetHashCode(ShopUpgrade shopUpgrade_IN)
    {
        return shopUpgrade_IN.GetName().GetHashCode();
    }
}


public class ShopUpgradeEqualityComparer_ByItem : IEqualityComparer<ShopUpgrade>
{
    public bool Equals(ShopUpgrade shopUpgrade_x, ShopUpgrade shopUpgrade_y)
    {
        if (shopUpgrade_x == null && shopUpgrade_y == null)
        {
            return true;
        }
        else if (shopUpgrade_x == null || shopUpgrade_y == null)
        {
            return false;
        }
        else if (shopUpgrade_x.GetName().Equals(shopUpgrade_y.GetName(), System.StringComparison.OrdinalIgnoreCase) 
            && shopUpgrade_x.GetLevel() == shopUpgrade_y.GetLevel())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public int GetHashCode(ShopUpgrade shopUpgrade_IN)
    {
        int hashCode = 0;
        hashCode += shopUpgrade_IN.GetName().GetHashCode(System.StringComparison.OrdinalIgnoreCase);
        hashCode += shopUpgrade_IN.GetLevel().GetHashCode();

        return hashCode;
    }
}
