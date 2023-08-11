using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new IngredientGenerators", menuName = "IngredientGenerators")]
public class IngredientGenerators_SO : ScriptableObject
{
    public IngredientGenerators ingredientGenerators;


    [Serializable]
    public struct IngredientGenerators
    {
        public IngredientGeneratorBaseInfo[] baseInfo;
        public IngredientGeneratorSpecTiers[] tier;
    }


    [Serializable]
    public struct IngredientGeneratorBaseInfo
    {
        //public IngredientType.Type type;
        public string name;
        [Range(0, 1)] public int tier;
        public Sprite spite;
        public string description;

    }

    [Serializable]
    public struct IngredientGeneratorSpecTiers
    {
        public IngredientGeneratorSpecs[] specsByLevel;
    }

    [Serializable]
    public struct IngredientGeneratorSpecs
    {
        public int level;
        public int upgradeGoldCost;
        public int upgradeGemCost;
        public float productionRate;
        public float upgradeDuration;
    }
}
