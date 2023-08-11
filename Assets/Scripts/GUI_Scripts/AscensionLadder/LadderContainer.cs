using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;

public abstract class LadderContainer : Container<AscensionRewardState> 
{
    public override RectTransform rt => _rect;
    [SerializeField] protected RectTransform _rect;
    //protected bool _isUnlocked = false;
    //protected bool _isclaimed = false;
    //protected ContentDisplay_JustSpriteAndText ascensionStarAndLevel;
    public int initialOrder;


    public abstract void SetContainerColor();
}
