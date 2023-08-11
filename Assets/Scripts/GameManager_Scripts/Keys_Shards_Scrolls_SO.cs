using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName ="New Keys_Shards_Scrolls", menuName = "Keys_Shards_Scrolls")]
public class Keys_Shards_Scrolls_SO : ScriptableObject
{
    public ResearchScrollInfo researchScrollInfo;
    public AscensionShardInfo ascensionShardInfo;
    public KeyInfo[] keyInfos;

    [Serializable]
    public struct AscensionShardInfo
    {
        public string name;
        public string description;
        public Sprite sprite;      // this will later go 
        public AssetReferenceAtlasedSprite spriteRef;
    }

    [Serializable]
    public struct ResearchScrollInfo
    {
        public string name;
        public string description;
        public Sprite sprite;      // this will later go 
        public AssetReferenceAtlasedSprite spriteRef;
    }

    [Serializable]
    public struct KeyInfo
    {
        public string name;
        public string description;
        public Sprite sprite;            // this will later go 
        public AssetReferenceAtlasedSprite spriteRef;
        public ChestType.Type type;
    }
}
