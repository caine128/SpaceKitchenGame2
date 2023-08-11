using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Character_SO : ScriptableObject
{
    public string characterName;
    [TextArea] public string characterDescription;
    public int requiredLevel;
    public AssetReferenceAtlasedSprite characterImageRef;
    public CharacterType.Type characterType;
}
