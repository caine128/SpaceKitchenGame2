using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="new ShopCosmetics", menuName = "ShopCosmetics")]
public class ShopCosmetics_SO : ScriptableObject
{
    [SerializeField] public FlooringCosmetics[] flooringCosmeticsList;
    [SerializeField] public ExteriorCosmetcics[] exteriorCosmeticsList;

    [Serializable]
    public struct FlooringCosmetics
    {
        public string name;
        public int prestigeBonus;
        public int unlockGoldCost;
        public int unlockGemcost;
        public int requiredLevel;
    }

    [Serializable]
    public struct WallpaperCosmetics
    {
        public string name;
        public int prestigeBonus;
        public int unlockGoldCost;
        public int unlockGemcost;
        public int requiredLevel;
    }

    [Serializable]
    public struct FacadeCosmetics
    {
        public string name;
        public int prestigeBonus;
        public int unlockGoldCost;
        public int unlockGemcost;
        public int requiredLevel;
    }



    [Serializable]
    public struct ExteriorCosmetcics
    {
        public string name;
        public Vector2Int size;
        public int prestigeBonus;
        public int unlockGoldCost;
        public int unlockGemcost;
        public int requiredLevel;
    }

    [Serializable]
    public struct CarpetCosmetics
    {

    }
}
