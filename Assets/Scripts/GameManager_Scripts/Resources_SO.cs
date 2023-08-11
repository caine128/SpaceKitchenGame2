using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName ="New Resources" , menuName ="Resources")]
public class Resources_SO : ScriptableObject
{
    [SerializeField] public ExtraComponent[] extraComponentsList;
    [SerializeField] public Ingredient[] ingredients;

    [Serializable]
    public struct ExtraComponent
    {
        public ExtraComponentsType.Type type;
        public string name;
        //public Sprite sprite;
        public AssetReferenceAtlasedSprite spriteRef;
        public string description;
    }

    [Serializable]
    public struct Ingredient
    {
        public IngredientType.Type type;
        public string name;
        //public Sprite sprite;
        public AssetReferenceAtlasedSprite spriteRef;
        public string description;
    }
}
