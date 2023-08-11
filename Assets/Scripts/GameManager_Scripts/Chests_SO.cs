using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName ="New Chest", menuName = "Chest")]
public class Chests_SO : ScriptableObject
{

    public Chest[] chest;

    [Serializable]
    public struct Chest
    {
        public string name;
        public string description;
        public Sprite sprite;          // this will later go 
        public AssetReferenceAtlasedSprite spriteRef;
        public ChestType.Type type;
        public int unlockCostGem;
        public ChestDrop[] chestDrops;
    }

    [Serializable]
    public struct ChestDrop
    {
        public ChestDropType dropType;
        public float dropRatio;
    }

    public enum ChestDropType
    {
        product,
        ingredient,
        extraComponent,
        consumable,
        fleetCredit,
        researchScroll,
        ascensionShard,
        exclusiveProduct,
        exclusiveBlueprint,
        key,
    }
}
