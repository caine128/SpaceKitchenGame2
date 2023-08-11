using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tab 
{
    public enum RecipeInfoTabs
    {
        None = 0,      // IS THIS NECESSARY ?? IF YES TAKE TO FIRST PLACE 
        RecipeTab = 1,
        CraftUpgradesTab = 3,
        AscensionTab = 4,
        StatsTab =5,
    }

    public enum GameItemInfoTabs
    {
        None = 0,
        InfoTab = 1,
        EnhancementsTab = 2,
    }

    public enum CharacterInfoTabs
    {
        None=0,
        InfoTab = 1,
        BlueprintsTab=2,
        StoryTab=3,
    }

    public enum ShopUpgradeInfoTabs
    {
        None=0,
        InfoTab =1,
        UpgradesTab=2,
    }
}
