using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static Recipes_SO;

public static class ImageManager
{

    private static readonly SpritesList_SO _spritesList_SO;

    static ImageManager()
    {
        _spritesList_SO = Resources.Load<SpritesList_SO>("Scriptable_Objects/GameMaterial/SpritesList_SO");
    }

    public static AssetReferenceT<Sprite> SelectSprite(string enumName_IN)
             => enumName_IN switch
             {
                 "CraftTimeReduction"
                 or "Duration"
                 or nameof(Recipes_SO.CraftUpgradeType.CraftTimeReduction)
                 or nameof(Recipes_SO.AscensionUpgradeType.CraftTimeReduction) => _spritesList_SO.HourGlassRef,

                 "IngredientReduction"
                 or nameof(Recipes_SO.CraftUpgradeType.IngredientReduction)
                 or nameof(Recipes_SO.AscensionUpgradeType.IngredientReduction) => _spritesList_SO.AttackIcon,

                 "ExtraComponentReduction"
                 or nameof(Recipes_SO.CraftUpgradeType.ExtraComponentReduction)
                 or nameof(Recipes_SO.AscensionUpgradeType.ExtraComponentReduction) => _spritesList_SO.DefenseIcon,

                 "ValueIncrease"
                 or nameof(AscensionTree_SO.AscensionTreeRewardType.GoldReward)
                 or nameof(AscensionTree_SO.AscensionTreeRewardType.SurchargeValueIncreasemodifier)
                 or nameof(Recipes_SO.CraftUpgradeType.ValueIncrease) => _spritesList_SO.CoinRef,

                 "QualityChanceIncrease"
                 or nameof(Recipes_SO.CraftUpgradeType.QualityChanceIncrease)
                 or nameof(Recipes_SO.AscensionUpgradeType.QualityChanceIncrease)
                 or nameof(AscensionTree_SO.AscensionTreeRewardType.QualityChanceIncrease) => _spritesList_SO.QualityIconRef,

                 "UnlockRecipe"
                 or nameof(Recipes_SO.CraftUpgradeType.UnlockRecipe) => _spritesList_SO.HpIcon,

                 "MultiCraftChance"
                 or nameof(Recipes_SO.AscensionUpgradeType.MultiCraftChance)
                 or nameof(AscensionTree_SO.AscensionTreeRewardType.MultiCraftChance) => _spritesList_SO.MultiCraftChanceIconRef,

                 "RequiredProductReduction"
                 or nameof(Recipes_SO.AscensionUpgradeType.RequiredProductReduction) => _spritesList_SO.DefenseIcon,

                 "StarIconYellow" => _spritesList_SO.StarIconYellow,

                 "StarIconRed"
                 or nameof(AscensionTree_SO.AscensionTreeRewardType.WorkerXPIncreaseModifier)
                 or nameof(AscensionTree_SO.AscensionTreeRewardType.CommanderBadge)
                 or nameof(AscensionTree_SO.AscensionTreeRewardType.ReduceSurchargeEnergyModifier) => _spritesList_SO.StarIconRed,

                 "MasteredIcon" => _spritesList_SO.MasteredIconRef,
                 "NotMasteredIcon" => _spritesList_SO.NotMasteredIconRef,
                 "PlusIcon" => _spritesList_SO.PlusSignRef,
                 "NotificationBG" => _spritesList_SO.NotificationIconBGRef,

                 "TokenIcon" => _spritesList_SO.TokenIcon,

                 "GemIcon"
                 or nameof(AscensionTree_SO.AscensionTreeRewardType.GemReward) => _spritesList_SO.GemIcon,

                 nameof(Recipes_SO.MealStatType.ATK) => _spritesList_SO.AttackIcon,
                 nameof(Recipes_SO.MealStatType.DEF) => _spritesList_SO.DefenseIcon,
                 nameof(Recipes_SO.MealStatType.HP) => _spritesList_SO.HpIcon,
                 nameof(Recipes_SO.MealStatType.CRIT) => _spritesList_SO.CriticalIcon,
                 nameof(Recipes_SO.MealStatType.EVA)=> _spritesList_SO.EvadeIcon,

                 nameof(WorkerType.Type.Krixath_The_Rotisseur) => _spritesList_SO.RotisseurIcon,
                 nameof(WorkerType.Type.Trilqeox_The_Entremetier) => _spritesList_SO.EntremetienrIcon,
                 nameof(WorkerType.Type.Qindrek_The_Poissonier) => _spritesList_SO.PoissonierIcon,
                 nameof(WorkerType.Type.Chophu_The_Patissier) => _spritesList_SO.PatisserIcon,
                 nameof(WorkerType.Type.Xaden_The_Fast_Fooder) => _spritesList_SO.FastFooderIcon,
                 nameof(WorkerType.Type.Trugmil_The_Legumier) => _spritesList_SO.LegumierIcon,
                 nameof(WorkerType.Type.Ekol_The_Potager) => _spritesList_SO.PotagerIcon,
                 nameof(WorkerType.Type.Master_Chef) => _spritesList_SO.MasterChefIcon,


                 nameof(HireCharacter_Panel) => _spritesList_SO.HirecharactersPanelBGImage,


                 "EnergyIcon" => _spritesList_SO.EnergyIcon,
                 _ => null,
             };


}
